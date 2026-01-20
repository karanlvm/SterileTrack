namespace SterileTrack.Domain.Entities;

public class Device
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
    public string? ModelNumber { get; set; }
    public DeviceStatus Status { get; set; } = DeviceStatus.Available;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
    public DateTime? LastSterilizedAt { get; set; }
    public int UsageCount { get; set; } = 0;
    public bool IsReusable { get; set; } = true;

    // Navigation properties
    public virtual ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
    public virtual ICollection<SterilizationCycle> SterilizationCycles { get; set; } = new List<SterilizationCycle>();
}
