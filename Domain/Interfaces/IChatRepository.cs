using Domain.Models;

namespace Domain.Interfaces;

public interface IChatRepository : IRepository<ChatMessage>
{
    /// <summary>
    ///     Получение сообщений по тикет Id
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    Task<IEnumerable<ChatMessage>> GetMessagesByRequestIdAsync(Guid requestId);
}