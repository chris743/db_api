using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Domain;
using System.Security.Claims;

namespace Api.Services
{
    public class ZohoSessionService : IZohoSessionService
    {
        private readonly AuthDbContext _db;
        private readonly ILogger<ZohoSessionService> _logger;

        public ZohoSessionService(AuthDbContext db, ILogger<ZohoSessionService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ClaimsPrincipal?> ValidateSessionKeyAsync(string sessionKey)
        {
            if (string.IsNullOrEmpty(sessionKey))
                return null;

            try
            {
                var session = await _db.UserSessions
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.TokenHash == sessionKey && s.IsActive);

                if (session == null || session.User == null)
                {
                    _logger.LogWarning("Invalid or expired Zoho session key: {SessionKey}", sessionKey);
                    return null;
                }

                // Check if session is expired (if it has an expiration date)
                if (session.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("Expired Zoho session key: {SessionKey}", sessionKey);
                    return null;
                }

                // Create claims principal
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, session.User.Id.ToString()),
                    new Claim(ClaimTypes.Name, session.User.Username),
                    new Claim(ClaimTypes.Role, session.User.Role),
                    new Claim("SessionType", "Zoho"),
                    new Claim("SessionId", session.Id.ToString())
                };

                if (!string.IsNullOrEmpty(session.User.Email))
                {
                    claims.Add(new Claim(ClaimTypes.Email, session.User.Email));
                }

                var identity = new ClaimsIdentity(claims, "ZohoSession");
                return new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Zoho session key: {SessionKey}", sessionKey);
                return null;
            }
        }

        public async Task<UserSession?> CreateZohoSessionAsync(string sessionKey, int userId)
        {
            try
            {
                // Check if session already exists
                var existingSession = await _db.UserSessions
                    .FirstOrDefaultAsync(s => s.TokenHash == sessionKey);

                if (existingSession != null)
                {
                    // Update existing session
                    existingSession.UserId = userId;
                    existingSession.IsActive = true;
                    existingSession.ExpiresAt = DateTime.MaxValue; // Never expires
                }
                else
                {
                    // Create new session
                    var session = new UserSession
                    {
                        UserId = userId,
                        TokenHash = sessionKey,
                        ExpiresAt = DateTime.MaxValue, // Never expires
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IpAddress = "Zoho-Embedded",
                        UserAgent = "Zoho-Embedded-App"
                    };

                    _db.UserSessions.Add(session);
                    existingSession = session;
                }

                await _db.SaveChangesAsync();
                return existingSession;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Zoho session for user {UserId} with key {SessionKey}", userId, sessionKey);
                return null;
            }
        }

        public async Task<bool> IsValidSessionKeyAsync(string sessionKey)
        {
            if (string.IsNullOrEmpty(sessionKey))
                return false;

            try
            {
                var session = await _db.UserSessions
                    .FirstOrDefaultAsync(s => s.TokenHash == sessionKey && s.IsActive);

                if (session == null)
                    return false;

                // Check if session is expired (if it has an expiration date)
                if (session.ExpiresAt < DateTime.UtcNow)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Zoho session key validity: {SessionKey}", sessionKey);
                return false;
            }
        }
    }
}
