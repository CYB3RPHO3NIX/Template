using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Shared.Models.Requests.Identity
{
    public class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
