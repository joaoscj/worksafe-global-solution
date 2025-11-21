namespace HabitFlow.Domain.Entities;

/// <summary>
/// Entidade representando uma avaliação de saúde do usuário
/// </summary>
public class HealthCheck
{
    public int Id { get; set; }

    public int UserId { get; set; }
    
    /// <summary>
    /// Nível de estresse (1-10)
    /// </summary>
    public int StressLevel { get; set; }

    /// <summary>
    /// Qualidade do sono (1-10)
    /// </summary>
    public int SleepQuality { get; set; }

    /// <summary>
    /// Satisfação profissional (1-10)
    /// </summary>
    public int JobSatisfaction { get; set; }

    /// <summary>
    /// Saúde mental (1-10)
    /// </summary>
    public int MentalHealth { get; set; }

    /// <summary>
    /// Saúde física (1-10)
    /// </summary>
    public int PhysicalHealth { get; set; }

    /// <summary>
    /// Observações do usuário
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Data da avaliação
    /// </summary>
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public User? User { get; set; }

    // Business Rules / Invariants
    public bool IsValid()
    {
        return StressLevel >= 1 && StressLevel <= 10 &&
               SleepQuality >= 1 && SleepQuality <= 10 &&
               JobSatisfaction >= 1 && JobSatisfaction <= 10 &&
               MentalHealth >= 1 && MentalHealth <= 10 &&
               PhysicalHealth >= 1 && PhysicalHealth <= 10;
    }

    public int CalculateWellnessScore()
    {
        // Score reverso para estresse (quanto maior, pior)
        var stressScore = (10 - StressLevel) * 2;
        var sleepScore = SleepQuality * 2;
        var jobScore = JobSatisfaction;
        var mentalScore = MentalHealth;
        var physicalScore = PhysicalHealth;

        return (stressScore + sleepScore + jobScore + mentalScore + physicalScore) / 5;
    }

    public bool RequiresAlert()
    {
        return StressLevel >= 8 || 
               SleepQuality <= 3 || 
               JobSatisfaction <= 3 || 
               MentalHealth <= 3 || 
               PhysicalHealth <= 3;
    }
}
