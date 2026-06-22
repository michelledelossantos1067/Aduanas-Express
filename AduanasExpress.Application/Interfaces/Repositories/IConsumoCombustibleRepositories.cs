using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Interfaces.Repositories;
public interface IConsumoCombustibleRepositories{
    public Task<List<ConsumoCombustible?>> ObtenerTodos();
    public Task<ConsumoCombustible?> ObtenerPorId(int Id);
    public Task Crear(ConsumoCombustible consumoCombustible);
    public Task Actualizar(int Id,ConsumoCombustible consumoCombustible);
    public Task Eliminar(int Id);
    Task<bool> ExisteParaVehiculo(int vehiculoId);

}
