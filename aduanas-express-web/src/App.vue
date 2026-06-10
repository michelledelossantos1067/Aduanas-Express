<template>
  <div class="app-layout">
    <AppSidebar v-if="authStore.token && !esRutaPublica" />
    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import AppSidebar from './components/AppSidebar.vue'
import { useAuthStore } from './stores/authStore'

const authStore = useAuthStore()
const route = useRoute()

const rutasPublicas = ['login', 'register', 'reset-password', 'change-password']

const esRutaPublica = computed(() => rutasPublicas.includes(route.name))
</script>

<style>
.app-layout {
  display: flex;
  min-height: 100vh;
}
.main-content {
  flex: 1;
  overflow-y: auto;
}
</style>