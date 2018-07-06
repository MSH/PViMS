using System;

namespace VPS.Common.Specifications
{
	public class Rule<T> : ISpecification<T>
	{
		private readonly Func<T, bool> _rule;

		public Rule(Func<T, bool> rule)
		{
			_rule = rule;
		}

		public bool IsSatisfiedBy(T candidate)
		{
			return _rule(candidate);
		}

		public static Rule<T> FromJson(string json)
		{
			throw new NotImplementedException();
		}
	}
}