<template>
  <div class="min-h-screen bg-slate-100">
    <header class="flex items-center justify-between bg-white px-8 py-4 shadow">
      <div>
        <h1 class="text-2xl font-semibold text-slate-700">Bienvenido, {{ auth.nombre || 'Usuario' }}</h1>
        <p class="text-sm text-slate-500">Panel principal de la clínica</p>
      </div>
      <button class="rounded-lg bg-rose-500 px-4 py-2 text-white hover:bg-rose-600" @click="handleLogout">
        Cerrar sesión
      </button>
    </header>

    <main class="mx-auto mt-10 grid max-w-5xl gap-6 px-6 md:grid-cols-2">
      <router-link
        to="/consultas"
        class="group rounded-2xl bg-white p-8 shadow transition hover:-translate-y-1 hover:shadow-lg"
      >
        <h2 class="text-xl font-bold text-indigo-600">Consultas</h2>
        <p class="mt-2 text-slate-500">Gestiona y revisa el historial de consultas médicas.</p>
      </router-link>
      <router-link
        to="/administracion"
        class="group rounded-2xl bg-white p-8 shadow transition hover:-translate-y-1 hover:shadow-lg"
      >
        <h2 class="text-xl font-bold text-indigo-600">Administración</h2>
        <p class="mt-2 text-slate-500">Crea, actualiza y desactiva usuarios y médicos.</p>
      </router-link>
    </main>
  </div>
</template>

<script setup>
import { useAuthStore } from '../stores/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

const handleLogout = () => {
  auth.logout()
  router.push({ name: 'login' })
}
</script>
