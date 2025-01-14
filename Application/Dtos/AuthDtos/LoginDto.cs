namespace Application.Dtos.AuthDtos;

public sealed record LoginDto(
    string Email,
    string Password);