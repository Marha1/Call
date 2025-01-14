namespace Application.Dtos.AuthDtos;

public sealed record RegisterDto(
    string Email,
    string Password,
    string FirstName,
    string Surname,
    string? LastName);