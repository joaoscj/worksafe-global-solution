namespace HabitFlow.Domain.Entities;

/// <summary>
/// Entidade representando um usuário do sistema
/// </summary>
public class User
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ICollection<HealthCheck> HealthChecks { get; set; } = new List<HealthCheck>();
    
    public ICollection<WellnessAlert> WellnessAlerts { get; set; } = new List<WellnessAlert>();
}
