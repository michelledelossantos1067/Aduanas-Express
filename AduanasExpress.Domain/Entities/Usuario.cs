namespace AduanasExpress.Domain.Entitis;
public class Usuario{
    public int Id {get;set;}
    public string Nombre {get;set;}
    public string Apellido {get;set;}
    public string Email {get;set;}
    public string Password {get;set;}
    public int RolId { get; set; } = 2;
    public Rol Rol { get; set; } = null!;
    public bool IsActive {get;set;} = true;
    public bool RequiereCambioPassword { get; set; } = false;

}
