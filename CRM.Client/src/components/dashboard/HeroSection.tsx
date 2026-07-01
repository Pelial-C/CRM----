import { Link } from 'react-router-dom'
import type { DashboardSummary } from '../../types/foundation'
import { formatMoney } from '../../utils/money'

interface HeroSectionProps {
  summary?: DashboardSummary
}

function HeroSection({ summary }: HeroSectionProps) {
  return (
    <section className="hero-section mb-4">
      <div className="row g-4 align-items-center">
        <div className="col-lg-7 hero-content">
          <div className="hero-eyebrow">企业合同全生命周期管理</div>
          <h1 className="hero-title">智能化企业合同管理平台</h1>
          <p className="hero-subtitle">
            统一管理客户、合同、审批、履约与风险，让企业合同全生命周期更加高效、透明、可追踪。
          </p>
          <div className="d-flex flex-wrap gap-2 mt-4">
            <Link className="btn btn-primary btn-lg" to="/">
              进入工作台
            </Link>
            <Link className="btn btn-light btn-lg" to="/contracts/create">
              新建合同
            </Link>
          </div>
        </div>
        <div className="col-lg-5 hero-dashboard">
          <div className="d-flex justify-content-between align-items-center mb-3">
            <div>
              <div className="text-white-50 small">实时经营看板</div>
              <strong>合同运营概览</strong>
            </div>
            <span className="badge text-bg-info">Live</span>
          </div>
          <div className="row g-3">
            <div className="col-6">
              <div className="hero-dashboard-card">
                <div className="metric-label">合同总数</div>
                <div className="metric-value">{summary?.contractCount ?? 0}</div>
              </div>
            </div>
            <div className="col-6">
              <div className="hero-dashboard-card">
                <div className="metric-label">待处理风险</div>
                <div className="metric-value">{summary?.overduePaymentPlanCount ?? 0}</div>
              </div>
            </div>
            <div className="col-6">
              <div className="hero-dashboard-card">
                <div className="metric-label">30天到期</div>
                <div className="metric-value">{summary?.contractsDueIn30DaysCount ?? 0}</div>
              </div>
            </div>
            <div className="col-6">
              <div className="hero-dashboard-card">
                <div className="metric-label">合同总金额</div>
                <div className="metric-value fs-5">{formatMoney(summary?.totalContractAmount)}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}

export default HeroSection
