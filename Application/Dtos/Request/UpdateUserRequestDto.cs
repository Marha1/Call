using Domain.Primitives;

namespace Application.Dtos.Request;

public class UpdateUserRequestDto
{
    public Department Topic { get; set; }
    public string Description { get; set; }
}
