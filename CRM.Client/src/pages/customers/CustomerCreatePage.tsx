import { useNavigate } from 'react-router-dom'
import { customerApi } from '../../api/customerApi'
import PageHeader from '../../components/PageHeader'
import CustomerForm from './CustomerForm'

function CustomerCreatePage() {
  const navigate = useNavigate()

  return (
    <>
      <PageHeader title="新增客户" />
      <CustomerForm
        submitText="保存客户"
        onSubmit={async (input) => {
          await customerApi.create(input)
          navigate('/customers')
        }}
      />
    </>
  )
}

export default CustomerCreatePage
