using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class EncounterListItem
    {
        [Key]
        public int EncounterId { get; set; }
        [Display(Name = "Patient")]
        public string PatientFullName { get; set; }
        [Display(Name = "Encounter Type")]
        public string EncounterType { get; set; }
        [Display(Name = "Encounter Date")]
        [DisplayFormat(DataFormatString="{0:dd/MM/yyyy}")]
        public DateTime EncounterDate { get; set; }
    }
}