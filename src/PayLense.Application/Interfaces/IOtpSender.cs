using PayLense.Domain.Enums;

namespace PayLense.Application.Interfaces;

public interface IOtpSender
{
    Task SendEmailAsync(string email, string code, OtpPurpose purpose);
    Task SendSmsAsync(string phoneNumber, string code, OtpPurpose purpose);
}
