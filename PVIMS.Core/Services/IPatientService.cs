using PVIMS.Core.Entities;
using PVIMS.Core.Models;

namespace PVIMS.Core.Services
{
    public interface IPatientService
    {
        DatasetInstanceValueList GetElementValuesForPatient(Patient patient, DatasetElement element, int records);

    }
}
