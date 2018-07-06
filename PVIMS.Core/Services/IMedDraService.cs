using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VPS.Common.Collections;
using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.Models;

namespace PVIMS.Core.Services
{
    public interface IMedDraService
    {
        string ValidateSourceData(string fileName, string subdirectory);
        string ImportSourceData(string fileName, string subdirectory);
    }
}
