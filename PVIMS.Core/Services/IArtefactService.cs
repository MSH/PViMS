using System;

using PVIMS.Core.Models;

namespace PVIMS.Core.Services
{
    public interface IArtefactService
    {
        ArtefactInfoModel CreateActiveDatasetForDownload(long patientId, long cohortGroupId);

        ArtefactInfoModel CreateDatasetInstanceForDownload(long datasetInstanceId);

        ArtefactInfoModel CreateE2B(long datasetInstanceId);

        ArtefactInfoModel CreatePatientSummaryForActiveReport(Guid contextGuid, bool isSerious);

        ArtefactInfoModel CreatePatientSummaryForSpontaneousReport(Guid contextGuid, bool isSerious);

        ArtefactInfoModel CreateSpontaneousDatasetForDownload();
    }
}
