using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Repositories;

public class ConductorRepositories : IConductorRepositories
{
    private readonly AppDbContext _context;

    public ConductorRepositories(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Conductor?>> ObtenerTodos()
    {
        return await _context.Conductores.ToListAsync();
    }

    public async Task<Conductor?> ObtenerPorId(int Id)
    {
        return await _context.Conductores.FindAsync(Id);
    }

    public async Task Crear(Conductor conductor)
    {
        _context.AddAsync(conductor);
        await _context.SaveChangesAsync();
    }

    public async Task Actualizar(int Id, Conductor conductor)
    {
        var conductores = await _context.Conductores.FindAsync(Id);
        conductores.Id = conductor.Id;
        conductores.Nombre = conductor.Nombre;
        conductores.Apellido = conductor.Apellido;
        conductores.Cedula = conductor.Cedula;
        conductores.NumeroLicencia = conductor.NumeroLicencia;
        conductores.TipoLicencia = conductor.TipoLicencia;
        conductores.Telefono = conductor.Telefono;
        conductores.Direccion = conductor.Direccion;
        conductores.SupervisorId = conductor.SupervisorId;
        conductores.Estado = conductor.Estado;
        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(int Id)
    {
        var conductores = await _context.Conductores.FindAsync(Id);
        if (conductores == null)
            throw new Exception("No se puede eliminar este conductor.");

        _context.Remove(conductores);
        await _context.SaveChangesAsync();
    }

    // Excluye conductores que ya tienen una asignación en la fecha indicada
    public async Task<List<Conductor>> ObtenerDisponiblesEnFecha(DateTime fecha)
    {
        return await _context.Conductores
            .Where(c => !_context.Asignaciones
                .Any(a => a.ConductorId == c.Id &&
                          a.Solicitud.FechaViaje.HasValue &&
                          a.Solicitud.FechaViaje.Value.Date == fecha.Date))
            .ToListAsync();
    }
}
