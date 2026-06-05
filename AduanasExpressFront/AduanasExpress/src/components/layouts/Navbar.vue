<script setup>
import { useNavbarStore } from '../../stores/navbar'

const navbar = useNavbarStore()
</script>

<template>
  <header class="navbar">
    <h1 class="navbar-titulo">{{ navbar.titulo }}</h1>

    <div class="navbar-acciones">
      <button
        v-for="accion in navbar.acciones"
        :key="accion.label"
        class="navbar-btn"
      >
        {{ accion.label }}
      </button>

      <button v-if="navbar.mostrarBuscador">
        <img src="../../assets/icons/buscador.png" width="33" height="33" />
      </button>
      <button v-if="navbar.mostrarNotificaciones">
        <img src="../../assets/icons/notificación.png" width="33" height="33" />
      </button>
    </div>
  </header>
</template>

<style scoped>
@import '@/assets/styles/navbar.css';
</style>