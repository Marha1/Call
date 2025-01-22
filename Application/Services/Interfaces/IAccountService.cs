using Application.Dtos.AuthDtos;

namespace Application.Services.Interfaces;

public interface IAccountService
{
    Task<string> RegisterUserAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task SendResetPasswordEmailAsync(string email);
    Task ResetPasswordAsync(string email, string resetToken, string newPassword);
}