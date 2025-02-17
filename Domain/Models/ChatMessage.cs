namespace Domain.Models;

public class ChatMessage
{
    public Guid id { get; set; }
    public Guid TicketId { get; set; }
    public string Message { get; set; }
    public string SenderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToUniversalTime();
    
}