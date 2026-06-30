import { useQuery } from '@tanstack/react-query'
import { useNavigate, useParams } from 'react-router-dom'
import { contractApi } from '../../api/contractApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import { formatDate } from '../../utils/date'
import ContractForm from './ContractForm'

function ContractEditPage() {
  const { id } = useParams()
  const contractId = Number(id)
  const navigate = useNavigate()
  const { data, isLoading, error } = useQuery({
    queryKey: ['contract', contractId],
    queryFn: () => contractApi.get(contractId),
    enabled: contractId > 0,
  })

  return (
    <>
      <PageHeader title="编辑合同" />
      <ErrorMessage message={error instanceof Error ? error.message : undefined} />
      {isLoading ? <Loading /> : null}
      {data ? (
        <ContractForm
          initial={{
            contractNo: data.contractNo ?? '',
            contractName: data.contractName ?? '',
            cabinetNo: data.cabinetNo ?? '',
            customerId: data.customerId,
            contactId: data.contactId ?? null,
            signDate: formatDate(data.signDate),
            startDate: formatDate(data.startDate),
            endDate: formatDate(data.endDate),
            totalAmount: data.totalAmount,
            paymentFrequency: data.paymentFrequency,
            serviceType: data.serviceType,
            contractType: data.contractType,
            warningDays: data.warningDays,
            regionalCompany: data.regionalCompany ?? '',
            affiliatedCompany: data.affiliatedCompany ?? '',
            remark: data.remark ?? '',
            items: data.items.length > 0 ? data.items : [{ productName: '', quantity: 1, unitPrice: 0, subtotal: 0 }],
          }}
          submitText="保存修改"
          onSubmit={async (input) => {
            await contractApi.update(contractId, input)
            navigate(`/contracts/${contractId}`)
          }}
        />
      ) : null}
    </>
  )
}

export default ContractEditPage
