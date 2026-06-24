using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class ConsumoCombustibleMapping
    {
        public static ConsumoCombustibleReponseDTOs ToResponse(this ConsumoCombustible consumo)
        {
            return new ConsumoCombustibleReponseDTOs
            {
                Id = consumo.Id,
                Fecha = consumo.Fecha,
                Galones = consumo.Galones,
                CostoPorGalon = consumo.CostoPorGalon,
                CostoTotal = consumo.CostoTotal,
                VehiculoId = consumo.VehiculoId,
                SolicitudId = consumo.SolicitudId ?? 0,
            };
        }
    }
}
