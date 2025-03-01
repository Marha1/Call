using System.Security.Claims;
using Application.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers;

[Authorize(Roles = "Operator")]
[ApiController]
[Route("api/operator")]
public class OperatorController : ControllerBase
{
    private readonly OperatorService _operatorService;

    public OperatorController(OperatorService operatorService)
    {
        _operatorService = operatorService;
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests()
    {
        var operatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (operatorId == null) return Unauthorized();

        var requests = await _operatorService.GetRequestsByDepartmentAsync(operatorId);
        return Ok(requests);
    }

    [HttpPost("take-request")]
    public async Task<IActionResult> TakeRequest([FromQuery] Guid requestId)
    {
        try
        {
            var operatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (operatorId == null) return Unauthorized();

            await _operatorService.TakeRequestAsync(operatorId, requestId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("check-current-ticket")]
    public async Task<IActionResult> GetCurrentTicket()
    {
        var operatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (operatorId == null) 
            return Unauthorized();

        var request = await _operatorService.GetCurrentRequestAsync(operatorId);
    
        if (request == null)
            return NotFound("Ты .... а ниче что тот факт что ты не взял тикет?");

        return Ok(request);
    }
}