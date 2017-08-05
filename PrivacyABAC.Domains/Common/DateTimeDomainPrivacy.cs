using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.Domains.Common
{
    public class DateTimeDomainPrivacy : IPluginDomain
    {
        public static string ShowDayAndMonth(string dateTime)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.Day.ToString() + "/" + dt.Month.ToString();
        }
        public static string ShowMonthAndYear(string dateTime)
        {
            DateTime dt = DateTime.Parse(dateTime);
            return dt.Month.ToString() + "/" + dt.Year.ToString();
        }
        public static string ShowYear(string dateTime)
        {
            return DateTime.Parse(dateTime).Year.ToString();
        }

        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            if (functionName.Equals("ShowDayAndMonth", StringComparison.OrdinalIgnoreCase))
                return ShowDayAndMonth(parameters[0]);
            else if (functionName.Equals("ShowMonthAndYear", StringComparison.OrdinalIgnoreCase))
                return ShowMonthAndYear(parameters[0]);
            else if (functionName.Equals("ShowYear", StringComparison.OrdinalIgnoreCase))
                return ShowYear(parameters[0]);

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }

        public string GetName() => "DateTimeDomain";

        public string[] GetRegisteredFunctions()
        {
            return new string[] { "ShowDayAndMonth", "ShowMonthAndYear", "ShowYear" };
        }
    }
}
