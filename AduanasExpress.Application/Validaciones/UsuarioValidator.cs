using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Domain.Entitis;
using FluentValidation;

namespace AduanasExpress.Application.Validaciones
{
    public class UsuarioValidator:AbstractValidator<CreateUsuario>
    {
        public UsuarioValidator() 
        {
            RuleFor(u => u.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(20).WithMessage("El nombre no puede exceder los 20 caracteres.");
            RuleFor(u => u.Apellido)
                .NotEmpty().WithMessage("El apellido es requerido.")
                .MaximumLength(20).WithMessage("El apellido no puede exceder los 20 caracteres.");
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("El email es requerido.")
                .EmailAddress().WithMessage("El email no es válido.");
        }
    }
}
