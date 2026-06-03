
using AduanasExpress.Application.DTOs.Usuario;

namespace AduanasExpress.Application.interfaces.Services;
public interface IUsuarioService{
    public Task<List<UsuarioResponse?>> ObtenerTodos();
    public Task<UsuarioResponse?> ObtenerPorId(int Id);
    public Task Crear(CreateUsuario createUsuario);
    public Task Actualizar(int Id,UpdateUsuario updateUsuario);
    public Task Eliminar(int Id);
}