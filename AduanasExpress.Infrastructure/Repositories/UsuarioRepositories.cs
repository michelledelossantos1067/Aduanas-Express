using AduanasExpress.Infrastructure.Data;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Application.interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
namespace AduanasExpress.Infrastructure.Repositories;

public class UsuarioRepositories : IUsuarioRepositories{
    private readonly AppDbContext _context;

    public UsuarioRepositories(AppDbContext context){
        _context = context;
    }

    public async Task<List<Usuario?>> ObtenerTodos(){
        return await _context.Usuarios.ToListAsync();
    }
    public async Task<Usuario?> ObtenerPorId(int id){
        return await _context.Usuarios.FindAsync(id);
    }
    public async Task Crear(Usuario usuario){
        _context.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }
    public async Task Actualizar(int id,Usuario usuario){
        var usuarios = await _context.Usuarios.FindAsync(id);
        usuarios.nombre = usuario.nombre;
        usuarios.apellido = usuario.apellido;
        usuarios.email = usuario.email;
        usuarios.telefono = usuario.telefono;
        usuarios.direccion = usuario.direccion;
        usuarios.rol = usuario.rol;
        await _context.SaveChangesAsync();
    }
    public async Task Eliminar(int id){
        var usuario = await _context.Usuarios.FindAsync(id);
        if(usuario == null){
            throw new Exception("No se puede eliminar este usuario.");
        };
        _context.Remove(usuario);
        await _context.SaveChangesAsync();
    }
}