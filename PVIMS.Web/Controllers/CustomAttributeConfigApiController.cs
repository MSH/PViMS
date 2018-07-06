using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VPS.Common.Repositories;
using VPS.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;

namespace PVIMS.Web.Controllers
{
    public class CustomAttributeConfigApiController : ApiController
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> customAttributeConfigRepository;
        private readonly IRepositoryInt<SelectionDataItem> selectionDataRepository;

        public CustomAttributeConfigApiController(IUnitOfWorkInt unitOfWork)
        {
            this.customAttributeConfigRepository = unitOfWork.Repository<CustomAttributeConfiguration>();
            this.selectionDataRepository = unitOfWork.Repository<SelectionDataItem>();
        }

        // GET: api/CustomAttributeConfigApi
        public IEnumerable<CustomAttributeConfigListItem> Get()
        {
            var customAttributes = customAttributeConfigRepository.List();

            IList<CustomAttributeConfigListItem> attributesOfEntity =
                (from c in customAttributes                 
                 select new CustomAttributeConfigListItem
                 {
                     CustomAttributeConfigId = c.Id,
                     EntityName = c.ExtendableTypeName,
                     Category = c.Category,
                     AttributeName = c.AttributeKey,
                     Required = c.IsRequired,
                     NumericMaxValue = c.NumericMaxValue,
                     NumericMinValue = c.NumericMinValue,
                     StringMaxLength = c.StringMaxLength,
                     FutureDateOnly = c.FutureDateOnly,
                     PastDateOnly = c.PastDateOnly,
                     AttributeTypeName = c.CustomAttributeType == CustomAttributeType.Numeric ? "Numeric" : c.CustomAttributeType == CustomAttributeType.String ? "Text" : c.CustomAttributeType == CustomAttributeType.DateTime ? "Date" : c.CustomAttributeType == CustomAttributeType.Selection ? "Selection" : ""
                 }).ToList();

            return attributesOfEntity;
        }

        [ActionName("filterCustomAttribute")]
        public IEnumerable<CustomAttributeConfigListItem> Get(string entityName)
        {
            var customAttributes = customAttributeConfigRepository.List();

            IList<CustomAttributeConfigListItem> attributesOfEntity =
                (from c in customAttributes
                 where c.ExtendableTypeName == entityName
                 select new CustomAttributeConfigListItem
                 {
                     CustomAttributeConfigId = c.Id,
                     EntityName = entityName,
                     Category = c.Category,
                     AttributeName = c.AttributeKey,
                     Required = c.IsRequired,
                     NumericMaxValue = c.NumericMaxValue,
                     NumericMinValue = c.NumericMinValue,
                     StringMaxLength = c.StringMaxLength,
                     FutureDateOnly = c.FutureDateOnly,
                     PastDateOnly = c.PastDateOnly,
                     AttributeTypeName = c.CustomAttributeType == CustomAttributeType.Numeric ? "Numeric" : c.CustomAttributeType == CustomAttributeType.String ? "Text" : c.CustomAttributeType == CustomAttributeType.DateTime ? "Date" : c.CustomAttributeType == CustomAttributeType.Selection ? "Selection" : ""
                 }).ToList();

            return attributesOfEntity;
        }

        [ActionName("selectiondata")]
        [HttpGet]
        public IEnumerable<SelectionDataItemDetail> ListSelectionDataItems()
        {
            var referenceData = selectionDataRepository.List();
                
            IList<SelectionDataItemDetail> selectionDataItems =
                (from item in referenceData
                 select new SelectionDataItemDetail
                 {
                     SelectionDataItemId = item.Id,
                     AttributeKey = item.AttributeKey,
                     SelectionKey = item.SelectionKey,
                     DataItemValue = item.Value
                 }).ToList();

            return selectionDataItems;
        }

        [ActionName("selectiondata")]
        [HttpGet]
        public IEnumerable<SelectionDataItemDetail> ListSelectionDataItems(string attributeName)
        {
            var referenceData = selectionDataRepository.Queryable()
                .Where(di => di.AttributeKey == attributeName)
                .ToList();

            IList<SelectionDataItemDetail> selectionDataItems =
                (from item in referenceData
                 select new SelectionDataItemDetail
                 {
                     SelectionDataItemId = item.Id,
                     AttributeKey = item.AttributeKey,
                     SelectionKey = item.SelectionKey,
                     DataItemValue = item.Value
                 }).ToList();

            return selectionDataItems;
        }

        // GET: api/CustomAttributeConfigApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CustomAttributeConfigApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CustomAttributeConfigApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CustomAttributeConfigApi/5
        public void Delete(int id)
        {
        }
    }
}
