namespace AduanasExpress.Domain.Entitis;

public class RolPermiso
{
    public int Id { get; set; }
    public int RolId { get; set; }
    public string Modulo { get; set; } = null!;
    public string Accion { get; set; } = null!;
    public bool Permitido { get; set; }

    public Rol Rol { get; set; } = null!;
}