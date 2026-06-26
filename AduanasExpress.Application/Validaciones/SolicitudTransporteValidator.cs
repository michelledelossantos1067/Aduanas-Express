using AduanasExpress.Application.DTOs.SolicitudTransporte;
using AduanasExpress.Domain.Entitis;
using FluentValidation;

namespace AduanasExpress.Application.Validaciones
{
    public class SolicitudTransporteValidator : AbstractValidator<CreateSolicitudTransporteDTOs>
    {
        public SolicitudTransporteValidator()
        {
            RuleFor(s => s.AreaSolicitante)
                .NotEmpty().WithMessage("El área solicitante es requerida.")
                .MaximumLength(50).WithMessage("El área solicitante no puede exceder los 50 caracteres.");
            RuleFor(s => s.CantidadColaboradores)
                .NotEmpty().WithMessage("La cantidad de colaboradores es requerida.")
                .GreaterThan(0).WithMessage("La cantidad de colaboradores debe ser mayor que 0.");
            RuleFor(s => s.FechaViaje)
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("La fecha del viaje no puede ser en el pasado.");
            RuleFor(s => s.HoraSalida)
                .NotEmpty().WithMessage("La hora de salida es requerida.");
            RuleFor(s => s.Destino)
                .NotEmpty().WithMessage("El destino es requerido.")
                .MaximumLength(100).WithMessage("El destino no puede exceder los 100 caracteres.");
            RuleFor(s => s.MotivoViaje)
                .NotEmpty().WithMessage("El motivo del viaje es requerido.")
                .MaximumLength(200).WithMessage("El motivo del viaje no puede exceder los 200 caracteres.");
            RuleFor(s => s.Estado)
                .IsInEnum().WithMessage("El estado no es válido.");
        }
    }
    public class SolicitudTransporteValidatorUpdate : AbstractValidator<UpdateSolicitudTransporteDTOs>
    {
        public SolicitudTransporteValidatorUpdate()
        {
            RuleFor(s => s.AreaSolicitante)
                .NotEmpty().WithMessage("El área solicitante es requerida.")
                .MaximumLength(50).WithMessage("El área solicitante no puede exceder los 50 caracteres.");
            RuleFor(s => s.CantidadColaboradores)
                .NotEmpty().WithMessage("La cantidad de colaboradores es requerida.")
                .GreaterThan(0).WithMessage("La cantidad de colaboradores debe ser mayor que 0.");
            RuleFor(s => s.FechaViaje)
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("La fecha del viaje no puede ser en el pasado.");
            RuleFor(s => s.HoraSalida)
                .NotEmpty().WithMessage("La hora de salida es requerida.");
            RuleFor(s => s.Destino)
                .NotEmpty().WithMessage("El destino es requerido.")
                .MaximumLength(255).WithMessage("El destino no puede exceder los 255 caracteres.");
            RuleFor(s => s.MotivoViaje)
                .NotEmpty().WithMessage("El motivo del viaje es requerido.")
                .MaximumLength(200).WithMessage("El motivo del viaje no puede exceder los 200 caracteres.");
            RuleFor(s => s.Estado)
                .IsInEnum().WithMessage("El estado no es válido.");
        }
    }
}
