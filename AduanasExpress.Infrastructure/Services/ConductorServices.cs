using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Application.Mappings;


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
        return conductor.Select(c => c.ToResponse()).ToList();
    }
    public async Task<ConductorReponseDTOs?> ObtenerPorId(int Id){
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if(conductor == null){
            throw new Exception("Error al buscar el conductor.");
        };
        return conductor.ToResponse();
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