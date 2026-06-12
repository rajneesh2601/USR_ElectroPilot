using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Helpers
{
    public static class AppSession
    {
        public static UserModel CurrentUser { get; private set; }

        public static bool IsAuthenticated
        {
            get { return CurrentUser != null; }
        }

        public static string Username
        {
            get { return CurrentUser == null ? string.Empty : CurrentUser.Username; }
        }

        public static string Role
        {
            get { return CurrentUser == null ? string.Empty : CurrentUser.Role; }
        }

        public static void SignIn(UserModel user)
        {
            CurrentUser = user;
        }

        public static void SignOut()
        {
            CurrentUser = null;
        }

        public static bool HasRole(params string[] roles)
        {
            if (CurrentUser == null || roles == null)
            {
                return false;
            }

            foreach (var role in roles)
            {
                if (string.Equals(CurrentUser.Role, role, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
