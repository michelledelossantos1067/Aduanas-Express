<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import {
    getReporteViajes,
    getReporteConsumo,
    getReporteSolicitudes,
    getReporteConductores,
} from '@/services/reporteService.js'
import api from '@/utils/axiosConfig'
import ReporteConfigModal from './ReporteConfigModal.vue'
import { useReporteConfig } from './composables/useReporteConfig.js'
import { usePermisos } from '@/composables/usePermisos'


const { puede } = usePermisos()
const tabActiva = ref('viajes')
const loading = ref(false)
const exportando = ref('')
const errorMsg = ref('')

const hoy = new Date()
const mesRef = ref(hoy.getMonth() + 1)
const añoRef = ref(hoy.getFullYear())
const mostrarConfig = ref(false)
const { config, aplicar, queryParams } = useReporteConfig()

const MESES = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre',
]
const AÑOS = Array.from({ length: 5 }, (_, i) => hoy.getFullYear() - i)

const periodoLabel = computed(() => `${MESES[mesRef.value - 1]} ${añoRef.value}`)
const conPeriodo = computed(() => ['viajes', 'consumo'].includes(tabActiva.value))

// ── Datos por tab ──────────────────────────────────────────
const datosViajes = ref(null)
const datosConsumo = ref(null)
const datosSolicitudes = ref(null)
const datosConductores = ref(null)

const datos = computed(() => ({
    viajes: datosViajes.value,
    consumo: datosConsumo.value,
    solicitudes: datosSolicitudes.value,
    conductores: datosConductores.value,
})[tabActiva.value])

// ── Formatters ─────────────────────────────────────────────
function fFecha(f) {
    if (!f) return '—'
    return new Date(f).toLocaleDateString('es-DO', {
        day: '2-digit', month: '2-digit', year: 'numeric',
    })
}
function fDinero(n) {
    return (parseFloat(n) || 0).toLocaleString('es-DO', {
        style: 'currency', currency: 'DOP', maximumFractionDigits: 0,
    })
}
function fNum(id) { return `#${String(id).padStart(4, '0')}` }

// ── Carga de datos ─────────────────────────────────────────
async function cargar() {
    loading.value = true
    errorMsg.value = ''
    try {
        if (tabActiva.value === 'viajes') {
            const r = await getReporteViajes(mesRef.value, añoRef.value)
            datosViajes.value = r.data
        } else if (tabActiva.value === 'consumo') {
            const r = await getReporteConsumo(mesRef.value, añoRef.value)
            datosConsumo.value = r.data
        } else if (tabActiva.value === 'solicitudes') {
            const r = await getReporteSolicitudes()
            datosSolicitudes.value = r.data
        } else if (tabActiva.value === 'conductores') {
            const r = await getReporteConductores()
            datosConductores.value = r.data
        }
    } catch (e) {
        console.error(e)
        errorMsg.value = 'No se pudo cargar el reporte. Verifica tu conexión e intenta de nuevo.'
    } finally {
        loading.value = false
    }
}

watch([tabActiva, mesRef, añoRef], cargar, { immediate: false })
onMounted(cargar)


async function descargar(formato) {
    exportando.value = formato
    errorMsg.value = ''
    try {
        const tab = tabActiva.value
        const mes = mesRef.value
        const año = añoRef.value
        const base = '/reportes'

        // Params base de período
        const periodoParams = conPeriodo.value ? `mes=${mes}&anio=${año}` : ''

        // Params de estilo (vienen del composable)
        const cfgP = queryParams()
        const extra = `estilo=${cfgP.estilo}&colorPrimary=${encodeURIComponent(cfgP.colorPrimary)}&colorAccent=${encodeURIComponent(cfgP.colorAccent)}`

        function url(segmento) {
            const q = [periodoParams, extra].filter(Boolean).join('&')
            return `${base}/${segmento}?${q}`
        }

        const rutas = {
            viajes: { pdf: url('viajes/pdf'), excel: url('viajes/excel') },
            consumo: { pdf: url('consumo/pdf'), excel: url('consumo/excel') },
            solicitudes: { pdf: url('solicitudes/pdf'), excel: url('solicitudes/excel') },
            conductores: { pdf: url('conductores/pdf'), excel: url('conductores/excel') },
        }

        const mimeType = formato === 'pdf'
            ? 'application/pdf'
            : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        const ext = formato === 'pdf' ? 'pdf' : 'xlsx'
        const nombre = conPeriodo.value ? `${tab}_${mes}_${año}.${ext}` : `${tab}.${ext}`

        const res = await api.get(rutas[tab][formato], { responseType: 'blob' })
        const blob = res.data
        const href = URL.createObjectURL(new Blob([blob], { type: mimeType }))
        const a = Object.assign(document.createElement('a'), { href, download: nombre })
        a.click()
        URL.revokeObjectURL(href)
    } catch (e) {
        console.error(e)
        errorMsg.value = `Error al exportar ${formato.toUpperCase()}. Intenta de nuevo.`
    } finally {
        exportando.value = ''
    }
}
// ── KPIs por tab ───────────────────────────────────────────
const kpis = computed(() => {
    const d = datos.value
    if (!d) return []

    if (tabActiva.value === 'viajes') return [
        { label: 'Total viajes', valor: d.totalViajes ?? 0, icon: 'truck', acento: 'azul' },
        { label: 'Finalizados', valor: d.completados ?? 0, icon: 'check', acento: 'verde' },
        { label: 'Pendientes / Aprobados', valor: d.pendientes ?? 0, icon: 'clock', acento: 'ambar' },
        { label: 'Cancelados', valor: d.cancelados ?? 0, icon: 'x', acento: 'rojo' },
        { label: 'Pasajeros', valor: d.totalPasajeros ?? 0, icon: 'users', acento: 'neutro' },
    ]

    if (tabActiva.value === 'consumo') return [
        { label: 'Costo total', valor: fDinero(d.costoTotal ?? 0), icon: 'money', acento: 'ambar' },
        { label: 'Galones consumidos', valor: `${(d.totalGalones ?? 0).toLocaleString('es-DO')} gal.`, icon: 'fuel', acento: 'ambar' },
        { label: 'Costo prom. / galón', valor: fDinero(d.costoPromedioGalon ?? 0), icon: 'calc', acento: 'neutro' },
        { label: 'Vehículos', valor: d.totalVehiculos ?? 0, icon: 'truck', acento: 'azul' },
    ]

    if (tabActiva.value === 'solicitudes') return [
        { label: 'Total', valor: d.total ?? 0, icon: 'docs', acento: 'azul' },
        { label: 'Aprobadas', valor: d.aprobadas ?? 0, icon: 'check', acento: 'verde' },
        { label: 'Pendientes', valor: d.pendientes ?? 0, icon: 'clock', acento: 'ambar' },
        { label: 'Rechazadas', valor: d.rechazadas ?? 0, icon: 'x', acento: 'rojo' },
        { label: 'Canceladas', valor: d.canceladas ?? 0, icon: 'x', acento: 'rojo' },
        { label: 'Finalizadas', valor: d.finalizadas ?? 0, icon: 'check', acento: 'verde' },
    ]

    if (tabActiva.value === 'conductores') return [
        { label: 'Conductores activos', valor: d.totalConductores ?? 0, icon: 'user', acento: 'azul' },
        { label: 'Total de viajes', valor: d.totalViajes ?? 0, icon: 'truck', acento: 'neutro' },
        { label: 'Pasajeros totales', valor: d.totalPasajeros ?? 0, icon: 'users', acento: 'neutro' },
        { label: 'Prom. pasajeros/viaje', valor: (d.promedioPasajerosPorViaje ?? 0).toFixed(1), icon: 'calc', acento: 'verde' },
    ]

    return []
})

// ── Configuración de tabla por tab ─────────────────────────
const tablaConfig = computed(() => {
    if (tabActiva.value === 'viajes') return {
        cols: ['#', 'Área', 'Destino', 'Fecha', 'Conductor', 'Vehículo', 'Pas.', 'Estado'],
        rows: (datos.value?.detalles ?? []).map(r => [
            fNum(r.id),
            r.areaSolicitante ?? '—',
            r.destino ?? '—',
            fFecha(r.fechaViaje),
            r.nombreConductor ?? 'Sin asignar',
            r.vehiculoPlaca ?? 'Sin asignar',
            r.cantidadPasajeros,
            r.estado,
        ]),
        estadoIdx: 7,
        monoIdx: [5],   // placa en mono
        numIdx: [0, 6],
    }

    if (tabActiva.value === 'consumo') return {
        cols: ['Vehículo', 'Placa', 'Galones', 'Costo total', 'Registros'],
        rows: (datos.value?.detalles ?? []).map(r => [
            r.vehiculoMarca ?? '—',
            r.vehiculoPlaca ?? '—',
            `${parseFloat(r.totalGalones ?? 0).toLocaleString('es-DO', { minimumFractionDigits: 1 })} gal.`,
            fDinero(r.costoTotal),
            r.totalRegistros,
        ]),
        estadoIdx: -1,
        monoIdx: [1],
        numIdx: [2, 3, 4],
    }

    if (tabActiva.value === 'solicitudes') return {
        cols: ['#', 'Área', 'Destino', 'Fecha de viaje', 'Pas.', 'Estado'],
        rows: (datos.value?.detalles ?? []).map(r => [
            fNum(r.id),
            r.areaSolicitante ?? '—',
            r.destino ?? '—',
            fFecha(r.fechaViaje),
            r.cantidadPasajeros,
            r.estado,
        ]),
        estadoIdx: 5,
        monoIdx: [],
        numIdx: [0, 4],
    }

    if (tabActiva.value === 'conductores') return {
        cols: ['Conductor', 'Núm. licencia', 'Viajes', 'Pasajeros', 'Último viaje'],
        rows: (datos.value?.detalles ?? []).map(r => [
            r.nombreConductor ?? '—',
            r.licencia ?? '—',
            r.totalViajes,
            r.totalPasajeros,
            fFecha(r.ultimoViaje),
        ]),
        estadoIdx: -1,
        monoIdx: [1],
        numIdx: [2, 3],
    }

    return { cols: [], rows: [], estadoIdx: -1, monoIdx: [], numIdx: [] }
})

function estadoClase(val) {
    const v = (val ?? '').toLowerCase()
    if (['finaliz', 'complet', 'aprobad'].some(x => v.includes(x))) return 'badge-ok'
    if (['pendiente', 'espera'].some(x => v.includes(x))) return 'badge-ambar'
    if (['cancel', 'rechaz'].some(x => v.includes(x))) return 'badge-rojo'
    if (['viaje', 'proceso', 'asign'].some(x => v.includes(x))) return 'badge-azul'
    return 'badge-gris'
}

const TABS = [
    { key: 'viajes', label: 'Viajes' },
    { key: 'consumo', label: 'Consumo' },
    { key: 'solicitudes', label: 'Solicitudes' },
    { key: 'conductores', label: 'Conductores' },
]
</script>

<template>
    <div class="rp">

        <!-- ── ENCABEZADO ────────────────────────────────────── -->
        <div class="rp-header">
            <div class="rp-header-texto">
                <p class="rp-empresa">AduanasExpress · Sistema de Gestión de Transporte</p>
                <h1 class="rp-titulo">Reportes</h1>
            </div>
            <div class="rp-header-acciones">

                <button class="btn-cfg" @click="mostrarConfig = true">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <circle cx="12" cy="12" r="3" />
                        <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1-2.83 2.83l-.06-.06
                 a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09
                 A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83-2.83
                 l.06-.06A1.65 1.65 0 0 0 4.68 15a1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09
                 A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 2.83-2.83
                 l.06.06A1.65 1.65 0 0 0 9 4.68a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09
                 a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 2.83
                 l-.06.06A1.65 1.65 0 0 0 19.4 9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09
                 a1.65 1.65 0 0 0-1.51 1z" />
                    </svg>
                    Diseño
                </button>
                <!-- Solo Admin y Supervisor pueden exportar reportes -->
                <button v-if="puede.exportarReportes.value" class="btn-exp btn-xlsx" :disabled="exportando !== ''" @click="descargar('excel')">
                    <span v-if="exportando === 'excel'" class="spin spin-ok"></span>
                    <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
                        <polyline points="14 2 14 8 20 8" />
                        <polyline points="8 13 12 17 16 13" />
                        <line x1="12" y1="17" x2="12" y2="7" />
                    </svg>
                    Exportar Excel
                </button>
                <!-- Solo Admin y Supervisor pueden exportar reportes -->
                <button v-if="puede.exportarReportes.value" class="btn-exp btn-pdf" :disabled="exportando !== ''" @click="descargar('pdf')">
                    <span v-if="exportando === 'pdf'" class="spin spin-pdf"></span>
                    <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
                        <polyline points="14 2 14 8 20 8" />
                        <polyline points="8 13 12 17 16 13" />
                        <line x1="12" y1="17" x2="12" y2="7" />
                    </svg>
                    Exportar PDF
                </button>
                <button class="btn-reload" @click="cargar">
                    <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.3">
                        <polyline points="1 4 1 10 7 10" />
                        <path d="M3.51 15a9 9 0 1 0 .49-4.95" />
                    </svg>
                    Actualizar
                </button>
            </div>
        </div>

        <!-- ── ALERTA DE ERROR ───────────────────────────────── -->
        <div v-if="errorMsg" class="alerta-error">
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="12" />
                <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            {{ errorMsg }}
        </div>

        <!-- ── BARRA DE TABS + PERÍODO ───────────────────────── -->
        <div class="tabs-bar">
            <div class="tabs-lista">
                <button v-for="tab in TABS" :key="tab.key" class="tab-btn"
                    :class="{ 'tab-activo': tabActiva === tab.key }" @click="tabActiva = tab.key">
                    {{ tab.label }}
                </button>
            </div>

            <div v-if="conPeriodo" class="periodo-grupo">
                <label class="periodo-label">Período:</label>
                <select v-model="mesRef" class="periodo-sel">
                    <option v-for="(m, i) in MESES" :key="i" :value="i + 1">{{ m }}</option>
                </select>
                <select v-model="añoRef" class="periodo-sel">
                    <option v-for="a in AÑOS" :key="a">{{ a }}</option>
                </select>
            </div>
        </div>

        <!-- ── KPIs ──────────────────────────────────────────── -->
        <div v-if="kpis.length" class="kpi-grid" :class="`kpi-cols-${Math.min(kpis.length, 4)}`">
            <div v-for="(k, i) in kpis" :key="i" class="kpi-card">
                <div class="kpi-acento" :class="`acento-${k.acento}`"></div>
                <div class="kpi-body">
                    <span class="kpi-valor">{{ k.valor }}</span>
                    <span class="kpi-label">{{ k.label }}</span>
                </div>
            </div>
        </div>

        <!-- ── TABLA ──────────────────────────────────────────── -->
        <div class="tabla-card">
            <!-- Cabecera de la tabla -->
            <div class="tabla-cabecera">
                <div class="tabla-cabecera-izq">
                    <span class="tabla-titulo">
                        {{TABS.find(t => t.key === tabActiva)?.label}}
                    </span>
                    <span v-if="conPeriodo" class="tabla-periodo-badge">{{ periodoLabel }}</span>
                </div>
                <span v-if="!loading && tablaConfig.rows.length" class="tabla-count">
                    {{ tablaConfig.rows.length }} registros
                </span>
            </div>

            <!-- Estado: cargando -->
            <div v-if="loading" class="estado-centro">
                <div class="spin-grande"></div>
                <p>Cargando reporte...</p>
            </div>

            <!-- Estado: sin datos -->
            <div v-else-if="!datos || tablaConfig.rows.length === 0" class="estado-vacio">
                <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="#d1d5db" stroke-width="1.2">
                    <rect x="3" y="3" width="18" height="18" rx="2" />
                    <line x1="9" y1="9" x2="15" y2="15" />
                    <line x1="15" y1="9" x2="9" y2="15" />
                </svg>
                <p>No hay datos disponibles para este período.</p>
            </div>

            <!-- Tabla de datos -->
            <div v-else class="tabla-scroll">
                <table class="data-table">
                    <thead>
                        <tr>
                            <th v-for="col in tablaConfig.cols" :key="col">{{ col }}</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(row, ri) in tablaConfig.rows" :key="ri">
                            <td v-for="(cell, ci) in row" :key="ci" :class="{
                                'td-mono': tablaConfig.monoIdx.includes(ci),
                                'td-num': tablaConfig.numIdx.includes(ci),
                            }">
                                <span v-if="ci === tablaConfig.estadoIdx" class="badge" :class="estadoClase(cell)">{{
                                    cell }}</span>
                                <template v-else>{{ cell }}</template>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Pie de la tabla -->
            <div v-if="!loading && tablaConfig.rows.length" class="tabla-pie">
                <span>AduanasExpress · Documento de uso interno</span>
                <span>Generado el {{ new Date().toLocaleDateString('es-DO') }}</span>
            </div>
        </div>
        <ReporteConfigModal v-model="mostrarConfig" @aplicar="aplicar" />

    </div>
</template>

<style scoped>
/* ─ Variables ─────────────────────────────────────────────── */
:root {
    --verde: #1C3829;
    --verde-cl: #2D5040;
    --bronce: #8A6A2E;
    --gris-bg: #F3F5F4;
    --gris-brd: #E2E8F0;
    --gris-tx: #1F2937;
    --gris-sec: #4B5563;
    --gris-lt: #6B7280;
    --blanco: #FFFFFF;
}

/* ─ Página ────────────────────────────────────────────────── */
.rp {
    padding: 28px 36px;
    background: #F0F2F1;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
    color: #1F2937;
}

/* ─ Encabezado ────────────────────────────────────────────── */
.rp-header {
    display: flex;
    align-items: flex-end;
    justify-content: space-between;
    margin-bottom: 24px;
    padding-bottom: 20px;
    border-bottom: 2px solid #1C3829;
}

.rp-empresa {
    font-size: .72rem;
    letter-spacing: .08em;
    text-transform: uppercase;
    color: #8A6A2E;
    font-weight: 600;
    margin: 0 0 4px;
}

.rp-titulo {
    font-size: 1.6rem;
    font-weight: 700;
    color: #1C3829;
    letter-spacing: -0.025em;
    margin: 0;
}

.rp-header-acciones {
    display: flex;
    gap: 8px;
    align-items: center;
}

/* ─ Botones de exportar ───────────────────────────────────── */
.btn-exp {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 8px 16px;
    border-radius: 6px;
    font-size: .82rem;
    font-weight: 600;
    cursor: pointer;
    border: 1.5px solid transparent;
    transition: opacity .15s, background .15s;
    font-family: inherit;
}

.btn-exp:disabled {
    opacity: .45;
    cursor: default;
}

.btn-xlsx {
    background: #D1FAE5;
    color: #065F46;
    border-color: #6EE7B7;
}

.btn-xlsx:hover:not(:disabled) {
    background: #A7F3D0;
}

.btn-pdf {
    background: #1C3829;
    color: #FFFFFF;
    border-color: #1C3829;
}

.btn-pdf:hover:not(:disabled) {
    background: #2D5040;
}

.btn-reload {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 8px 14px;
    background: #FFFFFF;
    border: 1.5px solid #D1D5DB;
    border-radius: 6px;
    font-size: .82rem;
    font-weight: 600;
    color: #374151;
    cursor: pointer;
    font-family: inherit;
    transition: background .15s;
}

.btn-reload:hover {
    background: #F9FAFB;
}

/* ─ Alerta ────────────────────────────────────────────────── */
.alerta-error {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 11px 16px;
    background: #FEF2F2;
    border: 1px solid #FECACA;
    border-left: 3px solid #DC2626;
    border-radius: 6px;
    font-size: .84rem;
    color: #991B1B;
    margin-bottom: 16px;
}

/* ─ Barra de tabs ─────────────────────────────────────────── */
.tabs-bar {
    display: flex;
    align-items: center;
    justify-content: space-between;
    background: #FFFFFF;
    border: 1px solid #E2E8F0;
    border-radius: 8px;
    padding: 6px 10px;
    margin-bottom: 16px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .05);
    flex-wrap: wrap;
    gap: 8px;
}

.tabs-lista {
    display: flex;
    gap: 2px;
}

.tab-btn {
    padding: 7px 18px;
    border: none;
    background: transparent;
    border-radius: 5px;
    font-size: .84rem;
    font-weight: 600;
    color: #6B7280;
    cursor: pointer;
    transition: all .14s;
    font-family: inherit;
}

.tab-btn:hover {
    background: #F3F5F4;
    color: #374151;
}

.tab-activo {
    background: #1C3829 !important;
    color: #FFFFFF !important;
}

/* ─ Selector de período ───────────────────────────────────── */
.periodo-grupo {
    display: flex;
    align-items: center;
    gap: 6px;
}

.periodo-label {
    font-size: .78rem;
    font-weight: 600;
    color: #6B7280;
}

.periodo-sel {
    padding: 6px 10px;
    background: #F9FAFB;
    border: 1.5px solid #E2E8F0;
    border-radius: 5px;
    font-size: .82rem;
    color: #374151;
    font-family: inherit;
    cursor: pointer;
    outline: none;
}

.periodo-sel:focus {
    border-color: #1C3829;
}

.btn-cfg {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 7px 14px;
    background: #fff;
    border: 1.5px solid #E2E8F0;
    border-radius: 6px;
    font-size: .82rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    transition: border-color .12s, background .12s;
}
.btn-cfg:hover {
    border-color: #1C3829;
    background: #EBF2EE;
    color: #1C3829;
}
/* ─ KPIs ──────────────────────────────────────────────────── */
.kpi-grid {
    display: grid;
    gap: 10px;
    margin-bottom: 16px;
}

.kpi-cols-4 {
    grid-template-columns: repeat(4, 1fr);
}

.kpi-cols-3 {
    grid-template-columns: repeat(3, 1fr);
}

.kpi-cols-2 {
    grid-template-columns: repeat(2, 1fr);
}

.kpi-cols-6 {
    grid-template-columns: repeat(6, 1fr);
}

.kpi-card {
    display: flex;
    background: #FFFFFF;
    border: 1px solid #E2E8F0;
    border-radius: 7px;
    overflow: hidden;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .05);
}

.kpi-acento {
    width: 4px;
    flex-shrink: 0;
}

/* Colores de acento por tipo */
.acento-azul {
    background: #1E3A5F;
}

.acento-verde {
    background: #166534;
}

.acento-ambar {
    background: #92400E;
}

.acento-rojo {
    background: #991B1B;
}

.acento-neutro {
    background: #1C3829;
}

.kpi-body {
    display: flex;
    flex-direction: column;
    padding: 12px 14px;
    gap: 3px;
}

.kpi-valor {
    font-size: 1.5rem;
    font-weight: 700;
    color: #111827;
    line-height: 1;
    letter-spacing: -0.02em;
}

.kpi-label {
    font-size: .71rem;
    color: #6B7280;
    font-weight: 500;
}

/* ─ Tabla card ────────────────────────────────────────────── */
.tabla-card {
    background: #FFFFFF;
    border: 1px solid #E2E8F0;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .05);
}

.tabla-cabecera {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 14px 20px;
    border-bottom: 1px solid #F3F4F6;
}

.tabla-cabecera-izq {
    display: flex;
    align-items: center;
    gap: 10px;
}

.tabla-titulo {
    font-size: .95rem;
    font-weight: 700;
    color: #111827;
}

.tabla-periodo-badge {
    font-size: .72rem;
    font-weight: 600;
    background: #EBF2EE;
    color: #1C3829;
    padding: 2px 10px;
    border-radius: 20px;
    border: 1px solid #C6D9CD;
}

.tabla-count {
    font-size: .75rem;
    color: #9CA3AF;
    font-weight: 500;
}

.tabla-scroll {
    overflow-x: auto;
}

/* ─ Tabla de datos ────────────────────────────────────────── */
.data-table {
    width: 100%;
    border-collapse: collapse;
    font-size: .85rem;
}

.data-table th {
    padding: 10px 14px;
    text-align: left;
    font-size: .71rem;
    font-weight: 700;
    color: #FFFFFF;
    background: #1C3829;
    letter-spacing: .06em;
    text-transform: uppercase;
    white-space: nowrap;
    border-bottom: 2px solid #8A6A2E;
    /* línea bronce bajo el encabezado */
}

.data-table td {
    padding: 10px 14px;
    color: #374151;
    border-bottom: 1px solid #F3F4F6;
    vertical-align: middle;
    white-space: nowrap;
}

.data-table tbody tr:nth-child(even) td {
    background: #F8F9FA;
}

.data-table tbody tr:last-child td {
    border-bottom: none;
}

.data-table tbody tr:hover td {
    background: #EBF2EE !important;
    transition: background .1s;
}

/* ─ Celdas especiales ─────────────────────────────────────── */
.td-mono {
    font-family: 'Courier New', Courier, monospace;
    font-size: .8rem;
    font-weight: 600;
    color: #1C3829;
    letter-spacing: .04em;
}

.td-num {
    text-align: right;
    font-variant-numeric: tabular-nums;
}

/* ─ Badges de estado ──────────────────────────────────────── */
.badge {
    display: inline-block;
    padding: 3px 9px;
    border-radius: 4px;
    font-size: .71rem;
    font-weight: 700;
    letter-spacing: .04em;
    text-transform: uppercase;
    white-space: nowrap;
}

.badge-ok {
    background: #DCFCE7;
    color: #166534;
}

.badge-ambar {
    background: #FEF3C7;
    color: #92400E;
}

.badge-rojo {
    background: #FEE2E2;
    color: #991B1B;
}

.badge-azul {
    background: #DBEAFE;
    color: #1E40AF;
}

.badge-gris {
    background: #F3F4F6;
    color: #4B5563;
}

/* ─ Pie de tabla ──────────────────────────────────────────── */
.tabla-pie {
    display: flex;
    justify-content: space-between;
    padding: 10px 20px;
    border-top: 1px solid #F3F4F6;
    font-size: .72rem;
    color: #9CA3AF;
    font-style: italic;
}

/* ─ Estados de carga / vacío ──────────────────────────────── */
.estado-centro {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 64px 0;
    color: #9CA3AF;
    font-size: .88rem;
}

.estado-vacio {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
    padding: 56px 0;
    color: #9CA3AF;
    font-size: .88rem;
}

/* ─ Spinners ──────────────────────────────────────────────── */
.spin {
    display: inline-block;
    width: 13px;
    height: 13px;
    border: 2px solid transparent;
    border-radius: 50%;
    animation: girar .7s linear infinite;
}

.spin-ok {
    border-color: #6EE7B7;
    border-top-color: #065F46;
}

.spin-pdf {
    border-color: rgba(255, 255, 255, .3);
    border-top-color: #fff;
}

.spin-grande {
    width: 30px;
    height: 30px;
    border: 3px solid #E5E7EB;
    border-top-color: #1C3829;
    border-radius: 50%;
    animation: girar .75s linear infinite;
}

@keyframes girar {
    to {
        transform: rotate(360deg);
    }
}

/* ─ Responsive ────────────────────────────────────────────── */
@media (max-width: 1100px) {

    .kpi-cols-4,
    .kpi-cols-6 {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 720px) {
    .rp {
        padding: 16px;
    }

    .rp-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 14px;
    }

    .kpi-cols-4,
    .kpi-cols-6,
    .kpi-cols-3 {
        grid-template-columns: repeat(2, 1fr);
    }

    .tabs-bar {
        flex-direction: column;
        align-items: flex-start;
    }

    .periodo-grupo {
        width: 100%;
    }

    .periodo-sel {
        flex: 1;
    }
}
</style>