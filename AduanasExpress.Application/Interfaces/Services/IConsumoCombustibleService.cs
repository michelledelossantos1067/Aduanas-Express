using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.DTOs.Vehiculo;

namespace AduanasExpress.Application.Interfaces.Services;
public interface IConsumoCombustibleService{
    public Task<List<ConsumoCombustibleReponseDTOs?>> ObtenerTodos();
    public Task<ConsumoCombustibleReponseDTOs?> ObtenerPorId(int Id);
    public Task Crear(CreateConsumoCombustibleDTOs createConsumoCombustibleDTOs);
    public Task Actualizar(int Id,UpdateConsumoCombustibleDTOs updateConsumoCombustibleDTOs);
    public Task Eliminar(int Id);
}