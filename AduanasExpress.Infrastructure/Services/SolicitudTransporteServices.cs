using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.DTOs.SolicitudTransporte;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;


namespace AduanasExpress.Infrastructure.Services;

public class SolicitudTransporteServices : ISolicitudTransporteService
{
    private readonly ISolicitudTransporteRepositories _solicitudTransporteRepositories;

    public SolicitudTransporteServices(ISolicitudTransporteRepositories solicitudTransporteRepositories)
    {
        _solicitudTransporteRepositories = solicitudTransporteRepositories;
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
            Destino = createSolicitudTransporteDTOs.Destino,
            MotivoViaje = createSolicitudTransporteDTOs.MotivoViaje,
            Estado = createSolicitudTransporteDTOs.Estado,
            VehiculoId = createSolicitudTransporteDTOs.VehiculoId ?? 0,
            ConductorId = createSolicitudTransporteDTOs.ConductorId ?? 0,
            UsuarioSolicitaId = usuarioId,
        };
        await _solicitudTransporteRepositories.Crear(solicitudTrans);
    }
    public async Task Actualizar(int Id, UpdateSolicitudTransporteDTOs updateSolicitudTransporteDTOs)
    {
        var solicitudTrans = await _solicitudTransporteRepositories.ObtenerPorId(Id);
        if (solicitudTrans == null)
        {
            throw new Exception("Error al actualizar el solicitud transporte.");
        }
        
        solicitudTrans.AreaSolicitante = updateSolicitudTransporteDTOs.AreaSolicitante;
        solicitudTrans.CantidadColaboradores = updateSolicitudTransporteDTOs.CantidadColaboradores;
        solicitudTrans.FechaViaje = updateSolicitudTransporteDTOs.FechaViaje;
        solicitudTrans.HoraSalida = updateSolicitudTransporteDTOs.HoraSalida;
        solicitudTrans.Destino = updateSolicitudTransporteDTOs.Destino;
        solicitudTrans.MotivoViaje = updateSolicitudTransporteDTOs.MotivoViaje;
        solicitudTrans.Estado = updateSolicitudTransporteDTOs.Estado;
        solicitudTrans.VehiculoId = updateSolicitudTransporteDTOs.VehiculoId;
        solicitudTrans.ConductorId = updateSolicitudTransporteDTOs.ConductorId;
        await _solicitudTransporteRepositories.Actualizar(Id, solicitudTrans);
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