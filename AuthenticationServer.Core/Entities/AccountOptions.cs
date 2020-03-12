using System;


namespace AuthenticationServer.Core.Entities
{
    public class AccountOptions
    {
        public static bool AllowLocalLogin = true;
        public static bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
