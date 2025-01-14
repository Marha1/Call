namespace Domain.Models;

public class AttachmentUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string FilePath { get; set; }

    // Связь с запросом
    public Guid RequestId { get; set; }
    public UserRequest Request { get; set; }
}
