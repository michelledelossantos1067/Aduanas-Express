using AduanasExpress.Application.DTOs.Vehiculo;

namespace AduanasExpress.Application.interfaces.Services;
public interface IVehiculoService{
    public Task<List<VehiculoResponseDTOs?>> ObtenerTodos();
    public Task<VehiculoResponseDTOs?> ObtenerPorId(int Id);
    public Task Crear(CreateVehiculoDTOs createVehiculoDTOs);
    public Task Actualizar(int Id,UpdateVehiculoDTOs updateVehiculoDTOs);
    public Task Eliminar(int Id);
    Task Desactivar(int id);
    Task Activar(int id);
    Task<List<VehiculoResponseDTOs>> ObtenerDisponiblesEnFecha(DateTime fecha);
}
