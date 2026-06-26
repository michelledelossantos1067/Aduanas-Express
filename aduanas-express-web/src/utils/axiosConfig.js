import axios from 'axios'

const api = axios.create({
    baseURL: 'http://localhost:5245/api',
    headers: {
        'Content-Type': 'application/json'
    }
})

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token')
    if (token) {
        console.log(localStorage.getItem('token'))
        config.headers.Authorization = `Bearer ${token}`
    }
    return config
})

export default api