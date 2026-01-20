using Microsoft.EntityFrameworkCore;
using SterileTrack.Domain.Interfaces;
using SterileTrack.Infrastructure.Data;

namespace SterileTrack.Infrastructure.Repositories;

public class StatusHistoryRepository : Repository<Domain.Entities.StatusHistory>, IStatusHistoryRepository
{
    public StatusHistoryRepository(SterileTrackDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Domain.Entities.StatusHistory>> GetByDeviceIdAsync(Guid deviceId)
    {
        return await _dbSet
            .Where(h => h.DeviceId == deviceId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }
}
