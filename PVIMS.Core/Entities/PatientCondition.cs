using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VPS.Common.Utilities;
using VPS.CustomAttributes;

namespace PVIMS.Core.Entities
{
    public class PatientCondition : EntityBase, IExtendable
    {
        public PatientCondition()
        {
            PatientConditionGuid = Guid.NewGuid();
        }

        public Guid PatientConditionGuid { get; set; }

        [Required]
        public DateTime DateStart { get; set; }
        public DateTime? OutcomeDate { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }

        public virtual TerminologyMedDra TerminologyMedDra { get; set; }
        public virtual Outcome Outcome { get; set; }
        public virtual TreatmentOutcome TreatmentOutcome { get; set; }

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