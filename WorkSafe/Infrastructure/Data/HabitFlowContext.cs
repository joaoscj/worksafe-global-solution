using HabitFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitFlow.Infrastructure.Data;

public class HabitFlowContext : DbContext
{
    public HabitFlowContext(DbContextOptions<HabitFlowContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<HealthCheck> HealthChecks { get; set; }
    public DbSet<WellnessAlert> WellnessAlerts { get; set; }
    public DbSet<WellnessTip> WellnessTips { get; set; }
    public DbSet<UserWellness> UserWellness { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(e => e.HealthChecks)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.WellnessAlerts)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HealthCheck>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasOne(e => e.User)
                .WithMany(e => e.HealthChecks)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WellnessAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Recommendation).HasMaxLength(1000);
            entity.HasOne(e => e.User)
                .WithMany(e => e.WellnessAlerts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WellnessTip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Category).HasMaxLength(50);
        });

        modelBuilder.Entity<UserWellness>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Trend).HasMaxLength(50);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.UserId).IsUnique();
        });
    }
}
