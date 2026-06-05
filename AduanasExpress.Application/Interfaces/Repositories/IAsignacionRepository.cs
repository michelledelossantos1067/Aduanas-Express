using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Interfaces.Repositories;
public interface IAsignacionRepository{
    public Task<List<Asignacion?>> ObtenerTodos();
    public Task<Asignacion?> ObtenerPorId(int Id);
    public Task Crear(Asignacion asignacion);
}