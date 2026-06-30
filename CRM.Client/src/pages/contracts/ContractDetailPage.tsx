import { useQuery } from '@tanstack/react-query'
import { Link, useParams } from 'react-router-dom'
import { useState } from 'react'
import { contractApi } from '../../api/contractApi'
import { foundationApi } from '../../api/foundationApi'
import ConfirmButton from '../../components/ConfirmButton'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import type { AddPaymentPlanInput, RecordPaymentInput } from '../../types/contract'
import { formatDate, todayString } from '../../utils/date'
import { contractStatusBadge, enumText, paymentStatusBadge } from '../../utils/enumText'
import { formatMoney } from '../../utils/money'

function ContractDetailPage() {
  const { id } = useParams()
  const contractId = Number(id)
  const [planForm, setPlanForm] = useState<AddPaymentPlanInput>({ planDate: todayString(), planAmount: 0, description: '' })
  const [recordPlanId, setRecordPlanId] = useState<number | null>(null)
  const [recordForm, setRecordForm] = useState<RecordPaymentInput>({ actualAmount: 0, actualDate: todayString() })
  const [error, setError] = useState('')

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

  const runAction = async (action: () => Promise<unknown>) => {
    setError('')
    try {
      await action()
      await contractQuery.refetch()
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : '操作失败')
    }
  }

  return (
    <>
      <PageHeader
        title="合同详情"
        actions={<Link className="btn btn-outline-primary" to={`/contracts/${contractId}/edit`}>编辑合同</Link>}
      />
      <ErrorMessage message={error || (contractQuery.error instanceof Error ? contractQuery.error.message : undefined)} />
      {contractQuery.isLoading || enums.isLoading ? <Loading /> : null}
      {contract ? (
        <>
          <div className="page-band mb-3">
            <div className="d-flex flex-wrap gap-2 mb-3">
              <ConfirmButton className="btn btn-success btn-sm" message="确认启动合同？" onConfirm={() => runAction(() => contractApi.start(contractId))}>启动合同</ConfirmButton>
              <ConfirmButton className="btn btn-danger btn-sm" message="确认作废合同？" onConfirm={() => runAction(() => contractApi.cancel(contractId))}>作废合同</ConfirmButton>
              <ConfirmButton className="btn btn-warning btn-sm" message="确认终止合同？" onConfirm={() => runAction(() => contractApi.terminate(contractId))}>终止合同</ConfirmButton>
              <button className="btn btn-outline-secondary btn-sm" type="button" onClick={() => runAction(() => contractApi.refreshOverdue(contractId))}>刷新逾期状态</button>
            </div>
            <div className="row g-3">
              <div className="col-md-3"><strong>合同编号：</strong>{contract.contractNo}</div>
              <div className="col-md-3"><strong>合同名称：</strong>{contract.contractName}</div>
              <div className="col-md-3"><strong>客户：</strong>{contract.customerName}</div>
              <div className="col-md-3"><strong>联系人：</strong>{contract.contactName ?? '-'}</div>
              <div className="col-md-3"><strong>合同金额：</strong>{formatMoney(contract.totalAmount)}</div>
              <div className="col-md-3"><strong>签订日期：</strong>{formatDate(contract.signDate)}</div>
              <div className="col-md-3"><strong>开始日期：</strong>{formatDate(contract.startDate)}</div>
              <div className="col-md-3"><strong>结束日期：</strong>{formatDate(contract.endDate)}</div>
              <div className="col-md-3">
                <strong>状态：</strong>
                <span className={`badge ${contractStatusBadge(contract.status)}`}>{enumText(enums.data?.contractStatuses, contract.status)}</span>
              </div>
              <div className="col-md-3"><strong>回款频率：</strong>{enumText(enums.data?.paymentFrequencies, contract.paymentFrequency)}</div>
              <div className="col-md-3"><strong>服务类型：</strong>{enumText(enums.data?.serviceTypes, contract.serviceType)}</div>
              <div className="col-md-3"><strong>合同类型：</strong>{enumText(enums.data?.contractTypes, contract.contractType)}</div>
            </div>
          </div>
          <div className="page-band mb-3">
            <h2 className="h5">合同明细</h2>
            <table className="table table-sm">
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
          <div className="page-band mb-3">
            <h2 className="h5">回款进度</h2>
            <div className="progress mb-2" role="progressbar" aria-valuenow={progress} aria-valuemin={0} aria-valuemax={100}>
              <div className="progress-bar" style={{ width: `${progress}%` }}>{progress.toFixed(0)}%</div>
            </div>
            <div className="text-secondary">已回款 {formatMoney(paidAmount)} / 合同金额 {formatMoney(contract.totalAmount)}</div>
          </div>
          <div className="page-band">
            <div className="d-flex justify-content-between align-items-center mb-2">
              <h2 className="h5 mb-0">回款计划</h2>
              <ConfirmButton className="btn btn-sm btn-outline-primary" message="确认自动生成回款计划？" onConfirm={() => runAction(() => contractApi.generatePaymentPlans(contractId))}>
                自动生成回款计划
              </ConfirmButton>
            </div>
            <form className="row g-2 mb-3" onSubmit={(event) => { event.preventDefault(); void runAction(() => contractApi.addPaymentPlan(contractId, planForm)) }}>
              <div className="col-md-3"><input className="form-control" type="date" value={planForm.planDate} onChange={(e) => setPlanForm({ ...planForm, planDate: e.target.value })} /></div>
              <div className="col-md-3"><input className="form-control" type="number" placeholder="计划金额" value={planForm.planAmount} onChange={(e) => setPlanForm({ ...planForm, planAmount: Number(e.target.value) })} /></div>
              <div className="col-md-4"><input className="form-control" placeholder="说明" value={planForm.description ?? ''} onChange={(e) => setPlanForm({ ...planForm, description: e.target.value })} /></div>
              <div className="col-md-2"><button className="btn btn-outline-primary w-100" type="submit">新增计划</button></div>
            </form>
            <table className="table table-sm">
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
            {recordPlanId ? (
              <form className="row g-2 mt-3" onSubmit={(event) => { event.preventDefault(); void runAction(async () => { await contractApi.recordPayment(contractId, recordPlanId, recordForm); setRecordPlanId(null) }) }}>
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
