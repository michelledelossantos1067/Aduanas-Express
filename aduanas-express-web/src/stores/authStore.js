import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        token: localStorage.getItem('token') || null,
        usuario: JSON.parse(localStorage.getItem('usuario')) || null,
        permisos: JSON.parse(localStorage.getItem('permisos')) || {}
    }),

    getters: {
        estaLogueado: (state) => state.token !== null,
        tienePermiso: (state) => (modulo, accion) => {
            return state.permisos[modulo]?.[accion] === true
        }
    },

    actions: {
        async iniciarSesion(token, usuario) {
            this.token = token
            this.usuario = usuario
            localStorage.setItem('token', token)
            localStorage.setItem('usuario', JSON.stringify(usuario))
            console.log('Usuario guardado:', this.usuario) 
            await this.cargarPermisos()
        },

        async cargarPermisos() {
            try {
                const { obtenerRoles } = await import('@/services/rolService')
                const { data } = await obtenerRoles()
                console.log('data completa:', JSON.stringify(data)) 
                console.log('Usuario en store:', this.usuario)        // ← ¿tiene rolId?
console.log('rolId buscado:', this.usuario?.rolId)    // ← ¿qué valor tiene?
console.log('Roles disponibles:', data.map(r => r.nombre)) // ← ¿coincide?

                const rol = data.find(r => r.nombre === this.usuario?.rolId)
                console.log('Resultado find:', rol)
console.log('Comparando:', data.map(r => `"${r.nombre}" === "${this.usuario?.rolId}" → ${r.nombre === this.usuario?.rolId}`))
                if (!rol) return

                const obj = {}
                for (const p of rol.permisos) {
                    if (!obj[p.modulo]) obj[p.modulo] = {}
                    obj[p.modulo][p.accion] = p.permitido
                }
                this.permisos = obj
                localStorage.setItem('permisos', JSON.stringify(obj))
            } catch (e) {
                console.error('Error cargando permisos:', e)
                this.permisos = {}
            }
        },

        cerrarSesion() {
            this.token = null
            this.usuario = null
            this.permisos = {}
            localStorage.removeItem('token')
            localStorage.removeItem('usuario')
            localStorage.removeItem('permisos')
        },


        logout() {
            this.cerrarSesion()
        }
    }
})