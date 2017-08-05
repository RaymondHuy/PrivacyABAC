using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Infrastructure.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> ErrorMessages { get; set; }

        public ValidationException(string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.ErrorMessages = new[] { message };
        }

        public ValidationException(IEnumerable<string> messages, Exception innerException = null)
            : base(string.Join("\r\n", messages), innerException)
        {
            ErrorMessages = messages;
        }
    }
}
