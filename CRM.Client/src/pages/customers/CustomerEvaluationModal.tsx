import { useQuery } from '@tanstack/react-query'
import { customerApi } from '../../api/customerApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import type { CustomerEvaluationDto } from '../../types/customer'

interface CustomerEvaluationModalProps {
  customerId: number | null
  customerName?: string
  onClose: () => void
}

const levelColors: Record<number, { bg: string; text: string; label: string }> = {
  0: { bg: '#dcfce7', text: '#047857', label: '战略客户' },
  1: { bg: '#dbeafe', text: '#1d4ed8', label: '重点客户' },
  2: { bg: '#f3f4f6', text: '#374151', label: '普通客户' },
  3: { bg: '#fee2e2', text: '#b91c1c', label: '风险客户' },
}

function ScoreBar({ label, score, max, color }: { label: string; score: number; max: number; color: string }) {
  const percent = Math.round((score / max) * 100)
  return (
    <div className="mb-3">
      <div className="d-flex justify-content-between mb-1">
        <span className="small fw-semibold">{label}</span>
        <span className="small text-muted">{score} / {max}</span>
      </div>
      <div className="progress" style={{ height: 8 }}>
        <div
          className="progress-bar"
          role="progressbar"
          style={{ width: `${percent}%`, backgroundColor: color }}
          aria-valuenow={score}
          aria-valuemin={0}
          aria-valuemax={max}
        />
      </div>
    </div>
  )
}

function CustomerEvaluationModal({ customerId, customerName, onClose }: CustomerEvaluationModalProps) {
  const { data, isLoading, error } = useQuery({
    queryKey: ['customer-evaluation', customerId],
    queryFn: () => customerApi.evaluate(customerId!),
    enabled: customerId !== null && customerId > 0,
  })

  if (customerId === null) return null

  const evaluation = data as CustomerEvaluationDto | undefined
  const levelInfo = evaluation ? (levelColors[evaluation.level] ?? levelColors[2]) : null

  return (
    <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.4)' }} onClick={onClose}>
      <div className="modal-dialog modal-dialog-centered" onClick={e => e.stopPropagation()}>
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
              客户等级评估
              {customerName && <span className="text-muted ms-2 fw-normal">{customerName}</span>}
            </h5>
            <button type="button" className="btn-close" onClick={onClose} />
          </div>
          <div className="modal-body">
            {isLoading && <Loading />}
            {error instanceof Error && <ErrorMessage message={error.message} />}

            {evaluation && levelInfo && (
              <>
                {/* 等级总览 */}
                <div
                  className="text-center p-4 rounded-3 mb-4"
                  style={{ backgroundColor: levelInfo.bg }}
                >
                  <div className="display-4 fw-bold" style={{ color: levelInfo.text }}>
                    {evaluation.totalScore}
                  </div>
                  <div className="fs-5 fw-bold mt-1" style={{ color: levelInfo.text }}>
                    {levelInfo.label}
                  </div>
                  <div className="text-muted small mt-1">
                    满分 100 分
                  </div>
                </div>

                {/* 三维度评分 */}
                <ScoreBar
                  label="合同金额"
                  score={evaluation.contractAmountScore}
                  max={40}
                  color="#2563eb"
                />
                <ScoreBar
                  label="回款及时性"
                  score={evaluation.paymentTimelinessScore}
                  max={40}
                  color="#059669"
                />
                <ScoreBar
                  label="满意度"
                  score={evaluation.satisfactionScore}
                  max={20}
                  color="#ea580c"
                />

                {/* 建议 */}
                <div className="alert alert-info mt-3 mb-0">
                  <strong>评估建议：</strong>{evaluation.suggestion}
                </div>
              </>
            )}
          </div>
          <div className="modal-footer">
            <button className="btn btn-secondary" onClick={onClose}>关闭</button>
          </div>
        </div>
      </div>
    </div>
  )
}

export default CustomerEvaluationModal
