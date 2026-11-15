using PayLense.Application.DTOs.Auth;

namespace PayLense.Application.Interfaces;

public interface IAuthService
{
    Task<MessageResponse> SignupAsync(SignupRequest request);
    Task<MessageResponse> VerifyEmailAsync(VerifyEmailRequest request);
    Task<MessageResponse> VerifyPhoneAsync(VerifyPhoneRequest request);
    Task<MessageResponse> ResendOtpAsync(ResendOtpRequest request);
    Task<SigninResponse> SigninAsync(SigninRequest request);
    Task<AuthResponse> VerifyTwoFactorAsync(VerifyTwoFactorRequest request);
    Task<AuthResponse> RefreshAsync(RefreshRequest request);
    Task<MessageResponse> LogoutAsync(LogoutRequest request);
}
