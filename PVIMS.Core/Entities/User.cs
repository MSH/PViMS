using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Linq;

using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
	public class User : Entity<int>
	{
        public User()
		{
			Facilities = new HashSet<UserFacility>();
            CohortGroupEnrolments = new HashSet<CohortGroupEnrolment>();
            Appointments = new HashSet<Appointment>();
            Attachments = new HashSet<Attachment>();
            Encounters = new HashSet<Encounter>();
            PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
            PatientConditions = new HashSet<PatientCondition>();
            PatientFacilities = new HashSet<PatientFacility>();
            PatientLabTests = new HashSet<PatientLabTest>();
            PatientMedications = new HashSet<PatientMedication>();
            PatientStatusHistories = new HashSet<PatientStatusHistory>();
            Patients = new HashSet<Patient>();
		}

		[StringLength(256)]
		public string Email { get; set; }

		public bool EmailConfirmed { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public string PhoneNumber { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public bool TwoFactorEnabled { get; set; }

		[DataType("datetime")]
		public DateTime? LockoutEndDateUtc { get; set; }

		public bool LockoutEnabled { get; set; }
		public int AccessFailedCount { get; set; }

		[Required]
		[StringLength(256)]
		public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName 
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        [DefaultValue(true)]
        public bool Active { get; set; }
        [DefaultValue(false)]
        public bool AllowDatasetDownload { get; set; }

        [StringLength(20)]
        public string CurrentContext { get; set; }

        [DataType("datetime")]
        public DateTime? EulaAcceptanceDate { get; set; }

        public virtual ICollection<UserFacility> Facilities { get; set; }
        public virtual ICollection<CohortGroupEnrolment> CohortGroupEnrolments { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Encounter> Encounters { get; set; }
        public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
        public virtual ICollection<PatientCondition> PatientConditions { get; set; }
        public virtual ICollection<PatientFacility> PatientFacilities { get; set; }
        public virtual ICollection<PatientLabTest> PatientLabTests { get; set; }
        public virtual ICollection<PatientMedication> PatientMedications { get; set; }
        public virtual ICollection<PatientStatusHistory> PatientStatusHistories { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }

        public bool HasFacility(int id)
        {
            return Facilities.Any(uf => uf.Facility.Id == id);
        }

    }
}