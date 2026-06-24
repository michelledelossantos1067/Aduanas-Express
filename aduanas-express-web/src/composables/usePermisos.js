import { computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'

export function usePermisos() {
    const auth = useAuthStore()
    const rol = computed(() => auth.usuario?.rol?.toLowerCase())

    const esAdmin = computed(() => rol.value === 'administrador')
    const esSupervisor = computed(() => rol.value === 'supervisor')
    const esOperador = computed(() => rol.value === 'operador')

    // Permisos específicos por módulo
    const puede = {
        // Usuarios
        verUsuarios: computed(() => esAdmin.value),
        crearUsuarios: computed(() => esAdmin.value),
        editarUsuarios: computed(() => esAdmin.value),
        eliminarUsuarios: computed(() => esAdmin.value),

        // Vehículos
        verVehiculos: computed(() => esAdmin.value || esSupervisor.value || esOperador.value),
        crearVehiculos: computed(() => esAdmin.value || esSupervisor.value),
        editarVehiculos: computed(() => esAdmin.value || esSupervisor.value),
        eliminarVehiculos: computed(() => esAdmin.value),

        // Conductores
        verConductores: computed(() => true),
        crearConductores: computed(() => esAdmin.value || esSupervisor.value),
        editarConductores: computed(() => esAdmin.value || esSupervisor.value),
        eliminarConductores: computed(() => esAdmin.value),
        
        // Consume-combistible
        verConsumoCombustible: computed(() => true),
        crearCombustible: computed(() => esAdmin.value || esSupervisor.value),
        editarCombustible: computed(() => esAdmin.value || esSupervisor.value),

        // Solicitudes
        verSolicitudes: computed(() => true),
        crearSolicitudes: computed(() => true),
        editarSolicitudes: computed(() => esAdmin.value || esSupervisor.value),
        eliminarSolicitudes: computed(() => esAdmin.value),

        // Asignaciones
        verAsignaciones: computed(() => true),
        gestionarAsignaciones: computed(() => esAdmin.value || esSupervisor.value),

        // Mantenimiento
        verMantenimiento: computed(() => true),
        gestionarMantenimiento: computed(() => esAdmin.value || esSupervisor.value),

        // Reportes
        verReportes: computed(() => esAdmin.value || esSupervisor.value),
        exportarReportes: computed(() => esAdmin.value || esSupervisor.value),
        verEstadisticas: computed(() => esAdmin.value),

        // Roles
        verRoles: computed(() => esAdmin.value),
    }

    return { esAdmin, esSupervisor, esOperador, puede }
}