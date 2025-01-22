using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[Authorize(Roles = "User")]
[ApiController]
[Route("api/[controller]")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;

    public AttachmentController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    /// <summary>
    ///     Удаление вложения
    /// </summary>
    /// <param name="attachmentId">Идентификатор вложения</param>
    /// <returns></returns>
    [HttpDelete("{attachmentId:guid}")]
    public async Task<IActionResult> DeleteAttachment(Guid attachmentId)
    {
        try
        {
            var success = await _attachmentService.DeleteAttachmentAsync(attachmentId, CancellationToken.None);
            if (!success)
                return NotFound(new { Message = "Attachment not found." });

            return Ok(new { Message = "Attachment deleted successfully." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting attachment: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}