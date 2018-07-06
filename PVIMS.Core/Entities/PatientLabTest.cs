using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VPS.Common.Utilities;
using VPS.CustomAttributes;

namespace PVIMS.Core.Entities
{
    public class PatientLabTest : EntityBase, IExtendable
	{
        public PatientLabTest()
        {
            PatientLabTestGuid = Guid.NewGuid();
        }

        public Guid PatientLabTestGuid { get; set; }

		public DateTime TestDate { get; set; }

		[StringLength(50)]
		public string TestResult { get; set; }
        [StringLength(20)]
        public string LabValue { get; set; }

        [StringLength(20)]
        public string ReferenceLower { get; set; }
        [StringLength(20)]
        public string ReferenceUpper { get; set; }

		public virtual LabTestUnit TestUnit { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }

        [Required]
		public virtual LabTest LabTest { get; set; }
        [Required]
		public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }

        private CustomAttributeSet customAttributes = new CustomAttributeSet();

        CustomAttributeSet IExtendable.CustomAttributes
        {
            get { return customAttributes; }
        }

        [Column(TypeName = "xml")]
        public string CustomAttributesXmlSerialised
        {

            get { return SerialisationHelper.SerialiseToXmlString(customAttributes); }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    customAttributes = new CustomAttributeSet();
                }
                else
                {
                    customAttributes = SerialisationHelper.DeserialiseFromXmlString<CustomAttributeSet>(value);
                }
            }
        }

        void IExtendable.SetAttributeValue<T>(string attributeKey, T attributeValue, string updatedByUser)
        {
            customAttributes.SetAttributeValue(attributeKey, attributeValue, updatedByUser);
        }

        object IExtendable.GetAttributeValue(string attributeKey)
        {
            return customAttributes.GetAttributeValue(attributeKey);
        }

        public void ValidateAndSetAttributeValue<T>(VPS.CustomAttributes.CustomAttributeConfiguration attributeConfig, T attributeValue, string updatedByUser)
        {
            customAttributes.ValidateAndSetAttributeValue(attributeConfig, attributeValue, updatedByUser);
        }

        public DateTime GetUpdatedDate(string attributeKey)
        {
            return customAttributes.GetUpdatedDate(attributeKey);
        }

        public string GetUpdatedByUser(string attributeKey)
        {
            return customAttributes.GetUpdatedByUser(attributeKey);
        }

    }
}