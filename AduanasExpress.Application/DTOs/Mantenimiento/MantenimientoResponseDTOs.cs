namespace AduanasExpress.Application.DTOs.Mantenimiento;

public class MantenimientoResponseDTOs
{
    public int Id { get; set; }
    public int VehiculoId { get; set; }
    public string VehiculoPlaca { get; set; }
    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public string Estado { get; set; }
    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaRealizada { get; set; }
    public decimal? Kilometraje { get; set; }
    public decimal Costo { get; set; }
    public string Taller { get; set; }
    public string Responsable { get; set; }
    public string Observaciones { get; set; }
}
