namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Role { get; private set; } = "Client"; // Client, Admin
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        public User(string email, string passwordHash, string role)
        {
            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

        public void UpdateRefreshToken(string token, DateTime expiryDate)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiryDate;
        }
    }
}
