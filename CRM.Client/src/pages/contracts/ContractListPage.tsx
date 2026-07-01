import { useQuery, useQueryClient } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { useMemo, useState } from 'react'
import { contractApi } from '../../api/contractApi'
import { foundationApi } from '../../api/foundationApi'
import ContractTable from '../../components/contracts/ContractTable'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import { getContractRisk, type RiskLevel } from '../../utils/contractVisual'

function ContractListPage() {
  const queryClient = useQueryClient()
  const [keyword, setKeyword] = useState('')
  const [customerName, setCustomerName] = useState('')
  const [status, setStatus] = useState<number | ''>('')
  const [riskLevel, setRiskLevel] = useState<RiskLevel | ''>('')
  const [signDate, setSignDate] = useState('')
  const [endDate, setEndDate] = useState('')
  const [pageIndex, setPageIndex] = useState(1)
  const [success, setSuccess] = useState('')

  const enums = useQuery({ queryKey: ['foundation-enums'], queryFn: foundationApi.getEnums })
  const contracts = useQuery({
    queryKey: ['contracts', keyword, status, pageIndex],
    queryFn: () => contractApi.getList({ keyword, status, pageIndex, pageSize: 10 }),
  })

  const filteredItems = useMemo(() => {
    return (contracts.data?.items ?? []).filter((contract) => {
      const risk = getContractRisk(contract)
      const matchCustomer = customerName ? contract.customerName?.includes(customerName) : true
      const matchRisk = riskLevel ? risk.level === riskLevel : true
      const matchSign = signDate ? contract.signDate?.slice(0, 10) >= signDate : true
      const matchEnd = endDate ? contract.endDate?.slice(0, 10) <= endDate : true
      return matchCustomer && matchRisk && matchSign && matchEnd
    })
  }, [contracts.data?.items, customerName, endDate, riskLevel, signDate])

  const totalPages = Math.max(1, Math.ceil((contracts.data?.totalCount ?? 0) / 10))
  const reload = () => queryClient.invalidateQueries({ queryKey: ['contracts'] })

  const runAction = async (message: string, action: () => Promise<unknown>) => {
    setSuccess('')
    await action()
    await reload()
    setSuccess(message)
  }

  const resetFilters = () => {
    setKeyword('')
    setCustomerName('')
    setStatus('')
    setRiskLevel('')
    setSignDate('')
    setEndDate('')
    setPageIndex(1)
  }

  return (
    <>
      <PageHeader
        title="合同管理工作台"
        description="集中处理合同查询、审批提交、归档、删除和风险识别。"
        actions={<Link className="btn btn-primary" to="/contracts/create">新建合同</Link>}
      />

      <div className="filter-panel mb-3">
        <div className="row g-3 align-items-end">
          <div className="col-12 col-md-4 col-xl-3">
            <label className="form-label">合同名称 / 编号</label>
            <input
              className="form-control"
              value={keyword}
              onChange={(e) => { setKeyword(e.target.value); setPageIndex(1) }}
              placeholder="输入合同名称或编号"
            />
          </div>
          <div className="col-12 col-md-4 col-xl-2">
            <label className="form-label">客户名称</label>
            <input
              className="form-control"
              value={customerName}
              onChange={(e) => { setCustomerName(e.target.value); setPageIndex(1) }}
              placeholder="输入客户名称"
            />
          </div>
          <div className="col-12 col-md-4 col-xl-2">
            <label className="form-label">合同状态</label>
            <select
              className="form-select"
              value={status}
              onChange={(e) => { setStatus(e.target.value === '' ? '' : Number(e.target.value)); setPageIndex(1) }}
            >
              <option value="">全部状态</option>
              {enums.data?.contractStatuses.map((item) => <option key={item.value} value={item.value}>{item.label}</option>)}
            </select>
          </div>
          <div className="col-12 col-md-4 col-xl-2">
            <label className="form-label">风险等级</label>
            <select className="form-select" value={riskLevel} onChange={(e) => setRiskLevel(e.target.value as RiskLevel | '')}>
              <option value="">全部风险</option>
              <option value="low">低风险</option>
              <option value="medium">中风险</option>
              <option value="high">高风险</option>
            </select>
          </div>
          <div className="col-12 col-md-4 col-xl-1">
            <label className="form-label">签署日期</label>
            <input className="form-control" type="date" value={signDate} onChange={(e) => setSignDate(e.target.value)} />
          </div>
          <div className="col-12 col-md-4 col-xl-1">
            <label className="form-label">到期日期</label>
            <input className="form-control" type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
          </div>
          <div className="col-12 col-xl-1 d-grid">
            <button className="btn btn-outline-secondary" type="button" onClick={resetFilters}>重置</button>
          </div>
        </div>
      </div>

      <ErrorMessage message={(contracts.error ?? enums.error) instanceof Error ? ((contracts.error ?? enums.error) as Error).message : undefined} />
      {success ? <div className="alert alert-success">{success}</div> : null}
      {contracts.isLoading ? <Loading /> : null}

      <div className="page-band">
        <ContractTable
          contracts={filteredItems}
          statuses={enums.data?.contractStatuses}
          onStart={(id) => runAction('合同已提交审批并进入执行流程。', () => contractApi.start(id))}
          onCancel={(id) => runAction('合同已归档。', () => contractApi.cancel(id))}
          onTerminate={(id) => runAction('合同已删除。', () => contractApi.remove(id))}
        />
        <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mt-3">
          <span className="text-secondary">共 {contracts.data?.totalCount ?? 0} 条，当前显示 {filteredItems.length} 条</span>
          <div className="btn-group">
            <button className="btn btn-outline-secondary btn-sm" disabled={pageIndex <= 1} onClick={() => setPageIndex((current) => current - 1)}>上一页</button>
            <span className="btn btn-outline-secondary btn-sm disabled">{pageIndex} / {totalPages}</span>
            <button className="btn btn-outline-secondary btn-sm" disabled={pageIndex >= totalPages} onClick={() => setPageIndex((current) => current + 1)}>下一页</button>
          </div>
        </div>
      </div>
    </>
  )
}

export default ContractListPage
