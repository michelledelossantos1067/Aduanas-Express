using AduanasExpress.Application.DTOs.Rol;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;

public class RolService : IRolService
{
    private readonly IRolRepository _rolRepository;
    public RolService(IRolRepository rolRepository) => _rolRepository = rolRepository;

    public async Task<List<RolDTO>> ObtenerTodos()
    {
        var roles = await _rolRepository.ObtenerTodos();
        return roles.Select(Mapear).ToList();
    }

    public async Task<RolDTO?> ObtenerPorId(int id)
    {
        var rol = await _rolRepository.ObtenerPorId(id);
        return rol == null ? null : Mapear(rol);
    }

    public async Task Crear(CreateRolDTO dto)
    {
        var rol = new Rol
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            EsSistema = false
        };
        await _rolRepository.Crear(rol);
    }

    public async Task ActualizarPermisos(int rolId, UpdatePermisosDTO dto)
    {
        var permisos = dto.Permisos.Select(p => new RolPermiso
        {
            RolId = rolId,
            Modulo = p.Modulo,
            Accion = p.Accion,
            Permitido = p.Permitido
        }).ToList();

        await _rolRepository.ActualizarPermisos(rolId, permisos);
    }

    private static RolDTO Mapear(Rol rol) => new()
    {
        Id = rol.Id,
        Nombre = rol.Nombre,
        Descripcion = rol.Descripcion,
        EsSistema = rol.EsSistema,
        Permisos = rol.Permisos.Select(p => new RolPermisoDTO
        {
            Modulo = p.Modulo,
            Accion = p.Accion,
            Permitido = p.Permitido
        }).ToList()
    };
}
