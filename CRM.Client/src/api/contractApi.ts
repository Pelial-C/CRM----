import { apiDelete, apiGet, apiPost, apiPut } from './http'
import type { PagedResultDto } from '../types/common'
import type {
  AddPaymentPlanInput,
  ContactSelectDto,
  ContractDto,
  ContractInput,
  ContractQuery,
  RecordPaymentInput,
} from '../types/contract'

export const contractApi = {
  getList: (query: ContractQuery) => apiGet<PagedResultDto<ContractDto>>('/contracts', query),
  get: (id: number) => apiGet<ContractDto>(`/contracts/${id}`),
  create: (input: ContractInput) => apiPost<object>('/contracts', input),
  update: (id: number, input: ContractInput) => apiPut<object>(`/contracts/${id}`, input),
  remove: (id: number) => apiDelete<object>(`/contracts/${id}`),
  start: (id: number) => apiPost<object>(`/contracts/${id}/start`),
  cancel: (id: number, reason?: string) => apiPost<object>(`/contracts/${id}/cancel`, { reason }),
  terminate: (id: number, reason?: string) => apiPost<object>(`/contracts/${id}/terminate`, { reason }),
  refreshOverdue: (id: number) => apiPost<object>(`/contracts/${id}/refresh-overdue`),
  getCustomerContacts: (customerId: number) =>
    apiGet<ContactSelectDto[]>(`/contracts/customers/${customerId}/contacts`),
  generatePaymentPlans: (id: number) => apiPost<object>(`/contracts/${id}/payment-plans/generate`),
  addPaymentPlan: (id: number, input: AddPaymentPlanInput) =>
    apiPost<object>(`/contracts/${id}/payment-plans`, input),
  recordPayment: (id: number, planId: number, input: RecordPaymentInput) =>
    apiPost<object>(`/contracts/${id}/payment-plans/${planId}/record-payment`, input),
}
