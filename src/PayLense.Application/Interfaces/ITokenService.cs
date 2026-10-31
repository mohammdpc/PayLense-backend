namespace PayLense.Application.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateAccessToken(string userId, string userName, string email, string deviceId);
    (string Plain, string Hash) CreateRefreshToken();
    string Hash(string value);
}
