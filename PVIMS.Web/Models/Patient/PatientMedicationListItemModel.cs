using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientMedicationListItemModel
    {
        public int PatientMedicationId { get; set; }
        public string DrugName { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EndDate { get; set; }
        public string Dose { get; set; }
        public string DoseFrequency { get; set; }
        public string DoseUnit { get; set; }
        public string IndicationType { get; set; }
    }
}