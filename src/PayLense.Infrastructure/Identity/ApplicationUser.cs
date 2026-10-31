using Microsoft.AspNetCore.Identity;

namespace PayLense.Infrastructure.Identity;

// UserName, Email, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed, PasswordHash...
// كلهم جايين جاهزين من IdentityUser
public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
