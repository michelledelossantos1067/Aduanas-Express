<script setup>
import { ref, computed, onMounted } from 'vue'
import { verSolicitud, eliminarSolicitud } from '../../services/solicitudService'
import ModalVerSolicitud from './ModalVerSolicitud.vue'
import ModalEliminarSolicitud from './ModalEliminarSolicitud.vue'
import { verVehiculos } from '../../services/vehiculoService'
import { verConductores } from '../../services/conductorService'
import { useRouter } from 'vue-router'
const router = useRouter()

const solicitudes = ref([])
const loading = ref(false)
const error = ref('')
const busqueda = ref('')
const filtroEstado = ref('')
const filtroFecha = ref('semana')
const tabActivo = ref('todas')
const mostrarConfirmacion = ref(false)
const solicitudAEliminar = ref(null)

const paginaActual = ref(1)
const porPagina = 10

const estadosSolicitud = [
    { label: 'Pendiente', value: 0 },
    { label: 'Aprobada', value: 1 },
    { label: 'Rechazada', value: 2 },
    { label: 'Cancelada', value: 3 },
    { label: 'Finalizada', value: 4 },
]

const estadoBadgeClase = {
    0: 'badge-pendiente',
    1: 'badge-aprobada',
    2: 'badge-rechazada',
    3: 'badge-cancelada',
    4: 'badge-finalizada',
}

const estadoLabel = (valor) =>
    estadosSolicitud.find((e) => e.value === valor)?.label ?? valor

const resumen = computed(() => ({
    total: solicitudes.value.length,
    pendientes: solicitudes.value.filter(s => s.estado === 0).length,
    aprobadas: solicitudes.value.filter(s => s.estado === 1).length,
    rechazadas: solicitudes.value.filter(s => s.estado === 2).length,
    canceladas: solicitudes.value.filter(s => s.estado === 3).length,
    finalizadas: solicitudes.value.filter(s => s.estado === 4).length,
}))

const solicitudesFiltradas = computed(() => {
    return solicitudes.value.filter((s) => {
        const q = busqueda.value.toLowerCase()

        const coincideBusqueda =
            !q ||
            s.areaSolicitante?.toLowerCase().includes(q) ||
            s.destino?.toLowerCase().includes(q) ||
            s.motivoViaje?.toLowerCase().includes(q) ||
            String(s.id).includes(q)

        const coincideEstado =
            filtroEstado.value === '' ||
            String(s.estado) === filtroEstado.value

        const coincideTab =
            tabActivo.value === 'todas' ? true :
                tabActivo.value === 'pendientes' ? s.estado === 0 :
                    tabActivo.value === 'aprobadas' ? s.estado === 1 :
                        tabActivo.value === 'historial' ? [2, 3, 4].includes(s.estado) :
                            true

        return coincideBusqueda && coincideEstado && coincideTab
    })
})

const modalVer = ref({ show: false, id: null })
const modalEliminar = ref({ show: false, solicitud: null })

function abrirVer(id) {
    modalVer.value = { show: true, id }
}

function abrirNuevo()      { router.push('/solicitudes/nuevo') }
function abrirEditar(id){
    router.push(`/solicitudes/${id}/editar`)
}
function confirmarEliminar(solicitud) {
    modalEliminar.value = { show: true, solicitud }
}

async function ejecutarEliminar() {
    try {
        await eliminarSolicitud(modalEliminar.value.solicitud.id)
        solicitudes.value = solicitudes.value.filter(
            s => s.id !== modalEliminar.value.solicitud.id
        )
    } catch (e) {
        console.error(e)
        error.value = 'Error al eliminar la solicitud.'
    } finally {
        modalEliminar.value.show = false
    }
}

const totalPaginas = computed(() =>
    Math.max(1, Math.ceil(solicitudesFiltradas.value.length / porPagina))
)

const solicitudesPagina = computed(() => {
    const ini = (paginaActual.value - 1) * porPagina
    return solicitudesFiltradas.value.slice(ini, ini + porPagina)
})

const paginasVisibles = computed(() => {
    const total = totalPaginas.value
    const actual = paginaActual.value
    const pages = []

    if (total <= 5) {
        for (let i = 1; i <= total; i++) pages.push(i)
    } else {
        pages.push(1)
        if (actual > 3) pages.push('...')
        for (let i = Math.max(2, actual - 1); i <= Math.min(total - 1, actual + 1); i++) {
            pages.push(i)
        }
        if (actual < total - 2) pages.push('...')
        pages.push(total)
    }

    return pages
})

function irPagina(p) {
    if (p === '...') return
    paginaActual.value = p
}

async function cargarSolicitudes() {
    loading.value = true
    error.value = ''
    try {
        const [resSolicitudes, resVehiculos, resConductores] = await Promise.all([
            verSolicitud(),
            verVehiculos(),
            verConductores(),
        ])

        const mapaVehiculos   = Object.fromEntries(resVehiculos.data.map(v => [v.id, v]))
        const mapaConductores = Object.fromEntries(resConductores.data.map(c => [c.id, c]))

        solicitudes.value = resSolicitudes.data.map(s => ({
            ...s,
            vehiculo:  mapaVehiculos[s.vehiculoId]   ?? null,
            conductor: mapaConductores[s.conductorId] ?? null,
        }))
    } catch (e) {
        console.error(e)
        error.value = 'No se pudieron cargar las solicitudes.'
    } finally {
        loading.value = false
    }
}
function exportar() {
    const headers = [
        'ID', 'Área Solicitante', 'Colaboradores',
        'Fecha Viaje', 'Hora Salida', 'Destino',
        'Motivo', 'Vehículo', 'Conductor', 'Estado'
    ]

    const filas = solicitudes.value.map((s) => [
        s.id,
        s.areaSolicitante,
        s.cantidadColaboradores,
        formatFecha(s.fechaViaje),
        formatHora(s.horaSalida),
        s.destino,
        s.motivoViaje,
        s.vehiculo?.matricula ?? '',
        s.conductor ? `${s.conductor.nombre} ${s.conductor.apellido}` : '',
        estadoLabel(s.estado)
    ])

    const csv = [headers, ...filas].map(f => f.join(',')).join('\n')
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = 'solicitudes.csv'
    a.click()
    URL.revokeObjectURL(url)
}

function formatFecha(fecha) {
    if (!fecha) return '—'
    return new Date(fecha).toLocaleDateString('es-DO', {
        day: '2-digit', month: '2-digit', year: 'numeric'
    })
}

function formatHora(hora) {
    if (!hora) return '—'
    return hora.toString().substring(0, 5)
}

function formatNumero(id) {
    return `#${String(id).padStart(4, '0')}`
}

onMounted(cargarSolicitudes)
</script>

<template>
    <div class="sol-page">

        <div class="sol-header">
            <h1 class="sol-title">Solicitudes de transporte</h1>
            <div class="sol-header-actions">
                <button class="btn-exportar" @click="exportar">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                        <polyline points="7 10 12 15 17 10" />
                        <line x1="12" y1="15" x2="12" y2="3" />
                    </svg>
                    Exportar
                </button>
                <button class="btn-nuevo" @click="abrirNuevo">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.5">
                        <line x1="12" y1="5" x2="12" y2="19" />
                        <line x1="5" y1="12" x2="19" y2="12" />
                    </svg>
                    Nuevo transporte
                </button>
            </div>
        </div>

        <div class="sol-resumen">
            <div class="resumen-card">
                <span class="resumen-dot dot-pendiente"></span>
                <div>
                    <p class="resumen-num">{{ resumen.pendientes }}</p>
                    <p class="resumen-label">Pendientes</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="resumen-dot dot-aprobada"></span>
                <div>
                    <p class="resumen-num">{{ resumen.aprobadas }}</p>
                    <p class="resumen-label">Aprobadas</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="resumen-dot dot-rechazada"></span>
                <div>
                    <p class="resumen-num">{{ resumen.rechazadas }}</p>
                    <p class="resumen-label">Rechazadas</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="resumen-dot dot-cancelada"></span>
                <div>
                    <p class="resumen-num">{{ resumen.canceladas }}</p>
                    <p class="resumen-label">Canceladas</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="resumen-dot dot-finalizada"></span>
                <div>
                    <p class="resumen-num">{{ resumen.finalizadas }}</p>
                    <p class="resumen-label">Finalizadas</p>
                </div>
            </div>
        </div>

        <div class="sol-filtros">
            <div class="filtro-search">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
                    <circle cx="11" cy="11" r="8" />
                    <line x1="21" y1="21" x2="16.65" y2="16.65" />
                </svg>
                <input v-model="busqueda" type="text" placeholder="Buscar por área, destino o Número de solicitud..."
                    class="filtro-input" />
            </div>
            <select v-model="filtroEstado" class="filtro-select">
                <option value="">Todos los estados</option>
                <option v-for="e in estadosSolicitud" :key="e.value" :value="String(e.value)">
                    {{ e.label }}
                </option>
            </select>
            <select v-model="filtroFecha" class="filtro-select">
                <option value="semana">Esta semana</option>
                <option value="mes">Este mes</option>
                <option value="todo">Todo</option>
            </select>
        </div>

        <div class="sol-tabs">
            <button v-for="tab in [
                { key: 'todas', label: 'Todas' },
                { key: 'pendientes', label: 'Pendientes' },
                { key: 'aprobadas', label: 'Aprobadas' },
                { key: 'historial', label: 'Historial' },
            ]" :key="tab.key" class="tab-btn" :class="{ 'tab-activo': tabActivo === tab.key }"
                @click="tabActivo = tab.key; paginaActual = 1">
                {{ tab.label }}
            </button>
        </div>

        <div v-if="loading" class="sol-estado">
            <div class="spinner"></div>
            <p>Cargando solicitudes...</p>
        </div>

        <div v-else-if="error" class="sol-error">
            <p>{{ error }}</p>
            <button class="btn-reintentar" @click="cargarSolicitudes">Reintentar</button>
        </div>

        <div v-else-if="solicitudesFiltradas.length > 0" class="sol-tabla-wrap">
            <table class="sol-tabla">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>ÁREA SOLICITANTE</th>
                        <th>DESTINO</th>
                        <th>FECHA · HORA</th>
                        <th>COLABORADORES</th>
                        <th>VEHÍCULO</th>
                        <th>CONDUCTOR</th>
                        <th>ESTADO</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="s in solicitudesPagina" :key="s.id">
                        <td class="td-id">{{ formatNumero(s.id) }}</td>
                        <td class="td-bold">{{ s.areaSolicitante }}</td>
                        <td class="td-bold">{{ s.destino }}</td>
                        <td class="td-fecha">
                            <span class="fecha-principal">{{ formatFecha(s.fechaViaje) }}</span>
                            <span class="fecha-hora">
                                {{ formatHora(s.horaSalida) }}
                                <template v-if="s.horaLlegada"> — {{ formatHora(s.horaLlegada) }}</template>
                            </span>
                        </td>
                        <td class="td-center">{{ s.cantidadColaboradores }}</td>
                        <td>{{ s.vehiculo?.matricula ?? 'Sin asignar' }}</td>
                        <td>
                            {{
                                s.conductor
                                    ? `${s.conductor.nombre} ${s.conductor.apellido}`
                                    : 'Sin asignar'
                            }}
                        </td>
                        <td>
                            <span class="badge" :class="estadoBadgeClase[s.estado]">
                                {{ estadoLabel(s.estado) }}
                            </span>
                        </td>
                        <td>
                            <div class="td-acciones">
                                <button class="btn-icon btn-ver" @click="abrirVer(s.id)">
                                    <svg width=" 14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                        stroke-width="2">
                                        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                                        <circle cx="12" cy="12" r="3" />
                                    </svg>
                                </button>
                                <button class="btn-icon btn-editar" @click="abrirEditar(s.id)">
                                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                        stroke-width="2">
                                        <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                                        <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                                    </svg>
                                </button>
                                <button class="btn-icon btn-eliminar" @click="confirmarEliminar(s)">
                                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                        stroke-width="2">
                                        <polyline points="3 6 5 6 21 6" />
                                        <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
                                        <path d="M10 11v6M14 11v6" />
                                    </svg>
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>

            <div class="paginacion">
                <button v-for="(p, i) in paginasVisibles" :key="i" class="pag-btn"
                    :class="{ 'pag-activo': p === paginaActual, 'pag-dots': p === '...' }" @click="irPagina(p)">{{ p
                    }}</button>

                <button class="pag-btn" :disabled="paginaActual === totalPaginas" @click="paginaActual++">&gt;</button>
            </div>
        </div>

        <div v-else class="sol-vacio">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#d1d5db" stroke-width="1.5">
                <rect x="1" y="3" width="15" height="13" rx="2" />
                <path d="M16 8h4l3 3v5h-7V8z" />
                <circle cx="5.5" cy="18.5" r="2.5" />
                <circle cx="18.5" cy="18.5" r="2.5" />
            </svg>
            <p>No se encontraron solicitudes</p>
            <span>Prueba ajustando los filtros o crea una nueva solicitud.</span>
        </div>
        <ModalVerSolicitud :show="modalVer.show" :solicitud-id="modalVer.id" @close="modalVer.show = false"
            @editar="abrirEditar" />

        <ModalEliminarSolicitud :show="modalEliminar.show" :solicitud="modalEliminar.solicitud"
            @close="modalEliminar.show = false" @confirmar="ejecutarEliminar" />
    </div>
</template>

<style scoped>

.sol-page {
    padding: 32px 40px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

.sol-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 28px;
}

.sol-title {
    font-size: 1.75rem;
    font-weight: 700;
    color: #111827;
    letter-spacing: -0.02em;
    margin: 0;
}

.sol-header-actions {
    display: flex;
    gap: 12px;
}

.btn-exportar {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    background: #fff;
    border: 1.5px solid #d1d5db;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    transition: border-color .15s, background .15s;
}

.btn-exportar:hover {
    border-color: #9ca3af;
    background: #f9fafb;
}

.btn-nuevo {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    background: #1a3a2a;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}

.btn-nuevo:hover {
    background: #14532d;
}

.sol-resumen {
    display: grid;
    grid-template-columns: repeat(5, 1fr);
    gap: 16px;
    margin-bottom: 24px;
}

.resumen-card {
    background: #fff;
    border-radius: 12px;
    padding: 18px 20px;
    display: flex;
    align-items: center;
    gap: 14px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .06);
}

.resumen-dot {
    width: 14px;
    height: 14px;
    border-radius: 4px;
    flex-shrink: 0;
}

.dot-pendiente {
    background: #fde68a;
}

.dot-aprobada {
    background: #bbf7d0;
}

.dot-rechazada {
    background: #fecaca;
}

.dot-cancelada {
    background: #bfdbfe;
}

.dot-finalizada {
    background: #e9d5ff;
}

.resumen-num {
    font-size: 1.5rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
    line-height: 1;
}

.resumen-label {
    font-size: .78rem;
    color: #6b7280;
    margin: 4px 0 0;
}

.sol-filtros {
    display: flex;
    gap: 12px;
    margin-bottom: 16px;
}

.filtro-search {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 10px;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 10px;
    padding: 0 14px;
    transition: border-color .15s;
}

.filtro-search:focus-within {
    border-color: #1a3a2a;
}

.filtro-input {
    flex: 1;
    border: none;
    outline: none;
    font-size: .9rem;
    color: #111827;
    padding: 11px 0;
    background: transparent;
}

.filtro-input::placeholder {
    color: #9ca3af;
}

.filtro-select {
    padding: 10px 14px;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 10px;
    font-size: .875rem;
    color: #374151;
    cursor: pointer;
    outline: none;
    transition: border-color .15s;
    min-width: 160px;
}

.filtro-select:focus {
    border-color: #1a3a2a;
}

.sol-tabs {
    display: flex;
    gap: 4px;
    margin-bottom: 20px;
}

.tab-btn {
    padding: 8px 20px;
    border: none;
    background: transparent;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 500;
    color: #6b7280;
    cursor: pointer;
    transition: background .15s, color .15s;
}

.tab-btn:hover {
    background: #e5e7eb;
    color: #111827;
}

.tab-activo {
    background: #fff;
    color: #111827;
    font-weight: 600;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .08);
}

.sol-tabla-wrap {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    overflow: hidden;
}

.sol-tabla {
    width: 100%;
    border-collapse: collapse;
    font-size: .875rem;
}

.sol-tabla thead tr {
    border-bottom: 1.5px solid #f3f4f6;
}

.sol-tabla th {
    padding: 14px 16px;
    text-align: left;
    font-size: .72rem;
    font-weight: 600;
    color: #9ca3af;
    letter-spacing: .05em;
    white-space: nowrap;
}

.sol-tabla td {
    padding: 14px 16px;
    color: #374151;
    border-bottom: 1px solid #f9fafb;
    vertical-align: middle;
}

.sol-tabla tbody tr:last-child td {
    border-bottom: none;
}

.sol-tabla tbody tr:hover {
    background: #fafafa;
}

.td-id {
    font-weight: 700;
    color: #111827;
}

.td-bold {
    font-weight: 600;
    color: #111827;
}

.td-center {
    text-align: center;
}

.td-fecha {
    line-height: 1;
}

.fecha-principal {
    display: block;
    font-weight: 600;
    color: #111827;
}

.fecha-hora {
    display: block;
    font-size: .78rem;
    color: #9ca3af;
    margin-top: 2px;
}

.td-acciones {
    display: flex;
    gap: 6px;
}

.btn-icon {
    width: 30px;
    height: 30px;
    border: none;
    border-radius: 7px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: filter .15s;
}

.btn-icon:hover {
    filter: brightness(.92);
}

.btn-icon.btn-ver {
    background: #d1fae5;
    color: #065f46;
}

.btn-icon.btn-editar {
    background: #fef3c7;
    color: #92400e;
}

.btn-icon.btn-eliminar {
    background: #fee2e2;
    color: #991b1b;
}

.badge {
    display: inline-block;
    padding: 3px 10px;
    border-radius: 20px;
    font-size: .73rem;
    font-weight: 600;
    white-space: nowrap;
}

.badge-pendiente {
    background: #fef3c7;
    color: #92400e;
}

.badge-aprobada {
    background: #d1fae5;
    color: #065f46;
}

.badge-rechazada {
    background: #fee2e2;
    color: #991b1b;
}

.badge-cancelada {
    background: #dbeafe;
    color: #1e40af;
}

.badge-finalizada {
    background: #ede9fe;
    color: #6d28d9;
}

.paginacion {
    display: flex;
    align-items: center;
    justify-content: flex-end;
    gap: 4px;
    padding: 14px 16px;
    border-top: 1px solid #f3f4f6;
}

.pag-btn {
    min-width: 32px;
    height: 32px;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    background: #fff;
    font-size: .8rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    transition: background .15s, border-color .15s;
    display: inline-flex;
    align-items: center;
    justify-content: center;
}

.pag-btn:hover:not(:disabled):not(.pag-dots) {
    background: #f3f4f6;
    border-color: #9ca3af;
}

.pag-btn:disabled {
    opacity: .4;
    cursor: default;
}

.pag-activo {
    background: #1a3a2a !important;
    border-color: #1a3a2a !important;
    color: #fff !important;
}

.pag-dots {
    border: none;
    background: transparent;
    cursor: default;
}

.sol-estado {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 60px 0;
    color: #6b7280;
}

.spinner {
    width: 36px;
    height: 36px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin {
    to {
        transform: rotate(360deg);
    }
}

.sol-error {
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 10px;
    padding: 20px 24px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    color: #991b1b;
    font-size: .9rem;
}

.btn-reintentar {
    padding: 7px 16px;
    background: #fff;
    border: 1.5px solid #fca5a5;
    border-radius: 8px;
    color: #991b1b;
    font-size: .8rem;
    cursor: pointer;
}

.sol-vacio {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8px;
    padding: 72px 0;
    color: #9ca3af;
    text-align: center;
}

.sol-vacio p {
    font-size: 1rem;
    font-weight: 600;
    color: #6b7280;
    margin: 8px 0 0;
}

.sol-vacio span {
    font-size: .85rem;
}

.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, .45);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 100;
}

.modal {
    background: #fff;
    border-radius: 16px;
    padding: 32px;
    width: 420px;
    max-width: 90vw;
    box-shadow: 0 20px 60px rgba(0, 0, 0, .2);
}

.modal-titulo {
    font-size: 1.1rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 10px;
}

.modal-desc {
    font-size: .9rem;
    color: #4b5563;
    line-height: 1.55;
    margin: 0 0 24px;
}

.modal-acciones {
    display: flex;
    gap: 10px;
    justify-content: flex-end;
}

.btn-cancelar-modal {
    padding: 9px 18px;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
}

.btn-confirmar-modal {
    padding: 9px 18px;
    background: #dc2626;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
}

.btn-confirmar-modal:hover {
    background: #b91c1c;
}

@media (max-width: 1024px) {
    .sol-resumen {
        grid-template-columns: repeat(3, 1fr);
    }

    .sol-tabla th:nth-child(5),
    .sol-tabla td:nth-child(5) {
        display: none;
    }
}

@media (max-width: 768px) {

    .sol-tabla th:nth-child(6),
    .sol-tabla td:nth-child(6),
    .sol-tabla th:nth-child(7),
    .sol-tabla td:nth-child(7) {
        display: none;
    }
}

@media (max-width: 640px) {
    .sol-page {
        padding: 20px 16px;
    }

    .sol-resumen {
        grid-template-columns: repeat(2, 1fr);
    }

    .sol-filtros {
        flex-direction: column;
    }

    .sol-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 14px;
    }
}
</style>
