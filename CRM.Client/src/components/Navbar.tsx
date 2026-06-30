import { NavLink } from 'react-router-dom'

function Navbar() {
  const navClass = ({ isActive }: { isActive: boolean }) =>
    `nav-link${isActive ? ' active fw-semibold' : ''}`

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark px-3">
      <NavLink className="navbar-brand fw-bold" to="/">
        CRM Lite
      </NavLink>
      <button
        className="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#mainNavbar"
        aria-controls="mainNavbar"
        aria-expanded="false"
        aria-label="切换导航"
      >
        <span className="navbar-toggler-icon" />
      </button>
      <div className="collapse navbar-collapse" id="mainNavbar">
        <div className="navbar-nav">
          <NavLink className={navClass} to="/">
            首页 / 看板
          </NavLink>
          <NavLink className={navClass} to="/customers">
            客户管理
          </NavLink>
          <NavLink className={navClass} to="/contracts">
            合同管理
          </NavLink>
          <NavLink className={navClass} to="/about">
            系统说明
          </NavLink>
        </div>
      </div>
    </nav>
  )
}

export default Navbar
