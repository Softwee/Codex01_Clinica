import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:57761/api'
})

api.interceptors.response.use(
  response => response,
  error => {
    if (error.response && error.response.status === 401) {
      window.localStorage.removeItem('token')
      window.localStorage.removeItem('nombre')
    }
    return Promise.reject(error)
  }
)

export default api
