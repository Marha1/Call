using Application.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetRequests([FromQuery] string operatorId)
    {
        var requests = await _operatorService.GetRequestsByDepartmentAsync(operatorId);
        return Ok(requests);
    }

    [HttpPost("take-request")]
    public async Task<IActionResult> TakeRequest([FromQuery] string operatorId, [FromQuery] Guid requestId)
    {
        await _operatorService.TakeRequestAsync(operatorId, requestId);
        return NoContent();
    }
}