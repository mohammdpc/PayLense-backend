namespace PayLense.Domain.Entities;

public class UserContactMethod
{
    public long ContactId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ContactType { get; set; } = "Phone"; // Phone / Email
    public string ContactValue { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ContactVerificationChallenge> VerificationChallenges { get; set; } = new List<ContactVerificationChallenge>();
    public ICollection<PhoneChangeRequest> PhoneChangeRequests { get; set; } = new List<PhoneChangeRequest>();
    public ICollection<EmailChangeRequest> EmailChangeRequests { get; set; } = new List<EmailChangeRequest>();
}
