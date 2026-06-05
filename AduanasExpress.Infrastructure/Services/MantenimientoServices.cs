using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;

namespace AduanasExpress.Infrastructure.Services;
public class MantenimientoServices : IMantenimientoService{
    private readonly IMantenimientoRepositories _mantenimientoRepositories;

    public MantenimientoServices(IMantenimientoRepositories mantenimientoRepositories){
        _mantenimientoRepositories = mantenimientoRepositories;
    }

    public async Task<List<MantenimientoResponseDTOs?>> ObtenerTodos(){
        var mantenimiento = await _mantenimientoRepositories.ObtenerTodos();
        if(mantenimiento == null){
            throw new Exception("Error al obtener los mantenimiento.");
        };
        return mantenimiento.Select(c => new MantenimientoResponseDTOs{
            Fecha = c.Fecha,
            TipoMantenimiento = c.TipoMantenimiento,
            Descripcion = c.Descripcion,
            Costo = c.Costo,
            Taller = c.Taller,
            ProximoMantenimiento = c.ProximoMantenimiento,
            VehiculoId = c.VehiculoId
        }).ToList();
    }
    public async Task<MantenimientoResponseDTOs?> ObtenerPorId(int Id){
        var mantenimiento = await _mantenimientoRepositories.ObtenerPorId(Id);
        if(mantenimiento == null){
            throw new Exception("Error al buscar el mantenimiento.");
        };
        return new MantenimientoResponseDTOs{
            Id = mantenimiento.Id,
            Fecha = mantenimiento.Fecha,
            TipoMantenimiento = mantenimiento.TipoMantenimiento,
            Descripcion = mantenimiento.Descripcion,
            Costo = mantenimiento.Costo,
            Taller = mantenimiento.Taller,
            ProximoMantenimiento = mantenimiento.ProximoMantenimiento,
            VehiculoId = mantenimiento.VehiculoId
        };
    }
    public async Task Crear(CreateMantenimientoDTOs createMantenimientoDTOs){
        var mantenimiento = new Mantenimiento{
            TipoMantenimiento = createMantenimientoDTOs.TipoMantenimiento,
            Descripcion = createMantenimientoDTOs.Descripcion,
            Costo = createMantenimientoDTOs.Costo,
            Taller = createMantenimientoDTOs.Taller,
            ProximoMantenimiento = createMantenimientoDTOs.ProximoMantenimiento,
            VehiculoId = createMantenimientoDTOs.VehiculoId
        };
        await _mantenimientoRepositories.Crear(mantenimiento);
    }
    public async Task Actualizar(int Id,UpdateMantenimientoDTOs updateMantenimientoDTOs){
        var mantenimiento = await _mantenimientoRepositories.ObtenerPorId(Id);
        if(mantenimiento == null){
            throw new Exception("Error al actualizar el mantenimiento.");
        };
            mantenimiento.Id = updateMantenimientoDTOs.Id;
            mantenimiento.Fecha = updateMantenimientoDTOs.Fecha;
            mantenimiento.TipoMantenimiento = updateMantenimientoDTOs.TipoMantenimiento;
            mantenimiento.Descripcion = updateMantenimientoDTOs.Descripcion;
            mantenimiento.Costo = updateMantenimientoDTOs.Costo;
            mantenimiento.Taller = updateMantenimientoDTOs.Taller;
            mantenimiento.ProximoMantenimiento = updateMantenimientoDTOs.ProximoMantenimiento;
            mantenimiento.VehiculoId = updateMantenimientoDTOs.VehiculoId;
        await _mantenimientoRepositories.Actualizar(Id,mantenimiento);
    }
    public async Task Eliminar(int Id){
        var mantenimiento = await _mantenimientoRepositories.ObtenerPorId(Id);
        if(mantenimiento == null){
            throw new Exception("Error al eliminar el mantenimiento.");
        };
        await _mantenimientoRepositories.Eliminar(Id);
    }
}