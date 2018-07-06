using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class Encounter : AuditedEntityBase, IHasCustomAttributes
	{
        protected Encounter()
        { 
        }

		public Encounter(Patient patient)
		{
			Attachments = new HashSet<Attachment>();
			PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
            EncounterGuid = Guid.NewGuid();
            Patient = patient;
		}

		public DateTime EncounterDate { get; set; }
		public string Notes { get; set; }
		public Guid EncounterGuid { get; set; }
		public bool Discharged { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }
        public string CustomAttributesXmlSerialised { get; set; }
        public int? AuditUser_Id { get; set; }

		public virtual ICollection<Attachment> Attachments { get; set; }
		public virtual EncounterType EncounterType { get; set; }
		public virtual Patient Patient { get; set; }
		public virtual Pregnancy Pregnancy { get; set; }
		public virtual Priority Priority { get; set; }
		public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
        public virtual User AuditUser { get; set; }
	}
}