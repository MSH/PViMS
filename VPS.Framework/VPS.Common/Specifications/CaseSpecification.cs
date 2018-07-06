using System.Collections.Generic;

namespace VPS.Common.Specifications
{
	public class CaseSpecification<TCandidate, TValueType>
	{
		public TValueType DefaultValue { get; set; }

		public CaseSpecification(TValueType defaultValue)
		{
			DefaultValue = defaultValue;
			Items = new List<CaseSpecificationItem>();
		}

		public IList<CaseSpecificationItem> Items { get; private set; }

		public TValueType FindBestMatch(TCandidate candidate)
		{
			foreach (var specification in Items)
			{
				if (specification.Specification.IsSatisfiedBy(candidate))
				{
					return specification.Value;
				}
			}
			return DefaultValue;
		}

		public class CaseSpecificationItem
		{
			public CaseSpecificationItem(ISpecification<TCandidate> specification, TValueType value)
			{
				Specification = specification;
				Value = value;
			}

			public ISpecification<TCandidate> Specification { get; private set; }
			public TValueType Value { get; private set; }
		}
	}
}