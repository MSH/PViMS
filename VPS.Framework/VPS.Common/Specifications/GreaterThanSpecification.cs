using System;

namespace VPS.Common.Specifications
{
	public class GreaterThanSpecification<T> : ComparableSpecification<T>
	{
		private readonly T _value;

		public GreaterThanSpecification(T value)
		{
			_value = value;
		}

		public override bool IsSatisfiedBy(T candidate)
		{
			var comparable = TryConvertToComparable(candidate);

			return comparable.CompareTo(_value) > 0;
		}
	}
}