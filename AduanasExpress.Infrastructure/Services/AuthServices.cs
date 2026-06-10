using Microsoft.Extensions.Configuration;

using AduanasExpress.Application.DTOs.Login;
using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Domain.Entitis;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
namespace AduanasExpress.Infrastructure.Services;


public class AuthServices : IAuthService {
    private readonly IUsuarioRepositories _usuarioRepositories;
    private readonly IConfiguration _configuration;

    public AuthServices(IUsuarioRepositories usuarioRepositories,IConfiguration configuration){
        _usuarioRepositories = usuarioRepositories;
        _configuration = configuration;
    }

    public async Task<AuthResponseDTOs?> Login(AuthDTOs authDTOs){
        if(authDTOs.Email == null){
            return null;
        };
        var usuario = await _usuarioRepositories.ObtenerPorEmail(authDTOs.Email);
        if(usuario == null){
            return null;
        };
        if (!BCrypt.Net.BCrypt.Verify(authDTOs.Password!, usuario.Password))
        {
            return null;
        };
        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
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
            Token = tokenString,
            Rol = usuario.Rol.ToString(),
            Nombre = usuario.Nombre
        };
    }
}