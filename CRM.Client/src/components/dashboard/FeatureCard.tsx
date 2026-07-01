import { Link } from 'react-router-dom'

interface FeatureCardProps {
  icon: string
  title: string
  description: string
  to: string
  action?: string
}

function FeatureCard({ icon, title, description, to, action = '进入模块' }: FeatureCardProps) {
  return (
    <div className="crm-card feature-card d-flex flex-column">
      <div className="feature-icon">{icon}</div>
      <h3 className="feature-title">{title}</h3>
      <p className="feature-description">{description}</p>
      <Link className="btn btn-outline-primary btn-sm align-self-start" to={to}>
        {action}
      </Link>
    </div>
  )
}

export default FeatureCard
