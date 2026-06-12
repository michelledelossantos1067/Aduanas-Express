using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Domain.Entitis;
using FluentValidation;

namespace AduanasExpress.Application.Validaciones
{
    public class ConductorValidator : AbstractValidator<CreateConductorDTOs>
    {
        public ConductorValidator()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(15).WithMessage("El nombre no puede exceder los 15 caracteres.");
            RuleFor(c => c.Apellido)
                .NotEmpty().WithMessage("El apellido es requerido.")
                .MaximumLength(15).WithMessage("El apellido no puede exceder los 15 caracteres.");
            RuleFor(c => c.Cedula)
                .NotEmpty().WithMessage("La cédula es requerida.")
                .MinimumLength(11).WithMessage("Digito incompleto")
                .MaximumLength(11).WithMessage("Cédula no valida");
            RuleFor(c => c.NumeroLicencia)
                .NotEmpty().WithMessage("El número de licencia es requerido.")
                .MinimumLength(10).WithMessage("El número de licencia debe tener al menos 10 caracteres.")
                .MaximumLength(20).WithMessage("El número de licencia no puede exceder los 20 caracteres.");
            RuleFor(c => c.TipoLicencia)
                .NotEmpty().WithMessage("El tipo de licencia es requerido.")
                .MaximumLength(10).WithMessage("El tipo de licencia no puede exceder los 10 caracteres.");
            RuleFor(c => c.FechaVencLicencia)
                .NotEmpty().WithMessage("La fecha de vencimiento de la licencia es requerida.")
                .GreaterThan(DateTime.Now).WithMessage("La fecha de vencimiento no es valida.");
            RuleFor(c => c.Telefono)
                .NotEmpty().WithMessage("El teléfono es requerido.")
                .MinimumLength(10).WithMessage("El teléfono debe tener al menos 10 caracteres.")
                .MaximumLength(15).WithMessage("El teléfono no puede exceder los 15 caracteres.");
            RuleFor(c => c.Direccion)
                .NotEmpty().WithMessage("La dirección es requerida.")
                .MaximumLength(50).WithMessage("La dirección no puede exceder los 50 caracteres.");
            RuleFor(c => c.SupervisorId)
                .NotEmpty().WithMessage("El ID del supervisor es requerido.")
                .GreaterThan(0).WithMessage("El ID del supervisor debe ser mayor que 0.");
            RuleFor(c => c.Estado)
               .IsInEnum().WithMessage("El estado no es válido.");
       
        }
       
    }
    public class ConductorValidatorUpdate : AbstractValidator<UpdateConductorDTOs>
    {
        public ConductorValidatorUpdate()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(15).WithMessage("El nombre no puede exceder los 15 caracteres.");
            RuleFor(c => c.Apellido)
                .NotEmpty().WithMessage("El apellido es requerido.")
                .MaximumLength(15).WithMessage("El apellido no puede exceder los 15 caracteres.");
            RuleFor(c => c.Cedula)
                .NotEmpty().WithMessage("La cédula es requerida.")
                .MinimumLength(11).WithMessage("Digito incompleto")
                .MaximumLength(11).WithMessage("Cédula no valida");
            RuleFor(c => c.NumeroLicencia)
                .NotEmpty().WithMessage("El número de licencia es requerido.")
                .MinimumLength(10).WithMessage("El número de licencia debe tener al menos 10 caracteres.")
                .MaximumLength(20).WithMessage("El número de licencia no puede exceder los 20 caracteres.");
            RuleFor(c => c.TipoLicencia)
                .NotEmpty().WithMessage("El tipo de licencia es requerido.")
                .MaximumLength(10).WithMessage("El tipo de licencia no puede exceder los 10 caracteres.");
            RuleFor(c => c.FechaVencLicencia)
                .NotEmpty().WithMessage("La fecha de vencimiento de la licencia es requerida.")
                .GreaterThan(DateTime.Now).WithMessage("La fecha de vencimiento no es valida.");
            RuleFor(c => c.Telefono)
                .NotEmpty().WithMessage("El teléfono es requerido.")
                .MinimumLength(10).WithMessage("El teléfono debe tener al menos 10 caracteres.")
                .MaximumLength(15).WithMessage("El teléfono no puede exceder los 15 caracteres.");
            RuleFor(c => c.Direccion)
                .NotEmpty().WithMessage("La dirección es requerida.")
                .MaximumLength(50).WithMessage("La dirección no puede exceder los 50 caracteres.");
            RuleFor(c => c.SupervisorId)
                .NotEmpty().WithMessage("El ID del supervisor es requerido.")
                .GreaterThan(0).WithMessage("El ID del supervisor debe ser mayor que 0.");
            RuleFor(c => c.Estado).IsInEnum().WithMessage("El estado no es válido.");
        }
    }
}
