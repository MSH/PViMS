using System.ComponentModel;

namespace PVIMS.Core.Entities
{
    public enum PatientOnStudyCriteria
    {
        [Description("Has Encounter in Date Range")]
        HasEncounterinDateRange = 1,
        [Description("Patient Registered in Facility in Date Range")]
        PatientRegisteredinFacilityinDateRange = 2
    }
}
