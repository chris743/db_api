using BCrypt.Net;

namespace Api.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly int _workFactor;

        public PasswordService(IConfiguration configuration)
        {
            _workFactor = configuration.GetValue<int>("BCrypt:WorkFactor", 12);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
