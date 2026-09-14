import type { Customer } from '../types/customer'

type CustomerListProps = {
  customers: Customer[]
  onEdit: (customer: Customer) => void
}

export function CustomerList({ customers, onEdit }: CustomerListProps) {
  if (customers.length === 0) {
    return (
      <p className="rounded-md border border-dashed border-slate-300 bg-white/70 px-4 py-8 text-center text-slate-600">
        No customers yet. Create one to get started.
      </p>
    )
  }

  return (
    <div className="overflow-x-auto rounded-lg border border-slate-200 bg-white shadow-sm">
      <table className="min-w-full text-left text-sm">
        <thead className="bg-slate-100 text-slate-700">
          <tr>
            <th className="px-4 py-3 font-semibold">First name</th>
            <th className="px-4 py-3 font-semibold">Last name</th>
            <th className="px-4 py-3 font-semibold">Email</th>
            <th className="px-4 py-3 font-semibold">Phone</th>
            <th className="px-4 py-3 font-semibold">Actions</th>
          </tr>
        </thead>
        <tbody>
          {customers.map((customer) => (
            <tr key={customer.id} className="border-t border-slate-100">
              <td className="px-4 py-3">{customer.firstName}</td>
              <td className="px-4 py-3">{customer.lastName}</td>
              <td className="px-4 py-3">{customer.email}</td>
              <td className="px-4 py-3">{customer.phone}</td>
              <td className="px-4 py-3">
                <button
                  type="button"
                  onClick={() => onEdit(customer)}
                  className="rounded border border-slate-300 px-3 py-1 text-slate-800 hover:bg-slate-50"
                >
                  Edit
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
