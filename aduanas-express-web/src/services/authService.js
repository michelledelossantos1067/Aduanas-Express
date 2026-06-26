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
export const changePassword = (data) => {
    console.log('Data recibida:', data)
    console.log('PasswordActual:', data.passwordActual)
    console.log('Longitud:', data.passwordActual?.length)
    
    const payload = {
        Email: data.email,
        PasswordActual: data.passwordActual,
        PasswordNueva: data.passwordNueva
    }
    
    console.log('Payload enviado:', JSON.stringify(payload))
    
    return api.put('/Auth/change-password', payload)
}
export const generateOtp = (email) => {
    return api.post('/Auth/generate-otp', { Email: email })
}

export const validateOtp = (email, otp) => {
    return api.post('/Auth/validate-otp', { Email: email, Otp: otp })
}

export const resetPasswordWithOtp = (email, otp, nuevaPassword) => {
    return api.post('/Auth/reset-password-otp', {
        Email: email,
        Otp: otp,
        NuevaPassword: nuevaPassword
    })
}