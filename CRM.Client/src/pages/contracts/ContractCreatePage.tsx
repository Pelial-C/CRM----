import { useNavigate } from 'react-router-dom'
import { contractApi } from '../../api/contractApi'
import PageHeader from '../../components/PageHeader'
import ContractForm from './ContractForm'

function ContractCreatePage() {
  const navigate = useNavigate()

  return (
    <>
      <PageHeader title="新增合同" />
      <ContractForm
        submitText="保存合同"
        onSubmit={async (input) => {
          await contractApi.create(input)
          navigate('/contracts')
        }}
      />
    </>
  )
}

export default ContractCreatePage
