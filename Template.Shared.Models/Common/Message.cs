using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Shared.Models.Common
{
    public static class Message
    {
        public static class User
        {
            public const string UserAlreadyExists = "A user with the same email or username already exists.";
            public const string UserNotFound = "The specified user was not found.";
            public const string InvalidCredentials = "The provided credentials are invalid.";
            public const string UserCreatedSuccessfully = "User created successfully.";
            public const string UserUpdatedSuccessfully = "User updated successfully.";
            public const string UserDeletedSuccessfully = "User deleted successfully.";
            public const string UserLockedOut = "The user account is locked out.";
            public const string UserDisabled = "The user account is disabled.";
        }
    }
}
