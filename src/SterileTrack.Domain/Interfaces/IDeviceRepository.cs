using SterileTrack.Domain.Entities;

namespace SterileTrack.Domain.Interfaces;

public interface IDeviceRepository : IRepository<Device>
{
    Task<Device?> GetByDeviceIdentifierAsync(string deviceIdentifier);
    Task<IEnumerable<Device>> GetByStatusAsync(DeviceStatus status);
    Task<IEnumerable<Device>> GetDevicesNeedingSterilizationAsync();
}
