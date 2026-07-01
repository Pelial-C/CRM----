import { Link } from 'react-router-dom'
import ConfirmButton from '../ConfirmButton'
import EmptyState from '../common/EmptyState'
import type { CustomerDto } from '../../types/customer'
import { formatDate } from '../../utils/date'

interface CustomerTableProps {
  customers: CustomerDto[]
  contractCounts?: Record<number, number>
  onRemove?: (id: number) => Promise<void>
}

function customerLevel(customer: CustomerDto) {
  const contactCount = customer.contacts?.length ?? 0
  if (contactCount >= 3) return '战略客户'
  if (contactCount >= 1) return '重点客户'
  return '普通客户'
}

function CustomerTable({ customers, contractCounts, onRemove }: CustomerTableProps) {
  if (!customers.length) {
    return (
      <EmptyState
        title="暂无客户记录"
        description="当前筛选条件下没有客户，可调整筛选项或新增客户档案。"
        actions={<Link className="btn btn-primary" to="/customers/create">新增客户</Link>}
      />
    )
  }

  return (
    <div className="responsive-table-wrap">
      <table className="table table-hover align-middle">
        <thead>
          <tr>
            <th>客户名称</th>
            <th>联系人</th>
            <th>联系电话</th>
            <th>邮箱</th>
            <th>客户等级</th>
            <th>合同数量</th>
            <th>最近合作时间</th>
            <th>状态</th>
            <th className="table-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          {customers.map((customer) => {
            const primaryContact = customer.contacts?.[0]
            return (
              <tr key={customer.id}>
                <td>
                  <div className="fw-semibold">{customer.name}</div>
                  <div className="text-secondary small">{customer.creditCode ?? '未维护信用代码'}</div>
                </td>
                <td>{primaryContact?.name ?? '-'}</td>
                <td>{primaryContact?.phone ?? '-'}</td>
                <td>{primaryContact?.email ?? '-'}</td>
                <td><span className="badge text-bg-primary">{customerLevel(customer)}</span></td>
                <td>{contractCounts?.[customer.id] ?? 0}</td>
                <td>{formatDate(customer.creationTime)}</td>
                <td>
                  <span className={`status-badge ${customer.isDeleted ? 'status-archived' : 'status-signed'}`}>
                    {customer.isDeleted ? '已删除' : '正常'}
                  </span>
                </td>
                <td>
                  <div className="btn-group btn-group-sm">
                    <Link className="btn btn-outline-primary" to={`/customers/${customer.id}`}>查看</Link>
                    <Link className="btn btn-outline-secondary" to={`/customers/${customer.id}/edit`}>编辑</Link>
                    {onRemove ? (
                      <ConfirmButton
                        className="btn btn-outline-danger"
                        message="确认删除该客户？有关联合同客户将进行逻辑删除。"
                        disabled={customer.isDeleted}
                        onConfirm={() => onRemove(customer.id)}
                      >
                        删除
                      </ConfirmButton>
                    ) : null}
                  </div>
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}

export default CustomerTable
