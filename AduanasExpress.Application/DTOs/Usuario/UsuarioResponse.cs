
namespace AduanasExpress.Application.DTOs.Usuario;
public class UsuarioResponse{
    public int id {get;set;}
    public string nombre {get;set;}
    public string apellido {get;set;}
    public string email {get;set;}
    public string telefono {get;set;}
    public string direccion {get;set;}
    public Rol rol {get;set;}
}