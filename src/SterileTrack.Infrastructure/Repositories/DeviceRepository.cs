using Microsoft.EntityFrameworkCore;
using SterileTrack.Domain.Entities;
using SterileTrack.Domain.Interfaces;
using SterileTrack.Infrastructure.Data;

namespace SterileTrack.Infrastructure.Repositories;

public class DeviceRepository : Repository<Device>, IDeviceRepository
{
    public DeviceRepository(SterileTrackDbContext context) : base(context)
    {
    }

    public async Task<Device?> GetByDeviceIdentifierAsync(string deviceIdentifier)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.DeviceIdentifier == deviceIdentifier);
    }

    public async Task<IEnumerable<Device>> GetByStatusAsync(DeviceStatus status)
    {
        return await _dbSet
            .Where(d => d.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Device>> GetDevicesNeedingSterilizationAsync()
    {
        return await _dbSet
            .Where(d => d.Status == DeviceStatus.PendingSterilization && d.IsReusable)
            .ToListAsync();
    }
}
