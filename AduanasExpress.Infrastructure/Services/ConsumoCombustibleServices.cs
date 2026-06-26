using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;

public class ConsumoCombustibleServices : IConsumoCombustibleService {
    private readonly IConsumoCombustibleRepositories _consumoCombustibleRepositories;
    private readonly IVehiculoRepositories _vehiculoRepositories;

    public ConsumoCombustibleServices(
        IConsumoCombustibleRepositories consumoCombustibleRepositories,
        IVehiculoRepositories vehiculoRepositories)
    {
        _consumoCombustibleRepositories = consumoCombustibleRepositories;
        _vehiculoRepositories = vehiculoRepositories;
    }

    public async Task<List<ConsumoCombustibleReponseDTOs?>> ObtenerTodos() {
        var consumo = await _consumoCombustibleRepositories.ObtenerTodos();
        if (consumo == null) {
            throw new Exception("Error al obtener los consumos de combustible.");
        }
        return consumo.Select(c => c.ToResponse()).ToList();
    }

    public async Task<ConsumoCombustibleReponseDTOs?> ObtenerPorId(int Id) {
        var consumo = await _consumoCombustibleRepositories.ObtenerPorId(Id);
        if (consumo == null) {
            throw new Exception("Error al buscar el consumo de combustible.");
        }
        return consumo.ToResponse();
    }

    public async Task Crear(CreateConsumoCombustibleDTOs createConsumoCombustibleDTOs) {
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(createConsumoCombustibleDTOs.VehiculoId);
        if (vehiculo == null) {
            throw new Exception("El vehículo no existe.");
        }

        if (createConsumoCombustibleDTOs.Galones > vehiculo.Capacidad) {
            throw new Exception($"El nivel de combustible no puede exceder la capacidad del tanque ({vehiculo.Capacidad} galones).");
        }

        if (createConsumoCombustibleDTOs.Galones < 0) {
            throw new Exception("El nivel de combustible no puede ser negativo.");
        }

        var consumo = new ConsumoCombustible {
            Galones = createConsumoCombustibleDTOs.Galones,
            CostoPorGalon = createConsumoCombustibleDTOs.CostoPorGalon,
            CostoTotal = createConsumoCombustibleDTOs.CostoTotal,
            VehiculoId = createConsumoCombustibleDTOs.VehiculoId,
            Fecha = DateTime.UtcNow,
        };

        await _consumoCombustibleRepositories.Crear(consumo);

        vehiculo.UltimoCombustible = createConsumoCombustibleDTOs.Galones;
        vehiculo.FechaUltimoCombustible = DateTime.UtcNow;
        await _vehiculoRepositories.Actualizar(vehiculo.Id, vehiculo);
    }

    public async Task Actualizar(int Id, UpdateConsumoCombustibleDTOs updateConsumoCombustibleDTOs) {
        var consumo = await _consumoCombustibleRepositories.ObtenerPorId(Id);
        if (consumo == null) {
            throw new Exception("Error al actualizar el consumo de combustible.");
        }

        var vehiculo = await _vehiculoRepositories.ObtenerPorId(updateConsumoCombustibleDTOs.VehiculoId);
        if (vehiculo == null) {
            throw new Exception("El vehículo no existe.");
        }

        if (updateConsumoCombustibleDTOs.Galones > vehiculo.Capacidad) {
            throw new Exception($"El nivel de combustible no puede exceder la capacidad del tanque ({vehiculo.Capacidad} galones).");
        }

        if (updateConsumoCombustibleDTOs.Galones < 0) {
            throw new Exception("El nivel de combustible no puede ser negativo.");
        }

        consumo.Id = updateConsumoCombustibleDTOs.Id;
        consumo.Galones = updateConsumoCombustibleDTOs.Galones;
        consumo.CostoPorGalon = updateConsumoCombustibleDTOs.CostoPorGalon;
        consumo.CostoTotal = updateConsumoCombustibleDTOs.CostoTotal;
        consumo.VehiculoId = updateConsumoCombustibleDTOs.VehiculoId;

        await _consumoCombustibleRepositories.Actualizar(Id, consumo);

        vehiculo.UltimoCombustible = updateConsumoCombustibleDTOs.Galones;
        vehiculo.FechaUltimoCombustible = DateTime.UtcNow;
        await _vehiculoRepositories.Actualizar(vehiculo.Id, vehiculo);
    }

    public async Task Eliminar(int Id) {
        var consumo = await _consumoCombustibleRepositories.ObtenerPorId(Id);
        if (consumo == null) {
            throw new Exception("Error al eliminar el consumo de combustible.");
        }
        await _consumoCombustibleRepositories.Eliminar(Id);
    }
}