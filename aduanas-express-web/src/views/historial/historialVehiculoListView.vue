<template>
  <div class="hist-page">

    <div class="hist-header">
      <h1 class="hist-title">Historial de Viajes</h1>
      <div class="hist-header-actions">
        <button @click="handleExportarPdf" class="btn-pdf">
          <span class="dot-red"></span>
          PDF
        </button>
        <button @click="handleExportarExcel" class="btn-excel">
          EXCEL
        </button>
        <button class="btn-filtros">
          Filtros Avanzados
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
      <input v-model="fechaHasta" type="date" class="filtro-date" />
      <select v-model="rol" class="filtro-select">
        <option v-for="r in ROLES" :key="r">{{ r }}</option>
      </select>
      <select v-model="estadoFiltro" class="filtro-select">
        <option v-for="e in ESTADOS" :key="e">{{ e }}</option>
      </select>
      <select v-model="area" class="filtro-select">
        <option v-for="a in AREAS" :key="a">{{ a }}</option>
      </select>
    </div>

    <div class="hist-tabla-wrap">
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
            <tr v-for="(v, i) in viajesPagina" :key="i">
              <td class="td-id">#{{ v.id }}</td>
              <td>{{ v.ruta }}</td>
              <td>
                <div class="conductor-cell">
                  <div class="avatar" :class="v.avatarClass">
                    {{ getInitials(v.conductor) }}
                  </div>
                  <span>{{ v.conductor }}</span>
                </div>
              </td>
              <td>{{ v.fecha }}</td>
              <td>{{ v.horario }}</td>
              <td><span class="badge-dur">{{ v.duracion }}</span></td>
              <td>{{ v.combustible }} gal</td>
              <td>
                <span class="badge-estado" :class="estadoClasses[v.estado]">
                  {{ v.estado }}
                </span>
              </td>
              <td>
                <div class="acciones">
                  <button @click="verDetalle(v)" class="btn-icon" title="Ver">👁</button>
                  <button @click="editarViaje(v)" class="btn-icon" title="Editar">✏️</button>
                  <button @click="eliminarViaje(v)" class="btn-icon del" title="Eliminar">🗑</button>
                </div>
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
import { ref, computed, watch } from 'vue'
import { exportarPdf, exportarExcel } from '../../services/reporteService'

const ESTADOS = ['Todos los estados', 'Activo', 'Inactivo', 'Bloqueado']
const ROLES   = ['Todos los roles', 'Conductor', 'Admin']
const AREAS   = ['Área - Todos', 'Sede Central', 'Sucursal Norte', 'Sucursal Sur']
const ITEMS_POR_PAGINA = 9

const columnas = ['#Viaje', 'Ruta', 'Conductor/Vehículo', 'Fecha', 'Horario', 'Duración', 'Combustible', 'Estado', 'Acciones']

const avatarClasses = ['av-orange', 'av-blue', 'av-green', 'av-purple', 'av-teal']

const estadoClasses = {
  Activo:    'estado-activo',
  Inactivo:  'estado-inactivo',
  Bloqueado: 'estado-bloqueado',
}

const filtro       = ref('')
const estadoFiltro = ref('Todos los estados')
const rol          = ref('Todos los roles')
const area         = ref('Área - Todos')
const fechaDesde   = ref('2026-01-01')
const fechaHasta   = ref('2026-01-30')
const pagina       = ref(1)

const datosMock = Array.from({ length: 48 }, (_, i) => ({
  id:          `V-${1248 + i}`,
  ruta:        'Sede Central',
  conductor:   'Juan Ramírez',
  fecha:       '29/05/2026',
  horario:     '07:00 - 09:15',
  duracion:    '2h 45m',
  combustible: [8.2, 5.8, 14.5, 4.1, 4.1, 9.0, 9.0, 9.0][i % 8],
  estado:      ['Activo', 'Inactivo', 'Activo', 'Inactivo', 'Bloqueado', 'Bloqueado', 'Bloqueado', 'Activo'][i % 8],
  avatarClass: avatarClasses[i % avatarClasses.length],
}))

const stats = [
  { label: 'Viajes totales', value: '1,240', colorClass: 'blue' },
  { label: 'Completas',      value: '1,190', colorClass: 'green' },
  { label: 'Cancelados',     value: '42',    colorClass: 'red' },
  { label: 'Galones usados', value: '9,840', colorClass: 'yellow' },
  { label: 'KM Recorridos',  value: '43,320',colorClass: 'gray' },
]

const viajesFiltrados = computed(() =>
  datosMock.filter((v) => {
    const matchFiltro =
      !filtro.value ||
      v.ruta.toLowerCase().includes(filtro.value.toLowerCase()) ||
      v.conductor.toLowerCase().includes(filtro.value.toLowerCase()) ||
      v.id.toLowerCase().includes(filtro.value.toLowerCase())
    const matchEstado = estadoFiltro.value === 'Todos los estados' || v.estado === estadoFiltro.value
    const matchArea   = area.value === 'Área - Todos' || v.ruta === area.value
    return matchFiltro && matchEstado && matchArea
  })
)

const totalPaginas = computed(() =>
  Math.ceil(viajesFiltrados.value.length / ITEMS_POR_PAGINA)
)

const viajesPagina = computed(() =>
  viajesFiltrados.value.slice(
    (pagina.value - 1) * ITEMS_POR_PAGINA,
    pagina.value * ITEMS_POR_PAGINA
  )
)

watch([filtro, estadoFiltro, area], () => { pagina.value = 1 })

function getInitials(name = '') {
  return name.split(' ').map((n) => n[0]).slice(0, 2).join('').toUpperCase()
}

async function handleExportarPdf() {
  try {
    const res = await exportarPdf(1, 2026)
    const url = window.URL.createObjectURL(new Blob([res.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'historial_1_2026.pdf')
    document.body.appendChild(link)
    link.click()
    link.remove()
  } catch (err) {
    console.error('Error exportando PDF', err)
  }
}

async function handleExportarExcel() {
  try {
    const res = await exportarExcel(1, 2026)
    const url = window.URL.createObjectURL(new Blob([res.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'historial_1_2026.xlsx')
    document.body.appendChild(link)
    link.click()
    link.remove()
  } catch (err) {
    console.error('Error exportando Excel', err)
  }
}

function verDetalle(viaje)  { console.log('Ver detalle:', viaje) }
function editarViaje(viaje) { console.log('Editar:', viaje) }
function eliminarViaje(viaje) {
  if (confirm(`¿Eliminar el viaje #${viaje.id}?`)) {
    console.log('Eliminar:', viaje)
  }
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
.estado-activo    { background: #d1fae5; color: #065f46; }
.estado-inactivo  { background: #f3f4f6; color: #4b5563; }
.estado-bloqueado { background: #fee2e2; color: #991b1b; }

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
