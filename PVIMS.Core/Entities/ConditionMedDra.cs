using System;
using System.ComponentModel;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class ConditionMedDra : EntityBase
	{
        [Required]
        public virtual Condition Condition { get; set; }
        [Required]
        public virtual TerminologyMedDra TerminologyMedDra { get; set; }

        public PatientCondition GetConditionForPatient(Patient patient)
        {
            PatientCondition tempCondition = patient.PatientConditions.OrderByDescending(pc => pc.DateStart).Where(pc => TerminologyMedDra.Id == pc.TerminologyMedDra.Id && pc.OutcomeDate == null).FirstOrDefault();
            return tempCondition;
        }

        public PatientCondition GetConditionForEncounter(Encounter encounter)
        {
            PatientCondition tempCondition = encounter.Patient.PatientConditions.OrderByDescending(pc => pc.DateStart).Where(pc => TerminologyMedDra.Id == pc.TerminologyMedDra.Id && ((pc.OutcomeDate == null && pc.DateStart <= encounter.EncounterDate) || (pc.OutcomeDate >= encounter.EncounterDate && pc.DateStart <= encounter.EncounterDate))).FirstOrDefault();
            return tempCondition;
        }

	}
}