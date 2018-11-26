using PVIMS.Core.Entities;
using PVIMS.Core.Dto;

namespace PVIMS.Core.Services
{
    public interface IPatientService
    {
        DatasetInstanceValueDto GetCurrentElementValueForPatient(long patientId, string datasetName, string elementName);
        DatasetInstanceValueListDto GetElementValuesForPatient(long patientId, string datasetName, string elementName, int records);
    }
}
