<template>
  <div class="min-h-screen bg-slate-100">
    <header class="flex items-center justify-between bg-white px-8 py-4 shadow">
      <div>
        <h1 class="text-2xl font-semibold text-slate-700">Consultas médicas</h1>
        <p class="text-sm text-slate-500">Registra nuevos casos y consulta el historial.</p>
      </div>
      <router-link class="rounded-lg bg-slate-200 px-4 py-2 text-slate-600 hover:bg-slate-300" to="/dashboard">
        Volver al dashboard
      </router-link>
    </header>

    <main class="mx-auto mt-8 grid max-w-6xl gap-8 px-6 lg:grid-cols-[320px_1fr]">
      <section class="rounded-2xl bg-white p-6 shadow">
        <h2 class="text-lg font-semibold text-slate-700">Nueva consulta</h2>
        <form @submit.prevent="handleSubmit" class="mt-4 space-y-4">
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-600">Médico</label>
            <select v-model.number="form.medicoId" class="w-full rounded-lg border border-slate-300 px-3 py-2 focus:outline-none">
              <option disabled value="">Selecciona un médico</option>
              <option v-for="medico in medicos" :key="medico.id" :value="medico.id">
                {{ medico.primerNombre }} {{ medico.apellidoPaterno }}
              </option>
            </select>
          </div>
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-600">Paciente</label>
            <select v-model.number="form.pacienteId" class="w-full rounded-lg border border-slate-300 px-3 py-2 focus:outline-none">
              <option disabled value="">Selecciona un paciente</option>
              <option v-for="paciente in pacientes" :key="paciente.id" :value="paciente.id">
                {{ paciente.primerNombre }} {{ paciente.apellidoPaterno }}
              </option>
            </select>
          </div>
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-600">Síntomas</label>
            <textarea
              v-model="form.sintomas"
              class="w-full rounded-lg border border-slate-300 px-3 py-2 focus:outline-none"
              rows="2"
              required
            ></textarea>
          </div>
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-600">Recomendaciones</label>
            <textarea
              v-model="form.recomendaciones"
              class="w-full rounded-lg border border-slate-300 px-3 py-2 focus:outline-none"
              rows="2"
              required
            ></textarea>
          </div>
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-600">Diagnóstico</label>
            <textarea
              v-model="form.diagnostico"
              class="w-full rounded-lg border border-slate-300 px-3 py-2 focus:outline-none"
              rows="2"
              required
            ></textarea>
          </div>
          <p v-if="error" class="text-sm text-red-600">{{ error }}</p>
          <button type="submit" class="w-full rounded-lg bg-indigo-600 py-2 text-white hover:bg-indigo-700" :disabled="loading">
            <span v-if="loading">Guardando...</span>
            <span v-else>Registrar consulta</span>
          </button>
        </form>
      </section>

      <section class="rounded-2xl bg-white p-6 shadow">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-slate-700">Historial</h2>
          <button class="text-sm text-indigo-600 hover:underline" @click="fetchConsultas">Actualizar</button>
        </div>
        <div class="mt-4 space-y-4">
          <article
            v-for="consulta in consultas"
            :key="consulta.id"
            class="rounded-xl border border-slate-200 p-4"
          >
            <header class="flex flex-wrap items-center justify-between gap-2">
              <h3 class="text-lg font-semibold text-slate-700">{{ consulta.medicoNombreCompleto }}</h3>
              <span class="rounded-full bg-indigo-100 px-3 py-1 text-xs font-medium text-indigo-700">
                Paciente: {{ consulta.pacienteNombreCompleto }}
              </span>
            </header>
            <div class="mt-3 space-y-2 text-sm text-slate-600">
              <p><span class="font-semibold">Síntomas:</span> {{ consulta.sintomas }}</p>
              <p><span class="font-semibold">Recomendaciones:</span> {{ consulta.recomendaciones }}</p>
              <p><span class="font-semibold">Diagnóstico:</span> {{ consulta.diagnostico }}</p>
            </div>
          </article>
          <p v-if="!consultas.length" class="text-sm text-slate-500">No hay consultas registradas.</p>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()
const consultas = ref([])
const medicos = ref([])
const pacientes = ref([])
const loading = ref(false)
const error = ref('')
const form = reactive({ medicoId: '', pacienteId: '', sintomas: '', recomendaciones: '', diagnostico: '' })

const ensureAuth = () => {
  if (!auth.isAuthenticated) {
    router.push({ name: 'login' })
  }
}

const fetchMedicos = async () => {
  const { data } = await api.get('/medicos')
  medicos.value = data.filter(m => m.activo)
}

const fetchPacientes = async () => {
  const { data } = await api.get('/pacientes')
  pacientes.value = data.filter(p => p.activo)
}

const fetchConsultas = async () => {
  const { data } = await api.get('/consultas')
  consultas.value = data
}

const resetForm = () => {
  form.medicoId = ''
  form.pacienteId = ''
  form.sintomas = ''
  form.recomendaciones = ''
  form.diagnostico = ''
}

const handleSubmit = async () => {
  loading.value = true
  error.value = ''
  try {
    await api.post('/consultas', { ...form, medicoId: Number(form.medicoId), pacienteId: Number(form.pacienteId) })
    resetForm()
    await fetchConsultas()
  } catch (err) {
    error.value = err.response?.data ?? 'Ocurrió un error al guardar la consulta.'
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  ensureAuth()
  await Promise.all([fetchMedicos(), fetchPacientes(), fetchConsultas()])
})
</script>
