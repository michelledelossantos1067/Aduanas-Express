
namespace AduanasExpress.Application.DTOs.Usuario;
public class UsuarioResponse{
    public int Id {get;set;}
    public string Nombre {get;set;}
    public string Apellido {get;set;}
    public string Email {get;set;}
    public Roles Rol{get;set;}
}