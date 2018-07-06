using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class ActivityListItem
    {
        public string Activity { get; set; }
        [Display(Name = "Execution Event")]
        public string ExecutionEvent { get; set; }
        [Display(Name = "Executed By")]
        public string ExecutionBy { get; set; }
        [Display(Name = "Executed Date")]
        public string ExecutionDate { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Receipt Date")]
        public string ReceiptDate { get; set; }
        [Display(Name = "Receipt Code")]
        public string ReceiptCode { get; set; }
        public int PatientSummaryFileId { get; set; }
        public int PatientExtractFileId { get; set; }
        public int E2bXmlFileId { get; set; }
    }
}
