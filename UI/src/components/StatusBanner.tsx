type StatusBannerProps = {
  kind: 'error' | 'success' | 'info'
  message: string
}

const styles: Record<StatusBannerProps['kind'], string> = {
  error: 'border-red-300 bg-red-50 text-red-900',
  success: 'border-teal-300 bg-teal-50 text-teal-900',
  info: 'border-slate-300 bg-slate-50 text-slate-800',
}

export function StatusBanner({ kind, message }: StatusBannerProps) {
  return (
    <div
      role="status"
      className={`rounded-md border px-4 py-3 text-sm ${styles[kind]}`}
    >
      {message}
    </div>
  )
}
