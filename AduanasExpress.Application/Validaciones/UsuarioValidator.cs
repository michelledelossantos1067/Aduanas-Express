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
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
            RuleFor(u => u.Rol)
               .IsInEnum().WithMessage("El rol no es válido.");
        }
        
    }
    public class UsuarioValidatorUpdate : AbstractValidator<UpdateUsuario>
    {
        public UsuarioValidatorUpdate()
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
            RuleFor(u => u.Password)
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
            RuleFor(u => u.Rol)
               .IsInEnum().WithMessage("El rol no es válido.");
        }
    }
}
