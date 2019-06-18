using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class PatientOnStudyList : Entity<int>
    {
        public string FacilityName { get; set; }
        public int FacilityId { get; set; }
        public int PatientCount { get; set; }
        public int PatientWithNonSeriousEventCount { get; set; }
        public int PatientWithSeriousEventCount { get; set; }
        public int PatientWithEventCount { get; set; }
    }
}
