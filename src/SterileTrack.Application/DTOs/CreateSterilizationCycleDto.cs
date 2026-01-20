using SterileTrack.Domain.Entities;

namespace SterileTrack.Application.DTOs;

public class CreateSterilizationCycleDto
{
    public Guid DeviceId { get; set; }
    public string CycleNumber { get; set; } = string.Empty;
    public SterilizationMethod Method { get; set; }
    public DateTime StartedAt { get; set; }
    public decimal? Temperature { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
}
