using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VPS.Common.Repositories;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;
using PVIMS.Core.ValueTypes;
using PVIMS.Core.Exceptions;
using PVIMS.Web.HttpResults;

namespace PVIMS.Web.Controllers
{
    public class DatasetInstanceApiController : ApiController
    {
        private readonly IUnitOfWorkInt unitOfWork;

        public DatasetInstanceApiController(IUnitOfWorkInt unitOfWork) 
        {
            this.unitOfWork = unitOfWork;
        }

        [WebApiUnitOfWork]
        public IHttpActionResult GetDatasetElementSubsForDatasetElementId(int id)
        {
            var datasetElement = unitOfWork.Repository<DatasetElement>()
                .Queryable()
                .Include(i => i.DatasetElementSubs.Select(i2 => i2.Field.FieldType))
                .Include(i => i.DatasetElementSubs.Select(i2 => i2.Field.FieldValues))
                .SingleOrDefault(de => de.Id == id);

            var items = datasetElement.DatasetElementSubs
                .Where(des => des.System == false)
                .OrderBy(o => o.FieldOrder)
                .Select(des => new
                {
                    DatasetElementSubId = des.Id,
                    DatasetElementSubName = des.ElementName,
                    DatasetElementSubDisplayName = String.IsNullOrWhiteSpace(des.FriendlyName) ? des.ElementName : des.FriendlyName,
                    DatasetElementSubHelp = des.Help,
                    DatasetElementSubType = des.Field.FieldType.Description,
                    DatasetElementSubRequired = des.Field.Mandatory,
                    FieldValues = des.Field.FieldValues.OrderBy(fv => fv.Value).Select(fv => new 
                    {
                        FieldValueId = fv.Id,
                        FieldValueName = fv.Value
                    })
                    .ToArray()
                })
                .ToArray();

            return Ok(items);
        }

        [WebApiUnitOfWork]
        [HttpPost]
        public IHttpActionResult SaveDatasetInstanceSubValues(DatasetInstanceSubValuesSaveModel model)
        {
            if (model.Values.All(v => v.Value == null) && model.SubValueContext == default(Guid))
            {
                // Nothing to do
                return Ok<string>("{ result: \"Ok\" }");
            }

            var errors = new Dictionary<string, string>();

            var datasetInstance = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include(i => i.Dataset)
                .Include(i => i.DatasetInstanceValues.Select(i2 => i2.DatasetInstanceSubValues))
                .SingleOrDefault(di => di.Id == model.DatasetInstanceId);

            if (datasetInstance == null)
            {
                return NotFound();
            }

            var datasetElementSubIds = model.Values.Select(v => v.DatasetElementSubId).ToArray();

            var datasetElementSubs = unitOfWork.Repository<DatasetElementSub>()
                .Queryable()
                .Where(des => datasetElementSubIds.Contains(des.Id))
                .ToList();

            var context = model.SubValueContext;
            DatasetInstanceSubValue datasetInstanceSubValue = null;
            int fieldValueId = 0;

            foreach (var datasetElementSub in datasetElementSubs)
            {
                var instanceSubValueModel = model.Values.SingleOrDefault(v => v.DatasetElementSubId == datasetElementSub.Id);

                if (instanceSubValueModel != null)
                {
                    switch ((FieldTypes)datasetElementSub.Field.FieldType.Id)
                    {
                        case FieldTypes.DropDownList:
                        case FieldTypes.Listbox:
                            if (int.TryParse(instanceSubValueModel.Value.ToString(), out fieldValueId))
                            {
                                var instanceSubValue = datasetElementSub.Field.FieldValues.SingleOrDefault(fv => fv.Id == fieldValueId);

                                if (instanceSubValue != null)
                                {
                                    try
                                    {
                                        datasetInstanceSubValue = datasetInstance.SetInstanceSubValue(datasetElementSub, instanceSubValue.Value, context);
                                    }
                                    catch (DatasetFieldSetException ex)
                                    {
                                        errors.Add(ex.Key, ex.Message);
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (datasetElementSub.Field.Mandatory)
                                    {
                                        errors.Add(datasetElementSub.Id.ToString(), string.Format("{0} is required.", datasetElementSub.ElementName));
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                if (datasetElementSub.Field.Mandatory)
                                {
                                    errors.Add(datasetElementSub.Id.ToString(), string.Format("{0} is required.", datasetElementSub.ElementName));
                                    continue;
                                }
                            }
                            break;
                        case FieldTypes.YesNo:
                        default:
                            try
                            {
                                datasetInstanceSubValue = datasetInstance.SetInstanceSubValue(datasetElementSub, instanceSubValueModel.Value.ToString(), context);
                            }
                            catch (DatasetFieldSetException ex)
                            {
                                errors.Add(ex.Key, ex.Message);
                                continue;
                            }
                            break;
                    }

                    if (datasetInstanceSubValue != null)
                    {
                        context = datasetInstanceSubValue.ContextValue;
                    }
                }
            }

            if (errors.Any())
            {
                return new ValidationErrorsResult(errors.Select(e => new ValidationError(e.Key.ToString(), e.Value)).ToArray());
            }

            unitOfWork.Repository<DatasetInstance>().Update(datasetInstance);
            unitOfWork.Complete();

            return Ok<string>("{ result: \"Ok\" }");
        }

        [WebApiUnitOfWork]
        [HttpGet]
        public IHttpActionResult GetDatasetInstanceSubValues(int datasetInstanceId, int datasetElementId, Guid subValueContext)
        {
            var datasetElement = unitOfWork.Repository<DatasetElement>()
                .Queryable()
                .Include(i => i.DatasetElementSubs.Select(i2 => i2.Field.FieldType))
                .Include(i => i.DatasetElementSubs.Select(i2 => i2.Field.FieldValues))
                .SingleOrDefault(de => de.Id == datasetElementId);

            var datasetInstanceSubValues = unitOfWork.Repository<DatasetInstanceSubValue>()
                .Queryable()
                .Where(disv => disv.DatasetElementSub.DatasetElement.Id == datasetElementId
                    && disv.ContextValue == subValueContext)
                .ToList();

            var items = datasetElement.DatasetElementSubs
                .Where(w => w.System == false)
                .OrderBy(o => o.FieldOrder)
                .Select(des => new
                {
                    DatasetElementSubId = des.Id,
                    DatasetElementSubName = des.ElementName,
                    DatasetElementSubDisplayName = String.IsNullOrWhiteSpace(des.FriendlyName) ? des.ElementName : des.FriendlyName,
                    DatasetElementSubHelp = des.Help,
                    DatasetElementSubType = des.Field.FieldType.Description,
                    DatasetElementSubRequired = des.Field.Mandatory,
                    DatasetInstanceSubValue = GetDatasetInstanceSubValue(datasetInstanceSubValues, des.Id),
                    FieldValues = des.Field.FieldValues.Select(fv => new
                    {
                        FieldValueId = fv.Id,
                        FieldValueName = fv.Value
                    })
                    .ToArray()
                })
                .ToArray();

            return Ok(items);
        }

        private string GetDatasetInstanceSubValue(List<DatasetInstanceSubValue> source, int datasetElementSubId)
        {
            var datasetInstanceSubValue = source.SingleOrDefault(s => s.DatasetElementSub.Id == datasetElementSubId);

            return datasetInstanceSubValue == null ? string.Empty : datasetInstanceSubValue.InstanceValue;
        }
    }
}
