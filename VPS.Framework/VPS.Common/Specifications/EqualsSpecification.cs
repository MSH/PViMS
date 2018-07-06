namespace VPS.Common.Specifications
{
	public class EqualsSpecification<T> : ISpecification<T>
	{
		private readonly T _value;

		public EqualsSpecification(T value)
		{
			_value = value;
		}

		public bool IsSatisfiedBy(T candidate)
		{
			return candidate.Equals(_value);
		}
	}
}