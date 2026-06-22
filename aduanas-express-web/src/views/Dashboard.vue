<template>
  <div class="dashboard">
    <AppSidebar />

    <div class="dashboard__main">
      <header class="topbar">
        <div class="topbar__left">
          <h1 class="topbar__title">Dashboard</h1>
        </div>
        <div class="topbar__right">
          <span class="topbar__date">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round">
              <rect x="3" y="5" width="18" height="16" rx="2" />
              <path d="M3 10h18M8 3v4M16 3v4" />
            </svg>
            {{ fechaHoy }}
          </span>
          <button class="icon-btn" type="button" aria-label="Buscar">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="11" cy="11" r="7" />
              <line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
          </button>
          <button class="icon-btn" type="button" aria-label="Notificaciones">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
              <path d="M18 8a6 6 0 0 0-12 0c0 7-3 9-3 9h18s-3-2-3-9" />
              <path d="M13.73 21a2 2 0 0 1-3.46 0" />
            </svg>
            <span v-if="solicitudesPendientesCount > 0" class="icon-btn__badge"></span>
          </button>
        </div>
      </header>

      <p v-if="errorMsg" class="banner banner--error">{{ errorMsg }}</p>

      <main class="content">
        <!-- KPIs -->
        <section class="kpi-grid">
          <div v-for="kpi in kpis" :key="kpi.label" class="card kpi-card">
            <p class="kpi-card__label">{{ kpi.label }}</p>
            <p class="kpi-card__value">
              <span v-if="loading" class="skeleton skeleton--num"></span>
              <template v-else>{{ kpi.value }}</template>
            </p>
            <p class="kpi-card__sub">
              <span class="dot" :class="`dot--${kpi.dot}`"></span>{{ kpi.sub }}
            </p>
          </div>
        </section>

        <!-- Gráficos -->
        <section class="charts-grid">
          <div class="card chart-card">
            <div class="card__header">
              <h2>Viajes por mes</h2>
              <router-link to="/reportes" class="link">Ver detalle</router-link>
            </div>
            <div v-if="!loading" class="bar-chart">
              <div v-for="(m, i) in viajesPorMes" :key="m.label" class="bar-chart__col">
                <span class="bar-chart__value">{{ m.total }}</span>
                <div
                  class="bar-chart__bar"
                  :class="{ 'bar-chart__bar--current': i === viajesPorMes.length - 1 }"
                  :style="{ height: barHeight(m.total) + '%' }"
                ></div>
                <span class="bar-chart__label">{{ m.label }}</span>
              </div>
            </div>
            <div v-else class="skeleton skeleton--chart"></div>
          </div>

          <div class="card chart-card">
            <div class="card__header">
              <h2>Estado de vehículos</h2>
            </div>
            <div v-if="!loading" class="pie-chart-wrap">
              <svg viewBox="0 0 120 120" class="pie-chart">
                <path
                  v-for="slice in pieSlices"
                  :key="slice.label"
                  :d="slice.path"
                  :fill="slice.color"
                  stroke="#ffffff"
                  stroke-width="2"
                />
                <text
                  v-for="slice in pieLabels"
                  :key="`${slice.label}-label`"
                  :x="slice.labelPos.x"
                  :y="slice.labelPos.y"
                  text-anchor="middle"
                  dominant-baseline="central"
                  fill="#ffffff"
                  class="pie-chart__label"
                >{{ slice.percent }}%</text>
              </svg>
              <ul class="pie-legend">
                <li v-for="seg in pieSegments" :key="seg.label">
                  <span class="dot" :style="{ backgroundColor: seg.color }"></span>
                  <span class="pie-legend__label">{{ seg.label }}</span>
                  <strong>{{ seg.value }}</strong>
                </li>
              </ul>
            </div>
            <div v-else class="skeleton skeleton--chart"></div>
          </div>
        </section>

        <!-- Tablas -->
        <section class="tables-grid">
          <div class="card">
            <div class="card__header">
              <h2>Solicitudes recientes</h2>
              <router-link to="/solicitudes" class="link">Ver todas</router-link>
            </div>
            <table v-if="!loading" class="table">
              <thead>
                <tr>
                  <th>Área</th>
                  <th>Destino</th>
                  <th>Fecha</th>
                  <th>Estado</th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="solicitudesRecientes.length === 0">
                  <td colspan="4" class="table__empty">Aún no hay solicitudes registradas.</td>
                </tr>
                <tr v-for="s in solicitudesRecientes" :key="s.id ?? s.Id ?? campo(s, ['fecha', 'Fecha'])">
                  <td>{{ campo(s, ['area', 'Area', 'areaSolicitante']) || '—' }}</td>
                  <td>{{ campo(s, ['destino', 'Destino']) || '—' }}</td>
                  <td>{{ formatearFecha(campo(s, ['fecha', 'Fecha', 'createdAt'])) }}</td>
                  <td>
                    <span class="badge" :class="badgeClase(campo(s, ['estado', 'Estado']))">
                      {{ campo(s, ['estado', 'Estado']) || '—' }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
            <div v-else class="skeleton skeleton--table"></div>
          </div>

          <div class="card">
            <div class="card__header">
              <h2>Actividad reciente</h2>
              <router-link to="/historial" class="link">Ver historial</router-link>
            </div>
            <ul v-if="!loading" class="activity">
              <li v-if="actividad.length === 0" class="table__empty">Sin actividad reciente.</li>
              <li v-for="(a, i) in actividad" :key="i">
                <span class="dot dot--blue"></span>
                <div>
                  <p class="activity__text">{{ a.texto }}</p>
                  <p class="activity__time">{{ a.relativo }}</p>
                </div>
              </li>
            </ul>
            <div v-else class="skeleton skeleton--table"></div>
          </div>
        </section>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { verVehiculos } from '../services/vehiculoService'
import { verConductores } from '../services/conductorService'
import { verSolicitud } from '../services/solicitudService'
import { verAsignaciones } from '../services/asignacionService'
import { verMantenimiento } from '../services/mantenimientoService'
import { getReporteViajes } from '../services/reporteService'


const loading = ref(true)
const errorMsg = ref('')

const vehiculos = ref([])
const conductores = ref([])
const solicitudes = ref([])
const asignaciones = ref([])
const mantenimientos = ref([])
const viajesPorMes = ref([])

function campo(obj, nombres, fallback = '') {
  for (const n of nombres) {
    if (obj && obj[n] !== undefined && obj[n] !== null && obj[n] !== '') return obj[n]
  }
  return fallback
}
function norm(v) {
  return (v ?? '').toString().toLowerCase().trim()
}
function estadoDe(obj) {
  return norm(campo(obj, ['estado', 'Estado', 'status', 'Status']))
}
const ESTADOS_VEHICULO = ['Disponible', 'EnViaje', 'EnMantenimiento', 'FueraDeServicio']
function estadoVehiculo(v) {
  const raw = campo(v, ['estado', 'Estado', 'status', 'Status'])
  if (raw === '' || raw === null || raw === undefined) return ''
  if (raw !== '' && !Number.isNaN(Number(raw))) {
    return norm(ESTADOS_VEHICULO[Number(raw)] ?? '')
  }
  return norm(raw)
}
function idDe(obj) {
  return campo(obj, ['id', 'Id'])
}

function conductorPorId(conductorId) {
  return conductores.value.find(c => idDe(c) === conductorId) || null
}
function nombreConductorPorId(conductorId) {
  const c = conductorPorId(conductorId)
  if (!c) return 'Un conductor'
  const nombre = [campo(c, ['nombre', 'Nombre']), campo(c, ['apellido', 'Apellido'])].filter(Boolean).join(' ')
  return nombre || 'Un conductor'
}
function vehiculoPorId(vehiculoId) {
  return vehiculos.value.find(v => idDe(v) === vehiculoId) || null
}
function placaVehiculoPorId(vehiculoId) {
  const v = vehiculoPorId(vehiculoId)
  return v ? campo(v, ['matricula', 'Matricula']) : ''
}
function solicitudPorId(solicitudId) {
  return solicitudes.value.find(s => idDe(s) === solicitudId) || null
}
function destinoDeSolicitud(solicitudId) {
  const s = solicitudPorId(solicitudId)
  return s ? campo(s, ['destino', 'Destino']) : ''
}

const fechaHoy = computed(() => {
  const hoy = new Date()
  const texto = hoy.toLocaleDateString('es-ES', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })
  return texto.charAt(0).toUpperCase() + texto.slice(1)
})

function formatearFecha(valor) {
  if (!valor) return '—'
  const d = new Date(valor)
  if (Number.isNaN(d.getTime())) return '—'
  return d.toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })
}

function tiempoRelativo(valor) {
  if (!valor) return ''
  const d = new Date(valor)
  if (Number.isNaN(d.getTime())) return ''
  const diffMs = Date.now() - d.getTime()
  const min = Math.round(diffMs / 60000)
  if (min < 1) return 'justo ahora'
  if (min < 60) return `hace ${min} min`
  const horas = Math.round(min / 60)
  if (horas < 24) return `hace ${horas} h`
  const dias = Math.round(horas / 24)
  return `hace ${dias} d`
}

const totalVehiculos = computed(() => vehiculos.value.length)
const vehiculosDisponibles = computed(() =>
  vehiculos.value.filter(v => estadoVehiculo(v).includes('dispon')).length
)

const hoyISO = new Date().toISOString().slice(0, 10)
const viajesHoy = computed(() =>
  asignaciones.value.filter(a => {
    const f = campo(a, ['fechaAsignacion', 'FechaAsignacion'])
    return f && f.toString().slice(0, 10) === hoyISO
  }).length
)

const solicitudesPendientes = computed(() => solicitudes.value.filter(s => estadoDe(s).includes('pendient')))
const solicitudesPendientesCount = computed(() => solicitudesPendientes.value.length)

const totalConductores = computed(() => conductores.value.length)
const conductoresEnViaje = computed(() => conductores.value.filter(c => estadoDe(c).includes('viaje')).length)

const kpis = computed(() => [
  {
    label: 'Total vehículo',
    value: totalVehiculos.value,
    sub: `${vehiculosDisponibles.value} disponibles`,
    dot: 'green',
  },
  {
    label: 'Viajes hoy',
    value: viajesHoy.value,
    sub: 'Asignaciones de hoy',
    dot: 'blue',
  },
  {
    label: 'Solicitudes pend.',
    value: solicitudesPendientesCount.value,
    sub: solicitudesPendientesCount.value > 0 ? 'Requieren acción' : 'Todo al día',
    dot: solicitudesPendientesCount.value > 0 ? 'red' : 'green',
  },
  {
    label: 'Conductores activos',
    value: totalConductores.value,
    sub: `${conductoresEnViaje.value} en viaje`,
    dot: conductoresEnViaje.value > 0 ? 'blue' : 'gray',
  },
])

async function cargarViajesPorMes() {
  const hoy = new Date()
  const meses = []
  for (let i = 4; i >= 0; i--) {
    const d = new Date(hoy.getFullYear(), hoy.getMonth() - i, 1)
    meses.push({
      mes: d.getMonth() + 1,
      anio: d.getFullYear(),
      label: d.toLocaleDateString('es-ES', { month: 'short' }).replace('.', ''),
    })
  }
  const resultados = await Promise.allSettled(meses.map(m => getReporteViajes(m.mes, m.anio)))
  viajesPorMes.value = meses.map((m, idx) => {
    const r = resultados[idx]
    const data = r.status === 'fulfilled' ? r.value?.data : null
    const total = data?.totalViajes ?? data?.TotalViajes ?? 0
    return { label: m.label, total }
  })
}

const maxViajes = computed(() => Math.max(1, ...viajesPorMes.value.map(m => m.total)))
function barHeight(valor) {
  return Math.max(6, Math.round((valor / maxViajes.value) * 100))
}

const estadoColores = {
  Disponibles: '#3fae5c',
  'En viaje': '#4a6fa5',
  Mantenimiento: '#e8a33d',
  'Fuera de servicio': '#d9534f',
}

const pieSegments = computed(() => {
  const buckets = { Disponibles: 0, 'En viaje': 0, Mantenimiento: 0, 'Fuera de servicio': 0 }
  vehiculos.value.forEach(v => {
    const e = estadoVehiculo(v)
    if (e.includes('mantenim')) buckets['Mantenimiento']++
    else if (e.includes('fuera') || e.includes('inactiv')) buckets['Fuera de servicio']++
    else if (e.includes('viaje') || e.includes('uso')) buckets['En viaje']++
    else buckets['Disponibles']++
  })
  return Object.entries(buckets).map(([label, value]) => ({ label, value, color: estadoColores[label] }))
})

const PIE_CENTER = 60
const PIE_RADIUS = 58

function puntoEnCirculo(anguloGrados, radio = PIE_RADIUS) {
  const anguloRad = ((anguloGrados - 90) * Math.PI) / 180
  return {
    x: PIE_CENTER + radio * Math.cos(anguloRad),
    y: PIE_CENTER + radio * Math.sin(anguloRad),
  }
}

function porcionPath(anguloInicio, anguloFin) {
  if (anguloFin - anguloInicio >= 360) {
    const p1 = puntoEnCirculo(anguloInicio)
    const p2 = puntoEnCirculo(anguloInicio + 180)
    return `M ${p1.x} ${p1.y} A ${PIE_RADIUS} ${PIE_RADIUS} 0 1 1 ${p2.x} ${p2.y} A ${PIE_RADIUS} ${PIE_RADIUS} 0 1 1 ${p1.x} ${p1.y} Z`
  }
  const inicio = puntoEnCirculo(anguloInicio)
  const fin = puntoEnCirculo(anguloFin)
  const arcoGrande = anguloFin - anguloInicio > 180 ? 1 : 0
  return `M ${PIE_CENTER} ${PIE_CENTER} L ${inicio.x} ${inicio.y} A ${PIE_RADIUS} ${PIE_RADIUS} 0 ${arcoGrande} 1 ${fin.x} ${fin.y} Z`
}

const pieSlices = computed(() => {
  const total = pieSegments.value.reduce((s, seg) => s + seg.value, 0)
  if (total === 0) {
    return [{ label: 'Sin datos', color: '#e5e7eb', path: porcionPath(0, 360), percent: 0, labelPos: { x: PIE_CENTER, y: PIE_CENTER } }]
  }
  let acc = 0
  return pieSegments.value
    .filter(seg => seg.value > 0)
    .map(seg => {
      const anguloInicio = (acc / total) * 360
      acc += seg.value
      const anguloFin = (acc / total) * 360
      const anguloMedio = (anguloInicio + anguloFin) / 2
      return {
        label: seg.label,
        color: seg.color,
        path: porcionPath(anguloInicio, anguloFin),
        percent: Math.round((seg.value / total) * 100),
        labelPos: puntoEnCirculo(anguloMedio, PIE_RADIUS * 0.62),
      }
    })
})

const pieLabels = computed(() => pieSlices.value.filter(slice => slice.percent > 0))

const solicitudesRecientes = computed(() =>
  [...solicitudes.value]
    .sort((a, b) => new Date(campo(b, ['fecha', 'Fecha', 'createdAt'])) - new Date(campo(a, ['fecha', 'Fecha', 'createdAt'])))
    .slice(0, 5)
)

function badgeClase(estado) {
  const e = norm(estado)
  if (e.includes('pendient')) return 'badge--pendiente'
  if (e.includes('aprob') || e.includes('asign')) return 'badge--aprobada'
  if (e.includes('rechaz') || e.includes('cancel')) return 'badge--rechazada'
  return 'badge--default'
}

function tagId(id) {
  if (id === '' || id === undefined || id === null) return ''
  return `#${String(id).padStart(4, '0')}`
}

const actividad = computed(() => {
  const items = []

  solicitudes.value.forEach(s => {
    const fecha = campo(s, ['fecha', 'Fecha', 'createdAt'])
    const area = campo(s, ['area', 'Area', 'areaSolicitante'], 'un área')
    if (fecha) items.push({ texto: `Nueva solicitud de transporte - ${area}`, fecha })
  })

  asignaciones.value.forEach(a => {
    const e = estadoDe(a)
    const fecha = campo(a, ['fecha', 'Fecha', 'fechaFinalizacion', 'fechaAsignacion'])
    if (!fecha) return

    const id = idDe(a)
    const solicitudId = campo(a, ['solicitudId', 'SolicitudId'])
    const conductorId = campo(a, ['conductorId', 'ConductorId'])

    if (e.includes('final')) {
      const destino = destinoDeSolicitud(solicitudId) || 'destino no especificado'
      items.push({ texto: `Viaje ${tagId(id)} finalizado - ${destino}`, fecha })
    } else if (e.includes('cancel')) {
      items.push({ texto: `Asignación ${tagId(id)} cancelada`, fecha })
    } else {
      const nombreConductor = nombreConductorPorId(conductorId)
      items.push({ texto: `Conductor ${nombreConductor} asignado a solicitud ${tagId(solicitudId)}`, fecha })
    }
  })

  mantenimientos.value.forEach(m => {
    const fecha = campo(m, ['fecha', 'Fecha', 'fechaInicio', 'FechaInicio', 'createdAt'])
    if (!fecha) return
    const vehiculoId = campo(m, ['vehiculoId', 'VehiculoId'])
    const placa = placaVehiculoPorId(vehiculoId) || 'sin placa'
    items.push({ texto: `Vehículo ${placa} enviado a mantenimiento`, fecha })
  })

  return items
    .sort((a, b) => new Date(b.fecha) - new Date(a.fecha))
    .slice(0, 4)
    .map(i => ({ ...i, relativo: tiempoRelativo(i.fecha) }))
})

onMounted(async () => {
  loading.value = true
  errorMsg.value = ''
  try {
    const [vRes, cRes, sRes, aRes, mRes] = await Promise.allSettled([
      verVehiculos(),
      verConductores(),
      verSolicitud(),
      verAsignaciones(),
      verMantenimiento(),
    ])
    vehiculos.value = vRes.status === 'fulfilled' ? vRes.value.data ?? [] : []
    conductores.value = cRes.status === 'fulfilled' ? cRes.value.data ?? [] : []
    solicitudes.value = sRes.status === 'fulfilled' ? sRes.value.data ?? [] : []
    asignaciones.value = aRes.status === 'fulfilled' ? aRes.value.data ?? [] : []
    mantenimientos.value = mRes.status === 'fulfilled' ? mRes.value.data ?? [] : []

    if ([vRes, cRes, sRes, aRes, mRes].some(r => r.status === 'rejected')) {
      errorMsg.value = 'Algunos datos no se pudieron cargar. Mostrando la información disponible.'
    }

    await cargarViajesPorMes()
  } catch (e) {
    console.error(e)
    errorMsg.value = 'No se pudo cargar el dashboard. Intenta recargar la página.'
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.dashboard {
  --color-bg: #eef0f3;
  --color-card: #ffffff;
  --color-text: #1f2430;
  --color-muted: #6b7280;
  --color-border: #e5e7eb;
  --color-link: #3b5fc4;
  --color-green: #3fae5c;
  --color-blue: #4a6fa5;
  --color-red: #d9534f;
  --color-gray: #9aa0a6;

  display: flex;
  min-height: 100vh;
  background: var(--color-bg);
  color: var(--color-text);
  font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
}

* {
  box-sizing: border-box;
}

/* ---------- Main / Topbar ---------- */
.dashboard__main {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
}

.topbar {
  background: var(--color-card);
  border-bottom: 1px solid var(--color-border);
  padding: 14px 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}
.topbar__left {
  display: flex;
  align-items: center;
  gap: 10px;
}
.topbar__title {
  margin: 0;
  font-size: 21px;
  font-weight: 700;
}
.topbar__right {
  display: flex;
  align-items: center;
  gap: 10px;
}
.topbar__date {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12.5px;
  color: var(--color-muted);
  white-space: nowrap;
}
.topbar__date svg {
  width: 15px;
  height: 15px;
}

.icon-btn {
  position: relative;
  width: 34px;
  height: 34px;
  border-radius: 8px;
  border: 1px solid var(--color-border);
  background: #fafafa;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--color-text);
}
.icon-btn svg {
  width: 16px;
  height: 16px;
}
.icon-btn:hover {
  background: #f0f0f0;
}
.icon-btn__badge {
  position: absolute;
  top: 6px;
  right: 7px;
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: var(--color-red);
}

.banner {
  margin: 14px 24px 0;
  padding: 10px 14px;
  border-radius: 8px;
  font-size: 13px;
}
.banner--error {
  background: #fdecea;
  color: #9b2c2c;
  border: 1px solid #f5c2c0;
}

.content {
  padding: 22px 24px 36px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* ---------- Cards ---------- */
.card {
  background: var(--color-card);
  border-radius: 12px;
  border: 1px solid var(--color-border);
  box-shadow: 0 1px 3px rgba(16, 24, 40, 0.06);
  padding: 18px 20px;
}
.card__header {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  margin-bottom: 14px;
}
.card__header h2 {
  margin: 0;
  font-size: 14.5px;
  font-weight: 700;
}
.link {
  font-size: 12.5px;
  color: var(--color-link);
  text-decoration: none;
  font-weight: 600;
}
.link:hover {
  text-decoration: underline;
}

/* ---------- KPIs ---------- */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}
.kpi-card__label {
  margin: 0 0 10px;
  font-size: 12.5px;
  font-weight: 700;
  color: var(--color-text);
}
.kpi-card__value {
  margin: 0 0 8px;
  font-size: 28px;
  font-weight: 700;
}
.kpi-card__sub {
  margin: 0;
  font-size: 12px;
  color: var(--color-muted);
  display: flex;
  align-items: center;
  gap: 6px;
}

.dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  display: inline-block;
  flex-shrink: 0;
}
.dot--green {
  background: var(--color-green);
}
.dot--blue {
  background: var(--color-link);
}
.dot--red {
  background: var(--color-red);
}
.dot--gray {
  background: var(--color-gray);
}

/* ---------- Gráficos ---------- */
.charts-grid {
  display: grid;
  grid-template-columns: 1.3fr 1fr;
  gap: 16px;
}

.bar-chart {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 10px;
  height: 170px;
  padding-top: 10px;
}
.bar-chart__col {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 100%;
  justify-content: flex-end;
}
.bar-chart__value {
  font-size: 11.5px;
  font-weight: 700;
  color: var(--color-muted);
  margin-bottom: 4px;
}
.bar-chart__bar {
  width: 60%;
  min-width: 22px;
  border-radius: 6px 6px 0 0;
  background: #94a3c9;
}
.bar-chart__bar--current {
  background: var(--color-blue);
}
.bar-chart__label {
  margin-top: 8px;
  font-size: 11.5px;
  color: var(--color-muted);
  text-transform: capitalize;
}

.pie-chart-wrap {
  display: flex;
  align-items: center;
  gap: 22px;
}
.pie-chart {
  width: 120px;
  height: 120px;
  flex-shrink: 0;
}
.pie-chart__label {
  font-size: 12px;
  font-weight: 700;
  pointer-events: none;
}
.pie-legend {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
  font-size: 12.5px;
}
.pie-legend li {
  display: flex;
  align-items: center;
  gap: 8px;
}
.pie-legend__label {
  color: var(--color-muted);
}
.pie-legend strong {
  margin-left: auto;
  font-size: 13px;
}

/* ---------- Tablas ---------- */
.tables-grid {
  display: grid;
  grid-template-columns: 1.3fr 1fr;
  gap: 16px;
}

.table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
.table th {
  text-align: left;
  font-size: 11px;
  letter-spacing: 0.05em;
  color: var(--color-muted);
  font-weight: 700;
  padding: 8px 6px;
  border-bottom: 2px solid var(--color-border);
}
.table td {
  padding: 11px 6px;
  border-bottom: 1px solid var(--color-border);
}
.table__empty {
  text-align: center;
  color: var(--color-muted);
  padding: 18px 6px;
  font-size: 12.5px;
}

.badge {
  display: inline-block;
  padding: 3px 10px;
  border-radius: 999px;
  font-size: 11px;
  font-weight: 600;
}
.badge--pendiente {
  background: #fff3cd;
  color: #8a6500;
}
.badge--aprobada {
  background: #e3f3e6;
  color: #1f6b34;
}
.badge--rechazada {
  background: #fdecea;
  color: #9b2c2c;
}
.badge--default {
  background: #eef0f3;
  color: var(--color-muted);
}

/* ---------- Actividad ---------- */
.activity {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.activity li {
  display: flex;
  align-items: flex-start;
  gap: 10px;
}
.activity .dot {
  margin-top: 6px;
}
.activity__text {
  margin: 0;
  font-size: 13px;
  font-weight: 600;
}
.activity__time {
  margin: 2px 0 0;
  font-size: 11.5px;
  color: var(--color-muted);
}

/* ---------- Skeletons ---------- */
.skeleton {
  background: linear-gradient(90deg, #eceef1 25%, #f4f5f7 37%, #eceef1 63%);
  background-size: 400% 100%;
  border-radius: 6px;
}
.skeleton--num {
  display: inline-block;
  width: 48px;
  height: 22px;
}
.skeleton--chart {
  height: 150px;
}
.skeleton--table {
  height: 140px;
}
@media (prefers-reduced-motion: no-preference) {
  .skeleton {
    animation: shimmer 1.4s ease infinite;
  }
}
@keyframes shimmer {
  0% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0 50%;
  }
}

/* ---------- Responsive ---------- */
@media (max-width: 960px) {
  .kpi-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  .charts-grid,
  .tables-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .topbar__date {
    display: none;
  }
  .kpi-grid {
    grid-template-columns: 1fr 1fr;
  }
  .content {
    padding: 16px;
  }
}

@media (max-width: 480px) {
  .kpi-grid {
    grid-template-columns: 1fr;
  }
}
</style>