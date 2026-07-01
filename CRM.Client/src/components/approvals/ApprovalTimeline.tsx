interface ApprovalStep {
  title: string
  owner: string
  status: 'done' | 'current' | 'pending' | 'rejected'
}

interface ApprovalTimelineProps {
  steps?: ApprovalStep[]
}

const defaultSteps: ApprovalStep[] = [
  { title: '提交合同', owner: '销售负责人', status: 'done' },
  { title: '部门负责人审核', owner: '业务部门', status: 'done' },
  { title: '法务审核', owner: '法务中心', status: 'current' },
  { title: '财务审核', owner: '财务中心', status: 'pending' },
  { title: '最终批准', owner: '总经理办公室', status: 'pending' },
  { title: '签署完成', owner: '合同管理员', status: 'pending' },
]

const statusText = {
  done: '已完成',
  current: '进行中',
  pending: '未开始',
  rejected: '已驳回',
}

function ApprovalTimeline({ steps = defaultSteps }: ApprovalTimelineProps) {
  return (
    <div className="approval-timeline">
      {steps.map((step, index) => (
        <div className={`approval-step is-${step.status}`} key={step.title}>
          <div className="d-flex align-items-center justify-content-between mb-3">
            <span className="step-index">{index + 1}</span>
            <span className="badge text-bg-light">{statusText[step.status]}</span>
          </div>
          <h3 className="h6 mb-1">{step.title}</h3>
          <div className="text-secondary small">{step.owner}</div>
        </div>
      ))}
    </div>
  )
}

export default ApprovalTimeline
