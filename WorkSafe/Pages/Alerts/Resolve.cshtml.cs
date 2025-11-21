using HabitFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HabitFlow.Pages.Alerts;

public class ResolveModel : PageModel
{
    private readonly IWellnessAlertService _alertService;

    public ResolveModel(IWellnessAlertService alertService)
    {
        _alertService = alertService;
    }

    public async Task<IActionResult> OnPostAsync(int alertId)
    {
        try
        {
            await _alertService.ResolveAlertAsync(alertId);
            return RedirectToPage("Index");
        }
        catch (Exception)
        {
            return RedirectToPage("Index");
        }
    }
}
