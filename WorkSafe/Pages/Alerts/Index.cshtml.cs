using HabitFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HabitFlow.Pages.Alerts;

public class IndexModel : PageModel
{
    private readonly IWellnessAlertService _alertService;

    [BindProperty(SupportsGet = true)]
    public int UserId { get; set; } = 1;

    public IEnumerable<HabitFlow.Application.DTOs.WellnessAlertDto> Alerts { get; set; } = new List<HabitFlow.Application.DTOs.WellnessAlertDto>();

    public IndexModel(IWellnessAlertService alertService)
    {
        _alertService = alertService;
    }

    public async Task OnGetAsync()
    {
        Alerts = await _alertService.GetAlertsByUserIdAsync(UserId);
    }
}
