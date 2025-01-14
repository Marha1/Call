using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;

public class AttachmentRepository : BaseRepository<AttachmentUser>
{
    public AttachmentRepository(ApplicationContext context) : base(context) { }

    public async Task<ICollection<AttachmentUser>> GetAttachmentsByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.Attachments
            .Where(a => a.RequestId == requestId)
            .ToListAsync(cancellationToken);
    }
}
