using AduanasExpress.Application.DTOs.Mantenimiento;

namespace AduanasExpress.Application.Interfaces.Services;
public interface IMantenimientoService{
    public Task<List<MantenimientoResponseDTOs?>> ObtenerTodos();
    public Task<MantenimientoResponseDTOs?> ObtenerPorId(int Id);
    public Task Crear(CreateMantenimientoDTOs createMantenimientoDTOs);
    public Task Actualizar(int Id,UpdateMantenimientoDTOs updateMantenimientoDTOs);
    public Task Eliminar(int Id);
}