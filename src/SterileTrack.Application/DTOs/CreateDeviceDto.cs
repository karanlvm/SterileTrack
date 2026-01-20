namespace SterileTrack.Application.DTOs;

public class CreateDeviceDto
{
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
    public string? ModelNumber { get; set; }
    public bool IsReusable { get; set; } = true;
}
