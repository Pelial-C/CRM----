export function formatDate(value?: string | null) {
  if (!value) return '-'
  return value.slice(0, 10)
}

export function todayString() {
  return new Date().toISOString().slice(0, 10)
}
