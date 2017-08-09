
using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.Utilities
{
    public static class FunctionUtility
    {
        public static string Convert(Function func)
        {
            if (func == null) return "";
            string result = "";
            if (func.FunctionName != null)
            {
                if (func.FunctionName.Equals("And") || func.FunctionName.Equals("Or"))
                {
                    result += Convert(func.Parameters[0]) + " " + func.FunctionName + " " + Convert(func.Parameters[1]);

                }
                else
                {
                    result += func.FunctionName + " (";
                    for (int i = 0; i < func.Parameters.Count; i++)
                    {
                        if (i == func.Parameters.Count - 1)
                            result += " " + Convert(func.Parameters[i]);
                        else result += " " + Convert(func.Parameters[i]) + " ,";
                    }
                    result += " )";
                }
            }
            else if (func.ResourceID != null)
            {
                result += func.ResourceID + "." + func.Value;
            }
            else result += func.Value;
            return result;
        }
    }
}
