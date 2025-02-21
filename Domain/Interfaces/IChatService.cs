using Domain.Models;

namespace Domain.Interfaces;

public interface IChatService
{
    /// <summary>
    ///     Сохраняет сообщение в чате.
    /// </summary>
    Task SaveMessageAsync(Guid requestId, string userId, string message, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Закрывает запрос (тикет).
    /// </summary>
    Task CloseRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Сохраняет рейтинг для чата.
    /// </summary>
    Task SaveRatingAsync(Guid requestId, int rating, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Получает список сообщений для чата.
    /// </summary>
    Task<IEnumerable<ChatMessage>> GetMessagesAsync(Guid requestId, CancellationToken cancellationToken = default);
}