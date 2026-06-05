using AduanasExpress.Application.DTOs.SolicitudTransporte;

namespace AduanasExpress.Application.Interfaces.Services;
public interface ISolicitudTransporteService{
    public Task<List<SolicitudTransporteReponseDTOs?>> ObtenerTodos();
    public Task<SolicitudTransporteReponseDTOs?> ObtenerPorId(int Id);
    public Task Crear(CreateSolicitudTransporteDTOs createSolicitudTransporteDTOs);
    public Task Actualizar(int Id,UpdateSolicitudTransporteDTOs updateSolicitudTransporteDTOs);
    public Task Eliminar(int Id);
}