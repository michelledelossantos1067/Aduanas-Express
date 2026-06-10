using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings
{
    public static class UsuarioMapping
    {
        public static UsuarioResponse ToResponse(this Usuario usuario)
        {
            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rol = usuario.Rol
            };
        }
    }
}