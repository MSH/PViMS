using System;
using Newtonsoft.Json.Linq;

namespace VPS.Common.Specifications
{
	public class RuleBuilder
	{
		public static ISpecification<T> FromJson<T>(string json)
		{
			if (String.IsNullOrWhiteSpace(json))
			{
				return null;
			}
			var jObject = JObject.Parse(json);
			var result = Build<T>(jObject);
			return result;
		}

		public static ISpecification<T> Build<T>(JToken jToken)
		{
			if (jToken["type"].Value<string>() == "Equals")
			{
				var type = typeof (T);
				if (type.IsEnum)
				{
					var value = (T) Enum.Parse(type, jToken["data"]["value"].Value<string>());
					return new EqualsSpecification<T>(value);
				}
				return new EqualsSpecification<T>(jToken["data"]["value"].Value<T>());
			}

			if (jToken["type"].Value<string>() == "GreaterThan")
			{
				return new GreaterThanSpecification<T>(jToken["data"]["value"].Value<T>());
			}

			if (jToken["type"].Value<string>() == "LessThan")
			{
				return new LessThanSpecification<T>(jToken["data"]["value"].Value<T>());
			}

			if (jToken["type"].Value<string>() == "Or")
			{
				return new OrSpecification<T>(Build<T>(jToken["data"]["left"]), Build<T>(jToken["data"]["right"]));
			}

			if (jToken["type"].Value<string>() == "And")
			{
				return new AndSpecification<T>(Build<T>(jToken["data"]["left"]), Build<T>(jToken["data"]["right"]));
			}

			if (jToken["type"].Value<string>() == "Not")
			{
				return new NotSpecification<T>(Build<T>(jToken["data"]["rule"]));
			}

			if (jToken["type"].Value<string>() == "Property")
			{
				var systemType = Type.GetType(jToken["data"]["systemType"].Value<string>());

				if (systemType == null)
				{
					throw new InvalidRuleSpecificationException(
						String.Format("Type '{0}' could not be loaded. Check that the string provided is a valid .Net type."
							, jToken["data"]["systemType"].Value<string>()));
				}

				return BuildPropertyRule<T>(jToken["data"]["name"].Value<string>(), systemType, jToken["data"]["rule"]);
			}

			if (jToken["type"].Value<string>() == "Count")
			{
				var itemSystemType = Type.GetType(jToken["data"]["itemSystemType"].Value<string>());

				if (itemSystemType == null)
				{
					throw new InvalidRuleSpecificationException(
						String.Format("Type '{0}' could not be loaded. Check that the string provided is a valid .Net type."
							, jToken["data"]["itemSystemType"].Value<string>()));
				}

				var @operator =
					(AggregationOperator)
						Enum.Parse(typeof (AggregationOperator), jToken["data"]["aggregationOperator"].Value<string>());
				return BuildCollectionPropertyRule<T>(jToken["data"]["name"].Value<string>(), itemSystemType, jToken["data"]["filterRule"],
					@operator,
					Build<int>(jToken["data"]["aggregationRule"]));
			}

			throw new NotSupportedException(
				"Could not determine the specification type. It is possibly not a valid rule specification or missing the 'type' field. Provided rule was:" +
				jToken);
		}

		private static ISpecification<T> BuildPropertyRule<T>(string propertyName, Type systemType, JToken ruleToken)
		{
			var rule = CallBuildViaRefection<T>(systemType, ruleToken);

			return CallPropertyRuleConstructor<T>(propertyName, systemType, rule);
		}

		private static ISpecification<T> BuildCollectionPropertyRule<T>(string propertyName, Type systemType,
			JToken filterRuleToken, AggregationOperator aggregationOperator,
			ISpecification<int> aggregationRule)
		{
			var rule = CallBuildViaRefection<T>(systemType, filterRuleToken);

			return CallCollectionPropertyRuleConstructor<T>(propertyName, systemType, rule, aggregationOperator, aggregationRule);
		}

		private static object CallBuildViaRefection<T>(Type systemType, JToken ruleToken)
		{
			var method = typeof (RuleBuilder).GetMethod("Build");

			var genericMethod = method.MakeGenericMethod(systemType);

			var rule = genericMethod.Invoke(null, new object[] {ruleToken});

			return rule;
		}

		private static ISpecification<T> CallPropertyRuleConstructor<T>(string propertyName, Type systemType, object rule)
		{
			var definition = typeof (PropertyRuleSpecification<,>);
			var genericDefinition = definition.MakeGenericType(typeof (T), systemType);

			var constructor = genericDefinition.GetConstructors()[0];
			return (ISpecification<T>) constructor.Invoke(new[] {propertyName, rule});
		}

		private static ISpecification<T> CallCollectionPropertyRuleConstructor<T>(string propertyName, Type systemType,
			object filterRule, AggregationOperator @operator, ISpecification<int> aggregationRule)
		{
			var definition = typeof (CollectionPropertyRuleSpecification<,>);
			var genericDefinition = definition.MakeGenericType(typeof (T), systemType);

			var constructor = genericDefinition.GetConstructors()[0];
			return (ISpecification<T>) constructor.Invoke(new[] {propertyName, filterRule, @operator, aggregationRule});
		}
	}
}