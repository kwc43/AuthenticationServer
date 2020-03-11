using Microsoft.AspNetCore.Identity;

namespace AuthenticationServer.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
