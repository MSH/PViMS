using System;
using Newtonsoft.Json.Linq;

namespace VPS.Common.Specifications
{
	public class CaseBuilder
	{
		public static CaseSpecification<TCandidate, TValue> FromJson<TCandidate, TValue>(string json)
		{
			if (String.IsNullOrWhiteSpace(json))
			{
				return null;
			}
			var jObject = JObject.Parse(json);
			var result = Build<TCandidate, TValue>(jObject);
			return result;
		}

		public static CaseSpecification<TCandidate, TValue> Build<TCandidate, TValue>(JToken jToken)
		{
			//var valueType = Type.GetType(jToken["valueType"].Value<string>());

			//if (valueType == null)
			//{
			//	throw new InvalidRuleSpecificationException(
			//		String.Format("Type '{0}' could not be loaded. Check that the string provided is a valid .Net type."
			//			, jToken["systemType"].Value<string>()));
			//}

			var defaultValue = Parse<TValue>(jToken["defaultValue"]);

			var rule = new CaseSpecification<TCandidate, TValue>(defaultValue);

			var casesTokens = jToken["cases"];
			foreach (var caseToken in casesTokens.Children())
			{
				var value = Parse<TValue>(caseToken["value"]);
				var specification = RuleBuilder.Build<TCandidate>(caseToken["rule"]);

				var item = new CaseSpecification<TCandidate, TValue>.CaseSpecificationItem(specification, value);
				rule.Items.Add(item);
			}

			return rule;
		}

		private static T Parse<T>(JToken token)
		{
			var type = typeof(T);
			if (type.IsEnum)
			{
				return (T)Enum.Parse(type, token.Value<string>());
			}
			return token.Value<T>();
		}
	}
}
