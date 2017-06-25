using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Infrastructure.Exceptions
{
    public class ConditionalExpressionException : Exception
    {
        public ConditionalExpressionException()
            : base() { }

        public ConditionalExpressionException(string message)
            : base(message) { }

        public ConditionalExpressionException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public ConditionalExpressionException(string message, Exception innerException)
            : base(message, innerException) { }

        public ConditionalExpressionException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}
