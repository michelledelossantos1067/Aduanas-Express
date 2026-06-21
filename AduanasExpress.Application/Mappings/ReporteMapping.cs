using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class ReporteMapping
    {
        /// <summary>
        /// Mapea una solicitud a su fila de detalle del reporte de viajes.
        /// La asignación es opcional; si no existe, conductor y vehículo
        /// aparecen como null y el reporte los muestra como "Sin asignar".
        /// </summary>
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