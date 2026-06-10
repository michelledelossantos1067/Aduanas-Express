using AduanasExpress.Domain.Entitis;
namespace AduanasExpress.Application.interfaces.Repositories;
public interface IUsuarioRepositories{
    public Task<List<Usuario?>> ObtenerTodos();
    public Task<Usuario?> ObtenerPorId(int Id);
    public Task Crear(Usuario usuario);
    public Task Actualizar(int Id,Usuario usuario);
    Task<Usuario?> ObtenerPorEmail(string Email);
    public Task Eliminar(int Id);
}