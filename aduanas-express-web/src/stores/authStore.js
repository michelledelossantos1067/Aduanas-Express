import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        token: null,
        usuario: null
    }),

    getters: {
        estaLogueado: (state) => state.token !== null
    },

    actions: {
        iniciarSesion(token, usuario) {
            this.token = token
            this.usuario = usuario
            localStorage.setItem('token', token)
        },
        cerrarSesion() {
            this.token = null
            this.usuario = null
            localStorage.removeItem('token')
        }
    }
})