using Domain.Models;

namespace Application.Services.Interfaces;

public interface IAttachmentService
{
    Task<AttachmentUser> UploadAttachmentAsync(IFormFile file, Guid requestId, CancellationToken cancellationToken);
    Task<bool> DeleteAttachmentAsync(Guid attachmentId, CancellationToken cancellationToken);
}