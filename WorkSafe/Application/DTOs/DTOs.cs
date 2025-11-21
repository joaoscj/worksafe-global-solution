using System.ComponentModel.DataAnnotations;

namespace HabitFlow.Application.DTOs;

public class HealthCheckDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int StressLevel { get; set; }
    public int SleepQuality { get; set; }
    public int JobSatisfaction { get; set; }
    public int MentalHealth { get; set; }
    public int PhysicalHealth { get; set; }
    public string? Notes { get; set; }
    public DateTime CheckedAt { get; set; }
    public int WellnessScore { get; set; }
}

public class CreateHealthCheckDto
{
    [Required(ErrorMessage = "ID do usuário é obrigatório")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Nível de estresse é obrigatório")]
    [Range(1, 10, ErrorMessage = "Nível de estresse deve estar entre 1 e 10")]
    public int StressLevel { get; set; }

    [Required(ErrorMessage = "Qualidade do sono é obrigatória")]
    [Range(1, 10, ErrorMessage = "Qualidade do sono deve estar entre 1 e 10")]
    public int SleepQuality { get; set; }

    [Required(ErrorMessage = "Satisfação profissional é obrigatória")]
    [Range(1, 10, ErrorMessage = "Satisfação profissional deve estar entre 1 e 10")]
    public int JobSatisfaction { get; set; }

    [Required(ErrorMessage = "Saúde mental é obrigatória")]
    [Range(1, 10, ErrorMessage = "Saúde mental deve estar entre 1 e 10")]
    public int MentalHealth { get; set; }

    [Required(ErrorMessage = "Saúde física é obrigatória")]
    [Range(1, 10, ErrorMessage = "Saúde física deve estar entre 1 e 10")]
    public int PhysicalHealth { get; set; }

    [MaxLength(500, ErrorMessage = "Notas não podem ter mais de 500 caracteres")]
    public string? Notes { get; set; }
}

public class UpdateHealthCheckDto
{
    [Required(ErrorMessage = "Nível de estresse é obrigatório")]
    [Range(1, 10, ErrorMessage = "Nível de estresse deve estar entre 1 e 10")]
    public int StressLevel { get; set; }

    [Required(ErrorMessage = "Qualidade do sono é obrigatória")]
    [Range(1, 10, ErrorMessage = "Qualidade do sono deve estar entre 1 e 10")]
    public int SleepQuality { get; set; }

    [Required(ErrorMessage = "Satisfação profissional é obrigatória")]
    [Range(1, 10, ErrorMessage = "Satisfação profissional deve estar entre 1 e 10")]
    public int JobSatisfaction { get; set; }

    [Required(ErrorMessage = "Saúde mental é obrigatória")]
    [Range(1, 10, ErrorMessage = "Saúde mental deve estar entre 1 e 10")]
    public int MentalHealth { get; set; }

    [Required(ErrorMessage = "Saúde física é obrigatória")]
    [Range(1, 10, ErrorMessage = "Saúde física deve estar entre 1 e 10")]
    public int PhysicalHealth { get; set; }

    [MaxLength(500, ErrorMessage = "Notas não podem ter mais de 500 caracteres")]
    public string? Notes { get; set; }
}

public class WellnessAlertDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Recommendation { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class UserWellnessDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AverageWellnessScore { get; set; }
    public int TotalHealthChecks { get; set; }
    public int UnresolvedAlerts { get; set; }
    public DateTime? LastCheckAt { get; set; }
    public string Trend { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "Email não pode ter mais de 100 caracteres")]
    public string Email { get; set; } = string.Empty;
}

public class WellnessTipDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int MinWellnessScore { get; set; }
    public int MaxWellnessScore { get; set; }
}

/// <summary>
/// DTO para resposta paginada de resultados
/// </summary>
public class PaginatedResponse<T>
{
    [Required]
    public List<T> Items { get; set; } = new();

    [Required]
    public int TotalCount { get; set; }

    [Required]
    public int PageNumber { get; set; }

    [Required]
    public int PageSize { get; set; }

    [Required]
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}

/// <summary>
/// DTO para filtros de busca de verificações de saúde
/// </summary>
public class HealthCheckSearchDto
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Página deve ser maior que 0")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 10;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [StringLength(50, ErrorMessage = "Campo de ordenação inválido")]
    public string SortBy { get; set; } = "CheckedAt";

    [StringLength(50, ErrorMessage = "Direção de ordenação deve ser 'asc' ou 'desc'")]
    public string SortDirection { get; set; } = "desc";
}
