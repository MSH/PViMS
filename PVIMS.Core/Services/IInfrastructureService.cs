using PVIMS.Core.Entities;

namespace PVIMS.Core.Services
{
    public interface IInfrastructureService
    {
        bool HasAssociatedData(DatasetElement element);
        DatasetElement GetTerminologyMedDra();
        Config GetOrCreateConfig(ConfigType configType);
    }
}
