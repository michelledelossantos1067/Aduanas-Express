namespace AduanasExpress.Domain.Entitis;
public class Vehiculo{
    public int Id {get;set;}
    public string Marca {get;set;}
    public string Modelo {get;set;}
    public int Año {get;set;}
    public string Matricula {get;set;}
    public string Color {get;set;}
    public string Tipo {get;set;}
    public int Capacidad {get;set;}
    public EstadosVehiculo Estado {get;set;}
    public decimal Kilometraje {get;set;}
    public DateTime? FechaUltimoMant {get;set;}
}
