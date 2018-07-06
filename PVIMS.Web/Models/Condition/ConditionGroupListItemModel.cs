using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class ConditionGroupListItemModel
    {
        public string ConditionGroup { get; set; }
        public string Status { get; set; }
        public string Detail { get; set; }
        public DateTime StartDate { get; set; }
        public int PatientConditionId { get; set; }
    }
}