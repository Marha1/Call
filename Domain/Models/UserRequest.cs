using System.Net.Mail;
using Domain.Primitives;

namespace Domain.Models;

public class UserRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Topic { get; set; } // Тема запроса
    public string Description { get; set; } // Описание запроса
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public RequestStatus Status { get; set; } = RequestStatus.New;

    // Связь с пользователем
    public string UserId { get; set; }    
    public User User { get; set; }

    // Связь с оператором
    public string? OperatorId { get; set; }
    public Operator? Operator { get; set; }

    // Вложения
    public ICollection<AttachmentUser> Attachments { get; set; } = new List<AttachmentUser>();
}