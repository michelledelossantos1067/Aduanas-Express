<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useMantenimientos } from './composables/useMantenimientos'
import { useMantenimientoForm } from './composables/useMantenimientoForm'
import MantenimientoForm from './MantenimientoForm.vue'

const route = useRoute()
const router = useRouter()

const { registros, cargar, guardar, avisar } = useMantenimientos()
const { form, modoForm, formError, abrirCrear, abrirEditar, validar, payload } = useMantenimientoForm()

const guardando = ref(false)
const cargandoRegistro = ref(false)
const noEncontrado = ref(false)

const idEditar = computed(() => {
    const id = route.params.id
    return id ? Number(id) : null
})

function irALaLista() {
    router.push({ name: 'mantenimiento' })
}

async function inicializar() {
    if (idEditar.value) {
        cargandoRegistro.value = true
        try {
            await cargar()
            const registro = registros.value.find(r => r.id === idEditar.value)
            if (registro) {
                abrirEditar(registro)
            } else {
                noEncontrado.value = true
            }
        } finally {
            cargandoRegistro.value = false
        }
    } else {
        const vehiculoIdPre = route.query?.vehiculoId
        abrirCrear(vehiculoIdPre ? Number(vehiculoIdPre) : undefined)
    }
}

async function guardarForm() {
    const err = validar()
    if (err) {
        formError.value = err
        return
    }
    guardando.value = true
    formError.value = ''
    try {
        await guardar(modoForm.value, payload(), form.value.id)
        avisar(modoForm.value === 'crear' ? 'Mantenimiento registrado correctamente.' : 'Mantenimiento actualizado correctamente.')
        irALaLista()
    } catch (e) {
        console.error(e)
        formError.value = e?.response?.data?.message ?? 'Error al guardar el registro.'
    } finally {
        guardando.value = false
    }
}

onMounted(inicializar)
</script>

<template>
    <div class="mant-form-page">

        <div v-if="cargandoRegistro" class="estado-carga">
            <div class="spinner"></div>
            <p>Cargando registro...</p>
        </div>

        <div v-else-if="noEncontrado" class="estado-vacio">
            <p>No se encontró el registro de mantenimiento solicitado.</p>
            <button class="btn-volver-simple" @click="irALaLista">Volver a la lista</button>
        </div>

        <MantenimientoForm
            v-else
            :form="form"
            :modo="modoForm"
            :guardando="guardando"
            :error-msg="formError"
            @guardar="guardarForm"
            @cancelar="irALaLista"
        />

    </div>
</template>

<style scoped>
.mant-form-page {
    padding: 28px 32px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

.estado-carga {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 80px 0;
    color: #6b7280;
}

.estado-vacio {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 16px;
    padding: 80px 0;
    color: #6b7280;
    font-size: .9rem;
    text-align: center;
}

.btn-volver-simple {
    padding: 9px 22px;
    background: #1a3a2a;
    border: none;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}
.btn-volver-simple:hover { background: #14532d; }

.spinner {
    width: 32px; height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

@media (max-width: 700px) {
    .mant-form-page { padding: 16px; }
}
</style>