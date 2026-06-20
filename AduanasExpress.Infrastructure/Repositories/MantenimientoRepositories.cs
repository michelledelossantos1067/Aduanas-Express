using System.Security.Claims;
using System.Text;
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
        return await _context.Mantenimientos.ToListAsync();
    }
    public async Task<Mantenimiento?> ObtenerPorId(int Id){
        return await _context.Mantenimientos.FindAsync(Id);
    }
    public async Task Crear(Mantenimiento mantenimiento){
        _context.AddAsync(mantenimiento);
        await _context.SaveChangesAsync();
    }
    public async Task Actualizar(int Id,Mantenimiento mantenimiento){
        var mantenimientos = await _context.Mantenimientos.FindAsync(Id);
        mantenimientos.Id = mantenimiento.Id;
        mantenimientos.Fecha = mantenimiento.Fecha;
        mantenimientos.TipoMantenimiento = mantenimiento.TipoMantenimiento;
        mantenimientos.Descripcion = mantenimiento.Descripcion;
        mantenimientos.Costo = mantenimiento.Costo;
        mantenimientos.Taller = mantenimiento.Taller;
        mantenimientos.ProximoMantenimiento = mantenimiento.ProximoMantenimiento;
        mantenimientos.VehiculoId = mantenimiento.VehiculoId;
        await _context.SaveChangesAsync();
    }
    public async Task Eliminar(int Id){
        var mantenimientos = await _context.Mantenimientos.FindAsync(Id);
        if(mantenimientos == null){
            throw new Exception("No se puede eliminar este vehículo.");
        };
        _context.Remove(mantenimientos);
        await _context.SaveChangesAsync();
    }
}
