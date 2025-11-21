namespace HabitFlow.Domain.Entities;

/// <summary>
/// Enum para tipos de alertas de bem-estar
/// </summary>
public enum AlertType
{
    HighStress,
    PoorSleep,
    LowJobSatisfaction,
    MentalHealthConcern,
    PhysicalHealthConcern
}

/// <summary>
/// Enum para severidade dos alertas
/// </summary>
public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Entidade representando um alerta de bem-estar
/// </summary>
public class WellnessAlert
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public AlertType Type { get; set; }

    public AlertSeverity Severity { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Recommendation { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }

    // Relationships
    public User? User { get; set; }

    public void MarkAsRead()
    {
        IsRead = true;
    }

    public void Resolve()
    {
        ResolvedAt = DateTime.UtcNow;
    }
}
