namespace SterileTrack.Application.DTOs;

public class CompleteSterilizationCycleDto
{
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
