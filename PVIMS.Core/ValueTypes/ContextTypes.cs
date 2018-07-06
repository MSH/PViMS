using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVIMS.Core.ValueTypes
{
    public enum ContextTypes
    {
        Encounter = 1,
        Patient = 2,
        Pregnancy = 3,
        Global = 4,
        PatientClinicalEvent = 5,
        DatasetInstance = 6
    }
}
