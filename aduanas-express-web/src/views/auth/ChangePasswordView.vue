<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'
import { ref } from 'vue'

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const error = ref('')
const success = ref(false)

const form = ref({
    email: authStore.usuario?.email || '',
    passwordActual: '',
    passwordNueva: '',
    confirmarNueva: ''
})

async function handleChangePassword() {
    if (!form.value.email || !form.value.passwordActual || !form.value.passwordNueva || !form.value.confirmarNueva) {
        error.value = 'Completa todos los campos'
        return
    }
    if (form.value.passwordNueva.length < 6) {
        error.value = 'La nueva contraseña debe tener al menos 6 caracteres'
        return
    }
    if (form.value.passwordNueva !== form.value.confirmarNueva) {
        error.value = 'Las contraseñas nuevas no coinciden'
        return
    }
    if (form.value.passwordActual === form.value.passwordNueva) {
        error.value = 'La nueva contraseña debe ser diferente a la actual'
        return
    }

    loading.value = true
    error.value = ''

    try {
        // await changePassword({
        //     email: form.value.email,
        //     passwordActual: form.value.passwordActual,
        //     passwordNueva: form.value.passwordNueva
        // })
        success.value = true
    } catch (e) {
        error.value = e.response?.data?.message || 'Error al cambiar la contraseña'
    } finally {
        loading.value = false
    }
}
</script>

<template>
    <div class="change-page">
        <div class="change-card">
            <div class="change-container">

                <!-- Panel izquierdo -->
                <div class="change-left">
                    <div>
                        <div class="brand">
                            <div class="brand-icon">
                                <i class="ti ti-truck-delivery"></i>
                            </div>
                            <div>
                                <h2>Aduanas Express</h2>
                                <h4>Sistema de transporte</h4>
                            </div>
                        </div>
                        <div class="left-divider"></div>
                        <p class="left-description">
                            Plataforma institucional para la gestión del transporte de trabajadores de aduanas.
                        </p>
                        <ul class="feature-list">
                            <li>
                                <i class="ti ti-lock" aria-hidden="true"></i>
                                <div>
                                    <span class="feature-title">Seguridad de cuenta</span>
                                    <span class="feature-sub">Actualiza tu contraseña regularmente</span>
                                </div>
                            </li>
                            <li>
                                <i class="ti ti-shield-check" aria-hidden="true"></i>
                                <div>
                                    <span class="feature-title">Acceso protegido</span>
                                    <span class="feature-sub">Solo tú controlas tu cuenta</span>
                                </div>
                            </li>
                        </ul>
                    </div>
                    <p class="info">© 2025 Aduanas Express — Uso institucional exclusivo.<br>Acceso restringido a personal autorizado.</p>
                </div>

                <!-- Panel derecho -->
                <div class="change-right">

                    <!-- Formulario -->
                    <template v-if="!success">
                        <h1>Cambiar contraseña</h1>
                        <h4>Verifica tu identidad e ingresa tu nueva contraseña</h4>

                        <div class="form-group">
                            <label class="form-label">Correo electrónico</label>
                            <input
                                v-model="form.email"
                                type="email"
                                class="form-input"
                                placeholder="correo@ejemplo.com"
                                :disabled="!!authStore.usuario?.email"
                            />

                            <label class="form-label">Contraseña actual</label>
                            <input
                                v-model="form.passwordActual"
                                type="password"
                                class="form-input"
                                placeholder="Tu contraseña actual"
                            />

                            <label class="form-label">Nueva contraseña</label>
                            <input
                                v-model="form.passwordNueva"
                                type="password"
                                class="form-input"
                                placeholder="Mínimo 6 caracteres"
                            />

                            <label class="form-label">Confirmar nueva contraseña</label>
                            <input
                                v-model="form.confirmarNueva"
                                type="password"
                                class="form-input"
                                placeholder="Repite tu nueva contraseña"
                            />

                            <p v-if="error" class="error-msg">{{ error }}</p>

                            <button @click="handleChangePassword" class="btn-action" :disabled="loading">
                                {{ loading ? 'Guardando...' : 'Cambiar contraseña' }}
                            </button>

                            <p>
                                <span class="support" @click="router.back()">← Volver</span>
                            </p>
                        </div>
                    </template>

                    <!-- Éxito -->
                    <template v-else>
                        <div class="success-state">
                            <div class="success-icon">
                                <i class="ti ti-circle-check"></i>
                            </div>
                            <h1>¡Contraseña actualizada!</h1>
                            <p>Tu contraseña ha sido cambiada correctamente.</p>
                            <button @click="router.push('/dashboard')" class="btn-action">
                                Ir al dashboard
                            </button>
                        </div>
                    </template>

                    <p class="mobile-footer">Aduanas Express — Uso institucional exclusivo © 2026</p>
                </div>
            </div>

            <div class="form-footer">
                <span>Aduanas Express</span>
                <span class="dot">•</span>
                <span>Sistema Institucional de Transporte</span>
                <span class="dot">•</span>
                <span>Acceso restringido</span>
            </div>
        </div>
    </div>
</template>

<style scoped>
@import url('https://cdn.jsdelivr.net/npm/@tabler/icons-webfont@latest/tabler-icons.min.css');

.change-page {
    width: 100%;
    min-height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    background: rgb(228, 228, 228);
}

.change-card {
    width: 900px;
    display: flex;
    flex-direction: column;
}

.change-container {
    display: flex;
    width: 900px;
    border-radius: 10px 10px 0 0;
    overflow: hidden;
    min-height: 500px;
}

/* ── Panel izquierdo ── */
.change-left {
    background: #1a4a2e;
    display: flex;
    width: 40%;
    padding: 40px;
    flex-direction: column;
    justify-content: space-between;
    color: white;
}

.brand { display: flex; align-items: center; gap: 10px; }

.brand-icon {
    width: 36px; height: 36px;
    background: rgba(255,255,255,0.15);
    border-radius: 8px;
    display: flex; align-items: center; justify-content: center;
    font-size: 20px; flex-shrink: 0;
}

.brand h2 { margin: 0; font-size: 16px; }
.brand h4 { margin: 0; font-weight: 400; font-size: 12px; opacity: 0.6; }

.left-divider {
    height: 0.5px;
    background: rgba(255,255,255,0.15);
    margin: 24px 0 16px;
}

.left-description {
    font-size: 13px;
    color: rgba(255,255,255,0.75);
    line-height: 1.6;
    margin: 0 0 20px;
}

.feature-list {
    list-style: none; padding: 0; margin: 0;
    display: flex; flex-direction: column; gap: 10px;
}

.feature-list li {
    display: flex; align-items: flex-start; gap: 12px;
    background: rgba(255,255,255,0.07);
    border-radius: 8px; padding: 12px;
}

.feature-list li i { font-size: 18px; color: rgba(255,255,255,0.8); margin-top: 1px; flex-shrink: 0; }
.feature-list li div { display: flex; flex-direction: column; gap: 2px; }
.feature-title { font-size: 13px; font-weight: 500; color: white; }
.feature-sub { font-size: 12px; color: rgba(255,255,255,0.55); }

.info {
    font-size: 11px;
    color: rgba(255,255,255,0.4);
    line-height: 1.5;
    margin: 24px 0 0;
}

/* ── Panel derecho ── */
.change-right {
    background: white;
    width: 60%;
    padding: 40px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    gap: 8px;
    min-height: 500px;
}

.change-right h1 { margin: 0 0 4px 0; font-size: 24px; }
.change-right h4 { margin: 0 0 8px 0; font-weight: 400; color: #555; font-size: 14px; }

.form-group { display: flex; flex-direction: column; gap: 8px; width: 100%; }

.form-label { font-size: 14px; font-weight: 600; color: #333; margin-top: 8px; }

.form-input {
    width: 100%; padding: 10px;
    border: 1px solid #ccc; border-radius: 6px;
    font-size: 14px; box-sizing: border-box;
    background: white;
}

.form-input:disabled {
    background: #f5f5f5;
    color: #999;
    cursor: not-allowed;
}

.form-input:focus {
    outline: none;
    border-color: #1a4a2e;
    box-shadow: 0 0 0 2px rgba(26,74,46,0.15);
}

.btn-action {
    width: 100%; padding: 12px;
    background: #1a4a2e; color: white;
    border: none; border-radius: 6px;
    font-size: 15px; font-weight: bold;
    cursor: pointer; margin-top: 8px;
    transition: background 0.2s;
}

.btn-action:hover { background: #255c3a; }
.btn-action:disabled { background: #7a9e86; cursor: not-allowed; }

.error-msg { color: #c0392b; font-size: 13px; margin: 0; }

.support {
    color: #1a4a2e; cursor: pointer;
    text-decoration: underline;
}

/* ── Éxito ── */
.success-state {
    display: flex; flex-direction: column;
    align-items: center; text-align: center;
    gap: 12px; padding: 20px 0;
}

.success-icon {
    width: 72px; height: 72px;
    background: #e8f5ee; border-radius: 50%;
    display: flex; align-items: center; justify-content: center;
    font-size: 36px; color: #1a4a2e;
}

.success-state h1 { margin: 0; }
.success-state p { color: #555; font-size: 14px; max-width: 300px; line-height: 1.6; }
.success-state .btn-action { width: auto; padding: 12px 32px; }

/* ── Footer ── */
.form-footer {
    width: 900px; margin-top: 0;
    padding: 14px 24px; background: white;
    border-top: 1px solid #dcdcdc;
    border-radius: 0 0 10px 10px;
    display: flex; justify-content: center;
    align-items: center; gap: 12px;
    font-size: 12px; color: #666;
    box-sizing: border-box;
}

.dot { color: #1a4a2e; font-weight: bold; }

.mobile-footer { display: none; }

/* ── Responsive ── */
@media (max-width: 768px) {
    .change-page { background: #1a4a2e; align-items: flex-end; padding: 0; }
    .change-card { width: 100%; }
    .change-container { flex-direction: column; width: 100%; border-radius: 0; min-height: auto; }
    .change-left { background: transparent; width: 100%; padding: 40px 24px 20px; display: block; }
    .feature-list, .left-divider, .left-description, .info { display: none !important; }
    .change-right {
        background: white; width: 100%;
        border-radius: 24px 24px 0 0;
        min-height: 70vh; padding: 30px 24px;
        justify-content: flex-start;
    }
    .form-footer { display: none; }
    .mobile-footer { display: block; font-size: 11px; color: #999; text-align: center; margin-top: 10px; }
    .form-input, .btn-action { width: 85%; }
}
</style>