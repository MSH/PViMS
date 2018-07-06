using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class Attachment : AuditedEntityBase
	{
		[Required]
		public byte[] Content { get; set; }

		[StringLength(100)]
		public string Description { get; set; }
		
		[Required]
		[StringLength(50)]
		public string FileName { get; set; }

		public long Size { get; set; }

        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }

		public virtual AttachmentType AttachmentType { get; set; }
		public virtual Encounter Encounter { get; set; }
		public virtual Patient Patient { get; set; }
        public virtual ActivityExecutionStatusEvent ActivityExecutionStatusEvent { get; set; }

        public virtual User AuditUser { get; set; }
        public int? AuditUser_Id { get; set; }

    }
}