<script setup>
import { useRouter } from 'vue-router'
import user from '../../mock/usuario.json'

const router = useRouter()

const menuItems = [
  {
    section: 'PRINCIPAL',
    items: [
      { name: 'Dashboard', route: '/dashboard' }
    ]
  },
  {
    section: 'GESTIÓN',
    items: [
      { name: 'Vehículos', route: '/vehiculos' },
      { name: 'Conductores', route: '/conductores' },
      { name: 'Solicitudes', route: '/solicitudes' },
      { name: 'Asignaciones', route: '/asignaciones' }
    ]
  },
  {
    section: 'OPERACIÓN',
    items: [
      { name: 'Agenda', route: '/agenda' },
      { name: 'Mantenimiento', route: '/mantenimiento' },
      { name: 'Reportes', route: '/reportes' },
      { name: 'Monitoreo', route: '/monitoreo' },
      { name: 'Historial', route: '/historial' }
    ]
  },
  {
    section: 'ADMINISTRACIÓN',
    items: [
      { name: 'Usuarios', route: '/usuarios' },
      { name: 'Roles', route: '/roles' }
    ]
  }
]
</script>

<template>
  <aside class="sidebar">
    <div class="sidebar-logo">
      <div class="logo-icon"><img src="../../assets/icons/logo.png" alt="buscar" width="90" height="90" /></div>
      <div>
        <p class="logo-title">Aduanas Express</p>
        <p class="logo-subtitle">Sistema de transporte</p>
      </div>
    </div>

    <nav class="sidebar-nav">
      <div v-for="group in menuItems" :key="group.section">
        <p class="nav-section">{{ group.section }}</p>
        <ul>
          <li
            v-for="item in group.items"
            :key="item.name"
            class="nav-item"
            @click="router.push(item.route)"
          >
            {{ item.name }}
          </li>
        </ul>
      </div>
    </nav>

    <div class="sidebar-user">
      <div class="user-avatar">{{ user.initials }}</div>
      <div>
        <p class="user-name">{{ user.name }}</p>
        <p class="user-role">{{ user.role }}</p>
      </div>
    </div>
  </aside>
</template>

<style scoped>
@import '@/assets/styles/sidebar.css';
</style>