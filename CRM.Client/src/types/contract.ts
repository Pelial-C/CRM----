export interface ContractItemDto {
  id?: number
  contractId?: number
  productName?: string | null
  quantity: number
  unitPrice: number
  subtotal: number
}

export interface PaymentPlanDto {
  id: number
  contractId: number
  planDate: string
  planAmount: number
  actualAmount: number
  actualDate?: string | null
  status: number
  description?: string | null
}

export interface ContractDto {
  id: number
  contractNo?: string | null
  contractName?: string | null
  cabinetNo?: string | null
  signDate: string
  startDate: string
  endDate: string
  totalAmount: number
  status: number
  customerId: number
  customerName?: string | null
  contactId?: number | null
  contactName?: string | null
  paymentFrequency: number
  serviceType: number
  contractType: number
  warningDays: number
  regionalCompany?: string | null
  affiliatedCompany?: string | null
  creationTime: string
  remark?: string | null
  items: ContractItemDto[]
  paymentPlans: PaymentPlanDto[]
}

export interface ContactSelectDto {
  contactId: number
  contactName?: string | null
  phone?: string | null
  email?: string | null
}

export interface ContractInput {
  contractNo: string
  contractName: string
  cabinetNo?: string
  customerId: number
  contactId?: number | null
  signDate: string
  startDate: string
  endDate: string
  totalAmount: number
  paymentFrequency: number
  serviceType: number
  contractType: number
  warningDays: number
  regionalCompany?: string
  affiliatedCompany?: string
  remark?: string
  items: ContractItemDto[]
}

export interface ContractQuery {
  keyword?: string
  status?: number | ''
  customerId?: number
  pageIndex: number
  pageSize: number
}

export interface AddPaymentPlanInput {
  planDate: string
  planAmount: number
  description?: string
}

export interface RecordPaymentInput {
  actualAmount: number
  actualDate: string
}
