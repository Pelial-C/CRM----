import { Link } from 'react-router-dom'
import ConfirmButton from '../ConfirmButton'
import EmptyState from '../common/EmptyState'
import RiskBadge from '../common/RiskBadge'
import StatusBadge from '../common/StatusBadge'
import type { ContractDto } from '../../types/contract'
import type { EnumItem } from '../../types/foundation'
import { getContractRisk } from '../../utils/contractVisual'
import { formatDate } from '../../utils/date'
import { enumText } from '../../utils/enumText'
import { formatMoney } from '../../utils/money'

interface ContractTableProps {
  contracts: ContractDto[]
  statuses?: EnumItem[]
  onStart?: (id: number) => Promise<void>
  onCancel?: (id: number) => Promise<void>
  onTerminate?: (id: number) => Promise<void>
}

function ContractTable({ contracts, statuses, onStart, onCancel, onTerminate }: ContractTableProps) {
  if (!contracts.length) {
    return (
      <EmptyState
        title="暂无合同记录"
        description="当前筛选条件下没有合同，可调整筛选项或新建一份合同。"
        actions={<Link className="btn btn-primary" to="/contracts/create">新建合同</Link>}
      />
    )
  }

  return (
    <div className="responsive-table-wrap">
      <table className="table table-hover align-middle">
        <thead>
          <tr>
            <th>合同编号</th>
            <th>合同名称</th>
            <th>客户名称</th>
            <th>联系人</th>
            <th>合同金额</th>
            <th>签署日期</th>
            <th>到期日期</th>
            <th>状态</th>
            <th>风险等级</th>
            <th className="table-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          {contracts.map((contract) => {
            const risk = getContractRisk(contract)
            return (
              <tr key={contract.id}>
                <td className="fw-semibold">{contract.contractNo}</td>
                <td>{contract.contractName}</td>
                <td>{contract.customerName}</td>
                <td>{contract.contactName ?? '-'}</td>
                <td>{formatMoney(contract.totalAmount)}</td>
                <td>{formatDate(contract.signDate)}</td>
                <td>{formatDate(contract.endDate)}</td>
                <td>
                  <StatusBadge status={contract.status} label={enumText(statuses, contract.status)} endDate={contract.endDate} />
                </td>
                <td>
                  <RiskBadge level={risk.level} />
                </td>
                <td>
                  <div className="btn-group btn-group-sm">
                    <Link className="btn btn-outline-primary" to={`/contracts/${contract.id}`}>查看</Link>
                    <Link className="btn btn-outline-secondary" to={`/contracts/${contract.id}/edit`}>编辑</Link>
                    {onStart ? (
                      <ConfirmButton className="btn btn-outline-success" message="确认提交并启动该合同？" onConfirm={() => onStart(contract.id)}>
                        提交审批
                      </ConfirmButton>
                    ) : null}
                    {onCancel ? (
                      <ConfirmButton className="btn btn-outline-dark" message="确认归档该合同？" onConfirm={() => onCancel(contract.id)}>
                        归档
                      </ConfirmButton>
                    ) : null}
                    {onTerminate ? (
                      <ConfirmButton className="btn btn-outline-danger" message="确认删除该合同？该操作将移除合同记录。" onConfirm={() => onTerminate(contract.id)}>
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

export default ContractTable
