<script setup>
import { ref, computed } from 'vue'

// ── Estado ────────────────────────────────────────────────
const vistaActiva = ref('mes') // 'dia' | 'semana' | 'mes'
const hoy = new Date()
const fechaActual = ref(new Date(hoy.getFullYear(), hoy.getMonth(), 1))

// ── Filtros activos ───────────────────────────────────────
const filtros = ref({ vehiculos: false, conductores: false, pendientes: false, cancelados: false })

function toggleFiltro(key) {
    filtros.value[key] = !filtros.value[key]
}

// ── Datos mock ────────────────────────────────────────────
const viajes = ref([
    { id: 1,  titulo: 'Reunión MICI',         fecha: new Date(hoy.getFullYear(), hoy.getMonth(), 2),  horaInicio: '08:00', horaFin: '09:30', vehiculo: 'Toyota Hiace', placa: 'A123BC', conductor: 'Carlos M.', estado: 'programado',  tipo: 'normal'  },
    { id: 2,  titulo: 'Aeropuerto',            fecha: new Date(hoy.getFullYear(), hoy.getMonth(), 6),  horaInicio: '08:00', horaFin: '09:30', vehiculo: 'Toyota Hiace', placa: 'A123BC', conductor: 'Carlos M.', estado: 'en_viaje',   tipo: 'normal'  },
    { id: 3,  titulo: 'Norte',                 fecha: new Date(hoy.getFullYear(), hoy.getMonth(), 6),  horaInicio: '14:00', horaFin: '15:30', vehiculo: 'Kia Sorento',  placa: 'B456DE', conductor: 'Pedro R.',  estado: 'programado',  tipo: 'normal'  },
    { id: 4,  titulo: 'Puerto',                fecha: new Date(hoy.getFullYear(), hoy.getMonth(), 14), horaInicio: '07:00', horaFin: '09:00', vehiculo: 'Toyota Hiace', placa: 'A123BC', conductor: 'Carlos M.', estado: 'en_viaje',   tipo: 'normal'  },
    { id: 5,  titulo: 'Zona Franca Este',      fecha: new Date(hoy.getFullYear(), hoy.getMonth(), 14), horaInicio: '13:50', horaFin: '15:00', vehiculo: 'Toyota Hiace', placa: 'A123BC', conductor: 'Carlos M.', estado: 'programado',  tipo: 'normal'  },
    { id: 6,  titulo: 'Urgente',               fecha: new Date(hoy.getFullYear(), hoy.getMonth(), 15), horaInicio: '08:00', horaFin: '09:30', vehiculo: 'Kia Sorento',  placa: 'B456DE', conductor: 'Pedro R.',  estado: 'pendiente',   tipo: 'urgente' },
    { id: 7,  titulo: 'Congreso Nacional',     fecha: new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate()), horaInicio: '08:00', horaFin: '09:30', vehiculo: 'Toyota Hiace', placa: 'A123BC', conductor: 'Carlos M.', estado: 'en_viaje',   tipo: 'normal'  },
    { id: 8,  titulo: 'Zona Franca Este',      fecha: new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate()), horaInicio: '08:00', horaFin: '09:30', vehiculo: 'Toyota Hiace', placa: 'A123BC', conductor: 'Carlos M.', estado: 'programado',  tipo: 'normal'  },
    { id: 9,  titulo: 'Urgente - Presidencia', fecha: new Date(hoy.getFullYear(), hoy.getMonth(), 19), horaInicio: '08:30', horaFin: '10:00', vehiculo: 'Toyota Hiace', placa: 'A123BC', conductor: 'Carlos M.', estado: 'espera',      tipo: 'urgente' },
    { id: 10, titulo: 'Urgente - Presidencia', fecha: new Date(hoy.getFullYear(), hoy.getMonth(), 20), horaInicio: '07:30', horaFin: '09:00', vehiculo: 'Toyota Hiace', placa: 'A123BC', conductor: 'Carlos M.', estado: 'cancelado',   tipo: 'urgente' },
])

// ── Navegación de mes ─────────────────────────────────────
function mesAnterior() {
    fechaActual.value = new Date(fechaActual.value.getFullYear(), fechaActual.value.getMonth() - 1, 1)
}
function mesSiguiente() {
    fechaActual.value = new Date(fechaActual.value.getFullYear(), fechaActual.value.getMonth() + 1, 1)
}
function irAHoy() {
    fechaActual.value = new Date(hoy.getFullYear(), hoy.getMonth(), 1)
}

// ── Helpers ───────────────────────────────────────────────
const MESES = ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre']
const DIAS  = ['Dom','Lun','Mar','Mié','Jue','Vie','Sáb']

function nombreMes(fecha) {
    return `${MESES[fecha.getMonth()]} ${fecha.getFullYear()}`
}

function esMismaFecha(a, b) {
    return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate()
}

function esMesActual(fecha) {
    return fecha.getMonth() === fechaActual.value.getMonth() && fecha.getFullYear() === fechaActual.value.getFullYear()
}

function formatHora(h) { return h?.substring(0,5) ?? '' }

function formatFechaCorta(d) {
    return d.toLocaleDateString('es-DO', { day: '2-digit', month: 'long' })
}

// ── Celdas del calendario ─────────────────────────────────
const celdasMes = computed(() => {
    const año  = fechaActual.value.getFullYear()
    const mes  = fechaActual.value.getMonth()
    const primero = new Date(año, mes, 1)
    const ultimo  = new Date(año, mes + 1, 0)
    const celdas  = []

    // días del mes anterior
    for (let i = 0; i < primero.getDay(); i++) {
        celdas.push(new Date(año, mes, -primero.getDay() + i + 1))
    }
    // días del mes
    for (let d = 1; d <= ultimo.getDate(); d++) {
        celdas.push(new Date(año, mes, d))
    }
    // completar hasta múltiplo de 7
    while (celdas.length % 7 !== 0) {
        celdas.push(new Date(año, mes + 1, celdas.length - ultimo.getDate() - primero.getDay() + 1))
    }
    return celdas
})

function viajesEnFecha(fecha) {
    return viajes.value.filter(v => esMismaFecha(v.fecha, fecha))
}

// ── Resumen del mes ───────────────────────────────────────
const resumen = computed(() => {
    const mes = viajes.value.filter(v =>
        v.fecha.getMonth() === fechaActual.value.getMonth() &&
        v.fecha.getFullYear() === fechaActual.value.getFullYear()
    )
    return {
        total:      mes.length,
        completados: mes.filter(v => v.estado === 'programado').length,
        pendientes:  mes.filter(v => v.estado === 'pendiente' || v.estado === 'espera').length,
        cancelados:  mes.filter(v => v.estado === 'cancelado').length,
    }
})

// ── Viajes de hoy ─────────────────────────────────────────
const viajesHoy = computed(() => viajes.value.filter(v => esMismaFecha(v.fecha, hoy)))

// ── Próximos viajes (después de hoy) ─────────────────────
const proximosViajes = computed(() =>
    viajes.value
        .filter(v => v.fecha > hoy && !esMismaFecha(v.fecha, hoy))
        .sort((a, b) => a.fecha - b.fecha)
        .slice(0, 6)
)

// ── Estilo de chip en calendario ──────────────────────────
function chipClase(viaje) {
    if (viaje.tipo === 'urgente') return 'chip-urgente'
    if (viaje.estado === 'en_viaje') return 'chip-en-viaje'
    return 'chip-normal'
}

// ── Estilo borde lateral tarjeta ─────────────────────────
function bordeClase(estado) {
    const map = {
        en_viaje:  'borde-en-viaje',
        programado:'borde-programado',
        pendiente: 'borde-pendiente',
        espera:    'borde-espera',
        cancelado: 'borde-cancelado',
    }
    return map[estado] ?? 'borde-programado'
}

function badgeEstado(estado) {
    const map = {
        en_viaje:  { label: 'En viaje',   clase: 'badge-en-viaje-pill'  },
        programado:{ label: 'Programado', clase: 'badge-programado-pill' },
        pendiente: { label: 'Pendiente',  clase: 'badge-pendiente-pill'  },
        espera:    { label: 'Espera',     clase: 'badge-espera-pill'     },
        cancelado: { label: 'Cancelado',  clase: 'badge-cancelado-pill'  },
    }
    return map[estado] ?? { label: estado, clase: '' }
}
</script>

<template>
    <div class="agenda-page">

        <!-- ── Encabezado ── -->
        <div class="agenda-header">
            <div>
                <h1 class="agenda-title">Agenda y Calendario</h1>
                <p class="agenda-sub">Sistema de transporte institucional</p>
            </div>
            <div class="vista-tabs">
                <button class="tab-btn" :class="{ 'tab-activo': vistaActiva === 'dia' }"    @click="vistaActiva = 'dia'">Día</button>
                <button class="tab-btn" :class="{ 'tab-activo': vistaActiva === 'semana' }" @click="vistaActiva = 'semana'">Semana</button>
                <button class="tab-btn" :class="{ 'tab-activo': vistaActiva === 'mes' }"    @click="vistaActiva = 'mes'">Mes</button>
            </div>
        </div>

        <!-- ── Layout principal ── -->
        <div class="agenda-layout">

            <!-- ═══ COLUMNA IZQUIERDA: Calendario ═══ -->
            <div class="cal-panel">

                <!-- Navegación de mes -->
                <div class="cal-nav">
                    <button class="nav-btn" @click="mesAnterior">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                            <polyline points="15 18 9 12 15 6"/>
                        </svg>
                    </button>
                    <span class="cal-mes-label">{{ nombreMes(fechaActual) }}</span>
                    <button class="nav-btn" @click="mesSiguiente">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                            <polyline points="9 18 15 12 9 6"/>
                        </svg>
                    </button>
                    <button class="btn-hoy" @click="irAHoy">Hoy</button>
                </div>

                <!-- Filtros -->
                <div class="filtros-row">
                    <button class="filtro-btn filtro-vehiculos" :class="{ activo: filtros.vehiculos }" @click="toggleFiltro('vehiculos')">Vehículos</button>
                    <button class="filtro-btn filtro-conductores" :class="{ activo: filtros.conductores }" @click="toggleFiltro('conductores')">Conductores</button>
                    <button class="filtro-btn filtro-pendientes" :class="{ activo: filtros.pendientes }" @click="toggleFiltro('pendientes')">Pendientes</button>
                    <button class="filtro-btn filtro-cancelados" :class="{ activo: filtros.cancelados }" @click="toggleFiltro('cancelados')">Cancelados</button>
                </div>

                <!-- Grid del mes -->
                <div class="cal-grid">
                    <!-- Cabecera días -->
                    <div class="cal-day-header" v-for="dia in DIAS" :key="dia">{{ dia }}</div>

                    <!-- Celdas -->
                    <div
                        v-for="(fecha, i) in celdasMes"
                        :key="i"
                        class="cal-celda"
                        :class="{
                            'celda-otro-mes': !esMesActual(fecha),
                            'celda-hoy': esMismaFecha(fecha, hoy)
                        }"
                    >
                        <span class="celda-numero">{{ fecha.getDate() }}</span>
                        <div class="celda-chips">
                            <div
                                v-for="viaje in viajesEnFecha(fecha).slice(0,2)"
                                :key="viaje.id"
                                class="cal-chip"
                                :class="chipClase(viaje)"
                                :title="viaje.titulo"
                            >
                                {{ formatHora(viaje.horaInicio) }} {{ viaje.titulo.substring(0,10) }}
                            </div>
                            <div v-if="viajesEnFecha(fecha).length > 2" class="chip-mas">
                                +{{ viajesEnFecha(fecha).length - 2 }} más
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ═══ COLUMNA DERECHA: Panel lateral ═══ -->
            <div class="side-panel">

                <!-- Resumen del mes -->
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

                <!-- Viajes de hoy -->
                <div class="side-section" v-if="viajesHoy.length > 0">
                    <h3 class="side-titulo">Viajes para hoy &mdash; {{ formatFechaCorta(hoy) }}</h3>
                    <div class="viajes-lista">
                        <div v-for="viaje in viajesHoy" :key="viaje.id" class="viaje-card" :class="bordeClase(viaje.estado)">
                            <div class="viaje-hora">{{ viaje.horaInicio }} - {{ viaje.horaFin }}</div>
                            <div class="viaje-titulo">{{ viaje.titulo }}</div>
                            <div class="viaje-recurso">{{ viaje.vehiculo }} · {{ viaje.placa }} / {{ viaje.conductor }}</div>
                            <span class="badge-pill" :class="badgeEstado(viaje.estado).clase">
                                {{ badgeEstado(viaje.estado).label }}
                            </span>
                        </div>
                    </div>
                </div>

                <!-- Próximos viajes -->
                <div class="side-section" v-if="proximosViajes.length > 0">
                    <h3 class="side-titulo">Próximos Viajes</h3>
                    <div class="viajes-lista">
                        <div v-for="viaje in proximosViajes" :key="viaje.id" class="viaje-card" :class="bordeClase(viaje.estado)">
                            <div class="viaje-hora">
                                {{ viaje.fecha.getDate() }} {{ MESES[viaje.fecha.getMonth()].substring(0,3) }}
                                &mdash; {{ viaje.horaInicio }}
                            </div>
                            <div class="viaje-titulo">{{ viaje.titulo }}</div>
                            <div class="viaje-recurso">{{ viaje.vehiculo }} · {{ viaje.placa }} / {{ viaje.conductor }}</div>
                            <span class="badge-pill" :class="badgeEstado(viaje.estado).clase">
                                {{ badgeEstado(viaje.estado).label }}
                            </span>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</template>

<style scoped>
/* ── Base ── */
.agenda-page {
    padding: 28px 32px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

/* ── Encabezado ── */
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

/* ── Tabs de vista ── */
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

.tab-btn:hover { background: #f9fafb; color: #374151; }

.tab-activo {
    background: #1a3a2a !important;
    color: #fff !important;
}

/* ── Layout ── */
.agenda-layout {
    display: grid;
    grid-template-columns: 1fr 340px;
    gap: 16px;
    align-items: start;
}

/* ═══ CALENDARIO ═══ */
.cal-panel {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    overflow: hidden;
}

/* Navegación del mes */
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
.nav-btn:hover { background: #f3f4f6; }

.cal-mes-label {
    font-size: .95rem;
    font-weight: 700;
    color: #111827;
    flex: 1;
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
.btn-hoy:hover { background: #14532d; }

/* Filtros */
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

.filtro-vehiculos  { border-color: #93c5fd; color: #1e40af; }
.filtro-vehiculos.activo  { background: #dbeafe; }
.filtro-conductores{ border-color: #6ee7b7; color: #065f46; }
.filtro-conductores.activo { background: #d1fae5; }
.filtro-pendientes { border-color: #fca5a5; color: #991b1b; }
.filtro-pendientes.activo  { background: #fee2e2; }
.filtro-cancelados { border-color: #d1d5db; color: #374151; }
.filtro-cancelados.activo  { background: #f3f4f6; }

/* Grid del calendario */
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
}

.cal-celda:nth-child(7n) { border-right: none; }

.celda-numero {
    display: block;
    font-size: .8rem;
    font-weight: 600;
    color: #374151;
    margin-bottom: 4px;
}

.celda-otro-mes .celda-numero { color: #d1d5db; }

.celda-hoy {
    background: #f0fdf4;
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

/* Chips en celdas */
.celda-chips {
    display: flex;
    flex-direction: column;
    gap: 2px;
}

.cal-chip {
    font-size: .68rem;
    font-weight: 600;
    padding: 2px 6px;
    border-radius: 4px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    cursor: pointer;
}

.chip-normal   { background: #a7f3d0; color: #065f46; }
.chip-en-viaje { background: #bfdbfe; color: #1e40af; }
.chip-urgente  { background: #fecaca; color: #991b1b; }

.chip-mas {
    font-size: .65rem;
    color: #9ca3af;
    padding: 1px 4px;
    font-weight: 600;
}

/* ═══ PANEL LATERAL ═══ */
.side-panel {
    display: flex;
    flex-direction: column;
    gap: 14px;
}

.side-section {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    padding: 18px 18px 14px;
}

.side-titulo {
    font-size: .9rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 14px;
}

/* Resumen */
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

.azul  { color: #2563eb; }
.verde { color: #16a34a; }
.rojo  { color: #dc2626; }
.gris  { color: #374151; }

/* Lista de viajes */
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

.borde-en-viaje   { border-left-color: #2563eb; }
.borde-programado { border-left-color: #16a34a; }
.borde-pendiente  { border-left-color: #d97706; }
.borde-espera     { border-left-color: #9ca3af; }
.borde-cancelado  { border-left-color: #dc2626; }

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

/* Badges de estado */
.badge-pill {
    display: inline-block;
    padding: 2px 10px;
    border-radius: 20px;
    font-size: .68rem;
    font-weight: 700;
    align-self: flex-start;
}

.badge-en-viaje-pill   { background: #dbeafe; color: #1e40af; }
.badge-programado-pill { background: #d1fae5; color: #065f46; }
.badge-pendiente-pill  { background: #fef3c7; color: #92400e; }
.badge-espera-pill     { background: #f3f4f6; color: #374151; }
.badge-cancelado-pill  { background: #fee2e2; color: #991b1b; }

/* ── Responsive ── */
@media (max-width: 1100px) {
    .agenda-layout { grid-template-columns: 1fr; }
    .side-panel { flex-direction: row; flex-wrap: wrap; }
    .side-section { flex: 1; min-width: 280px; }
}

@media (max-width: 700px) {
    .agenda-page { padding: 16px; }
    .agenda-header { flex-direction: column; align-items: flex-start; gap: 12px; }
    .cal-celda { min-height: 60px; }
    .side-panel { flex-direction: column; }
}
</style>