using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using VPS.Common.Repositories;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;
using LinqKit;
using VPS.CustomAttributes;
using Newtonsoft.Json.Linq;

namespace PVIMS.Web.Controllers
{
    public class PatientApiController : ApiController
    {
        private readonly IUnitOfWorkInt unitOfwork;

        public PatientApiController(IUnitOfWorkInt unitOfwork)
        {
            this.unitOfwork = unitOfwork;
        }

        // GET api/<controller>
        [WebApiUnitOfWork]
        public async Task<IEnumerable<PatientDTO>> Get()
        {
            return await Task.Run(() => unitOfwork.Repository<Patient>()
                .Queryable()
                .ToList()
                .Select(p => new PatientDTO 
                { 
                    PatientId = p.Id, 
                    PatientUniqueIdentifier = p.PatientGuid,
                    PatientFirstName = p.FirstName, 
                    PatientLastName = p.Surname, 
                    PatientDateOfBirth = p.DateOfBirth.GetValueOrDefault(DateTime.MinValue),
                }).ToArray());
        }

        [ActionName("lastpatients")]
        public async Task<Object> GetAll()
        {
            DateTime lastOperationDate = DateTime.Now;

            var patients = await Task.Run(() => unitOfwork.Repository<Patient>()
                .Queryable()
                .ToList()
                .Select(p => new PatientDTO
                {
                    PatientId = p.Id,
                    PatientUniqueIdentifier = p.PatientGuid,
                    PatientFirstName = p.FirstName,
                    PatientMiddleName = p.MiddleName,
                    PatientLastName = p.Surname,
                    FacilityId = p.GetCurrentFacility().Facility.Id,
                    CreatedBy = p.CreatedBy.FullName,                    
                    PatientCreatedDate = p.Created,
                    UpdatedBy = p.UpdatedBy != null ? p.UpdatedBy.FullName: string.Empty,
                    PatientUpdatedDate = p.LastUpdated.GetValueOrDefault(DateTime.MinValue),
                    PatientDateOfBirth = p.DateOfBirth.GetValueOrDefault(DateTime.MinValue),
                    Notes = p.Notes,
                    customAttributes = getCustomAttributes(p)
                }).ToArray());

            return new
            {
                patients = patients,
                lastUpdatedDate = lastOperationDate
            };
        }

        [ActionName("lastpatients")]
        public async Task<Object> GetAll(DateTime id)
        {
            DateTime lastOperationDate = DateTime.Now;

            DateTime lastDate = id;

            var patients = await Task.Run(() => unitOfwork.Repository<Patient>()
                .Queryable()
                .Where(k => k.LastUpdated >= lastDate || k.Created >= lastDate)
                .ToList()
                .Select(p => new PatientDTO
                {
                    PatientId = p.Id,
                    PatientUniqueIdentifier = p.PatientGuid,
                    PatientFirstName = p.FirstName,
                    PatientMiddleName = p.MiddleName,
                    PatientLastName = p.Surname,
                    FacilityId = p.GetCurrentFacility().Facility.Id,
                    CreatedBy = p.CreatedBy.FullName,
                    PatientCreatedDate = p.Created,
                    UpdatedBy = p.UpdatedBy != null ? p.UpdatedBy.FullName : string.Empty,
                    PatientUpdatedDate = p.LastUpdated.GetValueOrDefault(DateTime.MinValue),
                    PatientDateOfBirth = p.DateOfBirth.GetValueOrDefault(DateTime.MinValue),
                    Notes = p.Notes,
                    customAttributes = getCustomAttributes(p)
                }).ToArray());

            return new
            {
                patients = patients,
                lastUpdatedDate = lastOperationDate
            };
        }

        [ActionName("getFacilities")]
        public async Task<IEnumerable<Object>> GetConditions()
        {
            return await Task.Run(() => unitOfwork.Repository<Facility>()
                .Queryable()
                .Select(f => new 
                {
                    Id = f.Id,
                    FacilityCode = f.FacilityCode,
                    FacilityName = f.FacilityName,
                    FacilityType = f.FacilityType.Description
                })
                .ToArray());
        }

        [ActionName("Search")]
        [HttpGet]
        public async Task<IHttpActionResult> Search(int facilityId = default(int), string puid = default(string), string firstName = default(string), string surname = default(string))
        {
            var patientPredicate = PredicateBuilder.True<Patient>();

            if (facilityId != default(int))
            {
                // TODO: Apply the facility parameter once the link between facility and patient has been implemented.
            }

            if (!string.IsNullOrWhiteSpace(puid))
            {
                Guid patientGuid = default(Guid);

                if (Guid.TryParse(puid, out patientGuid))
                {
                    patientPredicate = patientPredicate.And(p => p.PatientGuid == patientGuid);
                }
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                patientPredicate = patientPredicate.And(p => p.FirstName.StartsWith(firstName));
            }

            if (!string.IsNullOrWhiteSpace(surname))
            {
                patientPredicate = patientPredicate.And(p => p.Surname.StartsWith(surname));
            }

            var patientSearchResults = await unitOfwork.Repository<Patient>()
                .Queryable()
                .Where(patientPredicate.Expand())
                .ToArrayAsync();

            var patients = patientSearchResults
                .Select(patient => new PatientDTO
                {
                    PatientUniqueIdentifier = patient.PatientGuid,
                    PatientFirstName = patient.FirstName,
                    PatientLastName = patient.Surname,
                    PatientDateOfBirth = patient.DateOfBirth.GetValueOrDefault(DateTime.MinValue)
                }).ToArray();

            return Ok(patients);
        }

        // GET api/<controller>/5
        [ResponseType(typeof(PatientDTO))]
        public IHttpActionResult Get(int id)
        {
            var patient = unitOfwork.Repository<Patient>()
                .Queryable()
                .SingleOrDefault(p => p.Id == id);

            var patientDto = new PatientDTO 
            { 
                PatientId = patient.Id,
                PatientUniqueIdentifier = patient.PatientGuid, 
                PatientFirstName = patient.FirstName, 
                PatientLastName = patient.Surname, 
                CreatedBy = patient.CreatedBy.FullName,
                PatientCreatedDate = patient.Created,
                UpdatedBy = patient.UpdatedBy != null ? patient.UpdatedBy.FullName: "",
                PatientUpdatedDate = patient.LastUpdated.GetValueOrDefault(DateTime.MinValue),
                PatientDateOfBirth = patient.DateOfBirth.GetValueOrDefault(DateTime.MinValue),
                Notes = patient.Notes,
                customAttributes = getCustomAttributes(patient)
            };                                  

            return Ok(patientDto);
        }

        

        // POST api/<controller>
        [ResponseType(typeof(PatientDTO))]
        public async Task<IHttpActionResult> Post([FromBody]PatientDTO value)
        {
            Patient patient = null;

            if(value.PatientUniqueIdentifier != null)
            {
                 patient = await unitOfwork.Repository<Patient>()
                .Queryable()
                .SingleOrDefaultAsync(p => p.PatientGuid == value.PatientUniqueIdentifier);
            }

            if(patient == null)
            {
                patient = new Patient
                {
                    PatientGuid = Guid.NewGuid(),
                    FirstName = value.PatientFirstName,
                    Surname = value.PatientLastName,
                    DateOfBirth = value.PatientDateOfBirth,
                    Notes = value.Notes

                };

                setCustomAttributes(value.customAttributes, patient);
                unitOfwork.Repository<Patient>().Save(patient);
            }
            else
            {
                patient.FirstName = value.PatientFirstName;
                patient.Surname = value.PatientLastName;
                patient.DateOfBirth = value.PatientDateOfBirth;
                patient.Notes = value.Notes;
                setCustomAttributes(value.customAttributes, patient);
                unitOfwork.Repository<Patient>().Update(patient);
            }

            unitOfwork.Complete();

            var patientDto = new PatientDTO
            {
                PatientId = patient.Id,
                PatientUniqueIdentifier = patient.PatientGuid,
                PatientFirstName = patient.FirstName,
                PatientLastName = patient.Surname,
                CreatedBy = patient.CreatedBy.FullName,
                PatientCreatedDate = patient.Created,
                UpdatedBy = patient.UpdatedBy != null? patient.UpdatedBy.FullName: "",
                PatientUpdatedDate = patient.LastUpdated.GetValueOrDefault(DateTime.MinValue),
                PatientDateOfBirth = patient.DateOfBirth.GetValueOrDefault(DateTime.MinValue),
                Notes = patient.Notes,
                customAttributes = getCustomAttributes(patient)
            };

            return Ok(patientDto);

        }

        [HttpPost]
        public HttpResponseMessage PostAll(JObject data)
        {
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NoContent, "No records to process ");

            dynamic json = data;

            List<Patient> synchedPatients = new List<Patient>();

            IList<PatientDTO> patients = ((JArray)json.items)
                .Select(t => new PatientDTO
                {
                    PatientUniqueIdentifier = ((dynamic)t).PatientUniqueIdentifier,
                    PatientFirstName = ((dynamic)t).PatientFirstName,
                    PatientLastName = ((dynamic)t).PatientLastName,
                    PatientDateOfBirth = ((dynamic)t).PatientDateOfBirth,
                    PatientMiddleName = ((dynamic)t).PatientMiddleName,
                    Notes = ((dynamic)t).Notes,
                    CreatedBy = ((dynamic)t).CreatedBy,
                    UpdatedBy = ((dynamic)t).UpdatedBy,
                    PatientCreatedDate = ((dynamic)t).PatientCreatedDate,
                    PatientUpdatedDate = ((dynamic)t).PatientUpdatedDate != "" ? ((dynamic)t).PatientUpdatedDate : null,
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

            foreach (PatientDTO patient in patients)
            {
                var obj = unitOfwork.Repository<Patient>()
                    .Queryable()
                    .SingleOrDefault(e => e.PatientGuid == patient.PatientUniqueIdentifier);

                if (obj == null)
                {

                    obj = new Patient()
                    {
                        PatientGuid = patient.PatientUniqueIdentifier,
                        FirstName = patient.PatientFirstName,
                        Surname = patient.PatientLastName,
                        Notes = patient.Notes,
                        DateOfBirth = patient.PatientDateOfBirth,
                        MiddleName = patient.PatientMiddleName,
                    };

                    setCustomAttributes(patient.customAttributes, obj);

                    unitOfwork.Repository<Patient>().Save(obj);
                    synchedPatients.Add(obj);
                }
                else
                {
                    obj.FirstName = patient.PatientFirstName;
                    obj.Surname = patient.PatientLastName;
                    obj.Notes = patient.Notes;
                    obj.DateOfBirth = patient.PatientDateOfBirth;
                    obj.MiddleName = patient.PatientMiddleName;

                    setCustomAttributes(patient.customAttributes, obj);

                    unitOfwork.Repository<Patient>().Update(obj);
                    synchedPatients.Add(obj);
                }
            }

            unitOfwork.Complete();

            var insertedObjs = synchedPatients.Select(p => new PatientDTO
            {
                PatientId = p.Id,
                PatientUniqueIdentifier = p.PatientGuid,
                PatientFirstName = p.FirstName,
                PatientMiddleName = p.MiddleName,
                PatientLastName = p.Surname,
                FacilityId = p.GetCurrentFacility().Facility.Id,
                CreatedBy = p.CreatedBy.FullName,
                PatientCreatedDate = p.Created,
                UpdatedBy = p.UpdatedBy != null ? p.UpdatedBy.FullName : string.Empty,
                PatientUpdatedDate = p.LastUpdated.GetValueOrDefault(DateTime.MinValue),
                PatientDateOfBirth = p.DateOfBirth.GetValueOrDefault(DateTime.MinValue),
                Notes = p.Notes,
                customAttributes = getCustomAttributes(p)
            }).ToArray();

            return Request.CreateResponse(HttpStatusCode.OK, insertedObjs);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
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

        private void setCustomAttributes(IEnumerable<CustomAttributeDTO> customAttributes, Patient patient)
        {
            if (customAttributes != null)
            {
                var patientConditionExtended = (IExtendable)patient;

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

        private List<CustomAttributeDTO> getCustomAttributes(Patient patient)
        {
            var extendable = (IExtendable)patient;

            var customAttributes = unitOfwork.Repository<PVIMS.Core.Entities.CustomAttributeConfiguration>()
              .Queryable()
              .Where(ca => ca.ExtendableTypeName == "Patient")
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