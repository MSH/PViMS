using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientLabTestListItemModel
    {
        public int PatientLabTestId { get; set; }
        public string TestName { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TestDate { get; set; }
        public string TestResult { get; set; }
        public string TestUnit { get; set; }
        public string LabValue { get; set; }
        public string Range { get; set; }
    }
}