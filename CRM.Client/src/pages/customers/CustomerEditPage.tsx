import { useQuery } from '@tanstack/react-query'
import { useNavigate, useParams } from 'react-router-dom'
import { customerApi } from '../../api/customerApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import CustomerForm from './CustomerForm'

function CustomerEditPage() {
  const { id } = useParams()
  const customerId = Number(id)
  const navigate = useNavigate()
  const { data, isLoading, error } = useQuery({
    queryKey: ['customer', customerId],
    queryFn: () => customerApi.get(customerId),
    enabled: customerId > 0,
  })

  return (
    <>
      <PageHeader title="编辑客户" />
      <ErrorMessage message={error instanceof Error ? error.message : undefined} />
      {isLoading ? <Loading /> : null}
      {data ? (
        <CustomerForm
          initial={{
            name: data.name ?? '',
            creditCode: data.creditCode ?? '',
            industry: data.industry ?? '',
            province: data.province ?? '',
            city: data.city ?? '',
            district: data.district ?? '',
            detailAddress: data.detailAddress ?? '',
            remark: data.remark ?? '',
          }}
          submitText="保存修改"
          onSubmit={async (input) => {
            await customerApi.update(customerId, input)
            navigate(`/customers/${customerId}`)
          }}
        />
      ) : null}
    </>
  )
}

export default CustomerEditPage
