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
    public class PatientConditionApiController : ApiController
    {
        private readonly IUnitOfWorkInt unitOfWork;

        public PatientConditionApiController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/PatientConditionApi
        public async Task<IEnumerable<PatientConditionDTO>> Get()
        {
            return await Task.Run(() => unitOfWork.Repository<PatientCondition>()
                .Queryable()
                .ToList()
                .Select(p => new PatientConditionDTO
                {
                    PatientConditionIdentifier = p.PatientConditionGuid,
                    PatientConditionId = p.Id,
                    PatientId = p.Patient.Id,
                    MedDraId = p.TerminologyMedDra.Id,
                    //ConditionId = p.Condition != null ? p.Condition.Id: default(int),
                    StartDate = p.DateStart,
                    EndDate = p.OutcomeDate,
                    Comments = p.Comments,
                    customAttributes = getCustomAttributes(p)
                }).ToArray());
        }

        [ActionName("getConditions")]
        public async Task<IEnumerable<ConditionDTO>> GetConditions()
        {
            return await Task.Run(() => unitOfWork.Repository<Condition>()
                .Queryable()
                .Select(m => new ConditionDTO
                {
                    ConditionId = m.Id,
                    Description = m.Description
                })
                .ToArray());
        }

        [HttpPost]
        public HttpResponseMessage Post(JObject items)
        {
            if (items == null)
                return Request.CreateResponse(HttpStatusCode.NoContent, "No records to process ");

            dynamic json = items;

            List<PatientCondition> synchedPatientConditions = new List<PatientCondition>();

            IList<PatientConditionDTO> patientConditions = ((JArray)json.items)
                .Select(t => new PatientConditionDTO
                {
                    PatientConditionIdentifier = ((dynamic)t).PatientConditionIdentifier,
                    PatientId = ((dynamic)t).PatientId,
                    PatientConditionId = ((dynamic)t).PatientConditionId,
                    MedDraId = ((dynamic)t).MedDraId,
                    //ConditionId = ((dynamic)t).ConditionId,
                    StartDate = ((dynamic)t).StartDate != "" ? ((dynamic)t).StartDate : null,
                    EndDate = ((dynamic)t).EndDate != "" ? ((dynamic)t).EndDate : null,
                    Comments = ((dynamic)t).Comments,
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
                }).ToList();

            // Load entities depedency
            List<Condition> conditions = unitOfWork.Repository<Condition>().Queryable().ToList();

            var termResults = unitOfWork.Repository<TerminologyMedDra>()
                            .Queryable().ToList();

            foreach (PatientConditionDTO patientCondition in patientConditions)
            {
                PatientCondition obj = unitOfWork.Repository<PatientCondition>()
                    .Queryable()
                    .SingleOrDefault(e => e.PatientConditionGuid == patientCondition.PatientConditionIdentifier);

                if (obj == null)
                {
                    obj = new PatientCondition
                    {
                        PatientConditionGuid = patientCondition.PatientConditionIdentifier,
                        //Id = patientCondition.PatientConditionId,
                        Patient = unitOfWork.Repository<Patient>().Queryable().SingleOrDefault(e => e.Id == patientCondition.PatientId),
                        DateStart = patientCondition.StartDate,
                        OutcomeDate = patientCondition.EndDate,
                        Comments = patientCondition.Comments,
                        //Condition = conditions.Find(x => x.Id == patientCondition.ConditionId)
                        TerminologyMedDra = termResults.FirstOrDefault(e => e.Id == patientCondition.MedDraId)
                    };

                    setCustomAttributes(patientCondition.customAttributes, obj);

                    if (obj.Patient == null)
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Can't find the patient for Patient Condition record " +
                            obj.PatientConditionGuid);

                    unitOfWork.Repository<PatientCondition>().Save(obj);
                    synchedPatientConditions.Add(obj);

                }
                else // update record
                {
                    obj.DateStart = patientCondition.StartDate;
                    obj.OutcomeDate = patientCondition.EndDate;
                    obj.Comments = patientCondition.Comments;
                    //obj.Condition = conditions.Find(x => x.Id == patientCondition.ConditionId);
                    obj.TerminologyMedDra = termResults.FirstOrDefault(e => e.Id == patientCondition.MedDraId);
                    setCustomAttributes(patientCondition.customAttributes, obj);
                    synchedPatientConditions.Add(obj);
                    unitOfWork.Repository<PatientCondition>().Update(obj);
                }

            }

            unitOfWork.Complete();

            var insertedObjs = synchedPatientConditions.Select(p => new PatientConditionDTO
            {
                PatientConditionIdentifier = p.PatientConditionGuid,
                PatientConditionId = p.Id,
                PatientId = p.Patient.Id,
                //ConditionId = p.Condition != null ? p.Condition.Id : default(int),
                StartDate = p.DateStart,
                EndDate = p.OutcomeDate,
                MedDraId = p.TerminologyMedDra.Id,
                Comments = p.Comments,
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

        private void setCustomAttributes(IEnumerable<CustomAttributeDTO> customAttributes, PatientCondition patientCondition)
        {
            if (customAttributes != null)
            {
                var patientConditionExtended = (IExtendable)patientCondition;

                foreach (var customAttribute in customAttributes)
                {

                    switch (customAttribute.AttributeTypeName)
                    {
                        case "Numeric":
                            decimal number = 0M;
                            if (decimal.TryParse(customAttribute.currentValue, out number))
                            {
                                patientConditionExtended.SetAttributeValue(customAttribute.AttributeName, number, User.Identity.Name);
                            }
                            break;
                        case "Selection":
                            Int32 selection = 0;
                            if (Int32.TryParse(customAttribute.currentValue, out selection))
                            {
                                patientConditionExtended.SetAttributeValue(customAttribute.AttributeName, selection, User.Identity.Name);
                            }
                            break;
                        case "DateTime":
                            DateTime parsedDate = DateTime.MinValue;
                            if (DateTime.TryParse(customAttribute.currentValue, out parsedDate))
                            {
                                patientConditionExtended.SetAttributeValue(customAttribute.AttributeName, parsedDate, User.Identity.Name);
                            }
                            break;
                        case "String":
                        default:
                            patientConditionExtended.SetAttributeValue(customAttribute.AttributeName, customAttribute.currentValue ?? string.Empty, User.Identity.Name);
                            break;
                    }
                }
            }
        }

        private List<CustomAttributeDTO> getCustomAttributes(PatientCondition patientCondition)
        {
            var extendable = (IExtendable)patientCondition;

            var customAttributes = unitOfWork.Repository<PVIMS.Core.Entities.CustomAttributeConfiguration>()
              .Queryable()
              .Where(ca => ca.ExtendableTypeName == typeof(PatientCondition).Name)
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
