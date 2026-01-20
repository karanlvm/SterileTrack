using AutoMapper;
using Microsoft.Extensions.Logging;
using SterileTrack.Application.DTOs;
using SterileTrack.Domain.Entities;
using SterileTrack.Domain.Interfaces;

namespace SterileTrack.Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DeviceService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceDto dto)
    {
        var existingDevice = await _unitOfWork.Devices.GetByDeviceIdentifierAsync(dto.DeviceIdentifier);
        if (existingDevice != null)
        {
            throw new InvalidOperationException($"Device with identifier '{dto.DeviceIdentifier}' already exists.");
        }

        var device = new Device
        {
            DeviceIdentifier = dto.DeviceIdentifier,
            Name = dto.Name,
            Description = dto.Description,
            Manufacturer = dto.Manufacturer,
            ModelNumber = dto.ModelNumber,
            IsReusable = dto.IsReusable,
            Status = DeviceStatus.Available
        };

        await _unitOfWork.Devices.AddAsync(device);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Device {DeviceId} created", device.Id);

        return _mapper.Map<DeviceDto>(device);
    }

    public async Task<DeviceDto?> GetDeviceByIdAsync(Guid id)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(id);
        return device != null ? _mapper.Map<DeviceDto>(device) : null;
    }

    public async Task<DeviceDto?> GetDeviceByIdentifierAsync(string deviceIdentifier)
    {
        var device = await _unitOfWork.Devices.GetByDeviceIdentifierAsync(deviceIdentifier);
        return device != null ? _mapper.Map<DeviceDto>(device) : null;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllDevicesAsync()
    {
        var devices = await _unitOfWork.Devices.GetAllAsync();
        return _mapper.Map<IEnumerable<DeviceDto>>(devices);
    }

    public async Task<IEnumerable<DeviceDto>> GetDevicesByStatusAsync(DeviceStatus status)
    {
        var devices = await _unitOfWork.Devices.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<DeviceDto>>(devices);
    }

    public async Task<DeviceDto> UpdateDeviceStatusAsync(Guid deviceId, UpdateDeviceStatusDto dto, string? changedBy)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(deviceId);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {deviceId} not found.");
        }

        if (!await ValidateDeviceStatusTransitionAsync(deviceId, dto.Status))
        {
            throw new InvalidOperationException($"Invalid status transition from {device.Status} to {dto.Status}.");
        }

        var previousStatus = device.Status;

        // Record status change history
        var statusHistory = new StatusHistory
        {
            DeviceId = deviceId,
            PreviousStatus = previousStatus,
            NewStatus = dto.Status,
            ChangedBy = changedBy,
            Notes = dto.Notes
        };

        await _unitOfWork.StatusHistories.AddAsync(statusHistory);

        device.Status = dto.Status;

        if (dto.Status == DeviceStatus.InUse)
        {
            device.LastUsedAt = DateTime.UtcNow;
            device.UsageCount++;
        }

        await _unitOfWork.Devices.UpdateAsync(device);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Device {DeviceId} status changed from {PreviousStatus} to {NewStatus} by {ChangedBy}",
            deviceId, previousStatus, dto.Status, changedBy);

        return _mapper.Map<DeviceDto>(device);
    }

    public async Task<bool> ValidateDeviceStatusTransitionAsync(Guid deviceId, DeviceStatus newStatus)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(deviceId);
        if (device == null)
        {
            return false;
        }

        var validTransitions = new Dictionary<DeviceStatus, List<DeviceStatus>>
        {
            { DeviceStatus.Available, new List<DeviceStatus> { DeviceStatus.InUse, DeviceStatus.Retired } },
            { DeviceStatus.InUse, new List<DeviceStatus> { DeviceStatus.PendingSterilization } },
            { DeviceStatus.PendingSterilization, new List<DeviceStatus> { DeviceStatus.Available } },
            { DeviceStatus.Retired, new List<DeviceStatus>() }
        };

        if (!validTransitions.ContainsKey(device.Status))
        {
            return false;
        }

        return validTransitions[device.Status].Contains(newStatus);
    }

    public async Task<IEnumerable<StatusHistoryDto>> GetDeviceStatusHistoryAsync(Guid deviceId)
    {
        var history = await _unitOfWork.StatusHistories.GetByDeviceIdAsync(deviceId);
        return _mapper.Map<IEnumerable<StatusHistoryDto>>(history);
    }
}
