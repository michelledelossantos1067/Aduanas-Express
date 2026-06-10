using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Interfaces.Repositories;
public interface IConductorRepositories{
    public Task<List<Conductor?>> ObtenerTodos();
    public Task<Conductor?> ObtenerPorId(int Id);
    public Task Crear(Conductor conductor);
    public Task Actualizar(int Id,Conductor conductor);
    public Task Eliminar(int Id);
}