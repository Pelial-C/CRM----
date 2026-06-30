import type { ContactDto } from './contact'

export interface CustomerDto {
  id: number
  name?: string | null
  creditCode?: string | null
  industry?: string | null
  remark?: string | null
  isDeleted: boolean
  province?: string | null
  city?: string | null
  district?: string | null
  detailAddress?: string | null
  creationTime: string
  contacts: ContactDto[]
}

export interface CustomerSelectDto {
  id: number
  name: string
}

export interface CustomerInput {
  name: string
  creditCode?: string
  industry?: string
  province?: string
  city?: string
  district?: string
  detailAddress?: string
  remark?: string
}

export interface CustomerQuery {
  keyword?: string
  industry?: string
  includeDeleted?: boolean
}
