using AutoMapper;
using Microsoft.Extensions.Logging;
using SterileTrack.Application.DTOs;
using SterileTrack.Domain.Entities;
using SterileTrack.Domain.Interfaces;

namespace SterileTrack.Application.Services;

public class SterilizationCycleService : ISterilizationCycleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SterilizationCycleService> _logger;

    public SterilizationCycleService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<SterilizationCycleService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SterilizationCycleDto> CreateSterilizationCycleAsync(CreateSterilizationCycleDto dto, string? performedBy)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(dto.DeviceId);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {dto.DeviceId} not found.");
        }

        if (device.Status != DeviceStatus.PendingSterilization)
        {
            throw new InvalidOperationException($"Device can only be sterilized when status is PendingSterilization. Current status: {device.Status}");
        }

        var existingCycle = await _unitOfWork.SterilizationCycles.GetLatestByDeviceIdAsync(dto.DeviceId);
        if (existingCycle != null && existingCycle.Status == SterilizationStatus.InProgress)
        {
            throw new InvalidOperationException($"There is already an in-progress sterilization cycle for this device.");
        }

        var cycle = new SterilizationCycle
        {
            DeviceId = dto.DeviceId,
            CycleNumber = dto.CycleNumber,
            Method = dto.Method,
            StartedAt = dto.StartedAt,
            Temperature = dto.Temperature,
            DurationMinutes = dto.DurationMinutes,
            Notes = dto.Notes,
            PerformedBy = performedBy,
            Status = SterilizationStatus.InProgress
        };

        await _unitOfWork.SterilizationCycles.AddAsync(cycle);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Sterilization cycle {CycleId} created by {PerformedBy}", cycle.Id, performedBy);

        var result = await _unitOfWork.SterilizationCycles.GetByIdAsync(cycle.Id);
        return _mapper.Map<SterilizationCycleDto>(result!);
    }

    public async Task<SterilizationCycleDto?> GetSterilizationCycleByIdAsync(Guid id)
    {
        var cycle = await _unitOfWork.SterilizationCycles.GetByIdAsync(id);
        return cycle != null ? _mapper.Map<SterilizationCycleDto>(cycle) : null;
    }

    public async Task<IEnumerable<SterilizationCycleDto>> GetSterilizationCyclesByDeviceIdAsync(Guid deviceId)
    {
        var cycles = await _unitOfWork.SterilizationCycles.GetByDeviceIdAsync(deviceId);
        return _mapper.Map<IEnumerable<SterilizationCycleDto>>(cycles);
    }

    public async Task<SterilizationCycleDto> CompleteSterilizationCycleAsync(Guid cycleId, CompleteSterilizationCycleDto dto)
    {
        var cycle = await _unitOfWork.SterilizationCycles.GetByIdAsync(cycleId);
        if (cycle == null)
        {
            throw new KeyNotFoundException($"Sterilization cycle with ID {cycleId} not found.");
        }

        if (cycle.Status != SterilizationStatus.InProgress)
        {
            throw new InvalidOperationException($"Sterilization cycle can only be completed when status is InProgress. Current status: {cycle.Status}");
        }

        cycle.Status = SterilizationStatus.Completed;
        cycle.CompletedAt = dto.CompletedAt;
        if (!string.IsNullOrEmpty(dto.Notes))
        {
            cycle.Notes = string.IsNullOrEmpty(cycle.Notes) ? dto.Notes : $"{cycle.Notes}\n{dto.Notes}";
        }

        // Update device status to Available
        var device = await _unitOfWork.Devices.GetByIdAsync(cycle.DeviceId);
        if (device != null)
        {
            device.Status = DeviceStatus.Available;
            device.LastSterilizedAt = dto.CompletedAt;

            // Record status change
            var statusHistory = new StatusHistory
            {
                DeviceId = device.Id,
                PreviousStatus = DeviceStatus.PendingSterilization,
                NewStatus = DeviceStatus.Available,
                ChangedBy = cycle.PerformedBy,
                Notes = "Sterilization cycle completed"
            };

            await _unitOfWork.StatusHistories.AddAsync(statusHistory);
            await _unitOfWork.Devices.UpdateAsync(device);
        }

        await _unitOfWork.SterilizationCycles.UpdateAsync(cycle);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Sterilization cycle {CycleId} completed", cycleId);

        return _mapper.Map<SterilizationCycleDto>(cycle);
    }
}
