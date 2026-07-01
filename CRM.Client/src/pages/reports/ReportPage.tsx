import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { contractApi } from '../../api/contractApi'
import { dashboardApi } from '../../api/dashboardApi'
import DashboardChartCard from '../../components/dashboard/DashboardChartCard'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import StatusBadge from '../../components/common/StatusBadge'
import StatCard from '../../components/dashboard/StatCard'
import { formatDate } from '../../utils/date'
import { formatMoney } from '../../utils/money'

function ReportPage() {
  const summary = useQuery({ queryKey: ['reports-summary'], queryFn: dashboardApi.getSummary })
  const contracts = useQuery({
    queryKey: ['reports-contracts'],
    queryFn: () => contractApi.getList({ pageIndex: 1, pageSize: 100 }),
  })

  const items = contracts.data?.items ?? []
  const customerRanks = Object.entries(
    items.reduce<Record<string, number>>((result, contract) => {
      const key = contract.customerName ?? '未命名客户'
      result[key] = (result[key] ?? 0) + 1
      return result
    }, {}),
  )
    .map(([label, value]) => ({ label, value }))
    .sort((a, b) => b.value - a.value)
    .slice(0, 5)

  return (
    <>
      <PageHeader
        title="数据报表"
        description="用统计卡片、趋势图、状态分布和客户排行辅助合同经营分析。"
        actions={<button className="btn btn-outline-primary" type="button" onClick={() => window.print()}>导出报表</button>}
      />
      <ErrorMessage message={(summary.error ?? contracts.error) instanceof Error ? ((summary.error ?? contracts.error) as Error).message : undefined} />
      {summary.isLoading || contracts.isLoading ? <Loading /> : null}

      <div className="row g-3 mb-3">
        <div className="col-12 col-sm-6 col-xl-3"><StatCard label="合同总数" value={summary.data?.contractCount ?? 0} icon="C" /></div>
        <div className="col-12 col-sm-6 col-xl-3"><StatCard label="客户总数" value={summary.data?.customerCount ?? 0} icon="K" color="#0ea5e9" /></div>
        <div className="col-12 col-sm-6 col-xl-3"><StatCard label="合同总金额" value={formatMoney(summary.data?.totalContractAmount)} icon="M" color="#12a150" /></div>
        <div className="col-12 col-sm-6 col-xl-3"><StatCard label="风险合同占比" value={`${summary.data?.contractCount ? Math.round(((summary.data?.overduePaymentPlanCount ?? 0) / summary.data.contractCount) * 100) : 0}%`} icon="R" color="#dc2626" /></div>
      </div>

      <div className="row g-3 mb-3">
        <div className="col-12 col-xl-6">
          <DashboardChartCard
            title="月度合同新增趋势"
            description="当前接口未提供月度统计，先使用最近合同创建月份聚合，后续可替换为报表接口。"
            items={Object.entries(items.reduce<Record<string, number>>((result, contract) => {
              const month = contract.creationTime?.slice(5, 7) ? `${contract.creationTime.slice(5, 7)}月` : '未知'
              result[month] = (result[month] ?? 0) + 1
              return result
            }, {})).map(([label, value]) => ({ label, value }))}
          />
        </div>
        <div className="col-12 col-xl-6">
          <DashboardChartCard
            title="客户合同数量排行"
            description="按客户关联合同数展示 Top 5。"
            items={customerRanks.length ? customerRanks : [{ label: '暂无客户', value: 0 }]}
          />
        </div>
        <div className="col-12 col-xl-6">
          <DashboardChartCard
            title="合同金额统计"
            items={[
              { label: '总金额', value: Math.round((summary.data?.totalContractAmount ?? 0) / 10000), suffix: '万' },
              { label: '已回款', value: Math.round((summary.data?.paidAmount ?? 0) / 10000), suffix: '万' },
              { label: '待回款', value: Math.round((summary.data?.pendingAmount ?? 0) / 10000), suffix: '万' },
            ]}
          />
        </div>
        <div className="col-12 col-xl-6">
          <DashboardChartCard
            title="合同状态分布"
            items={[
              { label: '执行中', value: summary.data?.executingContractCount ?? 0 },
              { label: '已完成', value: summary.data?.completedContractCount ?? 0 },
              { label: '已作废', value: summary.data?.cancelledContractCount ?? 0 },
              { label: '30天到期', value: summary.data?.contractsDueIn30DaysCount ?? 0 },
            ]}
          />
        </div>
      </div>

      <div className="page-band">
        <h2 className="section-title h5 mb-3">最近数据表</h2>
        <div className="responsive-table-wrap">
          <table className="table table-hover align-middle">
            <thead><tr><th>合同名称</th><th>客户</th><th>金额</th><th>到期日</th><th>状态</th><th>操作</th></tr></thead>
            <tbody>
              {items.slice(0, 8).map((contract) => (
                <tr key={contract.id}>
                  <td>{contract.contractName}</td>
                  <td>{contract.customerName}</td>
                  <td>{formatMoney(contract.totalAmount)}</td>
                  <td>{formatDate(contract.endDate)}</td>
                  <td><StatusBadge status={contract.status} endDate={contract.endDate} /></td>
                  <td><Link className="btn btn-outline-primary btn-sm" to={`/contracts/${contract.id}`}>详情</Link></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

export default ReportPage
