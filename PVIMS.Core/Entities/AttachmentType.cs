namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AttachmentType : EntityBase
    {
        public AttachmentType()
        {
            Attachments = new HashSet<Attachment>();
        }


        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        [StringLength(4)]
        public string Key { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
