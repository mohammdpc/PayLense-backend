using PayLense.Domain.Enums;

namespace PayLense.Application.Interfaces;

public interface IOtpService
{
    /// يولّد كود جديد (6 أرقام) ويبطّل القديم. بيرجّع الكود الـ plain عشان يتبعت.
    Task<string> GenerateAsync(string userId, OtpPurpose purpose);

    Task<bool> VerifyAsync(string userId, OtpPurpose purpose, string code);
}
