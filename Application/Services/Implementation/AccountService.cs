using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

namespace Application.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public AccountService(UserManager<AppUser> userManager, 
        IConfiguration configuration, IMapper mapper)
    {
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
    }

    /// <summary>
    ///     Генерация,сохранение в бд,отправка токена на почту для сброса
    /// </summary>
    /// <param name="email"></param>
    /// <exception cref="Exception"></exception>
    public async Task SendResetPasswordEmailAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            var resetToken = new Random().Next(100000, 999999).ToString("D6"); // 6-значный токен
            // Сохранение токена в таблицу AspNetUserTokens
            var result =
                await _userManager.SetAuthenticationTokenAsync(user, "PasswordReset", "ResetToken", resetToken);
            if (!result.Succeeded)
                throw new Exception(
                    $"Failed to store password reset token: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            // Формирование сообщения
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Call",
                "timyewlasow@gmail.com")); // Убедитесь, что email корректный
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Password Reset Request";
            message.Body = new TextPart("plain")
            {
                Text = $"Your password reset code is: {resetToken}"
            };

            using var client = new SmtpClient();

            try
            {
                // Установка соединения с SMTP-сервером
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Аутентификация на SMTP-сервере
                await client.AuthenticateAsync("timyewlasow@gmail.com", "ohpqbczbijhibsqa");

                // Отправка сообщения
                await client.SendAsync(message);

                // Отключение клиента
                await client.DisconnectAsync(true);
            }
            catch (AuthenticationException authEx)
            {
                // Проблема с аутентификацией на SMTP-сервере
                throw new Exception("Authentication failed. Please check your email and password credentials.", authEx);
            }
            catch (SmtpCommandException smtpEx)
            {
                // Проблема с SMTP-командами
                throw new Exception($"SMTP Command Error: {smtpEx.Message}. StatusCode: {smtpEx.StatusCode}", smtpEx);
            }
            catch (Exception ex)
            {
                // Логируем любые другие ошибки при отправке email
                throw new Exception($"An error occurred while sending the email: {ex.Message}", ex);
            }
        }
        catch (Exception ex)
        {
            // Логируем все ошибки
            Console.WriteLine($"Error in SendResetPasswordEmailAsync: {ex.Message}");
            throw; // Можно повторно выбросить исключение, чтобы обработать его выше
        }
    }


    /// <summary>
    ///     Сброс пароля
    /// </summary>
    /// <param name="email"></param>
    /// <param name="resetToken"></param>
    /// <param name="newPassword"></param>
    /// <exception cref="Exception"></exception>
    public async Task ResetPasswordAsync(string email, string resetToken, string newPassword)
    {
        // Найти пользователя
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new Exception("User not found");

        // Получить токен из AspNetUserTokens
        var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "PasswordReset", "ResetToken");
        if (storedToken != resetToken) throw new Exception("Invalid or expired password reset token");

        // Изменить пароль вручную
        var resetResult = await _userManager.RemovePasswordAsync(user);
        if (!resetResult.Succeeded)
            throw new Exception(
                $"Failed to remove the old password: {string.Join(", ", resetResult.Errors.Select(e => e.Description))}");

        var setPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
        if (!setPasswordResult.Succeeded)
            throw new Exception(
                $"Failed to set the new password: {string.Join(", ", setPasswordResult.Errors.Select(e => e.Description))}");

        // Удалить токен после успешного сброса
        await _userManager.RemoveAuthenticationTokenAsync(user, "PasswordReset", "ResetToken");
    }


    /// <summary>
    ///     Регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Сообщение</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> RegisterUserAsync(RegisterDto dto)
    {
        var user = _mapper.Map<User>(dto);

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(
                $"User registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        var roleResult = await _userManager.AddToRoleAsync(user, "User");
        if (!roleResult.Succeeded)
            throw new Exception(
                $"Assigning role failed: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

        return "User registered successfully";
    }

    /// <summary>
    ///     Авторизация
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Токен авторизации</returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Login) ??
                   await _userManager.FindByNameAsync(dto.Login);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedAccessException("Invalid login or password");

        if (!user.EmailConfirmed) throw new UnauthorizedAccessException("Email is not confirmed");

        if (await _userManager.IsLockedOutAsync(user)) throw new UnauthorizedAccessException("User is locked out.");

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "Unknown"),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ??
                                         throw new InvalidOperationException(
                                             "JWT Key is not configured or is invalid"));
        if (key.Length < 32)
            throw new InvalidOperationException("JWT Key length must be at least 256 bits (32 bytes).");

        var issuer = _configuration["Jwt:Issuer"] ??
                     throw new InvalidOperationException("JWT Issuer is not configured.");
        var audience = _configuration["Jwt:Audience"] ??
                       throw new InvalidOperationException("JWT Audience is not configured.");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}