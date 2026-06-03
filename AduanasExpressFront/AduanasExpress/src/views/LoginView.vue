<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const form = ref({
  email: '',
  password: ''
})

const submitForm = async () => {
  console.log('Login:', form.value)
  // Cuando el backend esté listo:
  // const response = await authService.login(form.value)
  // router.push('/dashboard')
}
</script>

<template>
  <div class="form-container">
    <h2>Iniciar Sesión</h2>

    <form @submit.prevent="submitForm">
      <div>
        <label>Email:</label>
        <input v-model="form.email" type="email" placeholder="correo@ejemplo.com" required />
      </div>

      <div>
        <label>Contraseña:</label>
        <input v-model="form.password" type="password" placeholderfind src -type f
="••••••••" required />
      </div>

      <button type="submit">Entrar</button>
    </form>
  </div>
</template>

<style scoped>
.form-container { max-width: 400px; margin: auto; }
input { display: block; width: 100%; margin-bottom: 10px; }
</style>