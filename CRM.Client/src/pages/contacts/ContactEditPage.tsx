import { useQuery } from '@tanstack/react-query'
import { useNavigate, useParams } from 'react-router-dom'
import { contactApi } from '../../api/contactApi'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import ContactForm from './ContactForm'

function ContactEditPage() {
  const { id } = useParams()
  const contactId = Number(id)
  const navigate = useNavigate()
  const { data, isLoading, error } = useQuery({
    queryKey: ['contact', contactId],
    queryFn: () => contactApi.get(contactId),
    enabled: contactId > 0,
  })

  return (
    <>
      <PageHeader title="编辑联系人" />
      <ErrorMessage message={error instanceof Error ? error.message : undefined} />
      {isLoading ? <Loading /> : null}
      {data ? (
        <ContactForm
          initial={{
            name: data.name,
            title: data.title ?? '',
            phone: data.phone ?? '',
            email: data.email ?? '',
            isKeyDecisionMaker: data.isKeyDecisionMaker,
          }}
          submitText="保存修改"
          onSubmit={async (input) => {
            await contactApi.update(contactId, input)
            navigate(`/customers/${data.customerId}`)
          }}
        />
      ) : null}
    </>
  )
}

export default ContactEditPage
