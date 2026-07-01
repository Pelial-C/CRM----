export interface OperationLogDto {
  id: number
  eventType: string
  entityName: string
  entityId: number
  description: string
  operatorName: string | null
  occurredAt: string
}

export interface OperationLogQuery {
  entityName?: string
  entityId?: number
  startDate?: string
  endDate?: string
  pageIndex: number
  pageSize: number
}
