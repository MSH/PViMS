using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public class SiteContactDetail : AuditedEntityBase
    {
        [Required]
        public virtual ContactType ContactType { get; set; }

        [Required]
        [StringLength(50)]
        public string OrganisationName { get; set; }

        [Required]
        [StringLength(30)]
        public string ContactFirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string ContactSurname { get; set; }

        [Required]
        [StringLength(100)]
        public string StreetAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(20)]
        public string PostCode { get; set; }

        [StringLength(50)]
        public string ContactNumber { get; set; }

        [StringLength(50)]
        public string ContactEmail { get; set; }

        [StringLength(10)]
        public string CountryCode { get; set; }

    }
}