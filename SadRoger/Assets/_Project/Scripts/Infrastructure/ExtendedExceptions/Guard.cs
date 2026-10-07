using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Infrastructure.ExtendedExceptions
{
    public static class Guard
    {
        public static void AgainstNull(Object value, Func<ExtendedException> exceptionFactory)
        {
            if (!value)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNull(object value, Func<ExtendedException> exceptionFactory)
        {
            if (value == null)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNullOrEmpty<T>(IReadOnlyList<T> values, Func<ExtendedException> exceptionFactory)
        {
            if (values == null
             || values.Count == 0)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNullOrEmpty(string value, Func<ExtendedException> exceptionFactory)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNonPositive(float value, Func<ExtendedException> exceptionFactory)
        {
            if (value <= 0f)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNonPositive(int value, Func<ExtendedException> exceptionFactory)
        {
            if (value <= 0)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstNegative(float value, Func<ExtendedException> exceptionFactory)
        {
            if (value < 0f)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstLessThan(
            float value,
            float minimum,
            Func<ExtendedException> exceptionFactory)
        {
            if (value < minimum)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstLessThan(
            int value,
            int minimum,
            Func<ExtendedException> exceptionFactory)
        {
            if (value < minimum)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstGreaterThan(
            float value,
            float maximum,
            Func<ExtendedException> exceptionFactory)
        {
            if (value > maximum)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstGreaterThan(
            int value,
            int maximum,
            Func<ExtendedException> exceptionFactory)
        {
            if (value > maximum)
            {
                throw exceptionFactory();
            }
        }

        public static void AgainstInvalidRange(
            float min,
            float max,
            Func<ExtendedException> exceptionFactory)
        {
            AgainstGreaterThan(min, max, exceptionFactory);
        }

        public static void AgainstInvalidRange(
            int min,
            int max,
            Func<ExtendedException> exceptionFactory)
        {
            AgainstGreaterThan(min, max, exceptionFactory);
        }

        public static void AgainstTrue(bool condition, Func<ExtendedException> exceptionFactory)
        {
            if (condition)
            {
                throw exceptionFactory();
            }
        }
    }
}
