using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AduanasExpress.Application.DTOs.Login;
using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Domain.Entitis;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace AduanasExpress.Infrastructure.Services;

public class AuthServices : IAuthService
{
    private readonly IUsuarioRepositories _usuarioRepositories;
    private readonly IConfiguration _configuration;
    private static Dictionary<string, (string otp, DateTime expiry)> _otpStorage = new();

    public AuthServices(IUsuarioRepositories usuarioRepositories, IConfiguration configuration)
    {
        _usuarioRepositories = usuarioRepositories;
        _configuration = configuration;
    }

    public async Task<bool> GenerateOtp(GenerateOtpDTO request)
    {
        var usuario = await _usuarioRepositories.ObtenerPorEmail(request.Email);
        if (usuario == null)
            throw new Exception("El email no existe.");

        var otp = new Random().Next(100000, 999999).ToString();
        var expiryTime = DateTime.Now.AddMinutes(10);

        if (_otpStorage.ContainsKey(request.Email))
            _otpStorage[request.Email] = (otp, expiryTime);
        else
            _otpStorage.Add(request.Email, (otp, expiryTime));

        await SendOtpEmail(request.Email, otp);
        
        return true;
    }

    public async Task<bool> ValidateOtp(ValidateOtpDTO request)
    {
        if (!_otpStorage.ContainsKey(request.Email))
            throw new Exception("OTP no solicitado o expirado.");

        var (storedOtp, expiry) = _otpStorage[request.Email];

        if (DateTime.Now > expiry)
        {
            _otpStorage.Remove(request.Email);
            throw new Exception("El OTP ha expirado.");
        }

        if (storedOtp != request.Otp)
            throw new Exception("El código OTP es incorrecto.");

        return true;
    }

    public async Task ResetPasswordWithOtp(ResetPasswordWithOtpDTO request)
    {
        await ValidateOtp(new ValidateOtpDTO { Email = request.Email, Otp = request.Otp });

        var usuario = await _usuarioRepositories.ObtenerPorEmail(request.Email);
        if (usuario == null)
            throw new Exception("El email no existe.");

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.NuevaPassword);
        usuario.Password = hashPassword;

        await _usuarioRepositories.Actualizar(usuario.Id, usuario);

        _otpStorage.Remove(request.Email);
    }

    private async Task SendOtpEmail(string email, string otp)
    {
        using (var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
        {
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(
                _configuration["Email:Username"],
                _configuration["Email:Password"]
            );

            var mailMessage = new System.Net.Mail.MailMessage(
                from: _configuration["Email:FromAddress"],
                to: email,
                subject: "Tu código de recuperación - Aduanas Express",
                body: $@"
                    <h1>Código de recuperación</h1>
                    <p>Tu código OTP es: <strong>{otp}</strong></p>
                    <p>Este código es válido por <strong>10 minutos</strong></p>
                    <p>Si no solicitaste esto, ignora este email.</p>
                "
            )
            {
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
    public async Task<AuthResponseDTOs?> Login(AuthDTOs authDTOs)
    {
        if (authDTOs.Email == null)
        {
            return null;
        }

        var usuario = await _usuarioRepositories.ObtenerPorEmail(authDTOs.Email);

        if (usuario == null)
        {
            return null;
        }

        if (!usuario.IsActive)
        {
            throw new Exception("Este usuario ha sido desactivado.");
        }

        if (!BCrypt.Net.BCrypt.Verify(authDTOs.Password!, usuario.Password))
        {
            return null;
        }

        var claims = new[]{
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.Name, usuario.Nombre),
        new Claim(ClaimTypes.Role, usuario.Rol.Nombre),
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDTOs
        {
            Id = usuario.Id,
            Token = tokenString,
            RolId = usuario.Rol.Nombre,
            Nombre = usuario.Nombre,
            RequiereCambioPassword = usuario.RequiereCambioPassword
        };
    }
    public async Task Logout()
    {
        await Task.CompletedTask;
    }
    public async Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO)
    {
        var usuarioExistente = await _usuarioRepositories.ObtenerPorEmail(resetPasswordDTO.Email);
        if (usuarioExistente == null)
        {
            throw new Exception("El email no existe.");
        }
        var passwordTemporal = Guid.NewGuid().ToString().Substring(0, 8);
        var hashPassword = BCrypt.Net.BCrypt.HashPassword(passwordTemporal);
        usuarioExistente.Password = hashPassword;

        await _usuarioRepositories.Actualizar(usuarioExistente.Id, usuarioExistente);
        return passwordTemporal;
    }
    public async Task ChangePassword(ChangePasswordDTO changePasswordDTO)
    {
        var usuarioExistente = await _usuarioRepositories.ObtenerPorEmail(changePasswordDTO.Email);
        if (usuarioExistente == null)
        {
            throw new Exception("El email no existe.");
        }
        if (!BCrypt.Net.BCrypt.Verify(changePasswordDTO.PasswordActual, usuarioExistente.Password))
        {
            throw new Exception("La contraseña actual es incorrecta.");
        }

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.PasswordNueva);
        usuarioExistente.Password = hashPassword;

        // Si era el primer inicio de sesión, marcar que ya cambió la contraseña
        if (usuarioExistente.RequiereCambioPassword)
            usuarioExistente.RequiereCambioPassword = false;

        await _usuarioRepositories.Actualizar(usuarioExistente.Id, usuarioExistente);
    }
    public async Task Register(RegisterDTO registerDTO)
    {
        var usuarioExistente = await _usuarioRepositories.ObtenerPorEmail(registerDTO.Email);
        if (usuarioExistente != null)
            throw new Exception("El email ya está registrado.");
        if (registerDTO.Email == null)
            throw new Exception("El email existe.");

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);
        var usuario = new Usuario
        {
            Nombre = registerDTO.Nombre,
            Apellido = registerDTO.Apellido,
            Email = registerDTO.Email,
            Password = hashPassword,
            RolId = registerDTO.RolId
        };
        await _usuarioRepositories.Crear(usuario);
    }

}
