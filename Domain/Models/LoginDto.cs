namespace Domain.Models;

public sealed record LoginDto(
    string Login,
    string Password);