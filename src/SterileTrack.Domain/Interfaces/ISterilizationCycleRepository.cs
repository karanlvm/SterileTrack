using SterileTrack.Domain.Entities;

namespace SterileTrack.Domain.Interfaces;

public interface ISterilizationCycleRepository : IRepository<SterilizationCycle>
{
    Task<IEnumerable<SterilizationCycle>> GetByDeviceIdAsync(Guid deviceId);
    Task<SterilizationCycle?> GetLatestByDeviceIdAsync(Guid deviceId);
}
