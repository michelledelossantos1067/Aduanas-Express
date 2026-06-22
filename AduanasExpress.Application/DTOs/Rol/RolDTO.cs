namespace AduanasExpress.Application.DTOs.Rol;

public class RolDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool EsSistema { get; set; }
    public List<RolPermisoDTO> Permisos { get; set; } = [];
}

public class RolPermisoDTO
{
    public string Modulo { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public bool Permitido { get; set; }
}

public class CreateRolDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class UpdatePermisosDTO
{
    public List<RolPermisoDTO> Permisos { get; set; } = [];
}
