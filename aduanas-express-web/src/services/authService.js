import api from '../utils/axiosConfig'

export const login = (email,password)=>{
    return api.post('/Auth/login',{email,password})
}
export const register = (data)=>{
    return api.post('/Auth/register',data)
}
export const logout = ()=>{
    return api.post('/Auth/logout')
}
export const resetPassword = (email)=>{
    return api.post('/Auth/reset-password',{email})
}
export const changePassword = (data)=>{
    return api.put('/Auth/change-password',data)
}