using HabitFlow.Application.DTOs;
using HabitFlow.Domain.Entities;
using HabitFlow.Infrastructure.Repositories;

namespace HabitFlow.Application.Services;

public interface IHealthCheckService
{
    Task<HealthCheckDto?> GetHealthCheckByIdAsync(int id);
    Task<IEnumerable<HealthCheckDto>> GetHealthChecksByUserIdAsync(int userId);
    Task<HealthCheckDto?> GetLatestHealthCheckAsync(int userId);
    Task<IEnumerable<HealthCheckDto>> GetHealthChecksByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    Task<HealthCheckDto> CreateHealthCheckAsync(CreateHealthCheckDto dto);
    Task<HealthCheckDto> UpdateHealthCheckAsync(int id, UpdateHealthCheckDto dto);
    Task DeleteHealthCheckAsync(int id);
}

public class HealthCheckService : IHealthCheckService
{
    private readonly IHealthCheckRepository _repository;
    private readonly IWellnessAlertService _alertService;
    private readonly IUserWellnessRepository _userWellnessRepository;

    public HealthCheckService(
        IHealthCheckRepository repository,
        IWellnessAlertService alertService,
        IUserWellnessRepository userWellnessRepository)
    {
        _repository = repository;
        _alertService = alertService;
        _userWellnessRepository = userWellnessRepository;
    }

    public async Task<HealthCheckDto?> GetHealthCheckByIdAsync(int id)
    {
        var healthCheck = await _repository.GetByIdAsync(id);
        return healthCheck != null ? MapToDto(healthCheck) : null;
    }

    public async Task<IEnumerable<HealthCheckDto>> GetHealthChecksByUserIdAsync(int userId)
    {
        var healthChecks = await _repository.GetByUserIdAsync(userId);
        return healthChecks.Select(MapToDto);
    }

    public async Task<HealthCheckDto?> GetLatestHealthCheckAsync(int userId)
    {
        var healthCheck = await _repository.GetLatestByUserIdAsync(userId);
        return healthCheck != null ? MapToDto(healthCheck) : null;
    }

    public async Task<IEnumerable<HealthCheckDto>> GetHealthChecksByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var healthChecks = await _repository.GetByDateRangeAsync(userId, startDate, endDate);
        return healthChecks.Select(MapToDto);
    }

    public async Task<HealthCheckDto> CreateHealthCheckAsync(CreateHealthCheckDto dto)
    {
        var healthCheck = new HealthCheck
        {
            UserId = dto.UserId,
            StressLevel = dto.StressLevel,
            SleepQuality = dto.SleepQuality,
            JobSatisfaction = dto.JobSatisfaction,
            MentalHealth = dto.MentalHealth,
            PhysicalHealth = dto.PhysicalHealth,
            Notes = dto.Notes
        };

        await _repository.AddAsync(healthCheck);
        await _repository.SaveChangesAsync();

        if (healthCheck.RequiresAlert())
        {
            await _alertService.CreateAlertFromHealthCheckAsync(healthCheck);
        }

        await UpdateUserWellnessAsync(dto.UserId);

        return MapToDto(healthCheck);
    }

    public async Task<HealthCheckDto> UpdateHealthCheckAsync(int id, UpdateHealthCheckDto dto)
    {
        var healthCheck = await _repository.GetByIdAsync(id);
        if (healthCheck == null)
            throw new InvalidOperationException("Health check not found.");

        healthCheck.StressLevel = dto.StressLevel;
        healthCheck.SleepQuality = dto.SleepQuality;
        healthCheck.JobSatisfaction = dto.JobSatisfaction;
        healthCheck.MentalHealth = dto.MentalHealth;
        healthCheck.PhysicalHealth = dto.PhysicalHealth;
        healthCheck.Notes = dto.Notes;

        await _repository.UpdateAsync(healthCheck);
        await _repository.SaveChangesAsync();

        if (healthCheck.RequiresAlert())
        {
            await _alertService.CreateAlertFromHealthCheckAsync(healthCheck);
        }

        return MapToDto(healthCheck);
    }

    public async Task DeleteHealthCheckAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
    }

    private async Task UpdateUserWellnessAsync(int userId)
    {
        var healthChecks = await _repository.GetByUserIdAsync(userId);
        var userWellness = await _userWellnessRepository.GetByUserIdAsync(userId);

        if (userWellness == null)
        {
            userWellness = new UserWellness
            {
                UserId = userId,
                TotalHealthChecks = healthChecks.Count(),
                AverageWellnessScore = healthChecks.Any() ? healthChecks.Select(h => h.CalculateWellnessScore()).Sum() / healthChecks.Count() : 0,
                LastCheckAt = healthChecks.FirstOrDefault()?.CheckedAt
            };
            await _userWellnessRepository.AddAsync(userWellness);
        }
        else
        {
            userWellness.TotalHealthChecks = healthChecks.Count();
            userWellness.AverageWellnessScore = healthChecks.Any() ? healthChecks.Select(h => h.CalculateWellnessScore()).Sum() / healthChecks.Count() : 0;
            userWellness.LastCheckAt = healthChecks.FirstOrDefault()?.CheckedAt;
            userWellness.UpdatedAt = DateTime.UtcNow;
            await _userWellnessRepository.UpdateAsync(userWellness);
        }

        await _userWellnessRepository.SaveChangesAsync();
    }

    private HealthCheckDto MapToDto(HealthCheck healthCheck)
    {
        return new HealthCheckDto
        {
            Id = healthCheck.Id,
            UserId = healthCheck.UserId,
            StressLevel = healthCheck.StressLevel,
            SleepQuality = healthCheck.SleepQuality,
            JobSatisfaction = healthCheck.JobSatisfaction,
            MentalHealth = healthCheck.MentalHealth,
            PhysicalHealth = healthCheck.PhysicalHealth,
            Notes = healthCheck.Notes,
            CheckedAt = healthCheck.CheckedAt,
            WellnessScore = healthCheck.CalculateWellnessScore()
        };
    }
}
