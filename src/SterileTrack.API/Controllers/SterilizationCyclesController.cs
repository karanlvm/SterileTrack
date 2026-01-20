using Microsoft.AspNetCore.Mvc;
using SterileTrack.Application.DTOs;
using SterileTrack.Application.Services;

namespace SterileTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SterilizationCyclesController : ControllerBase
{
    private readonly ISterilizationCycleService _sterilizationCycleService;
    private readonly ILogger<SterilizationCyclesController> _logger;

    public SterilizationCyclesController(
        ISterilizationCycleService sterilizationCycleService,
        ILogger<SterilizationCyclesController> logger)
    {
        _sterilizationCycleService = sterilizationCycleService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SterilizationCycleDto>> GetSterilizationCycleById(Guid id)
    {
        try
        {
            var cycle = await _sterilizationCycleService.GetSterilizationCycleByIdAsync(id);
            if (cycle == null)
            {
                return NotFound();
            }
            return Ok(cycle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sterilization cycle {CycleId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the sterilization cycle" });
        }
    }

    [HttpGet("device/{deviceId}")]
    public async Task<ActionResult<IEnumerable<SterilizationCycleDto>>> GetSterilizationCyclesByDeviceId(Guid deviceId)
    {
        try
        {
            var cycles = await _sterilizationCycleService.GetSterilizationCyclesByDeviceIdAsync(deviceId);
            return Ok(cycles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sterilization cycles for device {DeviceId}", deviceId);
            return StatusCode(500, new { message = "An error occurred while fetching sterilization cycles" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<SterilizationCycleDto>> CreateSterilizationCycle([FromBody] CreateSterilizationCycleDto dto)
    {
        try
        {
            var performedBy = Request.Headers.ContainsKey("X-User-Name") 
                ? Request.Headers["X-User-Name"].ToString() 
                : "System";
            
            var cycle = await _sterilizationCycleService.CreateSterilizationCycleAsync(dto, performedBy);
            return CreatedAtAction(nameof(GetSterilizationCycleById), new { id = cycle.Id }, cycle);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sterilization cycle");
            return StatusCode(500, new { message = "An error occurred while creating the sterilization cycle" });
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<SterilizationCycleDto>> CompleteSterilizationCycle(Guid id, [FromBody] CompleteSterilizationCycleDto dto)
    {
        try
        {
            var cycle = await _sterilizationCycleService.CompleteSterilizationCycleAsync(id, dto);
            return Ok(cycle);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing sterilization cycle");
            return StatusCode(500, new { message = "An error occurred while completing the sterilization cycle" });
        }
    }
}
