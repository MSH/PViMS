using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using VPS.Common.Utilities;
using VPS.CustomAttributes;

namespace PVIMS.Core.Entities
{
	public class PatientClinicalEvent : EntityBase, IExtendable
	{
        public PatientClinicalEvent()
        {
            PatientClinicalEventGuid = Guid.NewGuid();
        }

        public Guid PatientClinicalEventGuid { get; set; }

        [StringLength(500)]
        public string SourceDescription { get; set; }

        public virtual TerminologyMedDra SourceTerminologyMedDra { get; set; }

		[Column(TypeName = "date")]
		public DateTime? OnsetDate { get; set; }

		[Column(TypeName = "date")]
		public DateTime? ResolutionDate { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }

        [Required]
        public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }

        private CustomAttributeSet customAttributes = new CustomAttributeSet();

        public string AgeGroup
        {
            get
            {
                if (OnsetDate == null)
                {
                    return "";
                }
                if (Patient.DateOfBirth == null)
                {
                    return "";
                }

                DateTime onset = Convert.ToDateTime(OnsetDate);
                DateTime bday = Convert.ToDateTime(Patient.DateOfBirth);

                string ageGroup = "";
                if (onset <= bday.AddMonths(1)) { ageGroup = "Neonate <= 1 month"; };
                if (onset <= bday.AddMonths(48) && onset > bday.AddMonths(1)) { ageGroup = "Infant > 1 month and <= 4 years"; };
                if (onset <= bday.AddMonths(132) && onset > bday.AddMonths(48)) { ageGroup = "Child > 4 years and <= 11 years"; };
                if (onset <= bday.AddMonths(192) && onset > bday.AddMonths(132)) { ageGroup = "Adolescent > 11 years and <= 16 years"; };
                if (onset <= bday.AddMonths(828) && onset > bday.AddMonths(192)) { ageGroup = "Adult > 16 years and <= 69 years"; };
                if (onset > bday.AddMonths(828)) { ageGroup = "Elderly > 69 years"; };

                return ageGroup;
            }
        }

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