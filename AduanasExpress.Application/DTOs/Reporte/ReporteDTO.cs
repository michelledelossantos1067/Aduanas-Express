namespace AduanasExpress.Application.DTOs.Reporte;

// =====================================================================
// VIAJES
// =====================================================================
public class ReporteViajeDetalleDTO
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

public class ReporteViajesDTO
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public int TotalViajes { get; set; }
    public int Completados { get; set; }
    public int Pendientes { get; set; }
    public int Cancelados { get; set; }
    public int TotalPasajeros { get; set; }
    public List<ReporteViajeDetalleDTO> Detalles { get; set; } = new();
}

// =====================================================================
// CONSUMO DE COMBUSTIBLE
// =====================================================================
public class ReporteConsumoDetalleDTO
{
    public string VehiculoPlaca { get; set; }
    public string VehiculoMarca { get; set; }
    public decimal TotalGalones { get; set; }
    public decimal CostoTotal { get; set; }
    public int TotalRegistros { get; set; }
}

public class ReporteConsumoDTO
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal TotalGalones { get; set; }
    public decimal CostoPromedioGalon { get; set; }
    public int TotalVehiculos { get; set; }
    public List<ReporteConsumoDetalleDTO> Detalles { get; set; } = new();
}

// =====================================================================
// SOLICITUDES
// =====================================================================
public class ReporteSolicitudDetalleDTO
{
    public int Id { get; set; }
    public string AreaSolicitante { get; set; }
    public string Destino { get; set; }
    public DateTime? FechaViaje { get; set; }
    public int CantidadPasajeros { get; set; }
    public string Estado { get; set; }
}

public class ReporteSolicitudesDTO
{
    public int Total { get; set; }
    public int Pendientes { get; set; }
    public int Aprobadas { get; set; }
    public int Rechazadas { get; set; }
    public int Canceladas { get; set; }
    public int Finalizadas { get; set; }
    public List<ReporteSolicitudDetalleDTO> Detalles { get; set; } = new();
}

// =====================================================================
// CONDUCTORES
// =====================================================================
public class ReporteConductorDetalleDTO
{
    public string NombreConductor { get; set; }
    public string Licencia { get; set; }
    public int TotalViajes { get; set; }
    public int TotalPasajeros { get; set; }
    public DateTime? UltimoViaje { get; set; }
}

public class ReporteConductoresDTO
{
    public int TotalConductores { get; set; }
    public int TotalViajes { get; set; }
    public int TotalPasajeros { get; set; }
    public double PromedioPasajerosPorViaje { get; set; }
    public List<ReporteConductorDetalleDTO> Detalles { get; set; } = new();
}