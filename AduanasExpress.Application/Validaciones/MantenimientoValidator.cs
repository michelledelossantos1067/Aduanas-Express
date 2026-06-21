using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Domain.Entitis;
using FluentValidation;

namespace AduanasExpress.Application.Validaciones
{
    public class MantenimientoValidator : AbstractValidator<CreateMantenimientoDTOs>
    {
        public MantenimientoValidator()
        {
            RuleFor(m => m.VehiculoId)
                .NotEmpty().WithMessage("El ID del vehículo es requerido.")
                .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor que 0.");

            RuleFor(m => m.Tipo)
                .NotEmpty().WithMessage("El tipo de mantenimiento es requerido.")
                .MaximumLength(20).WithMessage("El tipo de mantenimiento no puede exceder los 20 caracteres.");

            RuleFor(m => m.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");

            RuleFor(m => m.Estado)
                .NotEmpty().WithMessage("El estado es requerido.")
                .Must(e => new[] { "Programado", "En proceso", "Completado", "Cancelado" }.Contains(e))
                .WithMessage("El estado debe ser Programado, En proceso, Completado o Cancelado.");

            RuleFor(m => m.FechaProgramada)
                .NotEmpty().WithMessage("La fecha programada es requerida.");

            RuleFor(m => m.FechaRealizada)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha realizada no puede ser una fecha futura.")
                .When(m => m.FechaRealizada.HasValue);

            RuleFor(m => m.Costo)
                .GreaterThanOrEqualTo(0).WithMessage("El costo no puede ser negativo.");

            RuleFor(m => m.Taller)
                .MaximumLength(50).WithMessage("El taller no puede exceder los 50 caracteres.");
        }
    }

    public class MantenimientoValidatorUpdate : AbstractValidator<UpdateMantenimientoDTOs>
    {
        public MantenimientoValidatorUpdate()
        {
            RuleFor(m => m.VehiculoId)
                .NotEmpty().WithMessage("El ID del vehículo es requerido.")
                .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor que 0.");

            RuleFor(m => m.Tipo)
                .NotEmpty().WithMessage("El tipo de mantenimiento es requerido.")
                .MaximumLength(20).WithMessage("El tipo de mantenimiento no puede exceder los 20 caracteres.");

            RuleFor(m => m.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");

            RuleFor(m => m.Estado)
                .NotEmpty().WithMessage("El estado es requerido.")
                .Must(e => new[] { "Programado", "En proceso", "Completado", "Cancelado" }.Contains(e))
                .WithMessage("El estado debe ser Programado, En proceso, Completado o Cancelado.");

            RuleFor(m => m.FechaProgramada)
                .NotEmpty().WithMessage("La fecha programada es requerida.");

            RuleFor(m => m.FechaRealizada)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha realizada no puede ser una fecha futura.")
                .When(m => m.FechaRealizada.HasValue);

            RuleFor(m => m.Costo)
                .GreaterThanOrEqualTo(0).WithMessage("El costo no puede ser negativo.");

            RuleFor(m => m.Taller)
                .MaximumLength(50).WithMessage("El taller no puede exceder los 50 caracteres.");
        }
    }
}