using System.Text.RegularExpressions;

namespace ECommerce.Helpers
{
    public static class PasswordValidator
    {
        public static bool IsStrong(string password)
        {
            return Regex.IsMatch(password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$");
        }
    }
}
