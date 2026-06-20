using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static  class ReporteMapping
    {
        public static ReporteViajeDTO ToReporteViajeDTO(this SolicitudTransporte solicitud)
        {
            return new ReporteViajeDTO
            {
                AreaSolicitante = solicitud.AreaSolicitante,
                Destino = solicitud.Destino,
                FechaViaje = solicitud.FechaViaje,
                CantidadPasajeros = solicitud.CantidadColaboradores,
                Estado = solicitud.Estado.ToString(),
            };
        }
    }
}
