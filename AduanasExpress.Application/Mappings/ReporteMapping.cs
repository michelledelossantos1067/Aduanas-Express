using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class ReporteMapping
    {
        public static ReporteViajeDetalleDTO ToReporteViajeDetalleDTO(
            this SolicitudTransporte solicitud,
            Asignacion asignacion = null)
        {
            return new ReporteViajeDetalleDTO
            {
                Id               = solicitud.Id,
                AreaSolicitante  = solicitud.AreaSolicitante,
                Destino          = solicitud.Destino,
                FechaViaje       = solicitud.FechaViaje,
                CantidadPasajeros = solicitud.CantidadColaboradores,
                Estado           = solicitud.Estado.ToString(),
                NombreConductor  = asignacion?.Conductor != null
                    ? $"{asignacion.Conductor.Nombre} {asignacion.Conductor.Apellido}"
                    : null,
                VehiculoPlaca    = asignacion?.Vehiculo?.Matricula,
            };
        }

        public static ReporteSolicitudDetalleDTO ToReporteSolicitudDetalleDTO(
            this SolicitudTransporte solicitud)
        {
            return new ReporteSolicitudDetalleDTO
            {
                Id               = solicitud.Id,
                AreaSolicitante  = solicitud.AreaSolicitante,
                Destino          = solicitud.Destino,
                FechaViaje       = solicitud.FechaViaje,
                CantidadPasajeros = solicitud.CantidadColaboradores,
                Estado           = solicitud.Estado.ToString(),
            };
        }
    }
}