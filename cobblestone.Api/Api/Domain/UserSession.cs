namespace Api.Domain
{
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; } = default!;   // SHA256 of refresh token
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsActive { get; set; } = true;

        public User? User { get; set; }
    }
}