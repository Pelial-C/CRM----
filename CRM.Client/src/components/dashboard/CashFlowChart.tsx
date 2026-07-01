import { useQuery } from '@tanstack/react-query'
import { cashFlowApi } from '../../api/cashFlowApi'
import EmptyState from '../common/EmptyState'
import Loading from '../Loading'
import type { CashFlowForecastDto } from '../../types/cashFlow'

function formatAmount(value: number): string {
  if (value >= 10000) return `${(value / 10000).toFixed(1)}万`
  if (value >= 1000) return `${(value / 1000).toFixed(1)}k`
  return value.toFixed(0)
}

function CashFlowChart() {
  const { data, isLoading } = useQuery({
    queryKey: ['cashflow-forecast'],
    queryFn: () => cashFlowApi.getForecast(12),
  })

  const items = data ?? []

  // 计算最大值用于缩放
  const maxValue = Math.max(
    ...items.map(i => Math.max(i.planAmount, i.actualAmount)),
    1
  )

  // 汇总数据
  const totalPlan = items.reduce((sum, i) => sum + i.planAmount, 0)
  const totalActual = items.reduce((sum, i) => sum + i.actualAmount, 0)
  const overallRate = totalPlan > 0 ? (totalActual / totalPlan) * 100 : 0

  if (isLoading) return <Loading />

  if (items.length === 0) {
    return (
      <div className="crm-card">
        <h2 className="section-title h5 mb-3">现金流预测</h2>
        <EmptyState title="暂无回款数据" description="创建合同并制定回款计划后，此处将展示未来 12 个月的现金流预测。" />
      </div>
    )
  }

  return (
    <div className="crm-card">
      <div className="d-flex flex-wrap justify-content-between align-items-start gap-3 mb-3">
        <div>
          <h2 className="section-title h5 mb-1">现金流预测</h2>
          <p className="section-subtitle mb-0">未来 12 个月计划回款 vs 实际回款对比</p>
        </div>
        <div className="d-flex gap-3 align-items-center">
          <span className="d-flex align-items-center gap-1 small">
            <span style={{ width: 12, height: 12, borderRadius: 2, backgroundColor: '#2563eb', display: 'inline-block' }} />
            计划回款
          </span>
          <span className="d-flex align-items-center gap-1 small">
            <span style={{ width: 12, height: 12, borderRadius: 2, backgroundColor: '#059669', display: 'inline-block' }} />
            实际回款
          </span>
        </div>
      </div>

      {/* 汇总统计 */}
      <div className="row g-3 mb-4">
        <div className="col-4">
          <div className="text-center p-2 rounded-3" style={{ backgroundColor: '#eff6ff' }}>
            <div className="text-muted small">计划总额</div>
            <div className="fw-bold" style={{ color: '#2563eb' }}>¥{formatAmount(totalPlan)}</div>
          </div>
        </div>
        <div className="col-4">
          <div className="text-center p-2 rounded-3" style={{ backgroundColor: '#ecfdf5' }}>
            <div className="text-muted small">实际总额</div>
            <div className="fw-bold" style={{ color: '#059669' }}>¥{formatAmount(totalActual)}</div>
          </div>
        </div>
        <div className="col-4">
          <div className="text-center p-2 rounded-3" style={{ backgroundColor: overallRate >= 80 ? '#ecfdf5' : overallRate >= 50 ? '#fef3c7' : '#fee2e2' }}>
            <div className="text-muted small">达成率</div>
            <div className="fw-bold" style={{ color: overallRate >= 80 ? '#059669' : overallRate >= 50 ? '#d97706' : '#dc2626' }}>
              {overallRate.toFixed(0)}%
            </div>
          </div>
        </div>
      </div>

      {/* 柱状图 */}
      <div className="cashflow-chart">
        <div className="cashflow-bars">
          {items.map((item) => {
            const planHeight = (item.planAmount / maxValue) * 100
            const actualHeight = (item.actualAmount / maxValue) * 100
            return (
              <div className="cashflow-bar-group" key={item.monthLabel}>
                <div className="cashflow-bar-pair">
                  <div
                    className="cashflow-bar cashflow-bar-plan"
                    style={{ height: `${Math.max(planHeight, 2)}%` }}
                    title={`计划: ¥${formatAmount(item.planAmount)}`}
                  />
                  <div
                    className="cashflow-bar cashflow-bar-actual"
                    style={{ height: `${Math.max(actualHeight, 2)}%` }}
                    title={`实际: ¥${formatAmount(item.actualAmount)}`}
                  />
                </div>
                <div className="cashflow-bar-label">{item.month}月</div>
              </div>
            )
          })}
        </div>
      </div>

      {/* 月度明细表 */}
      <div className="responsive-table-wrap mt-3">
        <table className="table table-sm align-middle mb-0">
          <thead>
            <tr>
              <th>月份</th>
              <th className="text-end">计划回款</th>
              <th className="text-end">实际回款</th>
              <th className="text-end">差额</th>
              <th className="text-end">达成率</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => {
              const rate = item.achievementRate * 100
              const rateColor = rate >= 80 ? '#059669' : rate >= 50 ? '#d97706' : '#dc2626'
              return (
                <tr key={item.monthLabel}>
                  <td>{item.monthLabel}</td>
                  <td className="text-end">¥{formatAmount(item.planAmount)}</td>
                  <td className="text-end">¥{formatAmount(item.actualAmount)}</td>
                  <td className="text-end" style={{ color: item.difference >= 0 ? '#059669' : '#dc2626' }}>
                    {item.difference >= 0 ? '+' : ''}{formatAmount(item.difference)}
                  </td>
                  <td className="text-end">
                    <span className="badge" style={{ backgroundColor: rateColor, color: '#fff' }}>
                      {rate.toFixed(0)}%
                    </span>
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default CashFlowChart
