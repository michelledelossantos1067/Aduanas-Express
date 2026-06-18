import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login'
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/auth/LoginView.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/auth/RegisterView.vue')
    },
    {
      path: '/reset-password',
      name: 'reset-password',
      component: () => import('../views/auth/ResetPasswordView.vue')
    },
    {
      path: '/change-password',
      name: 'change-password',
      component: () => import('../views/auth/ChangePasswordView.vue')
    },
    {
      path: '/vehiculos',
      name: 'vehiculos',
      component: () => import('../views/vehiculos/VehiculosListView.vue')
    },
    {
      path: '/vehiculos/nuevo',
      name: 'vehiculosc',
      component: () => import('../views/vehiculos/VehiculosFormView.vue')
    },
    { 
      path: '/vehiculos/:id/editar', 
      name: 'editarVehiculo', 
      component:() => import('../views/vehiculos/VehiculosFormView.vue') 
    },
    {
      path: '/conductores',
      name: 'conductores',
      component: () => import('../views/conductor/ConductorListView.vue')
    },
    {
      path: '/conductores/nuevo',
      name: 'conductores/nuevo',
      component: () => import('../views/conductor/ConductorFormView.vue')
    },
    { 
      path: '/conductores/:id/editar', 
      name: 'editarConductores', 
      component:() => import('../views/conductor/ConductorFormView.vue') 
    },
    {
      path: '/solicitudes',
      name: 'solicitudes',
      component: () => import('../views/solicitud/SolicitudesListView.vue')
    },
    { 
      path: '/solicitudes/nuevo', 
      name: 'solicitudes/nuevo', 
      component:() => import('../views/solicitud/SolicitudesFormView.vue') 
    },
    { 
      path: '/solicitudes/:id/editar', 
      name: 'editarSolicitudes', 
      component:() => import('../views/solicitud/SolicitudesFormView.vue') 
    },
    {
      path: '/asignaciones',
      name: 'asignaciones',
      component: () => import('../views/asignaciones/AsignacionListView.vue')
    },
    { 
      path: '/agenda', 
      name: 'agenda', 
      component:() => import('../views/agenda/agedaCalendarioListView.vue') 
    },
    { 
      path: '/mantenimiento', 
      component: () => import('@/views/mantenimiento/mantenimientoListView.vue') 
    },
    { 
      path: '/reportes', 
      component: () => import('@/views/reporte/reporteListView.vue') 
    }
    ,
    { 
      path: '/monitoreo', 
      component: () => import('@/views/monitoreo/monitoreoListView.vue') 
    } 
    ,
    { 
      path: '/historial', 
      component: () => import('@/views/historial/historialVehiculoListView.vue') 
    },{ 
      path: '/usuarios', 
      component: () => import('@/views/usuario/UsuarioListView.vue') 
    },
    
  ],
})

// navigation guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const rutasPublicas = ['login', 'register', 'reset-password','change-password']
  
  if (!rutasPublicas.includes(to.name) && !authStore.token) {
    next({ name: 'login' })
  } else {
    next()
  }
})

export default router