using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class ReporteMapping
    {
        public static ReporteViajeDTO ToReporteViajeDTO(
            this SolicitudTransporte solicitud,
            string? nombreConductor = null,
            string? vehiculoPlaca = null)
        {
            return new ReporteViajeDTO
            {
                Id = solicitud.Id,
                AreaSolicitante = solicitud.AreaSolicitante,
                Destino = solicitud.Destino,
                FechaViaje = solicitud.FechaViaje,
                CantidadPasajeros = solicitud.CantidadColaboradores,
                Estado = solicitud.Estado.ToString(),
                NombreConductor = nombreConductor ?? "Sin asignar",
                VehiculoPlaca = vehiculoPlaca ?? "—",
            };
        }
    }
}
