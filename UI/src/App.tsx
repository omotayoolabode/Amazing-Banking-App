import { Navigate, Route, Routes } from 'react-router-dom'
import { CustomersPage } from './pages/CustomersPage'
import { RegisterPage } from './pages/RegisterPage'

function App() {
  return (
    <div className="min-h-screen">
      <Routes>
        <Route path="/" element={<CustomersPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </div>
  )
}

export default App
