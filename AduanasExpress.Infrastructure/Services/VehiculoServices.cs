using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;
namespace AduanasExpress.Infrastructure.Services;

public class VehiculoServices : IVehiculoService
{
    private readonly IVehiculoRepositories _vehiculoRepositories;
    private readonly IAsignacionRepository _asignacionRepo;          // 👈 nuevo
    private readonly IConsumoCombustibleRepositories _consumoRepo;   // 👈 nuevo
    private readonly IMantenimientoRepositories _mantenimientoRepo; // 👈 nuevo

    public VehiculoServices(
        IVehiculoRepositories vehiculoRepositories,
        IAsignacionRepository asignacionRepo,
        IConsumoCombustibleRepositories consumoRepo,
        IMantenimientoRepositories mantenimientoRepo)
    {
        _vehiculoRepositories = vehiculoRepositories;
        _asignacionRepo = asignacionRepo;
        _consumoRepo = consumoRepo;
        _mantenimientoRepo = mantenimientoRepo;
    }

    private async Task<bool> TieneHistorialAsync(int vehiculoId)
    {
        return await _asignacionRepo.ExisteParaVehiculo(vehiculoId)
            || await _consumoRepo.ExisteParaVehiculo(vehiculoId)
            || await _mantenimientoRepo.ExisteParaVehiculo(vehiculoId);
    }

    public async Task<List<VehiculoResponseDTOs?>> ObtenerTodos()
    {
        var vehiculo = await _vehiculoRepositories.ObtenerTodos();
        if (vehiculo == null)
            throw new Exception("Error al obtener los vehiculos.");

        var responses = vehiculo.Select(c => c.ToResponse()).ToList();
        foreach (var r in responses)
            if (r != null)
                r.PuedeEliminarse = !await TieneHistorialAsync(r.Id);

        return responses;
    }

    public async Task<VehiculoResponseDTOs?> ObtenerPorId(int Id)
    {
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if (vehiculo == null)
            throw new Exception("Error al buscar el vehiculo.");

        var response = vehiculo.ToResponse();
        response.PuedeEliminarse = !await TieneHistorialAsync(Id);
        return response;
    }
    public async Task Crear(CreateVehiculoDTOs createVehiculoDTOs)
    {
        if (createVehiculoDTOs.Estado == EstadosVehiculo.EnMantenimiento)
            throw new Exception("Un vehículo no puede registrarse directamente en estado 'En Mantenimiento'. Créalo como Disponible y luego registra un mantenimiento desde ese módulo.");

        var vehiculo = new Vehiculo
        {
            Marca = createVehiculoDTOs.Marca,
            Modelo = createVehiculoDTOs.Modelo,
            Año = createVehiculoDTOs.Año,
            Matricula = createVehiculoDTOs.Matricula,
            Color = createVehiculoDTOs.Color,
            Tipo = createVehiculoDTOs.Tipo,
            Capacidad = createVehiculoDTOs.Capacidad,
            Estado = createVehiculoDTOs.Estado,
            Kilometraje = createVehiculoDTOs.Kilometraje,
            FechaUltimoMant = createVehiculoDTOs.FechaUltimoMant
        };
        await _vehiculoRepositories.Crear(vehiculo);
    }
    public async Task Actualizar(int Id, UpdateVehiculoDTOs updateVehiculoDTOs)
    {
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if (vehiculo == null)
            throw new Exception("Error al actualizar el vehiculo.");
        if (updateVehiculoDTOs.Estado == EstadosVehiculo.EnMantenimiento && vehiculo.Estado != EstadosVehiculo.EnMantenimiento)
            throw new Exception("El estado 'En Mantenimiento' no se puede asignar manualmente. Crea un mantenimiento desde el módulo de Mantenimiento para vincularlo automáticamente.");

        if (vehiculo.Estado == EstadosVehiculo.EnMantenimiento && updateVehiculoDTOs.Estado != EstadosVehiculo.EnMantenimiento)
            throw new Exception("Este vehículo tiene un mantenimiento activo. Complétalo o cancélalo desde el módulo de Mantenimiento para liberarlo.");

        vehiculo.Marca = updateVehiculoDTOs.Marca;
        vehiculo.Modelo = updateVehiculoDTOs.Modelo;
        vehiculo.Año = updateVehiculoDTOs.Año;
        vehiculo.Matricula = updateVehiculoDTOs.Matricula;
        vehiculo.Color = updateVehiculoDTOs.Color;
        vehiculo.Tipo = updateVehiculoDTOs.Tipo;
        vehiculo.Capacidad = updateVehiculoDTOs.Capacidad;
        vehiculo.Estado = updateVehiculoDTOs.Estado;
        vehiculo.Kilometraje = updateVehiculoDTOs.Kilometraje;
        vehiculo.FechaUltimoMant = updateVehiculoDTOs.FechaUltimoMant;
        await _vehiculoRepositories.Actualizar(Id, vehiculo);
    }
    public async Task Eliminar(int Id)
    {
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if (vehiculo == null)
            throw new Exception("Error al eliminar el vehiculo.");

        if (await TieneHistorialAsync(Id))
            throw new Exception("Este vehículo tiene historial registrado (viajes, mantenimientos o consumo). No se puede eliminar; desactívalo en su lugar.");

        await _vehiculoRepositories.Eliminar(Id);
    }

    public async Task Desactivar(int Id)
    {
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if (vehiculo == null)
            throw new Exception("Error al desactivar el vehiculo.");

        vehiculo.IsActive = false;
        await _vehiculoRepositories.Actualizar(Id, vehiculo);
    }

    public async Task Activar(int Id)
    {
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if (vehiculo == null)
            throw new Exception("Error al activar el vehiculo.");

        vehiculo.IsActive = true;
        await _vehiculoRepositories.Actualizar(Id, vehiculo);
    }
    public async Task<List<VehiculoResponseDTOs>> ObtenerDisponiblesEnFecha(DateTime fecha)
    {
        var vehiculos = await _vehiculoRepositories.ObtenerDisponiblesEnFecha(fecha);
        return vehiculos.Select(v => v.ToResponse()).ToList();
    }
}
