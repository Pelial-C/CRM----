import type { RiskLevel } from '../../utils/contractVisual'

interface RiskBadgeProps {
  level: RiskLevel
  label?: string
}

function RiskBadge({ level, label }: RiskBadgeProps) {
  const text = label ?? (level === 'high' ? '高风险' : level === 'medium' ? '中风险' : '低风险')

  return <span className={`risk-badge risk-${level}`}>{text}</span>
}

export default RiskBadge
