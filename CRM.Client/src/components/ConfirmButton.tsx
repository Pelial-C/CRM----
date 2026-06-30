interface ConfirmButtonProps {
  className?: string
  message: string
  children: string
  disabled?: boolean
  onConfirm: () => void | Promise<void>
}

function ConfirmButton({ className, message, children, disabled, onConfirm }: ConfirmButtonProps) {
  const handleClick = async () => {
    if (window.confirm(message)) {
      await onConfirm()
    }
  }

  return (
    <button type="button" className={className ?? 'btn btn-outline-danger btn-sm'} disabled={disabled} onClick={handleClick}>
      {children}
    </button>
  )
}

export default ConfirmButton
