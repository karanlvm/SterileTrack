using SterileTrack.Domain.Entities;

namespace SterileTrack.Application.DTOs;

public class DeviceDto
{
    public Guid Id { get; set; }
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
    public string? ModelNumber { get; set; }
    public DeviceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime? LastSterilizedAt { get; set; }
    public int UsageCount { get; set; }
    public bool IsReusable { get; set; }
}
