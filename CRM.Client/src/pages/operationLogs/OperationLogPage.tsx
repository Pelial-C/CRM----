import { useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { Link } from 'react-router-dom'
import { operationLogApi } from '../../api/operationLogApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import EmptyState from '../../components/common/EmptyState'
import { formatDate } from '../../utils/date'

const eventLabel: Record<string, string> = {
  CustomerCreatedEvent: '创建客户',
  ContactAddedEvent: '新增联系人',
  ContractCreatedEvent: '创建合同',
  ContractCompletedEvent: '合同完成',
  ContractCancelledEvent: '合同作废',
  PaymentRecordedEvent: '登记回款',
}

const eventColor: Record<string, string> = {
  CustomerCreatedEvent: '#059669',
  ContactAddedEvent: '#0891b2',
  ContractCreatedEvent: '#2563eb',
  ContractCompletedEvent: '#059669',
  ContractCancelledEvent: '#dc2626',
  PaymentRecordedEvent: '#ea580c',
}

const entityLabel: Record<string, string> = {
  Customer: '客户',
  Contract: '合同',
}

function OperationLogPage() {
  const [pageIndex, setPageIndex] = useState(1)
  const [entityFilter, setEntityFilter] = useState('')
  const pageSize = 20

  const { data, isLoading, error } = useQuery({
    queryKey: ['operationLogs', pageIndex, entityFilter],
    queryFn: () =>
      operationLogApi.getList({
        pageIndex,
        pageSize,
        entityName: entityFilter || undefined,
      }),
  })

  const logs = data?.items ?? []
  const totalPages = Math.max(1, Math.ceil((data?.totalCount ?? 0) / pageSize))

  return (
    <>
      <PageHeader
        title="操作日志"
        description="记录系统中所有关键业务操作，包括客户创建、合同状态变更和回款登记。"
      />

      <div className="filter-panel mb-3">
        <div className="row g-3 align-items-end">
          <div className="col-12 col-md-4 col-xl-3">
            <label className="form-label">聚合类型</label>
            <select
              className="form-select"
              value={entityFilter}
              onChange={(e) => { setEntityFilter(e.target.value); setPageIndex(1) }}
            >
              <option value="">全部</option>
              <option value="Customer">客户</option>
              <option value="Contract">合同</option>
            </select>
          </div>
          <div className="col-12 col-md-3 col-xl-2">
            <span className="text-secondary">
              共 {data?.totalCount ?? 0} 条记录
            </span>
          </div>
        </div>
      </div>

      <ErrorMessage message={error instanceof Error ? error.message : undefined} />
      {isLoading ? <Loading /> : null}

      <div className="page-band">
        {logs.length === 0 && !isLoading ? (
          <EmptyState
            title="暂无操作日志"
            description="当系统中发生客户创建、合同签订、回款登记等操作时，日志将自动记录于此。"
          />
        ) : (
          <div className="operation-log-timeline">
            {logs.map((log) => {
              const color = eventColor[log.eventType] ?? '#6b7280'
              const label = eventLabel[log.eventType] ?? log.eventType
              const entity = entityLabel[log.entityName] ?? log.entityName

              return (
                <div className="operation-log-item" key={log.id}>
                  <div className="operation-log-dot" style={{ backgroundColor: color }} />
                  <div className="operation-log-content">
                    <div className="d-flex flex-wrap justify-content-between align-items-start gap-2 mb-1">
                      <div>
                        <span
                          className="badge me-2"
                          style={{ backgroundColor: color, color: '#fff' }}
                        >
                          {label}
                        </span>
                        <span className="fw-semibold">{log.description}</span>
                      </div>
                      <Link
                        to={log.entityName === 'Customer' ? `/customers/${log.entityId}` : `/contracts/${log.entityId}`}
                        className="btn btn-outline-secondary btn-sm"
                      >
                        查看{entity} #{log.entityId}
                      </Link>
                    </div>
                    <div className="text-secondary small">
                      {formatDate(log.occurredAt)}
                      {log.operatorName ? <> · {log.operatorName}</> : null}
                    </div>
                  </div>
                </div>
              )
            })}
          </div>
        )}

        {logs.length > 0 && (
          <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mt-3">
            <span className="text-secondary">
              第 {pageIndex} / {totalPages} 页
            </span>
            <div className="btn-group">
              <button
                className="btn btn-outline-secondary btn-sm"
                disabled={pageIndex <= 1}
                onClick={() => setPageIndex((p) => p - 1)}
              >
                上一页
              </button>
              <button
                className="btn btn-outline-secondary btn-sm"
                disabled={pageIndex >= totalPages}
                onClick={() => setPageIndex((p) => p + 1)}
              >
                下一页
              </button>
            </div>
          </div>
        )}
      </div>
    </>
  )
}

export default OperationLogPage
