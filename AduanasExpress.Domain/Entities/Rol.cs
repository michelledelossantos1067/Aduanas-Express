namespace AduanasExpress.Domain.Entitis;

public class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public bool EsSistema { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public ICollection<RolPermiso> Permisos { get; set; } = new List<RolPermiso>();
}