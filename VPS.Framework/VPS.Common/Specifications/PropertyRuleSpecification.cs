using System;
using System.Reflection;

namespace VPS.Common.Specifications
{
	public class PropertyRuleSpecification<T, TPropertyType> : ISpecification<T>
	{
		private readonly string _propertyName;
		private readonly ISpecification<TPropertyType> _rule;
		private MethodInfo _getter;

		public PropertyRuleSpecification(string propertyName, ISpecification<TPropertyType> rule)
		{
			_propertyName = propertyName;
			_rule = rule;
			var propertyInfo = typeof(T).GetProperty(_propertyName);
			if (propertyInfo == null)
			{
				throw new ArgumentException(String.Format("Could not find property with name '{0}' on type '{1}'.", propertyName, typeof(T).Name));
			}
			_getter = propertyInfo.GetGetMethod();
		}

		public bool IsSatisfiedBy(T candidate)
		{
			var value = (TPropertyType)_getter.Invoke(candidate, new object[]{});

			return _rule.IsSatisfiedBy(value);
		}
	}
}