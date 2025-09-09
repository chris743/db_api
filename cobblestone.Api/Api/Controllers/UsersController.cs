using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Domain;
using Api.DTOs;
using Api.Services;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly AuthDbContext _db;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AuthDbContext db, IPasswordService passwordService, ILogger<UsersController> logger)
        {
            _db = db;
            _passwordService = passwordService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] int skip = 0, [FromQuery] int take = 100, CancellationToken ct = default)
        {
            take = Math.Clamp(take, 1, 500);
            
            var users = await _db.Users
                .AsNoTracking()
                .OrderBy(u => u.Username)
                .Skip(skip)
                .Take(take)
                .Select(u => new UserDto(
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt,
                    u.LastLogin
                ))
                .ToListAsync(ct);

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(int id, CancellationToken ct)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserDto(
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt,
                    u.LastLogin
                ))
                .FirstOrDefaultAsync(ct);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request, CancellationToken ct)
        {
            // Check if username already exists
            if (await _db.Users.AnyAsync(u => u.Username == request.Username, ct))
            {
                return BadRequest("Username already exists");
            }

            // Check if email already exists (if provided)
            if (!string.IsNullOrEmpty(request.Email) && await _db.Users.AnyAsync(u => u.Email == request.Email, ct))
            {
                return BadRequest("Email already exists");
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                FullName = request.FullName,
                Role = request.Role,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("User {Username} created by user {CreatedBy}", user.Username, currentUserId);

            var userDto = new UserDto(
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Role,
                user.IsActive,
                user.CreatedAt,
                user.LastLogin
            );

            return CreatedAtAction(nameof(GetUser), new { id = user.Id, version = "1" }, userDto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user == null)
            {
                return NotFound();
            }

            // Check if email already exists (if being changed)
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                if (await _db.Users.AnyAsync(u => u.Email == request.Email && u.Id != id, ct))
                {
                    return BadRequest("Email already exists");
                }
            }

            if (request.Email != null) user.Email = request.Email;
            if (request.FullName != null) user.FullName = request.FullName;
            if (request.Role != null) user.Role = request.Role;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;
            
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} updated", id);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user == null)
            {
                return NotFound();
            }

            // Soft delete - just deactivate
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} deactivated", id);

            return NoContent();
        }

        [HttpPost("{id:int}/reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user == null)
            {
                return NotFound();
            }

            user.PasswordHash = _passwordService.HashPassword(newPassword);
            user.PasswordChangedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Password reset for user {UserId}", id);

            return NoContent();
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest("Current password is incorrect");
            }

            user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
            user.PasswordChangedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Password changed for user {UserId}", userId);

            return NoContent();
        }
    }

    public record UserDto(
        int Id,
        string Username,
        string? Email,
        string? FullName,
        string Role,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? LastLogin
    );
}
