using Microsoft.EntityFrameworkCore;
using SterileTrack.Domain.Entities;

namespace SterileTrack.Infrastructure.Data;

public class SterileTrackDbContext : DbContext
{
    public SterileTrackDbContext(DbContextOptions<SterileTrackDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; }
    public DbSet<StatusHistory> StatusHistories { get; set; }
    public DbSet<SterilizationCycle> SterilizationCycles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Device configuration
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DeviceIdentifier).IsUnique();
            entity.Property(e => e.DeviceIdentifier).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<int>();
        });

        // StatusHistory configuration
        modelBuilder.Entity<StatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DeviceId);
            entity.HasIndex(e => e.ChangedAt);
            entity.Property(e => e.PreviousStatus).HasConversion<int>();
            entity.Property(e => e.NewStatus).HasConversion<int>();
            entity.HasOne(e => e.Device)
                .WithMany(d => d.StatusHistories)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SterilizationCycle configuration
        modelBuilder.Entity<SterilizationCycle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CycleNumber).IsUnique();
            entity.Property(e => e.CycleNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Method).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();
            entity.HasOne(e => e.Device)
                .WithMany(d => d.SterilizationCycles)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).HasConversion<int>();
        });

        // Seed initial admin user
        SeedInitialData(modelBuilder);
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        var adminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminUserId,
            Username = "admin",
            Email = "admin@steriletrack.com",
            PasswordHash = adminPasswordHash,
            FirstName = "System",
            LastName = "Administrator",
            Role = UserRole.Administrator,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }
}
