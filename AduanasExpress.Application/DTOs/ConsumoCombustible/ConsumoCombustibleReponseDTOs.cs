using AduanasExpress.Domain.Entitis;
namespace AduanasExpress.Application.DTOs.ConsumoCombustible;

public class ConsumoCombustibleReponseDTOs{
    public int Id {get;set;}
    public DateTime? Fecha {get;set;}
    public decimal Galones {get;set;}
    public decimal CostoPorGalon {get;set;}
    public decimal CostoTotal {get;set;}

    public int VehiculoId {get;set;}
    public int SolicitudId {get;set;}
}
