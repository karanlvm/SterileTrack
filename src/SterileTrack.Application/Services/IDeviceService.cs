using SterileTrack.Application.DTOs;
using SterileTrack.Domain.Entities;

namespace SterileTrack.Application.Services;

public interface IDeviceService
{
    Task<DeviceDto> CreateDeviceAsync(CreateDeviceDto dto);
    Task<DeviceDto?> GetDeviceByIdAsync(Guid id);
    Task<DeviceDto?> GetDeviceByIdentifierAsync(string deviceIdentifier);
    Task<IEnumerable<DeviceDto>> GetAllDevicesAsync();
    Task<IEnumerable<DeviceDto>> GetDevicesByStatusAsync(DeviceStatus status);
    Task<DeviceDto> UpdateDeviceStatusAsync(Guid deviceId, UpdateDeviceStatusDto dto, string? changedBy);
    Task<bool> ValidateDeviceStatusTransitionAsync(Guid deviceId, DeviceStatus newStatus);
    Task<IEnumerable<StatusHistoryDto>> GetDeviceStatusHistoryAsync(Guid deviceId);
}
