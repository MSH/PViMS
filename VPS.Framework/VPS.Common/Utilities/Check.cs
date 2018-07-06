using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Exceptions;

namespace VPS.Common.Utilities
{
    /// <summary>
    /// Static class used to Assert conditions, throwing an AssertionFailedException if the stated condition is not met.
    /// </summary>
    public class Check
    {
        /// <summary>
        /// Asserts that the specified value is true.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="message">The message.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void IsTrue(bool value, string message, params object[] values)
        {
            if (!value)
                throw new AssertionFailedException(string.Format(message, values));
        }

        /// <summary>
        /// Asserts that the specified value is false.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="message">The message.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void IsFalse(bool value, string message, params object[] values)
        {
            if (value)
                throw new AssertionFailedException(string.Format(message, values));
        }

        /// <summary>
        /// Asserts that that the specified value is not null.
        /// </summary>
        /// <param name="valueToCheck">The value to check.</param>
        /// <param name="message">The message.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void IsNotNull(object valueToCheck, string message = "Value cannot be null", params object[] values)
        {
	        if (valueToCheck == null)
	        {
		        throw new AssertionFailedException(string.Format(message, values));
	        }
        }

        /// <summary>
        /// Asserts that that the specified value is not emoty.
        /// </summary>
        /// <param name="valueToCheck">The value to check.</param>
        /// <param name="message">The message.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void IsEmpty(object valueToCheck, string message = "Value cannot be empty", params object[] values)
        {
            if (String.IsNullOrWhiteSpace(valueToCheck.ToString()))
            {
                throw new AssertionFailedException(string.Format(message, values));
            }
        }
    }
}
