
namespace AduanasExpress.Application.DTOs.Asignacion;

public class CreateAsignacionDTO{
    public int SolicitudId {get;set;}
    public int VehiculoId {get;set;}
    public int ConductorId {get;set;}
    public DateTime FechaAsignacion {get;set;}
    public int AsignadoPorId { get; set; }
}