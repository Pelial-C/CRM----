import { useState } from 'react'
import ErrorMessage from '../../components/ErrorMessage'
import FormField from '../../components/FormField'
import type { CustomerInput } from '../../types/customer'

interface CustomerFormProps {
  initial?: CustomerInput
  submitText: string
  onSubmit: (input: CustomerInput) => Promise<void>
}

const emptyCustomer: CustomerInput = {
  name: '',
  creditCode: '',
  industry: '',
  province: '',
  city: '',
  district: '',
  detailAddress: '',
  remark: '',
}

function CustomerForm({ initial, submitText, onSubmit }: CustomerFormProps) {
  const [form, setForm] = useState<CustomerInput>(initial ?? emptyCustomer)
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  const update = (name: keyof CustomerInput, value: string) => {
    setForm((current) => ({ ...current, [name]: value }))
  }

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    if (!form.name.trim()) {
      setError('企业名称不能为空')
      return
    }

    setSubmitting(true)
    setError('')
    try {
      await onSubmit(form)
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : '提交失败')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <form className="page-band" onSubmit={handleSubmit}>
      <ErrorMessage message={error} />
      <div className="row">
        <div className="col-md-6">
          <FormField label="企业名称" required>
            <input className="form-control" value={form.name} onChange={(e) => update('name', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-6">
          <FormField label="统一社会信用代码">
            <input
              className="form-control"
              value={form.creditCode ?? ''}
              onChange={(e) => update('creditCode', e.target.value)}
            />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="行业">
            <input className="form-control" value={form.industry ?? ''} onChange={(e) => update('industry', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-2">
          <FormField label="省">
            <input className="form-control" value={form.province ?? ''} onChange={(e) => update('province', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-2">
          <FormField label="市">
            <input className="form-control" value={form.city ?? ''} onChange={(e) => update('city', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-2">
          <FormField label="区">
            <input className="form-control" value={form.district ?? ''} onChange={(e) => update('district', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-12">
          <FormField label="详细地址">
            <input
              className="form-control"
              value={form.detailAddress ?? ''}
              onChange={(e) => update('detailAddress', e.target.value)}
            />
          </FormField>
        </div>
        <div className="col-md-12">
          <FormField label="备注">
            <textarea className="form-control" value={form.remark ?? ''} onChange={(e) => update('remark', e.target.value)} />
          </FormField>
        </div>
      </div>
      <button className="btn btn-primary" type="submit" disabled={submitting}>
        {submitting ? '提交中...' : submitText}
      </button>
    </form>
  )
}

export default CustomerForm
