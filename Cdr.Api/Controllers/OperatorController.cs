using Cdr.Api.Models.Request;
using Cdr.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cdr.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OperatorController : ControllerBase
{
    private readonly IOperatorService _operatorService;
    private readonly ILogger<OperatorController> _logger;

    public OperatorController(IOperatorService operatorService, ILogger<OperatorController> logger)
    {
        _operatorService = operatorService;
        _logger = logger;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var response = await _operatorService.GetAllUsersInfoAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching user report");
            return StatusCode(500);
        }
    }

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments()
    {
        try
        {
            var response = await _operatorService.GetAllDepartmentsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching departments");
            return StatusCode(500);
        }
    }

    [HttpPost("start-break")]
    [Authorize(Roles = "Central")]
    public async Task<IActionResult> StartBreak([FromBody] BreakRequest request)
    {
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var (success, breakInfo, message) = await _operatorService.StartBreakAsync(username, request?.Reason, request!.PlannedEndTime);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { breakInfo, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while starting break");
            return StatusCode(500);
        }
    }

    [HttpPost("end-break/{breakId}")]
    [Authorize(Roles = "Central")]
    public async Task<IActionResult> EndBreak(string breakId)
    {
        try
        {
            var (success, breakInfo, message) = await _operatorService.EndBreakAsync(new MongoDB.Bson.ObjectId(breakId));
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { breakInfo, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while ending break");
            return StatusCode(500);
        }
    }

    [HttpPost("end-shift")]
    [Authorize(Roles = "Central")]
    public async Task<IActionResult> EndShift([FromBody] BreakRequest? request)
    {
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var (success, breakInfo, message) = await _operatorService.EndShiftAsync(username, request?.Reason);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { breakInfo, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while ending shift");
            return StatusCode(500);
        }
    }

    [HttpGet("user-break-times")]
    [Authorize(Roles = "Central")]
    public async Task<IActionResult> GetUserBreakTimes([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("GetUserBreakTimes called - username: {Username}, startDate: {StartDate}, endDate: {EndDate}", username, startDate, endDate);
            var breakTimes = await _operatorService.GetUserBreakTimesAsync(username, startDate, endDate);
            _logger.LogInformation("GetUserBreakTimes result - username: {Username}, count: {Count}", username, breakTimes?.Count ?? 0);
            return Ok(breakTimes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserBreakTimes");
            return Ok(new List<object>());
        }
    }

    [HttpGet("ongoing-break")]
    [Authorize(Roles = "Central")]
    public async Task<IActionResult> GetOngoingBreak()
    {
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var ongoingBreak = await _operatorService.GetOngoingBreakAsync(username);
            return Ok(ongoingBreak);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching ongoing break");
            return StatusCode(500);
        }
    }

    [HttpGet("admin-ongoing-break/{number}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminGetOngoingBreak(string number)
    {
        try
        {
            var ongoingBreak = await _operatorService.GetOngoingBreakAsync(number);
            return Ok(ongoingBreak);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching ongoing break for user {Number}", number);
            return StatusCode(500);
        }
    }

    [HttpPost("admin-force-end-break/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminForceEndBreak(string userId)
    {
        try
        {
            var adminUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogWarning("Admin {Admin} force-ending break for user {UserId}", adminUsername, userId);

            var (success, breakInfo, message) = await _operatorService.ForceEndBreakAsync(userId);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { breakInfo, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while force-ending break");
            return StatusCode(500);
        }
    }
}