using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using PVIMS.Core.Models;

namespace PVIMS.Web.Models
{
    public class EncounterViewModel
    {
        [Key]
        public int EncounterId { get; set; }
        public int PatientId { get; set; }
        public int DatasetInstanceId { get; set; }
        [Display(Name = "Patient")]
        public string PatientFullName { get; set; }
        [Display(Name = "Encounter Type")]
        public string EncounterType { get; set; }
        [Display(Name = "Encounter Date")]
        public string EncounterDate { get; set; }
        [Display(Name = "Notes")]
        public string EncounterNotes { get; set; }

        public DatasetInstanceValueList InstanceValue { get; set; }

        public DatasetCategoryViewModel[] DatasetCategories { get; set; }
        public PatientConditionListItemModel[] PatientConditions { get; set; }
        public PatientClinicalEventListItemModel[] PatientClinicalEvents { get; set; }
        public PatientMedicationListItemModel[] PatientMedications { get; set; }
        public PatientLabTestListItemModel[] PatientLabTests { get; set; }
        public ConditionGroupListItemModel[] ConditionGroups { get; set; }
    }
}