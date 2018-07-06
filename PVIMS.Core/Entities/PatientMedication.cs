using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VPS.Common.Utilities;
using VPS.CustomAttributes;

namespace PVIMS.Core.Entities
{
    public class PatientMedication : EntityBase, IExtendable
	{
        public PatientMedication()
        {
            PatientMedicationGuid = Guid.NewGuid();
        }

        public Guid PatientMedicationGuid { get; set; }

		public DateTime DateStart { get; set; }
		public DateTime? DateEnd { get; set; }

		[StringLength(30)]
		public string Dose { get; set; }

		[StringLength(30)]
		public string DoseFrequency { get; set; }

		[StringLength(10)]
		public string DoseUnit { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }

		public virtual Medication Medication { get; set; }
		public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }

        public string DisplayName
        {
            get
            {
                return String.Format("{0}; {1} {2}", Medication.FullName, Dose, DoseUnit);
            }
        }

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