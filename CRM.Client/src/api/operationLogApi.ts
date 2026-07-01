import { apiGet } from './http'
import type { OperationLogDto, OperationLogQuery } from '../types/operationLog'
import type { PagedResultDto } from '../types/common'

export const operationLogApi = {
  getList: (query: OperationLogQuery) =>
    apiGet<PagedResultDto<OperationLogDto>>('/operation-logs', query),
}
