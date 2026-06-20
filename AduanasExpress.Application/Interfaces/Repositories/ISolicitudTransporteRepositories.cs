using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Interfaces.Repositories;
public interface ISolicitudTransporteRepositories{
    public Task<List<SolicitudTransporte?>> ObtenerTodos();
    public Task<SolicitudTransporte?> ObtenerPorId(int Id);
    public Task Crear(SolicitudTransporte solicitudTransporte);
    public Task Actualizar(int Id,SolicitudTransporte solicitudTransporte);
    public Task Eliminar(int Id);
}
