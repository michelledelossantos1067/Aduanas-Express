using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class MantenimientoMapping
    {
        public static MantenimientoResponseDTOs ToResponse(this Mantenimiento mantenimiento)
        {
            return new MantenimientoResponseDTOs
            {
                Id = mantenimiento.Id,
                TipoMantenimiento = mantenimiento.TipoMantenimiento,
                Descripcion = mantenimiento.Descripcion,
                Costo = mantenimiento.Costo,
                Taller = mantenimiento.Taller,
                ProximoMantenimiento = mantenimiento.ProximoMantenimiento,
                VehiculoId = mantenimiento.VehiculoId
            };
        }
    }
}
