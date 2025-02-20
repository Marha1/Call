using Domain.Primitives;

namespace Application.Dtos.Request;

public abstract class GetUserRequestDto
{
    public Guid Id { get; set; }
    public Department Topic { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; } 
    public RequestStatus Status { get; set; }
    public string UserId { get; set; }
}