import { useNavigate, useParams } from 'react-router-dom'
import { customerApi } from '../../api/customerApi'
import PageHeader from '../../components/PageHeader'
import ContactForm from './ContactForm'

function ContactCreatePage() {
  const { id } = useParams()
  const customerId = Number(id)
  const navigate = useNavigate()

  return (
    <>
      <PageHeader title="新增联系人" />
      <ContactForm
        submitText="保存联系人"
        onSubmit={async (input) => {
          await customerApi.createContact(customerId, input)
          navigate(`/customers/${customerId}`)
        }}
      />
    </>
  )
}

export default ContactCreatePage
