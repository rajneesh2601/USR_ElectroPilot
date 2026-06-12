using System;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class AuthService
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        private readonly UserRepository _userRepository;
        private readonly UserActivityService _userActivityService;

        public AuthService()
            : this(new UserRepository(), new UserActivityService())
        {
        }

        public AuthService(UserRepository userRepository, UserActivityService userActivityService)
        {
            _userRepository = userRepository;
            _userActivityService = userActivityService;
        }

        public bool Login(string username, string password, out string message)
        {
            DatabaseHelper.InitializeDatabase();

            var user = _userRepository.GetByUsername(username);
            if (user == null)
            {
                message = "Invalid username or password.";
                _userActivityService.RecordActivity(null, username, "LoginFailed", "Unknown username");
                return false;
            }

            if (!user.IsActive)
            {
                message = "User account is inactive.";
                _userActivityService.RecordActivity(user.Id, user.Username, "LoginBlocked", "Inactive account");
                return false;
            }

            if (user.LockoutUntil.HasValue && user.LockoutUntil.Value > DateTime.Now)
            {
                message = "User account is locked until " + user.LockoutUntil.Value.ToString("g") + ".";
                _userActivityService.RecordActivity(user.Id, user.Username, "LoginBlocked", "Account locked");
                return false;
            }

            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                RegisterFailedLogin(user);
                message = "Invalid username or password.";
                return false;
            }

            user.FailedLoginCount = 0;
            user.LockoutUntil = null;
            user.LastLoginAt = DateTime.Now;
            _userRepository.Update(user);
            AppSession.SignIn(user);
            _userActivityService.RecordActivity(user.Id, user.Username, "LoginSuccess", "User signed in");

            message = "Login successful.";
            return true;
        }

        public void Logout()
        {
            if (AppSession.CurrentUser != null)
            {
                _userActivityService.RecordActivity(
                    AppSession.CurrentUser.Id,
                    AppSession.CurrentUser.Username,
                    "Logout",
                    "User signed out");
            }

            AppSession.SignOut();
        }

        private void RegisterFailedLogin(UserModel user)
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= MaxFailedAttempts)
            {
                user.LockoutUntil = DateTime.Now.Add(LockoutDuration);
            }

            _userRepository.Update(user);
            _userActivityService.RecordActivity(
                user.Id,
                user.Username,
                "LoginFailed",
                "Failed attempt " + user.FailedLoginCount);
        }
    }
}
