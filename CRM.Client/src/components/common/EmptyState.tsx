import type { ReactNode } from 'react'

interface EmptyStateProps {
  title?: string
  description?: string
  actions?: ReactNode
}

function EmptyState({ title = '暂无数据', description = '当前筛选条件下没有可展示的记录。', actions }: EmptyStateProps) {
  return (
    <div className="empty-state">
      <div className="empty-state-icon">—</div>
      <h2 className="h5 text-dark">{title}</h2>
      <p className="mb-3">{description}</p>
      {actions}
    </div>
  )
}

export default EmptyState
