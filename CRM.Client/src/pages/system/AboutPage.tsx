import PageHeader from '../../components/PageHeader'

const settingCards = [
  ['组织权限', '可扩展 ASP.NET Core Identity、RBAC 和部门角色权限。', '规划中'],
  ['审批配置', '可配置部门负责人、法务、财务和最终批准节点。', '可扩展'],
  ['通知中心', '合同到期、回款逾期、审批超时提醒统一进入通知。', '前端已预留'],
  ['系统基础资料', '合同类型、服务类型、回款频率等枚举由后端 Foundation API 提供。', '已接入'],
]

function AboutPage() {
  return (
    <>
      <PageHeader title="系统设置" description="企业客户与合同管理系统 CRM Lite 的架构、能力和扩展入口。" />
      <div className="section-band mb-3">
        <div className="row g-3">
          <div className="col-12 col-lg-6">
            <div className="crm-card">
              <h2 className="section-title h5 mb-3">后端架构</h2>
              <div className="d-grid gap-2">
                {['ASP.NET Core Web API', 'EF Core + SQL Server LocalDB', 'DDD 分层架构', '统一 ApiResponse 返回模型'].map((item) => (
                  <div className="d-flex justify-content-between border rounded-3 p-3" key={item}>
                    <span>{item}</span>
                    <span className="badge text-bg-success">已启用</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
          <div className="col-12 col-lg-6">
            <div className="crm-card">
              <h2 className="section-title h5 mb-3">前端架构</h2>
              <div className="d-grid gap-2">
                {['React + Vite + TypeScript', 'Axios + React Router', 'TanStack Query 数据请求', 'Bootstrap 响应式视觉系统'].map((item) => (
                  <div className="d-flex justify-content-between border rounded-3 p-3" key={item}>
                    <span>{item}</span>
                    <span className="badge text-bg-primary">已接入</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="row g-3">
        {settingCards.map(([title, description, status]) => (
          <div className="col-12 col-md-6 col-xl-3" key={title}>
            <div className="crm-card">
              <div className="d-flex justify-content-between align-items-start gap-2 mb-3">
                <h2 className="h5 mb-0">{title}</h2>
                <span className="badge text-bg-light">{status}</span>
              </div>
              <p className="text-secondary mb-0">{description}</p>
            </div>
          </div>
        ))}
      </div>
    </>
  )
}

export default AboutPage
