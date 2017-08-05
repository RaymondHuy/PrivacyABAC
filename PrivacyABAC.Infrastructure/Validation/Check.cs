using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Infrastructure.Validation
{
    public class Check
    {
        public static void NotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ValidationException($"{name} must not be null.");
            }
        }

        public static void NotDefaultValue(Guid value, string name)
        {
            if (value == default(Guid))
            {
                throw new ValidationException($"{name} must be specified.");
            }
        }

        public static void Min(double value, double minValue, string name)
        {
            if (value < minValue)
            {
                throw new ValidationException($"{name} must be greater than {minValue}.");
            }
        }

        public static void NotEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ValidationException($"{name} must be specified.");
            }
        }

        public static void Min(int value, int minimumValue, string name)
        {
            if (value < minimumValue)
            {
                throw new ValidationException($"{name} must be greater than {minimumValue}.");
            }
        }

        public static void Max(int value, int maximumValue, string name)
        {
            if (value > maximumValue)
            {
                throw new ValidationException($"{name} must be less than {maximumValue}.");
            }
        }


        public static void Max(double value, double maximumValue, string name)
        {
            if (value > maximumValue)
            {
                throw new ValidationException($"{name} must be less than {maximumValue}.");
            }
        }

        public static void Is<T>(T value, T expectedValue, string name = "",
            string errorMesage = "")
        {
            if (!object.Equals(value, expectedValue))
            {
                var error = string.IsNullOrWhiteSpace(errorMesage)
                    ? $"{name} must be {expectedValue}."
                    : errorMesage;
                throw new ValidationException(error);
            }
        }
    }
}

