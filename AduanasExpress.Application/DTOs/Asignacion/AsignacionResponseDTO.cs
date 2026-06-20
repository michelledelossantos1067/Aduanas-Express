using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Application.DTOs.SolicitudTransporte;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.DTOs.Asignacion;

public class AsignacionResponseDTO
{
    public int Id { get; set; }
    public int SolicitudId { get; set; }
    public int VehiculoId { get; set; }
    public int ConductorId { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public int AsignadoPorId { get; set; }
    public EstadoAsignacion Estado { get; set; }
    public DateTime? FechaFinalizacion { get; set; }

    public ConductorReponseDTOs? Conductor { get; set; }
    public VehiculoResponseDTOs? Vehiculo { get; set; }
    public SolicitudTransporteReponseDTOs? Solicitud { get; set; }
}
