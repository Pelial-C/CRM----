import { apiGet } from './http'
import type { CashFlowForecastDto } from '../types/cashFlow'

export const cashFlowApi = {
  getForecast: (months = 12) =>
    apiGet<CashFlowForecastDto[]>('/dashboard/cashflow-forecast', { months }),
}
