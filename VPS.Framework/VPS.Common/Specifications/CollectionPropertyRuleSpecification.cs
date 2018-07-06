using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VPS.Common.Specifications
{
	// TODO: Design note: this code looks very similar to PropertyRuleSpecification - maybe find a way to merge them.
	public class CollectionPropertyRuleSpecification<T, TPropertyType> : ISpecification<T>
	{
		private readonly AggregationOperator _aggregationOperator;
		private readonly ISpecification<int> _aggregationRule;
		private readonly ISpecification<TPropertyType> _filterRule;
		private readonly MethodInfo _getter;
		private readonly string _propertyName;

		public CollectionPropertyRuleSpecification(string propertyName, ISpecification<TPropertyType> filterRule,
			AggregationOperator aggregationOperator
			, ISpecification<int> aggregationRule)
		{
			_propertyName = propertyName;
			_filterRule = filterRule;
			_aggregationOperator = aggregationOperator;
			_aggregationRule = aggregationRule;
			var propertyInfo = typeof (T).GetProperty(_propertyName);
			if (propertyInfo == null)
			{
				throw new ArgumentException(String.Format("Could not find property with name '{0}' on type '{1}'.", propertyName,
					typeof (T).Name));
			}
			if (!typeof(IEnumerable<TPropertyType>).IsAssignableFrom(propertyInfo.PropertyType))
			{
				throw new ArgumentException(
					string.Format(
						"Property with name '{0}' is not a valid collection type. It should be assignable from IEnumerable<'{1}'",
						propertyName
						, typeof (TPropertyType).Name));
			}
			_getter = propertyInfo.GetGetMethod();
		}

		public bool IsSatisfiedBy(T candidate)
		{
			var value = (IEnumerable<TPropertyType>) _getter.Invoke(candidate, new object[] {});

			if (_aggregationOperator == AggregationOperator.Count)
			{
				var count = value.Count(item => _filterRule.IsSatisfiedBy(item));
				return _aggregationRule.IsSatisfiedBy(count);
			}

			throw new NotSupportedException("Unknown aggregation operation.");
		}
	}
}