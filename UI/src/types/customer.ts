export interface Customer {
  id: number
  firstName: string
  lastName: string
  email: string
  phone: string
}

export interface CustomerFormInput {
  firstName: string
  lastName: string
  email: string
  phone: string
}

export const emptyCustomerForm = (): CustomerFormInput => ({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
})
