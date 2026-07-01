import { apiGet } from './http'
import type { DashboardSummary } from '../types/foundation'

export const dashboardApi = {
  getSummary: () => apiGet<DashboardSummary>('/dashboard/summary'),
}
