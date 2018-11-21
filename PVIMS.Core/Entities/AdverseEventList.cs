using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class AdverseEventList : Entity<int>
    {
        public string Description { get; set; }
        public string Criteria { get; set; }
        public string Serious { get; set; }
        public int PatientCount { get; set; }
    }
}
