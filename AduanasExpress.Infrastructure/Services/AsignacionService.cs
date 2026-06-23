using AduanasExpress.Application.DTOs.Asignacion;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;

public class AsignacionService : IAsignacionService
{
    private readonly IAsignacionRepository _asignacionRepository;
    private readonly ISolicitudTransporteRepositories _solicitudRepository;
    private readonly IVehiculoRepositories _vehiculoRepository;
    private readonly IConductorRepositories _conductorRepository;

    public AsignacionService(
        IAsignacionRepository asignacionRepository,
        ISolicitudTransporteRepositories solicitudRepository,
        IVehiculoRepositories vehiculoRepository,
        IConductorRepositories conductorRepository)
    {
        _asignacionRepository = asignacionRepository;
        _solicitudRepository = solicitudRepository;
        _vehiculoRepository = vehiculoRepository;
        _conductorRepository = conductorRepository;
    }

    public async Task<List<AsignacionResponseDTO?>> ObtenerTodos()
    {
        var asignacion = await _asignacionRepository.ObtenerTodos();
        if (asignacion == null)
            throw new Exception("Error al obtener las asignaciones.");

        return asignacion.Select(c => c.ToResponse()).ToList();
    }

    public async Task<AsignacionResponseDTO?> ObtenerPorId(int Id)
    {
        var asignacion = await _asignacionRepository.ObtenerPorId(Id);
        if (asignacion == null)
            throw new Exception("Error al buscar la asignacion.");

        return asignacion.ToResponse();
    }

    public async Task Crear(CreateAsignacionDTO createAsignacionDTO)
    {
        var vehiculo = await _vehiculoRepository.ObtenerPorId(createAsignacionDTO.VehiculoId);
        if (vehiculo == null)
            throw new Exception("Vehículo no encontrado.");
        if (vehiculo.Estado != EstadosVehiculo.Disponible)
            throw new Exception($"El vehículo no está disponible (estado actual: {vehiculo.Estado}).");

        var conductor = await _conductorRepository.ObtenerPorId(createAsignacionDTO.ConductorId);
        if (conductor == null)
            throw new Exception("Conductor no encontrado.");
        if (conductor.Estado != EstadosConductor.Disponible)
            throw new Exception($"El conductor no está disponible (estado actual: {conductor.Estado}).");
        if (vehiculo == null || !vehiculo.IsActive)
            throw new Exception("Vehículo no encontrado o está desactivado.");

        var asignacion = new Asignacion
        {
            SolicitudId = createAsignacionDTO.SolicitudId,
            VehiculoId = createAsignacionDTO.VehiculoId,
            ConductorId = createAsignacionDTO.ConductorId,
            FechaAsignacion = createAsignacionDTO.FechaAsignacion,
            AsignadoPorId = createAsignacionDTO.AsignadoPorId,
            Estado = EstadoAsignacion.Pendiente,
        };
        await _asignacionRepository.Crear(asignacion);

        var solicitud = await _solicitudRepository.ObtenerPorId(createAsignacionDTO.SolicitudId);
        if (solicitud != null)
        {
            solicitud.Estado = EstadosSolicitudes.Aprobada;
            await _solicitudRepository.Actualizar(solicitud.Id, solicitud);
        }
        var ahora = DateTime.UtcNow;
        var fechaViaje = solicitud?.FechaViaje;

        if (fechaViaje.HasValue && ahora >= fechaViaje.Value)
        {
            vehiculo.Estado = EstadosVehiculo.EnViaje;
            conductor.Estado = EstadosConductor.EnViaje;
        }
    }

    public async Task Finalizar(int id)
    {
        var asignacion = await _asignacionRepository.ObtenerPorId(id);
        if (asignacion == null)
            throw new Exception("Asignación no encontrada.");
        if (asignacion.Estado == EstadoAsignacion.Finalizada)
            throw new Exception("La asignación ya fue finalizada.");
        if (asignacion.Estado == EstadoAsignacion.Cancelada)
            throw new Exception("No se puede finalizar una asignación cancelada.");

        asignacion.Estado = EstadoAsignacion.Finalizada;
        asignacion.FechaFinalizacion = DateTime.UtcNow;
        await _asignacionRepository.Actualizar(asignacion.Id, asignacion);

        var solicitud = await _solicitudRepository.ObtenerPorId(asignacion.SolicitudId);
        if (solicitud != null)
        {
            solicitud.Estado = EstadosSolicitudes.Finalizada;
            await _solicitudRepository.Actualizar(solicitud.Id, solicitud);
        }

        var vehiculo = await _vehiculoRepository.ObtenerPorId(asignacion.VehiculoId);
        if (vehiculo != null)
        {
            vehiculo.Estado = EstadosVehiculo.Disponible;
            await _vehiculoRepository.Actualizar(vehiculo.Id, vehiculo);
        }

        var conductor = await _conductorRepository.ObtenerPorId(asignacion.ConductorId);
        if (conductor != null)
        {
            conductor.Estado = EstadosConductor.Disponible;
            await _conductorRepository.Actualizar(conductor.Id, conductor);
        }
    }

    // Cancela la asignación y restaura la disponibilidad del vehículo y el conductor
    public async Task Cancelar(int id, string motivo, int usuarioId)
    {
        var asignacion = await _asignacionRepository.ObtenerPorId(id);
        if (asignacion == null)
            throw new Exception("Asignación no encontrada.");
        if (asignacion.Estado == EstadoAsignacion.Finalizada)
            throw new Exception("No se puede cancelar una asignación ya finalizada.");
        if (asignacion.Estado == EstadoAsignacion.Cancelada)
            throw new Exception("La asignación ya fue cancelada.");

        asignacion.Estado = EstadoAsignacion.Cancelada;
        await _asignacionRepository.Actualizar(asignacion.Id, asignacion);

        var solicitud = await _solicitudRepository.ObtenerPorId(asignacion.SolicitudId);
        if (solicitud != null)
        {
            solicitud.Estado = EstadosSolicitudes.Cancelada;
            await _solicitudRepository.Actualizar(solicitud.Id, solicitud);
        }

        var vehiculo = await _vehiculoRepository.ObtenerPorId(asignacion.VehiculoId);
        if (vehiculo != null)
        {
            vehiculo.Estado = EstadosVehiculo.Disponible;
            await _vehiculoRepository.Actualizar(vehiculo.Id, vehiculo);
        }

        var conductor = await _conductorRepository.ObtenerPorId(asignacion.ConductorId);
        if (conductor != null)
        {
            conductor.Estado = EstadosConductor.Disponible;
            await _conductorRepository.Actualizar(conductor.Id, conductor);
        }
    }

    // Devuelve vehículos y conductores sin asignación en la fecha del viaje de la solicitud
    public async Task<DisponiblesResponseDTO> ObtenerDisponibles(int solicitudId)
    {
        var solicitud = await _solicitudRepository.ObtenerPorId(solicitudId);
        if (solicitud == null)
            throw new Exception("Solicitud no encontrada.");

        if (!solicitud.FechaViaje.HasValue)
            throw new Exception("La solicitud no tiene fecha de viaje.");

        var fecha = solicitud.FechaViaje.Value;

        var vehiculos = await _vehiculoRepository.ObtenerDisponiblesEnFecha(fecha);
        var conductores = await _conductorRepository.ObtenerDisponiblesEnFecha(fecha);

        return new DisponiblesResponseDTO
        {
            Vehiculos = vehiculos.Select(v => v.ToResponse()).ToList(),
            Conductores = conductores.Select(c => c.ToResponse()).ToList()
        };
    }
}
