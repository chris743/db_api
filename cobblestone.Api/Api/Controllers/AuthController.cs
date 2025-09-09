using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Domain;
using Api.DTOs;
using Api.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _db;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthDbContext db, 
            IJwtService jwtService, 
            IPasswordService passwordService,
            ILogger<AuthController> logger)
        {
            _db = db;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive, ct);

            if (user == null)
            {
                _logger.LogWarning("Login attempt with invalid username: {Username}", request.Username);
                return Unauthorized("Invalid username or password");
            }

            // Check if account is locked
            if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
            {
                _logger.LogWarning("Login attempt on locked account: {Username}", request.Username);
                return StatusCode(423, "Account is temporarily locked due to failed login attempts");
            }

            // Verify password
            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                // Increment failed login attempts
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5) // Max attempts
                {
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(15); // Lock for 15 minutes
                }
                await _db.SaveChangesAsync(ct);

                _logger.LogWarning("Login attempt with invalid password for user: {Username}", request.Username);
                return Unauthorized("Invalid username or password");
            }

            // Reset failed login attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.LastLogin = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Username, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Store refresh token in database
            var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
            var userSession = new UserSession
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString()
            };

            _db.UserSessions.Add(userSession);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("User {Username} logged in successfully", user.Username);

            return Ok(new LoginResponse(
                accessToken,
                DateTime.UtcNow.AddMinutes(30), // Access token expires in 30 minutes
                refreshToken,
                DateTime.UtcNow.AddDays(7) // Refresh token expires in 7 days
            ));
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout(RefreshRequest request, CancellationToken ct)
        {
            var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));
            
            var session = await _db.UserSessions
                .FirstOrDefaultAsync(s => s.TokenHash == tokenHash && s.IsActive, ct);

            if (session != null)
            {
                session.IsActive = false;
                await _db.SaveChangesAsync(ct);
            }

            return Ok();
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Refresh(RefreshRequest request, CancellationToken ct)
        {
            var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));
            
            var session = await _db.UserSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.TokenHash == tokenHash && s.IsActive && s.ExpiresAt > DateTime.UtcNow, ct);

            if (session?.User == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            // Generate new access token
            var accessToken = _jwtService.GenerateAccessToken(session.User.Id, session.User.Username, session.User.Role);

            return Ok(new LoginResponse(
                accessToken,
                DateTime.UtcNow.AddMinutes(30),
                request.RefreshToken, // Keep the same refresh token
                session.ExpiresAt
            ));
        }

        [HttpGet("verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Verify()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(new { UserId = userId, Username = username, Role = role });
        }
    }
}
