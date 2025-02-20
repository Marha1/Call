namespace Application.Dtos.AuthDtos;

public class EmailConfirmationDto
{
    public string Email { get; set; }
    public string ConfirmedCode { get; set; }
}