namespace AduanasExpress.Domain.Entitis;

public class Asignacion{
    public int Id { get; set; }
    public int SolicitudId { get; set; }
    public int VehiculoId { get; set; }
    public int ConductorId { get; set; }
    public DateTime? FechaAsignacion {get;set;}
    public int AsignadoPorId { get; set; }
    public EstadoAsignacion Estado { get; set; } = EstadoAsignacion.Pendiente;
    public DateTime? FechaFinalizacion { get; set; }

    public Usuario AsignadoPor { get; set; } = null!;
    public SolicitudTransporte Solicitud { get; set; } = null!;
    public Conductor Conductor { get; set; } = null!;
    public Vehiculo Vehiculo {get;set;} = null!;
}
