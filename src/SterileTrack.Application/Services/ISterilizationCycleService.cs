using SterileTrack.Application.DTOs;

namespace SterileTrack.Application.Services;

public interface ISterilizationCycleService
{
    Task<SterilizationCycleDto> CreateSterilizationCycleAsync(CreateSterilizationCycleDto dto, string? performedBy);
    Task<SterilizationCycleDto?> GetSterilizationCycleByIdAsync(Guid id);
    Task<IEnumerable<SterilizationCycleDto>> GetSterilizationCyclesByDeviceIdAsync(Guid deviceId);
    Task<SterilizationCycleDto> CompleteSterilizationCycleAsync(Guid cycleId, CompleteSterilizationCycleDto dto);
}
