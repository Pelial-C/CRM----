import { useState } from 'react'
import ErrorMessage from '../../components/ErrorMessage'
import FormField from '../../components/FormField'
import type { ContactInput } from '../../types/contact'

interface ContactFormProps {
  initial?: ContactInput
  submitText: string
  onSubmit: (input: ContactInput) => Promise<void>
}

const emptyContact: ContactInput = {
  name: '',
  title: '',
  phone: '',
  email: '',
  isKeyDecisionMaker: false,
}

function ContactForm({ initial, submitText, onSubmit }: ContactFormProps) {
  const [form, setForm] = useState<ContactInput>(initial ?? emptyContact)
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  const update = (name: keyof ContactInput, value: string | boolean) => {
    setForm((current) => ({ ...current, [name]: value }))
  }

  const validate = () => {
    if (!form.name.trim()) return '姓名不能为空'
    if (form.phone && !/^1?\d{7,15}$/.test(form.phone.replace(/[-\s]/g, ''))) return '手机号格式不正确'
    if (form.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) return '邮箱格式不正确'
    return ''
  }

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    const validationError = validate()
    if (validationError) {
      setError(validationError)
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
          <FormField label="姓名" required>
            <input className="form-control" value={form.name} onChange={(e) => update('name', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-6">
          <FormField label="职务">
            <input className="form-control" value={form.title ?? ''} onChange={(e) => update('title', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-6">
          <FormField label="手机号">
            <input className="form-control" value={form.phone ?? ''} onChange={(e) => update('phone', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-6">
          <FormField label="邮箱">
            <input className="form-control" value={form.email ?? ''} onChange={(e) => update('email', e.target.value)} />
          </FormField>
        </div>
        <div className="col-12">
          <div className="form-check mb-3">
            <input
              className="form-check-input"
              id="isKeyDecisionMaker"
              type="checkbox"
              checked={form.isKeyDecisionMaker}
              onChange={(e) => update('isKeyDecisionMaker', e.target.checked)}
            />
            <label className="form-check-label" htmlFor="isKeyDecisionMaker">关键决策人</label>
          </div>
        </div>
      </div>
      <button className="btn btn-primary" type="submit" disabled={submitting}>
        {submitting ? '提交中...' : submitText}
      </button>
    </form>
  )
}

export default ContactForm
