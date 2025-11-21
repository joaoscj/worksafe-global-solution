using HabitFlow.Application.DTOs;
using HabitFlow.Domain.Entities;
using HabitFlow.Infrastructure.Repositories;

namespace HabitFlow.Application.Services;

public interface IWellnessAlertService
{
    Task<WellnessAlertDto?> GetAlertByIdAsync(int id);
    Task<IEnumerable<WellnessAlertDto>> GetAlertsByUserIdAsync(int userId);
    Task<IEnumerable<WellnessAlertDto>> GetUnresolvedAlertsByUserIdAsync(int userId);
    Task CreateAlertFromHealthCheckAsync(HealthCheck healthCheck);
    Task MarkAlertAsReadAsync(int alertId);
    Task ResolveAlertAsync(int alertId);
    Task DeleteAlertAsync(int id);
}

public class WellnessAlertService : IWellnessAlertService
{
    private readonly IWellnessAlertRepository _repository;

    public WellnessAlertService(IWellnessAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<WellnessAlertDto?> GetAlertByIdAsync(int id)
    {
        var alert = await _repository.GetByIdAsync(id);
        return alert != null ? MapToDto(alert) : null;
    }

    public async Task<IEnumerable<WellnessAlertDto>> GetAlertsByUserIdAsync(int userId)
    {
        var alerts = await _repository.GetByUserIdAsync(userId);
        return alerts.Select(MapToDto);
    }

    public async Task<IEnumerable<WellnessAlertDto>> GetUnresolvedAlertsByUserIdAsync(int userId)
    {
        var alerts = await _repository.GetUnresolvedByUserIdAsync(userId);
        return alerts.Select(MapToDto);
    }

    public async Task CreateAlertFromHealthCheckAsync(HealthCheck healthCheck)
    {
        var alerts = new List<WellnessAlert>();

        if (healthCheck.StressLevel >= 8)
        {
            alerts.Add(new WellnessAlert
            {
                UserId = healthCheck.UserId,
                Type = AlertType.HighStress,
                Severity = healthCheck.StressLevel >= 9 ? AlertSeverity.Critical : AlertSeverity.High,
                Message = $"Seu nível de estresse está elevado ({healthCheck.StressLevel}/10). Considere técnicas de relaxamento.",
                Recommendation = "Tente meditação, respiração profunda, ou atividades relaxantes. Converse com um profissional se necessário."
            });
        }

        if (healthCheck.SleepQuality <= 3)
        {
            alerts.Add(new WellnessAlert
            {
                UserId = healthCheck.UserId,
                Type = AlertType.PoorSleep,
                Severity = AlertSeverity.High,
                Message = $"Sua qualidade de sono está comprometida ({healthCheck.SleepQuality}/10).",
                Recommendation = "Estabeleça uma rotina de sono consistente, evite telas antes de dormir, e mantenha um ambiente escuro e silencioso."
            });
        }

        if (healthCheck.JobSatisfaction <= 3)
        {
            alerts.Add(new WellnessAlert
            {
                UserId = healthCheck.UserId,
                Type = AlertType.LowJobSatisfaction,
                Severity = AlertSeverity.Medium,
                Message = $"Sua satisfação profissional está baixa ({healthCheck.JobSatisfaction}/10).",
                Recommendation = "Considere conversar com seu gerente sobre seus desafios ou buscar suporte em desenvolvimento profissional."
            });
        }

        if (healthCheck.MentalHealth <= 3)
        {
            alerts.Add(new WellnessAlert
            {
                UserId = healthCheck.UserId,
                Type = AlertType.MentalHealthConcern,
                Severity = AlertSeverity.Critical,
                Message = $"Sua saúde mental está em nível de preocupação ({healthCheck.MentalHealth}/10).",
                Recommendation = "É importante buscar suporte profissional. Entre em contato com um psicólogo ou psiquiatra para avaliação e apoio."
            });
        }

        if (healthCheck.PhysicalHealth <= 3)
        {
            alerts.Add(new WellnessAlert
            {
                UserId = healthCheck.UserId,
                Type = AlertType.PhysicalHealthConcern,
                Severity = AlertSeverity.High,
                Message = $"Sua saúde física está comprometida ({healthCheck.PhysicalHealth}/10).",
                Recommendation = "Considere consultar um médico, aumentar a atividade física regular, e melhorar sua nutrição."
            });
        }

        foreach (var alert in alerts)
        {
            await _repository.AddAsync(alert);
        }

        await _repository.SaveChangesAsync();
    }

    public async Task MarkAlertAsReadAsync(int alertId)
    {
        var alert = await _repository.GetByIdAsync(alertId);
        if (alert == null)
            throw new InvalidOperationException("Alert not found.");

        alert.MarkAsRead();
        await _repository.UpdateAsync(alert);
        await _repository.SaveChangesAsync();
    }

    public async Task ResolveAlertAsync(int alertId)
    {
        var alert = await _repository.GetByIdAsync(alertId);
        if (alert == null)
            throw new InvalidOperationException("Alert not found.");

        alert.Resolve();
        await _repository.UpdateAsync(alert);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAlertAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
    }

    private WellnessAlertDto MapToDto(WellnessAlert alert)
    {
        return new WellnessAlertDto
        {
            Id = alert.Id,
            UserId = alert.UserId,
            Type = alert.Type.ToString(),
            Severity = alert.Severity.ToString(),
            Message = alert.Message,
            Recommendation = alert.Recommendation,
            IsRead = alert.IsRead,
            CreatedAt = alert.CreatedAt,
            ResolvedAt = alert.ResolvedAt
        };
    }
}
