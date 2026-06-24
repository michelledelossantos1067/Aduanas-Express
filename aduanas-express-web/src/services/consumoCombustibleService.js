import api from '../utils/axiosConfig'

export const verConsumoPorId = (id) => {
    return api.get(`/ConsumoCombustible/${id}`)
}
export const crearConsumo = (data) => {
    return api.post('/ConsumoCombustible', data)
}
export const actualizarConsumo = (id, data) => {
    return api.put(`/ConsumoCombustible/${id}`, data)
}
export const eliminarConsumo = (id) => {
    return api.delete(`/ConsumoCombustible/${id}`)
}