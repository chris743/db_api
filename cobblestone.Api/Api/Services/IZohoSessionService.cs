using System.Security.Claims;
using Api.Domain;

namespace Api.Services
{
    public interface IZohoSessionService
    {
        Task<ClaimsPrincipal?> ValidateSessionKeyAsync(string sessionKey);
        Task<UserSession?> CreateZohoSessionAsync(string sessionKey, int userId);
        Task<bool> IsValidSessionKeyAsync(string sessionKey);
    }
}
