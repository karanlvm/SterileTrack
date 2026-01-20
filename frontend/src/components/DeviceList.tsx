import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import axios from 'axios'

interface Device {
  id: string
  deviceIdentifier: string
  name: string
  status: string
  lastUsedAt: string | null
  lastSterilizedAt: string | null
  usageCount: number
}

export default function DeviceList() {
  const [devices, setDevices] = useState<Device[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchDevices()
  }, [])

  const fetchDevices = async () => {
    try {
      const response = await axios.get('/api/devices')
      setDevices(response.data)
    } catch (error) {
      console.error('Error fetching devices:', error)
    } finally {
      setLoading(false)
    }
  }

  const getStatusBadgeClass = (status: string) => {
    const statusMap: { [key: string]: string } = {
      'Available': 'status-available',
      'InUse': 'status-inuse',
      'PendingSterilization': 'status-pending',
      'Retired': 'status-retired',
    }
    return statusMap[status] || ''
  }

  const formatDate = (dateString: string | null) => {
    if (!dateString) return '-'
    return new Date(dateString).toLocaleDateString()
  }

  if (loading) {
    return <div className="page-title">Loading...</div>
  }

  return (
    <div>
      <h1 className="page-title">Device Inventory</h1>
      <div className="card">
        <table className="table">
          <thead>
            <tr>
              <th>Identifier</th>
              <th>Name</th>
              <th>Status</th>
              <th>Usage Count</th>
              <th>Last Used</th>
              <th>Last Sterilized</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {devices.map((device) => (
              <tr key={device.id}>
                <td>{device.deviceIdentifier}</td>
                <td>{device.name}</td>
                <td>
                  <span className={`status-badge ${getStatusBadgeClass(device.status)}`}>
                    {device.status}
                  </span>
                </td>
                <td>{device.usageCount}</td>
                <td>{formatDate(device.lastUsedAt)}</td>
                <td>{formatDate(device.lastSterilizedAt)}</td>
                <td>
                  <Link to={`/devices/${device.id}`} className="button button-primary" style={{ textDecoration: 'none', padding: '0.5rem 1rem', fontSize: '0.875rem' }}>
                    View Details
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {devices.length === 0 && (
          <p style={{ textAlign: 'center', padding: '2rem' }}>No devices found</p>
        )}
      </div>
    </div>
  )
}
