using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;
public class ConductorServices : IConductorService{
    private readonly IConductorRepositories _conductorRepositories;

    public ConductorServices(IConductorRepositories conductorRepositories){
        _conductorRepositories = conductorRepositories;
    }

    public async Task<List<ConductorReponseDTOs?>> ObtenerTodos(){
        var conductor = await _conductorRepositories.ObtenerTodos();
        if(conductor == null){
            throw new Exception("Error al obtener los mantenimiento.");
        };
        return conductor.Select(c => new ConductorReponseDTOs{
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Cedula = c.Cedula,
            NumeroLicencia = c.NumeroLicencia,
            TipoLicencia = c.TipoLicencia,
            Telefono = c.Telefono,
            Direccion = c.Direccion,
            SupervisorId = c.SupervisorId,
            Estado = c.Estado
        }).ToList();
    }
    public async Task<ConductorReponseDTOs?> ObtenerPorId(int Id){
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if(conductor == null){
            throw new Exception("Error al buscar el conductor.");
        };
        return new ConductorReponseDTOs{
            Id = conductor.Id,
            Nombre = conductor.Nombre,
            Apellido = conductor.Apellido,
            Cedula = conductor.Cedula,
            NumeroLicencia = conductor.NumeroLicencia,
            TipoLicencia = conductor.TipoLicencia,
            Telefono = conductor.Telefono,
            Direccion = conductor.Direccion,
            SupervisorId = conductor.SupervisorId,
            Estado = conductor.Estado
        };
    }
    public async Task Crear(CreateConductorDTOs createConductorDTOs){
        var conductor = new Conductor
        {
            Nombre = createConductorDTOs.Nombre,
            Apellido = createConductorDTOs.Apellido,
            Cedula = createConductorDTOs.Cedula,
            NumeroLicencia = createConductorDTOs.NumeroLicencia,
            TipoLicencia = createConductorDTOs.TipoLicencia,
            Telefono = createConductorDTOs.Telefono,
            Direccion = createConductorDTOs.Direccion,
            SupervisorId = createConductorDTOs.SupervisorId,
            Estado = createConductorDTOs.Estado
        };
        await _conductorRepositories.Crear(conductor);
    }
    public async Task Actualizar(int Id,UpdateConductorDTOs updateConductorDTOs){
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if(conductor == null){
            throw new Exception("Error al actualizar el conductor.");
        }
            conductor.Id = updateConductorDTOs.Id;
            conductor.Nombre = updateConductorDTOs.Nombre;
            conductor.Apellido = updateConductorDTOs.Apellido;
            conductor.Cedula = updateConductorDTOs.Cedula;
            conductor.NumeroLicencia = updateConductorDTOs.NumeroLicencia;
            conductor.TipoLicencia = updateConductorDTOs.TipoLicencia;
            conductor.Telefono = updateConductorDTOs.Telefono;
            conductor.Direccion = updateConductorDTOs.Direccion;
            conductor.SupervisorId = updateConductorDTOs.SupervisorId;
            conductor.Estado = updateConductorDTOs.Estado;
        await _conductorRepositories.Actualizar(Id,conductor);
    }
    public async Task Eliminar(int Id){
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if(conductor == null){
            throw new Exception("Error al eliminar el conductor.");
        };
        await _conductorRepositories.Eliminar(Id);
    }
}