using AduanasExpress.Application.DTOs.Login;
using AduanasExpress.Domain.Entitis;
using FluentValidation;

namespace AduanasExpress.Application.Validaciones
{
    public class LoginValidator : AbstractValidator<AuthDTOs>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("El email es requerido.")
                .EmailAddress().WithMessage("El email no es válido.");
            RuleFor(l => l.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

        }
    }
    public class LoginValidatorChange : AbstractValidator<ChangePasswordDTO>
    {
        public LoginValidatorChange()
        {
            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("El email es requerido.")
                .EmailAddress().WithMessage("El email no es válido.");
            RuleFor(l => l.PasswordActual)
                .NotEmpty().WithMessage("La contraseña actual es requerida.")
                .MinimumLength(6).WithMessage("La contraseña actual debe tener al menos 6 caracteres.");
            RuleFor(l => l.PasswordNueva)
                .NotEmpty().WithMessage("La contraseña nueva es requerida.")
                .MinimumLength(6).WithMessage("La contraseña nueva debe tener al menos 6 caracteres.");
        }
    }
    public class LoginValidatorRegister : AbstractValidator<RegisterDTO>
    {
        public LoginValidatorRegister()
        {
            RuleFor(l => l.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(15).WithMessage("El nombre no puede exceder los 15 caracteres.");
            RuleFor(l => l.Apellido)
                .NotEmpty().WithMessage("El apellido es requerido.")
                .MaximumLength(15).WithMessage("El apellido no puede exceder los 15 caracteres.");
            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("El email es requerido.")
                .EmailAddress().WithMessage("El email no es válido.");
            RuleFor(l => l.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
            RuleFor(l => l.RolId)
               .IsInEnum().WithMessage("El rol no es válido.");
        }
    }
    public  class ResetPasswordValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("El email es requerido.")
                .EmailAddress().WithMessage("El email no es válido.");

        }
    }
}
