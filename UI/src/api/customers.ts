import { apiRequest } from './client'
import type { Customer, CustomerFormInput } from '../types/customer'

export function getCustomers(): Promise<Customer[]> {
  return apiRequest<Customer[]>('/Customers')
}

export function createCustomer(input: CustomerFormInput): Promise<void> {
  return apiRequest<void>('/Customers', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export function updateCustomer(
  id: number,
  input: CustomerFormInput,
): Promise<void> {
  return apiRequest<void>(`/Customers/${id}`, {
    method: 'PUT',
    body: JSON.stringify(input),
  })
}
