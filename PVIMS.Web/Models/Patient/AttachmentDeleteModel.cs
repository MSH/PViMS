using System;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class AttachmentDeleteModel
    {
        [Key]
        public int AttachmentId { get; set; }
        public string PatientFullName { get; set; }

        public string FileName { get; set; }
        public string Description { get; set; }

        [Required]
        [Display(Name = "Reason")]
        [StringLength(200, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string ArchiveReason { get; set; }
    }
}