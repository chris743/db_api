using Microsoft.EntityFrameworkCore;
using Api.Domain;

namespace Api.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users", "dbo");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();

                b.Property(x => x.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                b.Property(x => x.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                b.Property(x => x.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                b.Property(x => x.FullName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                b.Property(x => x.Role)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                b.Property(x => x.CreatedByUserId);
                b.Property(x => x.FailedLoginAttempts).HasDefaultValue(0);
                b.Property(x => x.IsActive).HasDefaultValue(true);

                // Indexes
                b.HasIndex(x => x.Username).IsUnique();
                b.HasIndex(x => x.Email).IsUnique();
                b.HasIndex(x => x.Role);
                b.HasIndex(x => x.IsActive);
                b.HasIndex(x => x.CreatedByUserId);

                // Foreign key
                b.HasOne(x => x.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // UserSession entity configuration
            modelBuilder.Entity<UserSession>(b =>
            {
                b.ToTable("UserSessions", "dbo");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();

                b.Property(x => x.UserId).IsRequired();
                b.Property(x => x.TokenHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                b.Property(x => x.IpAddress)
                    .HasMaxLength(45)
                    .IsUnicode(false);

                b.Property(x => x.UserAgent)
                    .IsUnicode(false);

                b.Property(x => x.IsActive).HasDefaultValue(true);

                // Indexes
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.TokenHash).IsUnique();
                b.HasIndex(x => x.ExpiresAt);
                b.HasIndex(x => x.IsActive);

                // Foreign key
                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PasswordResetToken entity configuration
            modelBuilder.Entity<PasswordResetToken>(b =>
            {
                b.ToTable("PasswordResetTokens", "dbo");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();

                b.Property(x => x.UserId).IsRequired();
                b.Property(x => x.TokenHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                b.Property(x => x.IsUsed).HasDefaultValue(false);

                // Indexes
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.TokenHash).IsUnique();
                b.HasIndex(x => x.ExpiresAt);
                b.HasIndex(x => x.IsUsed);

                // Foreign key
                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AuditLog entity configuration
            modelBuilder.Entity<AuditLog>(b =>
            {
                b.ToTable("AuditLogs", "dbo");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).ValueGeneratedOnAdd();

                b.Property(x => x.Action)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                b.Property(x => x.ResourceType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                b.Property(x => x.IpAddress)
                    .HasMaxLength(45)
                    .IsUnicode(false);

                b.Property(x => x.UserAgent)
                    .IsUnicode(false);

                b.Property(x => x.Details)
                    .IsUnicode(false);

                // Indexes
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.Action);
                b.HasIndex(x => x.ResourceType);
                b.HasIndex(x => x.ResourceId);
                b.HasIndex(x => x.CreatedAt);

                // Foreign key
                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
