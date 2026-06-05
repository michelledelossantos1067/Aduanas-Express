using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;
public class ConsumoCombustibleServices : IConsumoCombustibleService{
    private readonly IConsumoCombustibleRepositories _consumoCombustibleRepositories;

    public ConsumoCombustibleServices(IConsumoCombustibleRepositories consumoCombustibleRepositories){
        _consumoCombustibleRepositories = consumoCombustibleRepositories;
    }

    public async Task<List<ConsumoCombustibleReponseDTOs?>> ObtenerTodos(){
        var consumo = await _consumoCombustibleRepositories.ObtenerTodos();
        if(consumo == null){
            throw new Exception("Error al obtener los mantenimiento.");
        };
        return consumo.Select(c => new ConsumoCombustibleReponseDTOs{
            Fecha = c.Fecha,
            Galones = c.Galones,
            CostoPorGalon = c.CostoPorGalon,
            CostoTotal = c.CostoTotal,
            VehiculoId = c.VehiculoId,
            // SolicitudId = c.SolicitudId
        }).ToList();
    }
    public async Task<ConsumoCombustibleReponseDTOs?> ObtenerPorId(int Id){
        var consumo = await _consumoCombustibleRepositories.ObtenerPorId(Id);
        if(consumo == null){
            throw new Exception("Error al buscar el consumo de combustible.");
        };
        return new ConsumoCombustibleReponseDTOs{
            Id = consumo.Id,
            Fecha = consumo.Fecha,
            Galones = consumo.Galones,
            CostoPorGalon = consumo.CostoPorGalon,
            CostoTotal = consumo.CostoTotal,
            VehiculoId = consumo.VehiculoId,
            // SolicitudId = consumo.SolicitudId
        };
    }
    public async Task Crear(CreateConsumoCombustibleDTOs createConsumoCombustibleDTOs){
        var consumo = new ConsumoCombustible{
            Galones = createConsumoCombustibleDTOs.Galones,
            CostoPorGalon = createConsumoCombustibleDTOs.CostoPorGalon,
            CostoTotal = createConsumoCombustibleDTOs.CostoTotal,
            VehiculoId = createConsumoCombustibleDTOs.VehiculoId,
            // SolicitudId = createConsumoCombustibleDTOs.SolicitudId
        };
        await _consumoCombustibleRepositories.Crear(consumo);
    }
    public async Task Actualizar(int Id,UpdateConsumoCombustibleDTOs updateConsumoCombustibleDTOs){
        var consumo = await _consumoCombustibleRepositories.ObtenerPorId(Id);
        if(consumo == null){
            throw new Exception("Error al actualizar el consumo de combustible.");
        }

            consumo.Id = updateConsumoCombustibleDTOs.Id;
            consumo.Galones = updateConsumoCombustibleDTOs.Galones;
            consumo.CostoPorGalon = updateConsumoCombustibleDTOs.CostoPorGalon;
            consumo.CostoTotal = updateConsumoCombustibleDTOs.CostoTotal;
            consumo.VehiculoId = updateConsumoCombustibleDTOs.VehiculoId;
            // consumo.SolicitudId = updateConsumoCombustibleDTOs.SolicitudId;
        await _consumoCombustibleRepositories.Actualizar(Id,consumo);
    }
    public async Task Eliminar(int Id){
        var consumo = await _consumoCombustibleRepositories.ObtenerPorId(Id);
        if(consumo == null){
            throw new Exception("Error al eliminar el consumo de combustible.");
        };
        await _consumoCombustibleRepositories.Eliminar(Id);
    }
}