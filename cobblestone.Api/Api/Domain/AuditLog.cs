using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Domain
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; } = default!;
        public string? ResourceType { get; set; }
        public int? ResourceId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Details { get; set; } // JSON data
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
    }
}