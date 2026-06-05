<script setup>
import { onMounted } from 'vue'
import Sidebar from '../components/layouts/Sidebar.vue'
import Navbar from '../components/layouts/Navbar.vue'
import { useNavbarStore } from '../stores/navbar'
import stats from '../mock/stats.json'
import solicitudes from '../mock/solicitudes.json'
import actividad from '../mock/actividad.json'
import vehiculos from '../mock/vehiculos_estado.json'

const navbar = useNavbarStore()

onMounted(() => {
  navbar.setTitulo('Dashboard')
  navbar.setAcciones([])
})
</script>

<template>
  <div class="dashboard">
    <Sidebar />

    <div class="dashboard-content">
      <Navbar />

      <div class="stats-grid">
        <div v-for="stat in stats" :key="stat.label" class="stat-card">
          <p>{{ stat.label }}</p>
          <h2>{{ stat.value }}</h2>
          <span class="stat-sub" :class="stat.type">{{ stat.sub }}</span>
        </div>
      </div>

      <div class="charts-grid">
        <div class="chart-card">
          <div class="chart-card-header">
            <p>Viajes por mes</p>
            <a>Ver detalle</a>
          </div>
          <div class="chart-placeholder">Gráfico próximamente</div>
        </div>

        <div class="chart-card">
          <div class="chart-card-header">
            <p>Estado de vehículos</p>
          </div>
          <ul class="actividad-list">
            <li v-for="v in vehiculos" :key="v.label">
              <div class="actividad-dot" :style="{ borderColor: v.color }"></div>
              <span class="actividad-desc">{{ v.label }}</span>
              <span style="margin-left: auto; font-weight: bold;">{{ v.value }}</span>
            </li>
          </ul>
        </div>
      </div>

      <div class="bottom-grid">
        <div class="table-card">
          <div class="table-card-header">
            <p>Solicitudes recientes</p>
            <a>Ver todas</a>
          </div>
          <table>
            <thead>
              <tr>
                <th>Área</th>
                <th>Destino</th>
                <th>Fecha</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="s in solicitudes" :key="s.id">
                <td>{{ s.area }}</td>
                <td>{{ s.destino }}</td>
                <td>{{ s.fecha }}</td>
                <td>{{ s.estado }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="table-card">
          <div class="table-card-header">
            <p>Actividad reciente</p>
            <a>Ver historial</a>
          </div>
          <ul class="actividad-list">
            <li v-for="a in actividad" :key="a.id">
              <div class="actividad-dot"></div>
              <div>
                <p class="actividad-desc">{{ a.descripcion }}</p>
                <p class="actividad-tiempo">{{ a.tiempo }}</p>
              </div>
            </li>
          </ul>
        </div>
      </div>

    </div>
  </div>
</template>

<style scoped>
@import '@/assets/styles/dashboard.css';
</style>