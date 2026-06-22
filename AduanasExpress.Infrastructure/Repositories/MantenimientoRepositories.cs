using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace AduanasExpress.Infrastructure.Repositories;
public class MantenimientoRepositories : IMantenimientoRepositories{
    private readonly AppDbContext _context;
    public MantenimientoRepositories(AppDbContext context){
        _context = context;
    }
    public async Task<List<Mantenimiento?>> ObtenerTodos(){
        return await _context.Mantenimientos
            .Include(m => m.Vehiculo)
            .OrderByDescending(m => m.FechaProgramada)
            .ToListAsync();
    }
    public async Task<Mantenimiento?> ObtenerPorId(int Id){
        return await _context.Mantenimientos
            .Include(m => m.Vehiculo)
            .FirstOrDefaultAsync(m => m.Id == Id);
    }
    public async Task Crear(Mantenimiento mantenimiento){
        await _context.Mantenimientos.AddAsync(mantenimiento);
        await _context.SaveChangesAsync();
    }
    public async Task Actualizar(int Id,Mantenimiento mantenimiento){
        var mantenimientos = await _context.Mantenimientos.FindAsync(Id);
        if(mantenimientos == null){
            throw new Exception("No se encontró el mantenimiento a actualizar.");
        };
        mantenimientos.Tipo = mantenimiento.Tipo;
        mantenimientos.Descripcion = mantenimiento.Descripcion;
        mantenimientos.Estado = mantenimiento.Estado;
        mantenimientos.FechaProgramada = mantenimiento.FechaProgramada;
        mantenimientos.FechaRealizada = mantenimiento.FechaRealizada;
        mantenimientos.Kilometraje = mantenimiento.Kilometraje;
        mantenimientos.Costo = mantenimiento.Costo;
        mantenimientos.Taller = mantenimiento.Taller;
        mantenimientos.Responsable = mantenimiento.Responsable;
        mantenimientos.Observaciones = mantenimiento.Observaciones;
        mantenimientos.VehiculoId = mantenimiento.VehiculoId;
        await _context.SaveChangesAsync();
    }
    public async Task Eliminar(int Id){
        var mantenimientos = await _context.Mantenimientos.FindAsync(Id);
        if(mantenimientos == null){
            throw new Exception("No se puede eliminar este mantenimiento.");
        };
        _context.Remove(mantenimientos);
        await _context.SaveChangesAsync();
    }
}
