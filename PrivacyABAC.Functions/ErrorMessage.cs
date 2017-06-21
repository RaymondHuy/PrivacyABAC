using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Functions
{
    static class ErrorFunctionMessage
    {
        public static string InvalidFormatTwoParameter()
        {
            return "Invalid format paramters {0}, {1} in function {2}";
        }

        public static string NotFound()
        {
            return "Can not find {0} ";
        }
    }
}
