using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class VehiculoMapping
    {
        public static VehiculoResponseDTOs ToResponse(this Vehiculo vehiculo)
        {
            return new VehiculoResponseDTOs
            {
                Id = vehiculo.Id,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Año = vehiculo.Año,
                Matricula = vehiculo.Matricula,
                Color = vehiculo.Color,
                Tipo = vehiculo.Tipo,
                Capacidad = vehiculo.Capacidad,
                Estado = vehiculo.Estado,
                Kilometraje = vehiculo.Kilometraje,
                FechaUltimoMant = vehiculo.FechaUltimoMant

            };
        }
    }
}
