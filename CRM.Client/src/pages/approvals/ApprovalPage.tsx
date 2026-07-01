import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { contractApi } from '../../api/contractApi'
import ApprovalTimeline from '../../components/approvals/ApprovalTimeline'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import EmptyState from '../../components/common/EmptyState'
import StatusBadge from '../../components/common/StatusBadge'
import { formatDate } from '../../utils/date'
import { formatMoney } from '../../utils/money'

function ApprovalPage() {
  const contracts = useQuery({
    queryKey: ['approval-contracts'],
    queryFn: () => contractApi.getList({ pageIndex: 1, pageSize: 10 }),
  })

  return (
    <>
      <PageHeader
        title="审批流程"
        description="以流程视角管理提交合同、部门审核、法务审核、财务审核、最终批准和签署完成。"
        actions={<Link className="btn btn-primary" to="/contracts/create">提交新合同</Link>}
      />
      <ErrorMessage message={contracts.error instanceof Error ? contracts.error.message : undefined} />
      {contracts.isLoading ? <Loading /> : null}

      <div className="section-band mb-3">
        <div className="d-flex flex-wrap justify-content-between align-items-end gap-3 mb-3">
          <div>
            <h2 className="section-title">标准审批节点</h2>
            <p className="section-subtitle">不同节点使用已完成、进行中、未开始、驳回状态区分。</p>
          </div>
          <span className="badge text-bg-primary">SLA 48小时</span>
        </div>
        <ApprovalTimeline />
      </div>

      <div className="page-band">
        <h2 className="section-title h5 mb-3">审批合同列表</h2>
        {contracts.data?.items.length ? (
          <div className="responsive-table-wrap">
            <table className="table table-hover align-middle">
              <thead>
                <tr>
                  <th>合同名称</th>
                  <th>客户</th>
                  <th>合同金额</th>
                  <th>提交日期</th>
                  <th>当前节点</th>
                  <th>状态</th>
                  <th>操作</th>
                </tr>
              </thead>
              <tbody>
                {contracts.data.items.map((contract) => (
                  <tr key={contract.id}>
                    <td>{contract.contractName}</td>
                    <td>{contract.customerName}</td>
                    <td>{formatMoney(contract.totalAmount)}</td>
                    <td>{formatDate(contract.creationTime)}</td>
                    <td>法务审核</td>
                    <td><StatusBadge status={contract.status} endDate={contract.endDate} /></td>
                    <td><Link className="btn btn-outline-primary btn-sm" to={`/contracts/${contract.id}`}>查看流程</Link></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <EmptyState title="暂无审批合同" description="提交合同后会在此处展示审批进度。" />
        )}
      </div>
    </>
  )
}

export default ApprovalPage
