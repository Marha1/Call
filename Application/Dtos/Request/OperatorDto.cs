using Domain.Primitives;
using Domain.ValueObject;

namespace Application.Dtos.Request;

public class OperatorDto
{
 

    public FullName FullName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public Department DepartmentOperator { get; set; }
    public string Password { get; set; }
}