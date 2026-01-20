namespace SterileTrack.Domain.Entities;

public class StatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeviceId { get; set; }
    public DeviceStatus PreviousStatus { get; set; }
    public DeviceStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? ChangedBy { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Device Device { get; set; } = null!;
}
