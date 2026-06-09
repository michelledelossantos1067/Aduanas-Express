using AduanasExpress.Application.DTOs.Asignacion;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class AsignacionMapping
    {
        public static AsignacionResponseDTO ToResponse(this Asignacion asignacion)
        {
            return new AsignacionResponseDTO
            {
                Id = asignacion.Id,
                SolicitudId = asignacion.SolicitudId,
                VehiculoId = asignacion.VehiculoId,
                ConductorId = asignacion.ConductorId,
                FechaAsignacion = asignacion.FechaAsignacion,
                AsignadoPorId = asignacion.AsignadoPorId
            };
        }
    }
}
