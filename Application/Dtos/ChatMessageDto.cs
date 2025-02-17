namespace Application.Dtos;

public class ChatMessageDto
{
    public string UserId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}