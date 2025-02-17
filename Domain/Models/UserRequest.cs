using Domain.Primitives;

namespace Domain.Models;

/// <summary>
///     Тикеты пользователя
/// </summary>
public class UserRequest
{
    /// <summary>
    ///     Id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    ///     Тема тикета
    /// </summary>
    public Department Topic { get; set; }

    /// <summary>
    ///     Описание тикета
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Статус
    /// </summary>
    public RequestStatus Status { get; set; } = RequestStatus.Open;

    /// <summary>
    ///     Связь с пользователем
    /// </summary>
    public string UserId { get; set; }

    public User User { get; set; }

    /// <summary>
    ///     Связь с оператором
    /// </summary>
    public string? OperatorId { get; set; }

    public Operator? Operator { get; set; }
    /// <summary>
    /// Рейтинг(оценка,хз)
    /// </summary>
    public int Raiting { get; set; }

    /// <summary>
    ///     Вложение
    /// </summary>
    public ICollection<AttachmentUser> Attachments { get; set; } = new List<AttachmentUser>();
}