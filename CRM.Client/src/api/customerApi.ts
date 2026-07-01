import { apiDelete, apiGet, apiPost, apiPut } from './http'
import type { ContactDto, ContactInput } from '../types/contact'
import type { CustomerDto, CustomerEvaluationDto, CustomerInput, CustomerQuery, CustomerSelectDto } from '../types/customer'

export const customerApi = {
  getList: (query: CustomerQuery) => apiGet<CustomerDto[]>('/customers', query),
  getSelectList: () => apiGet<CustomerSelectDto[]>('/customers/select-list'),
  get: (id: number) => apiGet<CustomerDto>(`/customers/${id}`),
  create: (input: CustomerInput) => apiPost<object>('/customers', input),
  update: (id: number, input: CustomerInput) => apiPut<object>(`/customers/${id}`, input),
  remove: (id: number) => apiDelete<object>(`/customers/${id}`),
  evaluate: (id: number) => apiGet<CustomerEvaluationDto>(`/customers/${id}/evaluation`),
  getContacts: (customerId: number) => apiGet<ContactDto[]>(`/customers/${customerId}/contacts`),
  createContact: (customerId: number, input: ContactInput) =>
    apiPost<ContactDto>(`/customers/${customerId}/contacts`, input),
}
