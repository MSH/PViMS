using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Specifications
{
	public abstract class ComparableSpecification<T> : ISpecification<T>
	{
		protected IComparable TryConvertToComparable(T candidate)
		{
			var comparable = candidate as IComparable;
			if (comparable == null)
			{
				throw new InvalidOperationException(
					"Cannot compare candidate because the type provided does not implement IComparable.");
			}

			return comparable;
		}

		public abstract bool IsSatisfiedBy(T candidate);
	}
}
