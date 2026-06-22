using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Repositories;

public class RolRepository : IRolRepository
{
    private readonly AppDbContext _context;
    public RolRepository(AppDbContext context) => _context = context;

    public async Task<List<Rol>> ObtenerTodos() =>
        await _context.Roles.Include(r => r.Permisos).ToListAsync();

    public async Task<Rol?> ObtenerPorId(int id) =>
        await _context.Roles.Include(r => r.Permisos).FirstOrDefaultAsync(r => r.Id == id);

    public async Task Crear(Rol rol)
    {
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarPermisos(int rolId, List<RolPermiso> permisos)
    {
        var existentes = _context.RolPermisos.Where(p => p.RolId == rolId);
        _context.RolPermisos.RemoveRange(existentes);
        await _context.RolPermisos.AddRangeAsync(permisos);
        await _context.SaveChangesAsync();
    }
}
