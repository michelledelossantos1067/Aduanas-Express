using System.Security.Claims;
using System.Text;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Repositories;

public class ConsumoCombustibleRepositories : IConsumoCombustibleRepositories
{
    private readonly AppDbContext _context;

    public ConsumoCombustibleRepositories(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConsumoCombustible?>> ObtenerTodos()
    {
        return await _context.ConsumoCombustibles.ToListAsync();
    }
    public async Task<ConsumoCombustible?> ObtenerPorId(int Id)
    {
        return await _context.ConsumoCombustibles.FindAsync(Id);
    }
    public async Task Crear(ConsumoCombustible consumoCombustible)
    {
        _context.AddAsync(consumoCombustible);
        await _context.SaveChangesAsync();
    }
    public async Task Actualizar(int Id, ConsumoCombustible consumoCombustible)
    {
        var consumoCombustibles = await _context.ConsumoCombustibles.FindAsync(Id);
        consumoCombustibles.Id = consumoCombustible.Id;
        consumoCombustibles.Fecha = consumoCombustible.Fecha;
        consumoCombustibles.Galones = consumoCombustible.Galones;
        consumoCombustibles.CostoPorGalon = consumoCombustible.CostoPorGalon;
        consumoCombustibles.CostoTotal = consumoCombustible.CostoTotal;
        consumoCombustibles.VehiculoId = consumoCombustible.VehiculoId;

        await _context.SaveChangesAsync();
    }
    public async Task<bool> ExisteParaVehiculo(int vehiculoId)
    {
        return await _context.ConsumoCombustibles
                             .AnyAsync(c => c.VehiculoId == vehiculoId);
    }
    public async Task<ConsumoCombustible?> ObtenerUltimoPorVehiculo(int vehiculoId)
    {
        return await _context.ConsumoCombustibles
            .Where(c => c.VehiculoId == vehiculoId)
            .OrderByDescending(c => c.Fecha)
            .FirstOrDefaultAsync();
    }
    public async Task Eliminar(int Id)
    {
        var consumoCombustibles = await _context.ConsumoCombustibles.FindAsync(Id);
        if (consumoCombustibles == null)
        {
            throw new Exception("No se puede eliminar este Consumo de Combustibles.");
        }
        ;
        _context.Remove(consumoCombustibles);
        await _context.SaveChangesAsync();
    }

}
