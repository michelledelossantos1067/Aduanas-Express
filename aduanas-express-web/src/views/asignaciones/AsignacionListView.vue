<script setup>
import { ref, computed, onMounted } from 'vue'
import { verAsignaciones, crearAsignaciones, actualizarAsignaciones } from '@/services/asignacionService.js'
import { verSolicitud } from '@/services/solicitudService.js'

// ── Estado global ─────────────────────────────────────────
const solicitudes      = ref([])
const vehiculos        = ref([])
const conductores      = ref([])
const loadingSol       = ref(false)
const loadingRes       = ref(false)
const guardando        = ref(false)
const errorMsg         = ref('')
const exitoMsg         = ref('')

// ── Búsquedas ─────────────────────────────────────────────
const busquedaSol = ref('')
const busquedaRes = ref('')

// ── Selección activa ──────────────────────────────────────
const solicitudSeleccionada = ref(null)
const vehiculoSeleccionado  = ref(null)
const conductorSeleccionado = ref(null)

// ── Tab de vista ──────────────────────────────────────────
const vistaActiva = ref('asignar') // 'asignar' | 'historial'

// ── Historial ─────────────────────────────────────────────
const historial    = ref([])
const loadingHist  = ref(false)

// ── Estados ───────────────────────────────────────────────
const estadosSolicitud = [
    { label: 'Pendiente',  value: 0, clase: 'badge-pendiente' },
    { label: 'Aprobada',   value: 1, clase: 'badge-aprobada'  },
    { label: 'Rechazada',  value: 2, clase: 'badge-rechazada' },
    { label: 'Cancelada',  value: 3, clase: 'badge-cancelada' },
    { label: 'Finalizada', value: 4, clase: 'badge-finalizada'},
]

const estadoLabel = (val) => estadosSolicitud.find(e => e.value === val)?.label ?? val
const estadoClase = (val) => estadosSolicitud.find(e => e.value === val)?.clase ?? ''

// ── Solicitudes filtradas ─────────────────────────────────
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

// ── Recursos filtrados ────────────────────────────────────
const vehiculosFiltrados = computed(() => {
    const q = busquedaRes.value.toLowerCase()
    return vehiculos.value.filter(v =>
        !q ||
        v.placa?.toLowerCase().includes(q) ||
        v.modelo?.toLowerCase().includes(q) ||
        v.marca?.toLowerCase().includes(q)
    )
})

const conductoresFiltrados = computed(() => {
    const q = busquedaRes.value.toLowerCase()
    return conductores.value.filter(c =>
        !q ||
        c.nombre?.toLowerCase().includes(q) ||
        c.apellido?.toLowerCase().includes(q) ||
        c.licencia?.toLowerCase().includes(q)
    )
})

// ── Validaciones ──────────────────────────────────────────
const validaciones = computed(() => {
    if (!solicitudSeleccionada.value || !vehiculoSeleccionado.value || !conductorSeleccionado.value)
        return []

    const items = []
    const sol = solicitudSeleccionada.value
    const veh = vehiculoSeleccionado.value
    const con = conductorSeleccionado.value

    items.push({
        ok:    veh.disponible !== false,
        texto: 'Vehículo sin conflicto de horario',
    })
    items.push({
        ok:    con.disponible !== false,
        texto: 'Conductor disponible en franja horaria',
    })
    items.push({
        ok:    (veh.capacidad ?? 99) >= (sol.cantidadColaboradores ?? 0),
        texto: `Capacidad suficiente (${veh.capacidad ?? '?'} ≥ ${sol.cantidadColaboradores ?? 0})`,
    })
    items.push({
        ok:    con.licenciaVigente !== false,
        texto: 'Licencia vigente y habilitada',
    })
    return items
})

const puedeAsignar = computed(() =>
    solicitudSeleccionada.value &&
    vehiculoSeleccionado.value &&
    conductorSeleccionado.value
)

// ── Helpers de formato ────────────────────────────────────
function formatFecha(f) {
    if (!f) return '—'
    return new Date(f).toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

function formatHora(h) {
    if (!h) return ''
    return h.toString().substring(0, 5)
}

function formatNumero(id) {
    return `#${String(id).padStart(4, '0')}`
}

function iniciales(nombre, apellido) {
    return `${(nombre ?? '?')[0]}${(apellido ?? '?')[0]}`.toUpperCase()
}

// ── Carga de datos ────────────────────────────────────────
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

async function cargarRecursos() {
    loadingRes.value = true
    try {
        // Ajusta los endpoints a los de tu API
        const [resV, resC] = await Promise.all([
            fetch('/api/Vehiculos').then(r => r.json()).catch(() => []),
            fetch('/api/Conductores').then(r => r.json()).catch(() => []),
        ])
        vehiculos.value   = Array.isArray(resV) ? resV : (resV.data ?? [])
        conductores.value = Array.isArray(resC) ? resC : (resC.data ?? [])
    } catch (e) {
        console.error(e)
    } finally {
        loadingRes.value = false
    }
}

async function cargarHistorial() {
    loadingHist.value = true
    try {
        const res = await verAsignaciones()
        historial.value = res.data
    } catch (e) {
        console.error(e)
    } finally {
        loadingHist.value = false
    }
}

// ── Selección ─────────────────────────────────────────────
function seleccionarSolicitud(s) {
    solicitudSeleccionada.value = solicitudSeleccionada.value?.id === s.id ? null : s
    vehiculoSeleccionado.value  = null
    conductorSeleccionado.value = null
}

function seleccionarVehiculo(v) {
    vehiculoSeleccionado.value = vehiculoSeleccionado.value?.id === v.id ? null : v
}

function seleccionarConductor(c) {
    conductorSeleccionado.value = conductorSeleccionado.value?.id === c.id ? null : c
}

// ── Guardar asignación ────────────────────────────────────
async function guardarAsignacion() {
    if (!puedeAsignar.value) return
    guardando.value = true
    errorMsg.value  = ''
    exitoMsg.value  = ''
    try {
        await crearAsignaciones({
            solicitudId:  solicitudSeleccionada.value.id,
            vehiculoId:   vehiculoSeleccionado.value.id,
            conductorId:  conductorSeleccionado.value.id,
            fechaAsignacion: new Date().toISOString(),
            estado: 1,
        })
        exitoMsg.value = 'Asignación creada correctamente.'
        solicitudSeleccionada.value = null
        vehiculoSeleccionado.value  = null
        conductorSeleccionado.value = null
        await cargarSolicitudes()
        await cargarRecursos()
    } catch (e) {
        console.error(e)
        errorMsg.value = 'Error al guardar la asignación.'
    } finally {
        guardando.value = false
        setTimeout(() => { exitoMsg.value = ''; errorMsg.value = '' }, 3500)
    }
}

function irAHistorial() {
    vistaActiva.value = 'historial'
    cargarHistorial()
}

onMounted(() => {
    cargarSolicitudes()
    cargarRecursos()
})
</script>

<template>
    <div class="asig-page">

        <!-- ── Encabezado ── -->
        <div class="asig-header">
            <h1 class="asig-title">Asignación de vehículos y conductores</h1>
            <div class="asig-header-actions">
                <button class="btn-historial" :class="{ 'btn-activo': vistaActiva === 'historial' }"
                    @click="vistaActiva === 'historial' ? vistaActiva = 'asignar' : irAHistorial()">
                    Historial
                </button>
                <button class="btn-actualizar" @click="cargarSolicitudes(); cargarRecursos()">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                        <polyline points="1 4 1 10 7 10"/>
                        <path d="M3.51 15a9 9 0 1 0 .49-4.95"/>
                    </svg>
                    Actualizar
                </button>
            </div>
        </div>

        <!-- ── Notificaciones ── -->
        <div v-if="exitoMsg" class="notif notif-exito">{{ exitoMsg }}</div>
        <div v-if="errorMsg" class="notif notif-error">{{ errorMsg }}</div>

        <!-- ════════════ VISTA HISTORIAL ════════════ -->
        <div v-if="vistaActiva === 'historial'" class="historial-wrap">
            <div v-if="loadingHist" class="estado-carga">
                <div class="spinner"></div>
                <p>Cargando historial...</p>
            </div>
            <template v-else>
                <div v-if="historial.length === 0" class="estado-vacio">
                    <p>No hay asignaciones registradas.</p>
                </div>
                <table v-else class="hist-tabla">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Conductor</th>
                            <th>Vehículo</th>
                            <th>Solicitud</th>
                            <th>Fecha</th>
                            <th>Estado</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="a in historial" :key="a.id">
                            <td class="td-id">{{ formatNumero(a.id) }}</td>
                            <td>{{ a.conductor ? `${a.conductor.nombre} ${a.conductor.apellido}` : '—' }}</td>
                            <td>{{ a.vehiculo?.placa ?? '—' }}</td>
                            <td>{{ a.solicitud?.destino ?? '—' }}</td>
                            <td>{{ formatFecha(a.fechaAsignacion) }}</td>
                            <td>
                                <span class="badge" :class="estadoClase(a.estado)">{{ estadoLabel(a.estado) }}</span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </template>
        </div>

        <!-- ════════════ VISTA ASIGNAR (3 columnas) ════════════ -->
        <div v-else class="asig-board">

            <!-- ── Columna 1: Solicitudes ── -->
            <div class="col-panel">
                <div class="col-header">
                    <div class="col-header-left">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                            <polyline points="14 2 14 8 20 8"/>
                            <line x1="16" y1="13" x2="8" y2="13"/>
                            <line x1="16" y1="17" x2="8" y2="17"/>
                            <polyline points="10 9 9 9 8 9"/>
                        </svg>
                        <h2 class="col-titulo">Solicitudes</h2>
                    </div>
                    <span class="badge-count">{{ pendientesCount }} pendientes</span>
                </div>

                <div class="col-search">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
                        <circle cx="11" cy="11" r="8"/>
                        <line x1="21" y1="21" x2="16.65" y2="16.65"/>
                    </svg>
                    <input v-model="busquedaSol" type="text" placeholder="Buscar solicitud ....." class="search-input" />
                </div>

                <div v-if="loadingSol" class="col-loading"><div class="spinner-sm"></div></div>

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
                            <span>{{ formatHora(s.horaSalida) }}<template v-if="s.horaLlegada">–{{ formatHora(s.horaLlegada) }}</template></span>
                            <span>{{ s.cantidadColaboradores }} colab.</span>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ── Columna 2: Recursos disponibles ── -->
            <div class="col-panel">
                <div class="col-header">
                    <div class="col-header-left">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <rect x="1" y="3" width="15" height="13" rx="2"/>
                            <path d="M16 8h4l3 3v5h-7V8z"/>
                            <circle cx="5.5" cy="18.5" r="2.5"/>
                            <circle cx="18.5" cy="18.5" r="2.5"/>
                        </svg>
                        <h2 class="col-titulo">Recursos disponibles</h2>
                    </div>
                    <span class="badge-fecha">{{ formatFecha(solicitudSeleccionada?.fechaViaje ?? new Date()) }}</span>
                </div>

                <div class="col-search">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
                        <circle cx="11" cy="11" r="8"/>
                        <line x1="21" y1="21" x2="16.65" y2="16.65"/>
                    </svg>
                    <input v-model="busquedaRes" type="text" placeholder="Buscar vehículo o conductor ..." class="search-input" />
                </div>

                <div v-if="loadingRes" class="col-loading"><div class="spinner-sm"></div></div>

                <div v-else class="col-scroll">
                    <!-- Vehículos -->
                    <p class="recursos-grupo-label">VEHÍCULOS</p>
                    <div v-if="vehiculosFiltrados.length === 0" class="col-vacio">Sin vehículos registrados.</div>
                    <div
                        v-for="v in vehiculosFiltrados"
                        :key="'v'+v.id"
                        class="recurso-card"
                        :class="{ 'recurso-card-activo': vehiculoSeleccionado?.id === v.id }"
                        @click="seleccionarVehiculo(v)"
                    >
                        <div class="recurso-card-top">
                            <span class="recurso-placa">{{ v.placa }}</span>
                            <span class="badge" :class="v.disponible !== false ? 'badge-disponible' : 'badge-en-viaje'">
                                {{ v.disponible !== false ? 'Disponible' : 'En viaje' }}
                            </span>
                        </div>
                        <p class="recurso-sub">{{ v.marca }} {{ v.modelo }}</p>
                        <div class="recurso-meta">
                            <span>Capacidad <strong>{{ v.capacidad ?? '—' }} pasajeros</strong></span>
                            <span>Kilometraje <strong>{{ v.kilometraje ? v.kilometraje.toLocaleString('es-DO') + ' km' : '—' }}</strong></span>
                        </div>
                    </div>

                    <!-- Conductores -->
                    <p class="recursos-grupo-label" style="margin-top: 16px">CONDUCTORES</p>
                    <div v-if="conductoresFiltrados.length === 0" class="col-vacio">Sin conductores registrados.</div>
                    <div
                        v-for="c in conductoresFiltrados"
                        :key="'c'+c.id"
                        class="recurso-card recurso-card-conductor"
                        :class="{ 'recurso-card-activo': conductorSeleccionado?.id === c.id }"
                        @click="seleccionarConductor(c)"
                    >
                        <div class="recurso-card-top">
                            <div class="conductor-info">
                                <div class="conductor-avatar-sm">{{ iniciales(c.nombre, c.apellido) }}</div>
                                <span class="recurso-nombre">{{ c.nombre }} {{ c.apellido }}</span>
                            </div>
                            <span class="badge" :class="c.disponible !== false ? 'badge-disponible' : 'badge-en-viaje'">
                                {{ c.disponible !== false ? 'Disponible' : 'En viaje' }}
                            </span>
                        </div>
                        <div class="conductor-detalles">
                            <p class="recurso-sub">Lic. {{ c.tipoLicencia ?? 'Tipo B' }} • Vence {{ formatFecha(c.vencimientoLicencia) }}</p>
                            <div class="recurso-meta">
                                <span>Supervisor <strong>{{ c.supervisor ?? '—' }}</strong></span>
                                <span>Viajes este mes <strong>{{ c.viajesMes ?? 0 }}</strong></span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ── Columna 3: Panel de asignación ── -->
            <div class="col-panel col-panel-asig">
                <div class="col-header">
                    <div class="col-header-left">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <path d="M21.44 11.05l-9.19 9.19a6 6 0 0 1-8.49-8.49l9.19-9.19a4 4 0 0 1 5.66 5.66l-9.2 9.19a2 2 0 0 1-2.83-2.83l8.49-8.48"/>
                        </svg>
                        <h2 class="col-titulo">Panel de asignación</h2>
                    </div>
                </div>

                <!-- Sin selección -->
                <div v-if="!solicitudSeleccionada" class="panel-vacio">
                    <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="#d1d5db" stroke-width="1.2">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                        <polyline points="14 2 14 8 20 8"/>
                    </svg>
                    <p>Selecciona una solicitud<br>para comenzar</p>
                </div>

                <!-- Con selección -->
                <template v-else>
                    <!-- Solicitud seleccionada -->
                    <div class="panel-seccion">
                        <div class="panel-row-top">
                            <p class="panel-label">Solicitud seleccionada</p>
                            <span class="sol-id-badge">{{ formatNumero(solicitudSeleccionada.id) }}</span>
                        </div>
                        <p class="panel-ruta">
                            {{ solicitudSeleccionada.areaSolicitante }}
                            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align:middle;margin:0 4px">
                                <line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/>
                            </svg>
                            {{ solicitudSeleccionada.destino }}
                        </p>
                        <p class="panel-meta-sol">
                            {{ formatFecha(solicitudSeleccionada.fechaViaje) }}
                            &nbsp;·&nbsp;
                            {{ formatHora(solicitudSeleccionada.horaSalida) }}<template v-if="solicitudSeleccionada.horaLlegada">–{{ formatHora(solicitudSeleccionada.horaLlegada) }}</template>
                            &nbsp;·&nbsp;
                            {{ solicitudSeleccionada.cantidadColaboradores }} Colaboradores
                        </p>
                    </div>

                    <!-- Vehículo asignado -->
                    <div class="panel-seccion">
                        <p class="panel-label">Vehículo Asignado</p>
                        <div v-if="vehiculoSeleccionado" class="panel-card panel-card-vehiculo">
                            <p class="panel-card-titulo">{{ vehiculoSeleccionado.placa }} · {{ vehiculoSeleccionado.marca }} {{ vehiculoSeleccionado.modelo }}</p>
                            <p class="panel-card-sub">
                                {{ vehiculoSeleccionado.tipoVehiculo ?? 'Vehículo' }} · {{ vehiculoSeleccionado.capacidad ?? '—' }} pas. · {{ vehiculoSeleccionado.kilometraje ? vehiculoSeleccionado.kilometraje.toLocaleString('es-DO') + ' km' : '—' }}
                            </p>
                        </div>
                        <div v-else class="panel-card panel-card-placeholder">
                            <p>Selecciona un vehículo en la columna central</p>
                        </div>
                    </div>

                    <!-- Conductor asignado -->
                    <div class="panel-seccion">
                        <p class="panel-label">Conductor asignado</p>
                        <div v-if="conductorSeleccionado" class="panel-card panel-card-conductor">
                            <div class="panel-conductor-row">
                                <div class="conductor-avatar-md">{{ iniciales(conductorSeleccionado.nombre, conductorSeleccionado.apellido) }}</div>
                                <div>
                                    <p class="panel-card-titulo">{{ conductorSeleccionado.nombre }} {{ conductorSeleccionado.apellido }}</p>
                                    <p class="panel-card-sub">Lic. tipo {{ conductorSeleccionado.tipoLicencia ?? 'B' }} · Vence {{ formatFecha(conductorSeleccionado.vencimientoLicencia) }}</p>
                                </div>
                            </div>
                        </div>
                        <div v-else class="panel-card panel-card-placeholder">
                            <p>Selecciona un conductor en la columna central</p>
                        </div>
                    </div>

                    <!-- Validaciones -->
                    <div class="panel-seccion" v-if="vehiculoSeleccionado && conductorSeleccionado">
                        <p class="panel-label">Validaciones</p>
                        <div class="validaciones">
                            <div v-for="(v, i) in validaciones" :key="i" class="validacion-item" :class="{ 'val-ok': v.ok, 'val-fail': !v.ok }">
                                <svg v-if="v.ok" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                                    <polyline points="20 6 9 17 4 12"/>
                                </svg>
                                <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                                    <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
                                </svg>
                                {{ v.texto }}
                            </div>
                        </div>
                    </div>

                    <!-- Botón guardar -->
                    <div class="panel-acciones">
                        <button
                            class="btn-asignar"
                            :disabled="!puedeAsignar || guardando"
                            @click="guardarAsignacion"
                        >
                            <svg v-if="!guardando" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                                <polyline points="20 6 9 17 4 12"/>
                            </svg>
                            <div v-else class="spinner-btn"></div>
                            {{ guardando ? 'Guardando...' : 'Confirmar asignación' }}
                        </button>
                        <button class="btn-limpiar" @click="solicitudSeleccionada = null; vehiculoSeleccionado = null; conductorSeleccionado = null">
                            Limpiar
                        </button>
                    </div>
                </template>
            </div>

        </div><!-- /asig-board -->
    </div>
</template>

<style scoped>
/* ── Base ── */
.asig-page {
    padding: 28px 32px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

/* ── Encabezado ── */
.asig-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20px;
}

.asig-title {
    font-size: 1.45rem;
    font-weight: 700;
    color: #111827;
    letter-spacing: -0.02em;
    margin: 0;
}

.asig-header-actions {
    display: flex;
    gap: 10px;
}

.btn-historial {
    padding: 9px 20px;
    background: #fff;
    border: 1.5px solid #d1d5db;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #374151;
    cursor: pointer;
    transition: all .15s;
}

.btn-historial:hover,
.btn-historial.btn-activo {
    background: #f3f4f6;
    border-color: #9ca3af;
}

.btn-actualizar {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 20px;
    background: #1a3a2a;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}

.btn-actualizar:hover { background: #14532d; }

/* ── Notificaciones ── */
.notif {
    padding: 12px 18px;
    border-radius: 10px;
    font-size: .875rem;
    font-weight: 500;
    margin-bottom: 16px;
}

.notif-exito { background: #d1fae5; color: #065f46; border: 1px solid #6ee7b7; }
.notif-error { background: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

/* ── Board (3 columnas) ── */
.asig-board {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 16px;
    align-items: start;
}

/* ── Panel columna ── */
.col-panel {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    display: flex;
    flex-direction: column;
    overflow: hidden;
    max-height: calc(100vh - 140px);
}

.col-panel-asig {
    max-height: none;
}

.col-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 18px 18px 12px;
    border-bottom: 1px solid #f3f4f6;
    flex-shrink: 0;
}

.col-header-left {
    display: flex;
    align-items: center;
    gap: 8px;
    color: #374151;
}

.col-titulo {
    font-size: .95rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.badge-count {
    background: #fef3c7;
    color: #92400e;
    font-size: .72rem;
    font-weight: 700;
    padding: 3px 10px;
    border-radius: 20px;
}

.badge-fecha {
    background: #dbeafe;
    color: #1e40af;
    font-size: .72rem;
    font-weight: 700;
    padding: 3px 10px;
    border-radius: 20px;
}

.col-search {
    display: flex;
    align-items: center;
    gap: 8px;
    margin: 12px 14px;
    background: #f9fafb;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    padding: 0 12px;
    flex-shrink: 0;
}

.search-input {
    flex: 1;
    border: none;
    outline: none;
    font-size: .85rem;
    color: #111827;
    padding: 9px 0;
    background: transparent;
    font-family: inherit;
}

.search-input::placeholder { color: #9ca3af; }

.col-loading {
    display: flex;
    justify-content: center;
    padding: 32px;
}

.col-scroll {
    overflow-y: auto;
    padding: 0 14px 14px;
    flex: 1;
}

.col-vacio {
    text-align: center;
    color: #9ca3af;
    font-size: .85rem;
    padding: 24px 0;
}

/* ── Tarjeta solicitud ── */
.sol-card {
    border: 1.5px solid #e5e7eb;
    border-radius: 10px;
    padding: 14px;
    margin-bottom: 10px;
    cursor: pointer;
    transition: border-color .15s, background .15s;
}

.sol-card:hover { border-color: #9ca3af; background: #fafafa; }

.sol-card-activa {
    border-color: #1a3a2a !important;
    background: #f0fdf4 !important;
}

.sol-card-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 6px;
}

.sol-id {
    font-size: .8rem;
    font-weight: 700;
    color: #374151;
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

/* ── Recursos grupo ── */
.recursos-grupo-label {
    font-size: .68rem;
    font-weight: 700;
    letter-spacing: .08em;
    color: #9ca3af;
    margin: 10px 0 8px;
}

/* ── Tarjeta recurso ── */
.recurso-card {
    border: 1.5px solid #e5e7eb;
    border-radius: 10px;
    padding: 13px 14px;
    margin-bottom: 8px;
    cursor: pointer;
    transition: border-color .15s, background .15s;
}

.recurso-card:hover { border-color: #9ca3af; background: #fafafa; }

.recurso-card-activo {
    border-color: #1a3a2a !important;
    background: #f0fdf4 !important;
}

.recurso-card-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 4px;
}

.recurso-placa {
    font-size: .9rem;
    font-weight: 700;
    color: #111827;
}

.recurso-nombre {
    font-size: .9rem;
    font-weight: 700;
    color: #111827;
}

.recurso-sub {
    font-size: .78rem;
    color: #6b7280;
    margin: 2px 0 6px;
}

.recurso-meta {
    display: flex;
    gap: 16px;
    font-size: .75rem;
    color: #6b7280;
}

.recurso-meta strong { color: #374151; }

/* Conductor row en recurso */
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
    font-size: .65rem;
    font-weight: 700;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.conductor-detalles { padding-top: 2px; }

/* ── Badges ── */
.badge {
    display: inline-block;
    padding: 3px 9px;
    border-radius: 20px;
    font-size: .71rem;
    font-weight: 700;
    white-space: nowrap;
}

.badge-pendiente  { background: #fef3c7; color: #92400e; }
.badge-aprobada   { background: #d1fae5; color: #065f46; }
.badge-rechazada  { background: #fee2e2; color: #991b1b; }
.badge-cancelada  { background: #dbeafe; color: #1e40af; }
.badge-finalizada { background: #ede9fe; color: #6d28d9; }
.badge-disponible { background: #d1fae5; color: #065f46; }
.badge-en-viaje   { background: #dbeafe; color: #1e40af; }

/* ── Panel asignación ── */
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

.panel-seccion {
    padding: 16px 18px;
    border-bottom: 1px solid #f3f4f6;
}

.panel-seccion:last-child { border-bottom: none; }

.panel-row-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 6px;
}

.panel-label {
    font-size: .72rem;
    font-weight: 700;
    color: #9ca3af;
    text-transform: uppercase;
    letter-spacing: .07em;
    margin: 0 0 8px;
}

.sol-id-badge {
    font-size: .8rem;
    font-weight: 700;
    color: #374151;
    background: #f3f4f6;
    padding: 2px 8px;
    border-radius: 6px;
}

.panel-ruta {
    font-size: 1rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 5px;
}

.panel-meta-sol {
    font-size: .78rem;
    color: #6b7280;
    margin: 0;
}

/* Cards del panel */
.panel-card {
    border-radius: 10px;
    padding: 12px 14px;
}

.panel-card-vehiculo {
    background: #f0fdf4;
    border: 1.5px solid #bbf7d0;
}

.panel-card-conductor {
    background: #f0fdf4;
    border: 1.5px solid #bbf7d0;
}

.panel-card-placeholder {
    background: #f9fafb;
    border: 1.5px dashed #d1d5db;
    color: #9ca3af;
    font-size: .82rem;
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

/* ── Validaciones ── */
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

.val-ok   { color: #065f46; }
.val-fail { color: #991b1b; }

/* ── Acciones panel ── */
.panel-acciones {
    padding: 16px 18px;
    display: flex;
    flex-direction: column;
    gap: 8px;
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

.btn-asignar:disabled {
    opacity: .45;
    cursor: default;
}

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

/* ── Historial ── */
.historial-wrap {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    overflow: hidden;
}

.hist-tabla {
    width: 100%;
    border-collapse: collapse;
    font-size: .875rem;
}

.hist-tabla th {
    padding: 13px 16px;
    text-align: left;
    font-size: .72rem;
    font-weight: 600;
    color: #9ca3af;
    letter-spacing: .05em;
    border-bottom: 1.5px solid #f3f4f6;
}

.hist-tabla td {
    padding: 13px 16px;
    color: #374151;
    border-bottom: 1px solid #f9fafb;
    vertical-align: middle;
}

.hist-tabla tbody tr:last-child td { border-bottom: none; }
.hist-tabla tbody tr:hover { background: #fafafa; }
.td-id { font-weight: 700; color: #111827; }

/* ── Spinners ── */
.spinner {
    width: 32px; height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

.spinner-sm {
    width: 22px; height: 22px;
    border: 2.5px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

.spinner-btn {
    width: 16px; height: 16px;
    border: 2px solid rgba(255,255,255,.4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

.estado-carga {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 60px 0;
    color: #6b7280;
}

.estado-vacio {
    text-align: center;
    padding: 48px 0;
    color: #9ca3af;
    font-size: .9rem;
}

@keyframes spin { to { transform: rotate(360deg); } }

/* ── Responsive ── */
@media (max-width: 1100px) {
    .asig-board { grid-template-columns: 1fr 1fr; }
    .col-panel:last-child { grid-column: 1 / -1; max-height: none; }
}

@media (max-width: 700px) {
    .asig-page  { padding: 16px; }
    .asig-board { grid-template-columns: 1fr; }
    .col-panel  { max-height: 400px; }
    .asig-header { flex-direction: column; align-items: flex-start; gap: 12px; }
}
</style>