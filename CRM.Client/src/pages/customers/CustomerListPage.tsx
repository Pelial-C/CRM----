import { useQuery, useQueryClient } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { useMemo, useState } from 'react'
import { contractApi } from '../../api/contractApi'
import { customerApi } from '../../api/customerApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import CustomerTable from '../../components/customers/CustomerTable'
import CustomerEvaluationModal from './CustomerEvaluationModal'
import type { CustomerDto } from '../../types/customer'

const pageSize = 10

const levelNames: Record<number, string> = {
  0: '战略客户',
  1: '重点客户',
  2: '普通客户',
  3: '风险客户',
}

function CustomerListPage() {
  const queryClient = useQueryClient()
  const [keyword, setKeyword] = useState('')
  const [industry, setIndustry] = useState('')
  const [levelFilter, setLevelFilter] = useState<number | ''>('')
  const [includeDeleted, setIncludeDeleted] = useState(false)
  const [pageIndex, setPageIndex] = useState(1)
  const [success, setSuccess] = useState('')
  const [evalCustomerId, setEvalCustomerId] = useState<number | null>(null)
  const [evalCustomerName, setEvalCustomerName] = useState('')

  const customers = useQuery({
    queryKey: ['customers', keyword, industry, includeDeleted],
    queryFn: () => customerApi.getList({ keyword, industry, includeDeleted }),
  })
  const contracts = useQuery({
    queryKey: ['customer-contract-counts'],
    queryFn: () => contractApi.getList({ pageIndex: 1, pageSize: 1000 }),
  })

  const contractCounts = useMemo(() => {
    const result: Record<number, number> = {}
    for (const contract of contracts.data?.items ?? []) {
      result[contract.customerId] = (result[contract.customerId] ?? 0) + 1
    }
    return result
  }, [contracts.data?.items])

  const filteredCustomers = useMemo(() => {
    return (customers.data ?? []).filter((customer) => {
      if (levelFilter !== '' && customer.level !== levelFilter) return false
      return true
    })
  }, [customers.data, levelFilter])

  const totalPages = Math.max(1, Math.ceil(filteredCustomers.length / pageSize))
  const pageItems = filteredCustomers.slice((pageIndex - 1) * pageSize, pageIndex * pageSize)

  const resetFilters = () => {
    setKeyword('')
    setIndustry('')
    setLevelFilter('')
    setIncludeDeleted(false)
    setPageIndex(1)
  }

  const removeCustomer = async (id: number) => {
    setSuccess('')
    await customerApi.remove(id)
    await queryClient.invalidateQueries({ queryKey: ['customers'] })
    setSuccess('客户已删除。')
  }

  const openEvaluation = (customer: CustomerDto) => {
    setEvalCustomerId(customer.id)
    setEvalCustomerName(customer.name ?? '')
  }

  return (
    <>
      <PageHeader
        title="客户管理"
        description="维护客户档案、联系人、客户等级评估和关联合同信息。"
        actions={<Link className="btn btn-primary" to="/customers/create">新增客户</Link>}
      />

      <div className="filter-panel mb-3">
        <div className="row g-3 align-items-end">
          <div className="col-12 col-md-4">
            <label className="form-label">客户名称 / 信用代码</label>
            <input className="form-control" value={keyword} onChange={(e) => { setKeyword(e.target.value); setPageIndex(1) }} placeholder="输入企业名称或统一社会信用代码" />
          </div>
          <div className="col-12 col-md-3">
            <label className="form-label">行业</label>
            <input className="form-control" value={industry} onChange={(e) => { setIndustry(e.target.value); setPageIndex(1) }} placeholder="输入行业关键词" />
          </div>
          <div className="col-12 col-md-2">
            <label className="form-label">客户等级</label>
            <select className="form-select" value={levelFilter} onChange={(e) => { setLevelFilter(e.target.value === '' ? '' : Number(e.target.value)); setPageIndex(1) }}>
              <option value="">全部等级</option>
              <option value="0">战略客户</option>
              <option value="1">重点客户</option>
              <option value="2">普通客户</option>
              <option value="3">风险客户</option>
            </select>
          </div>
          <div className="col-12 col-md-2">
            <div className="form-check">
              <input className="form-check-input" id="includeDeleted" type="checkbox" checked={includeDeleted} onChange={(e) => setIncludeDeleted(e.target.checked)} />
              <label className="form-check-label" htmlFor="includeDeleted">显示已删除客户</label>
            </div>
          </div>
          <div className="col-12 col-md-1 d-grid">
            <button className="btn btn-outline-secondary" type="button" onClick={resetFilters}>重置</button>
          </div>
        </div>
      </div>

      <ErrorMessage message={(customers.error ?? contracts.error) instanceof Error ? ((customers.error ?? contracts.error) as Error).message : undefined} />
      {success ? <div className="alert alert-success">{success}</div> : null}
      {customers.isLoading ? <Loading /> : null}

      <div className="page-band">
        <CustomerTable
          customers={pageItems}
          contractCounts={contractCounts}
          onRemove={removeCustomer}
          onEvaluate={openEvaluation}
        />
        <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mt-3">
          <span className="text-secondary">共 {filteredCustomers.length} 条</span>
          <div className="btn-group">
            <button className="btn btn-outline-secondary btn-sm" disabled={pageIndex <= 1} onClick={() => setPageIndex((current) => current - 1)}>上一页</button>
            <span className="btn btn-outline-secondary btn-sm disabled">{pageIndex} / {totalPages}</span>
            <button className="btn btn-outline-secondary btn-sm" disabled={pageIndex >= totalPages} onClick={() => setPageIndex((current) => current + 1)}>下一页</button>
          </div>
        </div>
      </div>

      <CustomerEvaluationModal
        customerId={evalCustomerId}
        customerName={evalCustomerName}
        onClose={() => {
          setEvalCustomerId(null)
          queryClient.invalidateQueries({ queryKey: ['customers'] })
        }}
      />
    </>
  )
}

export default CustomerListPage
