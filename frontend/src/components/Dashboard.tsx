import { useEffect, useState } from 'react'
import axios from 'axios'
import './Dashboard.css'

interface Stats {
  totalDevices: number
  availableDevices: number
  inUseDevices: number
  pendingSterilization: number
}

export default function Dashboard() {
  const [stats, setStats] = useState<Stats | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchStats()
  }, [])

  const fetchStats = async () => {
    try {
      const [allDevices, available, inUse, pending] = await Promise.all([
        axios.get('/api/devices'),
        axios.get('/api/devices/status/1'), // Available
        axios.get('/api/devices/status/2'), // InUse
        axios.get('/api/devices/status/3'), // PendingSterilization
      ])

      setStats({
        totalDevices: allDevices.data.length,
        availableDevices: available.data.length,
        inUseDevices: inUse.data.length,
        pendingSterilization: pending.data.length,
      })
    } catch (error) {
      console.error('Error fetching stats:', error)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return <div className="page-title">Loading...</div>
  }

  return (
    <div>
      <h1 className="page-title">Dashboard</h1>
      <div className="stats-grid">
        <div className="stat-card">
          <h3>Total Devices</h3>
          <p className="stat-value">{stats?.totalDevices || 0}</p>
        </div>
        <div className="stat-card">
          <h3>Available</h3>
          <p className="stat-value">{stats?.availableDevices || 0}</p>
        </div>
        <div className="stat-card">
          <h3>In Use</h3>
          <p className="stat-value">{stats?.inUseDevices || 0}</p>
        </div>
        <div className="stat-card">
          <h3>Pending Sterilization</h3>
          <p className="stat-value">{stats?.pendingSterilization || 0}</p>
        </div>
      </div>
    </div>
  )
}
