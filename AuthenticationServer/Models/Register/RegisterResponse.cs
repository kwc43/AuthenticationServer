using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Core.Entities;

namespace AuthenticationServer.Models.Register
{
    public class RegisterResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public RegisterResponse() { }

        public RegisterResponse(AppUser user, string role)
        {
            Id = user.Id;
            FullName = user.FullName;
            Email = user.Email;
            Role = role;
        }
    }
}
