using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;
public class VehiculoServices : IVehiculoService{
    private readonly IVehiculoRepositories _vehiculoRepositories;

    public VehiculoServices(IVehiculoRepositories vehiculoRepositories){
        _vehiculoRepositories = vehiculoRepositories;
    }

    public async Task<List<VehiculoResponseDTOs?>> ObtenerTodos(){
        var vehiculo = await _vehiculoRepositories.ObtenerTodos();
        if(vehiculo == null){
            throw new Exception("Error al obtener los vehiculo.");
        };
        return vehiculo.Select(c => new VehiculoResponseDTOs{
            Marca = c.Marca,
            Modelo = c.Modelo,
            Año = c.Año,
            Matricula = c.Matricula,
            Color = c.Color,
            Tipo = c.Tipo,
            Capacidad = c.Capacidad,
            Estado = c.Estado,
            Kilometraje = c.Kilometraje,
            FechaUltimoMant = c.FechaUltimoMant
        }).ToList();
    }
    public async Task<VehiculoResponseDTOs?> ObtenerPorId(int Id){
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if(vehiculo == null){
            throw new Exception("Error al buscar el vehiculo.");
        };
        return new VehiculoResponseDTOs{
            Marca = vehiculo.Marca,
            Modelo = vehiculo.Modelo,
            Año = vehiculo.Año,
            Matricula = vehiculo.Matricula,
            Color = vehiculo.Color,
            Tipo = vehiculo.Tipo,
            Capacidad = vehiculo.Capacidad,
            Estado = vehiculo.Estado,
            Kilometraje = vehiculo.Kilometraje,
            FechaUltimoMant = vehiculo.FechaUltimoMant
        };
    }
    public async Task Crear(CreateVehiculoDTOs createVehiculoDTOs){
        var vehiculo = new Vehiculo{
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
    public async Task Actualizar(int Id,UpdateVehiculoDTOs updateVehiculoDTOs){
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if(vehiculo == null){
            throw new Exception("Error al actualizar el vehiculo.");
        };
            vehiculo.Id = updateVehiculoDTOs.Id;
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
        await _vehiculoRepositories.Actualizar(Id,vehiculo);
    }
    public async Task Eliminar(int Id){
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(Id);
        if(vehiculo == null){
            throw new Exception("Error al eliminar el vehiculo.");
        };
        await _vehiculoRepositories.Eliminar(Id);
    }
}