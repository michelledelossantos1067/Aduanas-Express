<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { verConsumos, eliminarConsumo } from '@/services/consumoCombustibleService'

const authStore = useAuthStore()
const consumos = ref([])
const cargando = ref(true)
const error = ref(null)

const puedeCrear = computed(() => authStore.tienePermiso('consumo', 'crear'))
const puedeEditar = computed(() => authStore.tienePermiso('consumo', 'editar'))
const puedeEliminar = computed(() => authStore.tienePermiso('consumo', 'eliminar'))

onMounted(async () => {
  try {
    const res = await verConsumos()
    consumos.value = res.data
  } catch (e) {
    error.value = 'Error al cargar los consumos.'
  } finally {
    cargando.value = false
  }
})

async function handleEliminar(id) {
  if (!confirm('¿Eliminar este registro?')) return
  try {
    await eliminarConsumo(id)
    consumos.value = consumos.value.filter(c => c.id !== id)
  } catch {
    alert('Error al eliminar.')
  }
}
</script>

<template>
  <div class="p-4">
    <div class="flex justify-between items-center mb-4">
      <h1 class="text-2xl font-bold">Consumo de Combustible</h1>
      <RouterLink v-if="puedeCrear" to="/consumo-combustible/nuevo" class="btn btn-primary">
        + Nuevo Registro
      </RouterLink>
    </div>

    <div v-if="cargando">Cargando...</div>
    <div v-else-if="error" class="text-red-500">{{ error }}</div>

    <table v-else class="table w-full">
      <thead>
        <tr>
          <th>ID</th>
          <th>Fecha</th>
          <th>Galones</th>
          <th>Costo/Galón</th>
          <th>Costo Total</th>
          <th>Vehículo</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="consumo in consumos" :key="consumo.id">
          <td>{{ consumo.id }}</td>
          <td>{{ consumo.fecha ? new Date(consumo.fecha).toLocaleDateString() : 'N/A' }}</td>
          <td>{{ consumo.galones }}</td>
          <td>{{ consumo.costoPorGalon }}</td>
          <td>{{ consumo.costoTotal }}</td>
          <td>{{ consumo.vehiculoId }}</td>
          <td class="flex gap-2">
            <RouterLink v-if="puedeEditar" :to="`/consumo-combustible/${consumo.id}/editar`"
              class="btn btn-sm btn-warning">
              Editar
            </RouterLink>
            <button v-if="puedeEliminar" @click="handleEliminar(consumo.id)" class="btn btn-sm btn-error">
              Eliminar
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
