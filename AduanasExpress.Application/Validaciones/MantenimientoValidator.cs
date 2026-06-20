using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Domain.Entitis;
using FluentValidation;

namespace AduanasExpress.Application.Validaciones
{
    public class MantenimientoValidator : AbstractValidator<CreateMantenimientoDTOs>
    {
        public MantenimientoValidator()
        {
            RuleFor(m => m.TipoMantenimiento)
                .NotEmpty().WithMessage("El tipo de mantenimiento es requerido.")
                .MaximumLength(20).WithMessage("El tipo de mantenimiento no puede exceder los 20 caracteres.");
            RuleFor(m => m.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");
            RuleFor(m => m.Costo)
                .NotEmpty().WithMessage("El costo es requerido.")
                .GreaterThan(0).WithMessage("El costo debe ser mayor que 0.");
            RuleFor(m => m.Taller)
                .NotEmpty().WithMessage("El taller es requerido.")
                .MaximumLength(50).WithMessage("El taller no puede exceder los 50 caracteres.");
            RuleFor(m => m.ProximoMantenimiento)
                .GreaterThan(DateTime.Now).WithMessage("La fecha del próximo mantenimiento no es valida");
            RuleFor(m => m.VehiculoId)
                .NotEmpty().WithMessage("El ID del vehículo es requerido.")
                .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor que 0.");

        }
    }
    public class MantenimientoValidatorUpdate : AbstractValidator<UpdateMantenimientoDTOs>
    {
        public MantenimientoValidatorUpdate()
        {
            RuleFor(m => m.TipoMantenimiento)
                .NotEmpty().WithMessage("El tipo de mantenimiento es requerido.")
                .MaximumLength(20).WithMessage("El tipo de mantenimiento no puede exceder los 20 caracteres.");
            RuleFor(m => m.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");
            RuleFor(m => m.Costo)
                .NotEmpty().WithMessage("El costo es requerido.")
                .GreaterThan(0).WithMessage("El costo debe ser mayor que 0.");
            RuleFor(m => m.Taller)
                .NotEmpty().WithMessage("El taller es requerido.")
                .MaximumLength(50).WithMessage("El taller no puede exceder los 50 caracteres.");
            RuleFor(m => m.ProximoMantenimiento)
                .GreaterThan(DateTime.Now).WithMessage("La fecha del próximo mantenimiento no es valida");
            RuleFor(m => m.VehiculoId)
                .NotEmpty().WithMessage("El ID del vehículo es requerido.")
                .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor que 0.");
        }
    }
}
