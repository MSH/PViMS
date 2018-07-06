using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class ContactDetailEditModel
    {
        [Key]
        public int ContactDetailId { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Contact Type")]
        public string ContactType { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Organisation Name")]
        public string OrganisationName { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Contact First Name")]
        public string ContactFirstName { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Contact Last Name")]
        public string ContactSurname { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        [Required]
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(50)]
        public string State { get; set; }
        [StringLength(20)]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }
        [StringLength(50)]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
        [StringLength(50)]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }
        [StringLength(10)]
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }
    }
}