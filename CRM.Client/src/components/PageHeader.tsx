import type { ReactNode } from 'react'

interface PageHeaderProps {
  title: string
  description?: string
  actions?: ReactNode
}

function PageHeader({ title, description, actions }: PageHeaderProps) {
  return (
    <div className="d-flex flex-wrap align-items-center justify-content-between gap-3 mb-3">
      <div>
        <h1 className="page-title">{title}</h1>
        {description ? <div className="text-secondary mt-1">{description}</div> : null}
      </div>
      {actions ? <div className="action-bar">{actions}</div> : null}
    </div>
  )
}

export default PageHeader
