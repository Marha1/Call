namespace Domain.Interfaces;

public interface IEmailSender
{
    Task SendEmailConfirmedCode(string email, string confirmCode);
    Task SendResetPasswordEmailAsync(string email, string resetToken);
}