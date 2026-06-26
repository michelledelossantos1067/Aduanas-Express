using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;

public class UsuarioServices : IUsuarioService
{
    private readonly IUsuarioRepositories _usuarioRepositories;
    private readonly IConductorRepositories _conductorRepositories;

    public UsuarioServices(
        IUsuarioRepositories usuarioRepositories,
        IConductorRepositories conductorRepositories)
    {
        _usuarioRepositories = usuarioRepositories;
        _conductorRepositories = conductorRepositories;
    }

    private async Task<bool> TieneHistorialAsync(int usuarioId)
        => await _conductorRepositories.ExisteConSupervisor(usuarioId);
    public async Task<List<UsuarioResponse?>> ObtenerTodos()
    {
        var usuario = await _usuarioRepositories.ObtenerTodos();
        if (usuario == null)
            throw new Exception("Error al obtener los usuario.");

        var responses = usuario.Select(c => c.ToResponse()).ToList();
        foreach (var r in responses)
            if (r != null)
                r.PuedeEliminarse = !await TieneHistorialAsync(r.Id);

        return responses;
    }

    public async Task<UsuarioResponse?> ObtenerPorId(int Id)
    {
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if (usuario == null)
            throw new Exception("Error al buscar el usuario.");

        var response = usuario.ToResponse();
        response.PuedeEliminarse = !await TieneHistorialAsync(Id);
        return response;
    }

    public async Task Crear(CreateUsuario createUsuario)
    {
        var usuarios = new Usuario
        {
            Nombre = createUsuario.Nombre,
            Apellido = createUsuario.Apellido,
            Email = createUsuario.Email,
            Password = createUsuario.Password,
            RolId = createUsuario.RolId,
            RequiereCambioPassword = true  // El usuario debe cambiar su contraseña al primer inicio
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

        usuario.Nombre = updateUsuario.Nombre;
        usuario.Apellido = updateUsuario.Apellido;
        usuario.Email = updateUsuario.Email;
        usuario.RolId = updateUsuario.RolId;


        if (!string.IsNullOrWhiteSpace(updateUsuario.Password))
        {
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(updateUsuario.Password);
        }

        await _usuarioRepositories.Actualizar(Id, usuario);
    }
    public async Task<List<UsuarioResponse>> ObtenerPorRol(string rol)
    {
        var usuarios = await _usuarioRepositories.ObtenerTodos();
        return usuarios
            .Where(u => u.Rol != null && u.Rol.Nombre == rol && u.IsActive)
            .Select(u => u.ToResponse())
            .ToList();
    }

    public async Task Eliminar(int Id)
    {
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if (usuario == null)
            throw new Exception("Error al eliminar el usuario.");

        if (await TieneHistorialAsync(Id))
            throw new Exception("Este usuario es supervisor de uno o más conductores. No se puede eliminar; desactívalo en su lugar.");

        await _usuarioRepositories.Eliminar(Id);
    }

    public async Task Desactivar(int Id)
    {
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if (usuario == null)
            throw new Exception("Error al desactivar el usuario.");

        usuario.IsActive = false;
        await _usuarioRepositories.Actualizar(Id, usuario);
    }

    public async Task Activar(int Id)
    {
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if (usuario == null)
            throw new Exception("Error al activar el usuario.");

        usuario.IsActive = true;
        await _usuarioRepositories.Actualizar(Id, usuario);
    }
}
