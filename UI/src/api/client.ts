export class ApiError extends Error {
  status: number

  constructor(message: string, status: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

function getBaseUrl(): string {
  const base = import.meta.env.VITE_API_BASE_URL
  if (!base) {
    throw new Error('VITE_API_BASE_URL is not configured')
  }
  return base.replace(/\/$/, '')
}

async function readErrorMessage(response: Response): Promise<string> {
  const fallback =
    response.status === 404
      ? 'The requested resource was not found.'
      : `Request failed (${response.status}).`

  const text = await response.text()
  if (!text) {
    return fallback
  }

  try {
    const body: unknown = JSON.parse(text)
    if (typeof body === 'string' && body.trim()) {
      return body
    }

    if (body && typeof body === 'object') {
      const record = body as Record<string, unknown>
      if (typeof record.message === 'string' && record.message.trim()) {
        return record.message
      }

      if (record.errors && typeof record.errors === 'object') {
        const messages = Object.values(
          record.errors as Record<string, unknown>,
        ).flatMap((value) =>
          Array.isArray(value)
            ? value.filter((item): item is string => typeof item === 'string')
            : [],
        )
        if (messages.length > 0) {
          return messages[0]
        }
      }

      if (typeof record.title === 'string' && record.title.trim()) {
        return record.title
      }
    }
  } catch {
    if (text.trim()) {
      return text
    }
  }

  return fallback
}

export async function apiRequest<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const url = `${getBaseUrl()}${path.startsWith('/') ? path : `/${path}`}`

  let response: Response
  try {
    response = await fetch(url, {
      ...options,
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
        ...options.headers,
      },
    })
  } catch {
    throw new ApiError(
      'Could not reach the API. Check that the backend is running and CORS is enabled.',
      0,
    )
  }

  if (!response.ok) {
    const message = await readErrorMessage(response)
    throw new ApiError(message, response.status)
  }

  if (response.status === 204) {
    return undefined as T
  }

  const text = await response.text()
  if (!text) {
    return undefined as T
  }

  return JSON.parse(text) as T
}
