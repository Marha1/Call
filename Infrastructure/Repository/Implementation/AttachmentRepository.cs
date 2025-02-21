using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;

/// <summary>
///     Реализация репозитория для вложений
/// </summary>
public class AttachmentRepository : BaseRepository<AttachmentUser>
{
    public AttachmentRepository(ApplicationContext context) : base(context)
    {
    }

    /// <summary>
    ///     Получение вложений по id чата
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ICollection<AttachmentUser>> GetAttachmentsByRequestIdAsync(Guid requestId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Attachments
            .Where(a => a.RequestId == requestId)
            .ToListAsync(cancellationToken);
    }
}