using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class EncounterAddModel
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }

        [Display(Name = "Notes")]
        public string EncounterNotes { get; set; }
        [Display(Name = "Encounter Type")]
        public int EncounterTypeId { get; set; }
        [Display(Name = "Priority")]
        public int PriorityId { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Encounter Date")]
        public DateTime EncounterDate { get; set; }
    }
}