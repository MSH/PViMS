using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVIMS.Core.Entities
{
    public enum AuditType
    {
        InvalidSubscriberAccess = 1,
        ValidSubscriberAccess = 2,
        InValidSubscriberPost = 3,
        ValidSubscriberPost = 4,
        UserLogin = 5,
        InValidMedDRAImport = 6,
        ValidMedDRAImport = 7
    }
}
