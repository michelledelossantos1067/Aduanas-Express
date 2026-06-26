<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'
import { register } from '../../services/authService'
import { ref } from 'vue'

const authStore = useAuthStore()
const router = useRouter()

const error = ref("")
const loading = ref(false)
const form = ref({ nombre: '', apellido: '', email: '', password: '' })

async function handleRegister() {
    if (!form.value.nombre || !form.value.apellido || !form.value.email || !form.value.password) {
        error.value = "Por favor completa todos los campos"
        return
    }
    if (form.value.password.length < 6) {
        error.value = "La contraseña debe tener al menos 6 caracteres"
        return
    }

    loading.value = true
    error.value = ""

    try {
        const response = await register(form.value)
        authStore.iniciarSesion(response.data.token, {
            nombre: response.data.nombre,
            rolId: response.data.rolId
        })
        router.push('/dashboard')
    } catch (e) {
        error.value = e.response?.data?.message || "Error al registrarse. Intenta de nuevo."
    } finally {
        loading.value = false
    }
}
</script>

<template>
    <div class="register-page">
        <div class="register-card">
        <div class="register-container">

            <div class="register-left">
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
                            <i class="ti ti-route"></i>
                            <div>
                                <span class="feature-title">Rutas y horarios</span>
                                <span class="feature-sub">Gestión completa de recorridos</span>
                            </div>
                        </li>
                        <li>
                            <i class="ti ti-users"></i>
                            <div>
                                <span class="feature-title">Unidades y personal</span>
                                <span class="feature-sub">Asignación y control de recursos</span>
                            </div>
                        </li>
                        <li>
                            <i class="ti ti-map-pin"></i>
                            <div>
                                <span class="feature-title">Trazabilidad en tiempo real</span>
                                <span class="feature-sub">Monitoreo y tiempos de servicio</span>
                            </div>
                        </li>
                        <li>
                            <i class="ti ti-chart-bar"></i>
                            <div>
                                <span class="feature-title">Reportes y auditoría</span>
                                <span class="feature-sub">Historial y control de servicios</span>
                            </div>
                        </li>
                    </ul>
                </div>

                <p class="info">© 2025 Aduanas Express — Uso institucional exclusivo.<br>Acceso restringido a personal
                    autorizado.</p>
            </div>

            <div class="register-right">
                <h1>Crear cuenta</h1>
                <h4>Completa los datos para registrarte</h4>

                <div class="form-group">
                    <label class="form-label">Nombre</label>
                    <input v-model="form.nombre" type="text" class="form-input" placeholder="Tu nombre" />

                    <label class="form-label">Apellido</label>
                    <input v-model="form.apellido" type="text" class="form-input" placeholder="Tu apellido" />

                    <label class="form-label">Correo electrónico</label>
                    <input v-model="form.email" type="email" class="form-input" placeholder="correo@ejemplo.com" />

                    <label class="form-label">Contraseña</label>
                    <input v-model="form.password" type="password" class="form-input"
                        placeholder="Mínimo 6 caracteres" />

                    <p v-if="error" class="error-msg">{{ error }}</p>

                    <button @click="handleRegister" class="btn-register" :disabled="loading">
                        {{ loading ? 'Registrando...' : 'Crear cuenta' }}
                    </button>

                    <p>¿Ya tienes cuenta?
                        <span class="support" @click="router.push('/login')">Inicia sesión</span>
                    </p>
                    <p class="mobile-footer">Aduanas Express Uso institucional exclusivo © 2026</p>
                </div>
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

.brand {
    display: flex;
    align-items: center;
    gap: 10px;
}

.brand-icon {
    width: 36px;
    height: 36px;
    background: rgba(255, 255, 255, 0.15);
    border-radius: 8px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 20px;
    flex-shrink: 0;
}

.left-divider {
    height: 0.5px;
    background: rgba(255, 255, 255, 0.15);
    margin: 24px 0 16px;
}

.left-description {
    font-size: 13px;
    color: rgba(255, 255, 255, 0.75);
    line-height: 1.6;
    margin: 0 0 20px;
}

.feature-list {
    list-style: none;
    padding: 0;
    margin: 0;
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.feature-list li {
    display: flex;
    align-items: flex-start;
    gap: 12px;
    background: rgba(255, 255, 255, 0.07);
    border-radius: 8px;
    padding: 12px;
}

.feature-list li i {
    font-size: 18px;
    color: rgba(255, 255, 255, 0.8);
    margin-top: 1px;
    flex-shrink: 0;
}

.feature-list li div {
    display: flex;
    flex-direction: column;
    gap: 2px;
}

.feature-title {
    font-size: 13px;
    font-weight: 500;
    color: white;
}

.feature-sub {
    font-size: 12px;
    color: rgba(255, 255, 255, 0.55);
}

.info {
    font-size: 11px;
    color: rgba(255, 255, 255, 0.4);
    line-height: 1.5;
    margin-top: 24px;
}

.register-page {
    width: 100%;
    height: 100vh;
    display: flex;
    background: rgb(228, 228, 228);
    justify-content: center;
    align-items: center;
}

h1 {
    font-size: 24px;
}

.brand h2 {
    margin: 0;
}

.brand h4 {
    margin: 0;
    font-weight: 400;
    opacity: 0.8;
}
.login-card {
    width: 900px;
    display: flex;
    flex-direction: column;
}
.form-group {
    margin-top: 20px;
    display: flex;
    flex-direction: column;
    gap: 8px;
    width: 100%;
}

.register-container {
    display: flex;
    width: 900px;
    border-radius: 10px 10px 0 0;
    overflow: hidden;
    min-height: 500px;
}

.register-left {
    background: #1a4a2e;
    display: flex;
    width: 40%;
    border-radius: 8px;
    padding: 40px;
    flex-direction: column;
    justify-content: space-between;
    color: white;
}

.register-left h4 {
    margin: 0;
    font-weight: 400;
    opacity: 0.8;
}

.register-right {
    background: rgb(255, 255, 255);
    width: 60%;
    padding: 40px;
    flex-direction: column;
    justify-content: flex-start;
    display: flex;
    gap: 8px;
}

.register-right h1 {
    margin: 0 0 4px 0;
}

.register-right h4 {
    margin: 0 0 16px 0;
    font-weight: 400;
    color: #555;
}

.form-label {
    font-size: 17px;
    font-weight: 600;
    color: #333;
    margin-top: 8px;
}

.form-input {
    width: 100%;
    min-height: 15px;
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 6px;
    margin-bottom: 4px;
    font-size: 14px;
    box-sizing: border-box;
}

.form-input:focus {
    outline: none;
    border-color: #1a4a2e;
    box-shadow: 0 0 0 2px rgba(26, 74, 46, 0.15);
}

.btn-register {
    width: 100%;
    padding: 12px;
    background: #1a4a2e;
    color: white;
    border: none;
    border-radius: 6px;
    font-size: 17px;
    font-weight: bold;
    cursor: pointer;
    margin-top: 8px;
    transition: background 0.2s;
}
.register-container {
    display: flex;
    width: 900px;
    border-radius: 10px 10px 0 0;
    overflow: hidden;
    min-height: 500px;
}

.form-footer {
    width: 900px;
    margin-top: 0;
    padding: 14px 24px;
    background: white;
    border-top: 1px solid #dcdcdc;
    border-radius: 0 0 10px 10px;

    display: flex;
    justify-content: center;
    align-items: center;
    gap: 12px;

    font-size: 12px;
    color: #666;
    box-sizing: border-box;
}
.btn-register:hover {
    background: #255c3a;
}
.register-card {
    width: 900px;
    display: flex;
    flex-direction: column;
}
.btn-register:disabled {
    background: #7a9e86;
    cursor: not-allowed;
}

.error-msg {
    color: #c0392b;
    font-size: 13px;
    margin: 0;
}

.support {
    color: #1a4a2e;
    cursor: pointer;
    margin-left: 4px;
    text-decoration: underline;
}

.info {
    opacity: 0.8;
    font-size: 13px;
}

.mobile-footer {
    display: none;
}

@media (max-width: 768px) {
    .register-page {
        background: #1a4a2e;
        align-items: flex-end;
        padding: 0;
    }

    .register-container {
        flex-direction: column;
        width: 100%;
        border-radius: 0;
        min-height: auto;
    }

    .register-left {
        background: transparent;
        width: 100%;
        padding: 40px 24px 20px;
        display: block;
        font-size: 13px;
        border-radius: 0;
    }

    .register-left p {
        font-size: 13px;
        white-space: normal;
        word-wrap: break-word;
    }

    ul {
        display: none;
    }

    .info {
        display: none !important;
    }

    .register-right {
        background: white;
        width: 100%;
        border-radius: 24px 24px 0 0;
        margin-top: 0;
        min-height: 70vh;
        padding: 30px 24px;
    }

    h1 {
        font-size: 20px;
    }

    .form-input {
        width: 85%;
    }

    .btn-register {
        width: 85%;
    }

    .mobile-footer {
        display: block;
        font-size: 11px;
        color: #999;
        text-align: center;
        margin-top: 10px;
    }
}
</style>
