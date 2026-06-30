export interface EnumItem {
  value: number
  label: string
}

export interface FoundationEnums {
  contractStatuses: EnumItem[]
  paymentPlanStatuses: EnumItem[]
  paymentFrequencies: EnumItem[]
  serviceTypes: EnumItem[]
  contractTypes: EnumItem[]
  customerIndustries: EnumItem[]
  contactTitles: EnumItem[]
}

export interface DashboardSummary {
  customerCount: number
  contactCount: number
  contractCount: number
  executingContractCount: number
  completedContractCount: number
  cancelledContractCount: number
  totalContractAmount: number
  paidAmount: number
  pendingAmount: number
  overduePaymentPlanCount: number
  contractsDueIn30DaysCount: number
}
