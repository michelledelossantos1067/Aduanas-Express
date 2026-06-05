<script setup>
import { ref, computed, onMounted } from 'vue'
import Sidebar from '../components/layouts/Sidebar.vue'
import Navbar from '../components/layouts/Navbar.vue'
import { useNavbarStore } from '../stores/navbar'
import vehiculos from '../mock/vehiculos.json'
import vehiculosEstado from '../mock/vehiculos_estado.json'

const navbar = useNavbarStore()

onMounted(() => {
  navbar.setTitulo('Vehículos')
  navbar.setAcciones([
    { label: 'Exportar', accion: 'exportar' },
    { label: '+ Nuevo Vehículo', accion: 'nuevo' }
  ])
    navbar.setExtras(false, false)
})

const busqueda = ref('')
const filtroEstado = ref('Todos los estados')
const filtroTipo = ref('Todos los tipos')

const vehiculosFiltrados = computed(() => {
  return vehiculos.filter(v => {
    const coincideBusqueda =
      v.placa.toLowerCase().includes(busqueda.value.toLowerCase()) ||
      v.modelo.toLowerCase().includes(busqueda.value.toLowerCase())
    const coincideEstado =
      filtroEstado.value === 'Todos los estados' || v.estado === filtroEstado.value
    const coincideTipo =
      filtroTipo.value === 'Todos los tipos' || v.tipo === filtroTipo.value
    return coincideBusqueda && coincideEstado && coincideTipo
  })
})
</script>

<template>
  <div class="vehiculos-page">
    <Sidebar />

    <div class="vehiculos-content">
      <Navbar />

      <!-- Stats -->
      <div class="vehiculos-stats">
        <div v-for="v in vehiculosEstado" :key="v.label" class="vstat-card">
          <div class="vstat-dot" :style="{ backgroundColor: v.color }"></div>
          <div>
            <h2>{{ v.value }}</h2>
            <p>{{ v.label }}</p>
          </div>
        </div>
      </div>

      <!-- Filtros -->
      <div class="vehiculos-filtros">
        <div class="filtro-busqueda">
          <span><img src="@/assets/icons/buscador.png" alt="Buscar"  width="15" height="15" /></span>
          <input v-model="busqueda" type="text" placeholder="Buscar por matrícula, marca o modelo..." />
        </div>

        <select v-model="filtroEstado">
          <option>Todos los estados</option>
          <option>Disponible</option>
          <option>En viaje</option>
          <option>Mantenimiento</option>
          <option>Fuera servicio</option>
        </select>
        <select v-model="filtroTipo">
          <option>Todos los tipos</option>
          <option>Minibús</option>
          <option>Bus</option>
          <option>Van</option>
        </select>
      </div>

      <div class="vehiculos-grid">
        <div v-for="v in vehiculosFiltrados" :key="v.id" class="vehiculo-card">
          <div class="vehiculo-card-header">
            <span class="vehiculo-placa">{{ v.placa }}</span>
            <span class="vehiculo-estado" :class="v.estado.toLowerCase().replace(' ', '-')">{{ v.estado }}</span>
          </div>
          <p class="vehiculo-modelo">{{ v.modelo }}</p>
          <p class="vehiculo-tipo">{{ v.tipo }} · {{ v.capacidad }} pasajeros</p>
          <div class="vehiculo-detalles">
            <div><span>Color</span><span>{{ v.color }}</span></div>
            <div><span>Kilometraje</span><span>{{ v.kilometraje }}</span></div>
            <div><span>Últ. Mantenimiento</span><span>{{ v.ultimoMantenimiento }}</span></div>
          </div>
          <div class="vehiculo-acciones">
            <button class="btn-ver">Ver</button>
            <button class="btn-editar">Editar</button>
            <button class="btn-eliminar">Eliminar</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import '@/assets/styles/vehiculos.css';
</style>