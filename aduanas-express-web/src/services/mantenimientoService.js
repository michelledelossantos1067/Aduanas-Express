import api from '../utils/axiosConfig'

export const crearMantenimiento = (data) => {
    return api.post('/Mantenimiento', data)
}
export const actualizarMantenimiento = (Id, data) => {
    return api.put(`/Mantenimiento/${Id}`, data)
}
export const eliminarMantenimiento = (Id) => {
    return api.delete(`/Mantenimiento/${Id}`)
}
export const verMantenimiento = () => {
    return api.get('/Mantenimiento')
}
export const verMantenimientoPorId = (Id) => {
    return api.get(`/Mantenimiento/${Id}`)
}
