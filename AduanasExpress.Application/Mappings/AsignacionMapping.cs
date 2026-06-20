using AduanasExpress.Application.DTOs.Asignacion;
using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Application.DTOs.SolicitudTransporte;
using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class AsignacionMapping
    {
        public static AsignacionResponseDTO ToResponse(this Asignacion asignacion)
        {
            return new AsignacionResponseDTO
            {
                Id              = asignacion.Id,
                SolicitudId     = asignacion.SolicitudId,
                VehiculoId      = asignacion.VehiculoId,
                ConductorId     = asignacion.ConductorId,
                FechaAsignacion = asignacion.FechaAsignacion,
                AsignadoPorId   = asignacion.AsignadoPorId,
                Estado            = asignacion.Estado,
                FechaFinalizacion = asignacion.FechaFinalizacion,

                Conductor = asignacion.Conductor == null ? null : new ConductorReponseDTOs
                {
                    Id                = asignacion.Conductor.Id,
                    Nombre            = asignacion.Conductor.Nombre,
                    Apellido          = asignacion.Conductor.Apellido,
                    Cedula            = asignacion.Conductor.Cedula,
                    NumeroLicencia    = asignacion.Conductor.NumeroLicencia,
                    TipoLicencia      = asignacion.Conductor.TipoLicencia,
                    FechaVencLicencia = asignacion.Conductor.FechaVencLicencia,
                    Telefono          = asignacion.Conductor.Telefono,
                    Direccion         = asignacion.Conductor.Direccion,
                    SupervisorId      = asignacion.Conductor.SupervisorId,
                    Estado            = asignacion.Conductor.Estado,
                },

                Vehiculo = asignacion.Vehiculo == null ? null : new VehiculoResponseDTOs
                {
                    Id              = asignacion.Vehiculo.Id,
                    Marca           = asignacion.Vehiculo.Marca,
                    Modelo          = asignacion.Vehiculo.Modelo,
                    Año             = asignacion.Vehiculo.Año,
                    Matricula       = asignacion.Vehiculo.Matricula,
                    Color           = asignacion.Vehiculo.Color,
                    Tipo            = asignacion.Vehiculo.Tipo,
                    Capacidad       = asignacion.Vehiculo.Capacidad,
                    Estado          = asignacion.Vehiculo.Estado,
                    Kilometraje     = asignacion.Vehiculo.Kilometraje,
                    FechaUltimoMant = asignacion.Vehiculo.FechaUltimoMant,
                },

                Solicitud = asignacion.Solicitud == null ? null : new SolicitudTransporteReponseDTOs
                {
                    Id                    = asignacion.Solicitud.Id,
                    AreaSolicitante       = asignacion.Solicitud.AreaSolicitante,
                    Destino               = asignacion.Solicitud.Destino,
                    FechaViaje            = asignacion.Solicitud.FechaViaje,
                    HoraSalida            = asignacion.Solicitud.HoraSalida,
                    CantidadColaboradores = asignacion.Solicitud.CantidadColaboradores,
                    MotivoViaje           = asignacion.Solicitud.MotivoViaje,
                    Estado                = asignacion.Solicitud.Estado,
                    UsuarioSolicitaId     = asignacion.Solicitud.UsuarioSolicitaId,
                    FechaCreacion         = asignacion.Solicitud.FechaCreacion,
                },
            };
        }
    }
}
