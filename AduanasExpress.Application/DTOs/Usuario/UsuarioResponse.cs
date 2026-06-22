namespace AduanasExpress.Application.DTOs.Usuario;
public class UsuarioResponse{
    public int Id {get;set;}
    public string Nombre {get;set;}
    public string Apellido {get;set;}
    public string Email {get;set;}
    public int RolId { get; set; }
    public string RolNombre { get; set; }
    public bool IsActive { get; set; }
    public bool PuedeEliminarse { get; set; }
}