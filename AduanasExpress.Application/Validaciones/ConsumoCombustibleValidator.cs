using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Domain.Entitis;
using FluentValidation;


namespace AduanasExpress.Application.Validaciones
{
    public class ConsumoCombustibleValidator: AbstractValidator<CreateConsumoCombustibleDTOs>
    {
        public ConsumoCombustibleValidator() 
        {
            RuleFor(c => c.Galones)
                .NotEmpty().WithMessage("La cantidad de galones es requerida.")
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad de galones debe ser mayor que 0.");
            RuleFor(c => c.CostoPorGalon)
                .NotEmpty().WithMessage("El costo por galón es requerido.")
                .GreaterThan(0).WithMessage("El costo por galón debe ser mayor que 0.");
            RuleFor(c => c.CostoTotal)
                .NotEmpty().WithMessage("El costo total es requerido.")
                .GreaterThan(0).WithMessage("El costo total debe ser mayor que 0.");
            RuleFor(c => c.VehiculoId)
                .NotEmpty().WithMessage("El ID del vehículo es requerido.")
                .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor que 0.");

        }
    }
}
