interface ErrorMessageProps {
  message?: string
}

function ErrorMessage({ message }: ErrorMessageProps) {
  if (!message) return null

  return <div className="alert alert-danger">{message}</div>
}

export default ErrorMessage
