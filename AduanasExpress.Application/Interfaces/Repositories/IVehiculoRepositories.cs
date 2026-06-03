
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.interfaces.Repositories;
public interface IVehiculoRepositories{
    public Task<List<Vehiculo?>> ObtenerTodos();
    public Task<Vehiculo?> ObtenerPorId(int Id);
    public Task Crear(Vehiculo vehiculo);
    public Task Actualizar(int Id,Vehiculo vehiculo);
    public Task Eliminar(int Id);
}