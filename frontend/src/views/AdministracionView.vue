<template>
  <div class="min-h-screen bg-slate-100">
    <header class="flex items-center justify-between bg-white px-8 py-4 shadow">
      <div>
        <h1 class="text-2xl font-semibold text-slate-700">Administración</h1>
        <p class="text-sm text-slate-500">Gestiona usuarios, médicos y pacientes de la clínica.</p>
      </div>
      <router-link class="rounded-lg bg-slate-200 px-4 py-2 text-slate-600 hover:bg-slate-300" to="/dashboard">
        Volver al dashboard
      </router-link>
    </header>

    <main class="mx-auto mt-8 flex max-w-6xl flex-col gap-8 px-6 pb-12">
      <section class="rounded-2xl bg-white p-6 shadow">
        <h2 class="text-lg font-semibold text-slate-700">Usuarios</h2>
        <div class="mt-4 grid gap-6 lg:grid-cols-[320px_1fr]">
          <form @submit.prevent="createUser" class="space-y-4">
            <div>
              <label class="mb-1 block text-sm font-medium text-slate-600">Correo</label>
              <input v-model="userForm.correo" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
            </div>
            <div>
              <label class="mb-1 block text-sm font-medium text-slate-600">Contraseña</label>
              <input v-model="userForm.password" type="password" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
            </div>
            <div>
              <label class="mb-1 block text-sm font-medium text-slate-600">Nombre completo</label>
              <input v-model="userForm.nombreCompleto" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
            </div>
            <div>
              <label class="mb-1 block text-sm font-medium text-slate-600">Médico asociado</label>
              <select v-model="userForm.medicoId" class="w-full rounded-lg border border-slate-300 px-3 py-2">
                <option :value="null">Sin asignar</option>
                <option v-for="medico in medicos" :key="medico.id" :value="medico.id">
                  {{ medico.primerNombre }} {{ medico.apellidoPaterno }}
                </option>
              </select>
            </div>
            <label class="inline-flex items-center gap-2 text-sm text-slate-600">
              <input type="checkbox" v-model="userForm.activo" class="rounded border-slate-300" /> Activo
            </label>
            <p v-if="userError" class="text-sm text-red-600">{{ userError }}</p>
            <button type="submit" class="w-full rounded-lg bg-indigo-600 py-2 text-white hover:bg-indigo-700">Crear usuario</button>
          </form>

          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-slate-200 text-sm">
              <thead class="bg-slate-50">
                <tr>
                  <th class="px-4 py-2 text-left font-medium text-slate-600">Correo</th>
                  <th class="px-4 py-2 text-left font-medium text-slate-600">Nombre</th>
                  <th class="px-4 py-2 text-left font-medium text-slate-600">Médico</th>
                  <th class="px-4 py-2 text-center font-medium text-slate-600">Activo</th>
                  <th class="px-4 py-2 text-center font-medium text-slate-600">Acciones</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="usuario in usuarios" :key="usuario.id" class="hover:bg-slate-50">
                  <td class="px-4 py-2">{{ usuario.correo }}</td>
                  <td class="px-4 py-2">{{ usuario.nombreCompleto }}</td>
                  <td class="px-4 py-2">{{ getMedicoName(usuario.medicoId) }}</td>
                  <td class="px-4 py-2 text-center">
                    <span
                      :class="[
                        'rounded-full px-3 py-1 text-xs font-semibold',
                        usuario.activo ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-200 text-slate-600'
                      ]"
                    >
                      {{ usuario.activo ? 'Sí' : 'No' }}
                    </span>
                  </td>
                  <td class="px-4 py-2 text-center">
                    <button class="text-sm text-rose-600 hover:underline" @click="deleteUser(usuario.id)">Eliminar</button>
                  </td>
                </tr>
                <tr v-if="!usuarios.length">
                  <td colspan="5" class="px-4 py-4 text-center text-slate-500">No hay usuarios registrados.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </section>

      <section class="rounded-2xl bg-white p-6 shadow">
        <h2 class="text-lg font-semibold text-slate-700">Médicos</h2>
        <div class="mt-4 grid gap-6 lg:grid-cols-[320px_1fr]">
          <form @submit.prevent="createMedico" class="space-y-4">
            <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Primer nombre</label>
                <input v-model="medicoForm.primerNombre" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
              </div>
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Segundo nombre</label>
                <input v-model="medicoForm.segundoNombre" class="w-full rounded-lg border border-slate-300 px-3 py-2" />
              </div>
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Apellido paterno</label>
                <input v-model="medicoForm.apellidoPaterno" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
              </div>
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Apellido materno</label>
                <input v-model="medicoForm.apellidoMaterno" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
              </div>
            </div>
            <div>
              <label class="mb-1 block text-sm font-medium text-slate-600">Cédula</label>
              <input v-model="medicoForm.cedula" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
            </div>
            <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Teléfono</label>
                <input v-model="medicoForm.telefono" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
              </div>
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Especialidad</label>
                <input v-model="medicoForm.especialidad" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
              </div>
            </div>
            <div>
              <label class="mb-1 block text-sm font-medium text-slate-600">Correo electrónico</label>
              <input type="email" v-model="medicoForm.email" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
            </div>
            <label class="inline-flex items-center gap-2 text-sm text-slate-600">
              <input type="checkbox" v-model="medicoForm.activo" class="rounded border-slate-300" /> Activo
            </label>
            <p v-if="medicoError" class="text-sm text-red-600">{{ medicoError }}</p>
            <button type="submit" class="w-full rounded-lg bg-indigo-600 py-2 text-white hover:bg-indigo-700">Registrar médico</button>
          </form>

          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-slate-200 text-sm">
              <thead class="bg-slate-50">
                <tr>
                  <th class="px-4 py-2 text-left font-medium text-slate-600">Nombre</th>
                  <th class="px-4 py-2 text-left font-medium text-slate-600">Cédula</th>
                  <th class="px-4 py-2 text-left font-medium text-slate-600">Especialidad</th>
                  <th class="px-4 py-2 text-center font-medium text-slate-600">Activo</th>
                  <th class="px-4 py-2 text-center font-medium text-slate-600">Acciones</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="medico in medicos" :key="medico.id" class="hover:bg-slate-50">
                  <td class="px-4 py-2">{{ medico.primerNombre }} {{ medico.apellidoPaterno }}</td>
                  <td class="px-4 py-2">{{ medico.cedula }}</td>
                  <td class="px-4 py-2">{{ medico.especialidad }}</td>
                  <td class="px-4 py-2 text-center">
                    <span
                      :class="[
                        'rounded-full px-3 py-1 text-xs font-semibold',
                        medico.activo ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-200 text-slate-600'
                      ]"
                    >
                      {{ medico.activo ? 'Sí' : 'No' }}
                    </span>
                  </td>
                  <td class="px-4 py-2 text-center">
                    <button class="text-sm text-rose-600 hover:underline" @click="deleteMedico(medico.id)">
                      Eliminar
                    </button>
                  </td>
                </tr>
                <tr v-if="!medicos.length">
                  <td colspan="5" class="px-4 py-4 text-center text-slate-500">No hay médicos registrados.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </section>
      <section class="rounded-2xl bg-white p-6 shadow">
        <h2 class="text-lg font-semibold text-slate-700">Pacientes</h2>
        <div class="mt-4 grid gap-6 lg:grid-cols-[320px_1fr]">
          <form @submit.prevent="createPaciente" class="space-y-4">
            <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Primer nombre</label>
                <input v-model="pacienteForm.primerNombre" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
              </div>
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Segundo nombre</label>
                <input v-model="pacienteForm.segundoNombre" class="w-full rounded-lg border border-slate-300 px-3 py-2" />
              </div>
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Apellido paterno</label>
                <input v-model="pacienteForm.apellidoPaterno" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
              </div>
              <div>
                <label class="mb-1 block text-sm font-medium text-slate-600">Apellido materno</label>
                <input v-model="pacienteForm.apellidoMaterno" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
              </div>
            </div>
            <div>
              <label class="mb-1 block text-sm font-medium text-slate-600">Teléfono</label>
              <input v-model="pacienteForm.telefono" class="w-full rounded-lg border border-slate-300 px-3 py-2" required />
            </div>
            <label class="inline-flex items-center gap-2 text-sm text-slate-600">
              <input type="checkbox" v-model="pacienteForm.activo" class="rounded border-slate-300" /> Activo
            </label>
            <p v-if="pacienteError" class="text-sm text-red-600">{{ pacienteError }}</p>
            <button type="submit" class="w-full rounded-lg bg-indigo-600 py-2 text-white hover:bg-indigo-700">
              Registrar paciente
            </button>
          </form>

          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-slate-200 text-sm">
              <thead class="bg-slate-50">
                <tr>
                  <th class="px-4 py-2 text-left font-medium text-slate-600">Nombre</th>
                  <th class="px-4 py-2 text-left font-medium text-slate-600">Teléfono</th>
                  <th class="px-4 py-2 text-center font-medium text-slate-600">Activo</th>
                  <th class="px-4 py-2 text-center font-medium text-slate-600">Acciones</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="paciente in pacientes" :key="paciente.id" class="hover:bg-slate-50">
                  <td class="px-4 py-2">{{ getPacienteNombre(paciente) }}</td>
                  <td class="px-4 py-2">{{ paciente.telefono }}</td>
                  <td class="px-4 py-2 text-center">
                    <span
                      :class="[
                        'rounded-full px-3 py-1 text-xs font-semibold',
                        paciente.activo ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-200 text-slate-600'
                      ]"
                    >
                      {{ paciente.activo ? 'Sí' : 'No' }}
                    </span>
                  </td>
                  <td class="px-4 py-2 text-center">
                    <button class="text-sm text-rose-600 hover:underline" @click="deletePaciente(paciente.id)">Eliminar</button>
                  </td>
                </tr>
                <tr v-if="!pacientes.length">
                  <td colspan="4" class="px-4 py-4 text-center text-slate-500">No hay pacientes registrados.</td>
                </tr>
              </tbody>
            </table>
          </div>
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

const usuarios = ref([])
const medicos = ref([])
const pacientes = ref([])
const userError = ref('')
const medicoError = ref('')
const pacienteError = ref('')

const userForm = reactive({ correo: '', password: '', nombreCompleto: '', medicoId: null, activo: true })
const medicoForm = reactive({
  primerNombre: '',
  segundoNombre: '',
  apellidoPaterno: '',
  apellidoMaterno: '',
  cedula: '',
  telefono: '',
  especialidad: '',
  email: '',
  activo: true
})
const pacienteForm = reactive({
  primerNombre: '',
  segundoNombre: '',
  apellidoPaterno: '',
  apellidoMaterno: '',
  telefono: '',
  activo: true
})

const ensureAuth = () => {
  if (!auth.isAuthenticated) {
    router.push({ name: 'login' })
  }
}

const fetchUsuarios = async () => {
  const { data } = await api.get('/usuarios')
  usuarios.value = data
}

const fetchMedicos = async () => {
  const { data } = await api.get('/medicos')
  medicos.value = data
}

const fetchPacientes = async () => {
  const { data } = await api.get('/pacientes')
  pacientes.value = data
}

const getMedicoName = id => {
  if (!id) return 'Sin asignar'
  const medico = medicos.value.find(m => m.id === id)
  return medico ? `${medico.primerNombre} ${medico.apellidoPaterno}` : 'Sin asignar'
}

const getPacienteNombre = paciente =>
  [paciente.primerNombre, paciente.segundoNombre, paciente.apellidoPaterno, paciente.apellidoMaterno].filter(Boolean).join(' ')

const resetUserForm = () => {
  userForm.correo = ''
  userForm.password = ''
  userForm.nombreCompleto = ''
  userForm.medicoId = null
  userForm.activo = true
}

const resetMedicoForm = () => {
  medicoForm.primerNombre = ''
  medicoForm.segundoNombre = ''
  medicoForm.apellidoPaterno = ''
  medicoForm.apellidoMaterno = ''
  medicoForm.cedula = ''
  medicoForm.telefono = ''
  medicoForm.especialidad = ''
  medicoForm.email = ''
  medicoForm.activo = true
}

const resetPacienteForm = () => {
  pacienteForm.primerNombre = ''
  pacienteForm.segundoNombre = ''
  pacienteForm.apellidoPaterno = ''
  pacienteForm.apellidoMaterno = ''
  pacienteForm.telefono = ''
  pacienteForm.activo = true
}

const createUser = async () => {
  userError.value = ''
  try {
    const payload = {
      ...userForm,
      medicoId: userForm.medicoId ? Number(userForm.medicoId) : null
    }
    await api.post('/usuarios', payload)
    resetUserForm()
    await fetchUsuarios()
  } catch (err) {
    userError.value = err.response?.data ?? 'No se pudo crear el usuario.'
  }
}

const createMedico = async () => {
  medicoError.value = ''
  try {
    await api.post('/medicos', { ...medicoForm })
    resetMedicoForm()
    await fetchMedicos()
  } catch (err) {
    medicoError.value = err.response?.data ?? 'No se pudo registrar el médico.'
  }
}

const createPaciente = async () => {
  pacienteError.value = ''
  try {
    await api.post('/pacientes', { ...pacienteForm })
    resetPacienteForm()
    await fetchPacientes()
  } catch (err) {
    pacienteError.value = err.response?.data ?? 'No se pudo registrar el paciente.'
  }
}

const deleteUser = async id => {
  await api.delete(`/usuarios/${id}`)
  await fetchUsuarios()
}

const deleteMedico = async id => {
  await api.delete(`/medicos/${id}`)
  await fetchMedicos()
  await fetchUsuarios()
}

const deletePaciente = async id => {
  await api.delete(`/pacientes/${id}`)
  await fetchPacientes()
}

onMounted(async () => {
  ensureAuth()
  await Promise.all([fetchMedicos(), fetchUsuarios(), fetchPacientes()])
})
</script>
