import api from '../utils/axiosConfig'

export const crearAsignaciones = (data) => {
    return api.post('/Asignaciones', data)
}

export const actualizarAsignaciones = (Id, data) => {
    return api.put(`/Asignaciones/${Id}`, data)
}

export const eliminarAsignaciones = (Id) => {
    return api.delete(`/Asignaciones/${Id}`)
}

export const verAsignaciones = () => {
    return api.get('/Asignaciones')
}

export const verAsignacionesPorId = (Id) => {
    return api.get(`/Asignaciones/${Id}`)
}

export const finalizarAsignacion = (id) => {
    return api.post(`/Asignaciones/${id}/finalizar`)
}

export const cancelarAsignacion = (id, motivo, usuarioId) => {
    return api.post(`/Asignaciones/${id}/cancelar`, { motivo, usuarioId })
}

export const obtenerDisponibles = (solicitudId) => {
    return api.get('/Asignaciones/disponibles', { params: { solicitudId } })
}