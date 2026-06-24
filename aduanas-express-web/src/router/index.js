import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', redirect: '/login' },
    { path: '/login', name: 'login', component: () => import('../views/auth/LoginView.vue') },
    { path: '/register', name: 'register', component: () => import('../views/auth/RegisterView.vue') },
    { path: '/reset-password', name: 'reset-password', component: () => import('../views/auth/ResetPasswordView.vue') },
    { path: '/change-password', name: 'change-password', component: () => import('../views/auth/ChangePasswordView.vue') },

    { path: '/dashboard', name: 'dashboard', component: () => import('../views/Dashboard.vue') },

    {
      path: '/archivados',
      name: 'archivados',
      meta: { roles: ['Administrador'] },
      component: () => import('../views/ArchivadosView.vue')
    },

    {
      path: '/roles',
      name: 'roles',
      meta: { roles: ['Administrador'] },
      component: () => import('@/views/roles/RolesView.vue'),
    },

    {
      path: '/vehiculos',
      name: 'vehiculos',
      meta: {
        roles: ['Administrador', 'Supervisor', 'Operador'],
        permiso: ['vehiculos', 'ver'],
      },
      component: () => import('../views/vehiculos/VehiculosListView.vue'),
    },
    {
      path: '/vehiculos/nuevo',
      name: 'vehiculosc',
      meta: {
        roles: ['Administrador', 'Supervisor'],
        permiso: ['vehiculos', 'crear'],
      },
      component: () => import('../views/vehiculos/VehiculosFormView.vue'),
    },
    {
      path: '/vehiculos/:id/editar',
      name: 'editarVehiculo',
      meta: {
        roles: ['Administrador', 'Supervisor'],
        permiso: ['vehiculos', 'editar'],
      },
      component: () => import('../views/vehiculos/VehiculosFormView.vue'),
    },

    {
      path: '/conductores',
      name: 'conductores',
      meta: {
        roles: ['Administrador', 'Supervisor', 'Operador'],
        permiso: ['conductores', 'ver'],
      },
      component: () => import('../views/conductor/ConductorListView.vue'),
    },
    {
      path: '/conductores/nuevo',
      name: 'conductores/nuevo',
      meta: {
        roles: ['Administrador', 'Supervisor'],
        permiso: ['conductores', 'crear'],
      },
      component: () => import('../views/conductor/ConductorFormView.vue'),
    },
    {
      path: '/conductores/:id/editar',
      name: 'editarConductores',
      meta: {
        roles: ['Administrador', 'Supervisor'],
        permiso: ['conductores', 'editar'],
      },
      component: () => import('../views/conductor/ConductorFormView.vue'),
    },

    {
      path: '/solicitudes',
      name: 'solicitudes',
      meta: {
        roles: ['Administrador', 'Supervisor', 'Operador'],
        permiso: ['solicitudes', 'ver'],
      },
      component: () => import('../views/solicitud/SolicitudesListView.vue'),
    },
    {
      path: '/solicitudes/nuevo',
      name: 'solicitudes/nuevo',
      meta: {
        roles: ['Administrador', 'Supervisor', 'Operador'],
        permiso: ['solicitudes', 'crear'],
      },
      component: () => import('../views/solicitud/SolicitudesFormView.vue'),
    },
    {
      path: '/solicitudes/:id/editar',
      name: 'editarSolicitudes',
      meta: {
        roles: ['Administrador', 'Supervisor'],
        permiso: ['solicitudes', 'editar'],
      },
      component: () => import('../views/solicitud/SolicitudesFormView.vue'),
    },

    {
      path: '/asignaciones',
      name: 'asignaciones',
      meta: {
        roles: ['Administrador', 'Supervisor', 'Operador'],
        permiso: ['asignaciones', 'ver'],
      },
      component: () => import('../views/asignaciones/AsignacionListView.vue'),
    },
    {
      path: '/asignaciones/nuevo',
      name: 'asignaciones/nuevo',
      meta: {
        roles: ['Administrador', 'Supervisor'],
        permiso: ['asignaciones', 'asignar'],
      },
      component: () => import('../views/asignaciones/AsignacionFormView.vue'),
    },
    {
      path: '/asignaciones/:id/editar',
      name: 'editarAsignaciones',
      meta: {
        roles: ['Administrador', 'Supervisor'],
        permiso: ['asignaciones', 'editar'],
      },
      component: () => import('../views/asignaciones/AsignacionFormView.vue'),
    },

    {
      path: '/agenda',
      name: 'agenda',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('../views/agenda/agedaCalendarioListView.vue'),
    },

    {
      path: '/monitoreo',
      name: 'monitoreo',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('@/views/monitoreo/monitoreoListView.vue'),
    },

   

    {
      path: '/mantenimiento',
      name: 'mantenimiento',
      meta: { roles: ['Administrador', 'Supervisor', 'Operador'] },
      component: () => import('@/views/mantenimiento/mantenimientoListView.vue'),
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
      meta: {
        roles: ['Administrador', 'Supervisor'],
        permiso: ['reportes', 'ver'],
      },
      component: () => import('@/views/reporte/reporteListView.vue'),
    },

    {
      path: '/usuarios',
      name: 'usuarios',
      meta: {
        roles: ['Administrador'],
        permiso: ['usuarios', 'ver'],
      },
      component: () => import('@/views/usuario/UsuarioListView.vue'),
    },
    {
      path: '/usuarios/nuevo',
      name: 'usuariosNuevo',
      meta: {
        roles: ['Administrador'],
        permiso: ['usuarios', 'crear'],
      },
      component: () => import('../views/usuario/UsuarioFormView.vue'),
    },
    {
      path: '/usuario/:id/editar',
      name: 'editarUsuario',
      meta: {
        roles: ['Administrador'],
        permiso: ['usuarios', 'editar'],
      },
      component: () => import('../views/usuario/UsuarioFormView.vue'),
    },
  ],
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const rutasPublicas = ['login', 'register', 'reset-password', 'change-password']

  if (!rutasPublicas.includes(to.name) && !authStore.token) {
    return next({ name: 'login' })
  }

  if (to.meta.roles && authStore.usuario) {
    const rolUsuario = authStore.usuario.rol
    if (!to.meta.roles.includes(rolUsuario)) {
      return next({ name: 'dashboard' })
    }
  }

  if (to.meta.permiso && authStore.usuario) {
    const [modulo, accion] = to.meta.permiso
    if (Object.keys(authStore.permisos).length > 0) {
        if (!authStore.tienePermiso(modulo, accion)) {
            return next({ name: 'dashboard' })
        }
    }
}

  next()
})

export default router