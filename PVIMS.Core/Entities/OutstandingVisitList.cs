using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class OutstandingVisitList : Entity<int>
    {
        public int Patient_Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
