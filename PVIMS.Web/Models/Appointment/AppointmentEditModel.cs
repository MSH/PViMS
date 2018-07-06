using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class AppointmentEditModel
    {
        public int AppointmentId { get; set; }
        
        public string PatientFullName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public string Cancelled { get; set; }
        [Display(Name = "Cancellation Reason")]
        public string CancellationReason { get; set; }

        public string Created { get; set; }
        public string Updated { get; set; }
    }
}