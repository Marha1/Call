using Domain.Models;

namespace Domain.Interfaces;

public interface IAccountService
{
    Task<string> RegisterUserAsync(RegisterDto dto);
    Task<UserInfo> LoginAsync(LoginDto dto);
    Task SendResetPasswordEmailAsync(string email);
    Task EmailConfirmed(string email, string confirmedCode);
    Task SendEmailConfirmedCode(string email);
    Task ResetPasswordAsync(string email, string resetToken, string newPassword);
}