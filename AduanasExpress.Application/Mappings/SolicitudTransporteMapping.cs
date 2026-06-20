using AduanasExpress.Application.DTOs.SolicitudTransporte;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
   public static class SolicitudTransporteMapping
    {
        public static SolicitudTransporteReponseDTOs ToResponse(this SolicitudTransporte solicitudTransporte)
        {
            return new SolicitudTransporteReponseDTOs
            {
                Id = solicitudTransporte.Id,
                AreaSolicitante = solicitudTransporte.AreaSolicitante,
                CantidadColaboradores = solicitudTransporte.CantidadColaboradores,
                FechaViaje = solicitudTransporte.FechaViaje,
                HoraSalida = solicitudTransporte.HoraSalida,
                Destino = solicitudTransporte.Destino,
                MotivoViaje = solicitudTransporte.MotivoViaje,
                Estado = solicitudTransporte.Estado,
                UsuarioSolicitaId = solicitudTransporte.UsuarioSolicitaId,
                FechaCreacion = solicitudTransporte.FechaCreacion
            };
        }
    }
}
