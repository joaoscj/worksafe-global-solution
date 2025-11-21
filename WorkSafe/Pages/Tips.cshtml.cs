using HabitFlow.Application.Services;
using HabitFlow.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HabitFlow.Pages;

public class TipsModel : PageModel
{
    private readonly ITipsService _tipsService;
    private readonly IHealthCheckService _healthCheckService;

    [BindProperty(SupportsGet = true)]
    public int UserId { get; set; } = 1;

    public IEnumerable<HabitFlow.Application.DTOs.WellnessTipDto> Tips { get; set; } = new List<HabitFlow.Application.DTOs.WellnessTipDto>();

    public TipsModel(ITipsService tipsService, IHealthCheckService healthCheckService)
    {
        _tipsService = tipsService;
        _healthCheckService = healthCheckService;
    }

    public async Task OnGetAsync()
    {
        var latestCheck = await _healthCheckService.GetLatestHealthCheckAsync(UserId);
        if (latestCheck != null)
        {
            Tips = await _tipsService.GetTipsForWellnessScoreAsync(latestCheck.WellnessScore);
        }
    }
}
