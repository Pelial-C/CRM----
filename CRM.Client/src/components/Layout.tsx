import type { ReactNode } from 'react'
import AppNavbar from './AppNavbar'

interface LayoutProps {
  children: ReactNode
}

function Layout({ children }: LayoutProps) {
  return (
    <div className="app-shell">
      <AppNavbar />
      <main className="page-main container-fluid">
        <div className="page-container">{children}</div>
      </main>
    </div>
  )
}

export default Layout
