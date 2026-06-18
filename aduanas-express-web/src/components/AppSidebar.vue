<script setup>
import { computed } from 'vue'
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
            <!-- PRINCIPAL -->
            <div class="nav-section">
                <span class="nav-section-title">PRINCIPAL</span>
                <router-link to="/dashboard" class="nav-item">
                    <span class="nav-icon">⊞</span>
                    <span>Dashboard</span>
                </router-link>
            </div>

            <!-- GESTIÓN -->
            <div class="nav-section">
                <span class="nav-section-title">GESTIÓN</span>
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

            <!-- OPERACIÓN -->
            <div class="nav-section">
                <span class="nav-section-title">OPERACIÓN</span>
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

            <!-- ADMINISTRACIÓN -->
            <div class="nav-section">
                <span class="nav-section-title">ADMINISTRACIÓN</span>
                <router-link to="/usuarios" class="nav-item">
                    <span class="nav-icon">👥</span>
                    <span>Usuarios</span>
                </router-link>
                <router-link to="/roles" class="nav-item">
                    <span class="nav-icon">⚙️</span>
                    <span>Roles</span>
                </router-link>
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
    width: 240px;
    min-height: 100vh;
    background-color: #1a3a2e;
    display: flex;
    flex-direction: column;
    color: #ffffff;
    font-family: sans-serif;
}

.sidebar-header {
    padding: 20px 16px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.sidebar-logo {
    display: flex;
    align-items: center;
    gap: 12px;
}

.logo-icon {
    font-size: 28px;
    background: rgba(255, 255, 255, 0.1);
    padding: 8px;
    border-radius: 8px;
}

.logo-text {
    display: flex;
    flex-direction: column;
}

.logo-title {
    font-size: 16px;
    font-weight: 700;
    color: #ffffff;
}

.logo-subtitle {
    font-size: 11px;
    color: rgba(255, 255, 255, 0.6);
}

.sidebar-nav {
    flex: 1;
    padding: 12px 0;
    overflow-y: auto;
}

.nav-section {
    margin-bottom: 8px;
}

.nav-section-title {
    display: block;
    font-size: 10px;
    font-weight: 600;
    color: rgba(255, 255, 255, 0.4);
    letter-spacing: 1px;
    padding: 10px 16px 4px;
}

.nav-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 9px 16px;
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
    width: 20px;
    text-align: center;
}

.sidebar-footer {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 16px;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.user-avatar {
    width: 36px;
    height: 36px;
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