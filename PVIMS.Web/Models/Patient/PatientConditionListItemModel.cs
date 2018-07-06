using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientConditionListItemModel
    {
        public int PatientConditionId { get; set; }
        public string TerminologyMedDRA { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateStart { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateEnd { get; set; }
        public string Outcome { get; set; }
    }
}