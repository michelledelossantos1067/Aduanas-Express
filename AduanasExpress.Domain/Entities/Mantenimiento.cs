
namespace AduanasExpress.Domain.Entitis;
public class Mantenimiento{
    public int Id {get;set;}
    public DateTime Fecha {get;set;}
    public string TipoMantenimiento {get;set;}
    public string Descripcion {get;set;}
    public decimal Costo {get;set;}
    public string Taller {get;set;}
    public DateTime? ProximoMantenimiento {get;set;}
    
    public int VehiculoId {get;set;}
    public Vehiculo Vehiculo {get;set;}
}