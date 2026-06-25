using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Shared.Models.DTOs.Identity
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
