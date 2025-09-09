using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Domain
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public bool IsUsed { get; set; } = false;

        public User? User { get; set; }
    }
}