using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Interfaces.Repositories;
public interface IAsignacionRepository{
    public Task<List<Asignacion?>> ObtenerTodos();
    public Task<Asignacion?> ObtenerPorId(int Id);
    public Task Crear(Asignacion asignacion);
    public Task Actualizar(int id, Asignacion asignacion);
    Task<bool> ExisteParaVehiculo(int vehiculoId);
    Task<bool> ExisteParaConductor(int conductorId);
    Task<List<Asignacion>> ObtenerPorEstado(EstadoAsignacion estado);
    Task<Asignacion?> ObtenerPorSolicitudId(int solicitudId);
}
