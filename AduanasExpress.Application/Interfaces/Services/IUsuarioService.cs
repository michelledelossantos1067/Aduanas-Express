
using AduanasExpress.Application.DTOs.Usuario;

namespace AduanasExpress.Application.interfaces.Services;
public interface IUsuarioService{
    public Task<List<UsuarioResponse?>> ObtenerTodos();
    public Task<UsuarioResponse?> ObtenerPorId(int id);
    public Task Crear(CreateUsuario createUsuario);
    public Task Actualizar(int id,UpdateUsuario updateUsuario);
    public Task Eliminar(int id);
}