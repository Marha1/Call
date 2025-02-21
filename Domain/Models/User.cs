namespace Domain.Models;

public class User : AppUser
{
    public ICollection<UserRequest> Requests { get; set; } = new List<UserRequest>();
}