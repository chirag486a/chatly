using Chatly.Interfaces.Services;

namespace Chatly.Services;

using System;
using System.Collections.Generic;
using System.Linq;

public class PasswordFormatValidator : IPasswordFormatValidator
{
    public bool IsValid(string? password, out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password cannot be empty.");
            return errors.Count == 0;
        }

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long.");

        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter.");

        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter.");

        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit.");

        if (!password.Any(ch => "!@#$%^&*()_+-=[]{}|;:',.<>?/`~".Contains(ch)))
            errors.Add("Password must contain at least one special character.");

        return errors.Count == 0;
    }
}