using HabitFlow.Application.DTOs;
using HabitFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HabitFlow.Pages.HealthChecks;

public class CreateModel : PageModel
{
    private readonly IHealthCheckService _healthCheckService;

    [BindProperty]
    public CreateHealthCheckDto HealthCheck { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int UserId { get; set; } = 1;

    public CreateModel(IHealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    public void OnGet()
    {
        HealthCheck.UserId = UserId;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _healthCheckService.CreateHealthCheckAsync(HealthCheck);
            return RedirectToPage("/Index", new { userId = HealthCheck.UserId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro ao criar verificação: {ex.Message}");
            return Page();
        }
    }
}
