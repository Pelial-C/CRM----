export interface ContactDto {
  id: number
  customerId: number
  name: string
  title?: string | null
  phone?: string | null
  email?: string | null
  isKeyDecisionMaker: boolean
}

export interface ContactInput {
  name: string
  title?: string
  phone?: string
  email?: string
  isKeyDecisionMaker: boolean
}
