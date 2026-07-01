interface ChartItem {
  label: string
  value: number
  suffix?: string
}

interface DashboardChartCardProps {
  title: string
  description?: string
  items: ChartItem[]
}

function DashboardChartCard({ title, description, items }: DashboardChartCardProps) {
  const max = Math.max(...items.map((item) => item.value), 1)

  return (
    <div className="crm-card">
      <div className="mb-3">
        <h2 className="section-title h5">{title}</h2>
        {description ? <p className="section-subtitle">{description}</p> : null}
      </div>
      <div className="chart-bars">
        {items.map((item) => (
          <div className="chart-row" key={item.label}>
            <span className="text-secondary">{item.label}</span>
            <div className="chart-track">
              <div className="chart-bar" style={{ width: `${Math.max(6, (item.value / max) * 100)}%` }} />
            </div>
            <strong>{item.value}{item.suffix ?? ''}</strong>
          </div>
        ))}
      </div>
    </div>
  )
}

export default DashboardChartCard
