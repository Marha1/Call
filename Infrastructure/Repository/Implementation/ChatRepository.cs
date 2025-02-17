using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;
/// <summary>
/// Реализация работы репозитория чата
/// </summary>
public class ChatRepository : BaseRepository<ChatMessage>,IChatRepository
{
    public ChatRepository(ApplicationContext context) : base(context)
    {
    }

    /// <summary>
    /// Получение сообщений по id тикета
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ChatMessage>> GetMessagesByRequestIdAsync(Guid requestId)
    {
        return await _context.ChatMessages.
            Where(x => x.TicketId == requestId).
            OrderBy(m=>m.CreatedAt).ToListAsync();
    }
}