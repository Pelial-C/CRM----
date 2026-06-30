import { useQuery, useQueryClient } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { useState } from 'react'
import { contractApi } from '../../api/contractApi'
import { foundationApi } from '../../api/foundationApi'
import ConfirmButton from '../../components/ConfirmButton'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import { formatDate } from '../../utils/date'
import { contractStatusBadge, enumText } from '../../utils/enumText'
import { formatMoney } from '../../utils/money'

function ContractListPage() {
  const queryClient = useQueryClient()
  const [keyword, setKeyword] = useState('')
  const [status, setStatus] = useState<number | ''>('')
  const [pageIndex, setPageIndex] = useState(1)
  const enums = useQuery({ queryKey: ['foundation-enums'], queryFn: foundationApi.getEnums })
  const contracts = useQuery({
    queryKey: ['contracts', keyword, status, pageIndex],
    queryFn: () => contractApi.getList({ keyword, status, pageIndex, pageSize: 10 }),
  })

  const totalPages = Math.max(1, Math.ceil((contracts.data?.totalCount ?? 0) / 10))
  const reload = () => queryClient.invalidateQueries({ queryKey: ['contracts'] })

  return (
    <>
      <PageHeader
        title="合同管理"
        description="合同、状态流转和回款计划"
        actions={<Link className="btn btn-primary" to="/contracts/create">新增合同</Link>}
      />
      <div className="page-band mb-3">
        <div className="row g-3 align-items-end">
          <div className="col-md-5">
            <label className="form-label">关键字</label>
            <input className="form-control" value={keyword} onChange={(e) => { setKeyword(e.target.value); setPageIndex(1) }} placeholder="合同编号、合同名称、客户名称" />
          </div>
          <div className="col-md-3">
            <label className="form-label">合同状态</label>
            <select className="form-select" value={status} onChange={(e) => { setStatus(e.target.value === '' ? '' : Number(e.target.value)); setPageIndex(1) }}>
              <option value="">全部状态</option>
              {enums.data?.contractStatuses.map((item) => <option key={item.value} value={item.value}>{item.label}</option>)}
            </select>
          </div>
        </div>
      </div>
      <ErrorMessage message={(contracts.error ?? enums.error) instanceof Error ? ((contracts.error ?? enums.error) as Error).message : undefined} />
      {contracts.isLoading ? <Loading /> : null}
      <div className="page-band">
        <div className="table-responsive">
          <table className="table table-hover">
            <thead>
              <tr>
                <th>合同编号</th>
                <th>合同名称</th>
                <th>客户名称</th>
                <th>联系人</th>
                <th>合同金额</th>
                <th>签订日期</th>
                <th>开始日期</th>
                <th>结束日期</th>
                <th>状态</th>
                <th>操作</th>
              </tr>
            </thead>
            <tbody>
              {contracts.data?.items.map((contract) => (
                <tr key={contract.id}>
                  <td>{contract.contractNo}</td>
                  <td>{contract.contractName}</td>
                  <td>{contract.customerName}</td>
                  <td>{contract.contactName ?? '-'}</td>
                  <td>{formatMoney(contract.totalAmount)}</td>
                  <td>{formatDate(contract.signDate)}</td>
                  <td>{formatDate(contract.startDate)}</td>
                  <td>{formatDate(contract.endDate)}</td>
                  <td>
                    <span className={`badge badge-fixed ${contractStatusBadge(contract.status)}`}>
                      {enumText(enums.data?.contractStatuses, contract.status)}
                    </span>
                  </td>
                  <td>
                    <div className="btn-group btn-group-sm">
                      <Link className="btn btn-outline-primary" to={`/contracts/${contract.id}`}>详情</Link>
                      <Link className="btn btn-outline-secondary" to={`/contracts/${contract.id}/edit`}>编辑</Link>
                      <ConfirmButton className="btn btn-outline-success" message="确认启动合同？" onConfirm={async () => { await contractApi.start(contract.id); await reload() }}>
                        启动
                      </ConfirmButton>
                      <ConfirmButton className="btn btn-outline-danger" message="确认作废合同？" onConfirm={async () => { await contractApi.cancel(contract.id); await reload() }}>
                        作废
                      </ConfirmButton>
                      <ConfirmButton className="btn btn-outline-warning" message="确认终止合同？" onConfirm={async () => { await contractApi.terminate(contract.id); await reload() }}>
                        终止
                      </ConfirmButton>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="d-flex justify-content-between align-items-center">
          <span className="text-secondary">共 {contracts.data?.totalCount ?? 0} 条</span>
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
