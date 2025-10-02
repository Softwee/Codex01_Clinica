import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../services/api'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '')
  const nombre = ref(localStorage.getItem('nombre') || '')

  const isAuthenticated = computed(() => !!token.value)

  async function login(credentials) {
    const { data } = await api.post('/auth/login', credentials)
    token.value = data.token
    nombre.value = data.nombreCompleto
    localStorage.setItem('token', token.value)
    localStorage.setItem('nombre', nombre.value)
    api.defaults.headers.common.Authorization = `Bearer ${token.value}`
    return data
  }

  function logout() {
    token.value = ''
    nombre.value = ''
    localStorage.removeItem('token')
    localStorage.removeItem('nombre')
    delete api.defaults.headers.common.Authorization
  }

  if (token.value) {
    api.defaults.headers.common.Authorization = `Bearer ${token.value}`
  }

  return { token, nombre, isAuthenticated, login, logout }
})
