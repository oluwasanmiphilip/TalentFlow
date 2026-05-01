using System;

namespace TalentFlow.Application.Common.Exceptions
{
    public class DuplicateEmailException : Exception
    {
        public string Email { get; }

        public DuplicateEmailException(string email)
            : base($"The email '{email}' is already registered.")
        {
            Email = email;
        }

        // ✅ Overload that accepts an inner exception
        public DuplicateEmailException(string email, Exception innerException)
            : base($"The email '{email}' is already registered.", innerException)
        {
            Email = email;
        }
    }
}
