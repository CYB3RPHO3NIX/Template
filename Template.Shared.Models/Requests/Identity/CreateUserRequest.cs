using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Template.Shared.Models.Common;

namespace Template.Shared.Models.Requests.Identity
{
    public class CreateUserRequest : IRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public ValidationResult Validate()
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(UserName))
            {
                validationResult.Errors.Add("UserName is required.");
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                validationResult.Errors.Add("Email is required.");
            }

            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                validationResult.Errors.Add("Invalid email format.");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                validationResult.Errors.Add("Password is required.");
            }

            return validationResult;
        }
    }
}
