export function formatFecha(f) {
    if (!f) return '—'
    return new Date(f).toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

export function formatHora(h) {
    if (!h) return ''
    return h.toString().substring(0, 5)
}

export function formatNumero(id) {
    return `#${String(id).padStart(4, '0')}`
}

export function iniciales(nombre, apellido) {
    return `${(nombre ?? '?')[0]}${(apellido ?? '?')[0]}`.toUpperCase()
}

export function estadoAsignacionLabel(a) {
    if (a.estado === 2) return 'Finalizada'
    if (a.estado === 3) return 'Cancelada'

    const fechaViaje = a.solicitud?.fechaViaje
    const horaSalida = a.solicitud?.horaSalida
    if (!fechaViaje || !horaSalida) return 'Pendiente'

    const inicio = new Date(`${fechaViaje.split('T')[0]}T${horaSalida}`)
    return new Date() >= inicio ? 'En curso' : 'Pendiente'
}

export function estadoAsignacionClase(a) {
    const label = estadoAsignacionLabel(a)
    return {
        'badge-pendiente': label === 'Pendiente',
        'badge-en-curso': label === 'En curso',
        'badge-finalizada': label === 'Finalizada',
        'badge-cancelada': label === 'Cancelada',
    }
}

export function puedeFinalizarse(a) {
    if (a.estado === 2 || a.estado === 3) return false
    return estadoAsignacionLabel(a) === 'En curso'
}

export function puedeCancelarse(a) {
    return a.estado !== 2 && a.estado !== 3
}

export const TABS_HISTORIAL = [
    { key: 'en-curso', label: 'En curso' },
    { key: 'pendientes', label: 'Pendientes' },
    { key: 'finalizadas', label: 'Finalizadas' },
    { key: 'canceladas', label: 'Canceladas' },
]
