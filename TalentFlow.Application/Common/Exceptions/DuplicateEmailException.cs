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
    }
}
