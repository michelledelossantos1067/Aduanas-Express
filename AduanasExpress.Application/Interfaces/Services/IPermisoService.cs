namespace AduanasExpress.Application.Interfaces.Services;

public interface IPermisoService
{
    Task<bool> TienePermiso(int rolId, string modulo, string accion);
}
