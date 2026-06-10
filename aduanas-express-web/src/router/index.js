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
      name: 'vehiculos/nuevo',
      component: () => import('../views/vehiculos/VehiculosFormView.vue')
    },
    {
      path: '/conductores',
      name: 'conductores',
      component: () => import('../views/conductor/ConductorListView.vue')
    },
    ,
    {
      path: '/conductores/nuevo',
      name: 'conductores/nuevo',
      component: () => import('../views/conductor/ConductorFormView.vue')
    },
    // {
    //   path: '/conductores/:id',
    //   name: 'ConductorDetail',
    //   component: () => import('../views/conductor/ConductorDetailView.vue')
    // },
    {
      path: '/solicitudes',
      name: 'solicitudes',
      component: () => import('../views/solicitud/SolicitudesListView.vue')
    }

    
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