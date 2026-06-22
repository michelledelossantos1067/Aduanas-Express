using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Interfaces.Repositories;

public interface IRolRepository
{
    Task<List<Rol>> ObtenerTodos();
    Task<Rol?> ObtenerPorId(int id);
    Task Crear(Rol rol);
    Task ActualizarPermisos(int rolId, List<RolPermiso> permisos);
}
