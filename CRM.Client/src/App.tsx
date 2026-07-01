import { Navigate, Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import DashboardPage from './pages/dashboard/DashboardPage'
import CustomerCreatePage from './pages/customers/CustomerCreatePage'
import CustomerDetailPage from './pages/customers/CustomerDetailPage'
import CustomerEditPage from './pages/customers/CustomerEditPage'
import CustomerListPage from './pages/customers/CustomerListPage'
import ContactCreatePage from './pages/contacts/ContactCreatePage'
import ContactEditPage from './pages/contacts/ContactEditPage'
import ContractCreatePage from './pages/contracts/ContractCreatePage'
import ContractDetailPage from './pages/contracts/ContractDetailPage'
import ContractEditPage from './pages/contracts/ContractEditPage'
import ContractListPage from './pages/contracts/ContractListPage'
import ApprovalPage from './pages/approvals/ApprovalPage'
import RiskPage from './pages/risks/RiskPage'
import ReportPage from './pages/reports/ReportPage'
import AboutPage from './pages/system/AboutPage'

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/customers" element={<CustomerListPage />} />
        <Route path="/customers/create" element={<CustomerCreatePage />} />
        <Route path="/customers/:id" element={<CustomerDetailPage />} />
        <Route path="/customers/:id/edit" element={<CustomerEditPage />} />
        <Route path="/customers/:id/contacts/create" element={<ContactCreatePage />} />
        <Route path="/contacts/:id/edit" element={<ContactEditPage />} />
        <Route path="/contracts" element={<ContractListPage />} />
        <Route path="/contracts/create" element={<ContractCreatePage />} />
        <Route path="/contracts/:id" element={<ContractDetailPage />} />
        <Route path="/contracts/:id/edit" element={<ContractEditPage />} />
        <Route path="/approvals" element={<ApprovalPage />} />
        <Route path="/risks" element={<RiskPage />} />
        <Route path="/reports" element={<ReportPage />} />
        <Route path="/about" element={<AboutPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  )
}

export default App
