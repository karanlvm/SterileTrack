namespace SterileTrack.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDeviceRepository Devices { get; }
    IStatusHistoryRepository StatusHistories { get; }
    ISterilizationCycleRepository SterilizationCycles { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
