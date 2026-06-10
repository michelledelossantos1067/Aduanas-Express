<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'
import { login } from '../../services/authService'
import { ref } from 'vue'
const authStore = useAuthStore()
const router = useRouter()
const error = ref("")
const form = ref({ email: '', password: '' })
async function handleLogin() {
    if (!form.value.email || !form.value.password) {
        error.value = "Ingresa tu email y contraseña"
        return
    }
    const response = await login(form.value.email, form.value.password)
    authStore.iniciarSesion(response.data.token, {
        nombre: response.data.nombre,
        rol: response.data.rol
    })
    router.push('/dashboard')
}
</script>

<template>
    <div class="login-page">
        <div class="login-container">

            <div class="login-left">
                <div class="brand">
                    <h2>Aduanas Express</h2>
                    <h4>Sistema de transporte</h4>

                </div>
                <p>Plataforma para el sistema de transporte para los trabajadores de aduanas</p>
                <ul>
                    <li>Gestión de rutas y horarios</li>
                    <li>Asignación de unidades y personal</li>
                    <li>Monitoreo de trazabilidad y tiempos</li>
                    <li>Reportes y auditoría de servicios</li>
                </ul>
                <p class="info">© 2025 Aduanas Express — Uso institucional exclusivo. Acceso restringido a personal
                    autorizado.</p>
            </div>

            <div class="login-right">
                <h1>Acceso al sistema de Transporte</h1>
                <h4>Ingrese sus credenciales para continuar</h4>
                <div class="form-group">
                    <label class="form-label">Correo electrónico</label>
                    <input v-model="form.email" type="email" class="form-input" />
                    <label class="form-label">Contraseña</label>
                    <input v-model="form.password" type="password" class="form-input" />
                    <button @click="handleLogin" class="btn-login">Inicio de Sesion</button>
                    <p>¿Problemas de acceso?<span class="suport">Contacta soporte técnico</span></p>
                    <p class="mobile-footer">Aduanas Express Uso institucional exclusivo © 2026</p>
                    <p v-if="error">{{ error }}</p>
                </div>

            </div>
        </div>
    </div>
</template>

<style scoped>
.login-page {
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

.brand {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.form-group {
    margin-top: 20px;
    display: flex;
    flex-direction: column;
    gap: 8px;
    width: 100%;
}

.brand h2 {
    margin: 0;
}

.brand h4 {
    margin: 0;
    font-weight: 400;
    opacity: 0.8;
}

.login-left h2 {
    margin: 0;
}

.suport {
    color: #1a4a2e;
    cursor: pointer;
    margin-left: 4px;
    text-decoration: underline;
}

.login-left h4 {
    margin: 0;
    font-weight: 400;
    opacity: 0.8;
}

.login-container {
    display: flex;
    width: 900px;
    border-radius: 10px;
    overflow: hidden;
    min-height: 500px;
}

.login-left {
    background: #1a4a2e;
    display: flex;
    width: 40%;
    border-radius: 8px;
    padding: 40px;
    flex-direction: column;
    justify-content: space-between;
    color: white;
}

.login-right {
    background: rgb(255, 255, 255);
    width: 60%;
    padding: 40px;
    flex-direction: column;
    justify-content: flex-start;
    display: flex;
    gap: 8px;
}

.login-right h1 {
    margin: 0 0 4px 0;
}

.login-right h4 {
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
    margin-bottom: 16px;
    font-size: 14px;
}

.btn-login {
    width: 100%;
    padding: 12px;
    background: #1a4a2e;
    color: white;
    border: none;
    border-radius: 6px;
    font-size: 17px;
    font-weight: bold;
    cursor: pointer;
}

.info {
    opacity: 0.8;
}

.mobile-footer {
    display: none;
}

@media (max-width: 768px) {

    .mobile-footer {
        display: block;
        font-size: 11px;
        color: #999;
        text-align: center;
        margin-top: 10px;
    }

    .info {
        display: none !important;
    }

    .login-container {
        flex-direction: column;
        width: 90%;
        min-height: auto;
    }

    .login-left {
        width: 100%;
        padding: 24px;
        min-height: auto;
        display: block;
        font-size: 13px;
    }

    .login-right {
        width: 100%;
        padding: 24px;
        border-radius: 20px 20px 0 0;
        margin-top: -20px;
    }

    ul {
        display: none;
    }

    .login-left p {
        font-size: 13px;
        overflow: hidden;
        white-space: normal;
        word-wrap: break-word;
    }

    h1 {
        font-size: 20px;
    }

    .login-page {
        background: #1a4a2e;
        align-items: flex-end;
        padding: 0;
    }

    .login-container {
        width: 100%;
        border-radius: 0;
    }

    .login-left {
        background: transparent;
        padding: 40px 24px 20px;
        display: block;
        font-size: 13px;
    }

    .login-right {
        background: white;
        border-radius: 24px 24px 0 0;
        margin-top: 0;
        min-height: 70vh;
        padding: 30px 24px;
    }

    .form-input {
        width: 85%;
    }

    .btn-login {
        width: 85%;
    }
}
</style>