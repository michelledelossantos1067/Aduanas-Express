import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        token: localStorage.getItem('token') || null,  // ← Lee al iniciar
        usuario: JSON.parse(localStorage.getItem('usuario')) || null  // ← También el usuario
    }),

    getters: {
        estaLogueado: (state) => state.token !== null
    },

    actions: {
        iniciarSesion(token, usuario) {
            this.token = token
            this.usuario = usuario
            localStorage.setItem('token', token)
            localStorage.setItem('usuario', JSON.stringify(usuario))  // ← Persiste usuario
        },
        cerrarSesion() {
            this.token = null
            this.usuario = null
            localStorage.removeItem('token')
            localStorage.removeItem('usuario')  // ← Limpia usuario
        }
    }
})