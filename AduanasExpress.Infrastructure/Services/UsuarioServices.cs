using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;

namespace AduanasExpress.Infrastructure.Services;
public class UsuarioServices : IUsuarioService{
    private readonly IUsuarioRepositories _usuarioRepositories;

    public UsuarioServices(IUsuarioRepositories usuarioRepositories){
        _usuarioRepositories = usuarioRepositories;
    }

    public async Task<List<UsuarioResponse?>> ObtenerTodos(){
        var usuario = await _usuarioRepositories.ObtenerTodos();
        if(usuario == null){
            throw new Exception("Error al obtener los usuario.");
        };
        return usuario.Select(c => new UsuarioResponse{
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Email = c.Email,
            Rol = c.Rol
        }).ToList();
    }
    public async Task<UsuarioResponse?> ObtenerPorId(int Id){
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if(usuario == null){
            throw new Exception("Error al buscar el usuario.");
        };
        return new UsuarioResponse{
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol
        };
    }
    public async Task Crear(CreateUsuario createUsuario){
        var usuarios = new Usuario{
            Nombre = createUsuario.Nombre,
            Apellido = createUsuario.Apellido,
            Email = createUsuario.Email,
            Rol = createUsuario.Rol
        };
        await _usuarioRepositories.Crear(usuarios);
    }
    public async Task Actualizar(int Id,UpdateUsuario updateUsuario){
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if(usuario == null){
            throw new Exception("Error al actualizar el usuario.");
        };
        usuario.Nombre = updateUsuario.Nombre;
        usuario.Apellido = updateUsuario.Apellido;
        usuario.Email = updateUsuario.Email;
        usuario.Rol = updateUsuario.Rol;
        
        await _usuarioRepositories.Actualizar(Id,usuario);
    }
    public async Task Eliminar(int Id){
        var usuario = await _usuarioRepositories.ObtenerPorId(Id);
        if(usuario == null){
            throw new Exception("Error al eliminar el usuario.");
        };
        await _usuarioRepositories.Eliminar(Id);
    }
}