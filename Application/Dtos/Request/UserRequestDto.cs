namespace Application.Dtos.Request;

public class UserRequestDto
{
    public Guid Id { get; set; }
    public string Topic { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public string UserId { get; set; }
    public string OperatorId { get; set; }
    public ICollection<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
}