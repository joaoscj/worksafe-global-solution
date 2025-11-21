# ?? GUIA DE DESENVOLVIMENTO - WorkSafe

## Como Adicionar Nova Funcionalidade

### Exemplo: Adicionar suporte a "Histórico de Exercícios"

#### 1. Criar a Entidade (Domain Layer)

Arquivo: `Domain/Entities/ExerciseHistory.cs`

```csharp
namespace HabitFlow.Domain.Entities;

public class ExerciseHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ExerciseType { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int Intensity { get; set; } // 1-10
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relationships
    public User? User { get; set; }
    
    // Business Rules
    public bool IsValid() => Intensity >= 1 && Intensity <= 10 && DurationMinutes > 0;
}
```

#### 2. Criar DTOs (Application Layer)

Arquivo: `Application/DTOs/DTOs.cs`

```csharp
public class ExerciseHistoryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ExerciseType { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int Intensity { get; set; }
    public DateTime PerformedAt { get; set; }
}

public class CreateExerciseHistoryDto
{
    [Required]
    [StringLength(100)]
    public string ExerciseType { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 480)]
    public int DurationMinutes { get; set; }
    
    [Required]
    [Range(1, 10)]
    public int Intensity { get; set; }
}
```

#### 3. Criar Interface de Repositório (Infrastructure)

Arquivo: `Infrastructure/Repositories/IRepositories.cs`

```csharp
public interface IExerciseHistoryRepository
{
    Task<ExerciseHistory?> GetByIdAsync(int id);
    Task<IEnumerable<ExerciseHistory>> GetByUserIdAsync(int userId);
    Task<(List<ExerciseHistory> Items, int TotalCount)> GetPaginatedByUserIdAsync(
        int userId, int pageNumber = 1, int pageSize = 10);
    Task AddAsync(ExerciseHistory history);
    Task UpdateAsync(ExerciseHistory history);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}
```

#### 4. Implementar Repositório

Arquivo: `Infrastructure/Repositories/Implementations.cs`

```csharp
public class ExerciseHistoryRepository : IExerciseHistoryRepository
{
    private readonly HabitFlowContext _context;

    public ExerciseHistoryRepository(HabitFlowContext context)
    {
        _context = context;
    }

    public async Task<ExerciseHistory?> GetByIdAsync(int id)
    {
        return await _context.ExerciseHistories.FindAsync(id);
    }

    public async Task<IEnumerable<ExerciseHistory>> GetByUserIdAsync(int userId)
    {
        return await _context.ExerciseHistories
            .Where(eh => eh.UserId == userId)
            .OrderByDescending(eh => eh.PerformedAt)
            .ToListAsync();
    }

    public async Task<(List<ExerciseHistory> Items, int TotalCount)> GetPaginatedByUserIdAsync(
        int userId, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.ExerciseHistories.Where(eh => eh.UserId == userId);
        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(eh => eh.PerformedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(ExerciseHistory history)
    {
        if (!history.IsValid())
            throw new InvalidOperationException("Exercise history has invalid values.");
        
        await _context.ExerciseHistories.AddAsync(history);
    }

    public async Task UpdateAsync(ExerciseHistory history)
    {
        if (!history.IsValid())
            throw new InvalidOperationException("Exercise history has invalid values.");
        
        _context.ExerciseHistories.Update(history);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var history = await GetByIdAsync(id);
        if (history != null)
        {
            _context.ExerciseHistories.Remove(history);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
```

#### 5. Criar Serviço (Application Layer)

Arquivo: `Application/Services/ExerciseHistoryService.cs`

```csharp
using HabitFlow.Application.DTOs;
using HabitFlow.Domain.Entities;
using HabitFlow.Infrastructure.Repositories;

namespace HabitFlow.Application.Services;

public interface IExerciseHistoryService
{
    Task<ExerciseHistoryDto?> GetByIdAsync(int id);
    Task<IEnumerable<ExerciseHistoryDto>> GetByUserIdAsync(int userId);
    Task<ExerciseHistoryDto> CreateAsync(CreateExerciseHistoryDto dto);
    Task<ExerciseHistoryDto> UpdateAsync(int id, CreateExerciseHistoryDto dto);
    Task DeleteAsync(int id);
}

public class ExerciseHistoryService : IExerciseHistoryService
{
    private readonly IExerciseHistoryRepository _repository;

    public ExerciseHistoryService(IExerciseHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExerciseHistoryDto?> GetByIdAsync(int id)
    {
        var history = await _repository.GetByIdAsync(id);
        return history != null ? MapToDto(history) : null;
    }

    public async Task<IEnumerable<ExerciseHistoryDto>> GetByUserIdAsync(int userId)
    {
        var histories = await _repository.GetByUserIdAsync(userId);
        return histories.Select(MapToDto);
    }

    public async Task<ExerciseHistoryDto> CreateAsync(CreateExerciseHistoryDto dto)
    {
        var history = new ExerciseHistory
        {
            UserId = dto.UserId,
            ExerciseType = dto.ExerciseType,
            DurationMinutes = dto.DurationMinutes,
            Intensity = dto.Intensity
        };

        await _repository.AddAsync(history);
        await _repository.SaveChangesAsync();

        return MapToDto(history);
    }

    public async Task<ExerciseHistoryDto> UpdateAsync(int id, CreateExerciseHistoryDto dto)
    {
        var history = await _repository.GetByIdAsync(id);
        if (history == null)
            throw new KeyNotFoundException("Exercise history not found.");

        history.ExerciseType = dto.ExerciseType;
        history.DurationMinutes = dto.DurationMinutes;
        history.Intensity = dto.Intensity;

        await _repository.UpdateAsync(history);
        await _repository.SaveChangesAsync();

        return MapToDto(history);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
    }

    private ExerciseHistoryDto MapToDto(ExerciseHistory history)
    {
        return new ExerciseHistoryDto
        {
            Id = history.Id,
            UserId = history.UserId,
            ExerciseType = history.ExerciseType,
            DurationMinutes = history.DurationMinutes,
            Intensity = history.Intensity,
            PerformedAt = history.PerformedAt
        };
    }
}
```

#### 6. Registrar no DbContext

Arquivo: `Infrastructure/Data/HabitFlowContext.cs`

```csharp
public DbSet<ExerciseHistory> ExerciseHistories { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ... existing code ...
    
    modelBuilder.Entity<ExerciseHistory>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.ExerciseType).IsRequired().HasMaxLength(100);
        entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    });
}
```

#### 7. Registrar Dependências

Arquivo: `Program.cs`

```csharp
builder.Services.AddScoped<IExerciseHistoryRepository, ExerciseHistoryRepository>();
builder.Services.AddScoped<IExerciseHistoryService, ExerciseHistoryService>();
```

#### 8. Criar Migration

```bash
dotnet ef migrations add AddExerciseHistory
dotnet ef database update
```

#### 9. Criar Razor Page

Arquivo: `Pages/Exercises/Index.cshtml.cs`

```csharp
using HabitFlow.Application.DTOs;
using HabitFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HabitFlow.Pages.Exercises;

public class IndexModel : PageModel
{
    private readonly IExerciseHistoryService _service;

    [BindProperty(SupportsGet = true)]
    public int UserId { get; set; } = 1;

    public IEnumerable<ExerciseHistoryDto> Exercises { get; set; } = new List<ExerciseHistoryDto>();

    public IndexModel(IExerciseHistoryService service)
    {
        _service = service;
    }

    public async Task OnGetAsync()
    {
        Exercises = await _service.GetByUserIdAsync(UserId);
    }
}
```

#### 10. Criar View

Arquivo: `Pages/Exercises/Index.cshtml`

```razor
@page
@model HabitFlow.Pages.Exercises.IndexModel
@{
    ViewData["Title"] = "Histórico de Exercícios";
}

<div class="container mt-4">
    <h1>Histórico de Exercícios</h1>
    
    @if (Model.Exercises.Any())
    {
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Tipo</th>
                    <th>Duração (min)</th>
                    <th>Intensidade</th>
                    <th>Data</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var exercise in Model.Exercises)
                {
                    <tr>
                        <td>@exercise.ExerciseType</td>
                        <td>@exercise.DurationMinutes</td>
                        <td>@exercise.Intensity/10</td>
                        <td>@exercise.PerformedAt.ToString("dd/MM/yyyy HH:mm")</td>
                    </tr>
                }
            </tbody>
        </table>
    }
    else
    {
        <p>Nenhum exercício registrado.</p>
    }
</div>
```

---

## Padrões de Código

### Validação
```csharp
// ? Bom: Validação em múltiplas camadas
if (!entity.IsValid())
    throw new InvalidOperationException("Entity has invalid values.");

// ? Evitar: Validação manual sem reutilização
if (value < 1 || value > 10) { /* ... */ }
```

### Async/Await
```csharp
// ? Bom
public async Task<IEnumerable<T>> GetAllAsync()
{
    return await _context.Set<T>().ToListAsync();
}

// ? Evitar
public IEnumerable<T> GetAll()
{
    return _context.Set<T>().ToList();
}
```

### Tratamento de Exceções
```csharp
// ? Bom: Exceções específicas
if (entity == null)
    throw new KeyNotFoundException("Entity not found.");

// ? Evitar: Exceções genéricas
throw new Exception("Error");
```

### DTOs
```csharp
// ? Bom: DTO específico para cada operação
public class CreateEntityDto { /* ... */ }
public class UpdateEntityDto { /* ... */ }

// ? Evitar: Usar entidade diretamente
public Task CreateAsync(Entity entity) { /* ... */ }
```

---

## Testes

### Exemplo de Teste Unitário

Arquivo: `Tests/Services/ExerciseHistoryServiceTests.cs`

```csharp
using Xunit;
using Moq;
using HabitFlow.Application.Services;
using HabitFlow.Infrastructure.Repositories;
using HabitFlow.Domain.Entities;

namespace HabitFlow.Tests.Services;

public class ExerciseHistoryServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsDtoWithId()
    {
        // Arrange
        var mockRepository = new Mock<IExerciseHistoryRepository>();
        var service = new ExerciseHistoryService(mockRepository.Object);
        var dto = new CreateExerciseHistoryDto 
        { 
            UserId = 1,
            ExerciseType = "Running",
            DurationMinutes = 30,
            Intensity = 7
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<ExerciseHistory>()), Times.Once);
        mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var mockRepository = new Mock<IExerciseHistoryRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((ExerciseHistory)null!);
        var service = new ExerciseHistoryService(mockRepository.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}
```

---

## Troubleshooting

### Erro: "The name or namespace ... does not exist"
**Causa:** Falta `using` ou referência
**Solução:** Adicione `using` namespace no topo do arquivo

### Erro: "DbContext options have not been configured"
**Causa:** Serviço não foi registrado em `Program.cs`
**Solução:** Adicione `builder.Services.AddScoped<IService, Service>();`

### Erro: "Entity Framework Core version not matching"
**Causa:** Versão diferente em diferentes pacotes
**Solução:** 
```bash
dotnet package update --dry-run
dotnet list package --outdated
```

---

## Recursos Úteis

- [Microsoft Docs - Razor Pages](https://learn.microsoft.com/aspnet/core/razor-pages)
- [Microsoft Docs - EF Core](https://learn.microsoft.com/ef/core/)
- [Microsoft Docs - Dependency Injection](https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection)
- [ASP.NET Core Best Practices](https://learn.microsoft.com/aspnet/core/fundamentals/best-practices)

---

**Última Atualização:** 2025-11-20
