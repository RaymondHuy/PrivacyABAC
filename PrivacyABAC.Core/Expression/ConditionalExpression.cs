using Newtonsoft.Json.Linq;
using PrivacyABAC.DbInterfaces.Model;
using PrivacyABAC.Functions;
using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Core.Expression
{
    public class ConditionalExpression
    {
        public bool Evaluate(Function function, JObject subject, JObject resource, JObject environment)
        {
            var parameters = new List<string>();

            foreach (var param in function.Parameters)
            {
                // if parameter is another function
                if (param.Value == null)
                {
                    parameters.Add(InvokeFunction(param, subject, resource, environment));
                }
                else
                {
                    // if parameter is a constant value
                    if (param.ResourceID == null)
                    {
                        parameters.Add(param.Value);
                    }
                    // if parameter is a value taken from repository
                    else
                    {
                        JToken value = null;
                        switch (param.ResourceID)
                        {
                            case "Subject":
                                value = subject.SelectToken(param.Value);
                                break;
                            case "Environment":
                                value = environment.SelectToken(param.Value);
                                break;
                            default:
                                value = resource.SelectToken(param.Value);
                                break;
                        }
                        if (value == null)
                            throw new ConditionalExpressionException(string.Format(ErrorMessage.MissingField, param.Value, param.ResourceID));
                        else parameters.Add(value.ToString());
                    }
                }
            }
            var factory = UserDefinedFunctionFactory.GetInstance();
            string result = factory.ExecuteFunction(function.FunctionName, parameters.ToArray()).ToString();
            bool isConvertSuccessfully = Boolean.TryParse(result, out bool expressionResult);
            if (!isConvertSuccessfully)
                throw new ConditionalExpressionException(string.Format("Method {0} didn't return boolean value", function.FunctionName));

            return expressionResult;
        }

        private string InvokeFunction(Function function, JObject subject, JObject resource, JObject environment)
        {
            var parameters = new List<string>();

            foreach (var param in function.Parameters)
            {
                // if parameter is another function
                if (param.Value == null)
                {
                    string resultFunctionInvoke = InvokeFunction(param, subject, resource, environment);

                    bool isOrOperatorEscape = (function.FunctionName.Equals("Or", StringComparison.OrdinalIgnoreCase) && resultFunctionInvoke.Equals("true"));
                    bool isAndOperatorEscape = (function.FunctionName.Equals("And", StringComparison.OrdinalIgnoreCase) && resultFunctionInvoke.Equals("false"));

                    if (isOrOperatorEscape || isAndOperatorEscape)
                        return resultFunctionInvoke;
                    else parameters.Add(resultFunctionInvoke);
                }
                else
                {
                    // if parameter is a constant value
                    if (param.ResourceID == null)
                    {
                        parameters.Add(param.Value);
                    }
                    // if parameter is a value taken from repository
                    else
                    {
                        JToken value = null;
                        switch (param.ResourceID)
                        {
                            case "Subject":
                                value = subject.SelectToken(param.Value);
                                break;
                            case "Environment":
                                value = environment.SelectToken(param.Value);
                                break;
                            default:
                                value = resource.SelectToken(param.Value);
                                break;
                        }
                        if (value == null)
                            throw new ConditionalExpressionException(string.Format(ErrorMessage.MissingField, param.Value, param.ResourceID));

                        parameters.Add(value.ToString());
                    }
                }
            }
            var factory = UserDefinedFunctionFactory.GetInstance();
            string result = factory.ExecuteFunction(function.FunctionName, parameters.ToArray());
            return result;
        }
    }
}
