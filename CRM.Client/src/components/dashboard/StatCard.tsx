import type { ReactNode } from 'react'
import type { CSSProperties } from 'react'

interface StatCardProps {
  label: string
  value: ReactNode
  hint?: string
  icon?: string
  color?: string
}

function StatCard({ label, value, hint, icon = 'D', color = '#0f3d75' }: StatCardProps) {
  return (
    <div className="crm-card stat-card" style={{ '--stat-color': color } as CSSProperties}>
      <div className="d-flex justify-content-between gap-3">
        <div>
          <div className="stat-label">{label}</div>
          <div className="stat-value">{value}</div>
          {hint ? <div className="text-secondary small mt-2">{hint}</div> : null}
        </div>
        <div className="stat-icon">{icon}</div>
      </div>
    </div>
  )
}

export default StatCard
