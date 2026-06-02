using AduanasExpress.Domain.Entitis;
namespace AduanasExpress.Application.interfaces.Repositories;
public interface IUsuarioRepositories{
    public Task<List<Usuario?>> ObtenerTodos();
    public Task<Usuario?> ObtenerPorId(int id);
    public Task Crear(Usuario usuario);
    public Task Actualizar(int id,Usuario usuario);
    public Task Eliminar(int id);
}