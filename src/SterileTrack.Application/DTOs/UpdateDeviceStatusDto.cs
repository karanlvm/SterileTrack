using SterileTrack.Domain.Entities;

namespace SterileTrack.Application.DTOs;

public class UpdateDeviceStatusDto
{
    public DeviceStatus Status { get; set; }
    public string? Notes { get; set; }
}
