using Domain.Primitives;

namespace Application.Dtos.Request;

public class UserRequestCreateDto
{
    public Department Topic { get; set; }
    public string Description { get; set; }
}