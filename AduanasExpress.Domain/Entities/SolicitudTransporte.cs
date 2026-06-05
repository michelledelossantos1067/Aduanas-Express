
namespace AduanasExpress.Domain.Entitis;
public class SolicitudTransporte{
    public int Id { get; set; }
    public string AreaSolicitante { get; set; }
    public int CantidadColaboradores {get;set;}
    public DateTime FechaViaje {get;set;}
    public TimeSpan HoraSalida {get;set;}
    public string Destino {get;set;}
    public string MotivoViaje {get;set;}
    public EstadosSolicitudes Estado {get;set;}
    public int VehiculoId {get;set;}
    public Vehiculo Vehiculo { get; set; }
    public int ConductorId { get; set; }
    public Conductor Conductor { get; set; }
    public int UsuarioSolicitaId { get; set; }
    public Usuario UsuarioSolicita { get; set; }
    public DateTime FechaCreacion {get;set;}
}