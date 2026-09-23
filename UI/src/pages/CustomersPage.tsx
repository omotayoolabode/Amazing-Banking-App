import { useCallback, useEffect, useState } from 'react'
import { ApiError } from '../api/client'
import {
  createCustomer,
  getCustomers,
  updateCustomer,
} from '../api/customers'
import { CustomerForm } from '../components/CustomerForm'
import { CustomerList } from '../components/CustomerList'
import { StatusBanner } from '../components/StatusBanner'
import type { Customer, CustomerFormInput } from '../types/customer'

export function CustomersPage() {
  const [customers, setCustomers] = useState<Customer[]>([])
  const [loadingList, setLoadingList] = useState(true)
  const [listError, setListError] = useState<string | null>(null)
  const [formMode, setFormMode] = useState<'create' | 'edit'>('create')
  const [editingId, setEditingId] = useState<number | null>(null)
  const [editingValues, setEditingValues] = useState<
    CustomerFormInput | undefined
  >(undefined)
  const [formError, setFormError] = useState<string | null>(null)
  const [formSuccess, setFormSuccess] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  const loadCustomers = useCallback(async () => {
    setLoadingList(true)
    setListError(null)
    try {
      const data = await getCustomers()
      setCustomers(data)
    } catch (error) {
      const message =
        error instanceof ApiError
          ? error.message
          : 'Customer data could not be loaded.'
      setListError(message)
      setCustomers([])
    } finally {
      setLoadingList(false)
    }
  }, [])

  useEffect(() => {
    void loadCustomers()
  }, [loadCustomers])

  function startEdit(customer: Customer) {
    setFormMode('edit')
    setEditingId(customer.id)
    setEditingValues({
      firstName: customer.firstName,
      lastName: customer.lastName,
      email: customer.email,
      phone: customer.phone,
    })
    setFormError(null)
    setFormSuccess(null)
  }

  function cancelEdit() {
    setFormMode('create')
    setEditingId(null)
    setEditingValues(undefined)
    setFormError(null)
  }

  async function handleCreate(values: CustomerFormInput) {
    setSubmitting(true)
    setFormError(null)
    setFormSuccess(null)
    try {
      await createCustomer(values)
      setFormSuccess('Customer created successfully.')
      await loadCustomers()
    } catch (error) {
      setFormError(
        error instanceof ApiError
          ? error.message
          : 'Could not create the customer.',
      )
    } finally {
      setSubmitting(false)
    }
  }

  async function handleUpdate(values: CustomerFormInput) {
    if (editingId == null) {
      setFormError('No customer selected for update.')
      return
    }

    setSubmitting(true)
    setFormError(null)
    setFormSuccess(null)
    try {
      await updateCustomer(editingId, values)
      setFormSuccess('Customer updated successfully.')
      cancelEdit()
      await loadCustomers()
    } catch (error) {
      if (error instanceof ApiError && error.status === 404) {
        setFormError('That customer no longer exists.')
      } else {
        setFormError(
          error instanceof ApiError
            ? error.message
            : 'Could not update the customer.',
        )
      }
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="mx-auto flex max-w-5xl flex-col gap-6 px-4 py-8 sm:px-6">
      <header className="space-y-2">
        <p className="text-sm font-semibold uppercase tracking-wide text-teal-800">
          Amazing Banking App
        </p>
        <h1 className="text-3xl font-bold text-slate-900">Customers</h1>
        <p className="max-w-2xl text-slate-600">
          View, create, and update customers through the Amazings API.
        </p>
      </header>

      {listError && <StatusBanner kind="error" message={listError} />}
      {formError && <StatusBanner kind="error" message={formError} />}
      {formSuccess && <StatusBanner kind="success" message={formSuccess} />}

      <CustomerForm
        key={formMode === 'edit' ? `edit-${editingId}` : 'create'}
        mode={formMode}
        initialValues={formMode === 'edit' ? editingValues : undefined}
        submitting={submitting}
        onSubmit={formMode === 'edit' ? handleUpdate : handleCreate}
        onCancel={formMode === 'edit' ? cancelEdit : undefined}
      />

      <section className="space-y-3">
        <div className="flex items-center justify-between gap-3">
          <h2 className="text-lg font-semibold text-slate-900">Customer list</h2>
          <button
            type="button"
            onClick={() => void loadCustomers()}
            disabled={loadingList}
            className="rounded-md border border-slate-300 px-3 py-1.5 text-sm hover:bg-white disabled:opacity-60"
          >
            {loadingList ? 'Refreshing…' : 'Refresh'}
          </button>
        </div>

        {loadingList && !listError ? (
          <StatusBanner kind="info" message="Loading customers…" />
        ) : (
          <CustomerList customers={customers} onEdit={startEdit} />
        )}
      </section>
    </div>
  )
}
