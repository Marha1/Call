namespace Domain.Models;

public sealed record RegisterDto(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string Surname,
    string? LastName);