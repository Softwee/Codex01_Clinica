<template>
  <div class="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-200 to-slate-400">
    <div class="w-full max-w-md rounded-xl bg-white p-8 shadow-xl">
      <h1 class="mb-6 text-center text-3xl font-bold text-slate-700">Iniciar sesión</h1>
      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label for="correo" class="mb-1 block text-sm font-semibold text-slate-600">Correo</label>
          <input
            v-model="form.correo"
            type="text"
            id="correo"
            class="w-full rounded-lg border border-slate-300 px-4 py-2 focus:border-indigo-500 focus:outline-none"
            placeholder="usuario"
            required
          />
        </div>
        <div>
          <label for="password" class="mb-1 block text-sm font-semibold text-slate-600">Contraseña</label>
          <input
            v-model="form.password"
            type="password"
            id="password"
            class="w-full rounded-lg border border-slate-300 px-4 py-2 focus:border-indigo-500 focus:outline-none"
            placeholder="••••••"
            required
          />
        </div>
        <p v-if="error" class="text-sm text-red-600">{{ error }}</p>
        <button
          type="submit"
          class="w-full rounded-lg bg-indigo-600 py-2 text-white transition hover:bg-indigo-700"
          :disabled="loading"
        >
          <span v-if="loading">Validando...</span>
          <span v-else>Entrar</span>
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()
const form = reactive({ correo: '', password: '' })
const loading = ref(false)
const error = ref('')

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  try {
    const data = await auth.login(form)
    if (!data.activo) {
      error.value = 'El usuario no está activo.'
      auth.logout()
      return
    }
    router.push({ name: 'dashboard' })
  } catch (err) {
    error.value = 'Credenciales inválidas.'
  } finally {
    loading.value = false
  }
}
</script>
