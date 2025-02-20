using Application.Dtos.AuthDtos;
using Application.Services.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _accountService.RegisterUserAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userInfo = await _accountService.LoginAsync(dto);
            return Ok(userInfo); // Отправляем всю инфу о пользователе
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("send-reset-password-email")]
    public async Task<IActionResult> SendResetPasswordEmail([FromBody] SendResetPasswordDto request)
    {
        try
        {
            await _accountService.SendResetPasswordEmailAsync(request.Email);
            return Ok(new { Message = "Password reset email sent successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        try
        {
            await _accountService.ResetPasswordAsync(request.Email, request.ResetToken, request.NewPassword);
            return Ok(new { Message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
    [HttpPost("send-email-confirmation-code")]
    public async Task<IActionResult> SendEmailConfirmationCode([FromBody] EmailConfirmationRequestDto request)
    {
        try
        {
            await _accountService.SendEmailConfirmedCode(request.Email);
            return Ok(new { Message = "Email confirmation code sent successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmationDto request)
    {
        try
        {
            await _accountService.EmailConfirmed(request.Email, request.ConfirmedCode);
            return Ok(new { Message = "Email confirmed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}