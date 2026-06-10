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
    }
  ],
})

// navigation guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const rutasPublicas = ['login', 'register', 'reset-password']
  
  if (!rutasPublicas.includes(to.name) && !authStore.token) {
    next({ name: 'login' })
  } else {
    next()
  }
})

export default router