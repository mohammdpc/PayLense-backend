using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayLense.Application.DTOs.Auth;
using PayLense.Application.Interfaces;

namespace PayLense.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("signup")]
    public async Task<ActionResult<MessageResponse>> Signup(SignupRequest request) =>
        Ok(await _auth.SignupAsync(request));

    [HttpPost("verify-email")]
    public async Task<ActionResult<MessageResponse>> VerifyEmail(VerifyEmailRequest request) =>
        Ok(await _auth.VerifyEmailAsync(request));

    [HttpPost("verify-phone")]
    public async Task<ActionResult<MessageResponse>> VerifyPhone(VerifyPhoneRequest request) =>
        Ok(await _auth.VerifyPhoneAsync(request));

    [HttpPost("resend-otp")]
    public async Task<ActionResult<MessageResponse>> ResendOtp(ResendOtpRequest request) =>
        Ok(await _auth.ResendOtpAsync(request));

    [HttpPost("signin")]
    public async Task<ActionResult<SigninResponse>> Signin(SigninRequest request) =>
        Ok(await _auth.SigninAsync(request));

    [HttpPost("signin/verify-2fa")]
    public async Task<ActionResult<AuthResponse>> VerifyTwoFactor(VerifyTwoFactorRequest request) =>
        Ok(await _auth.VerifyTwoFactorAsync(request));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request) =>
        Ok(await _auth.RefreshAsync(request));

    [HttpPost("logout")]
    public async Task<ActionResult<MessageResponse>> Logout(LogoutRequest request) =>
        Ok(await _auth.LogoutAsync(request));

    // endpoint لاختبار إن الـ JWT شغال (مؤقت)
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        userId = User.FindFirst("sub")?.Value,
        username = User.FindFirst("unique_name")?.Value,
        email = User.FindFirst("email")?.Value,
        deviceId = User.FindFirst("device_id")?.Value
    });
}
