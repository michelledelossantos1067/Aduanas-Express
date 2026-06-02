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
            nombre = c.nombre,
            apellido = c.apellido,
            email = c.email,
            telefono = c.telefono,
            direccion = c.direccion,
            rol = c.rol
        }).ToList();
    }
    public async Task<UsuarioResponse?> ObtenerPorId(int id){
        var usuario = await _usuarioRepositories.ObtenerPorId(id);
        if(usuario == null){
            throw new Exception("Error al buscar el usuario.");
        };
        return new UsuarioResponse{
            nombre = usuario.nombre,
            apellido = usuario.apellido,
            email = usuario.email,
            telefono = usuario.telefono,
            direccion = usuario.direccion,
            rol = usuario.rol
        };
    }
    public async Task Crear(CreateUsuario createUsuario){
        var usuarios = new Usuario{
            nombre = createUsuario.nombre,
            apellido = createUsuario.apellido,
            email = createUsuario.email,
            telefono = createUsuario.telefono,
            direccion = createUsuario.direccion,
            rol = createUsuario.rol
        };
        await _usuarioRepositories.Crear(usuarios);
    }
    public async Task Actualizar(int id,UpdateUsuario updateUsuario){
        var usuario = await _usuarioRepositories.ObtenerPorId(id);
        if(usuario == null){
            throw new Exception("Error al actualizar el usuario.");
        };
        usuario.nombre = updateUsuario.nombre;
        usuario.apellido = updateUsuario.apellido;
        usuario.email = updateUsuario.email;
        usuario.telefono = updateUsuario.telefono;
        usuario.direccion = updateUsuario.direccion;
        usuario.rol = updateUsuario.rol;
        
        await _usuarioRepositories.Actualizar(id,usuario);
    }
    public async Task Eliminar(int id){
        var usuario = await _usuarioRepositories.ObtenerPorId(id);
        if(usuario == null){
            throw new Exception("Error al eliminar el usuario.");
        };
        await _usuarioRepositories.Eliminar(id);
    }
}