using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VPS.Common.Repositories;
using VPS.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    public class PatientMedicationApiController : ApiController
    {
        private readonly IUnitOfWorkInt unitOfWork;

        public PatientMedicationApiController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/PatientMedicationApi
        public async Task<IEnumerable<PatientMedicationDTO>> Get()
        {
            return await Task.Run(() => unitOfWork.Repository<PatientMedication>()
                .Queryable()
                .ToList()
                .Select(p => new PatientMedicationDTO
                {
                    PatientMedicationIdentifier = p.PatientMedicationGuid,
                    PatientMedicationId  = p.Id,
                    PatientId = p.Patient.Id,
                    MedicationId = p.Medication != null? p.Medication.Id: default(int),
                    StartDate = p.DateStart,
                    EndDate = p.DateEnd,
                    Dose = p.Dose,
                    DoseFrequency = p.DoseFrequency,
                    DoseUnit = p.DoseUnit,
                    CustomAttributes = getCustomAttributes(p)
                }).ToArray());
        }

        [ActionName("getMedications")]
        public async Task<IEnumerable<MedicationDTO>> GetMedications()
        {
            return await Task.Run(() => unitOfWork.Repository<Medication>()
                .Queryable()
                .Select(m => new MedicationDTO
                {
                    MedicationId = m.Id,
                    DrugName = m.DrugName,
                    Active = m.Active,
                    CatalogNo = m.CatalogNo,
                    PackSize = m.PackSize,
                    Strength = m.Strength
                })
                .ToArray());
        }

        [HttpPost]
        public HttpResponseMessage Post(JObject items)
        {
            if (items == null)
                return Request.CreateResponse(HttpStatusCode.NoContent, "No records to process ");

            dynamic json = items;

            List<PatientMedication> synchedPatientMedications = new List<PatientMedication>();

            IList<PatientMedicationDTO> patientMedications = ((JArray)json.items)
                .Select(t => new PatientMedicationDTO
                {
                    PatientMedicationIdentifier = ((dynamic)t).PatientMedicationIdentifier,
                    PatientId = ((dynamic)t).PatientId,
                    PatientMedicationId = ((dynamic)t).PatientMedicationId,
                    MedicationId = ((dynamic)t).MedicationId,
                    StartDate = ((dynamic)t).StartDate,
                    EndDate = ((dynamic)t).EndDate,
                    Dose = ((dynamic)t).Dose,
                    DoseFrequency = ((dynamic)t).DoseFrequency,
                    DoseUnit = ((dynamic)t).DoseUnit,
                    CustomAttributes = ((dynamic)t).CustomAttributes == null ? null : ((JArray)(((dynamic)t).CustomAttributes))
                    .Select(x => new CustomAttributeDTO 
                    {
                        CustomAttributeConfigId = ((dynamic)x).CustomAttributeConfigId,
                        AttributeTypeName = ((dynamic)x).AttributeTypeName,
                        AttributeName = ((dynamic)x).AttributeName,
                        Category = ((dynamic)x).Category,
                        EntityName = ((dynamic)x).EntityName,
                        currentValue = ((dynamic)x).currentValue,
                        lastUpdated = ((dynamic)x).lastUpdated != "" ? ((dynamic)x).lastUpdated : null,
                        lastUpdatedUser = ((dynamic)x).lastUpdatedUser,
                        Required = ((dynamic)x).Required,
                        NumericMaxValue = ((dynamic)x).NumericMaxValue != "" ? ((dynamic)x).NumericMaxValue : null,
                        NumericMinValue = ((dynamic)x).NumericMinValue != "" ? ((dynamic)x).NumericMinValue : null,
                        StringMaxLength = ((dynamic)x).StringMaxLength != "" ? ((dynamic)x).StringMaxLength : null,
                        FutureDateOnly = ((dynamic)x).FutureDateOnly != "" ? ((dynamic)x).FutureDateOnly : null,
                        PastDateOnly = ((dynamic)x).PastDateOnly != "" ? ((dynamic)x).PastDateOnly : null
                    }).ToList()
                }).ToList();

            // Load entities depedency
            List<Medication> medications = unitOfWork.Repository<Medication>().Queryable().ToList();

            foreach(PatientMedicationDTO patientMedication in patientMedications)
            {
                PatientMedication obj = unitOfWork.Repository<PatientMedication>()
                    .Queryable()
                    .SingleOrDefault(e => e.PatientMedicationGuid == patientMedication.PatientMedicationIdentifier);

                if(obj == null)
                {
                    obj = new PatientMedication
                    {
                        PatientMedicationGuid = patientMedication.PatientMedicationIdentifier,
                        //Id = patientMedication.PatientMedicationId,
                        Patient = unitOfWork.Repository<Patient>().Queryable().SingleOrDefault(e => e.Id == patientMedication.PatientId),
                        DateStart = patientMedication.StartDate,
                        DateEnd = patientMedication.EndDate,
                        Dose = patientMedication.Dose,
                        DoseFrequency = patientMedication.DoseFrequency,
                        DoseUnit = patientMedication.DoseUnit,
                        Medication = medications.Find(x => x.Id == patientMedication.MedicationId)
                    };

                    setCustomAttributes(patientMedication.CustomAttributes, obj);

                    if (obj.Patient == null)
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Can't find the patient for patient Medication record " +
                            obj.PatientMedicationGuid);

                    unitOfWork.Repository<PatientMedication>().Save(obj);
                    synchedPatientMedications.Add(obj);

                }
                else // update record
                {
                    obj.DateStart = patientMedication.StartDate;
                    obj.DateEnd = patientMedication.EndDate;
                    obj.Dose = patientMedication.Dose;
                    obj.DoseFrequency = patientMedication.DoseFrequency;
                    obj.DoseUnit = patientMedication.DoseUnit;
                    obj.Medication = medications.Find(x => x.Id == patientMedication.MedicationId);
                    setCustomAttributes(patientMedication.CustomAttributes, obj);
                    synchedPatientMedications.Add(obj);
                    unitOfWork.Repository<PatientMedication>().Update(obj);
                }
            
            }

            unitOfWork.Complete();

            var insertedObjs = synchedPatientMedications.Select(p => new PatientMedicationDTO
                {
                    PatientMedicationIdentifier = p.PatientMedicationGuid,
                    PatientMedicationId  = p.Id,
                    PatientId = p.Patient.Id,
                    MedicationId = p.Medication != null? p.Medication.Id: default(int),
                    StartDate = p.DateStart,
                    EndDate = p.DateEnd,
                    Dose = p.Dose,
                    DoseFrequency = p.DoseFrequency,
                    DoseUnit = p.DoseUnit,
                    CustomAttributes = getCustomAttributes(p)
                }).ToArray();

            return Request.CreateResponse(HttpStatusCode.OK, insertedObjs);
        }

        private string GetCustomAttributeVale(IExtendable extendable, PVIMS.Core.Entities.CustomAttributeConfiguration customAttribute)
        {
            var attributeValue = extendable.GetAttributeValue(customAttribute.AttributeKey);

            if (attributeValue == null) return string.Empty;

            if (customAttribute.CustomAttributeType == CustomAttributeType.DateTime)
            {
                DateTime datetimeValue = DateTime.MinValue;

                if (DateTime.TryParse(attributeValue.ToString(), out datetimeValue))
                {
                    return datetimeValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return attributeValue.ToString();
            }
        }

        private void setCustomAttributes(IEnumerable<CustomAttributeDTO> customAttributes, PatientMedication patientMedication)
        {
            if (customAttributes != null)
            {
                var patientMedicaationConditionExtended = (IExtendable)patientMedication;

                foreach (var customAttribute in customAttributes)
                {

                    switch (customAttribute.AttributeTypeName)
                    {
                        case "Numeric":
                            decimal number = 0M;
                            if (decimal.TryParse(customAttribute.currentValue, out number))
                            {
                                patientMedicaationConditionExtended.SetAttributeValue(customAttribute.AttributeName, number, User.Identity.Name);
                            }
                            break;
                        case "Selection":
                            Int32 selection = 0;
                            if (Int32.TryParse(customAttribute.currentValue, out selection))
                            {
                                patientMedicaationConditionExtended.SetAttributeValue(customAttribute.AttributeName, selection, User.Identity.Name);
                            }
                            break;
                        case "DateTime":
                            DateTime parsedDate = DateTime.MinValue;
                            if (DateTime.TryParse(customAttribute.currentValue, out parsedDate))
                            {
                                patientMedicaationConditionExtended.SetAttributeValue(customAttribute.AttributeName, parsedDate, User.Identity.Name);
                            }
                            break;
                        case "String":
                        default:
                            patientMedicaationConditionExtended.SetAttributeValue(customAttribute.AttributeName, customAttribute.currentValue ?? string.Empty, User.Identity.Name);
                            break;
                    }
                }
            }
        }

        private List<CustomAttributeDTO> getCustomAttributes(PatientMedication patientMedication)
        {
            var extendable = (IExtendable)patientMedication;

            var customAttributes = unitOfWork.Repository<PVIMS.Core.Entities.CustomAttributeConfiguration>()
              .Queryable()
              .Where(ca => ca.ExtendableTypeName == typeof(PatientMedication).Name)
              .ToList();

            return customAttributes.Select(c => new CustomAttributeDTO
            {
                CustomAttributeConfigId = c.Id,
                AttributeName = c.AttributeKey,
                AttributeTypeName = c.CustomAttributeType.ToString(),
                Category = c.Category,
                EntityName = c.ExtendableTypeName,
                currentValue = GetCustomAttributeVale(extendable, c),
                lastUpdated = extendable.CustomAttributes.GetUpdatedDate(c.AttributeKey),
                lastUpdatedUser = extendable.GetUpdatedByUser(c.AttributeKey),
                Required = c.IsRequired,
                NumericMaxValue = c.NumericMaxValue,
                NumericMinValue = c.NumericMinValue,
                StringMaxLength = c.StringMaxLength,
                FutureDateOnly = c.FutureDateOnly,
                PastDateOnly = c.PastDateOnly,
            })
            .ToList();
        }
    }
}
