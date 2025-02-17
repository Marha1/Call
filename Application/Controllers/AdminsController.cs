using Application.Services.Implementation;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("block-user")]
    public async Task<IActionResult> BlockUser([FromQuery] string userId)
    {
        await _adminService.BlockUserAsync(userId);
        return NoContent();
    }

    [HttpPost("unblock-user")]
    public async Task<IActionResult> UnblockUser([FromQuery] string userId)
    {
        await _adminService.UnblockUserAsync(userId);
        return NoContent();
    }

    [HttpPost("create-operator")]
    public async Task<IActionResult> CreateOperator([FromBody] Operator newOperator, [FromQuery] string password)
    {
        var createdOperator = await _adminService.CreateOperatorAsync(newOperator, password);
        return CreatedAtAction(nameof(BlockUser), new { id = createdOperator.Id }, createdOperator);
    }
}