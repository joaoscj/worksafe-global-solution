namespace HabitFlow.Domain.Entities;

/// <summary>
/// Entidade representando uma dica de bem-estar personalizada
/// </summary>
public class WellnessTip
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Category { get; set; } // ex: "Sleep", "Stress", "Exercise", "Nutrition"

    public int MinWellnessScore { get; set; } = 0;

    public int MaxWellnessScore { get; set; } = 100;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
