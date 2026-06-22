import api from '../utils/axiosConfig'

export const crearVehiculo = (data) => {
    return api.post('/vehiculo', data)
}
export const actualizarVehiculo = (Id, data) => {
    return api.put(`/vehiculo/${Id}`, data)
}
export const eliminarVehiculo = (Id) => {
    return api.delete(`/vehiculo/${Id}`)
}
export const verVehiculos = () => {
    return api.get('/vehiculo')
}
export const verVehiculoPorId = (Id) => {
    return api.get(`/vehiculo/${Id}`)
}
export const desactivarVehiculo = (id) => api.patch(`/vehiculo/${id}/desactivar`)
export const activarVehiculo = (id) => api.patch(`/vehiculo/${id}/activar`)