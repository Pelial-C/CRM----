export interface ApiResponse<T> {
  success: boolean
  message?: string | null
  data?: T | null
}

export interface PagedResultDto<T> {
  items: T[]
  totalCount: number
  pageIndex: number
  pageSize: number
}
