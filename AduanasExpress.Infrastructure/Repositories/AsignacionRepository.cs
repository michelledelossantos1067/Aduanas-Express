using System.Security.Claims;
using System.Text;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Repositories;

public class AsignacionRepository : IAsignacionRepository
{
    private readonly AppDbContext _context;

    public AsignacionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asignacion?>> ObtenerTodos()
    {
        return await _context.Asignaciones
            .Include(a => a.Conductor)
            .Include(a => a.Vehiculo)
            .Include(a => a.Solicitud)
            .ToListAsync();
    }
    public async Task<Asignacion?> ObtenerPorId(int Id)
    {
        return await _context.Asignaciones.FindAsync(Id);
    }
    public async Task Crear(Asignacion asignacion)
    {
        _context.AddAsync(asignacion);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> ExisteParaVehiculo(int vehiculoId)
    {
        return await _context.Asignaciones
        .AnyAsync(c => c.VehiculoId == vehiculoId);
    }
    public async Task<bool> ExisteParaConductor(int conductorId)
    {
        return await _context.Asignaciones
        .AnyAsync(c => c.ConductorId == conductorId);
    }
    public async Task Actualizar(int id, Asignacion asignacion)
    {
        _context.Asignaciones.Update(asignacion);
        await _context.SaveChangesAsync();
    }
    public async Task<List<Asignacion>> ObtenerPorEstado(EstadoAsignacion estado)
    {
        return await _context.Asignaciones
            .Where(a => a.Estado == estado)
            .ToListAsync();
    }
    public async Task<Asignacion?> ObtenerPorSolicitudId(int solicitudId)
    {
        return await _context.Asignaciones
            .Include(a => a.Conductor)
            .Include(a => a.Vehiculo)
            .Include(a => a.Solicitud)
            .FirstOrDefaultAsync(a => a.SolicitudId == solicitudId);
    }
}
