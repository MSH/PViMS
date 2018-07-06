using System;
using System.Collections.Generic;

using PVIMS.Core.Entities;

namespace PVIMS.Core.Models
{
    public class ActivityExecutionStatusForPatient
    {
        public ActivityExecutionStatusForPatient()
        {
            ActivityItems = new List<ActivityExecutionStatusInfo>();
        }

        public PatientClinicalEvent PatientClinicalEvent { get; set; }
        public ICollection<ActivityExecutionStatusInfo> ActivityItems { get; private set; }

        public class ActivityExecutionStatusInfo
        {
            public string Status { get; set; }
            public string StatusDate { get; set; }
            public string Comments { get; set; }

            public string Display
            {
                get
                {
                    return String.Format("{0} on {1} {2}", Status, StatusDate, String.IsNullOrEmpty(Comments) ? "" : "| " + Comments);
                }
            }
        }
    }
}