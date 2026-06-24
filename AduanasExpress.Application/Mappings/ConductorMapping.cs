using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class ConductorMapping
    {
        public static ConductorReponseDTOs ToResponse(this Conductor conductor)
        {
            return new ConductorReponseDTOs
            {
                Id = conductor.Id,
                Nombre = conductor.Nombre,
                Apellido = conductor.Apellido,
                Cedula = conductor.Cedula,
                NumeroLicencia = conductor.NumeroLicencia,
                TipoLicencia = conductor.TipoLicencia,
                FechaVencLicencia = conductor.FechaVencLicencia,
                Telefono = conductor.Telefono,
                Direccion = conductor.Direccion,
                SupervisorId = conductor.SupervisorId,
                Estado = conductor.Estado,
                IsActive = conductor.IsActive
            };
        }

    }
}
