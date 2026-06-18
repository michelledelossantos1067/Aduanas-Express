<script setup>
import { useRouter } from 'vue-router'
import { ref } from 'vue'

const router = useRouter()

const step = ref('email')
const loading = ref(false)
const error = ref('')

const form = ref({
    email: '',
    password: '',
    confirmPassword: ''
})

async function handleVerifyEmail() {
    if (!form.value.email) {
        error.value = 'Ingresa tu correo electrónico'
        return
    }
    loading.value = true
    error.value = ''
    try {
        // await checkEmailExists(form.value.email)
        step.value = 'password'
    } catch (e) {
        error.value = e.response?.data?.message || 'Correo no encontrado'
    } finally {
        loading.value = false
    }
}

async function handleResetPassword() {
    if (!form.value.password || !form.value.confirmPassword) {
        error.value = 'Completa ambos campos'
        return
    }
    if (form.value.password.length < 6) {
        error.value = 'La contraseña debe tener al menos 6 caracteres'
        return
    }
    if (form.value.password !== form.value.confirmPassword) {
        error.value = 'Las contraseñas no coinciden'
        return
    }
    loading.value = true
    error.value = ''
    try {
        // await resetPassword(form.value.email, form.value.password)
        step.value = 'done'
    } catch (e) {
        error.value = e.response?.data?.message || 'Error al restablecer la contraseña'
    } finally {
        loading.value = false
    }
}
</script>

<template>
    <div class="reset-page">
        <div class="reset-card">
            <div class="reset-container">

                <div class="reset-left">
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
                                <i class="ti ti-lock-open" aria-hidden="true"></i>
                                <div>
                                    <span class="feature-title">Recuperación simple</span>
                                    <span class="feature-sub">Solo tu correo y nueva clave</span>
                                </div>
                            </li>
                            <li>
                                <i class="ti ti-shield-check" aria-hidden="true"></i>
                                <div>
                                    <span class="feature-title">Acceso protegido</span>
                                    <span class="feature-sub">Tu cuenta siempre segura</span>
                                </div>
                            </li>
                        </ul>
                    </div>
                    <p class="info">© 2025 Aduanas Express — Uso institucional exclusivo.<br>Acceso restringido a personal autorizado.</p>
                </div>

                <div class="reset-right">

                    <!-- Paso 1: Email -->
                    <template v-if="step === 'email'">
                        <h1>Recuperar contraseña</h1>
                        <h4>Ingresa tu correo para continuar</h4>
                        <div class="form-group">
                            <label class="form-label">Correo electrónico</label>
                            <input v-model="form.email" type="email" class="form-input" placeholder="correo@ejemplo.com" />
                            <p v-if="error" class="error-msg">{{ error }}</p>
                            <button @click="handleVerifyEmail" class="btn-action" :disabled="loading">
                                {{ loading ? 'Verificando...' : 'Continuar' }}
                            </button>
                            <p>¿Recordaste tu contraseña?
                                <span class="support" @click="router.push('/login')">Inicia sesión</span>
                            </p>
                        </div>
                    </template>

                    <!-- Paso 2: Nueva contraseña -->
                    <template v-if="step === 'password'">
                        <h1>Nueva contraseña</h1>
                        <h4>Elige una contraseña segura para <strong>{{ form.email }}</strong></h4>
                        <div class="form-group">
                            <label class="form-label">Nueva contraseña</label>
                            <input v-model="form.password" type="password" class="form-input" placeholder="Mínimo 6 caracteres" />
                            <label class="form-label">Confirmar contraseña</label>
                            <input v-model="form.confirmPassword" type="password" class="form-input" placeholder="Repite tu contraseña" />
                            <p v-if="error" class="error-msg">{{ error }}</p>
                            <button @click="handleResetPassword" class="btn-action" :disabled="loading">
                                {{ loading ? 'Guardando...' : 'Restablecer contraseña' }}
                            </button>
                            <p>
                                <span class="support" @click="step = 'email'">← Cambiar correo</span>
                            </p>
                        </div>
                    </template>

                    <!-- Éxito -->
                    <template v-if="step === 'done'">
                        <div class="success-state">
                            <div class="success-icon">
                                <i class="ti ti-circle-check"></i>
                            </div>
                            <h1>¡Contraseña restablecida!</h1>
                            <p>Tu contraseña ha sido actualizada correctamente. Ya puedes iniciar sesión.</p>
                            <button @click="router.push('/login')" class="btn-action">
                                Ir al inicio de sesión
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

.reset-page {
    width: 100%;
    min-height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    background: rgb(228, 228, 228);
}

.reset-card {
    width: 900px;
    display: flex;
    flex-direction: column;
}

.reset-container {
    display: flex;
    width: 900px;
    border-radius: 10px 10px 0 0;
    overflow: hidden;
    min-height: 500px;
}

.reset-left {
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

.reset-right {
    background: white;
    width: 60%;
    padding: 40px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    gap: 8px;
    min-height: 500px;
}

.reset-right h1 { margin: 0 0 4px 0; font-size: 24px; }
.reset-right h4 { margin: 0 0 8px 0; font-weight: 400; color: #555; font-size: 14px; }

.form-group { display: flex; flex-direction: column; gap: 8px; width: 100%; }

.form-label { font-size: 14px; font-weight: 600; color: #333; margin-top: 8px; }

.form-input {
    width: 100%; padding: 10px;
    border: 1px solid #ccc; border-radius: 6px;
    font-size: 14px; box-sizing: border-box;
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
    margin-left: 4px; text-decoration: underline;
}

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

@media (max-width: 768px) {
    .reset-page { background: #1a4a2e; align-items: flex-end; padding: 0; }
    .reset-card { width: 100%; }
    .reset-container { flex-direction: column; width: 100%; border-radius: 0; min-height: auto; }
    .reset-left { background: transparent; width: 100%; padding: 40px 24px 20px; display: block; }
    .feature-list, .left-divider, .left-description, .info { display: none !important; }
    .reset-right {
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