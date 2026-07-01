import { useEffect, useMemo, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { contractApi } from '../../api/contractApi'
import { customerApi } from '../../api/customerApi'
import { foundationApi } from '../../api/foundationApi'
import ErrorMessage from '../../components/ErrorMessage'
import FormField from '../../components/FormField'
import Loading from '../../components/Loading'
import type { ContractInput, ContractItemDto } from '../../types/contract'
import { todayString } from '../../utils/date'

interface ContractFormProps {
  initial?: ContractInput
  submitText: string
  onSubmit: (input: ContractInput) => Promise<void>
}

const emptyItem: ContractItemDto = {
  productName: '',
  quantity: 1,
  unitPrice: 0,
  subtotal: 0,
}

const emptyContract: ContractInput = {
  contractNo: '',
  contractName: '',
  cabinetNo: '',
  customerId: 0,
  contactId: null,
  signDate: todayString(),
  startDate: todayString(),
  endDate: todayString(),
  totalAmount: 0,
  paymentFrequency: 1,
  serviceType: 0,
  contractType: 0,
  warningDays: 30,
  regionalCompany: '',
  affiliatedCompany: '',
  remark: '',
  items: [{ ...emptyItem }],
}

function ContractForm({ initial, submitText, onSubmit }: ContractFormProps) {
  const [form, setForm] = useState<ContractInput>(initial ?? emptyContract)
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const customers = useQuery({ queryKey: ['customer-select'], queryFn: customerApi.getSelectList })
  const enums = useQuery({ queryKey: ['foundation-enums'], queryFn: foundationApi.getEnums })
  const contacts = useQuery({
    queryKey: ['contract-customer-contacts', form.customerId],
    queryFn: () => contractApi.getCustomerContacts(form.customerId),
    enabled: form.customerId > 0,
  })

  useEffect(() => {
    if (initial) setForm(initial)
  }, [initial])

  const itemsTotal = useMemo(
    () => form.items.reduce((sum, item) => sum + Number(item.quantity || 0) * Number(item.unitPrice || 0), 0),
    [form.items],
  )

  const update = (name: keyof ContractInput, value: string | number | null) => {
    setForm((current) => ({ ...current, [name]: value }))
  }

  const updateItem = (index: number, name: keyof ContractItemDto, value: string | number) => {
    setForm((current) => {
      const items = current.items.map((item, itemIndex) => {
        if (itemIndex !== index) return item
        const next = { ...item, [name]: value }
        next.subtotal = Number(next.quantity || 0) * Number(next.unitPrice || 0)
        return next
      })
      return { ...current, items }
    })
  }

  const validate = () => {
    if (!form.contractNo.trim()) return '合同编号不能为空'
    if (!form.contractName.trim()) return '合同名称不能为空'
    if (!form.customerId) return '必须选择客户'
    if (Number(form.totalAmount) <= 0) return '合同金额必须大于0'
    if (form.endDate < form.startDate) return '结束日期不能早于开始日期'
    if (!form.items.length) return '至少添加一项合同明细'
    for (const item of form.items) {
      if (!item.productName?.trim()) return '明细产品/服务名称不能为空'
      if (Number(item.quantity) <= 0) return '明细数量必须大于0'
      if (Number(item.unitPrice) < 0) return '明细单价不能小于0'
    }
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
      await onSubmit({
        ...form,
        totalAmount: Number(form.totalAmount),
        customerId: Number(form.customerId),
        contactId: form.contactId ? Number(form.contactId) : null,
        paymentFrequency: Number(form.paymentFrequency),
        serviceType: Number(form.serviceType),
        contractType: Number(form.contractType),
        warningDays: Number(form.warningDays),
        items: form.items.map((item) => ({
          ...item,
          quantity: Number(item.quantity),
          unitPrice: Number(item.unitPrice),
          subtotal: Number(item.quantity) * Number(item.unitPrice),
        })),
      })
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : '提交失败')
    } finally {
      setSubmitting(false)
    }
  }

  if (customers.isLoading || enums.isLoading) return <Loading />

  return (
    <form className="page-band" onSubmit={handleSubmit}>
      <ErrorMessage message={error || (customers.error instanceof Error ? customers.error.message : undefined)} />
      <div className="row">
        <div className="col-md-4">
          <FormField label="合同编号" required>
            <input className="form-control" value={form.contractNo} onChange={(e) => update('contractNo', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="合同名称" required>
            <input className="form-control" value={form.contractName} onChange={(e) => update('contractName', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="合同柜编号">
            <input className="form-control" value={form.cabinetNo ?? ''} onChange={(e) => update('cabinetNo', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="客户" required>
            <select
              className="form-select"
              value={form.customerId}
              onChange={(e) => setForm((current) => ({ ...current, customerId: Number(e.target.value), contactId: null }))}
            >
              <option value={0}>请选择客户</option>
              {customers.data?.map((customer) => (
                <option key={customer.id} value={customer.id}>{customer.name}</option>
              ))}
            </select>
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="联系人">
            <select className="form-select" value={form.contactId ?? ''} onChange={(e) => update('contactId', e.target.value ? Number(e.target.value) : null)}>
              <option value="">请选择联系人</option>
              {contacts.data?.map((contact) => (
                <option key={contact.contactId} value={contact.contactId}>{contact.contactName}</option>
              ))}
            </select>
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="合同总金额" required>
            <div className="input-group">
              <input className="form-control" type="number" value={form.totalAmount} onChange={(e) => update('totalAmount', Number(e.target.value))} />
              <button className="btn btn-outline-secondary" type="button" onClick={() => update('totalAmount', Number(itemsTotal.toFixed(2)))}>
                按明细填充
              </button>
            </div>
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="签订日期" required>
            <input className="form-control" type="date" value={form.signDate} onChange={(e) => update('signDate', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="开始日期" required>
            <input className="form-control" type="date" value={form.startDate} onChange={(e) => update('startDate', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="结束日期" required>
            <input className="form-control" type="date" value={form.endDate} onChange={(e) => update('endDate', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-3">
          <FormField label="回款频率">
            <select className="form-select" value={form.paymentFrequency} onChange={(e) => update('paymentFrequency', Number(e.target.value))}>
              {enums.data?.paymentFrequencies.map((item) => <option key={item.value} value={item.value}>{item.label}</option>)}
            </select>
          </FormField>
        </div>
        <div className="col-md-3">
          <FormField label="服务类型">
            <select className="form-select" value={form.serviceType} onChange={(e) => update('serviceType', Number(e.target.value))}>
              {enums.data?.serviceTypes.map((item) => <option key={item.value} value={item.value}>{item.label}</option>)}
            </select>
          </FormField>
        </div>
        <div className="col-md-3">
          <FormField label="合同类型">
            <select className="form-select" value={form.contractType} onChange={(e) => update('contractType', Number(e.target.value))}>
              {enums.data?.contractTypes.map((item) => <option key={item.value} value={item.value}>{item.label}</option>)}
            </select>
          </FormField>
        </div>
        <div className="col-md-3">
          <FormField label="预警天数">
            <input className="form-control" type="number" value={form.warningDays} onChange={(e) => update('warningDays', Number(e.target.value))} />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="区域公司">
            <input className="form-control" value={form.regionalCompany ?? ''} onChange={(e) => update('regionalCompany', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="所属公司">
            <input className="form-control" value={form.affiliatedCompany ?? ''} onChange={(e) => update('affiliatedCompany', e.target.value)} />
          </FormField>
        </div>
        <div className="col-md-4">
          <FormField label="备注">
            <input className="form-control" value={form.remark ?? ''} onChange={(e) => update('remark', e.target.value)} />
          </FormField>
        </div>
      </div>

      <div className="d-flex justify-content-between align-items-center mb-2">
        <h2 className="h5 mb-0">合同明细</h2>
        <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => setForm((current) => ({ ...current, items: [...current.items, { ...emptyItem }] }))}>
          新增明细
        </button>
      </div>
      <div className="table-responsive mb-3">
        <table className="table table-sm">
          <thead><tr><th>产品/服务名称</th><th>数量</th><th>单价</th><th>小计</th><th>操作</th></tr></thead>
          <tbody>
            {form.items.map((item, index) => (
              <tr key={index}>
                <td><input className="form-control form-control-sm" value={item.productName ?? ''} onChange={(e) => updateItem(index, 'productName', e.target.value)} /></td>
                <td><input className="form-control form-control-sm" type="number" value={item.quantity} onChange={(e) => updateItem(index, 'quantity', Number(e.target.value))} /></td>
                <td><input className="form-control form-control-sm" type="number" value={item.unitPrice} onChange={(e) => updateItem(index, 'unitPrice', Number(e.target.value))} /></td>
                <td>{(Number(item.quantity) * Number(item.unitPrice)).toFixed(2)}</td>
                <td>
                  <button
                    className="btn btn-sm btn-outline-danger"
                    type="button"
                    disabled={form.items.length <= 1}
                    onClick={() => setForm((current) => ({ ...current, items: current.items.filter((_, itemIndex) => itemIndex !== index) }))}
                  >
                    删除
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <button className="btn btn-primary" type="submit" disabled={submitting}>
        {submitting ? '提交中...' : submitText}
      </button>
    </form>
  )
}

export default ContractForm
