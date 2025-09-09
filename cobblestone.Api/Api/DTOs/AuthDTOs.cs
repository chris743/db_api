namespace Api.DTOs
{
    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, DateTime RefreshTokenExpiresAt);
    public record RefreshRequest(string RefreshToken);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
    public record CreateUserRequest(string Username, string? Email, string Password, string? FullName, string Role, bool IsActive);
    public record UpdateUserRequest(string? Email, string? FullName, string? Role, bool? IsActive);
}
