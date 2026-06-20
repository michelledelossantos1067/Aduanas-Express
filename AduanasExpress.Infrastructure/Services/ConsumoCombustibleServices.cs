using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Application.Mappings;

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
        return consumo.Select(c => c.ToResponse()).ToList();
    }
    public async Task<ConsumoCombustibleReponseDTOs?> ObtenerPorId(int Id){
        var consumo = await _consumoCombustibleRepositories.ObtenerPorId(Id);
        if(consumo == null){
            throw new Exception("Error al buscar el consumo de combustible.");
        };
        return consumo.ToResponse();
    }
    public async Task Crear(CreateConsumoCombustibleDTOs createConsumoCombustibleDTOs){
        var consumo = new ConsumoCombustible{
            Galones = createConsumoCombustibleDTOs.Galones,
            CostoPorGalon = createConsumoCombustibleDTOs.CostoPorGalon,
            CostoTotal = createConsumoCombustibleDTOs.CostoTotal,
            VehiculoId = createConsumoCombustibleDTOs.VehiculoId,

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
