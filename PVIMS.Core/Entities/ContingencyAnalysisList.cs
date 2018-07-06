using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class ContingencyAnalysisList : Entity<int>
    {
        public int TerminologyMeddra_Id { get; set; }
        public string MeddraTerm { get; set; }
    }
}
