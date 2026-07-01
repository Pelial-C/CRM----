import { useQuery } from '@tanstack/react-query'
import { Link, useParams } from 'react-router-dom'
import { useState } from 'react'
import { contractApi } from '../../api/contractApi'
import { foundationApi } from '../../api/foundationApi'
import ApprovalTimeline from '../../components/approvals/ApprovalTimeline'
import ConfirmButton from '../../components/ConfirmButton'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import EmptyState from '../../components/common/EmptyState'
import RiskBadge from '../../components/common/RiskBadge'
import StatusBadge from '../../components/common/StatusBadge'
import type { AddPaymentPlanInput, RecordPaymentInput } from '../../types/contract'
import { getContractRisk } from '../../utils/contractVisual'
import { formatDate, todayString } from '../../utils/date'
import { enumText, paymentStatusBadge } from '../../utils/enumText'
import { formatMoney } from '../../utils/money'

function ContractDetailPage() {
  const { id } = useParams()
  const contractId = Number(id)
  const [planForm, setPlanForm] = useState<AddPaymentPlanInput>({ planDate: todayString(), planAmount: 0, description: '' })
  const [recordPlanId, setRecordPlanId] = useState<number | null>(null)
  const [recordForm, setRecordForm] = useState<RecordPaymentInput>({ actualAmount: 0, actualDate: todayString() })
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  const enums = useQuery({ queryKey: ['foundation-enums'], queryFn: foundationApi.getEnums })
  const contractQuery = useQuery({
    queryKey: ['contract', contractId],
    queryFn: async () => {
      await contractApi.refreshOverdue(contractId).catch(() => undefined)
      return contractApi.get(contractId)
    },
    enabled: contractId > 0,
  })

  const contract = contractQuery.data
  const paidAmount = contract?.paymentPlans.reduce((sum, plan) => sum + plan.actualAmount, 0) ?? 0
  const progress = contract && contract.totalAmount > 0 ? Math.min(100, (paidAmount / contract.totalAmount) * 100) : 0
  const risk = contract ? getContractRisk(contract) : null

  const runAction = async (message: string, action: () => Promise<unknown>) => {
    setError('')
    setSuccess('')
    try {
      await action()
      await contractQuery.refetch()
      setSuccess(message)
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : '操作失败')
    }
  }

  return (
    <>
      <PageHeader
        title="合同详情"
        description="查看合同基础信息、审批流程、风险提示、回款计划和操作记录。"
        actions={<Link className="btn btn-outline-primary" to={`/contracts/${contractId}/edit`}>编辑合同</Link>}
      />
      <ErrorMessage message={error || (contractQuery.error instanceof Error ? contractQuery.error.message : undefined)} />
      {success ? <div className="alert alert-success">{success}</div> : null}
      {contractQuery.isLoading || enums.isLoading ? <Loading /> : null}
      {contract ? (
        <>
          <div className="section-band mb-3">
            <div className="d-flex flex-wrap justify-content-between align-items-start gap-3 mb-3">
              <div>
                <h2 className="section-title">{contract.contractName}</h2>
                <p className="section-subtitle">{contract.contractNo} · {contract.customerName}</p>
              </div>
              <div className="d-flex flex-wrap gap-2">
                <StatusBadge status={contract.status} label={enumText(enums.data?.contractStatuses, contract.status)} endDate={contract.endDate} />
                {risk ? <RiskBadge level={risk.level} /> : null}
              </div>
            </div>
            <div className="action-bar mb-3">
              <ConfirmButton className="btn btn-success btn-sm" message="确认启动合同？" onConfirm={() => runAction('合同已启动。', () => contractApi.start(contractId))}>启动合同</ConfirmButton>
              <ConfirmButton className="btn btn-dark btn-sm" message="确认归档合同？" onConfirm={() => runAction('合同已归档。', () => contractApi.cancel(contractId))}>归档合同</ConfirmButton>
              <ConfirmButton className="btn btn-warning btn-sm" message="确认终止合同？" onConfirm={() => runAction('合同已终止。', () => contractApi.terminate(contractId))}>终止合同</ConfirmButton>
              <button className="btn btn-outline-secondary btn-sm" type="button" onClick={() => runAction('逾期状态已刷新。', () => contractApi.refreshOverdue(contractId))}>刷新逾期状态</button>
            </div>
            <div className="detail-grid">
              <div className="detail-item"><div className="detail-label">合同金额</div><div className="detail-value">{formatMoney(contract.totalAmount)}</div></div>
              <div className="detail-item"><div className="detail-label">签署日期</div><div className="detail-value">{formatDate(contract.signDate)}</div></div>
              <div className="detail-item"><div className="detail-label">开始日期</div><div className="detail-value">{formatDate(contract.startDate)}</div></div>
              <div className="detail-item"><div className="detail-label">到期日期</div><div className="detail-value">{formatDate(contract.endDate)}</div></div>
              <div className="detail-item"><div className="detail-label">联系人</div><div className="detail-value">{contract.contactName ?? '-'}</div></div>
              <div className="detail-item"><div className="detail-label">回款频率</div><div className="detail-value">{enumText(enums.data?.paymentFrequencies, contract.paymentFrequency)}</div></div>
              <div className="detail-item"><div className="detail-label">服务类型</div><div className="detail-value">{enumText(enums.data?.serviceTypes, contract.serviceType)}</div></div>
              <div className="detail-item"><div className="detail-label">合同类型</div><div className="detail-value">{enumText(enums.data?.contractTypes, contract.contractType)}</div></div>
            </div>
          </div>

          <div className="row g-3 mb-3">
            <div className="col-12 col-xl-8">
              <div className="crm-card">
                <h2 className="section-title h5 mb-3">审批流程</h2>
                <ApprovalTimeline />
              </div>
            </div>
            <div className="col-12 col-xl-4">
              <div className="crm-card">
                <h2 className="section-title h5 mb-3">风险提示</h2>
                {risk ? (
                  <>
                    <RiskBadge level={risk.level} />
                    <p className="text-secondary mt-3 mb-0">{risk.reason}</p>
                  </>
                ) : (
                  <EmptyState title="暂无风险" description="当前合同没有识别到异常风险。" />
                )}
              </div>
            </div>
          </div>

          <div className="row g-3 mb-3">
            <div className="col-12 col-xl-7">
              <div className="page-band h-100">
                <h2 className="section-title h5 mb-3">合同明细</h2>
                <div className="responsive-table-wrap">
                  <table className="table table-sm align-middle">
                    <thead><tr><th>产品/服务名称</th><th>数量</th><th>单价</th><th>小计</th></tr></thead>
                    <tbody>
                      {contract.items.map((item, index) => (
                        <tr key={item.id ?? index}>
                          <td>{item.productName}</td>
                          <td>{item.quantity}</td>
                          <td>{formatMoney(item.unitPrice)}</td>
                          <td>{formatMoney(item.subtotal)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
            <div className="col-12 col-xl-5">
              <div className="page-band h-100">
                <h2 className="section-title h5 mb-3">回款进度</h2>
                <div className="progress mb-2" role="progressbar" aria-valuenow={progress} aria-valuemin={0} aria-valuemax={100}>
                  <div className="progress-bar" style={{ width: `${progress}%` }}>{progress.toFixed(0)}%</div>
                </div>
                <div className="text-secondary">已回款 {formatMoney(paidAmount)} / 合同金额 {formatMoney(contract.totalAmount)}</div>
              </div>
            </div>
          </div>

          <div className="page-band">
            <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
              <div>
                <h2 className="section-title h5">回款计划</h2>
                <p className="section-subtitle">登记计划金额、实际回款和逾期状态。</p>
              </div>
              <ConfirmButton className="btn btn-sm btn-outline-primary" message="确认自动生成回款计划？" onConfirm={() => runAction('回款计划已生成。', () => contractApi.generatePaymentPlans(contractId))}>
                自动生成回款计划
              </ConfirmButton>
            </div>
            <form className="row g-2 mb-3" onSubmit={(event) => { event.preventDefault(); void runAction('回款计划已新增。', () => contractApi.addPaymentPlan(contractId, planForm)) }}>
              <div className="col-md-3"><input className="form-control" aria-label="计划日期" type="date" value={planForm.planDate} onChange={(e) => setPlanForm({ ...planForm, planDate: e.target.value })} /></div>
              <div className="col-md-3"><input className="form-control" aria-label="计划金额" type="number" placeholder="计划金额" value={planForm.planAmount} onChange={(e) => setPlanForm({ ...planForm, planAmount: Number(e.target.value) })} /></div>
              <div className="col-md-4"><input className="form-control" aria-label="计划说明" placeholder="说明" value={planForm.description ?? ''} onChange={(e) => setPlanForm({ ...planForm, description: e.target.value })} /></div>
              <div className="col-md-2"><button className="btn btn-outline-primary w-100" type="submit">新增计划</button></div>
            </form>
            <div className="responsive-table-wrap">
              <table className="table table-sm align-middle">
                <thead><tr><th>计划日期</th><th>计划金额</th><th>实际金额</th><th>实际回款日期</th><th>状态</th><th>说明</th><th>操作</th></tr></thead>
                <tbody>
                  {contract.paymentPlans.map((plan) => (
                    <tr key={plan.id}>
                      <td>{formatDate(plan.planDate)}</td>
                      <td>{formatMoney(plan.planAmount)}</td>
                      <td>{formatMoney(plan.actualAmount)}</td>
                      <td>{formatDate(plan.actualDate)}</td>
                      <td><span className={`badge ${paymentStatusBadge(plan.status)}`}>{enumText(enums.data?.paymentPlanStatuses, plan.status)}</span></td>
                      <td>{plan.description ?? '-'}</td>
                      <td><button className="btn btn-sm btn-outline-success" type="button" onClick={() => setRecordPlanId(plan.id)}>登记回款</button></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {recordPlanId ? (
              <form className="row g-2 mt-3" onSubmit={(event) => { event.preventDefault(); void runAction('回款已登记。', async () => { await contractApi.recordPayment(contractId, recordPlanId, recordForm); setRecordPlanId(null) }) }}>
                <div className="col-md-3"><input className="form-control" type="number" placeholder="实际回款金额" value={recordForm.actualAmount} onChange={(e) => setRecordForm({ ...recordForm, actualAmount: Number(e.target.value) })} /></div>
                <div className="col-md-3"><input className="form-control" type="date" value={recordForm.actualDate} onChange={(e) => setRecordForm({ ...recordForm, actualDate: e.target.value })} /></div>
                <div className="col-md-2"><button className="btn btn-success w-100" type="submit">确认登记</button></div>
                <div className="col-md-2"><button className="btn btn-outline-secondary w-100" type="button" onClick={() => setRecordPlanId(null)}>取消</button></div>
              </form>
            ) : null}
          </div>
        </>
      ) : null}
    </>
  )
}

export default ContractDetailPage
