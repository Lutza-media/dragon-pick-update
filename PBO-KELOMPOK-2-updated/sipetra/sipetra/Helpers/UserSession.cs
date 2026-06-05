using System;
using System.Collections.Generic;
using System.Text;
using sipetra.Models;

namespace sipetra.Helpers
{
    public sealed class UserSession
    {
        private static readonly Lazy<UserSession> lazy =
            new Lazy<UserSession>(() => new UserSession());
        public static UserSession Instance { get { return lazy.Value; } }
        public User CurrentUser { get; private set; }
        private UserSession() {}
        public void SetUser(User user)
        {
            CurrentUser = user;
        }
        public void ClearSession()
        {
            CurrentUser = null;
        }

        public bool IsLoggedIn()
        {
            return CurrentUser != null;
        }
    }
}
