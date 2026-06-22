<template>
  <div class="hist-page">

    <div class="hist-header">
      <h1 class="hist-title">Historial de Viajes</h1>
      <div class="hist-header-actions">
        <button @click="cargarDatos" class="btn-filtros" :disabled="cargando">
          {{ cargando ? 'Cargando...' : 'Actualizar' }}
        </button>
      </div>
    </div>

    <div class="hist-stats">
      <div v-for="stat in stats" :key="stat.label" class="stat-card" :class="stat.colorClass">
        <p class="stat-value">{{ stat.value }}</p>
        <p class="stat-label">{{ stat.label }}</p>
      </div>
    </div>

    <div class="hist-filtros">
      <input
        v-model="filtro"
        type="text"
        placeholder="Buscar por área, destino o Número de solicitud..."
        class="filtro-input"
      />
      <input v-model="fechaDesde" type="date" class="filtro-date" />
    </div>

    <!-- Error banner -->
    <div v-if="error" class="error-banner">⚠️ {{ error }}</div>

    <!-- Loading state -->
    <div v-if="cargando" class="cargando-wrap">
      <span class="spinner"></span>
      <span>Cargando historial...</span>
    </div>

    <div v-else class="hist-tabla-wrap">
      <div class="tabla-header">
        <h2 class="tabla-titulo">Registro de viajes</h2>
        <span class="tabla-badge">{{ viajesFiltrados.length.toLocaleString() }} Viajes</span>
      </div>

      <div class="tabla-scroll">
        <table>
          <thead>
            <tr>
              <th v-for="col in columnas" :key="col">{{ col }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="viajesFiltrados.length === 0">
              <td :colspan="columnas.length" class="td-empty">No hay viajes que coincidan con los filtros.</td>
            </tr>
            <tr v-for="(v, i) in viajesPagina" :key="i">
              <td class="td-id">#{{ v.id }}</td>
              <td>{{ v.ruta }}</td>
              <td>
                <div class="conductor-cell">
                  <div class="avatar" :class="v.avatarClass">
                    {{ getInitials(v.conductor) }}
                  </div>
                  <div>
                    <div>{{ v.conductor }}</div>
                    <div class="vehiculo-sub">{{ v.vehiculo }}</div>
                  </div>
                </div>
              </td>
              <td>{{ v.fecha }}</td>
              <td>
                <span class="badge-estado" :class="estadoClasses[v.estado]">
                  {{ v.estado }}
                </span>
              </td>

            </tr>
          </tbody>
        </table>
      </div>

      <div class="hist-paginacion">
        <span class="pag-info">
          Mostrando {{ (pagina - 1) * ITEMS_POR_PAGINA + 1 }}–{{ Math.min(pagina * ITEMS_POR_PAGINA, viajesFiltrados.length) }}
          de {{ viajesFiltrados.length.toLocaleString() }} viajes
        </span>
        <div class="pag-btns">
          <button
            @click="pagina = Math.max(1, pagina - 1)"
            :disabled="pagina === 1"
            class="btn-pag"
          >&lt;</button>
          <button
            @click="pagina = Math.min(totalPaginas, pagina + 1)"
            :disabled="pagina === totalPaginas"
            class="btn-pag"
          >&gt;</button>
        </div>
      </div>
    </div>

  </div>
</template>


<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { getReporteViajes } from '../../services/reporteService'

const ITEMS_POR_PAGINA = 9

const columnas = ['#Viaje', 'Ruta', 'Conductor/Vehículo', 'Fecha', 'Estado']

const avatarClasses = ['av-orange', 'av-blue', 'av-green', 'av-purple', 'av-teal']

const estadoClasses = {
  Pendiente:  'estado-pendiente',
  Aprobada:   'estado-activo',
  Finalizada: 'estado-finalizada',
  Rechazada:  'estado-bloqueado',
  Cancelada:  'estado-inactivo',
}

const filtro     = ref('')
const fechaDesde = ref('')
const pagina     = ref(1)
const cargando     = ref(false)
const error        = ref('')

const viajesDatos = ref([])

// Stats calculadas dinámicamente
const stats = computed(() => {
  const data = viajesDatos.value
  const total      = data.length
  const finalizadas = data.filter(v => v.estado === 'Finalizada').length
  const canceladas  = data.filter(v => v.estado === 'Cancelada').length
  const rechazadas  = data.filter(v => v.estado === 'Rechazada').length
  const pendientes  = data.filter(v => v.estado === 'Pendiente').length
  return [
    { label: 'Viajes totales',  value: total.toLocaleString(),      colorClass: 'blue' },
    { label: 'Finalizadas',     value: finalizadas.toLocaleString(), colorClass: 'green' },
    { label: 'Cancelados',      value: canceladas.toLocaleString(),  colorClass: 'red' },
    { label: 'Rechazadas',      value: rechazadas.toLocaleString(),  colorClass: 'yellow' },
    { label: 'Pendientes',      value: pendientes.toLocaleString(),  colorClass: 'gray' },
  ]
})

// Mapea los datos recibidos desde la API a la estructura usada por la vista
function mapReporteToVista(item) {
  const rawFecha = item.fechaViaje ?? item.FechaViaje ?? null
  return {
    id:          item.id ?? item.Id,
    ruta:        item.areaSolicitante && item.destino
                   ? `${item.areaSolicitante} → ${item.destino}`
                   : (item.areaSolicitante ?? item.destino ?? '—'),
    conductor:   item.nombreConductor ?? item.NombreConductor ?? 'Sin asignar',
    vehiculo:    item.vehiculoPlaca   ?? item.VehiculoPlaca   ?? '—',
    fecha:       rawFecha ? formatDate(rawFecha) : '-',
    fechaRaw:    rawFecha,
    estado:      item.estado ?? item.Estado ?? 'Desconocido',
    avatarClass: avatarClasses[
      ((item.nombreConductor ?? item.NombreConductor ?? 'X').length) % avatarClasses.length
    ],
  }
}

function formatDate(d) {
  if (!d) return '-'
  const date = new Date(d)
  if (isNaN(date)) return String(d)
  return date.toLocaleDateString()
}

async function cargarDatos() {
  cargando.value = true
  error.value = ''
  try {
    // Usamos mes=0 y año=0 como parámetros genéricos ya que el backend actualmente ignora el filtro
    const res = await getReporteViajes(0, 0)
    viajesDatos.value = (res.data ?? []).map(mapReporteToVista)
  } catch (err) {
    console.error('Error cargando historial:', err)
    error.value = 'No se pudo cargar el historial. Verifica la conexión con el servidor.'
  } finally {
    cargando.value = false
  }
}

onMounted(() => {
  cargarDatos()
})


const viajesFiltrados = computed(() =>
  viajesDatos.value.filter((v) => {
    const q = (filtro.value || '').toString().toLowerCase()
    const matchFiltro =
      !q ||
      (v.ruta || '').toString().toLowerCase().includes(q) ||
      (v.conductor || '').toString().toLowerCase().includes(q) ||
      (v.id || '').toString().toLowerCase().includes(q)

    const matchFecha = !fechaDesde.value || !v.fechaRaw
      ? true
      : new Date(v.fechaRaw).toISOString().slice(0, 10) === fechaDesde.value

    return matchFiltro && matchFecha
  })
)

const totalPaginas = computed(() => Math.max(1, Math.ceil(viajesFiltrados.value.length / ITEMS_POR_PAGINA)))

const viajesPagina = computed(() => viajesFiltrados.value.slice((pagina.value - 1) * ITEMS_POR_PAGINA, pagina.value * ITEMS_POR_PAGINA))

watch([filtro, fechaDesde], () => { pagina.value = 1 })

function getInitials(name = '') {
  return (name || '').split(' ').map((n) => n[0] || '').slice(0, 2).join('').toUpperCase()
}


</script>


<style scoped>

.hist-page {
  padding: 32px 40px;
  background: #f3f4f6;
  min-height: 100vh;
  font-family: 'Inter', 'Segoe UI', sans-serif;
}

.hist-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 28px;
}

.hist-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
  letter-spacing: -0.02em;
  margin: 0;
}

.hist-header-actions {
  display: flex;
  gap: 10px;
}

.btn-pdf {
  display: inline-flex;
  align-items: center;
  gap: 7px;
  padding: 9px 18px;
  background: #fff;
  border: 1.5px solid #fca5a5;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 500;
  color: #dc2626;
  cursor: pointer;
  transition: background 0.15s;
}
.btn-pdf:hover { background: #fef2f2; }

.dot-red {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #ef4444;
  flex-shrink: 0;
}

.btn-excel {
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
  transition: background 0.15s;
}
.btn-excel:hover { background: #f9fafb; }

.btn-filtros {
  display: inline-flex;
  align-items: center;
  gap: 7px;
  padding: 9px 18px;
  background: #1f2937;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  color: #fff;
  cursor: pointer;
  transition: background 0.15s;
}
.btn-filtros:hover { background: #111827; }

.hist-stats {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 14px;
  margin-bottom: 24px;
}

.stat-card {
  background: #fff;
  border-radius: 12px;
  border-left: 4px solid #d1d5db;
  box-shadow: 0 1px 3px rgba(0,0,0,.06);
  padding: 16px 18px;
}
.stat-card.blue   { border-left-color: #60a5fa; }
.stat-card.green  { border-left-color: #4ade80; }
.stat-card.red    { border-left-color: #fca5a5; }
.stat-card.yellow { border-left-color: #fbbf24; }
.stat-card.gray   { border-left-color: #d1d5db; }

.stat-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
  margin: 0;
  line-height: 1;
}

.stat-label {
  font-size: 0.75rem;
  color: #6b7280;
  margin: 5px 0 0;
}

.hist-filtros {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 12px;
  margin-bottom: 16px;
  box-shadow: 0 1px 3px rgba(0,0,0,.05);
}

.filtro-input {
  flex: 1;
  min-width: 200px;
  border: 1.5px solid #e5e7eb;
  border-radius: 8px;
  padding: 9px 14px;
  font-size: 0.875rem;
  color: #111827;
  outline: none;
  transition: border-color 0.15s;
}
.filtro-input:focus { border-color: #9ca3af; }
.filtro-input::placeholder { color: #9ca3af; }

.filtro-date,
.filtro-select {
  padding: 9px 12px;
  border: 1.5px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.875rem;
  color: #374151;
  outline: none;
  background: #fff;
  cursor: pointer;
  transition: border-color 0.15s;
}
.filtro-date:focus,
.filtro-select:focus { border-color: #9ca3af; }

.hist-tabla-wrap {
  background: #fff;
  border-radius: 14px;
  border: 1px solid #e5e7eb;
  box-shadow: 0 1px 4px rgba(0,0,0,.06);
  overflow: hidden;
}

.tabla-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 20px;
  border-bottom: 1px solid #f3f4f6;
}

.tabla-titulo {
  font-size: 0.9rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

.tabla-badge {
  background: #1f2937;
  color: #fff;
  font-size: 0.75rem;
  font-weight: 600;
  padding: 3px 12px;
  border-radius: 20px;
}

.tabla-scroll {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;
}

thead tr {
  border-bottom: 1px solid #f3f4f6;
}

thead th {
  padding: 11px 20px;
  text-align: left;
  font-size: 0.75rem;
  font-weight: 500;
  color: #6b7280;
  white-space: nowrap;
}

tbody tr {
  border-bottom: 1px solid #f9fafb;
  transition: background 0.15s;
}
tbody tr:hover { background: #f9fafb; }

tbody td {
  padding: 12px 20px;
  color: #4b5563;
}

.td-id {
  font-weight: 600;
  color: #111827;
}

.conductor-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.72rem;
  font-weight: 700;
  color: #fff;
  flex-shrink: 0;
}
.av-orange { background: #fb923c; }
.av-blue   { background: #3b82f6; }
.av-green  { background: #22c55e; }
.av-purple { background: #a855f7; }
.av-teal   { background: #14b8a6; }

.badge-dur {
  background: #f3f4f6;
  color: #4b5563;
  padding: 3px 8px;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: 500;
}

.badge-estado {
  display: inline-block;
  padding: 3px 12px;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  white-space: nowrap;
}
/* Estados reales del backend */
.estado-activo    { background: #d1fae5; color: #065f46; } /* Aprobada */
.estado-finalizada{ background: #dbeafe; color: #1e40af; }
.estado-pendiente { background: #fef3c7; color: #92400e; }
.estado-bloqueado { background: #fee2e2; color: #991b1b; } /* Rechazada */
.estado-inactivo  { background: #f3f4f6; color: #4b5563; } /* Cancelada */

/* Sub-label placa del vehículo */
.vehiculo-sub {
  font-size: 0.7rem;
  color: #9ca3af;
  margin-top: 1px;
}

/* Fila vacía */
.td-empty {
  text-align: center;
  padding: 32px 20px;
  color: #9ca3af;
  font-size: 0.875rem;
}

/* Banner de error */
.error-banner {
  background: #fee2e2;
  color: #991b1b;
  border: 1px solid #fca5a5;
  border-radius: 10px;
  padding: 12px 18px;
  margin-bottom: 16px;
  font-size: 0.875rem;
}

/* Loading spinner */
.cargando-wrap {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 64px 20px;
  color: #6b7280;
  font-size: 0.9rem;
}

.spinner {
  display: inline-block;
  width: 22px;
  height: 22px;
  border: 3px solid #e5e7eb;
  border-top-color: #374151;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
  flex-shrink: 0;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.acciones {
  display: flex;
  gap: 4px;
}

.btn-icon {
  width: 26px;
  height: 26px;
  border: 1.5px solid #e5e7eb;
  border-radius: 6px;
  background: #fff;
  font-size: 0.75rem;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: border-color 0.15s;
  color: #9ca3af;
}
.btn-icon:hover     { border-color: #9ca3af; }
.btn-icon.del:hover { border-color: #fca5a5; }

.hist-paginacion {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 20px;
  border-top: 1px solid #f3f4f6;
}

.pag-info {
  font-size: 0.75rem;
  color: #6b7280;
}

.pag-btns {
  display: flex;
  gap: 4px;
}

.btn-pag {
  padding: 4px 12px;
  border: 1.5px solid #e5e7eb;
  border-radius: 6px;
  background: #fff;
  font-size: 0.8rem;
  color: #6b7280;
  cursor: pointer;
  transition: background 0.15s;
}
.btn-pag:hover:not(:disabled) { background: #f3f4f6; }
.btn-pag:disabled { opacity: 0.4; cursor: default; }

@media (max-width: 1024px) {
  .hist-stats { grid-template-columns: repeat(3, 1fr); }
}

@media (max-width: 640px) {
  .hist-page   { padding: 20px 16px; }
  .hist-stats  { grid-template-columns: repeat(2, 1fr); }
  .hist-header { flex-direction: column; align-items: flex-start; gap: 14px; }
}
</style>
