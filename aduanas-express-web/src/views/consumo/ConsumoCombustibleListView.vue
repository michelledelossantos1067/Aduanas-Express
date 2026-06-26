<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { verConsumos, eliminarConsumo } from '@/services/consumoCombustibleService'

const authStore = useAuthStore()
const consumos = ref([])
const cargando = ref(false)
const error = ref('')
const busqueda = ref('')

const puedeCrear = computed(() => authStore.tienePermiso('consumo-combustible', 'crear'))
const puedeEditar = computed(() => authStore.tienePermiso('consumo-combustible', 'editar'))
const puedeEliminar = computed(() => authStore.tienePermiso('consumo-combustible', 'eliminar'))

const resumen = computed(() => {
  const total = consumos.value.length
  const totalGalones = consumos.value.reduce((acc, c) => acc + (c.galones || 0), 0)
  const totalCosto = consumos.value.reduce((acc, c) => acc + (c.costoTotal || 0), 0)
  return { total, totalGalones: totalGalones.toFixed(2), totalCosto: totalCosto.toFixed(2) }
})

const consumosFiltrados = computed(() => {
  const q = busqueda.value.toLowerCase()
  if (!q) return consumos.value
  return consumos.value.filter(c =>
    String(c.vehiculoId).includes(q) ||
    String(c.id).includes(q)
  )
})

async function cargarConsumos() {
  cargando.value = true
  error.value = ''
  try {
    const res = await verConsumos()
    consumos.value = res.data
  } catch (e) {
    error.value = 'No se pudieron cargar los consumos.'
  } finally {
    cargando.value = false
  }
}

async function handleEliminar(id) {
  if (!confirm('¿Eliminar este registro?')) return
  try {
    await eliminarConsumo(id)
    consumos.value = consumos.value.filter(c => c.id !== id)
  } catch {
    alert('Error al eliminar.')
  }
}

function formatFecha(fecha) {
  if (!fecha) return '—'
  return new Date(fecha).toLocaleDateString('es-DO', {
    day: '2-digit', month: '2-digit', year: 'numeric',
  })
}

onMounted(cargarConsumos)
</script>

<template>
  <div class="page">

    <div class="page-header">
      <h1 class="page-title">Consumo de Combustible</h1>
      <RouterLink v-if="puedeCrear" to="/consumo-combustible/nuevo" class="btn-nuevo">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
          <line x1="12" y1="5" x2="12" y2="19" />
          <line x1="5" y1="12" x2="19" y2="12" />
        </svg>
        Nuevo Registro
      </RouterLink>
    </div>

    <div class="resumen">
      <div class="resumen-card">
        <span class="resumen-dot dot-total"></span>
        <div>
          <p class="resumen-num">{{ resumen.total }}</p>
          <p class="resumen-label">Total Registros</p>
        </div>
      </div>
      <div class="resumen-card">
        <span class="resumen-dot dot-galones"></span>
        <div>
          <p class="resumen-num">{{ resumen.totalGalones }}</p>
          <p class="resumen-label">Total Galones</p>
        </div>
      </div>
      <div class="resumen-card">
        <span class="resumen-dot dot-costo"></span>
        <div>
          <p class="resumen-num">${{ resumen.totalCosto }}</p>
          <p class="resumen-label">Costo Total</p>
        </div>
      </div>
    </div>

    <div class="filtros">
      <div class="filtro-search">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
          <circle cx="11" cy="11" r="8" />
          <line x1="21" y1="21" x2="16.65" y2="16.65" />
        </svg>
        <input v-model="busqueda" type="text" placeholder="Buscar por ID o vehículo..." class="filtro-input" />
      </div>
    </div>

    <div v-if="cargando" class="estado-carga">
      <div class="spinner"></div>
      <p>Cargando registros…</p>
    </div>

    <div v-else-if="error" class="estado-error">
      <p>{{ error }}</p>
      <button class="btn-reintentar" @click="cargarConsumos">Reintentar</button>
    </div>

    <div v-else-if="consumosFiltrados.length === 0" class="estado-vacio">
      <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#d1d5db" stroke-width="1.5">
        <path d="M3 3h18v4H3zM3 7l2 14h14l2-14" />
        <path d="M10 11h4" />
      </svg>
      <p>No se encontraron registros</p>
      <span>Agrega un nuevo registro de consumo.</span>
    </div>

    <div v-else class="tabla-wrap">
      <table class="tabla">
        <thead>
          <tr>
            <th>#</th>
            <th>Fecha</th>
            <th>Vehículo</th>
            <th>Galones</th>
            <th>Costo/Galón</th>
            <th>Costo Total</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="consumo in consumosFiltrados" :key="consumo.id">
            <td class="td-id">{{ consumo.id }}</td>
            <td>{{ formatFecha(consumo.fecha) }}</td>
            <td>{{ consumo.vehiculoId }}</td>
            <td>{{ consumo.galones }} gal</td>
            <td>${{ consumo.costoPorGalon }}</td>
            <td class="td-total">${{ consumo.costoTotal }}</td>
            <td>
              <div class="acciones">
                <RouterLink v-if="puedeEditar" :to="`/consumo-combustible/${consumo.id}/editar`" class="btn-accion btn-editar">
                  Editar
                </RouterLink>
                <button v-if="puedeEliminar" @click="handleEliminar(consumo.id)" class="btn-accion btn-eliminar">
                  Eliminar
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

  </div>
</template>

<style scoped>
.page {
  padding: 32px 40px;
  background: #f3f4f6;
  min-height: 100vh;
  font-family: 'Inter', 'Segoe UI', sans-serif;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 28px;
}

.page-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
  letter-spacing: -0.02em;
  margin: 0;
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
  text-decoration: none;
  transition: background 0.15s;
}

.btn-nuevo:hover { background: #14532d; }

.resumen {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
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
  box-shadow: 0 1px 3px rgba(0,0,0,.06);
}

.resumen-dot {
  width: 14px;
  height: 14px;
  border-radius: 4px;
  flex-shrink: 0;
}

.dot-total { background: #d1d5db; }
.dot-galones { background: #bfdbfe; }
.dot-costo { background: #bbf7d0; }

.resumen-num {
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
  margin: 0;
  line-height: 1;
}

.resumen-label {
  font-size: 0.78rem;
  color: #6b7280;
  margin: 4px 0 0;
}

.filtros {
  margin-bottom: 20px;
}

.filtro-search {
  display: flex;
  align-items: center;
  gap: 10px;
  background: #fff;
  border: 1.5px solid #e5e7eb;
  border-radius: 10px;
  padding: 0 14px;
  max-width: 400px;
  transition: border-color 0.15s;
}

.filtro-search:focus-within { border-color: #1a3a2a; }

.filtro-input {
  flex: 1;
  border: none;
  outline: none;
  font-size: 0.9rem;
  color: #111827;
  padding: 11px 0;
  background: transparent;
}

.filtro-input::placeholder { color: #9ca3af; }

.tabla-wrap {
  background: #fff;
  border-radius: 14px;
  box-shadow: 0 1px 4px rgba(0,0,0,.07);
  overflow: hidden;
}

.tabla {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;
}

.tabla thead {
  background: #f9fafb;
  border-bottom: 1.5px solid #e5e7eb;
}

.tabla th {
  padding: 13px 16px;
  text-align: left;
  font-size: 0.75rem;
  font-weight: 600;
  color: #6b7280;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.tabla tbody tr {
  border-bottom: 1px solid #f3f4f6;
  transition: background 0.12s;
}

.tabla tbody tr:last-child { border-bottom: none; }
.tabla tbody tr:hover { background: #f9fafb; }

.tabla td {
  padding: 13px 16px;
  color: #374151;
}

.td-id {
  font-weight: 700;
  color: #111827;
}

.td-total {
  font-weight: 600;
  color: #065f46;
}

.acciones {
  display: flex;
  gap: 8px;
}

.btn-accion {
  padding: 5px 12px;
  border-radius: 6px;
  font-size: 0.78rem;
  font-weight: 600;
  cursor: pointer;
  border: none;
  text-decoration: none;
  transition: filter 0.15s;
}

.btn-accion:hover { filter: brightness(0.93); }
.btn-editar { background: #fef3c7; color: #92400e; }
.btn-eliminar { background: #fee2e2; color: #991b1b; }

.estado-carga {
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
  animation: spin 0.75s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.estado-error {
  background: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 10px;
  padding: 20px 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  color: #991b1b;
}

.btn-reintentar {
  padding: 7px 16px;
  background: #fff;
  border: 1.5px solid #fca5a5;
  border-radius: 8px;
  color: #991b1b;
  font-size: 0.8rem;
  cursor: pointer;
}

.estado-vacio {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 72px 0;
  color: #9ca3af;
  text-align: center;
}

.estado-vacio p {
  font-size: 1rem;
  font-weight: 600;
  color: #6b7280;
  margin: 8px 0 0;
}

.estado-vacio span { font-size: 0.85rem; }

@media (max-width: 768px) {
  .page { padding: 20px 16px; }
  .resumen { grid-template-columns: 1fr; }
  .tabla-wrap { overflow-x: auto; }
}
</style>