using Application.Dtos.Request;
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
        return Ok();
    }

    [HttpPost("unblock-user")]
    public async Task<IActionResult> UnblockUser([FromQuery] string userId)
    {
        await _adminService.UnblockUserAsync(userId);
        return Ok();
    }

    [HttpPost("create-operator")]
    public async Task<IActionResult> CreateOperator([FromForm] OperatorDto newOperator)
    {
        try
        {

            var createdOperator = await _adminService.CreateOperatorAsync(newOperator, newOperator.Password);
            return Ok(new { Message = "Оператор успешно создан", OperatorId = createdOperator.Id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Внутренняя ошибка сервера" });
        }
    }
}

