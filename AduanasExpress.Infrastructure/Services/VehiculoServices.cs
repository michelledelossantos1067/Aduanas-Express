using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;

public class VehiculoServices : IVehiculoService
{
    private readonly IVehiculoRepositories _vehiculoRepositories;

    public VehiculoServices(IVehiculoRepositories vehiculoRepositories)
    {
        _vehiculoRepositories = vehiculoRepositories;
    }

    public async Task<List<VehiculoResponseDTOs?>> ObtenerTodos()
    {
        var vehiculo = await _vehiculoRepositories.ObtenerTodos();
        if (vehiculo == null)
            throw new Exception("Error al obtener los vehiculos.");

        return vehiculo.Select(c => c.ToResponse()).ToList();
    }

    public async Task<VehiculoResponseDTOs?> ObtenerPorId(int Id)
    {
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if (vehiculo == null)
            throw new Exception("Error al buscar el vehiculo.");

        return vehiculo.ToResponse();
    }

    public async Task Crear(CreateVehiculoDTOs createVehiculoDTOs)
    {
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

        await _vehiculoRepositories.Eliminar(Id);
    }

    public async Task<List<VehiculoResponseDTOs>> ObtenerDisponiblesEnFecha(DateTime fecha)
    {
        var vehiculos = await _vehiculoRepositories.ObtenerDisponiblesEnFecha(fecha);
        return vehiculos.Select(v => v.ToResponse()).ToList();
    }
}
