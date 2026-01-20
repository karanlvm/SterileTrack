using SterileTrack.Domain.Entities;

namespace SterileTrack.Application.DTOs;

public class SterilizationCycleDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string? DeviceIdentifier { get; set; }
    public string CycleNumber { get; set; } = string.Empty;
    public SterilizationMethod Method { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public SterilizationStatus Status { get; set; }
    public decimal? Temperature { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
