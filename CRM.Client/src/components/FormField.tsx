import type { ReactNode } from 'react'

interface FormFieldProps {
  label: string
  required?: boolean
  error?: string
  children: ReactNode
}

function FormField({ label, required, error, children }: FormFieldProps) {
  return (
    <div className="mb-3">
      <label className={`form-label${required ? ' required' : ''}`}>{label}</label>
      {children}
      {error ? <div className="form-text text-danger">{error}</div> : null}
    </div>
  )
}

export default FormField
