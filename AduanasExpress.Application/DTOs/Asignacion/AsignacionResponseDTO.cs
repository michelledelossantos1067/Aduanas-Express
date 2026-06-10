
namespace AduanasExpress.Application.DTOs.Asignacion;

public class AsignacionResponseDTO{
    public int Id { get; set; }
    public int SolicitudId {get;set;}
    public int VehiculoId {get;set;}
    public int ConductorId {get;set;}
    public DateTime FechaAsignacion {get;set;}
    public int AsignadoPorId { get; set; }
}