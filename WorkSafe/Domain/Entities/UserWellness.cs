namespace HabitFlow.Domain.Entities;

public class UserWellness
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int AverageWellnessScore { get; set; }

    public int TotalHealthChecks { get; set; }

    public int UnresolvedAlerts { get; set; }

    public DateTime? LastCheckAt { get; set; }

    public string Trend { get; set; } = "Estável";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
