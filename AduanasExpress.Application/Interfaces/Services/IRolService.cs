using AduanasExpress.Application.DTOs.Rol;

namespace AduanasExpress.Application.Interfaces.Services;

public interface IRolService
{
    Task<List<RolDTO>> ObtenerTodos();
    Task<RolDTO?> ObtenerPorId(int id);
    Task Crear(CreateRolDTO dto);
    Task ActualizarPermisos(int rolId, UpdatePermisosDTO dto);
}
