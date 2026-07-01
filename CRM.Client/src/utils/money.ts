export function formatMoney(value?: number | null) {
  return `￥${Number(value ?? 0).toFixed(2)}`
}
