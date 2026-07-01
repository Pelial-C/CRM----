import type { ContractDto } from '../types/contract'

export type RiskLevel = 'low' | 'medium' | 'high'

export function getContractVisualStatus(status: number, label?: string, endDate?: string | null) {
  const today = new Date()
  const due = endDate ? new Date(endDate) : null
  const daysToDue = due ? Math.ceil((due.getTime() - today.getTime()) / 86400000) : Number.POSITIVE_INFINITY

  if (status === 0) return { label: label && label !== '-' ? label : '草稿', className: 'status-draft' }
  if (status === 3) return { label: label && label !== '-' ? label : '已归档', className: 'status-cancelled' }
  if (status === 4) return { label: label && label !== '-' ? label : '已终止', className: 'status-terminated' }
  if (status === 2) return { label: label && label !== '-' ? label : '已签署', className: 'status-completed' }
  if (Number.isFinite(daysToDue) && daysToDue >= 0 && daysToDue <= 30) {
    return { label: '即将到期', className: 'status-due' }
  }
  if (status === 1) return { label: label && label !== '-' ? label : '履约中', className: 'status-executing' }

  return { label: label && label !== '-' ? label : '审批中', className: 'status-review' }
}

export function getContractRisk(contract: Pick<ContractDto, 'status' | 'endDate' | 'paymentPlans'>): {
  level: RiskLevel
  label: string
  reason: string
} {
  const today = new Date()
  const endDate = new Date(contract.endDate)
  const daysToDue = Math.ceil((endDate.getTime() - today.getTime()) / 86400000)
  const overduePlans = contract.paymentPlans?.filter((plan) => plan.status === 3).length ?? 0

  if (contract.status === 3 || contract.status === 4 || overduePlans > 0) {
    return { level: 'high', label: '高风险', reason: overduePlans > 0 ? '存在逾期回款计划' : '合同已作废或终止' }
  }

  if (daysToDue >= 0 && daysToDue <= 30) {
    return { level: 'medium', label: '中风险', reason: '合同即将到期' }
  }

  return { level: 'low', label: '低风险', reason: '暂无异常风险' }
}
