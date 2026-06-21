using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.DTOs.SolicitudTransporte;
public class CreateSolicitudTransporteDTOs{
    public string AreaSolicitante { get; set; }
    public int CantidadColaboradores {get;set;}
    public DateTime? FechaViaje {get;set;}
    public TimeSpan HoraSalida {get;set;}
    public string PuntoOrigen {get;set;}
    public string Destino {get;set;}
    public TipoViaje TipoViaje {get;set;}
    public string MotivoViaje {get;set;}
    public EstadosSolicitudes Estado {get;set;}
}
