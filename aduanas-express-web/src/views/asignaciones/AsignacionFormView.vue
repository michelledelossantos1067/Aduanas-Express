<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { crearAsignaciones, obtenerDisponibles, obtenerVehiculosPorUbicacion } from '@/services/asignacionService.js'
import { verSolicitud } from '@/services/solicitudService.js'
import {
    formatFecha,
    formatHora,
    formatNumero,
    iniciales,
} from './composables/useAsignacionHelpers'

const emit = defineEmits(['exito', 'error', 'asignacion-creada'])

const authStore = useAuthStore()
const solicitudes = ref([])
const vehiculos = ref([])
const conductores = ref([])
const loadingSol = ref(false)
const loadingRes = ref(false)
const guardando = ref(false)
const busquedaSol = ref('')
const busquedaRes = ref('')
const solicitudSeleccionada = ref(null)
const vehiculoSeleccionado = ref(null)
const conductorSeleccionado = ref(null)

const solicitudesFiltradas = computed(() => {
    const q = busquedaSol.value.toLowerCase()
    return solicitudes.value.filter(s =>
        s.estado === 0 && (
            !q ||
            s.areaSolicitante?.toLowerCase().includes(q) ||
            s.destino?.toLowerCase().includes(q) ||
            String(s.id).includes(q)
        )
    )
})

const pendientesCount = computed(() => solicitudes.value.filter(s => s.estado === 0).length)

const vehiculosFiltrados = computed(() => {
    const q = busquedaRes.value.toLowerCase()
    return vehiculos.value.filter(v =>
        !q ||
        v.matricula?.toLowerCase().includes(q) ||
        v.modelo?.toLowerCase().includes(q) ||
        v.marca?.toLowerCase().includes(q)
    )
})

const conductoresFiltrados = computed(() => {
    const q = busquedaRes.value.toLowerCase()
    return conductores.value.filter(c =>
        !q ||
        c.nombre?.toLowerCase().includes(q) ||
        c.apellido?.toLowerCase().includes(q)
    )
})

const validaciones = computed(() => {
    if (!solicitudSeleccionada.value || !vehiculoSeleccionado.value || !conductorSeleccionado.value)
        return []

    const sol = solicitudSeleccionada.value
    const veh = vehiculoSeleccionado.value
    const con = conductorSeleccionado.value
    const hoy = new Date()
    hoy.setHours(0, 0, 0, 0)

    return [
        {
            ok: (veh.capacidad ?? 0) >= (sol.cantidadColaboradores ?? 0),
            texto: `Capacidad suficiente (${veh.capacidad ?? '?'} ≥ ${sol.cantidadColaboradores ?? 0})`,
        },
        {
            ok: !con.fechaVencLicencia || new Date(con.fechaVencLicencia) >= hoy,
            texto: 'Licencia vigente',
        },
    ]
})

const puedeAsignar = computed(() =>
    solicitudSeleccionada.value &&
    vehiculoSeleccionado.value &&
    conductorSeleccionado.value
)

async function cargarSolicitudes() {
    loadingSol.value = true
    try {
        const res = await verSolicitud()
        solicitudes.value = res.data
    } catch (e) {
        console.error(e)
    } finally {
        loadingSol.value = false
    }
}

async function cargarRecursosDisponibles(solicitudId) {
    if (!solicitudId) {
        vehiculos.value = []
        conductores.value = []
        return
    }
    loadingRes.value = true
    try {
        const res = await obtenerDisponibles(solicitudId)
        vehiculos.value = res.data?.vehiculos ?? []
        conductores.value = res.data?.conductores ?? []
        
        // NUEVO: Si tenemos una solicitud seleccionada, filtramos vehículos por su ubicación
        if (solicitudSeleccionada.value?.puntoOrigen) {
            await filtrarVehiculosPorUbicacion(solicitudSeleccionada.value.puntoOrigen)
        }
    } catch (e) {
        console.error(e)
        vehiculos.value = []
        conductores.value = []
    } finally {
        loadingRes.value = false
    }
}

// NUEVO: Filtrar vehículos por ubicación actual
async function filtrarVehiculosPorUbicacion(puntoOrigen) {
    try {
        const res = await obtenerVehiculosPorUbicacion(puntoOrigen)
        // Solo mostrar vehículos que estén en la ubicación del puntoOrigen
        vehiculos.value = res.data ?? []
    } catch (e) {
        console.error('Error al filtrar vehículos por ubicación:', e)
        vehiculos.value = []
    }
}

function seleccionarSolicitud(s) {
    const yaEstabaSeleccionada = solicitudSeleccionada.value?.id === s.id
    solicitudSeleccionada.value = yaEstabaSeleccionada ? null : s
    vehiculoSeleccionado.value = null
    conductorSeleccionado.value = null
    cargarRecursosDisponibles(solicitudSeleccionada.value?.id ?? null)
}

function seleccionarVehiculo(v) {
    vehiculoSeleccionado.value = vehiculoSeleccionado.value?.id === v.id ? null : v
}

function seleccionarConductor(c) {
    conductorSeleccionado.value = conductorSeleccionado.value?.id === c.id ? null : c
}

function limpiarSeleccion() {
    solicitudSeleccionada.value = null
    vehiculoSeleccionado.value = null
    conductorSeleccionado.value = null
    vehiculos.value = []
    conductores.value = []
}

async function guardarAsignacion() {
    if (!puedeAsignar.value) return
    guardando.value = true

    const ahora = new Date()
    const fechaAsignacionLocal =
        `${ahora.getFullYear()}-${String(ahora.getMonth() + 1).padStart(2, '0')}-${String(ahora.getDate()).padStart(2, '0')}` +
        `T${String(ahora.getHours()).padStart(2, '0')}:${String(ahora.getMinutes()).padStart(2, '0')}:${String(ahora.getSeconds()).padStart(2, '0')}`

    const payload = {
        solicitudId: solicitudSeleccionada.value.id,
        vehiculoId: vehiculoSeleccionado.value.id,
        conductorId: conductorSeleccionado.value.id,
        fechaAsignacion: fechaAsignacionLocal,
        asignadoPorId: authStore.usuario.id,
    }

    try {
        await crearAsignaciones(payload)
        emit('exito', 'Asignación creada correctamente.')
        limpiarSeleccion()
        await cargarSolicitudes()
        emit('asignacion-creada')
    } catch (e) {
        console.error('Error detalle:', e.response?.data)
        emit('error', 'Error al guardar la asignación.')
    } finally {
        guardando.value = false
    }
}

async function actualizar() {
    await cargarSolicitudes()
    if (solicitudSeleccionada.value) {
        await cargarRecursosDisponibles(solicitudSeleccionada.value.id)
    }
}

defineExpose({ cargarSolicitudes, actualizar })

onMounted(cargarSolicitudes)
</script>

<template>
    <div class="asig-board">
        <div class="col-panel">
            <div class="col-header">
                <div class="col-header-left">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
                        <polyline points="14 2 14 8 20 8" />
                        <line x1="16" y1="13" x2="8" y2="13" />
                        <line x1="16" y1="17" x2="8" y2="17" />
                        <polyline points="10 9 9 9 8 9" />
                    </svg>
                    <h2 class="col-titulo">Solicitudes</h2>
                </div>
                <span class="badge-count">{{ pendientesCount }} pendientes</span>
            </div>

            <div class="col-search">
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
                    <circle cx="11" cy="11" r="8" />
                    <line x1="21" y1="21" x2="16.65" y2="16.65" />
                </svg>
                <input v-model="busquedaSol" type="text" placeholder="Buscar solicitud ....." class="search-input" />
            </div>

            <div v-if="loadingSol" class="col-loading">
                <div class="spinner-sm"></div>
            </div>

            <div v-else class="col-scroll">
                <div v-if="solicitudesFiltradas.length === 0" class="col-vacio">
                    No hay solicitudes pendientes.
                </div>
                <div
                    v-for="s in solicitudesFiltradas"
                    :key="s.id"
                    class="sol-card"
                    :class="{ 'sol-card-activa': solicitudSeleccionada?.id === s.id }"
                    @click="seleccionarSolicitud(s)"
                >
                    <div class="sol-card-top">
                        <span class="sol-id">{{ formatNumero(s.id) }}</span>
                        <span class="badge badge-pendiente">Pendiente</span>
                    </div>
                    <p class="sol-destino">{{ s.destino }}</p>
                    <div class="sol-meta">
                        <span>{{ formatFecha(s.fechaViaje) }}</span>
                        <span>{{ formatHora(s.horaSalida) }}</span>
                        <span>{{ s.cantidadColaboradores }} colab.</span>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-panel">
            <div class="col-header">
                <div class="col-header-left">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <rect x="1" y="3" width="15" height="13" rx="2" />
                        <path d="M16 8h4l3 3v5h-7V8z" />
                        <circle cx="5.5" cy="18.5" r="2.5" />
                        <circle cx="18.5" cy="18.5" r="2.5" />
                    </svg>
                    <h2 class="col-titulo">Recursos disponibles</h2>
                </div>
                <span class="badge-fecha" v-if="solicitudSeleccionada">
                    {{ formatFecha(solicitudSeleccionada.fechaViaje) }}
                </span>
                <span class="badge-fecha" v-else>Selecciona solicitud</span>
            </div>

            <div class="col-search">
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
                    <circle cx="11" cy="11" r="8" />
                    <line x1="21" y1="21" x2="16.65" y2="16.65" />
                </svg>
                <input v-model="busquedaRes" type="text" placeholder="Buscar vehículo o conductor ..." class="search-input" />
            </div>

            <div v-if="loadingRes" class="col-loading">
                <div class="spinner-sm"></div>
            </div>

            <div v-else class="col-scroll">
                <p class="recursos-grupo-label">VEHÍCULOS</p>

                <div v-if="!solicitudSeleccionada" class="col-vacio">
                    Selecciona una solicitud para ver los vehículos disponibles en esa fecha.
                </div>
                <div v-else-if="vehiculosFiltrados.length === 0" class="col-vacio">
                    Sin vehículos disponibles en esta ubicación y fecha.
                </div>
                <div
                    v-else
                    v-for="v in vehiculosFiltrados"
                    :key="'v' + v.id"
                    class="recurso-card"
                    :class="{ 'recurso-card-activo': vehiculoSeleccionado?.id === v.id }"
                    @click="seleccionarVehiculo(v)"
                >
                    <div class="recurso-card-top">
                        <span class="recurso-placa">{{ v.matricula }}</span>
                        <span class="badge badge-disponible">{{ v.estado }}</span>
                    </div>
                    <p class="recurso-sub">{{ v.marca }} {{ v.modelo }}</p>
                    <div class="recurso-meta">
                        <span>Capacidad <strong>{{ v.capacidad ?? '—' }} pasajeros</strong></span>
                        <span v-if="v.ubicacionActual">📍 {{ v.ubicacionActual }}</span>
                    </div>
                </div>

                <p class="recursos-grupo-label recursos-grupo-sep">CONDUCTORES</p>

                <div v-if="conductoresFiltrados.length === 0" class="col-vacio">
                    Sin conductores disponibles.
                </div>
                <div
                    v-else
                    v-for="c in conductoresFiltrados"
                    :key="'c' + c.id"
                    class="recurso-card"
                    :class="{ 'recurso-card-activo': conductorSeleccionado?.id === c.id }"
                    @click="seleccionarConductor(c)"
                >
                    <div class="conductor-info">
                        <div class="conductor-avatar-sm">
                            {{ iniciales(c.nombre, c.apellido) }}
                        </div>
                        <div>
                            <p class="recurso-nombre">{{ c.nombre }} {{ c.apellido }}</p>
                            <p class="recurso-sub">{{ c.cedula }}</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-panel">
            <div v-if="solicitudSeleccionada && vehiculoSeleccionado && conductorSeleccionado" class="panel-contenido">
                <div class="panel-seccion">
                    <p class="panel-label">Solicitud</p>
                    <div class="panel-ruta">
                        {{ solicitudSeleccionada.puntoOrigen }}
                        <svg class="panel-flecha" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12h14M12 5l7 7-7 7" /></svg>
                        {{ solicitudSeleccionada.destino }}
                    </div>
                    <p class="panel-meta-sol">{{ formatFecha(solicitudSeleccionada.fechaViaje) }} a las {{ formatHora(solicitudSeleccionada.horaSalida) }}</p>
                    <p class="panel-meta-sol">{{ solicitudSeleccionada.cantidadColaboradores }} colaboradores</p>
                </div>

                <div class="panel-seccion">
                    <p class="panel-label">Vehículo asignado</p>
                    <div class="panel-card panel-card-vehiculo">
                        <p class="panel-card-titulo">{{ vehiculoSeleccionado.marca }} {{ vehiculoSeleccionado.modelo }}</p>
                        <p class="panel-card-sub">{{ vehiculoSeleccionado.matricula }}</p>
                    </div>
                </div>

                <div class="panel-seccion">
                    <p class="panel-label">Conductor asignado</p>
                    <div class="panel-card panel-card-conductor">
                        <div class="panel-conductor-row">
                            <div class="conductor-avatar-md">
                                {{ iniciales(conductorSeleccionado.nombre, conductorSeleccionado.apellido) }}
                            </div>
                            <div>
                                <p class="panel-card-titulo">{{ conductorSeleccionado.nombre }} {{ conductorSeleccionado.apellido }}</p>
                                <p class="panel-card-sub">{{ conductorSeleccionado.cedula }}</p>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel-seccion">
                    <p class="panel-label">Validaciones</p>
                    <div class="validaciones">
                        <div v-for="(v, idx) in validaciones" :key="idx" class="validacion-item" :class="v.ok ? 'val-ok' : 'val-fail'">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                <path v-if="v.ok" d="M20 6L9 17l-5-5" />
                                <circle v-else cx="12" cy="12" r="10" />
                                <line v-if="!v.ok" x1="15" y1="9" x2="9" y2="15" />
                                <line v-if="!v.ok" x1="9" y1="9" x2="15" y2="15" />
                            </svg>
                            {{ v.texto }}
                        </div>
                    </div>
                </div>

                <div class="panel-acciones">
                    <button
                        class="btn-asignar"
                        :disabled="!puedeAsignar || guardando"
                        @click="guardarAsignacion"
                    >
                        <svg v-if="!guardando" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z" /></svg>
                        <span v-if="!guardando">Crear asignación</span>
                        <span v-else>Guardando...</span>
                    </button>
                    <button class="btn-limpiar" @click="limpiarSeleccion">Limpiar selección</button>
                </div>
            </div>

            <div v-else class="panel-vacio">
                <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                    <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
                    <polyline points="14 2 14 8 20 8" />
                </svg>
                <p>Selecciona una solicitud, vehículo y conductor para crear una asignación.</p>
            </div>
        </div>
    </div>
</template>

<style scoped>
.asig-board {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 16px;
    height: 100vh;
}

.col-panel {
    display: flex;
    flex-direction: column;
    background: #fff;
    border-radius: 12px;
    overflow: hidden;
    max-height: 100vh;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .1);
}

.col-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 14px 18px;
    border-bottom: 1px solid #e5e7eb;
}

.col-header-left {
    display: flex;
    align-items: center;
    gap: 8px;
}

.col-titulo {
    font-size: .95rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.badge-count {
    font-size: .75rem;
    font-weight: 600;
    color: #6b7280;
}

.badge-fecha {
    font-size: .75rem;
    font-weight: 500;
    color: #9ca3af;
}

.col-search {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 0 16px;
    border-bottom: 1px solid #e5e7eb;
}

.search-input {
    flex: 1;
    padding: 10px 0;
    border: none;
    outline: none;
    font-size: .85rem;
    background: transparent;
}

.col-loading,
.col-vacio {
    display: flex;
    align-items: center;
    justify-content: center;
    flex: 1;
    color: #9ca3af;
}

.col-vacio {
    flex-direction: column;
    font-size: .85rem;
    padding: 24px;
    text-align: center;
}

.col-scroll {
    flex: 1;
    overflow-y: auto;
    padding: 8px;
}

.spinner-sm {
    width: 24px;
    height: 24px;
    border: 2px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .6s linear infinite;
}

@keyframes spin {
    to { transform: rotate(360deg); }
}

.sol-card {
    border: 1.5px solid #e5e7eb;
    border-radius: 10px;
    padding: 11px 13px;
    margin-bottom: 8px;
    cursor: pointer;
    transition: border-color .15s, box-shadow .15s;
}

.sol-card:hover {
    border-color: #d1d5db;
    box-shadow: 0 2px 8px rgba(0, 0, 0, .06);
}

.sol-card-activa {
    border-color: #1a3a2a !important;
    box-shadow: 0 0 0 3px rgba(26, 58, 42, .08);
}

.sol-card-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 6px;
}

.sol-id {
    font-size: .75rem;
    font-weight: 700;
    color: #6b7280;
}

.sol-destino {
    font-size: .9rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 6px;
}

.sol-meta {
    display: flex;
    gap: 10px;
    font-size: .75rem;
    color: #6b7280;
    flex-wrap: wrap;
}

.recursos-grupo-label {
    font-size: .68rem;
    font-weight: 700;
    color: #9ca3af;
    letter-spacing: .08em;
    margin: 4px 0 8px;
}

.recursos-grupo-sep { margin-top: 16px; }

.recurso-card {
    border: 1.5px solid #e5e7eb;
    border-radius: 10px;
    padding: 11px 13px;
    margin-bottom: 8px;
    cursor: pointer;
    transition: border-color .15s, box-shadow .15s;
}

.recurso-card:hover {
    border-color: #d1d5db;
    box-shadow: 0 2px 8px rgba(0, 0, 0, .06);
}

.recurso-card-activo {
    border-color: #1a3a2a !important;
    box-shadow: 0 0 0 3px rgba(26, 58, 42, .08);
}

.recurso-card-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 5px;
}

.recurso-placa {
    font-size: .85rem;
    font-weight: 700;
    color: #111827;
}

.recurso-nombre {
    font-size: .875rem;
    font-weight: 600;
    color: #111827;
}

.recurso-sub {
    font-size: .78rem;
    color: #6b7280;
    margin: 0 0 5px;
}

.recurso-meta {
    display: flex;
    gap: 12px;
    font-size: .74rem;
    color: #6b7280;
    flex-wrap: wrap;
}

.conductor-info {
    display: flex;
    align-items: center;
    gap: 8px;
}

.conductor-avatar-sm {
    width: 28px;
    height: 28px;
    border-radius: 50%;
    background: #d1fae5;
    color: #065f46;
    font-size: .68rem;
    font-weight: 700;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.panel-vacio {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 48px 24px;
    color: #9ca3af;
    font-size: .875rem;
    text-align: center;
    line-height: 1.5;
}

.panel-contenido {
    flex: 1;
    overflow-y: auto;
}

.panel-seccion {
    padding: 16px 18px;
    border-bottom: 1px solid #f3f4f6;
}

.panel-label {
    font-size: .72rem;
    font-weight: 700;
    color: #9ca3af;
    text-transform: uppercase;
    letter-spacing: .07em;
    margin: 0 0 8px;
}

.panel-ruta {
    font-size: 1rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 5px;
}

.panel-flecha {
    vertical-align: middle;
    margin: 0 4px;
}

.panel-meta-sol {
    font-size: .78rem;
    color: #6b7280;
    margin: 0;
}

.panel-card {
    border-radius: 10px;
    padding: 12px 14px;
}

.panel-card-vehiculo,
.panel-card-conductor {
    background: #f0fdf4;
    border: 1.5px solid #bbf7d0;
}

.panel-card-titulo {
    font-size: .9rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 3px;
}

.panel-card-sub {
    font-size: .78rem;
    color: #6b7280;
    margin: 0;
}

.panel-conductor-row {
    display: flex;
    align-items: center;
    gap: 10px;
}

.conductor-avatar-md {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    background: #d1fae5;
    color: #065f46;
    font-size: .75rem;
    font-weight: 700;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.validaciones {
    display: flex;
    flex-direction: column;
    gap: 6px;
}

.validacion-item {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: .82rem;
    font-weight: 500;
}

.val-ok { color: #065f46; }
.val-fail { color: #991b1b; }

.panel-acciones {
    padding: 16px 18px;
    display: flex;
    flex-direction: column;
    gap: 8px;
    border-top: 1px solid #f3f4f6;
}

.btn-asignar {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
    width: 100%;
    padding: 11px 0;
    background: #1a3a2a;
    border: none;
    border-radius: 9px;
    font-size: .9rem;
    font-weight: 700;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}

.btn-asignar:hover:not(:disabled) { background: #14532d; }
.btn-asignar:disabled { opacity: .45; cursor: default; }

.btn-limpiar {
    width: 100%;
    padding: 9px 0;
    background: transparent;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    font-size: .85rem;
    font-weight: 500;
    color: #6b7280;
    cursor: pointer;
    transition: background .15s;
}

.btn-limpiar:hover { background: #f3f4f6; }

@media (max-width: 1100px) {
    .asig-board { grid-template-columns: 1fr 1fr; }
    .col-panel:last-child { grid-column: 1 / -1; max-height: none; }
}

@media (max-width: 700px) {
    .asig-board { grid-template-columns: 1fr; }
    .col-panel { max-height: 400px; }
}
</style>