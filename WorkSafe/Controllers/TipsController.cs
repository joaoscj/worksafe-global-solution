using HabitFlow.Application.DTOs;
using HabitFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WorkSafe.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TipsController : ControllerBase
{
    private readonly ITipsService _tipsService;
    private readonly IHealthCheckService _healthCheckService;
    private readonly ILogger<TipsController> _logger;

    public TipsController(
        ITipsService tipsService,
        IHealthCheckService healthCheckService,
        ILogger<TipsController> logger)
    {
        _tipsService = tipsService;
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém dicas de bem-estar baseadas no score de bem-estar
    /// </summary>
    [HttpGet("wellness-score/{wellnessScore}")]
    [ProducesResponseType(typeof(IEnumerable<WellnessTipDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WellnessTipDto>>> GetByWellnessScore(int wellnessScore)
    {
        if (wellnessScore < 0 || wellnessScore > 100)
        {
            return BadRequest(new { message = "Wellness score deve estar entre 0 e 100." });
        }

        var tips = await _tipsService.GetTipsForWellnessScoreAsync(wellnessScore);
        return Ok(tips);
    }

    /// <summary>
    /// Obtém dicas de bem-estar baseadas no último health check do usuário
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<WellnessTipDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<WellnessTipDto>>> GetByUserId(int userId)
    {
        var latestCheck = await _healthCheckService.GetLatestHealthCheckAsync(userId);
        
        if (latestCheck == null)
        {
            return NotFound(new { message = $"Nenhuma verificação encontrada para o usuário {userId}." });
        }

        var tips = await _tipsService.GetTipsForWellnessScoreAsync(latestCheck.WellnessScore);
        return Ok(tips);
    }

    /// <summary>
    /// Obtém dicas de bem-estar por categoria
    /// </summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<WellnessTipDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WellnessTipDto>>> GetByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return BadRequest(new { message = "Categoria não pode ser vazia." });
        }

        var tips = await _tipsService.GetTipsByCategoryAsync(category);
        return Ok(tips);
    }
}

