
namespace AduanasExpress.Domain.Entitis;
public class Conductor{
    public int Id {get;set;}
    public string Nombre {get;set;}
    public string Apellido {get;set;}
    public string Cedula {get;set;}
    public string NumeroLicencia {get;set;}
    public string TipoLicencia { get; set; }
    public DateTime FechaVencLicencia { get; set; }
    public string Telefono {get;set;}
    public string Direccion { get; set; }
    public int SupervisorId { get; set; }
    public Usuario Supervisor { get; set; }
    public EstadosConductor Estado { get; set; }

}