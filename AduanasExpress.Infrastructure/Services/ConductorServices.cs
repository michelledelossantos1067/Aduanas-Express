using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;

public class ConductorServices : IConductorService
{
    private readonly IConductorRepositories _conductorRepositories;
    private readonly IAsignacionRepository _asignacionRepo;   // 👈 nuevo

    public ConductorServices(
        IConductorRepositories conductorRepositories,
        IAsignacionRepository asignacionRepo)
    {
        _conductorRepositories = conductorRepositories;
        _asignacionRepo = asignacionRepo;
    }

    private async Task<bool> TieneHistorialAsync(int conductorId)
        => await _asignacionRepo.ExisteParaConductor(conductorId);

    public async Task<List<ConductorReponseDTOs?>> ObtenerTodos()
    {
        var conductor = await _conductorRepositories.ObtenerTodos();
        if (conductor == null)
            throw new Exception("Error al obtener los conductores.");

        var responses = conductor.Select(c => c.ToResponse()).ToList();
        foreach (var r in responses)
            if (r != null)
                r.PuedeEliminarse = !await TieneHistorialAsync(r.Id);

        return responses;
    }

    public async Task<ConductorReponseDTOs?> ObtenerPorId(int Id)
    {
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if (conductor == null)
            throw new Exception("Error al buscar el conductor.");

        var response = conductor.ToResponse();
        response.PuedeEliminarse = !await TieneHistorialAsync(Id);
        return response;
    }

    public async Task Crear(CreateConductorDTOs createConductorDTOs)
    {
        var conductor = new Conductor
        {
            Nombre = createConductorDTOs.Nombre,
            Apellido = createConductorDTOs.Apellido,
            Cedula = createConductorDTOs.Cedula,
            NumeroLicencia = createConductorDTOs.NumeroLicencia,
            FechaVencLicencia = createConductorDTOs.FechaVencLicencia.Value,
            TipoLicencia = createConductorDTOs.TipoLicencia,
            Telefono = createConductorDTOs.Telefono,
            Direccion = createConductorDTOs.Direccion,
            SupervisorId = createConductorDTOs.SupervisorId,
            Estado = createConductorDTOs.Estado
        };
        await _conductorRepositories.Crear(conductor);
    }

    public async Task Actualizar(int Id, UpdateConductorDTOs updateConductorDTOs)
    {
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if (conductor == null)
            throw new Exception("Error al actualizar el conductor.");

        conductor.Nombre = updateConductorDTOs.Nombre;
        conductor.Apellido = updateConductorDTOs.Apellido;
        conductor.Cedula = updateConductorDTOs.Cedula;
        conductor.NumeroLicencia = updateConductorDTOs.NumeroLicencia;
        conductor.FechaVencLicencia = updateConductorDTOs.FechaVencLicencia
                                      ?? conductor.FechaVencLicencia;
        conductor.TipoLicencia = updateConductorDTOs.TipoLicencia;
        conductor.Telefono = updateConductorDTOs.Telefono;
        conductor.Direccion = updateConductorDTOs.Direccion;
        conductor.SupervisorId = updateConductorDTOs.SupervisorId;
        conductor.Estado = updateConductorDTOs.Estado;

        await _conductorRepositories.Actualizar(Id, conductor);
    }

    public async Task Eliminar(int Id)
    {
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if (conductor == null)
            throw new Exception("Error al eliminar el conductor.");

        if (await TieneHistorialAsync(Id))
            throw new Exception("Este conductor tiene viajes/asignaciones registradas. No se puede eliminar; desactívalo en su lugar.");

        await _conductorRepositories.Eliminar(Id);
    }

    public async Task Desactivar(int Id)
    {
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if (conductor == null)
            throw new Exception("Error al desactivar el conductor.");

        conductor.IsActive = false;
        await _conductorRepositories.Actualizar(Id, conductor);
    }

    public async Task Activar(int Id)
    {
        var conductor = await _conductorRepositories.ObtenerPorId(Id);
        if (conductor == null)
            throw new Exception("Error al activar el conductor.");

        conductor.IsActive = true;
        await _conductorRepositories.Actualizar(Id, conductor);
    }

    public async Task<List<ConductorReponseDTOs>> ObtenerDisponiblesEnFecha(DateTime fecha)
    {
        var conductores = await _conductorRepositories.ObtenerDisponiblesEnFecha(fecha);
        return conductores.Select(c => c.ToResponse()).ToList();
    }
}
