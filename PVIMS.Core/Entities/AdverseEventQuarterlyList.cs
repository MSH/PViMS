using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class AdverseEventQuarterlyList : Entity<int>
    {
        public int? PeriodYear { get; set; }
        public int? PeriodQuarter { get; set; }
        public string FacilityName { get; set; }
        public string MedDraTerm { get; set; }
        public string SeverityGrade { get; set; }
        public int? PatientCount { get; set; }
    }
}
