import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { contractApi } from '../../api/contractApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import EmptyState from '../../components/common/EmptyState'
import RiskBadge from '../../components/common/RiskBadge'
import StatCard from '../../components/dashboard/StatCard'
import { getContractRisk } from '../../utils/contractVisual'
import { formatDate } from '../../utils/date'
import { formatMoney } from '../../utils/money'

function RiskPage() {
  const contracts = useQuery({
    queryKey: ['risk-contracts'],
    queryFn: () => contractApi.getList({ pageIndex: 1, pageSize: 100 }),
  })

  const items = contracts.data?.items ?? []
  const riskItems = items.map((contract) => ({ contract, risk: getContractRisk(contract) }))
  const highRisk = riskItems.filter((item) => item.risk.level === 'high')
  const dueSoon = riskItems.filter((item) => item.risk.level === 'medium')
  const unfinished = items.filter((contract) => contract.status === 0 || contract.status === 1)

  return (
    <>
      <PageHeader
        title="风险预警"
        description="集中识别高风险合同、即将到期合同、金额异常、未完成审批和履约逾期事项。"
        actions={<Link className="btn btn-outline-primary" to="/contracts">进入合同列表</Link>}
      />
      <ErrorMessage message={contracts.error instanceof Error ? contracts.error.message : undefined} />
      {contracts.isLoading ? <Loading /> : null}

      <div className="row g-3 mb-3">
        <div className="col-12 col-sm-6 col-xl-3"><StatCard label="高风险合同" value={highRisk.length} hint="逾期、作废或终止" icon="H" color="#dc2626" /></div>
        <div className="col-12 col-sm-6 col-xl-3"><StatCard label="即将到期合同" value={dueSoon.length} hint="未来30天内" icon="D" color="#f59e0b" /></div>
        <div className="col-12 col-sm-6 col-xl-3"><StatCard label="未完成审批合同" value={unfinished.length} hint="草稿或执行中" icon="A" color="#2563eb" /></div>
        <div className="col-12 col-sm-6 col-xl-3"><StatCard label="履约逾期合同" value={highRisk.length} hint="需优先处理" icon="O" color="#b91c1c" /></div>
      </div>

      <div className="row g-3">
        {[
          ['高风险合同', highRisk, '立即查看'],
          ['即将到期合同提醒', dueSoon, '续约跟进'],
          ['金额异常合同', riskItems.filter((item) => item.contract.totalAmount >= 100000), '复核金额'],
          ['未完成审批合同', riskItems.filter((item) => item.contract.status === 0), '推动审批'],
          ['履约逾期合同', highRisk, '登记处理'],
        ].map(([title, list, action]) => (
          <div className="col-12 col-xl-6" key={String(title)}>
            <div className="crm-card">
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h2 className="section-title h5">{String(title)}</h2>
                <span className="badge text-bg-light">{(list as typeof riskItems).length} 项</span>
              </div>
              {(list as typeof riskItems).length ? (
                <div className="d-grid gap-3">
                  {(list as typeof riskItems).slice(0, 5).map(({ contract, risk }) => (
                    <div className="d-flex flex-wrap justify-content-between gap-3 p-3 border rounded-3" key={`${title}-${contract.id}`}>
                      <div>
                        <Link className="fw-semibold" to={`/contracts/${contract.id}`}>{contract.contractName}</Link>
                        <div className="text-secondary small">{contract.customerName} · {formatMoney(contract.totalAmount)} · 到期 {formatDate(contract.endDate)}</div>
                        <div className="text-secondary small">{risk.reason}</div>
                      </div>
                      <div className="d-flex align-items-center gap-2">
                        <RiskBadge level={risk.level} />
                        <Link className="btn btn-outline-primary btn-sm" to={`/contracts/${contract.id}`}>{String(action)}</Link>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <EmptyState title="暂无风险项" description="当前分类没有需要处理的合同。" />
              )}
            </div>
          </div>
        ))}
      </div>
    </>
  )
}

export default RiskPage
