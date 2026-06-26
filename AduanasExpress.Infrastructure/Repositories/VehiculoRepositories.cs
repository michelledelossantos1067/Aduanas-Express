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
        await _context.Vehiculos.AddAsync(vehiculo);
        await _context.SaveChangesAsync();
    }
    public async Task Actualizar(int Id, Vehiculo vehiculo)
    {
        var vehiculos = await _context.Vehiculos.FindAsync(Id);
        if (vehiculos == null)
            throw new Exception("No se encontró el vehículo a actualizar.");
        vehiculos.Marca = vehiculo.Marca;
        vehiculos.Modelo = vehiculo.Modelo;
        vehiculos.Año = vehiculo.Año;
        vehiculos.Matricula = vehiculo.Matricula;
        vehiculos.Color = vehiculo.Color;
        vehiculos.Tipo = vehiculo.Tipo;
        vehiculos.Capacidad = vehiculo.Capacidad;
        vehiculos.Estado = vehiculo.Estado;
        vehiculos.Kilometraje = vehiculo.Kilometraje;
        vehiculos.FechaUltimoMant = vehiculo.FechaUltimoMant;
        vehiculos.UbicacionActual = vehiculo.UbicacionActual;
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
    public async Task<List<Vehiculo>> ObtenerDisponiblesEnFecha(DateTime fecha)
    {
        var vehiculosOcupados = await _context.Asignaciones
            .Where(a => a.FechaAsignacion.HasValue
                     && a.FechaAsignacion.Value.Date == fecha.Date
                     && a.Estado != EstadoAsignacion.Cancelada
                     && a.Estado != EstadoAsignacion.Finalizada)
            .Select(a => a.VehiculoId)
            .ToListAsync();

        return await _context.Vehiculos
            .Where(v => v.Estado == EstadosVehiculo.Disponible
                     && v.IsActive == true
                     && !vehiculosOcupados.Contains(v.Id))
            .ToListAsync();
    }
}
