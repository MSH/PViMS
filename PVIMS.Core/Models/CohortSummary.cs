namespace PVIMS.Core.Models
{
    public class CohortSummary
    {
        public CohortSummary()
        {
            PatientCount = 0;
            SeriesEventCount = 0;
            NonSeriesEventCount = 0;
        }

        public long CohortGroupId { get; set; }
        public int PatientCount { get; set; }
        public int SeriesEventCount { get; set; }
        public int NonSeriesEventCount { get; set; }
    }
}
