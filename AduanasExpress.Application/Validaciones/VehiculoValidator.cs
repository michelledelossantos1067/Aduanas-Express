using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Domain.Entitis;
using FluentValidation;

namespace AduanasExpress.Application.Validaciones
{
    public  class VehiculoValidator: AbstractValidator<CreateVehiculoDTOs>
    {
        public VehiculoValidator()
        {
            RuleFor(v => v.Marca)
                .NotEmpty().WithMessage("La marca es requerida.")
                .MaximumLength(15).WithMessage("La marca no puede exceder los 15 caracteres.");
            RuleFor(v => v.Modelo)
                .NotEmpty().WithMessage("El modelo es requerido.")
                .MaximumLength(15).WithMessage("El modelo no puede exceder los 15 caracteres.");
            RuleFor(v => v.Año)
                .NotEmpty().WithMessage("El año es requerido.")
                .InclusiveBetween(1885, DateTime.Now.Year).WithMessage($"El año debe estar entre 1885 y {DateTime.Now.Year}."); 
            RuleFor(v => v.Matricula)
                .NotEmpty().WithMessage("matricula es requerida.")
                .MinimumLength(7).WithMessage("La placa debe tener 7 caracteres.")
                .MaximumLength(7).WithMessage("La placa debe tener 7 caracteres.");
            RuleFor(v => v.Color)
                .NotEmpty().WithMessage("El color es requerido.")
                .MaximumLength(10).WithMessage("El color no puede exceder los 10 caracteres.");
            RuleFor(v => v.Tipo)
                .NotEmpty().WithMessage("El tipo es requerido.")
                .MaximumLength(15).WithMessage("El tipo no puede exceder los 15 caracteres.");
            RuleFor(v => v.Capacidad)
                .NotEmpty().WithMessage("La capacidad es requerida.")
                .GreaterThan(0).WithMessage("La capacidad debe ser mayor que 0.");
            RuleFor(v => v.Estado)
                .IsInEnum().WithMessage("El estado no es válido.");
            RuleFor(v => v.Kilometraje)
                .NotEmpty().WithMessage("El kilometraje es requerido.")
                .GreaterThanOrEqualTo(0).WithMessage("El kilometraje debe ser mayor o igual a 0.");
            RuleFor(v => v.FechaUltimoMant)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha del último mantenimiento no es valida");
        }
}
}
