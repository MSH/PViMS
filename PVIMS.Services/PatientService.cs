using System;
using System.Data.Entity;
using System.Linq;

using VPS.Common.Repositories;
using VPS.Common.Utilities;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;
using PVIMS.Core.Dto;

namespace PVIMS.Services
{
    public class PatientService : IPatientService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        private readonly IRepositoryInt<Patient> _patientRepository;

        public PatientService(IUnitOfWorkInt unitOfWork)
        {
            Check.IsNotNull(unitOfWork, "unitOfWork may not be null");

            _unitOfWork = unitOfWork;

            _patientRepository = unitOfWork.Repository<Patient>();
        }

        public DatasetInstanceValueListDto GetElementValuesForPatient(long patientId, string datasetName, string elementName, int records)
        {
            var encounters = _unitOfWork.Repository<Encounter>().Queryable()
                .Where(e => e.Patient.Id == patientId)
                .OrderByDescending(e => e.EncounterDate).Take(records);

            var datasetElement = _unitOfWork.Repository<DatasetElement>().Queryable()
                .SingleOrDefault(de => de.ElementName == elementName && de.DatasetCategoryElements.Any(dce => dce.DatasetCategory.Dataset.DatasetName == datasetName));

            var model = new DatasetInstanceValueListDto()
            {
                DatasetElement = datasetElement
            };

            if (datasetElement != null)
            {
                foreach (Encounter encounter in encounters)
                {
                    var val = GetValueForElement(_unitOfWork.Repository<DatasetInstance>().Queryable().SingleOrDefault(di => di.ContextID == encounter.Id), datasetElement);
                    var modelItem = new DatasetInstanceValueListItem()
                    {
                        Value = !String.IsNullOrWhiteSpace(val) ? val : "NO VALUE",
                        ValueDate = encounter.EncounterDate
                    };
                    model.Values.Add(modelItem);
                }
            }

            return model;
        }

        public DatasetInstanceValueDto GetCurrentElementValueForPatient(long patientId, string datasetName, string elementName)
        {
            var model = new DatasetInstanceValueDto() {
                ElementName = elementName,
                Value = "NO VALUE"
            };

            var patient = _unitOfWork.Repository<Patient>().Queryable()
                .Include(p => p.Encounters)
                .Single(p => p.Id == patientId);

            var datasetElement = _unitOfWork.Repository<DatasetElement>().Queryable()
                .SingleOrDefault(de => de.ElementName == elementName && de.DatasetCategoryElements.Any(dce => dce.DatasetCategory.Dataset.DatasetName == datasetName));

            if (patient.Encounters.Count > 0 && datasetElement != null)
            {
                var encounter = patient.GetCurrentEncounter();

                // Get Dataset Instance for encounter
                var instance = _unitOfWork.Repository<DatasetInstance>().Queryable()
                    .SingleOrDefault(di => di.ContextID == encounter.Id && di.Dataset.ContextType.Description == "Encounter");
                if (instance != null)
                {
                    model.CollectionDate = encounter.EncounterDate;
                    model.Value = instance.GetInstanceValue(datasetElement);
                }
            }

            return model;
        }

        #region "Private"

        private string GetValueForElement(DatasetInstance instance, DatasetElement element)
        {
            if (instance == null) { return string.Empty; };

            var value = instance.DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == element.Id);
            if (value == null) { return string.Empty; };
            return value.InstanceValue;
        }

        #endregion
    }
}
