using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly IUnitOfWorkInt _unitOfWork;

        public DatasetInstanceApiController(IUnitOfWorkInt unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        [WebApiUnitOfWork]
        public IHttpActionResult GetDatasetElementSubsForDatasetElementId(int id)
        {
            var datasetElement = _unitOfWork.Repository<DatasetElement>()
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

            var datasetInstance = _unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include(i => i.Dataset)
                .Include(i => i.DatasetInstanceValues.Select(i2 => i2.DatasetInstanceSubValues))
                .SingleOrDefault(di => di.Id == model.DatasetInstanceId);
            var datasetElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(u => u.Id == model.DatasetElementId);

            if (datasetInstance == null)
            {
                return NotFound();
            }

            var datasetElementSubIds = model.Values.Select(v => v.DatasetElementSubId).ToArray();

            var datasetElementSubs = _unitOfWork.Repository<DatasetElementSub>()
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

            _unitOfWork.Repository<DatasetInstance>().Update(datasetInstance);
            _unitOfWork.Complete();

            // Prepare response
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table class='table table-bordered table-striped table-responsive' style='width: 100 %;' id='{0}'>", model.DatasetElementId.ToString());
            sb.Append("<tr>");
            // Populate tablet headers
            foreach (DatasetElementSub elementSub in datasetElementSubs
                .Where(es1 => es1.System == false)
                .Take(6)
                .OrderBy(es2 => es2.FieldOrder))
            {
                sb.AppendFormat("<th>{0}</th>", elementSub.ElementName);
            }
            sb.Append("<th></th>");
            sb.Append("</tr>");
            // Now populate data
            var instanceSubValueGroups = _unitOfWork.Repository<DatasetInstanceValue>().Queryable()
                .Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == model.DatasetElementId)
                .DatasetInstanceSubValues.GroupBy(g => g.ContextValue)
                .Select(disv => new DatasetInstanceSubValueGroupingModel
                {
                    Context = disv.Key,
                    Values = disv.Select(v => new DatasetInstanceSubValueModel
                    {
                        DatasetElementSubId = v.DatasetElementSub.Id,
                        InstanceSubValueId = v.Id,
                        InstanceValue = v.InstanceValue,
                        InstanceValueType = (FieldTypes)v.DatasetElementSub.Field.FieldType.Id,
                        InstanceValueRequired = v.DatasetElementSub.Field.Mandatory
                    }).ToArray()
                }).ToArray();

            foreach (DatasetInstanceSubValueGroupingModel group in instanceSubValueGroups)
            {
                sb.Append("<tr>");
                foreach (DatasetElementSub elementSub in datasetElementSubs
                    .Where(es1 => es1.System == false)
                    .Take(6)
                    .OrderBy(es2 => es2.FieldOrder))
                {
                    var value = group.Values.SingleOrDefault(gv => gv.DatasetElementSubId == elementSub.Id);
                    sb.AppendFormat("<td>{0}</td>", value != null ? value.InstanceValue : "");
                }
                // Action button
                sb.Append("<td><div class='btn-group'><button data-toggle='dropdown' class='btn btn-default btn-sm dropdown-toggle'>Action<span class='caret'></span></button><ul class='dropdown-menu pull-right'>");
                sb.AppendFormat("<li><a data-toggle='modal' data-target='#editDatasetElementSubModal' data-id='{0}' data-context='{1}' data-datasetinstance='{2}' data-original-title='Edit {3} item'> Edit {3} item </a></li>", model.DatasetElementId, group.Context.ToString(), model.DatasetInstanceId, datasetElement.ElementName);
                sb.AppendFormat("<li><a data-toggle='modal' data-target='#deleteDatasetElementSubModal' data-id='{0}' data-context='{1}' data-datasetinstance='{2}' data-original-title='Delete {3} item'> Delete {3} item </a></li>", model.DatasetElementId, group.Context.ToString(), model.DatasetInstanceId, datasetElement.ElementName);
                sb.Append("</ul></div></td>");

                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return Ok<string>(sb.ToString());
        }

        [WebApiUnitOfWork]
        [HttpPost]
        public IHttpActionResult DeleteDatasetInstanceSubValues(DatasetInstanceSubValuesSaveModel model)
        {
            var datasetInstanceSubValueRepository = _unitOfWork.Repository<DatasetInstanceSubValue>();

            var datasetInstance = _unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include(i => i.Dataset)
                .Include(i => i.DatasetInstanceValues.Select(i2 => i2.DatasetInstanceSubValues))
                .SingleOrDefault(di => di.Id == model.DatasetInstanceId);
            var datasetElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(u => u.Id == model.DatasetElementId);
            var context = model.SubValueContext;

            if (datasetInstance == null)
            {
                return NotFound();
            }

            var datasetElementSubIds = model.Values.Select(v => v.DatasetElementSubId).ToArray();

            var datasetElementSubs = _unitOfWork.Repository<DatasetElementSub>()
                .Queryable()
                .Where(des => datasetElementSubIds.Contains(des.Id))
                .ToList();

            var instanceSubValues = datasetInstance.GetInstanceSubValues(datasetElement, context);
            foreach (var instanceSubValue in instanceSubValues)
            {
                datasetInstanceSubValueRepository.Delete(instanceSubValue);
            }

            _unitOfWork.Complete();

            // Prepare response
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table class='table table-bordered table-striped table-responsive' style='width: 100 %;' id='{0}'>", model.DatasetElementId.ToString());
            sb.Append("<tr>");
            // Populate tablet headers
            foreach (DatasetElementSub elementSub in datasetElementSubs
                .Where(es1 => es1.System == false)
                .OrderBy(es2 => es2.FieldOrder)
                .Take(6))
            {
                sb.AppendFormat("<th>{0}</th>", elementSub.ElementName);
            }
            sb.Append("<th></th>");
            sb.Append("</tr>");
            // Now populate data
            var instanceSubValueGroups = _unitOfWork.Repository<DatasetInstanceValue>().Queryable()
                .Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == model.DatasetElementId)
                .DatasetInstanceSubValues.GroupBy(g => g.ContextValue)
                .Select(disv => new DatasetInstanceSubValueGroupingModel
                {
                    Context = disv.Key,
                    Values = disv.Select(v => new DatasetInstanceSubValueModel
                    {
                        DatasetElementSubId = v.DatasetElementSub.Id,
                        InstanceSubValueId = v.Id,
                        InstanceValue = v.InstanceValue,
                        InstanceValueType = (FieldTypes)v.DatasetElementSub.Field.FieldType.Id,
                        InstanceValueRequired = v.DatasetElementSub.Field.Mandatory
                    }).ToArray()
                }).ToArray();

            foreach (DatasetInstanceSubValueGroupingModel group in instanceSubValueGroups)
            {
                sb.Append("<tr>");
                foreach (DatasetElementSub elementSub in datasetElementSubs
                    .Where(es1 => es1.System == false)
                    .OrderBy(es2 => es2.FieldOrder)
                    .Take(6))
                {
                    var value = group.Values.SingleOrDefault(gv => gv.DatasetElementSubId == elementSub.Id);
                    sb.AppendFormat("<td>{0}</td>", value != null ? value.InstanceValue : "");
                }
                // Action button
                sb.Append("<td><div class='btn-group'><button data-toggle='dropdown' class='btn btn-default btn-sm dropdown-toggle'>Action<span class='caret'></span></button><ul class='dropdown-menu pull-right'>");
                sb.AppendFormat("<li><a data-toggle='modal' data-target='#editDatasetElementSubModal' data-id='{0}' data-context='{1}' data-datasetinstance='{2}' data-original-title='Edit {3} item'> Edit {3} item </a></li>", model.DatasetElementId, group.Context.ToString(), model.DatasetInstanceId, datasetElement.ElementName);
                sb.AppendFormat("<li><a data-toggle='modal' data-target='#deleteDatasetElementSubModal' data-id='{0}' data-context='{1}' data-datasetinstance='{2}' data-original-title='Delete {3} item'> Delete {3} item </a></li>", model.DatasetElementId, group.Context.ToString(), model.DatasetInstanceId, datasetElement.ElementName);
                sb.Append("</ul></div></td>");

                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return Ok<string>(sb.ToString());
        }

        [WebApiUnitOfWork]
        [HttpGet]
        public IHttpActionResult GetDatasetInstanceSubValues(int datasetInstanceId, int datasetElementId, Guid subValueContext)
        {
            var datasetElement = _unitOfWork.Repository<DatasetElement>()
                .Queryable()
                .Include(i => i.DatasetElementSubs.Select(i2 => i2.Field.FieldType))
                .Include(i => i.DatasetElementSubs.Select(i2 => i2.Field.FieldValues))
                .SingleOrDefault(de => de.Id == datasetElementId);

            var datasetInstanceSubValues = _unitOfWork.Repository<DatasetInstanceSubValue>()
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
