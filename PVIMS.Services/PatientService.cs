using System;
using System.Data.Entity;
using System.Linq;

using VPS.Common.Repositories;
using VPS.Common.Utilities;

using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;

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

        #region "Referential Checks"

        public DatasetInstanceValueList GetElementValuesForPatient(Patient patient, DatasetElement element, int records)
        {
            var encounters = _unitOfWork.Repository<Encounter>().Queryable().Where(e => e.Patient.Id == patient.Id).OrderByDescending(e => e.EncounterDate).Take(records);
            var model = new DatasetInstanceValueList()
            {
                DatasetElement = element
            };
            foreach(Encounter encounter in encounters)
            {
                var val = GetValueForElement(_unitOfWork.Repository<DatasetInstance>().Queryable().SingleOrDefault(di => di.ContextID == encounter.Id), element);
                var modelItem = new DatasetInstanceValueListItem()
                {
                    Value = !String.IsNullOrWhiteSpace(val) ? val : "NO VALUE",
                    ValueDate = encounter.EncounterDate
                };
                model.Values.Add(modelItem);
            }

            return model;
        }


        #endregion

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
