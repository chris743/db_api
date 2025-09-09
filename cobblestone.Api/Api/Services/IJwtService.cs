using System.Security.Claims;

namespace Api.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(int userId, string username, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        bool ValidateRefreshToken(string refreshToken);
    }
}
