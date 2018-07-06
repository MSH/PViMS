using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PVIMS.Core.Exceptions;
using PVIMS.Core.ValueTypes;
using PVIMS.Core.Extensions;

namespace PVIMS.Core.Entities
{
	public class DatasetElementSub : EntityBase
	{
        public DatasetElementSub()
		{
		}

		[Required]
		[StringLength(100)]
		public string ElementName { get; set; }

        [StringLength(150)]
        public string FriendlyName { get; set; }
        [StringLength(350)]
        public string Help { get; set; }

		public short FieldOrder { get; set; }
		public virtual DatasetElement DatasetElement { get; set; }
		public virtual Field Field { get; set; }

        [StringLength(50)]
        public string OID { get; set; }
        public string DefaultValue { get; set; }
        public bool System { get; set; }

        public void Validate(string instanceSubValue)
        {
            if (string.IsNullOrWhiteSpace(instanceSubValue))
            {
                if (Field.Mandatory)
                    throw new DatasetFieldSetException(ElementName, string.Format("{0} is required.", ElementName));
                else
                    return;
            }

            var fieldType = (FieldTypes)Field.FieldType.Id;

            switch (fieldType)
            {
                case FieldTypes.AlphaNumericTextbox:
                    if (Field.MaxLength.HasValue)
                    {
                        if (instanceSubValue != null && instanceSubValue.Length > Field.MaxLength.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not contain more than {1} characters.", ElementName, Field.MaxLength.Value));
                    }
                    break;
                case FieldTypes.NumericTextbox:
                    if (!instanceSubValue.IsNumeric())
                    {
                        throw new DatasetFieldSetException(ElementName, string.Format("{0} must be a numeric value.", ElementName));
                    }

                    var decimalValue = decimal.Parse(instanceSubValue);


                    if (Field.MinSize.HasValue)
                    {
                        if (decimalValue < Field.MinSize.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not be less than {1}.", ElementName, Field.MinSize.Value));
                    }

                    if (Field.MaxSize.HasValue)
                    {
                        if (decimalValue > Field.MaxSize.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not be more than {1}.", ElementName, Field.MaxSize.Value));
                    }
                    break;
                case FieldTypes.Date:
                    break;
                default:
                    break;
            }
        }
    }
}