using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VPS.Common.Repositories;
using VPS.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;
using System.Data.Entity;

namespace PVIMS.Web.Controllers
{
    public class PatientLabTestApiController : ApiController
    {

        private readonly IUnitOfWorkInt unitOfWork;

        public PatientLabTestApiController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/PatientLabTestApi
        public async Task<IEnumerable<PatientLabTestDTO>> Get()
        {
            return await Task.Run(() => unitOfWork.Repository<PatientLabTest>()
                .Queryable()
                .Include(p => p.TestUnit)
                .ToList()
                .Select(p => new PatientLabTestDTO
                {
                   PatientId = p.Patient.Id,
                   PatientLabTestId = p.Id,
                   PatientLabTestIdentifier = p.PatientLabTestGuid,
                   TestName = p.LabTest != null ? p.LabTest.Description : "",
                   TestDate = p.TestDate,
                   TestResult = p.TestResult,
                   LabValue = p.LabValue,
                   TestUnit = p.TestUnit != null ? p.TestUnit.Id : 0,
                   customAttributes = getCustomAttributes(p)
                }).ToArray());
        }

        [ActionName("getLabTestType")]
        public async Task<IEnumerable<System.Web.Mvc.SelectListItem>> GetLabTestType()
        {
            return await Task.Run(() => unitOfWork.Repository<LabTest>()
                .Queryable()
                .Select(c => new System.Web.Mvc.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
        }

        [ActionName("getLabTestUnit")]
        public async Task<IEnumerable<Object>> GetLabTestUnits()
        {
            return await Task.Run(() => unitOfWork.Repository<LabTestUnit>()
                .Queryable()
                .Select(c => new 
                {
                    Id = c.Id,
                    Description = c.Description
                })
                .ToArray());
        }

        [HttpPost]
        public HttpResponseMessage Post(JObject items)
        {
            if (items == null)
                return Request.CreateResponse(HttpStatusCode.NoContent, "No records to process ");

            dynamic json = items;

            List<PatientLabTest> synchedPatientLabTest = new List<PatientLabTest>();

            IList<PatientLabTestDTO> patientLabTests = ((JArray)json.items)
                .Select(t => new PatientLabTestDTO
                {
                    PatientId = ((dynamic)t).PatientId,
                    PatientLabTestId = ((dynamic)t).PatientLabTestId,
                    PatientLabTestIdentifier = ((dynamic)t).PatientLabTestIdentifier,
                    TestDate = ((dynamic)t).TestDate != null ? ((dynamic)t).TestDate : null,
                    TestName = ((dynamic)t).TestName ,
                    TestResult = (((dynamic)t).TestResult != null && ((dynamic)t).TestResult != "") ? ((dynamic)t).TestResult : null,
                    TestUnit = (((dynamic)t).TestUnit != null && ((dynamic)t).TestUnit != "") ? ((dynamic)t).TestUnit : default(decimal?),
                    LabValue = (((dynamic)t).LabValue != null && ((dynamic)t).LabValue != "") ? ((dynamic)t).LabValue : default(int?),
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

            var labTestUnits = unitOfWork.Repository<LabTestUnit>().Queryable().ToList();
            var labTests = unitOfWork.Repository<LabTest>().Queryable().ToList();

            foreach (PatientLabTestDTO labTest in patientLabTests)
            {
                PatientLabTest obj = unitOfWork.Repository<PatientLabTest>()
                    .Queryable()
                    .SingleOrDefault(e => e.PatientLabTestGuid == labTest.PatientLabTestIdentifier);

                if (obj == null)
                {
                    obj = new PatientLabTest
                    {
                        PatientLabTestGuid = labTest.PatientLabTestIdentifier,
                        Patient = unitOfWork.Repository<Patient>().Queryable().SingleOrDefault(e => e.Id == labTest.PatientId),
                        //Id = labTest.PatientLabTestId,
                        TestDate = labTest.TestDate,
                        TestResult = labTest.TestResult,
                        LabValue = labTest.LabValue,
                        LabTest = labTests.SingleOrDefault(e => e.Description == labTest.TestName),
                        TestUnit = labTestUnits.SingleOrDefault(e => e.Id == labTest.TestUnit)
                    };

                    setCustomAttributes(labTest.customAttributes, obj);

                    unitOfWork.Repository<PatientLabTest>().Save(obj);
                    synchedPatientLabTest.Add(obj);
                }
                else
                {
                    obj.TestDate = labTest.TestDate;
                    obj.TestResult = labTest.TestResult;
                    obj.LabValue = labTest.LabValue;
                    obj.LabTest = labTests.SingleOrDefault(e => e.Description == labTest.TestName);
                    obj.TestUnit = labTestUnits.SingleOrDefault(e => e.Id == labTest.TestUnit);

                    setCustomAttributes(labTest.customAttributes, obj);
                    synchedPatientLabTest.Add(obj);
                    unitOfWork.Repository<PatientLabTest>().Update(obj);
                }
            }

            unitOfWork.Complete();

            var insertedObjs = synchedPatientLabTest.Select(p => new PatientLabTestDTO
            {
                PatientId = p.Patient.Id,
                PatientLabTestId = p.Id,
                PatientLabTestIdentifier = p.PatientLabTestGuid,
                TestName = p.LabTest != null ? p.LabTest.Description : "",
                TestDate = p.TestDate,
                TestResult = p.TestResult,
                LabValue = p.LabValue,
                TestUnit = p.TestUnit != null ? p.TestUnit.Id : 0,
                customAttributes = getCustomAttributes(p)
            }).ToArray();

            return Request.CreateResponse(HttpStatusCode.OK, insertedObjs);
        }

        // GET: api/PatientLabTestApi/5
        public string Get(int id)
        {
            return "value";
        }

        // PUT: api/PatientLabTestApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PatientLabTestApi/5
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

        private void setCustomAttributes(IEnumerable<CustomAttributeDTO> customAttributes, PatientLabTest labTest)
        {
            if (customAttributes != null)
            {
                var labTestConditionExtended = (IExtendable)labTest;

                foreach (var customAttribute in customAttributes)
                {

                    switch (customAttribute.AttributeTypeName)
                    {
                        case "Numeric":
                            decimal number = 0M;
                            if (decimal.TryParse(customAttribute.currentValue, out number))
                            {
                                labTestConditionExtended.SetAttributeValue(customAttribute.AttributeName, number, User.Identity.Name);
                            }
                            break;
                        case "Selection":
                            Int32 selection = 0;
                            if (Int32.TryParse(customAttribute.currentValue, out selection))
                            {
                                labTestConditionExtended.SetAttributeValue(customAttribute.AttributeName, selection, User.Identity.Name);
                            }
                            break;
                        case "DateTime":
                            DateTime parsedDate = DateTime.MinValue;
                            if (DateTime.TryParse(customAttribute.currentValue, out parsedDate))
                            {
                                labTestConditionExtended.SetAttributeValue(customAttribute.AttributeName, parsedDate, User.Identity.Name);
                            }
                            break;
                        case "String":
                        default:
                            labTestConditionExtended.SetAttributeValue(customAttribute.AttributeName, customAttribute.currentValue ?? string.Empty, User.Identity.Name);
                            break;
                    }
                }
            }
        }

        private List<CustomAttributeDTO> getCustomAttributes(PatientLabTest labTest)
        {
            var extendable = (IExtendable)labTest;

            var customAttributes = unitOfWork.Repository<PVIMS.Core.Entities.CustomAttributeConfiguration>()
              .Queryable()
              .Where(ca => ca.ExtendableTypeName == typeof(PatientLabTest).Name)
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
