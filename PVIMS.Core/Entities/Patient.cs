using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using VPS.Common.Utilities;
using VPS.CustomAttributes;

namespace PVIMS.Core.Entities
{
    public class Patient : AuditedEntityBase, IExtendable
	{
		public Patient()
		{
			Attachments = new HashSet<Attachment>();
			Encounters = new HashSet<Encounter>();
			PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
			PatientConditions = new HashSet<PatientCondition>();
			PatientFacilities = new HashSet<PatientFacility>();
			PatientLabTests = new HashSet<PatientLabTest>();
			PatientLanguages = new HashSet<PatientLanguage>();
			PatientMedications = new HashSet<PatientMedication>();
			PatientStatusHistories = new HashSet<PatientStatusHistory>();
            Appointments = new HashSet<Appointment>();
            CohortEnrolments = new HashSet<CohortGroupEnrolment>();
		}

		[Column(TypeName = "date")]
		public DateTime? DateOfBirth { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Surname { get; set; }
        public string MiddleName { get; set; }

		public string Notes { get; set; }
		public Guid PatientGuid { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }

        public int? AuditUser_Id { get; set; }

        public virtual User AuditUser { get; set; }
		public virtual ICollection<Attachment> Attachments { get; set; }
		public virtual ICollection<Encounter> Encounters { get; set; }
		public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
		public virtual ICollection<PatientCondition> PatientConditions { get; set; }
		public virtual ICollection<PatientFacility> PatientFacilities { get; set; }
		public virtual ICollection<PatientLabTest> PatientLabTests { get; set; }
		public virtual ICollection<PatientLanguage> PatientLanguages { get; set; }
		public virtual ICollection<PatientMedication> PatientMedications { get; set; }
		public virtual ICollection<PatientStatusHistory> PatientStatusHistories { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<CohortGroupEnrolment> CohortEnrolments { get; set; }

        public int Age
        {
            get
            {
                if (DateOfBirth == null) {
                    return 0;
                }

                DateTime today = DateTime.Today;
                int age = today.Year - Convert.ToDateTime(DateOfBirth).Year;
                if (Convert.ToDateTime(DateOfBirth) > today.AddYears(-age)) age--;

                return age;
            }
        }

        public string AgeGroup
        {
            get
            {
                if (DateOfBirth == null)
                {
                    return "";
                }

                DateTime today = DateTime.Today;
                DateTime bday = Convert.ToDateTime(DateOfBirth);

                string ageGroup = "";
                if (today <= bday.AddMonths(1)) { ageGroup = "Neonate <= 1 month"; };
                if (today <= bday.AddMonths(48) && today > bday.AddMonths(1)) { ageGroup = "Infant > 1 month and <= 4 years"; };
                if (today <= bday.AddMonths(132) && today > bday.AddMonths(48)) { ageGroup = "Child > 4 years and <= 11 years"; };
                if (today <= bday.AddMonths(192) && today > bday.AddMonths(132)) { ageGroup = "Adolescent > 11 years and <= 16 years"; };
                if (today <= bday.AddMonths(828) && today > bday.AddMonths(192)) { ageGroup = "Adult > 16 years and <= 69 years"; };
                if (today > bday.AddMonths(828)) { ageGroup = "Elderly > 69 years"; };

                return ageGroup;
            }
        }

        public string FullName
        {
            get
            {
                return FirstName.Trim() + ' ' + Surname.Trim();
            }
        }

        public DateTime? LastEncounterDate()
        {
            if(Encounters.Count == 0) { 
                return null;
            }
            else {
                return Encounters.OrderByDescending(e => e.EncounterDate).FirstOrDefault().EncounterDate;
            }
        }

        public Encounter GetEncounterForAppointment(Appointment app)
        {
            if (Encounters.Count == 0 || app == null) {
                return null;
            }
            else {
                return Encounters.FirstOrDefault(e => e.EncounterDate >= app.AppointmentDate.AddDays(-3) && e.EncounterDate <= app.AppointmentDate.AddDays(3));
            }
        }

        public PatientFacility GetCurrentFacility()
        {
            if (PatientFacilities.Count == 0) {
                return null;
            }
            else {
                return PatientFacilities.OrderByDescending(f => f.EnrolledDate).ThenByDescending(f => f.Id).First();
            }
        }

        public Encounter GetCurrentEncounter()
        {
            if (Encounters.Count == 0) {
                return null;
            }
            else
            {
                return Encounters.OrderByDescending(e => e.EncounterDate).ThenByDescending(e => e.Id).First();
            }
        }

        public PatientStatusHistory GetCurrentStatus()
        {
            if (PatientStatusHistories.Count == 0)
            {
                return null;
            }
            else
            {
                return PatientStatusHistories.OrderByDescending(psh => psh.EffectiveDate).ThenByDescending(psh => psh.Id).First();
            }
        }

        public Encounter GetEncounterForToday()
        {
            if (Encounters.Count == 0) {
                return null;
            }
            else
            {
                return Encounters.SingleOrDefault(e => e.EncounterDate == DateTime.Today);
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

        public bool HasAppointment(int id, DateTime apptDate)
        {
            if (Appointments.Count == 0) {
                return false;
            }
            else 
            {
                if(id > 0) {
                    return Appointments.Any(a => a.AppointmentDate == apptDate && a.Id != id);
                }
                else {
                    return Appointments.Any(a => a.AppointmentDate == apptDate);
                }
            }
        }

        public CohortGroupEnrolment GetCohortEnrolled(CohortGroup cohort)
        {
            if (CohortEnrolments.Count == 0) {
                return null;
            }
            else {
                return CohortEnrolments.FirstOrDefault(ce => ce.CohortGroup.Id == cohort.Id && !ce.Archived );
            }
        }

        public bool HasCondition(List<Condition> conditions)
        {
            if (PatientConditions.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var pc in PatientConditions)
                {
                    foreach (var termcond in pc.TerminologyMedDra.ConditionMedDras)
                    {
                        // Go and check each condition this terminology is tied to
                        if (conditions.Contains(termcond.Condition)) {
                            return true;
                        }

                    }

                }
                return false;
            }
        }

        public PatientCondition GetConditionForGroupAndDate(Condition group, DateTime date)
        {
            if (PatientConditions.Count == 0)
            {
                return null;
            }
            else
            {
                return PatientConditions.Where(pc => pc.Archived == false 
                        && pc.DateStart <= date 
                        && pc.TerminologyMedDra.ConditionMedDras.Any(cm => cm.Condition.Id == group.Id))
                    .OrderByDescending(pc => pc.DateStart)
                    .FirstOrDefault();
            }
        }

        public bool HasClinicalData()
        {
            var hasData = false;

            if (PatientClinicalEvents.Count() > 0 || PatientConditions.Count() > 0 || PatientLabTests.Count() > 0 || PatientMedications.Count() > 0 || Encounters.Count() > 0) { hasData = true; };

            return hasData;
        }
    }
}