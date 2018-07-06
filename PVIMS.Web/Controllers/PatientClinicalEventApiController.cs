using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using System.Net;
using System.Net.Http;

using System.Threading.Tasks;
using System.Web.Http;

using VPS.Common.Repositories;
using VPS.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;

namespace PVIMS.Web.Controllers
{
    public class PatientClinicalEventApiController : ApiController
    {
        private readonly IUnitOfWorkInt unitOfWork;

        public PatientClinicalEventApiController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/PatientClinicalEventApi
        public async Task<IEnumerable<PatientClinicalEventDTO>> Get()
        {
            DateTime dttemp;
            return await Task.Run(() => unitOfWork.Repository<PatientClinicalEvent>()
                .Queryable()
                .ToList()
                .Select(p => new PatientClinicalEventDTO
                {
                    PatientId = p.Patient.Id,
                    PatientClinicalEventIdentifier = p.PatientClinicalEventGuid,
                    PatientClinicalEventId = p.Id,
                    Description = p.SourceDescription,
                    MedDraId = p.SourceTerminologyMedDra.Id,
                    OnsetDate = p.OnsetDate,
                    ResolutionDate = p.ResolutionDate,
                    ReportedDate = DateTime.TryParse(GetCustomAttributeValue(unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Date of Report"), (IExtendable)p), out dttemp) ? dttemp : (DateTime?)null,
                    customAttributes = getCustomAttributes(p)
                }).ToArray());
        }

        private string GetCustomAttributeValue(CustomAttributeConfiguration config, IExtendable extended)
        {
            if (extended.GetAttributeValue(config.AttributeKey) == null) { return ""; };

            var val = extended.GetAttributeValue(config.AttributeKey).ToString();
            if (config.CustomAttributeType == CustomAttributeType.Selection)
            {
                var selection = unitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(s => s.AttributeKey == config.AttributeKey && s.SelectionKey == val);
                return selection.Value;
            }
            else
            {
                return val;
            }
        }

        [ActionName("getTerminologyMedDra")]
        public async Task<IEnumerable<Object>> GetTerminology()
        {
            return await Task.Run(() => unitOfWork.Repository<TerminologyMedDra>()
                            .Queryable()                            
                            .Select(c => new 
                            {
                                Id = c.Id,
                                MedDraCode = c.MedDraCode,
                                MedDraTermType = c.MedDraTermType,
                                Description = c.MedDraTerm
                            })
                            .OrderBy(c => c.Description)
                            .ToArray());
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(JObject items)
        {
            if (items == null)
                return Request.CreateResponse(HttpStatusCode.NoContent, "No records to process ");

            dynamic json = items;

            List<PatientClinicalEvent> synchedPatientClinicalEvents =  new List<PatientClinicalEvent>();

            IList<PatientClinicalEventDTO> patientClinicalEvents = await Task.Run(() => ((JArray)json.items)
                .Select(t => new PatientClinicalEventDTO
                {
                    PatientClinicalEventId = ((dynamic)t).PatientClinicalEventId,
                    PatientClinicalEventIdentifier = ((dynamic)t).PatientClinicalEventIdentifier,
                    PatientId = ((dynamic)t).PatientId,
                    OnsetDate = ((dynamic)t).OnsetDate != null ? ((dynamic)t).OnsetDate : null,
                    ReportedDate = ((dynamic)t).ReportedDate != null ? ((dynamic)t).ReportedDate : null,
                    ResolutionDate = ((dynamic)t).ResolutionDate != null ? ((dynamic)t).ResolutionDate : null,
                    Description = ((dynamic)t).Description,
                    MedDraId = ((dynamic)t).MedDraId,
                    customAttributes = ((dynamic)t).customAttributes == null ? null : ((JArray)(((dynamic)t).customAttributes))
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
                }).ToList());

            var termResults = await Task.Run(() => unitOfWork.Repository<TerminologyMedDra>()
                            .Queryable().ToList());

            foreach (PatientClinicalEventDTO patientEvent in patientClinicalEvents)
            {
                PatientClinicalEvent obj = await unitOfWork.Repository<PatientClinicalEvent>()
                    .Queryable()
                    .SingleOrDefaultAsync(e => e.PatientClinicalEventGuid == patientEvent.PatientClinicalEventIdentifier);

                if(obj == null)
                {
                    obj = new PatientClinicalEvent
                    {
                        PatientClinicalEventGuid = patientEvent.PatientClinicalEventIdentifier,
                        Patient = unitOfWork.Repository<Patient>().Queryable().SingleOrDefault(e => e.Id == patientEvent.PatientId),
                        //Id = patientEvent.PatientClinicalEventId,
                        OnsetDate = patientEvent.OnsetDate,
                        ResolutionDate = patientEvent.ResolutionDate,
                        SourceDescription = patientEvent.Description,
                        SourceTerminologyMedDra = termResults.FirstOrDefault(e => e.Id == patientEvent.MedDraId)  
                                   
                    };

                    setCustomAttributes(patientEvent.customAttributes, obj);                   

                    unitOfWork.Repository<PatientClinicalEvent>().Save(obj);
                    synchedPatientClinicalEvents.Add(obj);
                }
                else
                {
                    obj.OnsetDate = patientEvent.OnsetDate;
                    obj.ResolutionDate = patientEvent.ResolutionDate;
                    obj.SourceDescription = patientEvent.Description;
                    obj.SourceTerminologyMedDra = termResults.FirstOrDefault(e => e.Id == patientEvent.MedDraId);

                    setCustomAttributes(patientEvent.customAttributes, obj);

                    unitOfWork.Repository<PatientClinicalEvent>().Update(obj);
                    synchedPatientClinicalEvents.Add(obj);
                }               
            }

            unitOfWork.Complete();
            
            DateTime dttemp;
            var insertedObjs = synchedPatientClinicalEvents.Select(p => new PatientClinicalEventDTO
            {
                PatientId = p.Patient.Id,
                PatientClinicalEventIdentifier = p.PatientClinicalEventGuid,
                PatientClinicalEventId = p.Id,
                Description = p.SourceDescription,
                MedDraId = p.SourceTerminologyMedDra.Id,
                OnsetDate = p.OnsetDate,
                ResolutionDate = p.ResolutionDate,
                ReportedDate = DateTime.TryParse(GetCustomAttributeValue(unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Date of Report"), (IExtendable)p), out dttemp) ? dttemp : (DateTime?)null,
                customAttributes = getCustomAttributes(p)
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

        private void setCustomAttributes(IEnumerable<CustomAttributeDTO> customAttributes, PatientClinicalEvent clinicalEvent)
        {
            if (customAttributes != null)
            {
                var clinicalEventConditionExtended = (IExtendable)clinicalEvent;

                foreach (var customAttribute in customAttributes)
                {

                    switch (customAttribute.AttributeTypeName)
                    {
                        case "Numeric":
                            decimal number = 0M;
                            if (decimal.TryParse(customAttribute.currentValue, out number))
                            {
                                clinicalEventConditionExtended.SetAttributeValue(customAttribute.AttributeName, number, User.Identity.Name);
                            }
                            break;
                        case "Selection":
                            Int32 selection = 0;
                            if (Int32.TryParse(customAttribute.currentValue, out selection))
                            {
                                clinicalEventConditionExtended.SetAttributeValue(customAttribute.AttributeName, selection, User.Identity.Name);
                            }
                            break;
                        case "DateTime":
                            DateTime parsedDate = DateTime.MinValue;
                            if (DateTime.TryParse(customAttribute.currentValue, out parsedDate))
                            {
                                clinicalEventConditionExtended.SetAttributeValue(customAttribute.AttributeName, parsedDate, User.Identity.Name);
                            }
                            break;
                        case "String":
                        default:
                            clinicalEventConditionExtended.SetAttributeValue(customAttribute.AttributeName, customAttribute.currentValue ?? string.Empty, User.Identity.Name);
                            break;
                    }
                }
            }
        }

        private List<CustomAttributeDTO> getCustomAttributes(PatientClinicalEvent clinicalEvent)
        {
            var extendable = (IExtendable)clinicalEvent;

            var customAttributes = unitOfWork.Repository<PVIMS.Core.Entities.CustomAttributeConfiguration>()
              .Queryable()
              .Where(ca => ca.ExtendableTypeName == typeof(PatientClinicalEvent).Name)
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
