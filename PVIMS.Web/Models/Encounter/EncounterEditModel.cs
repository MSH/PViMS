using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class EncounterEditModel
    {
        public int EncounterId { get; set; }
        public string PatientFullName { get; set; }
        public string EncounterType { get; set; }
        public DateTime EncounterDate { get; set; }
        public string EncounterNotes { get; set; }
        public DatasetCategoryEditModel[] DatasetCategories { get; set; }
        public int DatasetInstanceId { get; set; }
    }
}