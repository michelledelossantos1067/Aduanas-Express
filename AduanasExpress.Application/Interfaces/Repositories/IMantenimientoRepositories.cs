using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Interfaces.Repositories;
public interface IMantenimientoRepositories{
    public Task<List<Mantenimiento?>> ObtenerTodos();
    public Task<Mantenimiento?> ObtenerPorId(int Id);
    public Task Crear(Mantenimiento mantenimiento);
    public Task Actualizar(int Id,Mantenimiento mantenimiento);
    public Task Eliminar(int Id);
}
