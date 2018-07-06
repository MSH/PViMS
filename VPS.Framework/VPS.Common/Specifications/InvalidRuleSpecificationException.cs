using System;

namespace VPS.Common.Specifications
{
	public class InvalidRuleSpecificationException : Exception
	{
		public InvalidRuleSpecificationException(string message) : base(message)
		{
		}
	}
}