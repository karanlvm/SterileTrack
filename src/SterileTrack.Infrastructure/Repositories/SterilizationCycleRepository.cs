using Microsoft.EntityFrameworkCore;
using SterileTrack.Domain.Interfaces;
using SterileTrack.Infrastructure.Data;

namespace SterileTrack.Infrastructure.Repositories;

public class SterilizationCycleRepository : Repository<Domain.Entities.SterilizationCycle>, ISterilizationCycleRepository
{
    public SterilizationCycleRepository(SterileTrackDbContext context) : base(context)
    {
    }

    public override async Task<Domain.Entities.SterilizationCycle?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Device)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public override async Task<IEnumerable<Domain.Entities.SterilizationCycle>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Device)
            .ToListAsync();
    }

    public async Task<IEnumerable<Domain.Entities.SterilizationCycle>> GetByDeviceIdAsync(Guid deviceId)
    {
        return await _dbSet
            .Include(s => s.Device)
            .Where(s => s.DeviceId == deviceId)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync();
    }

    public async Task<Domain.Entities.SterilizationCycle?> GetLatestByDeviceIdAsync(Guid deviceId)
    {
        return await _dbSet
            .Include(s => s.Device)
            .Where(s => s.DeviceId == deviceId)
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync();
    }
}
