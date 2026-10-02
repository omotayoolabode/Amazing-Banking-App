import { apiRequest } from './client'
import type { RegisterInput, RegisterResponse } from '../types/auth'

export function registerUser(input: RegisterInput): Promise<RegisterResponse> {
  return apiRequest<RegisterResponse>('/Auth/register', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}
