import type { EnumItem } from '../types/foundation'

export function enumText(items: EnumItem[] | undefined, value?: number | null) {
  return items?.find((item) => item.value === value)?.label ?? '-'
}

export function contractStatusBadge(status: number) {
  if (status === 1) return 'text-bg-primary'
  if (status === 2) return 'text-bg-success'
  if (status === 3) return 'text-bg-danger'
  if (status === 4) return 'text-bg-secondary'
  return 'text-bg-warning'
}

export function paymentStatusBadge(status: number) {
  if (status === 2) return 'text-bg-success'
  if (status === 3) return 'text-bg-danger'
  if (status === 1) return 'text-bg-info'
  return 'text-bg-warning'
}
