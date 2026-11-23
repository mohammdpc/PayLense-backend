using Microsoft.AspNetCore.Identity;

namespace PayLense.Infrastructure.Identity;

// UserName, Email, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed, PasswordHash...
// كلهم جايين جاهزين من IdentityUser مع إضافة أعمدة جدول Users من الـ Schema
public class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
