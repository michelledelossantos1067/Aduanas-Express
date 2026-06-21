<script setup>
import { ref, onMounted } from 'vue'
import { verVehiculos } from '@/services/vehiculoService'
import { TIPOS, ESTADOS } from './composables/useMantenimientos'

const props = defineProps({
    form:      { type: Object, required: true },
    modo:      { type: String, required: true },
    guardando: { type: Boolean, default: false },
    errorMsg:  { type: String, default: '' },
})
const emit = defineEmits(['guardar', 'cancelar'])

const vehiculos = ref([])
const cargandoVehiculos = ref(false)

async function cargarVehiculos() {
    cargandoVehiculos.value = true
    try {
        const res = await verVehiculos()
        vehiculos.value = res.data
    } catch (e) {
        console.error(e)
    } finally {
        cargandoVehiculos.value = false
    }
}

onMounted(cargarVehiculos)
</script>

<template>
    <div class="form-wrap">

        <div class="form-header">
            <button class="btn-volver" @click="emit('cancelar')">
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                    <polyline points="15 18 9 12 15 6"/>
                </svg>
                Volver a la lista
            </button>
            <h2 class="form-titulo">
                {{ modo === 'crear' ? 'Registrar mantenimiento' : 'Editar mantenimiento' }}
            </h2>
        </div>

        <div v-if="errorMsg" class="notif notif-error" style="margin-bottom:16px">{{ errorMsg }}</div>

        <div class="form-grid">

            <div class="form-section">
                <p class="form-section-title">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <rect x="1" y="3" width="15" height="13" rx="2"/>
                        <path d="M16 8h4l3 3v5h-7V8z"/>
                        <circle cx="5.5" cy="18.5" r="2.5"/>
                        <circle cx="18.5" cy="18.5" r="2.5"/>
                    </svg>
                    Vehículo
                </p>
                <div class="form-group">
                    <label class="form-label">Vehículo <span class="req">*</span></label>
                    <select v-model="form.vehiculoId" class="form-select" :disabled="cargandoVehiculos">
                        <option value="" disabled>
                            {{ cargandoVehiculos ? 'Cargando vehículos...' : 'Selecciona un vehículo' }}
                        </option>
                        <option v-for="v in vehiculos" :key="v.id" :value="v.id">
                            {{ v.matricula }} — {{ v.marca }} {{ v.modelo }}
                        </option>
                    </select>
                </div>
            </div>

            <div class="form-section">
                <p class="form-section-title">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
                    </svg>
                    Detalles del mantenimiento
                </p>
                <div class="form-row-2">
                    <div class="form-group">
                        <label class="form-label">Tipo <span class="req">*</span></label>
                        <select v-model="form.tipo" class="form-select">
                            <option v-for="t in TIPOS" :key="t">{{ t }}</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Estado</label>
                        <select v-model="form.estado" class="form-select">
                            <option v-for="e in ESTADOS" :key="e">{{ e }}</option>
                        </select>
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Descripción <span class="req">*</span></label>
                    <textarea v-model="form.descripcion" class="form-textarea" rows="3" placeholder="Describe el trabajo a realizar o realizado..." />
                </div>
            </div>

            <div class="form-section">
                <p class="form-section-title">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                        <line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/>
                        <line x1="3" y1="10" x2="21" y2="10"/>
                    </svg>
                    Fechas y kilometraje
                </p>
                <div class="form-row-3">
                    <div class="form-group">
                        <label class="form-label">Fecha programada <span class="req">*</span></label>
                        <input v-model="form.fechaProgramada" type="date" class="form-input" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Fecha realizada</label>
                        <input v-model="form.fechaRealizada" type="date" class="form-input" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Kilometraje actual</label>
                        <input v-model="form.kilometraje" type="number" class="form-input" placeholder="km" min="0" />
                    </div>
                </div>
            </div>

            <div class="form-section">
                <p class="form-section-title">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <line x1="12" y1="1" x2="12" y2="23"/>
                        <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                    </svg>
                    Taller y costo
                </p>
                <div class="form-row-3">
                    <div class="form-group">
                        <label class="form-label">Taller / Proveedor</label>
                        <input v-model="form.taller" type="text" class="form-input" placeholder="Nombre del taller" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Responsable</label>
                        <input v-model="form.responsable" type="text" class="form-input" placeholder="Nombre del responsable" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Costo (DOP)</label>
                        <input v-model="form.costo" type="number" class="form-input" placeholder="0.00" min="0" step="0.01" />
                    </div>
                </div>
            </div>

            <div class="form-section">
                <p class="form-section-title">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                        <polyline points="14 2 14 8 20 8"/>
                        <line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/>
                    </svg>
                    Observaciones
                </p>
                <div class="form-group">
                    <textarea v-model="form.observaciones" class="form-textarea" rows="3" placeholder="Notas adicionales, piezas cambiadas..." />
                </div>
            </div>

        </div>

        <div class="form-acciones">
            <button class="btn-cancelar" @click="emit('cancelar')">Cancelar</button>
            <button class="btn-guardar" :disabled="guardando" @click="emit('guardar')">
                <div v-if="guardando" class="spinner-btn"></div>
                <svg v-else width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                    <polyline points="20 6 9 17 4 12"/>
                </svg>
                {{ guardando ? 'Guardando...' : (modo === 'crear' ? 'Registrar mantenimiento' : 'Guardar cambios') }}
            </button>
        </div>
    </div>
</template>

<style scoped>
.form-wrap {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    overflow: hidden;
}

.form-header {
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    gap: 14px;
    padding: 18px 24px;
    border-bottom: 1px solid #f3f4f6;
}

.btn-volver {
    display: inline-flex;
    align-items: center;
    gap: 5px;
    padding: 6px 14px;
    background: transparent;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: .82rem;
    font-weight: 600;
    color: #374151;
    cursor: pointer;
    transition: all .15s;
    flex-shrink: 0;
}
.btn-volver:hover { background: #f3f4f6; }

.form-titulo {
    font-size: 1rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.form-grid {
    padding: 20px 24px;
    display: flex;
    flex-direction: column;
    gap: 0;
}

.form-section {
    padding: 18px 0;
    border-bottom: 1px solid #f3f4f6;
}
.form-section:last-child { border-bottom: none; }

.form-section-title {
    display: flex;
    align-items: center;
    gap: 7px;
    font-size: .72rem;
    font-weight: 700;
    color: #6b7280;
    text-transform: uppercase;
    letter-spacing: .07em;
    margin: 0 0 14px;
}

.form-row-2 {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 14px;
}

.form-row-3 {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 14px;
}

.form-group {
    display: flex;
    flex-direction: column;
    gap: 5px;
    margin-bottom: 12px;
}
.form-group:last-child { margin-bottom: 0; }

.form-label {
    font-size: .78rem;
    font-weight: 600;
    color: #374151;
}

.req { color: #dc2626; }

.form-input,
.form-select,
.form-textarea {
    padding: 9px 12px;
    background: #f9fafb;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: .875rem;
    color: #111827;
    font-family: inherit;
    outline: none;
    transition: border-color .15s;
}

.form-input:focus,
.form-select:focus,
.form-textarea:focus {
    border-color: #1a3a2a;
    background: #fff;
}

.form-textarea { resize: vertical; min-height: 72px; }

.form-acciones {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    padding: 18px 24px;
    border-top: 1px solid #f3f4f6;
    background: #fafafa;
}

.btn-cancelar {
    padding: 10px 22px;
    background: transparent;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 500;
    color: #6b7280;
    cursor: pointer;
    transition: background .15s;
}
.btn-cancelar:hover { background: #f3f4f6; }

.btn-guardar {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 10px 24px;
    background: #1a3a2a;
    border: none;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 700;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}
.btn-guardar:hover:not(:disabled) { background: #14532d; }
.btn-guardar:disabled { opacity: .5; cursor: default; }

.notif {
    padding: 12px 18px;
    border-radius: 10px;
    font-size: .875rem;
    font-weight: 500;
}
.notif-error { background: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

.spinner-btn {
    width: 15px; height: 15px;
    border: 2px solid rgba(255,255,255,.4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

@media (max-width: 1000px) {
    .form-row-3 { grid-template-columns: 1fr 1fr; }
}

@media (max-width: 700px) {
    .form-row-2,
    .form-row-3 { grid-template-columns: 1fr; }

    .form-wrap    { border-radius: 10px; }
    .form-header  { padding: 14px 16px; }
    .form-titulo  { font-size: .92rem; }
    .form-grid    { padding: 16px; }
    .form-section { padding: 14px 0; }

    .form-acciones {
        flex-direction: column-reverse;
        padding: 14px 16px;
    }
    .btn-cancelar,
    .btn-guardar {
        width: 100%;
        display: flex;
        justify-content: center;
    }
}
</style>