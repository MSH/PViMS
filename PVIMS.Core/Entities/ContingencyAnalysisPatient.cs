using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class ContingencyAnalysisPatient : Entity<int>
    {
        public string PatientName { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public string Drug { get; set; }
        public int DaysContributed { get; set; }
        public int ADR { get; set; }
        public string RiskFactor { get; set; }
        public string RiskFactorOption { get; set; }
        public string FactorMet { get; set; }
    }
}
