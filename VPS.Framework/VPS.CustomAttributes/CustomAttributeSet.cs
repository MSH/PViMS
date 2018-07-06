using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VPS.CustomAttributes
{
    public class CustomAttributeSet
    {
        private readonly Type[] allowedTypes = new[] { typeof(int), typeof(string), typeof(DateTime), typeof(decimal) };

        public CustomAttributeSet()
        {
            CustomSelectionAttributes = new List<CustomSelectionAttribute>();
            CustomStringAttributes = new List<CustomStringAttribute>();
            CustomDateTimeAttributes = new List<CustomDateTimeAttribute>();
            CustomNumericAttributes = new List<CustomNumericAttribute>();
        }

        [XmlElement(ElementName = "CustomSelectionAttribute")]
        public List<CustomSelectionAttribute> CustomSelectionAttributes { get; set; }
        [XmlElement(ElementName = "CustomStringAttribute")]
        public List<CustomStringAttribute> CustomStringAttributes { get; set; }
        [XmlElement(ElementName = "CustomDateTimeAttribute")]
        public List<CustomDateTimeAttribute> CustomDateTimeAttributes { get; set; }
        [XmlElement(ElementName = "CustomNumericAttribute")]
        public List<CustomNumericAttribute> CustomNumericAttributes { get; set; }

        public void ValidateAndSetAttributeValue<T>(CustomAttributeConfiguration attributeConfig, T attributeValue, string updatedByUser)
        {
            if (attributeConfig.IsRequired && attributeValue == null)
            {
                throw new CustomAttributeValidationException(string.Format("{0} is required", attributeConfig.AttributeKey));
            }

            if (attributeValue.GetType() == typeof(string) && attributeConfig.StringMaxLength.HasValue)
            {
                if (attributeValue.ToString().Length > attributeConfig.StringMaxLength.Value)
                    throw new CustomAttributeValidationException("Length of {0} may not exceed {1} characters", attributeConfig.AttributeKey, attributeConfig.StringMaxLength.Value);
            }

            if (attributeValue.GetType() == typeof(decimal) && (attributeConfig.NumericMinValue.HasValue || attributeConfig.NumericMaxValue.HasValue))
            {
                var decimalValue = Convert.ToDecimal(attributeValue);

                if (attributeConfig.NumericMinValue.HasValue && decimalValue < attributeConfig.NumericMinValue.Value)
                {
                    throw new CustomAttributeValidationException("Value of {0} may not be lower than {1}", attributeConfig.AttributeKey, attributeConfig.NumericMinValue.Value);
                }

                if (attributeConfig.NumericMaxValue.HasValue && decimalValue > attributeConfig.NumericMaxValue.Value)
                {
                    throw new CustomAttributeValidationException("Value of {0} may not be higher than {1}", attributeConfig.AttributeKey, attributeConfig.NumericMaxValue.Value);
                }
            }

            if (attributeValue.GetType() == typeof(DateTime) && (attributeConfig.PastDateOnly || attributeConfig.FutureDateOnly))
            {
                var dateTimeValue = Convert.ToDateTime(attributeValue);

                if (attributeConfig.PastDateOnly && dateTimeValue.Date > DateTime.Now.Date)
                {
                    throw new CustomAttributeValidationException("{0} may only be in the past", attributeConfig.AttributeKey);
                }

                if (attributeConfig.FutureDateOnly && dateTimeValue.Date < DateTime.Now.Date)
                {
                    throw new CustomAttributeValidationException("{0} may only be in the future", attributeConfig.AttributeKey);
                }
            }

            SetAttributeValue(attributeConfig.AttributeKey, attributeValue, updatedByUser);
        }

        public void SetAttributeValue<T>(string attributeKey, T attributeValue, string updatedByUser)
        {
            if (!allowedTypes.Contains(typeof(T)))
                throw new CustomAttributeException("Custom attribute of Type {0} is not supported.", typeof(T).Name);

            var existingType = GetCurrentAttributeTypeIfExists(attributeKey);

            switch (typeof(T).Name)
            {
                case "Int32":
                    var intValue = Convert.ToInt32(attributeValue);

                    var attribute = CustomSelectionAttributes.SingleOrDefault(a => a.Key == attributeKey);

                    if (attribute != null)
                    {
                        attribute.SetAttributeValue(intValue, updatedByUser);
                    }
                    else
                    {
                        if (existingType != CustomAttributeType.None) throw new CustomAttributeException("Custom Attribute value for {0} is of an incorrect type. Please cast the value before setting.", attributeKey);

                        CustomSelectionAttributes.Add(new CustomSelectionAttribute { Key = attributeKey, Value = intValue, UpdatedByUser = updatedByUser });
                    }
                    break;
                case "String":
                    var stringValue = attributeValue.ToString();

                    var stringAttribute = CustomStringAttributes.SingleOrDefault(a => a.Key == attributeKey);

                    if (stringAttribute != null)
                        stringAttribute.SetAttributeValue(stringValue, updatedByUser);
                    else
                    {
                        if (existingType != CustomAttributeType.None) throw new CustomAttributeException("Custom Attribute value for {0} is of an incorrect type. Please cast the value before setting.", attributeKey);

                        CustomStringAttributes.Add(new CustomStringAttribute { Key = attributeKey, Value = stringValue, UpdatedByUser = updatedByUser });
                    }
                    break;
                case "DateTime":
                    var dateTimeValue = Convert.ToDateTime(attributeValue);

                    var dateTimeAttribute = CustomDateTimeAttributes.SingleOrDefault(a => a.Key == attributeKey);

                    if (dateTimeAttribute != null)
                        dateTimeAttribute.SetAttributeValue(dateTimeValue, updatedByUser);
                    else
                    {
                        if (existingType != CustomAttributeType.None) throw new CustomAttributeException("Custom Attribute value for {0} is of an incorrect type. Please cast the value before setting.", attributeKey);

                        CustomDateTimeAttributes.Add(new CustomDateTimeAttribute { Key = attributeKey, Value = dateTimeValue, UpdatedByUser = updatedByUser });
                    }
                    break;
                case "Decimal":
                    var decimalValue = Convert.ToDecimal(attributeValue);

                    var decimalAttribute = CustomNumericAttributes.SingleOrDefault(a => a.Key == attributeKey);

                    if (decimalAttribute != null)
                        decimalAttribute.SetAttributeValue(decimalValue, updatedByUser);
                    else
                    {
                        if (existingType != CustomAttributeType.None) throw new CustomAttributeException("Custom Attribute value for {0} is of an incorrect type. Please cast the value before setting.", attributeKey);
                        CustomNumericAttributes.Add(new CustomNumericAttribute { Key = attributeKey, Value = decimalValue, UpdatedByUser = updatedByUser });
                    }
                    break;
                default:
                    // This should never happen
                    throw new CustomAttributeException("Unable to set custom attribute; Key: {0}, Value: {1}, Type: {2}", attributeKey, attributeValue, typeof(T).Name);
            }
        }

        private CustomAttributeType GetCurrentAttributeTypeIfExists(string attributeKey)
        {
            if (CustomDateTimeAttributes.Any(c => c.Key == attributeKey)) return CustomAttributeType.DateTime;
            if (CustomStringAttributes.Any(c => c.Key == attributeKey)) return CustomAttributeType.String;
            if (CustomSelectionAttributes.Any(c => c.Key == attributeKey)) return CustomAttributeType.Selection;
            if (CustomNumericAttributes.Any(c => c.Key == attributeKey)) return CustomAttributeType.Numeric;

            return CustomAttributeType.None;
        }

        public object GetAttributeValue(string attributeKey)
        {
            var selectionAttribute = CustomSelectionAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (selectionAttribute != null) return selectionAttribute.Value;

            var stringAttribute = CustomStringAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (stringAttribute != null) return stringAttribute.Value;

            var dateTimeAttribute = CustomDateTimeAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (dateTimeAttribute != null) return dateTimeAttribute.Value;

            var numericAttribute = CustomNumericAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (numericAttribute != null) return numericAttribute.Value;

            return null;
        }

        public DateTime GetUpdatedDate(string attributeKey)
        {
            var selectionAttribute = CustomSelectionAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (selectionAttribute != null) return selectionAttribute.UpdatedDate;

            var stringAttribute = CustomStringAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (stringAttribute != null) return stringAttribute.UpdatedDate;

            var dateTimeAttribute = CustomDateTimeAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (dateTimeAttribute != null) return dateTimeAttribute.UpdatedDate;

            var numericAttribute = CustomNumericAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (numericAttribute != null) return numericAttribute.UpdatedDate;

            return default(DateTime);
        }

        public string GetUpdatedByUser(string attributeKey)
        {
            var selectionAttribute = CustomSelectionAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (selectionAttribute != null) return selectionAttribute.UpdatedByUser;

            var stringAttribute = CustomStringAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (stringAttribute != null) return stringAttribute.UpdatedByUser;

            var dateTimeAttribute = CustomDateTimeAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (dateTimeAttribute != null) return dateTimeAttribute.UpdatedByUser;

            var numericAttribute = CustomNumericAttributes.SingleOrDefault(s => s.Key == attributeKey);
            if (numericAttribute != null) return numericAttribute.UpdatedByUser;

            return string.Empty;
        }
    }
}
