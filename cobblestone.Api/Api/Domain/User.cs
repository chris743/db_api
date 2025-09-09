namespace Api.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string? Email { get; set; }
        public string PasswordHash { get; set; } = default!;
        public string? FullName { get; set; }
        public string Role { get; set; } = "user"; // admin|manager|user|readonly
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }

        // Navigation property
        public User? CreatedByUser { get; set; }
    }
}