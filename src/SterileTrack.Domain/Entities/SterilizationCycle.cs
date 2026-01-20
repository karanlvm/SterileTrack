namespace SterileTrack.Domain.Entities;

public class SterilizationCycle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeviceId { get; set; }
    public string CycleNumber { get; set; } = string.Empty;
    public SterilizationMethod Method { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public SterilizationStatus Status { get; set; } = SterilizationStatus.InProgress;
    public decimal? Temperature { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Device Device { get; set; } = null!;
}
