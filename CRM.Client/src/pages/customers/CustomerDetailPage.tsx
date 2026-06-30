import { useQuery } from '@tanstack/react-query'
import { Link, useParams } from 'react-router-dom'
import { contractApi } from '../../api/contractApi'
import { customerApi } from '../../api/customerApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
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

  return (
    <>
      <PageHeader
        title="客户详情"
        actions={<Link className="btn btn-outline-primary" to={`/customers/${customerId}/edit`}>编辑客户</Link>}
      />
      <ErrorMessage message={customerQuery.error instanceof Error ? customerQuery.error.message : undefined} />
      {customerQuery.isLoading ? <Loading /> : null}
      {customer ? (
        <>
          <div className="page-band mb-3">
            <div className="row g-3">
              <div className="col-md-4"><strong>企业名称：</strong>{customer.name}</div>
              <div className="col-md-4"><strong>信用代码：</strong>{customer.creditCode ?? '-'}</div>
              <div className="col-md-4"><strong>行业：</strong>{customer.industry ?? '-'}</div>
              <div className="col-md-8">
                <strong>地址：</strong>{[customer.province, customer.city, customer.district, customer.detailAddress].filter(Boolean).join(' ') || '-'}
              </div>
              <div className="col-md-4"><strong>状态：</strong>{customer.isDeleted ? '已删除' : '正常'}</div>
              <div className="col-12"><strong>备注：</strong>{customer.remark ?? '-'}</div>
            </div>
          </div>
          <div className="page-band mb-3">
            <div className="d-flex justify-content-between align-items-center mb-2">
              <h2 className="h5 mb-0">联系人</h2>
              <Link className="btn btn-sm btn-primary" to={`/customers/${customerId}/contacts/create`}>新增联系人</Link>
            </div>
            <table className="table table-sm">
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
          <div className="page-band">
            <h2 className="h5">关联合同</h2>
            {contractsQuery.isLoading ? <Loading /> : null}
            <table className="table table-sm">
              <thead><tr><th>合同编号</th><th>合同名称</th><th>金额</th><th>结束日期</th><th>操作</th></tr></thead>
              <tbody>
                {contractsQuery.data?.items.map((contract) => (
                  <tr key={contract.id}>
                    <td>{contract.contractNo}</td>
                    <td>{contract.contractName}</td>
                    <td>{formatMoney(contract.totalAmount)}</td>
                    <td>{formatDate(contract.endDate)}</td>
                    <td><Link className="btn btn-outline-primary btn-sm" to={`/contracts/${contract.id}`}>详情</Link></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      ) : null}
    </>
  )
}

export default CustomerDetailPage
