namespace AduanasExpress.Application.DTOs.Reporte;
public class ReporteViajeDTO
{
    public int Id { get; set; }
    public string AreaSolicitante { get; set; }
    public string Destino { get; set; }
    public DateTime? FechaViaje { get; set; }
    public string NombreConductor { get; set; }
    public string VehiculoPlaca { get; set; }
    public int CantidadPasajeros { get; set; }
    public string Estado { get; set; }
}

public class ReporteConsumoDTO
{
    public string VehiculoPlaca { get; set; }
    public string VehiculoMarca { get; set; }
    public decimal TotalGalones { get; set; }
    public decimal CostoTotal { get; set; }
    public int TotalViajes { get; set; }
}

public class ReporteConductorDTO
{
    public string NombreConductor { get; set; }
    public string Licencia { get; set; }
    public int TotalViajes { get; set; }
    public int TotalPasajeros { get; set; }
}

public class ReporteSolicitudDTO
{
    public int TotalSolicitudes { get; set; }
    public int Pendientes { get; set; }
    public int Aprobadas { get; set; }
    public int Rechazadas { get; set; }
    public int Canceladas { get; set; }
    public int Finalizadas { get; set; }
}
