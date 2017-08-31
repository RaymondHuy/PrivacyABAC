using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PrivacyABAC.DbInterfaces.Model;
using PrivacyABAC.Functions;
using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PrivacyABAC.Core.Service
{
    public class ConditionalExpressionService
    {
        private readonly ILogger<ConditionalExpressionService> _logger;

        public ConditionalExpressionService(ILogger<ConditionalExpressionService> logger)
        {
            _logger = logger;
        }

        public bool Evaluate(Function function, JObject subject, JObject resource, JObject environment)
        {
            if (function == null)
                return true;

            var parameters = new List<object>();

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
                        //if (value == null)
                        //    throw new ConditionalExpressionException(string.Format(ErrorMessage.MissingField, param.Value, param.ResourceID));
                        parameters.Add(value);
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

        public bool IsAccessControlPolicyRelateToContext(AccessControlPolicy policy, JObject user, JObject resource, JObject environment)
        {
            return true;
        }

        public bool IsPrivacyPolicyRelateToContext(PrivacyPolicy policy, JObject user, JObject resource, JObject environment)
        {
            return false;
        }

        /// <summary>
        /// Or (Equal (Resource._id,1232), Equal (Subject.role,leader))
        /// Equal ( Resource._id , Function1 ( Resource.name , b ) ) Or Equal ( Subject.role , leader )
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Function Parse(string condition)
        {
            var queue = PolandNotationProcess(condition);
            var stackBuilder = new Stack<Function>();
            while (queue.Any())
            {
                string keyword = queue.Dequeue();
                var method = GetUserFunction(keyword);
                if (method != null)
                {
                    var function = new Function()
                    {
                        FunctionName = keyword,
                        Parameters = new List<Function>()
                    };
                    int count = method.NumberParameters;
                    for (int i = 0; i < count; i++)
                    {
                        function.Parameters.Insert(0, stackBuilder.Pop());
                    }
                    stackBuilder.Push(function);
                }
                else if (keyword.Equals("Logic.And", StringComparison.OrdinalIgnoreCase))
                {
                    var function = new Function()
                    {
                        FunctionName = "Logic.And",
                        Parameters = new List<Function>()
                    };
                    function.Parameters.Insert(0, stackBuilder.Pop());
                    function.Parameters.Insert(0, stackBuilder.Pop());
                    stackBuilder.Push(function);
                }
                else if (keyword.Equals("Logic.Or"))
                {
                    var function = new Function()
                    {
                        FunctionName = "Logic.Or",
                        Parameters = new List<Function>()
                    };
                    function.Parameters.Insert(0, stackBuilder.Pop());
                    function.Parameters.Insert(0, stackBuilder.Pop());
                    stackBuilder.Push(function);
                }
                else if (keyword.Equals("Logic.Not"))
                {
                    var function = new Function()
                    {
                        FunctionName = "Logic.Not",
                        Parameters = new List<Function>()
                    };
                    function.Parameters.Add(stackBuilder.Pop());
                    stackBuilder.Push(function);
                }
                else if (keyword.Contains("."))
                {
                    int idxResourceName = keyword.IndexOf('.');
                    var function = new Function()
                    {
                        ResourceID = keyword.Substring(0, idxResourceName),
                        Value = keyword.Substring(idxResourceName + 1)
                    };
                    stackBuilder.Push(function);
                }
                else stackBuilder.Push(new Function() { Value = keyword });
            }
            return stackBuilder.Pop();
        }
        public Queue<string> PolandNotationProcess(string condition)
        {
            var stack = new Stack<string>();
            var queue = new Queue<string>();

            var resultFunction = new Function();
            ICollection<string> keywords = ParseTokens(condition);
            #region Poland Notation
            foreach (var keyword in keywords)
            {
                if (IsLogicOperator(keyword))
                {
                    if (!stack.Any())
                    {
                        stack.Push(keyword);
                        continue;
                    }
                    string op = stack.Peek();
                    if (op.Equals("("))
                    {
                        stack.Push(keyword);
                        continue;
                    }
                    if (Priority(keyword) <= Priority(op))
                    {
                        while (stack.Count() != 0)
                        {
                            string s = stack.Peek();
                            if (s.Equals("("))
                                break;
                            else
                            {
                                string temp = stack.Pop();
                                queue.Enqueue(temp);
                            }
                        }
                        stack.Push(keyword);
                        continue;
                    }
                    stack.Push(keyword);
                }
                else if (keyword.Equals("(", StringComparison.OrdinalIgnoreCase))
                {
                    stack.Push(keyword);
                }
                else if (GetUserFunction(keyword) != null)
                {
                    stack.Push(keyword);
                }
                else if (keyword.Equals(")", StringComparison.OrdinalIgnoreCase))
                {
                    while (stack.Count() != 0)
                    {
                        string s = stack.Pop();
                        if (s.Equals("(", StringComparison.OrdinalIgnoreCase))
                        {
                            string methodName = stack.Any() ? stack.Pop() : String.Empty;
                            if (methodName != null)
                                queue.Enqueue(methodName);
                            break;
                        }
                        queue.Enqueue(s);
                    }
                }
                else if (keyword.Equals(",") || keyword.Equals(""))
                    continue;
                else queue.Enqueue(keyword);
            }
            while (stack.Count() != 0)
            {
                queue.Enqueue(stack.Pop());
            }
            #endregion
            return queue;
        }
        private FunctionInfo GetUserFunction(string keyword)
        {
            var factory = UserDefinedFunctionFactory.GetInstance();
            return factory.GetFunction(keyword);
        }
        private ICollection<string> ParseTokens(string s)
        {
            var result = new List<string>();
            string[] tokens = s.Split(' ');
            for (int i = 0; i < s.Length; i++)
            {
                string keyword = s[i].ToString();
                string token = "";
                if (keyword.Equals(",") || keyword.Equals(" ") || keyword.Equals("")) continue;
                if (keyword.Equals("'"))
                {
                    ++i;
                    while (i < s.Length)
                    {
                        if (s[i].Equals('\'')) break;
                        else token += s[i].ToString();
                        ++i;
                    }
                }
                else
                {
                    while (i < s.Length)
                    {
                        if (s[i].Equals('\'') || s[i].Equals(' ')) break;
                        else token += s[i].ToString();
                        ++i;
                    }
                }
                result.Add(token);
            }
            return result;
        }
        private int Priority(string op)
        {
            if (op.Equals("NOT", StringComparison.OrdinalIgnoreCase))
                return 3;
            else if (op.Equals("AND", StringComparison.OrdinalIgnoreCase))
                return 2;
            else return 1;
        }
        private bool IsLogicOperator(string keyword)
        {
            if (keyword.Equals("AND", StringComparison.OrdinalIgnoreCase)
             || keyword.Equals("OR", StringComparison.OrdinalIgnoreCase)
             || keyword.Equals("NOT", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}
