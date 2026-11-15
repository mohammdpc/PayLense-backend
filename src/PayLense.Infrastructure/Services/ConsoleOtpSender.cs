using Microsoft.Extensions.Logging;
using PayLense.Application.Interfaces;
using PayLense.Domain.Enums;

namespace PayLense.Infrastructure.Services;

/// Mock للتطوير: بيطبع الـ OTP في الـ Console بدل ما يبعته فعلاً.
/// لما تربط SMS/Email provider حقيقي، اعمل implementation جديد لـ IOtpSender
/// وبدّله في DependencyInjection.cs. وامسح الـ logging ده في الـ production.
public class ConsoleOtpSender : IOtpSender
{
    private readonly ILogger<ConsoleOtpSender> _logger;

    public ConsoleOtpSender(ILogger<ConsoleOtpSender> logger) => _logger = logger;

    public Task SendEmailAsync(string email, string code, OtpPurpose purpose)
    {
        _logger.LogWarning("[DEV OTP] EMAIL to {Email} | {Purpose} | code: {Code}", email, purpose, code);
        return Task.CompletedTask;
    }

    public Task SendSmsAsync(string phoneNumber, string code, OtpPurpose purpose)
    {
        _logger.LogWarning("[DEV OTP] SMS to {Phone} | {Purpose} | code: {Code}", phoneNumber, purpose, code);
        return Task.CompletedTask;
    }
}
