import { useQuery } from '@tanstack/react-query'
import { Link, useParams } from 'react-router-dom'
import { contractApi } from '../../api/contractApi'
import { customerApi } from '../../api/customerApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import EmptyState from '../../components/common/EmptyState'
import RiskBadge from '../../components/common/RiskBadge'
import StatusBadge from '../../components/common/StatusBadge'
import { getContractRisk } from '../../utils/contractVisual'
import { formatDate } from '../../utils/date'
import { formatMoney } from '../../utils/money'

function CustomerDetailPage() {
  const { id } = useParams()
  const customerId = Number(id)
  const customerQuery = useQuery({
    queryKey: ['customer', customerId],
    queryFn: () => customerApi.get(customerId),
    enabled: customerId > 0,
  })
  const contractsQuery = useQuery({
    queryKey: ['customer-contracts', customerId],
    queryFn: () => contractApi.getList({ customerId, pageIndex: 1, pageSize: 100 }),
    enabled: customerId > 0,
  })

  const customer = customerQuery.data
  const contracts = contractsQuery.data?.items ?? []
  const highRiskCount = contracts.filter((contract) => getContractRisk(contract).level !== 'low').length

  return (
    <>
      <PageHeader
        title="客户详情"
        description="客户基础档案、联系人、关联合同、风险记录和最近动态。"
        actions={<Link className="btn btn-outline-primary" to={`/customers/${customerId}/edit`}>编辑客户</Link>}
      />
      <ErrorMessage message={customerQuery.error instanceof Error ? customerQuery.error.message : undefined} />
      {customerQuery.isLoading ? <Loading /> : null}
      {customer ? (
        <>
          <div className="section-band mb-3">
            <div className="d-flex flex-wrap justify-content-between align-items-start gap-3 mb-3">
              <div>
                <h2 className="section-title">{customer.name}</h2>
                <p className="section-subtitle">{customer.industry ?? '未维护行业'} · {customer.creditCode ?? '未维护信用代码'}</p>
              </div>
              <span className={`status-badge ${customer.isDeleted ? 'status-archived' : 'status-signed'}`}>
                {customer.isDeleted ? '已删除' : '正常'}
              </span>
            </div>
            <div className="detail-grid">
              <div className="detail-item"><div className="detail-label">企业名称</div><div className="detail-value">{customer.name}</div></div>
              <div className="detail-item"><div className="detail-label">行业</div><div className="detail-value">{customer.industry ?? '-'}</div></div>
              <div className="detail-item"><div className="detail-label">联系人数量</div><div className="detail-value">{customer.contacts.length}</div></div>
              <div className="detail-item"><div className="detail-label">关联合同</div><div className="detail-value">{contracts.length}</div></div>
              <div className="detail-item"><div className="detail-label">风险记录</div><div className="detail-value">{highRiskCount}</div></div>
              <div className="detail-item"><div className="detail-label">最近合作时间</div><div className="detail-value">{formatDate(customer.creationTime)}</div></div>
              <div className="detail-item"><div className="detail-label">地址</div><div className="detail-value">{[customer.province, customer.city, customer.district, customer.detailAddress].filter(Boolean).join(' ') || '-'}</div></div>
              <div className="detail-item"><div className="detail-label">备注</div><div className="detail-value">{customer.remark ?? '-'}</div></div>
            </div>
          </div>

          <div className="row g-3 mb-3">
            <div className="col-12 col-xl-7">
              <div className="page-band h-100">
                <div className="d-flex justify-content-between align-items-center mb-3">
                  <h2 className="section-title h5">联系人与沟通记录</h2>
                  <Link className="btn btn-sm btn-primary" to={`/customers/${customerId}/contacts/create`}>新增联系人</Link>
                </div>
                {customer.contacts.length ? (
                  <div className="responsive-table-wrap">
                    <table className="table table-sm align-middle">
                      <thead><tr><th>姓名</th><th>职务</th><th>手机号</th><th>邮箱</th><th>关键决策人</th><th>操作</th></tr></thead>
                      <tbody>
                        {customer.contacts.map((contact) => (
                          <tr key={contact.id}>
                            <td>{contact.name}</td>
                            <td>{contact.title ?? '-'}</td>
                            <td>{contact.phone ?? '-'}</td>
                            <td>{contact.email ?? '-'}</td>
                            <td>{contact.isKeyDecisionMaker ? '是' : '否'}</td>
                            <td><Link className="btn btn-outline-secondary btn-sm" to={`/contacts/${contact.id}/edit`}>编辑</Link></td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                ) : (
                  <EmptyState title="暂无联系人" description="添加联系人后可维护沟通记录和关键决策人。" />
                )}
              </div>
            </div>
            <div className="col-12 col-xl-5">
              <div className="page-band h-100">
                <h2 className="section-title h5 mb-3">最近动态</h2>
                <div className="d-grid gap-3">
                  <div className="p-3 border rounded-3">
                    <div className="fw-semibold">客户档案创建</div>
                    <div className="text-secondary small">{formatDate(customer.creationTime)}</div>
                  </div>
                  <div className="p-3 border rounded-3">
                    <div className="fw-semibold">合同关联更新</div>
                    <div className="text-secondary small">当前共 {contracts.length} 份关联合同</div>
                  </div>
                  <div className="p-3 border rounded-3">
                    <div className="fw-semibold">风险扫描完成</div>
                    <div className="text-secondary small">识别 {highRiskCount} 条需关注记录</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="page-band">
            <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
              <h2 className="section-title h5">关联合同</h2>
              <Link className="btn btn-outline-primary btn-sm" to="/contracts/create">新建合同</Link>
            </div>
            {contractsQuery.isLoading ? <Loading /> : null}
            {contracts.length ? (
              <div className="responsive-table-wrap">
                <table className="table table-sm align-middle">
                  <thead><tr><th>合同编号</th><th>合同名称</th><th>金额</th><th>结束日期</th><th>状态</th><th>风险</th><th>操作</th></tr></thead>
                  <tbody>
                    {contracts.map((contract) => {
                      const risk = getContractRisk(contract)
                      return (
                        <tr key={contract.id}>
                          <td>{contract.contractNo}</td>
                          <td>{contract.contractName}</td>
                          <td>{formatMoney(contract.totalAmount)}</td>
                          <td>{formatDate(contract.endDate)}</td>
                          <td><StatusBadge status={contract.status} endDate={contract.endDate} /></td>
                          <td><RiskBadge level={risk.level} /></td>
                          <td><Link className="btn btn-outline-primary btn-sm" to={`/contracts/${contract.id}`}>详情</Link></td>
                        </tr>
                      )
                    })}
                  </tbody>
                </table>
              </div>
            ) : (
              <EmptyState title="暂无关联合同" description="新建合同时选择该客户后会自动关联。" />
            )}
          </div>
        </>
      ) : null}
    </>
  )
}

export default CustomerDetailPage
