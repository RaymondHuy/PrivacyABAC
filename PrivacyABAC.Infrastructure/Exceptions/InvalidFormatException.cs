using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Infrastructure.Exceptions
{
    public class InvalidFormatException : Exception
    {
        public InvalidFormatException()
            : base() { }

        public InvalidFormatException(string message)
            : base(message) { }

        public InvalidFormatException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public InvalidFormatException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidFormatException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}
