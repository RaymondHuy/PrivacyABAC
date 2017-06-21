using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Infrastructure.Exceptions
{
    public class FunctionNotFoundException : Exception
    {
        public FunctionNotFoundException()
            : base() { }

        public FunctionNotFoundException(string message)
            : base(message) { }

        public FunctionNotFoundException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public FunctionNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        public FunctionNotFoundException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}
