using SterileTrack.Domain.Entities;

namespace SterileTrack.Domain.Interfaces;

public interface IStatusHistoryRepository : IRepository<StatusHistory>
{
    Task<IEnumerable<StatusHistory>> GetByDeviceIdAsync(Guid deviceId);
}
