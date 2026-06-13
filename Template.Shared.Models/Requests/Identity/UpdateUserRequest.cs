using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Template.Shared.Models.Common;

namespace Template.Shared.Models.Requests.Identity
{
    public class UpdateUserRequest : IRequest
    {
        public string? UserName { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? Password { get; set; } = null;
        public bool? IsActive { get; set; } = null;

        public ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            
            if (!string.IsNullOrWhiteSpace(Email))
            {
                if(!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    result.Errors.Add("Invalid email address.");
                }
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                if(Password.Length < 6)
                {
                    result.Errors.Add("Password must be at least 6 characters long.");
                }
            }

            return result;
        }
    }
}