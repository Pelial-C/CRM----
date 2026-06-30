import PageHeader from '../../components/PageHeader'

function AboutPage() {
  return (
    <>
      <PageHeader title="系统说明" description="企业客户与合同管理系统 CRM Lite" />
      <div className="page-band">
        <p>
          本系统用于替代 Excel 台账，管理客户、联系人、合同、合同明细、回款计划和基础统计看板。
        </p>
        <hr />
        <div className="row g-3">
          <div className="col-md-6">
            <h2 className="h5">后端架构</h2>
            <ul className="mb-0">
              <li>ASP.NET Core Web API</li>
              <li>EF Core + SQL Server LocalDB</li>
              <li>DDD 分层架构</li>
            </ul>
          </div>
          <div className="col-md-6">
            <h2 className="h5">前端架构</h2>
            <ul className="mb-0">
              <li>React + Vite + TypeScript</li>
              <li>Axios + React Router</li>
              <li>Bootstrap 样式</li>
            </ul>
          </div>
        </div>
        <hr />
        <p className="text-secondary mb-0">
          课程设计版本暂未实现完整 RBAC、审批流程、电子签章和附件管理，后续可接入 ASP.NET Core Identity 扩展。
        </p>
      </div>
    </>
  )
}

export default AboutPage
