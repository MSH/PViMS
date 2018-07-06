using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Specifications
{
	public static class SpecificationExtensionMethods
	{
		public static ISpecification<TCandidate> And<TCandidate>(this ISpecification<TCandidate> specification,
			ISpecification<TCandidate> other)
		{
			return new AndSpecification<TCandidate>(specification, other);
		}

		public static ISpecification<TCandidate> Or<TCandidate>(this ISpecification<TCandidate> specification,
			ISpecification<TCandidate> other)
		{
			return new OrSpecification<TCandidate>(specification, other);
		}

		public static ISpecification<TCandidate> Not<TCandidate>(this ISpecification<TCandidate> specification)
		{
			return new NotSpecification<TCandidate>(specification);
		}
	}
}
