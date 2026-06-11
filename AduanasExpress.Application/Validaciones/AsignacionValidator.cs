using AduanasExpress.Application.DTOs.Asignacion;
using AduanasExpress.Domain.Entitis;
using FluentValidation;

namespace AduanasExpress.Application.Validaciones
{
    public class AsignacionValidator : AbstractValidator<CreateAsignacionDTO>
    {
        public AsignacionValidator() 
        {
            RuleFor(a => a.SolicitudId)
                .NotEmpty().WithMessage("El ID de la solicitud es requerido.")
                .GreaterThan(0).WithMessage("El ID de la solicitud debe ser mayor que 0.");
            RuleFor(a => a.VehiculoId)
                .NotEmpty().WithMessage("El ID del vehículo es requerido.")
                .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor que 0.");
            RuleFor(a => a.ConductorId)
                .NotEmpty().WithMessage("El ID del conductor es requerido.")
                .GreaterThan(0).WithMessage("El ID del conductor debe ser mayor que 0.");
            RuleFor(a => a.FechaAsignacion)
                .NotEmpty().WithMessage("La fecha de asignación es requerida.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de asignación no es valida");
            RuleFor(a => a.AsignadoPorId)
               .NotEmpty().WithMessage("El ID del asignador es requerido.")
               .GreaterThan(0).WithMessage("El ID del asignador debe ser mayor que 0.");
        }
    }
}
