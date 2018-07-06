using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class PatientFacility : EntityBase
	{
		public DateTime EnrolledDate { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }

		public virtual Facility Facility { get; set; }
		public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }
	}
}