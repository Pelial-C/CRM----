export interface CashFlowForecastDto {
  year: number
  month: number
  monthLabel: string
  planAmount: number
  actualAmount: number
  difference: number
  achievementRate: number
}
