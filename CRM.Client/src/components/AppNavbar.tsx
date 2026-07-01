import { useState } from 'react'
import { Link, NavLink } from 'react-router-dom'

const navItems = [
  { to: '/', label: '首页', end: true },
  { to: '/dashboard', label: '仪表盘' },
  { to: '/customers', label: '客户管理' },
  { to: '/contracts', label: '合同管理' },
  { to: '/approvals', label: '审批流程' },
  { to: '/risks', label: '风险预警' },
  { to: '/reports', label: '数据报表' },
  { to: '/about', label: '系统设置' },
]

function AppNavbar() {
  const [open, setOpen] = useState(false)
  const navClass = ({ isActive }: { isActive: boolean }) => `nav-link${isActive ? ' active' : ''}`

  return (
    <nav className="navbar navbar-expand-xl crm-navbar px-3">
      <div className="container-fluid page-container">
        <Link className="navbar-brand d-flex align-items-center gap-2 fw-bold text-dark" to="/" onClick={() => setOpen(false)}>
          <span className="navbar-brand-mark">C</span>
          <span>合同管理平台</span>
        </Link>
        <button
          className="navbar-toggler"
          type="button"
          aria-controls="mainNavbar"
          aria-expanded={open}
          aria-label="切换导航"
          onClick={() => setOpen((current) => !current)}
        >
          <span className="navbar-toggler-icon" />
        </button>
        <div className={`collapse navbar-collapse${open ? ' show' : ''}`} id="mainNavbar">
          <div className="navbar-nav mx-xl-auto gap-xl-1">
            {navItems.map((item, index) => (
              <NavLink
                className={navClass}
                end={item.end}
                key={`${item.label}-${index}`}
                to={item.to}
                onClick={() => setOpen(false)}
              >
                {item.label}
              </NavLink>
            ))}
          </div>
          <div className="d-flex flex-wrap align-items-center gap-2 mt-3 mt-xl-0">
            <span className="user-pill">
              <span className="notification-dot" />
              通知
            </span>
            <span className="user-pill">合同管理员</span>
            <Link className="btn btn-primary btn-sm" to="/contracts/create" onClick={() => setOpen(false)}>
              快速新建合同
            </Link>
          </div>
        </div>
      </div>
    </nav>
  )
}

export default AppNavbar
