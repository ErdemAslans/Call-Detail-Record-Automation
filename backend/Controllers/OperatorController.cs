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
            var (success, breakInfo, message) = await _operatorService.StartBreakAsync(username, request?.Reason);

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

    [HttpGet("user-break-times")]
    [Authorize(Roles = "Central")]
    // TODO: Range keyword u alacak şekilde güncelle. Veri aktarımı yaptıktan sonra
    public async Task<IActionResult> GetUserBreakTimes([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var breakTimes = await _operatorService.GetUserBreakTimesAsync(username, startDate, endDate);
        return Ok(breakTimes);
    }
}