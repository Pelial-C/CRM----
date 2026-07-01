import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { contractApi } from '../../api/contractApi'
import { dashboardApi } from '../../api/dashboardApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import EmptyState from '../../components/common/EmptyState'
import RiskBadge from '../../components/common/RiskBadge'
import StatusBadge from '../../components/common/StatusBadge'
import DashboardChartCard from '../../components/dashboard/DashboardChartCard'
import CashFlowChart from '../../components/dashboard/CashFlowChart'
import FeatureCard from '../../components/dashboard/FeatureCard'
import HeroSection from '../../components/dashboard/HeroSection'
import StatCard from '../../components/dashboard/StatCard'
import { getContractRisk } from '../../utils/contractVisual'
import { formatDate } from '../../utils/date'
import { formatMoney } from '../../utils/money'

const features = [
  ['K', '客户档案管理', '统一沉淀客户画像、联系人、合作地址和业务备注。', '/customers'],
  ['C', '合同全生命周期管理', '覆盖合同创建、编辑、状态流转、归档与终止。', '/contracts'],
  ['A', '合同审批流', '用可视化步骤追踪部门、法务、财务与最终批准。', '/approvals'],
  ['R', '风险预警', '聚合到期、逾期、异常金额与未完成审批风险。', '/risks'],
  ['P', '履约跟踪', '跟踪回款计划、逾期节点和履约进度。', '/contracts'],
  ['D', '数据统计分析', '以经营指标、状态分布和排行辅助管理决策。', '/reports'],
]

function DashboardPage() {
  const summaryQuery = useQuery({
    queryKey: ['dashboard-summary'],
    queryFn: dashboardApi.getSummary,
  })
  const recentContracts = useQuery({
    queryKey: ['dashboard-recent-contracts'],
    queryFn: () => contractApi.getList({ pageIndex: 1, pageSize: 6 }),
  })

  const summary = summaryQuery.data
  const riskContracts = recentContracts.data?.items
    .map((contract) => ({ contract, risk: getContractRisk(contract) }))
    .filter((item) => item.risk.level !== 'low')
    .slice(0, 4) ?? []

  return (
    <>
      <HeroSection summary={summary} />
      <ErrorMessage message={(summaryQuery.error ?? recentContracts.error) instanceof Error ? ((summaryQuery.error ?? recentContracts.error) as Error).message : undefined} />
      {summaryQuery.isLoading ? <Loading /> : null}

      <div className="row g-3 mb-4">
        <div className="col-12 col-sm-6 col-xl-3">
          <StatCard label="合同总数" value={summary?.contractCount ?? 0} hint="平台累计合同" icon="C" color="#0f3d75" />
        </div>
        <div className="col-12 col-sm-6 col-xl-3">
          <StatCard label="客户总数" value={summary?.customerCount ?? 0} hint={`${summary?.contactCount ?? 0} 位联系人`} icon="K" color="#0ea5e9" />
        </div>
        <div className="col-12 col-sm-6 col-xl-3">
          <StatCard label="待审批合同" value={summary?.executingContractCount ?? 0} hint="执行中和待跟进" icon="A" color="#2563eb" />
        </div>
        <div className="col-12 col-sm-6 col-xl-3">
          <StatCard label="高风险合同" value={summary?.overduePaymentPlanCount ?? 0} hint="逾期回款计划" icon="R" color="#dc2626" />
        </div>
        <div className="col-12 col-sm-6 col-xl-3">
          <StatCard label="即将到期合同" value={summary?.contractsDueIn30DaysCount ?? 0} hint="未来30天内" icon="T" color="#f59e0b" />
        </div>
        <div className="col-12 col-sm-6 col-xl-3">
          <StatCard label="已完成合同" value={summary?.completedContractCount ?? 0} hint="完成闭环合同" icon="F" color="#12a150" />
        </div>
        <div className="col-12 col-sm-6 col-xl-3">
          <StatCard label="合同总金额" value={formatMoney(summary?.totalContractAmount)} hint="累计签约金额" icon="M" color="#7c3aed" />
        </div>
        <div className="col-12 col-sm-6 col-xl-3">
          <StatCard label="待回款金额" value={formatMoney(summary?.pendingAmount)} hint="需持续跟踪" icon="P" color="#ea580c" />
        </div>
      </div>

      <section className="section-band mb-4">
        <div className="d-flex flex-wrap justify-content-between align-items-end gap-3 mb-3">
          <div>
            <h2 className="section-title">核心业务模块</h2>
            <p className="section-subtitle">围绕客户、合同、审批、风险和数据分析搭建统一工作台。</p>
          </div>
          <Link className="btn btn-outline-primary" to="/contracts/create">新建合同</Link>
        </div>
        <div className="row g-3">
          {features.map(([icon, title, description, to]) => (
            <div className="col-12 col-md-6 col-xl-4" key={title}>
              <FeatureCard icon={icon} title={title} description={description} to={to} />
            </div>
          ))}
        </div>
      </section>

      <div className="row g-3 mb-4">
        <div className="col-12 col-xl-7">
          <div className="crm-card">
            <div className="d-flex justify-content-between align-items-center mb-3">
              <div>
                <h2 className="section-title h5">最近合同</h2>
                <p className="section-subtitle">按合同创建与列表顺序展示最近业务记录。</p>
              </div>
              <Link className="btn btn-outline-primary btn-sm" to="/contracts">查看全部</Link>
            </div>
            {recentContracts.isLoading ? <Loading /> : null}
            {recentContracts.data?.items.length ? (
              <div className="responsive-table-wrap">
                <table className="table table-hover align-middle">
                  <thead>
                    <tr>
                      <th>合同名称</th>
                      <th>客户</th>
                      <th>金额</th>
                      <th>到期日</th>
                      <th>状态</th>
                    </tr>
                  </thead>
                  <tbody>
                    {recentContracts.data.items.map((contract) => (
                      <tr key={contract.id}>
                        <td><Link className="fw-semibold" to={`/contracts/${contract.id}`}>{contract.contractName}</Link></td>
                        <td>{contract.customerName}</td>
                        <td>{formatMoney(contract.totalAmount)}</td>
                        <td>{formatDate(contract.endDate)}</td>
                        <td><StatusBadge status={contract.status} endDate={contract.endDate} /></td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <EmptyState title="暂无最近合同" description="创建合同后会在此处展示最新业务动态。" />
            )}
          </div>
        </div>
        <div className="col-12 col-xl-5">
          <div className="crm-card">
            <div className="d-flex justify-content-between align-items-center mb-3">
              <div>
                <h2 className="section-title h5">风险提醒</h2>
                <p className="section-subtitle">优先处理逾期、即将到期和异常状态合同。</p>
              </div>
              <Link className="btn btn-outline-danger btn-sm" to="/risks">处理风险</Link>
            </div>
            {riskContracts.length ? (
              <div className="d-grid gap-3">
                {riskContracts.map(({ contract, risk }) => (
                  <div className="d-flex justify-content-between gap-3 p-3 rounded-3 border" key={contract.id}>
                    <div>
                      <Link className="fw-semibold" to={`/contracts/${contract.id}`}>{contract.contractName}</Link>
                      <div className="text-secondary small">{risk.reason} · {formatDate(contract.endDate)}</div>
                    </div>
                    <RiskBadge level={risk.level} />
                  </div>
                ))}
              </div>
            ) : (
              <EmptyState title="暂无高优先级风险" description="当前合同风险状态平稳。" />
            )}
          </div>
        </div>
      </div>

      <div className="row g-3">
        <div className="col-12 col-lg-6">
          <DashboardChartCard
            title="合同状态分布"
            description="基于现有合同汇总构建的轻量图表。"
            items={[
              { label: '执行中', value: summary?.executingContractCount ?? 0 },
              { label: '已完成', value: summary?.completedContractCount ?? 0 },
              { label: '已作废', value: summary?.cancelledContractCount ?? 0 },
              { label: '30天到期', value: summary?.contractsDueIn30DaysCount ?? 0 },
            ]}
          />
        </div>
        <div className="col-12 col-lg-6">
          <DashboardChartCard
            title="合同金额概览"
            description="展示累计金额、已回款和待回款规模。"
            items={[
              { label: '总金额', value: Math.round((summary?.totalContractAmount ?? 0) / 10000), suffix: '万' },
              { label: '已回款', value: Math.round((summary?.paidAmount ?? 0) / 10000), suffix: '万' },
              { label: '待回款', value: Math.round((summary?.pendingAmount ?? 0) / 10000), suffix: '万' },
            ]}
          />
        </div>
      </div>

      <div className="row g-3">
        <div className="col-12">
          <CashFlowChart />
        </div>
      </div>
    </>
  )
}

export default DashboardPage
