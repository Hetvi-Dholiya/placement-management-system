namespace PlacementManagementSystem.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 10);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword)) return false;
            try
            {
                // Handle BCrypt hash
                if (hashedPassword.StartsWith("$2a$") || hashedPassword.StartsWith("$2b$") || hashedPassword.StartsWith("$2y$"))
                {
                    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                }
                // Fallback for plain-text legacy if any
                return password == hashedPassword;
            }
            catch
            {
                return false;
            }
        }
    }
}
