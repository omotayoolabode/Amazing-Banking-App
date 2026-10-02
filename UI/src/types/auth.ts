export type RegisterInput = {
  firstName: string
  lastName: string
  email: string
  phone: string
  username: string
  password: string
}

export type RegisterResponse = {
  message: string
  userId: number
  username: string
  customerId: number
}

export const emptyRegisterForm = (): RegisterInput & { confirmPassword: string } => ({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  username: '',
  password: '',
  confirmPassword: '',
})
