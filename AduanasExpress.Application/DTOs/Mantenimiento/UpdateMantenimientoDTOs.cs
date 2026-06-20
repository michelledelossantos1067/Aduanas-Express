using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.DTOs.Mantenimiento;
public class UpdateMantenimientoDTOs{
    public int Id {get;set;}
    public DateTime? Fecha {get;set;}
    public string TipoMantenimiento {get;set;}
    public string Descripcion {get;set;}
    public decimal Costo {get;set;}
    public string Taller {get;set;}
    public DateTime? ProximoMantenimiento {get;set;}

    public int VehiculoId {get;set;}
}
