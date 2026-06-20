using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Application.DTOs.Vehiculo;

namespace AduanasExpress.Application.DTOs.Asignacion;

public class DisponiblesResponseDTO
{
    public List<VehiculoResponseDTOs> Vehiculos { get; set; } = [];
    public List<ConductorReponseDTOs> Conductores { get; set; } = [];
}
