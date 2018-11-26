namespace PVIMS.Core.Models
{
    public class PatientEventSummary
    {
        public long PatientId { get; set; }
        public int SeriesEventCount { get; set; }
        public int NonSeriesEventCount { get; set; }
    }
}
