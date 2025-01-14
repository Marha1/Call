using Domain.Primitives;

namespace Domain.Models;

public class Operator : AppUser
{
    public Department AssignedDepartment { get; set; } // Отдел оператора

    // Связь с запросами, которые обслуживает оператор
    public ICollection<UserRequest> AssignedRequests { get; set; } = new List<UserRequest>();
}