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

public class AuthServices : IAuthService {

    private readonly IUsuarioRepositories _usuarioRepositories;
    private readonly IConfiguration _configuration;

    public AuthServices(IUsuarioRepositories usuarioRepositories, IConfiguration configuration)
    {
        _usuarioRepositories = usuarioRepositories;
        _configuration = configuration;
    }

    public async Task<AuthResponseDTOs?> Login(AuthDTOs authDTOs)
    {
        if (authDTOs.Email == null)
        {
            return null;
        }
        ;
        var usuario = await _usuarioRepositories.ObtenerPorEmail(authDTOs.Email);
        if (usuario == null)
        {
            return null;
        }
        ;
        if (!BCrypt.Net.BCrypt.Verify(authDTOs.Password!, usuario.Password))
        {
            return null;
        }
        ;
        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.Nombre),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
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
            Rol = usuario.Rol.Nombre,
            Nombre = usuario.Nombre
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
            throw new Exception("La contraseña no existe.");
        }
        ;
        var hashPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.PasswordNueva);
        usuarioExistente.Password = hashPassword;
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
