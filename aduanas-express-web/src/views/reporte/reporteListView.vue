<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import {
    getReporteViajes,
    getReporteConsumo,
    getReporteSolicitudes,
    getReporteConductores,
    exportarPdf,
    exportarExcel,
} from '@/services/reporteService.js'

// ── Estado general ────────────────────────────────────────
const tabActiva   = ref('viajes')   // 'viajes' | 'consumo' | 'solicitudes' | 'conductores'
const loading     = ref(false)
const exportando  = ref('')         // '' | 'pdf' | 'excel'
const errorMsg    = ref('')

// ── Selector de período (viajes y consumo) ────────────────
const hoy    = new Date()
const mesRef = ref(hoy.getMonth() + 1)
const añoRef = ref(hoy.getFullYear())

const MESES = [
    'Enero','Febrero','Marzo','Abril','Mayo','Junio',
    'Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre',
]
const AÑOS = Array.from({ length: 5 }, (_, i) => hoy.getFullYear() - i)

const periodoLabel = computed(() =>
    `${MESES[mesRef.value - 1]} ${añoRef.value}`
)

// ── Datos de reportes ─────────────────────────────────────
const datosViajes      = ref(null)
const datosConsumo     = ref(null)
const datosSolicitudes = ref(null)
const datosConductores = ref(null)

// ── Helpers ───────────────────────────────────────────────
function formatFecha(f) {
    if (!f) return '—'
    return new Date(f).toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

function formatMoney(n) {
    return (parseFloat(n) || 0).toLocaleString('es-DO', {
        style: 'currency', currency: 'DOP', maximumFractionDigits: 0,
    })
}

function formatNumero(id) {
    return `#${String(id).padStart(4, '0')}`
}

// ── Carga por tab ─────────────────────────────────────────
async function cargar() {
    loading.value = true
    errorMsg.value = ''
    try {
        if (tabActiva.value === 'viajes') {
            const res = await getReporteViajes(mesRef.value, añoRef.value)
            datosViajes.value = res.data
        } else if (tabActiva.value === 'consumo') {
            const res = await getReporteConsumo(mesRef.value, añoRef.value)
            datosConsumo.value = res.data
        } else if (tabActiva.value === 'solicitudes') {
            const res = await getReporteSolicitudes()
            datosSolicitudes.value = res.data
        } else if (tabActiva.value === 'conductores') {
            const res = await getReporteConductores()
            datosConductores.value = res.data
        }
    } catch (e) {
        console.error(e)
        errorMsg.value = 'No se pudo cargar el reporte. Intenta de nuevo.'
    } finally {
        loading.value = false
    }
}

watch([tabActiva, mesRef, añoRef], cargar, { immediate: false })
onMounted(cargar)

// ── Exportar ──────────────────────────────────────────────
async function descargar(tipo) {
    exportando.value = tipo
    errorMsg.value   = ''
    try {
        const fn   = tipo === 'pdf' ? exportarPdf : exportarExcel
        const res  = await fn(mesRef.value, añoRef.value)
        const blob = new Blob([res.data], {
            type: tipo === 'pdf'
                ? 'application/pdf'
                : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        })
        const url  = URL.createObjectURL(blob)
        const a    = document.createElement('a')
        a.href     = url
        a.download = `reporte_${tabActiva.value}_${mesRef.value}_${añoRef.value}.${tipo === 'pdf' ? 'pdf' : 'xlsx'}`
        a.click()
        URL.revokeObjectURL(url)
    } catch (e) {
        console.error(e)
        errorMsg.value = `Error al exportar ${tipo.toUpperCase()}.`
    } finally {
        exportando.value = ''
    }
}

// ── Acceso fácil a datos actuales ─────────────────────────
const datos = computed(() => ({
    viajes:      datosViajes.value,
    consumo:     datosConsumo.value,
    solicitudes: datosSolicitudes.value,
    conductores: datosConductores.value,
})[tabActiva.value])

// ── KPIs dinámicos según tab ──────────────────────────────
const kpis = computed(() => {
    const d = datos.value
    if (!d) return []

    if (tabActiva.value === 'viajes') return [
        { label: 'Total viajes',    valor: d.totalViajes    ?? 0,                  color: 'azul',    icono: 'viajes' },
        { label: 'Completados',     valor: d.completados    ?? 0,                  color: 'verde',   icono: 'check'  },
        { label: 'Cancelados',      valor: d.cancelados     ?? 0,                  color: 'rojo',    icono: 'x'      },
        { label: 'Km recorridos',   valor: `${(d.kmTotales ?? 0).toLocaleString('es-DO')} km`, color: 'morado', icono: 'km' },
    ]

    if (tabActiva.value === 'consumo') return [
        { label: 'Costo total',      valor: formatMoney(d.costoTotal     ?? 0), color: 'azul',   icono: 'dinero'   },
        { label: 'Combustible',      valor: formatMoney(d.costoCombustible ?? 0), color: 'naranja', icono: 'fuel'  },
        { label: 'Mantenimiento',    valor: formatMoney(d.costoMantenim  ?? 0), color: 'morado', icono: 'llave'    },
        { label: 'Promedio / viaje', valor: formatMoney(d.promedioPorViaje ?? 0), color: 'verde', icono: 'grafico' },
    ]

    if (tabActiva.value === 'solicitudes') return [
        { label: 'Total solicitudes', valor: d.total      ?? 0, color: 'azul',   icono: 'docs'   },
        { label: 'Aprobadas',         valor: d.aprobadas  ?? 0, color: 'verde',  icono: 'check'  },
        { label: 'Pendientes',        valor: d.pendientes ?? 0, color: 'naranja',icono: 'reloj'  },
        { label: 'Rechazadas',        valor: d.rechazadas ?? 0, color: 'rojo',   icono: 'x'      },
    ]

    if (tabActiva.value === 'conductores') return [
        { label: 'Conductores activos', valor: d.totalConductores ?? 0, color: 'azul',   icono: 'persona' },
        { label: 'Viajes este mes',     valor: d.viajesEsteMes    ?? 0, color: 'verde',  icono: 'viajes'  },
        { label: 'Km promedio',         valor: `${(d.kmPromedio ?? 0).toLocaleString('es-DO')} km`, color: 'morado', icono: 'km' },
        { label: 'Sin incidentes',      valor: d.sinIncidentes    ?? 0, color: 'verde',  icono: 'check'   },
    ]

    return []
})

// ── Columnas de tabla por tab ─────────────────────────────
const tablaConfig = computed(() => {
    if (tabActiva.value === 'viajes') return {
        cols: ['#', 'Destino', 'Fecha', 'Conductor', 'Vehículo', 'Km', 'Estado'],
        rows: (datos.value?.detalles ?? []).map(r => [
            formatNumero(r.id),
            r.destino,
            formatFecha(r.fecha),
            r.conductor,
            r.placa,
            r.km ? `${r.km.toLocaleString('es-DO')} km` : '—',
            r.estado,
        ]),
        estadoIdx: 6,
    }

    if (tabActiva.value === 'consumo') return {
        cols: ['Vehículo', 'Placa', 'Combustible', 'Mantenimiento', 'Otros', 'Total'],
        rows: (datos.value?.detalles ?? []).map(r => [
            r.vehiculo,
            r.placa,
            formatMoney(r.combustible),
            formatMoney(r.mantenimiento),
            formatMoney(r.otros),
            formatMoney(r.total),
        ]),
        estadoIdx: -1,
    }

    if (tabActiva.value === 'solicitudes') return {
        cols: ['#', 'Área', 'Destino', 'Fecha solicitada', 'Solicitante', 'Estado'],
        rows: (datos.value?.detalles ?? []).map(r => [
            formatNumero(r.id),
            r.area,
            r.destino,
            formatFecha(r.fechaSolicitud),
            r.solicitante,
            r.estado,
        ]),
        estadoIdx: 5,
    }

    if (tabActiva.value === 'conductores') return {
        cols: ['Conductor', 'Licencia', 'Viajes', 'Km totales', 'Incidentes', 'Disponibilidad'],
        rows: (datos.value?.detalles ?? []).map(r => [
            `${r.nombre} ${r.apellido}`,
            r.licencia,
            r.viajes ?? 0,
            r.km ? `${r.km.toLocaleString('es-DO')} km` : '—',
            r.incidentes ?? 0,
            r.disponibilidad ?? '—',
        ]),
        estadoIdx: -1,
    }

    return { cols: [], rows: [], estadoIdx: -1 }
})

// ── Badge de estado ───────────────────────────────────────
function estadoBadgeClase(val) {
    const v = (val ?? '').toLowerCase()
    if (['completado','aprobada','aprobado','finalizado'].some(x => v.includes(x))) return 'badge-completado'
    if (['pendiente','espera'].some(x => v.includes(x)))                             return 'badge-pendiente'
    if (['cancelado','rechazada','rechazado'].some(x => v.includes(x)))              return 'badge-cancelado'
    if (['en viaje','proceso','en_viaje'].some(x => v.includes(x)))                 return 'badge-en-proceso'
    return 'badge-default'
}

// ── Tabs config ───────────────────────────────────────────
const TABS = [
    { key: 'viajes',      label: 'Viajes',      icono: 'viajes'   },
    { key: 'consumo',     label: 'Consumo',     icono: 'dinero'   },
    { key: 'solicitudes', label: 'Solicitudes', icono: 'docs'     },
    { key: 'conductores', label: 'Conductores', icono: 'persona'  },
]

const conPeriodo = computed(() => ['viajes', 'consumo'].includes(tabActiva.value))
</script>

<template>
    <div class="rep-page">

        <!-- ── Encabezado ── -->
        <div class="rep-header">
            <div>
                <h1 class="rep-title">Reportes</h1>
                <p class="rep-sub">Análisis y estadísticas del sistema de transporte</p>
            </div>
            <div class="rep-header-actions">
                <button class="btn-export btn-excel" :disabled="exportando !== ''" @click="descargar('excel')">
                    <div v-if="exportando === 'excel'" class="spinner-btn-dark"></div>
                    <svg v-else width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                        <polyline points="14 2 14 8 20 8"/>
                        <polyline points="8 13 12 17 16 13"/><line x1="12" y1="17" x2="12" y2="7"/>
                    </svg>
                    Excel
                </button>
                <button class="btn-export btn-pdf" :disabled="exportando !== ''" @click="descargar('pdf')">
                    <div v-if="exportando === 'pdf'" class="spinner-btn"></div>
                    <svg v-else width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                        <polyline points="14 2 14 8 20 8"/>
                        <polyline points="8 13 12 17 16 13"/><line x1="12" y1="17" x2="12" y2="7"/>
                    </svg>
                    PDF
                </button>
                <button class="btn-actualizar" @click="cargar">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                        <polyline points="1 4 1 10 7 10"/>
                        <path d="M3.51 15a9 9 0 1 0 .49-4.95"/>
                    </svg>
                    Actualizar
                </button>
            </div>
        </div>

        <!-- ── Error ── -->
        <div v-if="errorMsg" class="notif notif-error">{{ errorMsg }}</div>

        <!-- ── Tabs ── -->
        <div class="tabs-row">
            <button
                v-for="tab in TABS"
                :key="tab.key"
                class="tab-btn"
                :class="{ 'tab-activo': tabActiva === tab.key }"
                @click="tabActiva = tab.key"
            >
                <!-- Viajes -->
                <svg v-if="tab.icono === 'viajes'" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                    <rect x="1" y="3" width="15" height="13" rx="2"/><path d="M16 8h4l3 3v5h-7V8z"/>
                    <circle cx="5.5" cy="18.5" r="2.5"/><circle cx="18.5" cy="18.5" r="2.5"/>
                </svg>
                <!-- Dinero -->
                <svg v-else-if="tab.icono === 'dinero'" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                    <line x1="12" y1="1" x2="12" y2="23"/>
                    <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                </svg>
                <!-- Docs -->
                <svg v-else-if="tab.icono === 'docs'" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                    <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                    <polyline points="14 2 14 8 20 8"/>
                    <line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/>
                </svg>
                <!-- Persona -->
                <svg v-else-if="tab.icono === 'persona'" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                    <circle cx="12" cy="7" r="4"/>
                </svg>
                {{ tab.label }}
            </button>

            <!-- Selector de período -->
            <div class="periodo-wrap" v-if="conPeriodo">
                <select v-model="mesRef" class="periodo-select">
                    <option v-for="(m, i) in MESES" :key="i" :value="i+1">{{ m }}</option>
                </select>
                <select v-model="añoRef" class="periodo-select">
                    <option v-for="a in AÑOS" :key="a">{{ a }}</option>
                </select>
            </div>
        </div>

        <!-- ── KPIs ── -->
        <div class="kpi-row">
            <div v-for="(k, i) in kpis" :key="i" class="kpi-card">
                <div class="kpi-icon" :class="`kpi-icon-${k.color}`">
                    <!-- Viajes -->
                    <svg v-if="k.icono === 'viajes'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <rect x="1" y="3" width="15" height="13" rx="2"/><path d="M16 8h4l3 3v5h-7V8z"/>
                        <circle cx="5.5" cy="18.5" r="2.5"/><circle cx="18.5" cy="18.5" r="2.5"/>
                    </svg>
                    <!-- Check -->
                    <svg v-else-if="k.icono === 'check'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <polyline points="20 6 9 17 4 12"/>
                    </svg>
                    <!-- X -->
                    <svg v-else-if="k.icono === 'x'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
                    </svg>
                    <!-- km -->
                    <svg v-else-if="k.icono === 'km'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                    </svg>
                    <!-- dinero -->
                    <svg v-else-if="k.icono === 'dinero'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <line x1="12" y1="1" x2="12" y2="23"/>
                        <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                    </svg>
                    <!-- fuel -->
                    <svg v-else-if="k.icono === 'fuel'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <path d="M3 22V8a2 2 0 0 1 2-2h6a2 2 0 0 1 2 2v14"/><path d="M2 22h12"/><path d="M13 8h3l2 2v8a2 2 0 0 1-2 2h-1"/>
                    </svg>
                    <!-- llave -->
                    <svg v-else-if="k.icono === 'llave'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
                    </svg>
                    <!-- grafico -->
                    <svg v-else-if="k.icono === 'grafico'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <line x1="18" y1="20" x2="18" y2="10"/><line x1="12" y1="20" x2="12" y2="4"/>
                        <line x1="6" y1="20" x2="6" y2="14"/><line x1="2" y1="20" x2="22" y2="20"/>
                    </svg>
                    <!-- docs -->
                    <svg v-else-if="k.icono === 'docs'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                        <polyline points="14 2 14 8 20 8"/>
                    </svg>
                    <!-- reloj -->
                    <svg v-else-if="k.icono === 'reloj'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                    </svg>
                    <!-- persona -->
                    <svg v-else width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/>
                    </svg>
                </div>
                <div>
                    <p class="kpi-valor">{{ k.valor }}</p>
                    <p class="kpi-label">{{ k.label }}</p>
                </div>
            </div>
        </div>

        <!-- ── Tabla ── -->
        <div class="tabla-wrap">

            <!-- Cabecera tabla -->
            <div class="tabla-header">
                <h3 class="tabla-titulo">
                    Detalle — {{ TABS.find(t => t.key === tabActiva)?.label }}
                    <span v-if="conPeriodo" class="tabla-periodo">{{ periodoLabel }}</span>
                </h3>
                <span class="tabla-count" v-if="!loading && tablaConfig.rows.length">
                    {{ tablaConfig.rows.length }} registros
                </span>
            </div>

            <!-- Spinner -->
            <div v-if="loading" class="estado-carga">
                <div class="spinner"></div>
                <p>Cargando reporte...</p>
            </div>

            <!-- Sin datos -->
            <div v-else-if="!datos || tablaConfig.rows.length === 0" class="estado-vacio">
                <svg width="44" height="44" viewBox="0 0 24 24" fill="none" stroke="#d1d5db" stroke-width="1.2">
                    <line x1="18" y1="20" x2="18" y2="10"/><line x1="12" y1="20" x2="12" y2="4"/>
                    <line x1="6" y1="20" x2="6" y2="14"/><line x1="2" y1="20" x2="22" y2="20"/>
                </svg>
                <p>No hay datos para este período.</p>
            </div>

            <!-- Tabla -->
            <div v-else class="tabla-scroll">
                <table class="rep-tabla">
                    <thead>
                        <tr>
                            <th v-for="col in tablaConfig.cols" :key="col">{{ col }}</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(row, ri) in tablaConfig.rows" :key="ri">
                            <td
                                v-for="(cell, ci) in row"
                                :key="ci"
                                :class="{
                                    'td-id':   ci === 0 && tablaConfig.cols[0] === '#',
                                    'td-mono': tablaConfig.cols[ci] === 'Placa',
                                }"
                            >
                                <span
                                    v-if="ci === tablaConfig.estadoIdx"
                                    class="badge"
                                    :class="estadoBadgeClase(cell)"
                                >{{ cell }}</span>
                                <template v-else>{{ cell }}</template>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

        </div>

    </div>
</template>

<style scoped>
/* ── Base ── */
.rep-page {
    padding: 28px 32px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

/* ── Encabezado ── */
.rep-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20px;
}

.rep-title {
    font-size: 1.45rem;
    font-weight: 700;
    color: #111827;
    letter-spacing: -0.02em;
    margin: 0 0 2px;
}

.rep-sub {
    font-size: .8rem;
    color: #9ca3af;
    margin: 0;
}

.rep-header-actions {
    display: flex;
    gap: 10px;
    align-items: center;
}

.btn-actualizar {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    background: #fff;
    border: 1.5px solid #d1d5db;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #374151;
    cursor: pointer;
    transition: all .15s;
}
.btn-actualizar:hover { background: #f3f4f6; border-color: #9ca3af; }

.btn-export {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 9px 18px;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    cursor: pointer;
    transition: background .15s;
}
.btn-export:disabled { opacity: .5; cursor: default; }

.btn-excel { background: #d1fae5; color: #065f46; border: 1.5px solid #6ee7b7; }
.btn-excel:hover:not(:disabled) { background: #a7f3d0; }

.btn-pdf { background: #fee2e2; color: #991b1b; border: 1.5px solid #fca5a5; }
.btn-pdf:hover:not(:disabled) { background: #fecaca; }

/* ── Notificación ── */
.notif {
    padding: 12px 18px;
    border-radius: 10px;
    font-size: .875rem;
    font-weight: 500;
    margin-bottom: 16px;
}
.notif-error { background: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

/* ── Tabs ── */
.tabs-row {
    display: flex;
    align-items: center;
    gap: 6px;
    margin-bottom: 16px;
    background: #fff;
    border-radius: 12px;
    padding: 8px 10px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    flex-wrap: wrap;
}

.tab-btn {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 8px 16px;
    border: none;
    background: transparent;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #6b7280;
    cursor: pointer;
    transition: all .15s;
}
.tab-btn:hover { background: #f3f4f6; color: #374151; }

.tab-activo {
    background: #1a3a2a !important;
    color: #fff !important;
}

/* Período */
.periodo-wrap {
    display: flex;
    gap: 8px;
    margin-left: auto;
}

.periodo-select {
    padding: 7px 12px;
    background: #f9fafb;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: .82rem;
    color: #374151;
    font-family: inherit;
    cursor: pointer;
    outline: none;
    transition: border-color .15s;
}
.periodo-select:focus { border-color: #1a3a2a; }

/* ── KPIs ── */
.kpi-row {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 12px;
    margin-bottom: 16px;
}

.kpi-card {
    background: #fff;
    border-radius: 12px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    padding: 16px 18px;
    display: flex;
    align-items: center;
    gap: 14px;
}

.kpi-icon {
    width: 42px;
    height: 42px;
    border-radius: 11px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.kpi-icon-azul    { background: #dbeafe; color: #1e40af; }
.kpi-icon-verde   { background: #d1fae5; color: #065f46; }
.kpi-icon-rojo    { background: #fee2e2; color: #991b1b; }
.kpi-icon-morado  { background: #ede9fe; color: #6d28d9; }
.kpi-icon-naranja { background: #fef3c7; color: #92400e; }

.kpi-valor {
    font-size: 1.45rem;
    font-weight: 800;
    color: #111827;
    margin: 0 0 1px;
    line-height: 1;
}

.kpi-label {
    font-size: .72rem;
    color: #6b7280;
    font-weight: 500;
    margin: 0;
}

/* ── Tabla ── */
.tabla-wrap {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    overflow: hidden;
}

.tabla-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 16px 20px;
    border-bottom: 1px solid #f3f4f6;
}

.tabla-titulo {
    font-size: .95rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
    display: flex;
    align-items: center;
    gap: 8px;
}

.tabla-periodo {
    font-size: .75rem;
    font-weight: 600;
    background: #dbeafe;
    color: #1e40af;
    padding: 2px 10px;
    border-radius: 20px;
}

.tabla-count {
    font-size: .75rem;
    color: #9ca3af;
    font-weight: 500;
}

.tabla-scroll {
    overflow-x: auto;
}

.rep-tabla {
    width: 100%;
    border-collapse: collapse;
    font-size: .875rem;
}

.rep-tabla th {
    padding: 12px 16px;
    text-align: left;
    font-size: .7rem;
    font-weight: 600;
    color: #9ca3af;
    letter-spacing: .05em;
    border-bottom: 1.5px solid #f3f4f6;
    white-space: nowrap;
}

.rep-tabla td {
    padding: 12px 16px;
    color: #374151;
    border-bottom: 1px solid #f9fafb;
    vertical-align: middle;
    white-space: nowrap;
}

.rep-tabla tbody tr:last-child td { border-bottom: none; }
.rep-tabla tbody tr:hover { background: #fafafa; }

.td-id   { font-weight: 700; color: #111827; }
.td-mono { font-family: monospace; font-weight: 700; font-size: .8rem; background: #111827; color: #fff; padding: 2px 8px; border-radius: 5px; display: inline-block; }

/* ── Badges ── */
.badge {
    display: inline-block;
    padding: 3px 9px;
    border-radius: 20px;
    font-size: .71rem;
    font-weight: 700;
    white-space: nowrap;
}

.badge-completado { background: #d1fae5; color: #065f46; }
.badge-pendiente  { background: #fef3c7; color: #92400e; }
.badge-cancelado  { background: #fee2e2; color: #991b1b; }
.badge-en-proceso { background: #dbeafe; color: #1e40af; }
.badge-default    { background: #f3f4f6; color: #374151; }

/* ── Estados carga / vacío ── */
.estado-carga {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 60px 0;
    color: #6b7280;
}

.estado-vacio {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
    padding: 56px 0;
    color: #9ca3af;
    font-size: .9rem;
}

/* ── Spinners ── */
.spinner {
    width: 32px; height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

.spinner-btn {
    width: 14px; height: 14px;
    border: 2px solid rgba(255,255,255,.4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

.spinner-btn-dark {
    width: 14px; height: 14px;
    border: 2px solid #6ee7b7;
    border-top-color: #065f46;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

/* ── Responsive ── */
@media (max-width: 1000px) {
    .kpi-row { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 700px) {
    .rep-page       { padding: 16px; }
    .rep-header     { flex-direction: column; align-items: flex-start; gap: 12px; }
    .kpi-row        { grid-template-columns: 1fr 1fr; }
    .tabs-row       { gap: 4px; }
    .periodo-wrap   { margin-left: 0; width: 100%; }
    .periodo-select { flex: 1; }
}
</style>