using SterileTrack.Domain.Interfaces;
using SterileTrack.Infrastructure.Data;

namespace SterileTrack.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SterileTrackDbContext _context;

    private IDeviceRepository? _devices;
    private IStatusHistoryRepository? _statusHistories;
    private ISterilizationCycleRepository? _sterilizationCycles;

    public UnitOfWork(SterileTrackDbContext context)
    {
        _context = context;
    }

    public IDeviceRepository Devices =>
        _devices ??= new DeviceRepository(_context);

    public IStatusHistoryRepository StatusHistories =>
        _statusHistories ??= new StatusHistoryRepository(_context);

    public ISterilizationCycleRepository SterilizationCycles =>
        _sterilizationCycles ??= new SterilizationCycleRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
