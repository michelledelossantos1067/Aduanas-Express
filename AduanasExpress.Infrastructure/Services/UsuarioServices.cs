using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Application.Mappings;

namespace AduanasExpress.Infrastructure.Services;

public class UsuarioServices : IUsuarioService
{
    private readonly IUsuarioRepositories _usuarioRepositories;

    public UsuarioServices(IUsuarioRepositories usuarioRepositories)
    {
        _usuarioRepositories = usuarioRepositories;
    }

    public async Task<List<UsuarioResponse?>> ObtenerTodos()
    {
        var usuario = await _usuarioRepositories.ObtenerTodos();
        if (usuario == null)
        {
            throw new Exception("Error al obtener los usuario.");
        }
        ;
        return usuario.Select(c => c.ToResponse()).ToList();
    }

    public async Task<UsuarioResponse?> ObtenerPorId(int Id)
    {
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if (usuario == null)
        {
            throw new Exception("Error al buscar el usuario.");
        }
        ;
        return usuario.ToResponse();
    }

    public async Task Crear(CreateUsuario createUsuario)
    {
        var usuarios = new Usuario
        {
            Nombre = createUsuario.Nombre,
            Apellido = createUsuario.Apellido,
            Email = createUsuario.Email,
            Password = createUsuario.Password,
            Rol = createUsuario.Rol
        };
        if (usuarios.Password != null)
        {
            var PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuarios.Password);
            usuarios.Password = PasswordHash;
        }
        await _usuarioRepositories.Crear(usuarios);
    }

    public async Task Actualizar(int Id, UpdateUsuario updateUsuario)
    {
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if (usuario == null)
        {
            throw new Exception("Error al actualizar el usuario.");
        }
        ;
        usuario.Nombre = updateUsuario.Nombre;
        usuario.Apellido = updateUsuario.Apellido;
        usuario.Email = updateUsuario.Email;
        usuario.Password = updateUsuario.Password;
        usuario.Rol = updateUsuario.Rol;
        if (!string.IsNullOrWhiteSpace(updateUsuario.Password))
        {
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(updateUsuario.Password);
        }
        await _usuarioRepositories.Actualizar(Id, usuario);
    }

    public async Task Eliminar(int Id)
    {
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if (usuario == null)
        {
            throw new Exception("Error al eliminar el usuario.");
        }
        ;
        await _usuarioRepositories.Eliminar(Id);
    }
}