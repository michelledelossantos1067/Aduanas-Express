using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Repositories;

public class VehiculoRepositories : IVehiculoRepositories
{
    private readonly AppDbContext _context;

    public VehiculoRepositories(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vehiculo?>> ObtenerTodos()
    {
        return await _context.Vehiculos.ToListAsync();
    }

    public async Task<Vehiculo?> ObtenerPorId(int Id)
    {
        return await _context.Vehiculos.FindAsync(Id);
    }

    public async Task Crear(Vehiculo vehiculo)
    {
        _context.AddAsync(vehiculo);
        await _context.SaveChangesAsync();
    }

    public async Task Actualizar(int Id, Vehiculo vehiculo)
    {
        var vehiculos = await _context.Vehiculos.FindAsync(Id);
        vehiculos.Id = vehiculo.Id;
        vehiculos.Marca = vehiculo.Marca;
        vehiculos.Modelo = vehiculo.Modelo;
        vehiculos.Año = vehiculo.Año;
        vehiculos.Matricula = vehiculo.Matricula;
        vehiculos.Color = vehiculo.Color;
        vehiculos.Tipo = vehiculo.Tipo;
        vehiculos.Estado = vehiculo.Estado;
        vehiculos.Kilometraje = vehiculo.Kilometraje;
        vehiculos.FechaUltimoMant = vehiculo.FechaUltimoMant;
        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(int Id)
    {
        var vehiculo = await _context.Vehiculos.FindAsync(Id);
        if (vehiculo == null)
            throw new Exception("No se puede eliminar este vehículo.");

        _context.Remove(vehiculo);
        await _context.SaveChangesAsync();
    }

    // Excluye vehículos que ya tienen una asignación en la fecha indicada
    public async Task<List<Vehiculo>> ObtenerDisponiblesEnFecha(DateTime fecha)
    {
        return await _context.Vehiculos
            .Where(v => !_context.Asignaciones
                .Any(a => a.VehiculoId == v.Id &&
                          a.Solicitud.FechaViaje.HasValue &&
                          a.Solicitud.FechaViaje.Value.Date == fecha.Date))
            .ToListAsync();
    }
}
