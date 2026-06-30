import { useQuery } from '@tanstack/react-query'
import { dashboardApi } from '../../api/dashboardApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import { formatMoney } from '../../utils/money'

function DashboardPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['dashboard-summary'],
    queryFn: dashboardApi.getSummary,
  })

  const metrics = data
    ? [
        ['客户总数', data.customerCount],
        ['合同总数', data.contractCount],
        ['执行中合同', data.executingContractCount],
        ['已完成合同', data.completedContractCount],
        ['已作废合同', data.cancelledContractCount],
        ['合同总金额', formatMoney(data.totalContractAmount)],
        ['已回款金额', formatMoney(data.paidAmount)],
        ['待回款金额', formatMoney(data.pendingAmount)],
        ['逾期回款计划', data.overduePaymentPlanCount],
        ['未来30天到期合同', data.contractsDueIn30DaysCount],
      ]
    : []

  return (
    <>
      <PageHeader title="统计看板" description="客户、合同与回款关键指标" />
      <ErrorMessage message={error instanceof Error ? error.message : undefined} />
      {isLoading ? <Loading /> : null}
      <div className="row g-3">
        {metrics.map(([label, value]) => (
          <div className="col-12 col-sm-6 col-lg-3" key={label}>
            <div className="metric">
              <div className="metric-label">{label}</div>
              <div className="metric-value">{value}</div>
            </div>
          </div>
        ))}
      </div>
    </>
  )
}

export default DashboardPage
