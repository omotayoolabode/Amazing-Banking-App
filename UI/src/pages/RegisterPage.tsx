import { useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { registerUser } from '../api/auth'
import { ApiError } from '../api/client'
import { StatusBanner } from '../components/StatusBanner'
import { emptyRegisterForm } from '../types/auth'

const fieldClass =
  'mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm outline-none focus:border-teal-600 focus:ring-1 focus:ring-teal-600'

function hasLetterAndNumber(password: string): boolean {
  return /[A-Za-z]/.test(password) && /\d/.test(password)
}

function validate(values: ReturnType<typeof emptyRegisterForm>): string | null {
  const firstName = values.firstName.trim()
  const lastName = values.lastName.trim()
  const email = values.email.trim()
  const phone = values.phone.trim()
  const username = values.username.trim()

  if (!firstName || !lastName || !email || !phone || !username) {
    return 'All fields are required (whitespace alone is not allowed).'
  }

  if (!/^[a-zA-Z0-9]{3,30}$/.test(username)) {
    return 'Username must be 3 to 30 letters or numbers.'
  }

  if (values.password.length < 8 || !hasLetterAndNumber(values.password)) {
    return 'Password must be at least 8 characters and contain at least one letter and one number.'
  }

  if (values.password !== values.confirmPassword) {
    return 'Password and confirm password must match.'
  }

  return null
}

export function RegisterPage() {
  const [values, setValues] = useState(emptyRegisterForm)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    const validationError = validate(values)
    if (validationError) {
      setError(validationError)
      setSuccess(null)
      return
    }

    setSubmitting(true)
    setError(null)
    setSuccess(null)
    try {
      const result = await registerUser({
        firstName: values.firstName.trim(),
        lastName: values.lastName.trim(),
        email: values.email.trim(),
        phone: values.phone.trim(),
        username: values.username.trim(),
        password: values.password,
      })
      setSuccess(
        `${result.message} You can now log in with username ${result.username}.`,
      )
      setValues(emptyRegisterForm())
    } catch (err) {
      setError(
        err instanceof ApiError
          ? err.message
          : 'Could not create the account.',
      )
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="mx-auto flex max-w-xl flex-col gap-6 px-4 py-8 sm:px-6">
      <header className="space-y-2">
        <p className="text-sm font-semibold uppercase tracking-wide text-teal-800">
          Amazing Banking App
        </p>
        <h1 className="text-3xl font-bold text-slate-900">Create an account</h1>
        <p className="text-slate-600">
          Register with your details. You can open a bank account after you log
          in.
        </p>
      </header>

      {error && <StatusBanner kind="error" message={error} />}
      {success && <StatusBanner kind="success" message={success} />}

      <form
        onSubmit={handleSubmit}
        className="space-y-4 rounded-lg border border-slate-200 bg-white p-5 shadow-sm"
      >
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
          <label className="block text-sm font-medium text-slate-700 sm:col-span-2">
            Username
            <input
              className={fieldClass}
              value={values.username}
              onChange={(e) =>
                setValues((prev) => ({ ...prev, username: e.target.value }))
              }
              disabled={submitting}
              autoComplete="username"
            />
          </label>
          <label className="block text-sm font-medium text-slate-700">
            Password
            <input
              type="password"
              className={fieldClass}
              value={values.password}
              onChange={(e) =>
                setValues((prev) => ({ ...prev, password: e.target.value }))
              }
              disabled={submitting}
              autoComplete="new-password"
            />
          </label>
          <label className="block text-sm font-medium text-slate-700">
            Confirm password
            <input
              type="password"
              className={fieldClass}
              value={values.confirmPassword}
              onChange={(e) =>
                setValues((prev) => ({
                  ...prev,
                  confirmPassword: e.target.value,
                }))
              }
              disabled={submitting}
              autoComplete="new-password"
            />
          </label>
        </div>

        <p className="text-xs text-slate-500">
          Password must be at least 8 characters and include a letter and a
          number. Username must be 3 to 30 letters or numbers.
        </p>

        <button
          type="submit"
          disabled={submitting}
          className="rounded-md bg-teal-700 px-4 py-2 text-sm font-medium text-white hover:bg-teal-800 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {submitting ? 'Creating account…' : 'Register'}
        </button>
      </form>

      <p className="text-sm text-slate-600">
        After registering, you can log in. This screen does not open a bank
        account yet.{' '}
        <Link className="font-medium text-teal-800 hover:underline" to="/">
          Back to customers
        </Link>
      </p>
    </div>
  )
}
