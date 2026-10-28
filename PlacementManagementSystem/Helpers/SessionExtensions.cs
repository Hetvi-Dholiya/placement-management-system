using System.Text.Json;

namespace PlacementManagementSystem.Helpers
{
    public static class SessionConstants
    {
        public const string UserId = "UserId";
        public const string UserName = "UserName";
        public const string UserEmail = "UserEmail";
        public const string UserRole = "UserRole";
        public const string UserProfilePicture = "UserProfilePicture";
        public const string RoleAdmin = "Admin";
        public const string RoleStudent = "Student";
    }

    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static bool IsLoggedIn(this ISession session)
        {
            return session.GetInt32(SessionConstants.UserId).HasValue;
        }

        public static bool IsAdmin(this ISession session)
        {
            return session.GetString(SessionConstants.UserRole) == SessionConstants.RoleAdmin;
        }

        public static bool IsStudent(this ISession session)
        {
            return session.GetString(SessionConstants.UserRole) == SessionConstants.RoleStudent;
        }

        public static int? GetCurrentUserId(this ISession session)
        {
            return session.GetInt32(SessionConstants.UserId);
        }

        public static string GetCurrentUserName(this ISession session)
        {
            return session.GetString(SessionConstants.UserName) ?? "Guest";
        }

        public static string GetCurrentUserRole(this ISession session)
        {
            return session.GetString(SessionConstants.UserRole) ?? string.Empty;
        }
    }
}
