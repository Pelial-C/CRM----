import { formatDate } from '../../utils/date'
import { getContractVisualStatus } from '../../utils/contractVisual'

interface StatusBadgeProps {
  status: number
  label?: string
  endDate?: string | null
}

function StatusBadge({ status, label, endDate }: StatusBadgeProps) {
  const visual = getContractVisualStatus(status, label, endDate)

  return (
    <span className={`status-badge ${visual.className}`} title={endDate ? `到期日：${formatDate(endDate)}` : undefined}>
      {visual.label}
    </span>
  )
}

export default StatusBadge
