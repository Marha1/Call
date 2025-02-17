namespace Application.Dtos.AuthDtos;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string ResetToken { get; set; }
    public string NewPassword { get; set; }
}
