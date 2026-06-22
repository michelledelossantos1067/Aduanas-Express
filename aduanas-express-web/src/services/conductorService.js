import api from '../utils/axiosConfig'

export const crearConductor = (data) => {
    return api.post('/conductor', data)
}
export const actualizarConductor = (Id, data) => {
    return api.put(`/conductor/${Id}`, data)
}
export const eliminarConductor = (Id) => {
    return api.delete(`/conductor/${Id}`)
}
export const verConductores = () => {
    return api.get('/conductor')
}
export const verConductorPorId = (Id) => {
    return api.get(`/conductor/${Id}`)
}
export const desactivarConductor = (id) => api.patch(`/conductor/${id}/desactivar`)
export const activarConductor = (id) => api.patch(`/conductor/${id}/activar`)