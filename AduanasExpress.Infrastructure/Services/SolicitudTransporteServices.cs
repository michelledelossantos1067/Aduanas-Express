using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.DTOs.SolicitudTransporte;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;

public class SolicitudTransporteServices : ISolicitudTransporteService
{
    private readonly ISolicitudTransporteRepositories _solicitudTransporteRepositories;
    private readonly IAsignacionRepository _asignacionRepository;
    private readonly IVehiculoRepositories _vehiculoRepository;
    private readonly IConductorRepositories _conductorRepository;

    public SolicitudTransporteServices(
        ISolicitudTransporteRepositories solicitudTransporteRepositories,
        IAsignacionRepository asignacionRepository,
        IVehiculoRepositories vehiculoRepository,
        IConductorRepositories conductorRepository)
    {
        _solicitudTransporteRepositories = solicitudTransporteRepositories;
        _asignacionRepository = asignacionRepository;
        _vehiculoRepository = vehiculoRepository;
        _conductorRepository = conductorRepository;
    }

    public async Task<List<SolicitudTransporteReponseDTOs?>> ObtenerTodos()
    {
        var solicitudTrans = await _solicitudTransporteRepositories.ObtenerTodos();
        if (solicitudTrans == null)
        {
            throw new Exception("Error al obtener los solicitud transporte.");
        }
        ;
        return solicitudTrans.Select(c => c.ToResponse()).ToList();
    }
    public async Task<SolicitudTransporteReponseDTOs?> ObtenerPorId(int Id)
    {
        var solicitudTrans = await _solicitudTransporteRepositories.ObtenerPorId(Id);
        if (solicitudTrans == null)
        {
            throw new Exception("Error al buscar el solicitud transporte.");
        }
        ;
        return solicitudTrans.ToResponse();
    }
    public async Task Crear(CreateSolicitudTransporteDTOs createSolicitudTransporteDTOs, int usuarioId)
    {
        var solicitudTrans = new SolicitudTransporte
        {
            AreaSolicitante = createSolicitudTransporteDTOs.AreaSolicitante,
            CantidadColaboradores = createSolicitudTransporteDTOs.CantidadColaboradores,
            FechaViaje = createSolicitudTransporteDTOs.FechaViaje,
            HoraSalida = createSolicitudTransporteDTOs.HoraSalida,
            PuntoOrigen = createSolicitudTransporteDTOs.PuntoOrigen,
            Destino = createSolicitudTransporteDTOs.Destino,
            TipoViaje = createSolicitudTransporteDTOs.TipoViaje,
            MotivoViaje = createSolicitudTransporteDTOs.MotivoViaje,
            Estado = createSolicitudTransporteDTOs.Estado,
            UsuarioSolicitaId = usuarioId,
        };
        await _solicitudTransporteRepositories.Crear(solicitudTrans);
    }
    public async Task Actualizar(int Id, UpdateSolicitudTransporteDTOs updateSolicitudTransporteDTOs)
    {
        var solicitudTrans = await _solicitudTransporteRepositories.ObtenerPorId(Id);
        if (solicitudTrans == null)
            throw new Exception("Error al actualizar el solicitud transporte.");

        bool cambioFechaHora = solicitudTrans.FechaViaje != updateSolicitudTransporteDTOs.FechaViaje
                            || solicitudTrans.HoraSalida != updateSolicitudTransporteDTOs.HoraSalida;

        solicitudTrans.AreaSolicitante = updateSolicitudTransporteDTOs.AreaSolicitante;
        solicitudTrans.CantidadColaboradores = updateSolicitudTransporteDTOs.CantidadColaboradores;
        solicitudTrans.FechaViaje = updateSolicitudTransporteDTOs.FechaViaje;
        solicitudTrans.HoraSalida = updateSolicitudTransporteDTOs.HoraSalida;
        solicitudTrans.PuntoOrigen = updateSolicitudTransporteDTOs.PuntoOrigen;
        solicitudTrans.Destino = updateSolicitudTransporteDTOs.Destino;
        solicitudTrans.TipoViaje = updateSolicitudTransporteDTOs.TipoViaje;
        solicitudTrans.MotivoViaje = updateSolicitudTransporteDTOs.MotivoViaje;
        solicitudTrans.Estado = updateSolicitudTransporteDTOs.Estado;
        await _solicitudTransporteRepositories.Actualizar(Id, solicitudTrans);

        if (cambioFechaHora)
        {
            var asignacion = await _asignacionRepository.ObtenerPorSolicitudId(Id);
            if (asignacion != null && asignacion.Estado == EstadoAsignacion.EnCurso)
            {
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
        }
    }
    public async Task Eliminar(int Id)
    {
        var solicitudTrans = await _solicitudTransporteRepositories.ObtenerPorId(Id);
        if (solicitudTrans == null)
        {
            throw new Exception("Error al eliminar el solicitud transporte.");
        }
        ;
        await _solicitudTransporteRepositories.Eliminar(Id);
    }
}
