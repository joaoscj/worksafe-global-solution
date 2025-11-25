using HabitFlow.Application.DTOs;
using HabitFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WorkSafe.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AlertsController : ControllerBase
{
    private readonly IWellnessAlertService _alertService;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(
        IWellnessAlertService alertService,
        ILogger<AlertsController> logger)
    {
        _alertService = alertService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém um alerta por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WellnessAlertDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WellnessAlertDto>> GetById(int id)
    {
        var alert = await _alertService.GetAlertByIdAsync(id);
        
        if (alert == null)
        {
            return NotFound(new { message = $"Alerta com ID {id} não encontrado." });
        }

        return Ok(alert);
    }

    /// <summary>
    /// Lista todos os alertas de um usuário
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<WellnessAlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WellnessAlertDto>>> GetByUserId(int userId)
    {
        var alerts = await _alertService.GetAlertsByUserIdAsync(userId);
        return Ok(alerts);
    }

    /// <summary>
    /// Lista apenas os alertas não resolvidos de um usuário
    /// </summary>
    [HttpGet("user/{userId}/unresolved")]
    [ProducesResponseType(typeof(IEnumerable<WellnessAlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WellnessAlertDto>>> GetUnresolvedByUserId(int userId)
    {
        var alerts = await _alertService.GetUnresolvedAlertsByUserIdAsync(userId);
        return Ok(alerts);
    }

    /// <summary>
    /// Marca um alerta como lido
    /// </summary>
    [HttpPatch("{id}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            await _alertService.MarkAlertAsReadAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar alerta {Id} como lido", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Resolve um alerta
    /// </summary>
    [HttpPatch("{id}/resolve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resolve(int id)
    {
        try
        {
            await _alertService.ResolveAlertAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resolver alerta {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deleta um alerta
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _alertService.DeleteAlertAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar alerta {Id}", id);
            return NotFound(new { message = $"Alerta com ID {id} não encontrado." });
        }
    }
}

