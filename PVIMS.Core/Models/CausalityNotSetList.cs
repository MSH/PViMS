using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class CausalityNotSetList : Entity<int>
    {
        public int Patient_Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AdverseEvent { get; set; }
        public string Serious { get; set; }
        public DateTime OnsetDate { get; set; }
        public string NaranjoCausality { get; set; }
        public string WhoCausality { get; set; }
        public string Medicationidentifier { get; set; }
    }
}
