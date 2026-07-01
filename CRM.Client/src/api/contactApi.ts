import { apiDelete, apiGet, apiPut } from './http'
import type { ContactDto, ContactInput } from '../types/contact'

export const contactApi = {
  get: (id: number) => apiGet<ContactDto>(`/contacts/${id}`),
  update: (id: number, input: ContactInput) => apiPut<object>(`/contacts/${id}`, input),
  remove: (id: number) => apiDelete<object>(`/contacts/${id}`),
}
