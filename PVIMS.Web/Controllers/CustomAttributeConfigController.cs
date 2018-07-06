using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using VPS.Common.Exceptions;
using VPS.Common.Repositories;
using VPS.Common.Utilities;

using PVIMS.Core.Services;
using PVIMS.Core.Models;
using PVIMS.Core.Entities;

namespace PVIMS.Web.Controllers
{
    public class CustomAttributeConfigController : BaseController
    {
        private static string CurrentMenuItem = "AdminCustom";

        private readonly ICustomAttributeService customAttributeService;
        private readonly IUnitOfWorkInt _unitOfWork;

        public CustomAttributeConfigController(ICustomAttributeService customAttributeService, IUnitOfWorkInt unitOfWork)
        {
            Check.IsNotNull(customAttributeService, "customAttributeService may not be null");
            Check.IsNotNull(unitOfWork, "unitOfWork may not be null");

            this.customAttributeService = customAttributeService;
            _unitOfWork = unitOfWork;
        }

        //
        // GET: /CustomAttributeConfig/
        public ActionResult Index()
        {
            return RedirectToAction("ViewExtendableEntities");
        }

        [HttpGet]
        public ActionResult ViewExtendableEntities()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewData.Model = customAttributeService.ListExtendableEntities();

            return View();
        }

        [HttpGet]
        public ActionResult ViewCustomAttributes(string entityName)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var customAttributes = customAttributeService.ListCustomAttributes(entityName);

            ViewBag.Categories = customAttributes.Select(a => a.Category).Distinct();

            ViewData.Model = customAttributes;

            ViewBag.EntityName = entityName;

            return View();
        }

        [HttpGet]
        public ActionResult ViewSelectionDataItems(string AttributeName)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            IList<SelectionDataItemDetail> selectionDataItems = customAttributeService.ListSelectionDataItems(AttributeName);

            ViewBag.AttributeKey = AttributeName;
            ViewData.Model = selectionDataItems;

            return View();
        }

        [HttpGet]
        public ActionResult ViewCustomAttribute(string entityName, string attributeName)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var config = _unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Single(ca => ca.ExtendableTypeName == entityName && ca.AttributeKey == attributeName);

            List<SelectListItem> listItems = new List<SelectListItem>();
            SelectListItem listItem = new SelectListItem { Text = "Numeric", Value = "1" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "String", Value = "2" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "Selection", Value = "3" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "DateTime", Value = "4" };
            listItems.Add(listItem);

            ViewBag.CustomAttributeTypes = listItems;

            ViewData.Model = new CustomAttributeConfigDetail { EntityName = entityName, Category = config.Category, AttributeDetail = config.AttributeDetail, CustomAttributeType = config.CustomAttributeType, Required = config.IsRequired, Searchable = config.IsSearchable, NumericMaxValue = config.NumericMaxValue, NumericMinValue = config.NumericMinValue, FutureDateOnly = config.FutureDateOnly, PastDateOnly = config.PastDateOnly, StringMaxLength = config.StringMaxLength };

            return View();
        }

        [HttpGet]
        public ActionResult AddCustomAttribute(string entityName)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            List<SelectListItem> listItems = new List<SelectListItem>();
            SelectListItem listItem = new SelectListItem { Text = "Numeric", Value = "1" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "String", Value = "2" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "Selection", Value = "3" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "DateTime", Value = "4" };
            listItems.Add(listItem);
                                
            ViewBag.CustomAttributeTypes = listItems;

            ViewData.Model = new CustomAttributeConfigDetail { EntityName = entityName };
            
            return View();
        }

        [HttpPost]
        public ActionResult AddCustomAttribute(CustomAttributeConfigDetail customAttribute)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            TryValidateModel(customAttribute);
            if (ModelState.IsValid)
            {
                try
                {
                    customAttributeService.AddCustomAttribute(customAttribute);
                    return RedirectToAction("ViewCustomAttributes", new { entityName = customAttribute.EntityName });
                }
                catch (BusinessException bex)
                {
                    ModelState.AddModelError("Name", bex.Message);
                }
            }

            ViewData.Model = customAttribute;

            List<SelectListItem> listItems = new List<SelectListItem>();
            SelectListItem listItem = new SelectListItem { Text = "Numeric", Value = "1" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "String", Value = "2" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "Selection", Value = "3" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "DateTime", Value = "4" };
            listItems.Add(listItem);

            ViewBag.CustomAttributeTypes = listItems;

            return View();
        }

        [HttpGet]
        public ActionResult AddSelectionDataItem(string attributeKey)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            SelectionDataItemDetail newSelectionDataItem = new SelectionDataItemDetail();
            newSelectionDataItem.AttributeKey = attributeKey;
            ViewData.Model = newSelectionDataItem;

            return View();
        }

        [HttpPost]
        public ActionResult AddSelectionDataItem(SelectionDataItemDetail selectionDataItem)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            TryValidateModel(selectionDataItem);
            if (ModelState.IsValid)
            {
                try
                {
                    customAttributeService.AddSelectionDataItem(selectionDataItem);
                    return RedirectToAction("ViewSelectionDataItems", new { AttributeName = selectionDataItem.AttributeKey });
                }
                catch (BusinessException bex)
                {
                    ModelState.AddModelError("Name", bex.Message);
                }
            }

            ViewData.Model = selectionDataItem;

            return View();
        }

        [HttpGet]
        public ActionResult EditCustomAttribute(string entityName, string attributeName)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var config = _unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Single(ca => ca.ExtendableTypeName == entityName && ca.AttributeKey == attributeName);

            List<SelectListItem> listItems = new List<SelectListItem>();
            SelectListItem listItem = new SelectListItem { Text = "Numeric", Value = "1" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "String", Value = "2" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "Selection", Value = "3" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "DateTime", Value = "4" };
            listItems.Add(listItem);

            ViewBag.CustomAttributeTypes = listItems;

            ViewData.Model = new CustomAttributeConfigDetail { EntityName = entityName, Category = config.Category, AttributeDetail = config.AttributeDetail, CustomAttributeType = config.CustomAttributeType, Required = config.IsRequired, Searchable = config.IsSearchable, NumericMaxValue = config.NumericMaxValue, NumericMinValue = config.NumericMinValue, FutureDateOnly = config.FutureDateOnly, PastDateOnly = config.PastDateOnly, StringMaxLength = config.StringMaxLength };

            return View();
        }

        [HttpPost]
        public ActionResult EditCustomAttribute(CustomAttributeConfigDetail customAttribute)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            TryValidateModel(customAttribute);
            if (ModelState.IsValid)
            {
                try
                {
                    customAttributeService.UpdateCustomAttribute(customAttribute);
                    
                    return RedirectToAction("ViewCustomAttributes", new { entityName = customAttribute.EntityName });
                }
                catch (BusinessException bex)
                {
                    ModelState.AddModelError("Name", bex.Message);
                }
            }

            ViewData.Model = customAttribute;

            List<SelectListItem> listItems = new List<SelectListItem>();
            SelectListItem listItem = new SelectListItem { Text = "Numeric", Value = "1" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "String", Value = "2" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "Selection", Value = "3" };
            listItems.Add(listItem);
            listItem = new SelectListItem { Text = "DateTime", Value = "4" };
            listItems.Add(listItem);

            ViewBag.CustomAttributeTypes = listItems;

            return View();
        }

    }
}