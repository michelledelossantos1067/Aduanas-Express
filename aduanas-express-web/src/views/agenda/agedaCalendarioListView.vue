<script setup>
import { ref, computed, onMounted } from 'vue'
import { verSolicitud } from '@/services/solicitudService'
import { verAsignaciones } from '@/services/asignacionService'
import { usePermisos } from '@/composables/usePermisos'
const { puede } = usePermisos()

const vistaActiva = ref('mes')
const hoy = new Date()
const fechaActual = ref(new Date(hoy.getFullYear(), hoy.getMonth(), 1))

const filtros = ref({ vehiculos: false, conductores: false, pendientes: false, cancelados: false })

function toggleFiltro(key) {
    filtros.value[key] = !filtros.value[key]
}

const viajes = ref([])
const cargando = ref(true)
const errorCarga = ref(null)

// Estados para los nuevos cuadros de diálogo (Modales)
const fechaSeleccionada = ref(null)
const viajeSeleccionado = ref(null)

const MAP_ESTADO_SOLICITUD = {
    0: 'pendiente',
    1: 'programado',
    2: 'cancelado',
    3: 'cancelado',
    4: 'programado',
}

function mapearEstadoSolicitud(estado) {
    return MAP_ESTADO_SOLICITUD[estado] ?? 'programado'
}

async function cargarDatos() {
    cargando.value = true
    errorCarga.value = null
    try {
        const [resSolicitudes, resAsignaciones] = await Promise.all([
            verSolicitud(),
            verAsignaciones(),
        ])

        const asignacionPorSolicitudId = new Map(
            resAsignaciones.data.map(a => [a.solicitudId, a])
        )

        viajes.value = resSolicitudes.data.map(s => {
            const asignacion = asignacionPorSolicitudId.get(s.id)
            const vehiculo = asignacion?.vehiculo
            const conductor = asignacion?.conductor
            const [h, m] = (s.horaSalida ?? '00:00:00').split(':')
            const horaInicio = `${h}:${m}`
            const horaFin = `${String((Number(h) + 1) % 24).padStart(2, '0')}:${m}`

            return {
                id: s.id,
                titulo: s.motivoViaje || s.destino,
                fecha: new Date(s.fechaViaje),
                horaInicio,
                horaFin,
                vehiculo: vehiculo ? `${vehiculo.marca} ${vehiculo.modelo}` : 'Sin asignar',
                placa: vehiculo ? vehiculo.matricula : '—',
                conductor: conductor ? `${conductor.nombre} ${conductor.apellido?.charAt(0) ?? ''}.` : 'Sin asignar',
                estado: mapearEstadoSolicitud(s.estado),
                tipo: s.estado === 0 ? 'urgente' : 'normal',
                destinoCompleto: s.destino || 'No especificado',
                motivoCompleto: s.motivoViaje || 'No especificado'
            }
        })
    } catch (err) {
        errorCarga.value = 'No se pudieron cargar los datos del calendario.'
        console.error("Error al cargar datos:", err)
    } finally {
        cargando.value = false
    }
}

onMounted(cargarDatos)

function mesAnterior() {
    fechaActual.value = new Date(fechaActual.value.getFullYear(), fechaActual.value.getMonth() - 1, 1)
}
function mesSiguiente() {
    fechaActual.value = new Date(fechaActual.value.getFullYear(), fechaActual.value.getMonth() + 1, 1)
}
function irAHoy() {
    fechaActual.value = new Date(hoy.getFullYear(), hoy.getMonth(), 1)
    mostrarPicker.value = false
}

const mostrarPicker = ref(false)
const vistaPickerAño = ref(false)
const pickerAño = ref(fechaActual.value.getFullYear())

const MESES_CORTOS = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic']

function seleccionarMesPicker(mes) {
    fechaActual.value = new Date(pickerAño.value, mes, 1)
    mostrarPicker.value = false
    vistaPickerAño.value = false
}

function toggleVistaAño() {
    vistaPickerAño.value = !vistaPickerAño.value
}

function añoAnteriorPicker() { pickerAño.value-- }
function añoSiguientePicker() { pickerAño.value++ }

function seleccionarAñoPicker(año) {
    pickerAño.value = año
    vistaPickerAño.value = false
}

const añosRango = computed(() => {
    const base = Math.floor(pickerAño.value / 12) * 12
    return Array.from({ length: 12 }, (_, i) => base + i)
})

const MESES = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre']
const DIAS = ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb']

function nombreMes(fecha) {
    return `${MESES[fecha.getMonth()]} ${fecha.getFullYear()}`
}

// Funciones para manejar la apertura de cuadros de diálogo
function seleccionarFecha(fecha) {
    const viajesDelDia = viajesEnFecha(fecha)
    if (viajesDelDia.length > 0) {
        fechaSeleccionada.value = fecha
    }
}

function seleccionarViaje(viaje) {
    viajeSeleccionado.value = viaje
}

function esMismaFecha(a, b) {
    return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate()
}

function esMesActual(fecha) {
    return fecha.getMonth() === fechaActual.value.getMonth() && fecha.getFullYear() === fechaActual.value.getFullYear()
}

function formatHora(h) { return h?.substring(0, 5) ?? '' }

function formatFechaCorta(d) {
    if (!d) return ''
    return d.toLocaleDateString('es-DO', { day: '2-digit', month: 'long', year: 'numeric' })
}

const celdasMes = computed(() => {
    const año = fechaActual.value.getFullYear()
    const mes = fechaActual.value.getMonth()
    const primero = new Date(año, mes, 1)
    const ultimo = new Date(año, mes + 1, 0)
    const celdas = []

    for (let i = 0; i < primero.getDay(); i++) {
        celdas.push(new Date(año, mes, -primero.getDay() + i + 1))
    }
    for (let d = 1; d <= ultimo.getDate(); d++) {
        celdas.push(new Date(año, mes, d))
    }
    while (celdas.length % 7 !== 0) {
        celdas.push(new Date(año, mes + 1, celdas.length - ultimo.getDate() - primero.getDay() + 1))
    }
    return celdas
})

function viajesEnFecha(fecha) {
    if (!fecha) return []
    return viajes.value.filter(v => esMismaFecha(v.fecha, fecha))
}

const resumen = computed(() => {
    const mes = viajes.value.filter(v =>
        v.fecha.getMonth() === fechaActual.value.getMonth() &&
        v.fecha.getFullYear() === fechaActual.value.getFullYear()
    )
    return {
        total: mes.length,
        completados: mes.filter(v => v.estado === 'programado').length,
        pendientes: mes.filter(v => v.estado === 'pendiente' || v.estado === 'espera').length,
        cancelados: mes.filter(v => v.estado === 'cancelado').length,
    }
})

const viajesHoy = computed(() => viajes.value.filter(v => esMismaFecha(v.fecha, hoy)))

const proximosViajes = computed(() =>
    viajes.value
        .filter(v => v.fecha > hoy && !esMismaFecha(v.fecha, hoy))
        .sort((a, b) => a.fecha - b.fecha)
        .slice(0, 6)
)

function chipClase(viaje) {
    if (viaje.tipo === 'urgente') return 'chip-urgente'
    if (viaje.estado === 'en_viaje') return 'chip-en-viaje'
    return 'chip-normal'
}

function bordeClase(estado) {
    const map = {
        en_viaje: 'borde-en-viaje',
        programado: 'borde-programado',
        pendiente: 'borde-pendiente',
        espera: 'borde-espera',
        cancelado: 'borde-cancelado',
    }
    return map[estado] ?? 'borde-programado'
}

function badgeEstado(estado) {
    const map = {
        en_viaje: { label: 'En viaje', clase: 'badge-en-viaje-pill' },
        programado: { label: 'Programado', clase: 'badge-programado-pill' },
        pendiente: { label: 'Pendiente', clase: 'badge-pendiente-pill' },
        espera: { label: 'Espera', clase: 'badge-espera-pill' },
        cancelado: { label: 'Cancelado', clase: 'badge-cancelado-pill' },
    }
    return map[estado] ?? { label: estado, clase: '' }
}
</script>

<template>
    <div class="agenda-page" @click.self="mostrarPicker = false">

        <div class="agenda-header">
            <div>
                <h1 class="agenda-title">Agenda y Calendario</h1>
                <p class="agenda-sub">Sistema de transporte institucional</p>
            </div>
            <div class="vista-tabs">
                <button class="tab-btn" :class="{ 'tab-activo': vistaActiva === 'dia' }"
                    @click="vistaActiva = 'dia'">Día</button>
                <button class="tab-btn" :class="{ 'tab-activo': vistaActiva === 'semana' }"
                    @click="vistaActiva = 'semana'">Semana</button>
                <button class="tab-btn" :class="{ 'tab-activo': vistaActiva === 'mes' }"
                    @click="vistaActiva = 'mes'">Mes</button>
            </div>
        </div>

        <div v-if="cargando" class="estado-carga">Cargando agenda...</div>
        <div v-else-if="errorCarga" class="estado-error">{{ errorCarga }}</div>

        <div v-else class="agenda-layout">
            <div class="cal-panel">

                <div class="cal-nav">
                    <button class="nav-btn" @click="mesAnterior">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                            stroke-width="2.5">
                            <polyline points="15 18 9 12 15 6" />
                        </svg>
                    </button>

                    <div class="picker-wrapper">
                        <button class="cal-mes-label-btn"
                            @click.stop="mostrarPicker = !mostrarPicker; pickerAño = fechaActual.getFullYear(); vistaPickerAño = false">
                            {{ nombreMes(fechaActual) }}
                            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                stroke-width="2.5">
                                <polyline points="6 9 12 15 18 9" />
                            </svg>
                        </button>

                        <div v-if="mostrarPicker" class="picker-dropdown" @click.stop>

                            <template v-if="!vistaPickerAño">
                                <div class="picker-nav">
                                    <button class="picker-nav-btn" @click="añoAnteriorPicker">
                                        <svg width="12" height="12" viewBox="0 0 24 24" fill="none"
                                            stroke="currentColor" stroke-width="2.5">
                                            <polyline points="15 18 9 12 15 6" />
                                        </svg>
                                    </button>
                                    <button class="picker-año-btn" @click="toggleVistaAño">{{ pickerAño }}</button>
                                    <button class="picker-nav-btn" @click="añoSiguientePicker">
                                        <svg width="12" height="12" viewBox="0 0 24 24" fill="none"
                                            stroke="currentColor" stroke-width="2.5">
                                            <polyline points="9 18 15 12 9 6" />
                                        </svg>
                                    </button>
                                </div>
                                <div class="picker-meses-grid">
                                    <button v-for="(mes, i) in MESES_CORTOS" :key="i" class="picker-mes-btn" :class="{
                                        'picker-mes-activo': i === fechaActual.getMonth() && pickerAño === fechaActual.getFullYear(),
                                        'picker-mes-hoy': i === hoy.getMonth() && pickerAño === hoy.getFullYear()
                                    }" @click="seleccionarMesPicker(i)">{{ mes }}</button>
                                </div>
                            </template>

                            <template v-else>
                                <div class="picker-nav">
                                    <button class="picker-nav-btn" @click="pickerAño -= 12">
                                        <svg width="12" height="12" viewBox="0 0 24 24" fill="none"
                                            stroke="currentColor" stroke-width="2.5">
                                            <polyline points="15 18 9 12 15 6" />
                                        </svg>
                                    </button>
                                    <span class="picker-rango-label">{{ añosRango[0] }} – {{ añosRango[11] }}</span>
                                    <button class="picker-nav-btn" @click="pickerAño += 12">
                                        <svg width="12" height="12" viewBox="0 0 24 24" fill="none"
                                            stroke="currentColor" stroke-width="2.5">
                                            <polyline points="9 18 15 12 9 6" />
                                        </svg>
                                    </button>
                                </div>
                                <div class="picker-meses-grid">
                                    <button v-for="año in añosRango" :key="año" class="picker-mes-btn" :class="{
                                        'picker-mes-activo': año === fechaActual.getFullYear(),
                                        'picker-mes-hoy': año === hoy.getFullYear()
                                    }" @click="seleccionarAñoPicker(año)">{{ año }}</button>
                                </div>
                            </template>

                        </div>
                    </div>

                    <button class="nav-btn" @click="mesSiguiente">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                            stroke-width="2.5">
                            <polyline points="9 18 15 12 9 6" />
                        </svg>
                    </button>
                    <button class="btn-hoy" @click="irAHoy">Hoy</button>
                </div>

                <div class="filtros-row">
                    <button class="filtro-btn filtro-vehiculos" :class="{ activo: filtros.vehiculos }"
                        @click="toggleFiltro('vehiculos')">Vehículos</button>
                    <button class="filtro-btn filtro-conductores" :class="{ activo: filtros.conductores }"
                        @click="toggleFiltro('conductores')">Conductores</button>
                    <button class="filtro-btn filtro-pendientes" :class="{ activo: filtros.pendientes }"
                        @click="toggleFiltro('pendientes')">Pendientes</button>
                    <button class="filtro-btn filtro-cancelados" :class="{ activo: filtros.cancelados }"
                        @click="toggleFiltro('cancelados')">Cancelados</button>
                </div>

                <div class="cal-grid">
                    <div class="cal-day-header" v-for="dia in DIAS" :key="dia">{{ dia }}</div>
                    <div v-for="(fecha, i) in celdasMes" :key="i" class="cal-celda" :class="{
                        'celda-otro-mes': !esMesActual(fecha),
                        'celda-hoy': esMismaFecha(fecha, hoy),
                        'celda-con-viajes': viajesEnFecha(fecha).length > 0
                    }" @click="seleccionarFecha(fecha)">
                        <span class="celda-numero">{{ fecha.getDate() }}</span>
                        <div class="celda-chips">
                            <div v-for="viaje in viajesEnFecha(fecha).slice(0, 2)" :key="viaje.id" class="cal-chip"
                                :class="chipClase(viaje)" :title="viaje.titulo">
                                {{ formatHora(viaje.horaInicio) }} {{ viaje.titulo.substring(0, 10) }}
                            </div>
                            <div v-if="viajesEnFecha(fecha).length > 2" class="chip-mas">
                                +{{ viajesEnFecha(fecha).length - 2 }} más
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="side-panel">
                <div class="side-section">
                    <h3 class="side-titulo">Resumen del mes</h3>
                    <div class="resumen-grid">
                        <div class="resumen-card">
                            <span class="resumen-num azul">{{ resumen.total }}</span>
                            <span class="resumen-label">Viajes</span>
                        </div>
                        <div class="resumen-card">
                            <span class="resumen-num verde">{{ resumen.completados }}</span>
                            <span class="resumen-label">Completados</span>
                        </div>
                        <div class="resumen-card">
                            <span class="resumen-num rojo">{{ resumen.pendientes }}</span>
                            <span class="resumen-label">Pendientes</span>
                        </div>
                        <div class="resumen-card">
                            <span class="resumen-num gris">{{ resumen.cancelados }}</span>
                            <span class="resumen-label">Cancelados</span>
                        </div>
                    </div>
                </div>

                <div class="side-section" v-if="viajesHoy.length > 0">
                    <h3 class="side-titulo">Viajes para hoy &mdash; {{ formatFechaCorta(hoy) }}</h3>
                    <div class="viajes-lista">
                        <div v-for="viaje in viajesHoy" :key="viaje.id" class="viaje-card clickable"
                            :class="bordeClase(viaje.estado)" @click="seleccionarViaje(viaje)">
                            <div class="viaje-hora">{{ viaje.horaInicio }} - {{ viaje.horaFin }}</div>
                            <div class="viaje-titulo">{{ viaje.titulo }}</div>
                            <div class="viaje-recurso">{{ viaje.vehiculo }} · {{ viaje.placa }} / {{ viaje.conductor }}
                            </div>
                            <span class="badge-pill" :class="badgeEstado(viaje.estado).clase">{{
                                badgeEstado(viaje.estado).label
                                }}</span>
                        </div>
                    </div>
                </div>

                <div class="side-section" v-if="proximosViajes.length > 0">
                    <h3 class="side-titulo">Próximos Viajes</h3>
                    <div class="viajes-lista">
                        <div v-for="viaje in proximosViajes" :key="viaje.id" class="viaje-card clickable"
                            :class="bordeClase(viaje.estado)" @click="seleccionarViaje(viaje)">
                            <div class="viaje-hora">
                                {{ viaje.fecha.getDate() }} {{ MESES[viaje.fecha.getMonth()].substring(0, 3) }}
                                &mdash; {{ viaje.horaInicio }}
                            </div>
                            <div class="viaje-titulo">{{ viaje.titulo }}</div>
                            <div class="viaje-recurso">{{ viaje.vehiculo }} · {{ viaje.placa }} / {{ viaje.conductor }}
                            </div>
                            <span class="badge-pill" :class="badgeEstado(viaje.estado).clase">{{
                                badgeEstado(viaje.estado).label
                                }}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div v-if="fechaSeleccionada" class="modal-overlay" @click="fechaSeleccionada = null">
            <div class="modal-content modal-lista" @click.stop>
                <div class="modal-header-box">
                    <div>
                        <h3 class="modal-main-title">Viajes Programados</h3>
                        <p class="modal-subtitle">{{ formatFechaCorta(fechaSeleccionada) }}</p>
                    </div>
                    <button class="modal-close-btn" @click="fechaSeleccionada = null">&times;</button>
                </div>
                <div class="modal-body-scroll">
                    <div class="viajes-lista-modal">
                        <div v-for="viaje in viajesEnFecha(fechaSeleccionada)" :key="viaje.id" 
                            class="viaje-modal-row" :class="bordeClase(viaje.estado)"
                            @click="seleccionarViaje(viaje)">
                            <div class="viaje-modal-meta">
                                <span class="v-modal-time">{{ viaje.horaInicio }}</span>
                                <span class="badge-pill" :class="badgeEstado(viaje.estado).clase">
                                    {{ badgeEstado(viaje.estado).label }}
                                </span>
                            </div>
                            <div class="v-modal-title">{{ viaje.titulo }}</div>
                            <div class="v-modal-subtext">{{ viaje.conductor }} · {{ viaje.vehiculo }}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div v-if="viajeSeleccionado" class="modal-overlay z-top" @click="viajeSeleccionado = null">
            <div class="modal-content modal-detalle" @click.stop>
                <div class="modal-header-box border-b">
                    <div>
                        <span class="badge-pill mb-2" :class="badgeEstado(viajeSeleccionado.estado).clase">
                            {{ badgeEstado(viajeSeleccionado.estado).label }}
                        </span>
                        <h3 class="modal-main-title text-xl">{{ viajeSeleccionado.titulo }}</h3>
                    </div>
                    <button class="modal-close-btn" @click="viajeSeleccionado = null">&times;</button>
                </div>
                <div class="modal-body-detail">
                    <div class="detail-grid">
                        <div class="detail-item">
                            <span class="detail-label">Fecha</span>
                            <span class="detail-value">{{ formatFechaCorta(viajeSeleccionado.fecha) }}</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Horario</span>
                            <span class="detail-value">{{ viajeSeleccionado.horaInicio }} hs a {{ viajeSeleccionado.horaFin }} hs</span>
                        </div>
                        <div class="detail-item full-w">
                            <span class="detail-label">Destino</span>
                            <span class="detail-value highlight">{{ viajeSeleccionado.destinoCompleto }}</span>
                        </div>
                        <div class="detail-divider">RECURSOS ASIGNADOS</div>
                        <div class="detail-item">
                            <span class="detail-label">Conductor encargado</span>
                            <span class="detail-value font-semibold">{{ viajeSeleccionado.conductor }}</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Vehículo institucional</span>
                            <span class="detail-value font-semibold">{{ viajeSeleccionado.vehiculo }}</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Número de Placa / Matrícula</span>
                            <span class="detail-value code-style">{{ viajeSeleccionado.placa }}</span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Prioridad del servicio</span>
                            <span class="detail-value capitalize">{{ viajeSeleccionado.tipo }}</span>
                        </div>
                    </div>
                </div>
                <div class="modal-footer-box">
                    <button class="btn-modal-action" @click="viajeSeleccionado = null">Entendido</button>
                </div>
            </div>
        </div>

    </div>
</template>

<style scoped>
.agenda-page {
    padding: 28px 32px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

.estado-carga,
.estado-error {
    text-align: center;
    padding: 60px 20px;
    font-size: .95rem;
    color: #6b7280;
    background: #fff;
    border-radius: 14px;
}

.estado-error {
    color: #dc2626;
}

.agenda-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20px;
}

.agenda-title {
    font-size: 1.45rem;
    font-weight: 700;
    color: #111827;
    letter-spacing: -0.02em;
    margin: 0 0 2px;
}

.agenda-sub {
    font-size: .8rem;
    color: #9ca3af;
    margin: 0;
}

.vista-tabs {
    display: flex;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    overflow: hidden;
}

.tab-btn {
    padding: 8px 22px;
    border: none;
    background: transparent;
    font-size: .875rem;
    font-weight: 600;
    color: #6b7280;
    cursor: pointer;
    transition: all .15s;
}

.tab-btn:hover {
    background: #f9fafb;
    color: #374151;
}

.tab-activo {
    background: #1a3a2a !important;
    color: #fff !important;
}

.agenda-layout {
    display: grid;
    grid-template-columns: 1fr 340px;
    gap: 16px;
    align-items: start;
}

.cal-panel {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    overflow: hidden;
}

.cal-nav {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 16px 20px 12px;
    border-bottom: 1px solid #f3f4f6;
}

.nav-btn {
    width: 30px;
    height: 30px;
    display: flex;
    align-items: center;
    justify-content: center;
    border: 1.5px solid #e5e7eb;
    border-radius: 7px;
    background: #fff;
    cursor: pointer;
    color: #374151;
    transition: background .15s;
}

.nav-btn:hover {
    background: #f3f4f6;
}

.picker-wrapper {
    position: relative;
    flex: 1;
}

.cal-mes-label-btn {
    display: flex;
    align-items: center;
    gap: 4px;
    background: transparent;
    border: none;
    font-size: .95rem;
    font-weight: 700;
    color: #111827;
    cursor: pointer;
    padding: 4px 8px;
    border-radius: 7px;
    transition: background .15s;
}

.cal-mes-label-btn:hover {
    background: #f3f4f6;
}

.picker-dropdown {
    position: absolute;
    top: 38px;
    left: 0;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 12px;
    padding: 12px;
    box-shadow: 0 8px 24px rgba(0, 0, 0, .12);
    z-index: 200;
    width: 220px;
}

.picker-nav {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 10px;
}

.picker-nav-btn {
    width: 26px;
    height: 26px;
    display: flex;
    align-items: center;
    justify-content: center;
    border: 1.5px solid #e5e7eb;
    border-radius: 6px;
    background: #fff;
    cursor: pointer;
    color: #374151;
    transition: background .15s;
}

.picker-nav-btn:hover {
    background: #f3f4f6;
}

.picker-año-btn {
    font-size: .9rem;
    font-weight: 700;
    color: #111827;
    background: transparent;
    border: none;
    cursor: pointer;
    padding: 4px 10px;
    border-radius: 6px;
    transition: background .15s;
}

.picker-año-btn:hover {
    background: #f3f4f6;
}

.picker-rango-label {
    font-size: .85rem;
    font-weight: 700;
    color: #111827;
}

.picker-meses-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 4px;
}

.picker-mes-btn {
    padding: 8px 4px;
    border: none;
    border-radius: 7px;
    font-size: .8rem;
    font-weight: 600;
    color: #374151;
    background: transparent;
    cursor: pointer;
    transition: background .15s;
    text-align: center;
}

.picker-mes-btn:hover {
    background: #f3f4f6;
}

.picker-mes-activo {
    background: #1a3a2a !important;
    color: #fff !important;
}

.picker-mes-hoy {
    color: #1a3a2a;
    font-weight: 800;
}

.btn-hoy {
    padding: 6px 16px;
    background: #1a3a2a;
    border: none;
    border-radius: 7px;
    font-size: .8rem;
    font-weight: 700;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}

.btn-hoy:hover {
    background: #14532d;
}

.filtros-row {
    display: flex;
    gap: 8px;
    padding: 10px 20px 12px;
    flex-wrap: wrap;
}

.filtro-btn {
    padding: 4px 14px;
    border-radius: 20px;
    font-size: .75rem;
    font-weight: 600;
    cursor: pointer;
    border: 1.5px solid transparent;
    background: transparent;
    transition: all .15s;
}

.filtro-vehiculos {
    border-color: #93c5fd;
    color: #1e40af;
}

.filtro-vehiculos.activo {
    background: #dbeafe;
}

.filtro-conductores {
    border-color: #6ee7b7;
    color: #065f46;
}

.filtro-conductores.activo {
    background: #d1fae5;
}

.filtro-pendientes {
    border-color: #fca5a5;
    color: #991b1b;
}

.filtro-pendientes.activo {
    background: #fee2e2;
}

.filtro-cancelados {
    border-color: #d1d5db;
    color: #374151;
}

.filtro-cancelados.activo {
    background: #f3f4f6;
}

.cal-grid {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    border-top: 1px solid #f3f4f6;
}

.cal-day-header {
    padding: 10px 0;
    text-align: center;
    font-size: .72rem;
    font-weight: 700;
    color: #fff;
    background: #1a3a2a;
    letter-spacing: .05em;
}

.cal-celda {
    min-height: 88px;
    padding: 6px 6px 4px;
    border-right: 1px solid #f3f4f6;
    border-bottom: 1px solid #f3f4f6;
    vertical-align: top;
    position: relative;
    transition: background-color 0.1s;
}

.cal-celda:nth-child(7n) {
    border-right: none;
}

.celda-con-viajes {
    cursor: pointer;
}

.celda-con-viajes:hover {
    background-color: #f9fafb;
}

.celda-numero {
    display: block;
    font-size: .8rem;
    font-weight: 600;
    color: #374151;
    margin-bottom: 4px;
}

.celda-otro-mes .celda-numero {
    color: #d1d5db;
}

.celda-hoy {
    background: #f0fdf4;
}

.celda-hoy:hover {
    background-color: #dcfee7;
}

.celda-hoy .celda-numero {
    background: #1a3a2a;
    color: #fff;
    width: 22px;
    height: 22px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: .75rem;
}

.celda-chips {
    display: flex;
    flex-direction: column;
    gap: 2px;
    pointer-events: none; /* Hace que el clic se registre directamente en la celda entera */
}

.cal-chip {
    font-size: .68rem;
    font-weight: 600;
    padding: 2px 6px;
    border-radius: 4px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.chip-normal {
    background: #a7f3d0;
    color: #065f46;
}

.chip-en-viaje {
    background: #bfdbfe;
    color: #1e40af;
}

.chip-urgente {
    background: #fecaca;
    color: #991b1b;
}

.chip-mas {
    font-size: .65rem;
    color: #9ca3af;
    padding: 1px 4px;
    font-weight: 600;
}

.side-panel {
    display: flex;
    flex-direction: column;
    gap: 14px;
}

.side-section {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    padding: 18px 18px 14px;
}

.side-titulo {
    font-size: .9rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 14px;
}

.resumen-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 10px;
}

.resumen-card {
    background: #f9fafb;
    border: 1px solid #f3f4f6;
    border-radius: 10px;
    padding: 12px 14px;
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    gap: 2px;
}

.resumen-num {
    font-size: 1.7rem;
    font-weight: 800;
    line-height: 1;
}

.resumen-label {
    font-size: .74rem;
    color: #6b7280;
    font-weight: 500;
}

.azul {
    color: #2563eb;
}

.verde {
    color: #16a34a;
}

.rojo {
    color: #dc2626;
}

.gris {
    color: #374151;
}

.viajes-lista {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.viaje-card {
    border-left: 3.5px solid #e5e7eb;
    padding: 8px 10px 8px 12px;
    border-radius: 0 8px 8px 0;
    background: #fafafa;
    display: flex;
    flex-direction: column;
    gap: 2px;
}

.viaje-card.clickable {
    cursor: pointer;
    transition: background-color 0.15s;
}

.viaje-card.clickable:hover {
    background-color: #f3f4f6;
}

.borde-en-viaje {
    border-left-color: #2563eb;
}

.borde-programado {
    border-left-color: #16a34a;
}

.borde-pendiente {
    border-left-color: #d97706;
}

.borde-espera {
    border-left-color: #9ca3af;
}

.borde-cancelado {
    border-left-color: #dc2626;
}

.viaje-hora {
    font-size: .72rem;
    color: #6b7280;
    font-weight: 500;
}

.viaje-titulo {
    font-size: .95rem;
    font-weight: 700;
    color: #111827;
}

.viaje-recurso {
    font-size: .75rem;
    color: #6b7280;
    margin-bottom: 4px;
}

.badge-pill {
    display: inline-block;
    padding: 2px 10px;
    border-radius: 20px;
    font-size: .68rem;
    font-weight: 700;
    align-self: flex-start;
}

.badge-en-viaje-pill {
    background: #dbeafe;
    color: #1e40af;
}

.badge-programado-pill {
    background: #d1fae5;
    color: #065f46;
}

.badge-pendiente-pill {
    background: #fef3c7;
    color: #92400e;
}

.badge-espera-pill {
    background: #f3f4f6;
    color: #374151;
}

.badge-cancelado-pill {
    background: #fee2e2;
    color: #991b1b;
}

/* ==========================================================================
   ESTILOS DE LOS CUADROS DE DIÁLOGO (MODALES)
   ========================================================================== */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background: rgba(0, 0, 0, 0.4);
    backdrop-filter: blur(2px);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 300;
    padding: 16px;
}

.modal-overlay.z-top {
    z-index: 400; /* Asegura que el detalle quede sobre la lista */
}

.modal-content {
    background: #fff;
    border-radius: 16px;
    box-shadow: 0 12px 32px rgba(0, 0, 0, 0.15);
    display: flex;
    flex-direction: column;
    max-height: 85vh;
    animation: scaleUp 0.2s ease-out;
}

@keyframes scaleUp {
    from { transform: scale(0.95); opacity: 0; }
    to { transform: scale(1); opacity: 1; }
}

.modal-lista {
    width: 100%;
    max-width: 440px;
}

.modal-detalle {
    width: 100%;
    max-width: 520px;
}

.modal-header-box {
    padding: 18px 24px;
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
}

.modal-header-box.border-b {
    border-bottom: 1px solid #f3f4f6;
}

.modal-main-title {
    font-size: 1.15rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.text-xl {
    font-size: 1.3rem;
}

.modal-subtitle {
    font-size: 0.82rem;
    color: #6b7280;
    margin: 2px 0 0;
    font-weight: 500;
}

.modal-close-btn {
    background: transparent;
    border: none;
    font-size: 1.6rem;
    color: #9ca3af;
    cursor: pointer;
    line-height: 1;
    padding: 0 4px;
    border-radius: 6px;
    transition: color 0.15s;
}

.modal-close-btn:hover {
    color: #374151;
}

.modal-body-scroll {
    padding: 0 24px 24px;
    overflow-y: auto;
}

.viajes-lista-modal {
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.viaje-modal-row {
    border-left: 4px solid #e5e7eb;
    background: #f9fafb;
    padding: 12px 14px;
    border-radius: 0 10px 10px 0;
    cursor: pointer;
    transition: all 0.15s;
}

.viaje-modal-row:hover {
    background: #f3f4f6;
    transform: translateX(2px);
}

.viaje-modal-meta {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 4px;
}

.v-modal-time {
    font-size: 0.75rem;
    font-weight: 700;
    color: #374151;
}

.v-modal-title {
    font-size: 0.95rem;
    font-weight: 700;
    color: #111827;
    margin-bottom: 2px;
}

.v-modal-subtext {
    font-size: 0.78rem;
    color: #6b7280;
}

/* Cuerpo detallado */
.modal-body-detail {
    padding: 24px;
    overflow-y: auto;
}

.detail-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 16px;
}

.detail-item {
    display: flex;
    flex-direction: column;
    gap: 3px;
}

.detail-item.full-w {
    grid-column: span 2;
}

.detail-label {
    font-size: 0.72rem;
    font-weight: 700;
    color: #9ca3af;
    text-transform: uppercase;
    letter-spacing: 0.03em;
}

.detail-value {
    font-size: 0.92rem;
    color: #374151;
}

.detail-value.highlight {
    font-size: 1.05rem;
    font-weight: 600;
    color: #1a3a2a;
}

.detail-divider {
    grid-column: span 2;
    font-size: 0.7rem;
    font-weight: 800;
    color: #1a3a2a;
    background: #f0fdf4;
    padding: 4px 8px;
    border-radius: 4px;
    margin-top: 8px;
    letter-spacing: 0.05em;
}

.font-semibold { font-weight: 600; }
.capitalize { text-transform: capitalize; }
.mb-2 { margin-bottom: 8px; }

.code-style {
    font-family: monospace;
    background: #f3f4f6;
    padding: 2px 6px;
    border-radius: 4px;
    font-size: 0.85rem;
    width: fit-content;
    font-weight: 600;
}

.modal-footer-box {
    padding: 14px 24px 18px;
    border-top: 1px solid #f3f4f6;
    display: flex;
    justify-content: flex-end;
}

.btn-modal-action {
    padding: 8px 20px;
    background: #1a3a2a;
    color: #fff;
    border: none;
    border-radius: 8px;
    font-size: 0.85rem;
    font-weight: 600;
    cursor: pointer;
    transition: background 0.15s;
}

.btn-modal-action:hover {
    background: #14532d;
}

@media (max-width: 1100px) {
    .agenda-layout {
        grid-template-columns: 1fr;
    }

    .side-panel {
        flex-direction: row;
        flex-wrap: wrap;
    }

    .side-section {
        flex: 1;
        min-width: 280px;
    }
}

@media (max-width: 700px) {
    .agenda-page {
        padding: 16px;
    }

    .agenda-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 12px;
    }

    .cal-celda {
        min-height: 60px;
    }

    .side-panel {
        flex-direction: column;
    }
    
    .detail-grid {
        grid-template-columns: 1fr;
    }
    .detail-item.full-w {
        grid-column: span 1;
    }
    .detail-divider {
        grid-column: span 1;
    }
}
</style>