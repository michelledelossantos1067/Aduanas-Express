<script setup>
import { computed, reactive } from 'vue'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()

const userInitials = computed(() => {
    const name = authStore.usuario?.nombre || ''
    return name
        .split(' ')
        .map(n => n[0])
        .join('')
        .substring(0, 2)
        .toUpperCase()
})

// Estado de apertura/cierre de cada sección (tipo dropdown)
const openSections = reactive({
    gestion: true,
    operacion: false,
    administracion: false
})

function toggleSection(key) {
    openSections[key] = !openSections[key]
}
</script>

<template>
    <aside class="sidebar">
        <!-- Header -->
        <div class="sidebar-header">
            <div class="sidebar-logo">
                <div class="logo-icon">🚛</div>
                <div class="logo-text">
                    <span class="logo-title">Aduanas Express</span>
                    <span class="logo-subtitle">Sistema de transporte</span>
                </div>
            </div>
        </div>

        <!-- Navigation -->
        <nav class="sidebar-nav">
            <!-- PRINCIPAL (siempre visible) -->
            <div class="nav-section">
                <span class="nav-section-title">PRINCIPAL</span>
                <router-link to="/dashboard" class="nav-item">
                    <span class="nav-icon">⊞</span>
                    <span>Dashboard</span>
                </router-link>
            </div>

            <!-- GESTIÓN (dropdown) -->
            <div class="nav-section">
                <button class="nav-section-toggle" @click="toggleSection('gestion')">
                    <span class="nav-section-title">GESTIÓN</span>
                    <span class="nav-chevron" :class="{ open: openSections.gestion }">▾</span>
                </button>
                <div class="nav-collapsible" v-show="openSections.gestion">
                    <router-link to="/vehiculos" class="nav-item">
                        <span class="nav-icon">🚌</span>
                        <span>Vehiculos</span>
                    </router-link>
                    <router-link to="/conductores" class="nav-item">
                        <span class="nav-icon">👤</span>
                        <span>Conductores</span>
                    </router-link>
                    <router-link to="/solicitudes" class="nav-item">
                        <span class="nav-icon">📋</span>
                        <span>Solicitudes</span>
                    </router-link>
                    <router-link to="/asignaciones" class="nav-item">
                        <span class="nav-icon">📝</span>
                        <span>Asignaciones</span>
                    </router-link>
                </div>
            </div>

            <!-- OPERACIÓN (dropdown) -->
            <div class="nav-section">
                <button class="nav-section-toggle" @click="toggleSection('operacion')">
                    <span class="nav-section-title">OPERACIÓN</span>
                    <span class="nav-chevron" :class="{ open: openSections.operacion }">▾</span>
                </button>
                <div class="nav-collapsible" v-show="openSections.operacion">
                    <router-link to="/agenda" class="nav-item">
                        <span class="nav-icon">📅</span>
                        <span>Agenda</span>
                    </router-link>
                    <router-link to="/mantenimiento" class="nav-item">
                        <span class="nav-icon">🔧</span>
                        <span>Mantenimiento</span>
                    </router-link>
                    <router-link to="/reportes" class="nav-item">
                        <span class="nav-icon">📊</span>
                        <span>Reportes</span>
                    </router-link>
                    <router-link to="/monitoreo" class="nav-item">
                        <span class="nav-icon">🖥️</span>
                        <span>Monitoreo</span>
                    </router-link>
                    <router-link to="/historial" class="nav-item">
                        <span class="nav-icon">🗂️</span>
                        <span>Historial</span>
                    </router-link>
                </div>
            </div>

            <!-- ADMINISTRACIÓN (dropdown) -->
            <div class="nav-section">
                <button class="nav-section-toggle" @click="toggleSection('administracion')">
                    <span class="nav-section-title">ADMINISTRACIÓN</span>
                    <span class="nav-chevron" :class="{ open: openSections.administracion }">▾</span>
                </button>
                <div class="nav-collapsible" v-show="openSections.administracion">
                    <router-link to="/usuarios" class="nav-item">
                        <span class="nav-icon">👥</span>
                        <span>Usuarios</span>
                    </router-link>
                    <router-link to="/roles" class="nav-item">
                        <span class="nav-icon">⚙️</span>
                        <span>Roles</span>
                    </router-link>
                </div>
            </div>
        </nav>

        <!-- Footer -->
        <div class="sidebar-footer">
            <div class="user-avatar">{{ userInitials }}</div>
            <div class="user-info">
                <span class="user-name">{{ authStore.usuario?.nombre || 'Usuario' }}</span>
                <span class="user-role">{{ authStore.usuario?.rol || 'Sin rol' }}</span>
            </div>
        </div>
    </aside>
</template>

<style scoped>
.sidebar {
    position: sticky;
    top: 0;
    width: 250px;
    height: 100vh;
    background-color: #1a3a2e;
    display: flex;
    flex-direction: column;
    color: #ffffff;
    font-family: sans-serif;
    flex-shrink: 0;
    z-index: 100;
}

.sidebar-header {
    padding: 14px 14px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.sidebar-logo {
    display: flex;
    align-items: center;
    gap: 8px;
}

.logo-icon {
    font-size: 20px;
    background: rgba(255, 255, 255, 0.1);
    padding: 6px;
    border-radius: 6px;
}

.logo-text {
    display: flex;
    flex-direction: column;
}

.logo-title {
    font-size: 15px;
    font-weight: 700;
    color: #ffffff;
    line-height: 1.2;
}

.logo-subtitle {
    font-size: 11px;
    color: rgba(255, 255, 255, 0.6);
}

.sidebar-nav {
    flex: 1;
    padding: 6px 0;
    overflow-y: auto;
}

.nav-section {
    margin-bottom: 2px;
}

.nav-section-title {
    display: block;
    font-size: 10.5px;
    font-weight: 600;
    color: rgba(255, 255, 255, 0.4);
    letter-spacing: 0.5px;
}

.nav-section-toggle {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
    background: none;
    border: none;
    cursor: pointer;
    padding: 9px 16px 5px;
    color: inherit;
    font-family: inherit;
}

.nav-section-toggle .nav-section-title {
    padding: 0;
}

.nav-chevron {
    font-size: 10px;
    color: rgba(255, 255, 255, 0.4);
    transition: transform 0.2s;
}

.nav-chevron.open {
    transform: rotate(180deg);
}

.nav-collapsible {
    display: flex;
    flex-direction: column;
}

.nav-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 7px 16px;
    color: rgba(255, 255, 255, 0.75);
    text-decoration: none;
    font-size: 14px;
    transition: background 0.2s, color 0.2s;
}

.nav-item:hover {
    background: rgba(255, 255, 255, 0.08);
    color: #ffffff;
}

.nav-item.router-link-active {
    background: rgba(255, 255, 255, 0.12);
    color: #ffffff;
    border-left: 3px solid #4caf82;
}

.nav-icon {
    font-size: 16px;
    width: 18px;
    text-align: center;
}

.sidebar-footer {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 10px 14px;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.user-avatar {
    width: 32px;
    height: 32px;
    background: #4caf82;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    font-size: 13px;
    flex-shrink: 0;
}

.user-info {
    display: flex;
    flex-direction: column;
}

.user-name {
    font-size: 13px;
    font-weight: 600;
    color: #ffffff;
}

.user-role {
    font-size: 11px;
    color: rgba(255, 255, 255, 0.55);
}
</style>