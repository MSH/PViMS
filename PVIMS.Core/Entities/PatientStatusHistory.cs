using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class PatientStatusHistory : AuditedEntityBase
	{
		public DateTime EffectiveDate { get; set; }

		[StringLength(100)]
        public string Comments { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }
        public int? AuditUser_Id { get; set; }

		public virtual Patient Patient { get; set; }
		public virtual PatientStatus PatientStatus { get; set; }
        public virtual User AuditUser { get; set; }

	}
}