import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', redirect: '/login' },
    { path: '/dashboard', name: 'dashboard', component: () => import('../views/Dashboard.vue') },
    { path: '/login', name: 'login', component: () => import('../views/auth/LoginView.vue') },
    { path: '/register', name: 'register', component: () => import('../views/auth/RegisterView.vue') },
    { path: '/reset-password', name: 'reset-password', component: () => import('../views/auth/ResetPasswordView.vue') },
    { path: '/change-password', name: 'change-password', component: () => import('../views/auth/ChangePasswordView.vue') },
    { path: '/archivados', name: 'archivados', component: () => import('../views/usuario/ArchivadosView.vue') },

    {
      path: '/roles',
      name: 'roles',
      meta: { roles: ['Administrador'] },
      component: () => import('@/views/roles/RolesView.vue'),
    },

    {
      path: '/vehiculos',
      name: 'vehiculos',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('../views/vehiculos/VehiculosListView.vue'),
    },
    {
      path: '/vehiculos/nuevo',
      name: 'vehiculosc',
      meta: { roles: ['Administrador', 'Supervisor'] },
      component: () => import('../views/vehiculos/VehiculosFormView.vue'),
    },
    {
      path: '/vehiculos/:id/editar',
      name: 'editarVehiculo',
      meta: { roles: ['Administrador', 'Supervisor'] },
      component: () => import('../views/vehiculos/VehiculosFormView.vue'),
    },

    {
      path: '/conductores',
      name: 'conductores',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('../views/conductor/ConductorListView.vue'),
    },
    {
      path: '/conductores/nuevo',
      name: 'conductores/nuevo',
      meta: { roles: ['Administrador', 'Supervisor'] },
      component: () => import('../views/conductor/ConductorFormView.vue'),
    },
    {
      path: '/conductores/:id/editar',
      name: 'editarConductores',
      meta: { roles: ['Administrador', 'Supervisor'] },
      component: () => import('../views/conductor/ConductorFormView.vue'),
    },

    {
      path: '/solicitudes',
      name: 'solicitudes',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('../views/solicitud/SolicitudesListView.vue'),
    },
    {
      path: '/solicitudes/nuevo',
      name: 'solicitudes/nuevo',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('../views/solicitud/SolicitudesFormView.vue'),
    },
    {
      path: '/solicitudes/:id/editar',
      name: 'editarSolicitudes',
      meta: { roles: ['Administrador', 'Supervisor'] },
      component: () => import('../views/solicitud/SolicitudesFormView.vue'),
    },

    {
      path: '/asignaciones',
      name: 'asignaciones',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('../views/asignaciones/AsignacionListView.vue'),
    },

    {
      path: '/agenda',
      name: 'agenda',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('../views/agenda/agedaCalendarioListView.vue'),
    },
    {
      path: '/monitoreo',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('@/views/monitoreo/monitoreoListView.vue'),
    },
    {
      path: '/historial',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('@/views/historial/historialVehiculoListView.vue'),
    },

    {
      path: '/mantenimiento',
      name: 'mantenimiento',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('@/views/mantenimiento/MantenimientoListView.vue'),
    },
    {
      path: '/mantenimiento/nuevo',
      name: 'mantenimientoNuevo',
      meta: { roles: ['Administrador', 'Supervisor'] },
      component: () => import('@/views/mantenimiento/MantenimientoFormView.vue'),
    },
    {
      path: '/mantenimiento/:id/editar',
      name: 'editarMantenimiento',
      meta: { roles: ['Administrador', 'Supervisor'] },
      component: () => import('../views/mantenimiento/MantenimientoFormView.vue'),
    },

    {
      path: '/reportes',
      name: 'reportes',
      meta: { roles: ['Administrador', 'Supervisor'] },
      component: () => import('@/views/reporte/reporteListView.vue'),
    },

    {
      path: '/usuarios',
      name: 'usuarios',
      meta: { roles: ['Administrador'] },
      component: () => import('@/views/usuario/UsuarioListView.vue'),
    },
    { 
      path: '/usuarios/nuevo',
      meta: { roles: ['Administrador'] },
      component: () => import('../views/usuario/UsuarioFormView.vue') },
    { 
      path: '/usuario/:id/editar',
      meta: { roles: ['Administrador'] },
      component: () => import('../views/usuario/UsuarioFormView.vue') },
  ],
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const rutasPublicas = ['login', 'register', 'reset-password', 'change-password']

  // Verificar si está autenticado en rutas privadas
  if (!rutasPublicas.includes(to.name) && !authStore.token) {
    return next({ name: 'login' })
  }

  // Verificar permisos por rol
  if (to.meta.roles && authStore.usuario) {
    const rolUsuario = authStore.usuario.rol
    if (!to.meta.roles.includes(rolUsuario)) {
      return next({ name: 'dashboard' })
    }
  }

  next()
})

export default router