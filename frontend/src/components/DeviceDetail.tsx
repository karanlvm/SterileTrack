import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import axios from 'axios'

interface Device {
  id: string
  deviceIdentifier: string
  name: string
  description: string | null
  manufacturer: string | null
  modelNumber: string | null
  status: string
  lastUsedAt: string | null
  lastSterilizedAt: string | null
  usageCount: number
}

interface StatusHistory {
  id: string
  previousStatus: string
  newStatus: string
  changedAt: string
  changedBy: string | null
  notes: string | null
}

export default function DeviceDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [device, setDevice] = useState<Device | null>(null)
  const [history, setHistory] = useState<StatusHistory[]>([])
  const [loading, setLoading] = useState(true)
  const [newStatus, setNewStatus] = useState<string>('')
  const [notes, setNotes] = useState<string>('')
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    if (id) {
      fetchDevice()
      fetchHistory()
    }
  }, [id])

  const fetchDevice = async () => {
    try {
      const response = await axios.get(`/api/devices/${id}`)
      setDevice(response.data)
      setNewStatus(response.data.status)
    } catch (error) {
      console.error('Error fetching device:', error)
    } finally {
      setLoading(false)
    }
  }

  const fetchHistory = async () => {
    try {
      const response = await axios.get(`/api/devices/${id}/history`)
      setHistory(response.data)
    } catch (error) {
      console.error('Error fetching history:', error)
    }
  }

  const handleStatusUpdate = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!device || !id) return

    if (newStatus === device.status) {
      alert('Please select a different status')
      return
    }

    setSubmitting(true)
    try {
      await axios.put(`/api/devices/${id}/status`, {
        status: parseInt(newStatus),
        notes: notes || undefined,
      }, {
        headers: {
          'X-User-Name': 'Current User',
        },
      })
      await fetchDevice()
      await fetchHistory()
      setNotes('')
      alert('Status updated successfully')
    } catch (error: any) {
      alert(error.response?.data?.message || 'Failed to update status')
    } finally {
      setSubmitting(false)
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

  if (loading) {
    return <div className="page-title">Loading...</div>
  }

  if (!device) {
    return <div className="page-title">Device not found</div>
  }

  return (
    <div>
      <div style={{ marginBottom: '1rem' }}>
        <button onClick={() => navigate('/devices')} className="button">
          ← Back to Devices
        </button>
      </div>
      <h1 className="page-title">Device Details</h1>

      <div className="card">
        <h2 style={{ marginBottom: '1rem' }}>{device.name}</h2>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '1rem', marginBottom: '1.5rem' }}>
          <div>
            <strong>Identifier:</strong> {device.deviceIdentifier}
          </div>
          <div>
            <strong>Status:</strong>{' '}
            <span className={`status-badge ${getStatusBadgeClass(device.status)}`}>
              {device.status}
            </span>
          </div>
          <div>
            <strong>Manufacturer:</strong> {device.manufacturer || '-'}
          </div>
          <div>
            <strong>Model:</strong> {device.modelNumber || '-'}
          </div>
          <div>
            <strong>Usage Count:</strong> {device.usageCount}
          </div>
          <div>
            <strong>Last Used:</strong> {device.lastUsedAt ? new Date(device.lastUsedAt).toLocaleDateString() : '-'}
          </div>
          <div>
            <strong>Last Sterilized:</strong> {device.lastSterilizedAt ? new Date(device.lastSterilizedAt).toLocaleDateString() : '-'}
          </div>
        </div>
        {device.description && (
          <div style={{ marginBottom: '1.5rem' }}>
            <strong>Description:</strong> {device.description}
          </div>
        )}

        <h3 style={{ marginBottom: '1rem', marginTop: '2rem' }}>Update Status</h3>
        <form onSubmit={handleStatusUpdate}>
          <div className="form-group">
            <label htmlFor="status">New Status</label>
            <select
              id="status"
              className="select"
              value={newStatus}
              onChange={(e) => setNewStatus(e.target.value)}
              required
            >
              <option value="1">Available</option>
              <option value="2">In Use</option>
              <option value="3">Pending Sterilization</option>
              <option value="4">Retired</option>
            </select>
          </div>
          <div className="form-group">
            <label htmlFor="notes">Notes (Optional)</label>
            <textarea
              id="notes"
              className="input"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              rows={3}
            />
          </div>
          <button type="submit" className="button button-primary" disabled={submitting || newStatus === device.status}>
            {submitting ? 'Updating...' : 'Update Status'}
          </button>
        </form>
      </div>

      <div className="card">
        <h3 style={{ marginBottom: '1rem' }}>Status History</h3>
        <table className="table">
          <thead>
            <tr>
              <th>Date</th>
              <th>From</th>
              <th>To</th>
              <th>Changed By</th>
              <th>Notes</th>
            </tr>
          </thead>
          <tbody>
            {history.map((item) => (
              <tr key={item.id}>
                <td>{new Date(item.changedAt).toLocaleString()}</td>
                <td>{item.previousStatus}</td>
                <td>{item.newStatus}</td>
                <td>{item.changedBy || '-'}</td>
                <td>{item.notes || '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {history.length === 0 && (
          <p style={{ textAlign: 'center', padding: '2rem' }}>No status history available</p>
        )}
      </div>
    </div>
  )
}
