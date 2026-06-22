namespace AduanasExpress.Domain.Entitis;

public class Mantenimiento
{
    public int Id { get; set; }

    public int VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; }

    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public EstadosMantenimiento Estado { get; set; } = EstadosMantenimiento.Programado;

    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaRealizada { get; set; }

    public decimal? Kilometraje { get; set; }
    public decimal Costo { get; set; }
    public string Taller { get; set; }
    public string Responsable { get; set; }
    public string Observaciones { get; set; }
}
