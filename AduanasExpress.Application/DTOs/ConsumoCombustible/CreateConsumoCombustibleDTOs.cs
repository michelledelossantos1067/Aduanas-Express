namespace AduanasExpress.Application.DTOs.ConsumoCombustible;

public class CreateConsumoCombustibleDTOs{
    public decimal Galones {get;set;}
    public decimal CostoPorGalon {get;set;}
    public decimal CostoTotal {get;set;}

    public int VehiculoId {get;set;}
    public int SolicitudId {get;set;}
}
