using Domain.ValueObject;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public abstract class AppUser : IdentityUser
{
    public FullName FullName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not AppUser entity)
            return false;

        return Id == entity.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}