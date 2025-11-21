using HabitFlow.Application.DTOs;
using HabitFlow.Application.Services;
using HabitFlow.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HabitFlow.Pages;

public class IndexModel : PageModel
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly IWellnessAlertService _alertService;
    private readonly IUserWellnessRepository _userWellnessRepository;

    [BindProperty(SupportsGet = true)]
    public int UserId { get; set; } = 1;

    public UserWellnessDto? UserWellness { get; set; }
    public HealthCheckDto? LatestHealthCheck { get; set; }
    public IEnumerable<WellnessAlertDto> UnresolvedAlerts { get; set; } = new List<WellnessAlertDto>();
    public IEnumerable<WellnessTipDto> RecommendedTips { get; set; } = new List<WellnessTipDto>();

    public IndexModel(
        IHealthCheckService healthCheckService,
        IWellnessAlertService alertService,
        IUserWellnessRepository userWellnessRepository)
    {
        _healthCheckService = healthCheckService;
        _alertService = alertService;
        _userWellnessRepository = userWellnessRepository;
    }

    public async Task OnGetAsync()
    {
        LatestHealthCheck = await _healthCheckService.GetLatestHealthCheckAsync(UserId);
        UnresolvedAlerts = await _alertService.GetUnresolvedAlertsByUserIdAsync(UserId);
        
        var userWellnessData = await _userWellnessRepository.GetByUserIdAsync(UserId);
        if (userWellnessData != null)
        {
            UserWellness = new UserWellnessDto
            {
                Id = userWellnessData.Id,
                UserId = userWellnessData.UserId,
                AverageWellnessScore = userWellnessData.AverageWellnessScore,
                TotalHealthChecks = userWellnessData.TotalHealthChecks,
                UnresolvedAlerts = userWellnessData.UnresolvedAlerts,
                LastCheckAt = userWellnessData.LastCheckAt,
                Trend = userWellnessData.Trend
            };
        }
    }
}
