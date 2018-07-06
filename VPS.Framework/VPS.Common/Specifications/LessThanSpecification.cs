using System;

namespace VPS.Common.Specifications
{
	public class LessThanSpecification<T> : ComparableSpecification<T>
	{
		private readonly T _value;

		public LessThanSpecification(T value)
		{
			_value = value;
		}

		public override bool IsSatisfiedBy(T candidate)
		{
			return TryConvertToComparable(candidate).CompareTo(_value) < 0;
		}
	}
}