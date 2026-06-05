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

      <span class="navbar-fecha"><img src="../../assets/icons/calendario.png" alt="calendario" width="15" height="15" />   Jueves, 28 mayo 2026</span>
      <button><img src="../../assets/icons/buscador.png" alt="buscar" width="15" height="15" /></button>
      <button><img src="../../assets/icons/notificación.png" alt="notificación" width="15" height="15" /></button>
    </div>
  </header>
</template>

<style scoped>
@import '@/assets/styles/navbar.css';
</style>