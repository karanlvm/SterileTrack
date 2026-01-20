using SterileTrack.Domain.Entities;

namespace SterileTrack.Application.DTOs;

public class StatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public DeviceStatus PreviousStatus { get; set; }
    public DeviceStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public string? Notes { get; set; }
}
