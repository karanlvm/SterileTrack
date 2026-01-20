using Microsoft.AspNetCore.Mvc;
using SterileTrack.Application.DTOs;
using SterileTrack.Application.Services;
using SterileTrack.Domain.Entities;

namespace SterileTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(IDeviceService deviceService, ILogger<DevicesController> logger)
    {
        _deviceService = deviceService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAllDevices()
    {
        try
        {
            var devices = await _deviceService.GetAllDevicesAsync();
            return Ok(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching devices");
            return StatusCode(500, new { message = "An error occurred while fetching devices" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceDto>> GetDeviceById(Guid id)
    {
        try
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return Ok(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching device {DeviceId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the device" });
        }
    }

    [HttpGet("identifier/{deviceIdentifier}")]
    public async Task<ActionResult<DeviceDto>> GetDeviceByIdentifier(string deviceIdentifier)
    {
        try
        {
            var device = await _deviceService.GetDeviceByIdentifierAsync(deviceIdentifier);
            if (device == null)
            {
                return NotFound();
            }
            return Ok(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching device {DeviceIdentifier}", deviceIdentifier);
            return StatusCode(500, new { message = "An error occurred while fetching the device" });
        }
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDevicesByStatus(DeviceStatus status)
    {
        try
        {
            var devices = await _deviceService.GetDevicesByStatusAsync(status);
            return Ok(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching devices by status {Status}", status);
            return StatusCode(500, new { message = "An error occurred while fetching devices" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<DeviceDto>> CreateDevice([FromBody] CreateDeviceDto dto)
    {
        try
        {
            var device = await _deviceService.CreateDeviceAsync(dto);
            return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");
            return StatusCode(500, new { message = "An error occurred while creating the device" });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<DeviceDto>> UpdateDeviceStatus(Guid id, [FromBody] UpdateDeviceStatusDto dto)
    {
        try
        {
            var changedBy = Request.Headers.ContainsKey("X-User-Name") 
                ? Request.Headers["X-User-Name"].ToString() 
                : "System";
            
            var device = await _deviceService.UpdateDeviceStatusAsync(id, dto, changedBy);
            return Ok(device);
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
            _logger.LogError(ex, "Error updating device status");
            return StatusCode(500, new { message = "An error occurred while updating device status" });
        }
    }

    [HttpGet("{id}/history")]
    public async Task<ActionResult<IEnumerable<StatusHistoryDto>>> GetDeviceStatusHistory(Guid id)
    {
        try
        {
            var history = await _deviceService.GetDeviceStatusHistoryAsync(id);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching device status history");
            return StatusCode(500, new { message = "An error occurred while fetching status history" });
        }
    }
}
