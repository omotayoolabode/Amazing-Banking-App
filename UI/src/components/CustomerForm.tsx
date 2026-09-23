import { useEffect, useState, type FormEvent } from 'react'
import type { CustomerFormInput } from '../types/customer'
import { emptyCustomerForm } from '../types/customer'

type CustomerFormProps = {
  mode: 'create' | 'edit'
  initialValues?: CustomerFormInput
  submitting: boolean
  onSubmit: (values: CustomerFormInput) => Promise<void> | void
  onCancel?: () => void
}

function validate(values: CustomerFormInput): string | null {
  const trimmed = {
    firstName: values.firstName.trim(),
    lastName: values.lastName.trim(),
    email: values.email.trim(),
    phone: values.phone.trim(),
  }

  if (
    !trimmed.firstName ||
    !trimmed.lastName ||
    !trimmed.email ||
    !trimmed.phone
  ) {
    return 'All fields are required (whitespace alone is not allowed).'
  }

  return null
}

export function CustomerForm({
  mode,
  initialValues,
  submitting,
  onSubmit,
  onCancel,
}: CustomerFormProps) {
  const [values, setValues] = useState<CustomerFormInput>(
    initialValues ?? emptyCustomerForm(),
  )
  const [validationError, setValidationError] = useState<string | null>(null)

  useEffect(() => {
    setValues(initialValues ?? emptyCustomerForm())
    setValidationError(null)
  }, [initialValues, mode])

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    const error = validate(values)
    if (error) {
      setValidationError(error)
      return
    }

    setValidationError(null)
    await onSubmit({
      firstName: values.firstName.trim(),
      lastName: values.lastName.trim(),
      email: values.email.trim(),
      phone: values.phone.trim(),
    })

    if (mode === 'create') {
      setValues(emptyCustomerForm())
    }
  }

  const fieldClass =
    'mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-teal-600 focus:ring-1 focus:ring-teal-600'

  return (
    <form
      onSubmit={handleSubmit}
      className="space-y-4 rounded-lg border border-slate-200 bg-white p-5 shadow-sm"
    >
      <h2 className="text-lg font-semibold text-slate-900">
        {mode === 'create' ? 'Create customer' : 'Edit customer'}
      </h2>

      {validationError && (
        <p className="text-sm text-red-700" role="alert">
          {validationError}
        </p>
      )}

      <div className="grid gap-4 sm:grid-cols-2">
        <label className="block text-sm font-medium text-slate-700">
          First name
          <input
            className={fieldClass}
            value={values.firstName}
            onChange={(e) =>
              setValues((prev) => ({ ...prev, firstName: e.target.value }))
            }
            disabled={submitting}
            autoComplete="given-name"
          />
        </label>
        <label className="block text-sm font-medium text-slate-700">
          Last name
          <input
            className={fieldClass}
            value={values.lastName}
            onChange={(e) =>
              setValues((prev) => ({ ...prev, lastName: e.target.value }))
            }
            disabled={submitting}
            autoComplete="family-name"
          />
        </label>
        <label className="block text-sm font-medium text-slate-700">
          Email
          <input
            type="email"
            className={fieldClass}
            value={values.email}
            onChange={(e) =>
              setValues((prev) => ({ ...prev, email: e.target.value }))
            }
            disabled={submitting}
            autoComplete="email"
          />
        </label>
        <label className="block text-sm font-medium text-slate-700">
          Phone
          <input
            className={fieldClass}
            value={values.phone}
            onChange={(e) =>
              setValues((prev) => ({ ...prev, phone: e.target.value }))
            }
            disabled={submitting}
            autoComplete="tel"
          />
        </label>
      </div>

      <div className="flex flex-wrap gap-3">
        <button
          type="submit"
          disabled={submitting}
          className="rounded-md bg-teal-700 px-4 py-2 text-sm font-medium text-white hover:bg-teal-800 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {submitting
            ? 'Saving…'
            : mode === 'create'
              ? 'Create customer'
              : 'Save changes'}
        </button>
        {mode === 'edit' && onCancel && (
          <button
            type="button"
            onClick={onCancel}
            disabled={submitting}
            className="rounded-md border border-slate-300 px-4 py-2 text-sm font-medium text-slate-800 hover:bg-slate-50 disabled:opacity-60"
          >
            Cancel
          </button>
        )}
      </div>
    </form>
  )
}
