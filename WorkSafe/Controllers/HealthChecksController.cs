using HabitFlow.Application.DTOs;
using HabitFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WorkSafe.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthChecksController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly ILogger<HealthChecksController> _logger;

    public HealthChecksController(
        IHealthCheckService healthCheckService,
        ILogger<HealthChecksController> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HealthCheckDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HealthCheckDto>> GetById(int id)
    {
        var healthCheck = await _healthCheckService.GetHealthCheckByIdAsync(id);
        
        if (healthCheck == null)
        {
            return NotFound(new { message = $"Health check com ID {id} não encontrado." });
        }

        return Ok(healthCheck);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<HealthCheckDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HealthCheckDto>>> GetByUserId(
        int userId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        IEnumerable<HealthCheckDto> healthChecks;

        if (startDate.HasValue && endDate.HasValue)
        {
            healthChecks = await _healthCheckService.GetHealthChecksByDateRangeAsync(
                userId, startDate.Value, endDate.Value);
        }
        else
        {
            healthChecks = await _healthCheckService.GetHealthChecksByUserIdAsync(userId);
        }

        return Ok(healthChecks);
    }

    [HttpGet("user/{userId}/latest")]
    [ProducesResponseType(typeof(HealthCheckDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HealthCheckDto>> GetLatest(int userId)
    {
        var healthCheck = await _healthCheckService.GetLatestHealthCheckAsync(userId);
        
        if (healthCheck == null)
        {
            return NotFound(new { message = $"Nenhuma verificação encontrada para o usuário {userId}." });
        }

        return Ok(healthCheck);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HealthCheckDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HealthCheckDto>> Create([FromBody] CreateHealthCheckDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var healthCheck = await _healthCheckService.CreateHealthCheckAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = healthCheck.Id },
                healthCheck);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar health check");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(HealthCheckDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HealthCheckDto>> Update(int id, [FromBody] UpdateHealthCheckDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var healthCheck = await _healthCheckService.UpdateHealthCheckAsync(id, dto);
            return Ok(healthCheck);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar health check {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _healthCheckService.DeleteHealthCheckAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar health check {Id}", id);
            return NotFound(new { message = $"Health check com ID {id} não encontrado." });
        }
    }
}

