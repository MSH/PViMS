using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Web.ActionFilters;
using PVIMS.Web.Models;

using PVIMS.Core.Exceptions;
using PVIMS.Core.Services;
using PVIMS.Core.Models;

namespace PVIMS.Web.Controllers
{
    public class PublicController : Controller
    {
        private static string CurrentMenuItem = "SpontaneousReporting"; 

        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workflowService;

        public PublicController(IUnitOfWorkInt unitOfWork, IWorkFlowService workFlowService)
        {
            _unitOfWork = unitOfWork;
            _workflowService = workFlowService;
        }

        [MvcUnitOfWork]
        public ActionResult Index()
        {
            var encounters = _unitOfWork.Repository<Encounter>()
                .Queryable()
                .Include(p => p.Patient)
                .Include(et => et.EncounterType)
                .ToList()
                .Select(e => new EncounterListItem
                {
                    EncounterId = e.Id,
                    PatientFullName = e.Patient.FullName,
                    EncounterType = e.EncounterType.Description,
                    EncounterDate = e.EncounterDate
                })
                .ToList();

            return View(encounters);
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult AddSpontaneous()
        {
            var instanceModel = new DatasetInstanceModel { };

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = 0;

            // Create a new instance
            var dataset = _unitOfWork.Repository<Dataset>()
                .Queryable()
                .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType")
                .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType")
                .SingleOrDefault(d => d.DatasetName == "Spontaneous Report");

            if (dataset != null)
            {
                var datasetInstance = dataset.CreateInstance(1, null);
                _unitOfWork.Repository<DatasetInstance>().Save(datasetInstance);

                instanceModel.DatasetInstanceId = datasetInstance.Id;

                bool[] validCat;
                bool[] validMan;
                var rule = dataset.GetRule(DatasetRuleType.MandatoryFieldsProminent);
                if(rule.RuleActive)
                {
                    validCat = new[] { true, false };
                    validMan = new[] { true, false };
                }
                else
                {
                    validCat = new[] { false };
                    validMan = new[] { true, false };
                }
                
                var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories.Where(dc => validCat.Contains(dc.System)).OrderBy(dc => dc.CategoryOrder)
                    .SelectMany(dc => dc.DatasetCategoryElements).Where(dce => (rule.RuleActive && (dce.DatasetCategory.System == true && dce.DatasetElement.Field.Mandatory == true) || (dce.DatasetCategory.System == false && dce.DatasetElement.Field.Mandatory == false)) || (!rule.RuleActive && validMan.Contains(dce.DatasetElement.Field.Mandatory))).OrderBy(dce => dce.FieldOrder)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                instanceModel.DatasetCategories = groupedDatasetCategoryElements
                    .Select(dsc => new DatasetCategoryEditModel
                    {
                        DatasetCategoryId = dsc.Key.Id,
                        DatasetCategoryDisplayName = String.IsNullOrWhiteSpace(dsc.Key.FriendlyName) ? dsc.Key.DatasetCategoryName : dsc.Key.FriendlyName,
                        DatasetCategoryHelp = dsc.Key.Help,
                        DatasetElements = dsc.Select(e => new DatasetElementEditModel
                        {
                            DatasetElementId = e.DatasetElement.Id,
                            DatasetElementName = e.DatasetElement.ElementName,
                            DatasetElementDisplayName = String.IsNullOrWhiteSpace(e.FriendlyName) ? e.DatasetElement.ElementName : e.FriendlyName,
                            DatasetElementHelp = e.Help,
                            DatasetElementRequired = e.DatasetElement.Field.Mandatory,
                            DatasetElementDisplayed = true,
                            DatasetElementChronic = false,
                            DatasetElementType = e.DatasetElement.Field.FieldType.Description,
                            DatasetElementValue = datasetInstance.GetInstanceValue(e.DatasetElement),
                            DatasetElementSubs = e.DatasetElement.DatasetElementSubs.Select(es => new DatasetElementSubEditModel
                            {
                                DatasetElementSubId = es.Id,
                                DatasetElementSubName = es.ElementName,
                                DatasetElementSubRequired = es.Field.Mandatory,
                                DatasetElementSubType = es.Field.FieldType.Description//,
                                //DatasetElementSubValue = datasetInstance.GetInstanceSubValue(es)
                            }).ToArray(),
                            TableHeaderColumns = e.DatasetElement.DatasetElementSubs.Where(es1 => es1.System == false).Take(6).OrderBy(es2 => es2.FieldOrder).Select(des => new DatasetElementTableHeaderRowModel 
                            {
                                DatasetElementSubId = des.Id,
                                DatasetElementSubName = des.ElementName
                            }).ToArray(),
                            InstanceSubValues = e.DatasetElement.DatasetElementSubs.Count > 0 ? _unitOfWork.Repository<DatasetInstanceValue>().Queryable().Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == e.DatasetElement.Id).DatasetInstanceSubValues.GroupBy(g => g.ContextValue).Select(disv => new DatasetInstanceSubValueGroupingModel
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
                            }).ToArray() : null
                        })
                        .ToArray()
                    })
                    .ToArray();

                var selectTypeDatasetElements = instanceModel.DatasetCategories
                    .SelectMany(dc => dc.DatasetElements)
                    .Where(de => de.DatasetElementType == FieldTypes.Listbox.ToString()
                        || de.DatasetElementType == FieldTypes.DropDownList.ToString())
                    .ToArray();

                var yesNoDatasetElements = instanceModel.DatasetCategories
                    .SelectMany(dc => dc.DatasetElements)
                    .Where(de => de.DatasetElementType == FieldTypes.YesNo.ToString())
                    .ToArray();

                var datasetElementRepository = _unitOfWork.Repository<DatasetElement>();

                foreach (var element in selectTypeDatasetElements)
                {
                    var elementFieldValues = datasetElementRepository.Queryable()
                        .SingleOrDefault(de => de.Id == element.DatasetElementId)
                        .Field.FieldValues
                        .ToList();

                    var elementFieldValueList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
                    elementFieldValueList.AddRange(elementFieldValues.Select(ev => new SelectListItem { Value = ev.Value, Text = ev.Value, Selected = element.DatasetElementValue == ev.Value }));

                    ViewData.Add(element.DatasetElementName, elementFieldValueList.ToArray());
                }

                foreach (var element in yesNoDatasetElements)
                {
                    var yesNo = new[] { new SelectListItem { Value = "", Text = "" }, new SelectListItem { Value = "No", Text = "No" }, new SelectListItem { Value = "Yes", Text = "Yes" } };

                    var selectedYesNo = yesNo.SingleOrDefault(yn => yn.Value == element.DatasetElementValue);
                    if (selectedYesNo != null)
                    {
                        selectedYesNo.Selected = true;
                    }

                    ViewData.Add(element.DatasetElementName, yesNo);
                }
            }

            return View(instanceModel);
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult AddSpontaneous(DatasetInstanceModel model)
        {
            DatasetInstance datasetInstance = null;

            var datasetInstanceRepository = _unitOfWork.Repository<DatasetInstance>();

            datasetInstance = datasetInstanceRepository
                .Queryable()
                .Include(di => di.Dataset)
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub")
                .Include("DatasetInstanceValues.DatasetElement")
                .SingleOrDefault(di => di.Id == model.DatasetInstanceId);

            if (ModelState.IsValid)
            {
                if (datasetInstance == null)
                {
                    ViewBag.Entity = "DatasetInstance";
                    return View("NotFound");
                }

                datasetInstance.Status = DatasetInstanceStatus.COMPLETE;

                var datasetElementIds = model.DatasetCategories.SelectMany(dc => dc.DatasetElements.Select(dse => dse.DatasetElementId)).ToArray();

                var datasetElements = _unitOfWork.Repository<DatasetElement>()
                    .Queryable()
                    .Where(de => datasetElementIds.Contains(de.Id))
                    .ToDictionary(e => e.Id);

                var datasetElementSubs = datasetElements
                    .SelectMany(de => de.Value.DatasetElementSubs)
                    .ToDictionary(des => des.Id);

                try
                {
                    for (int i = 0; i < model.DatasetCategories.Length; i++)
                    {
                        for (int j = 0; j < model.DatasetCategories[i].DatasetElements.Length; j++)
                        {
                            try
                            {
                                if (model.DatasetCategories[i].DatasetElements[j].DatasetElementType != "Table")
                                {
                                    datasetInstance.SetInstanceValue(datasetElements[model.DatasetCategories[i].DatasetElements[j].DatasetElementId], model.DatasetCategories[i].DatasetElements[j].DatasetElementValue);
                                }
                            }
                            catch (DatasetFieldSetException ex)
                            {
                                // Need to rename the key in order for the message to be bound to the correct control.
                                throw new DatasetFieldSetException(string.Format("DatasetCategories[{0}].DatasetElements[{1}].DatasetElementValue", i, j), ex.Message);
                            }
                        }
                    }

                    datasetInstanceRepository.Update(datasetInstance);

                    // Instantiate new instance of work flow
                    var patientIdentifier = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Identification Number"));
                    if (String.IsNullOrWhiteSpace(patientIdentifier))
                    {
                        patientIdentifier = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Initials"));
                    }
                    var sourceIdentifier = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Description of reaction"));
                    _workflowService.CreateWorkFlowInstance("New Spontaneous Surveilliance Report", datasetInstance.DatasetInstanceGuid, patientIdentifier, sourceIdentifier);

                    // Prepare medications
                    List<ReportInstanceMedicationListItem> medications = new List<ReportInstanceMedicationListItem>();
                    var sourceProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Product Information");
                    var destinationProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Medicinal Products");
                    var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceProductElement);
                    foreach (Guid sourceContext in sourceContexts)
                    {
                        var drugItemValues = datasetInstance.GetInstanceSubValues(sourceProductElement, sourceContext);
                        var drugName = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue;

                        if (drugName != string.Empty)
                        {
                            var item = new ReportInstanceMedicationListItem()
                            {
                                MedicationIdentifier = drugName,
                                ReportInstanceMedicationGuid = sourceContext
                            };
                            medications.Add(item);
                        }
                    }
                    _workflowService.AddOrUpdateMedicationsForWorkFlowInstance(datasetInstance.DatasetInstanceGuid, medications);

                    _unitOfWork.Complete();

                    return RedirectToAction("FormAdded", "Public");
                }
                catch (DatasetFieldSetException dse)
                {
                    ModelState.AddModelError(dse.Key, dse.Message);
                }
            }

            bool[] validCat;
            bool[] validMan;
            var rule = datasetInstance.Dataset.GetRule(DatasetRuleType.MandatoryFieldsProminent);
            if (rule.RuleActive)
            {
                validCat = new[] { true, false };
                validMan = new[] { true, false };
            }
            else
            {
                validCat = new[] { false };
                validMan = new[] { true, false };
            }

            var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories.Where(dc => validCat.Contains(dc.System)).OrderBy(dc => dc.CategoryOrder)
                .SelectMany(dc => dc.DatasetCategoryElements).Where(dce => (rule.RuleActive && (dce.DatasetCategory.System == true && dce.DatasetElement.Field.Mandatory == true) || (dce.DatasetCategory.System == false && dce.DatasetElement.Field.Mandatory == false)) || (!rule.RuleActive && validMan.Contains(dce.DatasetElement.Field.Mandatory))).OrderBy(dce => dce.FieldOrder)
                .GroupBy(dce => dce.DatasetCategory)
                .ToList();

            model.DatasetCategories = groupedDatasetCategoryElements
                .Select(dsc => new DatasetCategoryEditModel
                {
                    DatasetCategoryId = dsc.Key.Id,
                    DatasetCategoryDisplayName = String.IsNullOrWhiteSpace(dsc.Key.FriendlyName) ? dsc.Key.DatasetCategoryName : dsc.Key.FriendlyName,
                    DatasetCategoryHelp = dsc.Key.Help,
                    DatasetElements = dsc.Select(e => new DatasetElementEditModel
                    {
                        DatasetElementId = e.DatasetElement.Id,
                        DatasetElementName = e.DatasetElement.ElementName,
                        DatasetElementDisplayName = String.IsNullOrWhiteSpace(e.FriendlyName) ? e.DatasetElement.ElementName : e.FriendlyName,
                        DatasetElementHelp = e.Help,
                        DatasetElementRequired = e.DatasetElement.Field.Mandatory,
                        DatasetElementDisplayed = true,
                        DatasetElementChronic = false,
                        DatasetElementType = e.DatasetElement.Field.FieldType.Description,
                        DatasetElementValue = datasetInstance.GetInstanceValue(e.DatasetElement),
                        DatasetElementSubs = e.DatasetElement.DatasetElementSubs.Select(es => new DatasetElementSubEditModel
                        {
                            DatasetElementSubId = es.Id,
                            DatasetElementSubName = es.ElementName,
                            DatasetElementSubRequired = es.Field.Mandatory,
                            DatasetElementSubType = es.Field.FieldType.Description//,
                            //DatasetElementSubValue = datasetInstance.GetInstanceSubValue(es)
                        }).ToArray()
                    })
                    .ToArray()
                })
                .ToArray();

            var selectTypeDatasetElements = model.DatasetCategories
                .SelectMany(dc => dc.DatasetElements)
                .Where(de => de.DatasetElementType == FieldTypes.Listbox.ToString()
                    || de.DatasetElementType == FieldTypes.DropDownList.ToString())
                .ToArray();

            var yesNoDatasetElements = model.DatasetCategories
                .SelectMany(dc => dc.DatasetElements)
                .Where(de => de.DatasetElementType == FieldTypes.YesNo.ToString())
                .ToArray();

            var datasetElementRepository = _unitOfWork.Repository<DatasetElement>();
            var datasetElementSubRepository = _unitOfWork.Repository<DatasetElementSub>();

            foreach (var element in selectTypeDatasetElements)
            {
                var elementFieldValues = datasetElementRepository.Queryable()
                    .SingleOrDefault(de => de.Id == element.DatasetElementId)
                    .Field.FieldValues
                    .ToList();

                var elementFieldValueList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
                elementFieldValueList.AddRange(elementFieldValues.Select(ev => new SelectListItem { Value = ev.Value, Text = ev.Value, Selected = element.DatasetElementValue == ev.Value }));

                ViewData.Add(element.DatasetElementName, elementFieldValueList.ToArray());
            }

            foreach (var element in yesNoDatasetElements)
            {
                var yesNo = new[] { new SelectListItem { Value = "", Text = "" }, new SelectListItem { Value = "No", Text = "No" }, new SelectListItem { Value = "Yes", Text = "Yes" } };

                var selectedYesNo = yesNo.SingleOrDefault(yn => yn.Value == element.DatasetElementValue);
                if (selectedYesNo != null)
                {
                    selectedYesNo.Selected = true;
                }

                ViewData.Add(element.DatasetElementName, yesNo);
            }

            return View(model);
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult FormAdded()
        {
            return View("");
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult EditSpontaneous(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = "/Analytical/SpontaneousReporting.aspx?id=" + id.ToString();
            TempData["returnUrl"] = returnUrl;
            ViewBag.ReturnUrl = returnUrl;

            var instanceModel = new DatasetInstanceModel { };

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = id;

            var datasetInstance = _unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Id == id).Single();

            if (datasetInstance != null)
            {
                var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories
                    .SelectMany(dc => dc.DatasetCategoryElements)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                instanceModel.DatasetCategories = groupedDatasetCategoryElements
                    .Select(dsc => new DatasetCategoryEditModel
                    {
                        DatasetCategoryId = dsc.Key.Id,
                        DatasetCategoryDisplayName = String.IsNullOrWhiteSpace(dsc.Key.FriendlyName) ? dsc.Key.DatasetCategoryName : dsc.Key.FriendlyName,
                        DatasetCategoryHelp = dsc.Key.Help,
                        DatasetElements = dsc.Select(e => new DatasetElementEditModel
                        {
                            DatasetElementId = e.DatasetElement.Id,
                            DatasetElementName = e.DatasetElement.ElementName,
                            DatasetElementDisplayName = String.IsNullOrWhiteSpace(e.FriendlyName) ? e.DatasetElement.ElementName : e.FriendlyName,
                            DatasetElementHelp = e.Help,
                            DatasetElementSystem = e.DatasetElement.System,
                            DatasetElementRequired = e.DatasetElement.Field.Mandatory,
                            DatasetElementDisplayed = true,
                            DatasetElementChronic = false,
                            DatasetElementType = e.DatasetElement.Field.FieldType.Description,
                            DatasetElementValue = datasetInstance.GetInstanceValue(e.DatasetElement),
                            DatasetElementSubs = e.DatasetElement.DatasetElementSubs.Select(es => new DatasetElementSubEditModel
                            {
                                DatasetElementSubId = es.Id,
                                DatasetElementSubName = es.ElementName,
                                DatasetElementSubRequired = es.Field.Mandatory,
                                DatasetElementSubType = es.Field.FieldType.Description//,
                                //DatasetElementSubValue = datasetInstance.GetInstanceSubValue(es)
                            }).ToArray()
                        })
                        .ToArray()
                    })
                    .ToArray();

                var selectTypeDatasetElements = instanceModel.DatasetCategories
                    .SelectMany(dc => dc.DatasetElements)
                    .Where(de => de.DatasetElementType == FieldTypes.Listbox.ToString()
                        || de.DatasetElementType == FieldTypes.DropDownList.ToString())
                    .ToArray();

                var yesNoDatasetElements = instanceModel.DatasetCategories
                    .SelectMany(dc => dc.DatasetElements)
                    .Where(de => de.DatasetElementType == FieldTypes.YesNo.ToString())
                    .ToArray();

                var datasetElementRepository = _unitOfWork.Repository<DatasetElement>();

                foreach (var element in selectTypeDatasetElements)
                {
                    var elementFieldValues = datasetElementRepository.Queryable()
                        .SingleOrDefault(de => de.Id == element.DatasetElementId)
                        .Field.FieldValues
                        .ToList();

                    var elementFieldValueList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
                    elementFieldValueList.AddRange(elementFieldValues.Select(ev => new SelectListItem { Value = ev.Value, Text = ev.Value, Selected = element.DatasetElementValue == ev.Value }));

                    ViewData.Add(element.DatasetElementName, elementFieldValueList.ToArray());
                }

                foreach (var element in yesNoDatasetElements)
                {
                    var yesNo = new[] { new SelectListItem { Value = "", Text = "" }, new SelectListItem { Value = "No", Text = "No" }, new SelectListItem { Value = "Yes", Text = "Yes" } };

                    var selectedYesNo = yesNo.SingleOrDefault(yn => yn.Value == element.DatasetElementValue);
                    if (selectedYesNo != null)
                    {
                        selectedYesNo.Selected = true;
                    }

                    ViewData.Add(element.DatasetElementName, yesNo);
                }
            }

            return View(instanceModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MvcUnitOfWork]
        public ActionResult EditSpontaneous(DatasetInstanceModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            DatasetInstance datasetInstance = null;

            var datasetInstanceRepository = _unitOfWork.Repository<DatasetInstance>();

            datasetInstance = datasetInstanceRepository
                .Queryable()
                .Include(di => di.Dataset)
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub")
                .Include("DatasetInstanceValues.DatasetElement")
                .SingleOrDefault(di => di.Id == model.DatasetInstanceId);

            if (datasetInstance == null)
            {
                ViewBag.Entity = "DatasetInstance";
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                var datasetElementIds = model.DatasetCategories.SelectMany(dc => dc.DatasetElements.Select(dse => dse.DatasetElementId)).ToArray();

                var datasetElements = _unitOfWork.Repository<DatasetElement>()
                    .Queryable()
                    .Where(de => datasetElementIds.Contains(de.Id))
                    .ToDictionary(e => e.Id);

                var datasetElementSubs = datasetElements
                    .SelectMany(de => de.Value.DatasetElementSubs)
                    .ToDictionary(des => des.Id);

                try
                {
                    for (int i = 0; i < model.DatasetCategories.Length; i++)
                    {
                        for (int j = 0; j < model.DatasetCategories[i].DatasetElements.Length; j++)
                        {
                            try
                            {
                                if (!model.DatasetCategories[i].DatasetElements[j].DatasetElementSystem)
                                {
                                    var eleVal = model.DatasetCategories[i].DatasetElements[j].DatasetElementValue != null ? model.DatasetCategories[i].DatasetElements[j].DatasetElementValue : string.Empty;
                                    if (eleVal != string.Empty)
                                    {
                                        datasetInstance.SetInstanceValue(datasetElements[model.DatasetCategories[i].DatasetElements[j].DatasetElementId], eleVal);
                                    }
                                }
                            }
                            catch (DatasetFieldSetException ex)
                            {
                                // Need to rename the key in order for the message to be bound to the correct control.
                                throw new DatasetFieldSetException(string.Format("DatasetCategories[{0}].DatasetElements[{1}].DatasetElementValue", i, j), ex.Message);
                            }
                        }
                    }

                    datasetInstanceRepository.Update(datasetInstance);

                    _unitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Spontaneous record updated successfully";
                    Response.Cookies.Add(cookie);

                    return Redirect("/Home/Index");
                }
                catch (DatasetFieldSetException dse)
                {
                    ModelState.AddModelError(dse.Key, dse.Message);
                }
            }

            var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories
                .SelectMany(dc => dc.DatasetCategoryElements)
                .GroupBy(dce => dce.DatasetCategory)
                .ToList();

            model.DatasetCategories = groupedDatasetCategoryElements
                .Select(dsc => new DatasetCategoryEditModel
                {
                    DatasetCategoryId = dsc.Key.Id,
                    DatasetCategoryDisplayName = String.IsNullOrWhiteSpace(dsc.Key.FriendlyName) ? dsc.Key.DatasetCategoryName : dsc.Key.FriendlyName,
                    DatasetCategoryHelp = dsc.Key.Help,
                    DatasetElements = dsc.Select(e => new DatasetElementEditModel
                    {
                        DatasetElementId = e.DatasetElement.Id,
                        DatasetElementName = e.DatasetElement.ElementName,
                        DatasetElementDisplayName = String.IsNullOrWhiteSpace(e.FriendlyName) ? e.DatasetElement.ElementName : e.FriendlyName,
                        DatasetElementHelp = e.Help,
                        DatasetElementSystem = e.DatasetElement.System,
                        DatasetElementRequired = e.DatasetElement.Field.Mandatory,
                        DatasetElementDisplayed = true,
                        DatasetElementChronic = false,
                        DatasetElementType = e.DatasetElement.Field.FieldType.Description,
                        DatasetElementValue = datasetInstance.GetInstanceValue(e.DatasetElement),
                        DatasetElementSubs = e.DatasetElement.DatasetElementSubs.Select(es => new DatasetElementSubEditModel
                        {
                            DatasetElementSubId = es.Id,
                            DatasetElementSubName = es.ElementName,
                            DatasetElementSubRequired = es.Field.Mandatory,
                            DatasetElementSubType = es.Field.FieldType.Description//,
                            //DatasetElementSubValue = datasetInstance.GetInstanceSubValue(es)
                        }).ToArray()
                    })
                    .ToArray()
                })
                .ToArray();

            var selectTypeDatasetElements = model.DatasetCategories
                .SelectMany(dc => dc.DatasetElements)
                .Where(de => de.DatasetElementType == FieldTypes.Listbox.ToString()
                    || de.DatasetElementType == FieldTypes.DropDownList.ToString())
                .ToArray();

            var yesNoDatasetElements = model.DatasetCategories
                .SelectMany(dc => dc.DatasetElements)
                .Where(de => de.DatasetElementType == FieldTypes.YesNo.ToString())
                .ToArray();

            var datasetElementRepository = _unitOfWork.Repository<DatasetElement>();
            var datasetElementSubRepository = _unitOfWork.Repository<DatasetElementSub>();

            foreach (var element in selectTypeDatasetElements)
            {
                var elementFieldValues = datasetElementRepository.Queryable()
                    .SingleOrDefault(de => de.Id == element.DatasetElementId)
                    .Field.FieldValues
                    .ToList();

                var elementFieldValueList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
                elementFieldValueList.AddRange(elementFieldValues.Select(ev => new SelectListItem { Value = ev.Value, Text = ev.Value, Selected = element.DatasetElementValue == ev.Value }));

                ViewData.Add(element.DatasetElementName, elementFieldValueList.ToArray());
            }

            foreach (var element in yesNoDatasetElements)
            {
                var yesNo = new[] { new SelectListItem { Value = "", Text = "" }, new SelectListItem { Value = "No", Text = "No" }, new SelectListItem { Value = "Yes", Text = "Yes" } };

                var selectedYesNo = yesNo.SingleOrDefault(yn => yn.Value == element.DatasetElementValue);
                if (selectedYesNo != null)
                {
                    selectedYesNo.Selected = true;
                }

                ViewData.Add(element.DatasetElementName, yesNo);
            }

            return View(model);
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult ViewSpontaneous(int id)
        {
            var instanceModel = new DatasetInstanceModel { };

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = 0;

            var datasetInstanceQuery = _unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Id == id);

            var datasetInstance = datasetInstanceQuery.SingleOrDefault();

            if (datasetInstance != null)
            {
                instanceModel.DatasetInstanceId = datasetInstance.Id;

                var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories
                    .SelectMany(dc => dc.DatasetCategoryElements)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                instanceModel.DatasetCategories = groupedDatasetCategoryElements
                    .Select(dsc => new DatasetCategoryEditModel
                    {
                        DatasetCategoryId = dsc.Key.Id,
                        DatasetCategoryDisplayName = String.IsNullOrWhiteSpace(dsc.Key.FriendlyName) ? dsc.Key.DatasetCategoryName : dsc.Key.FriendlyName,
                        DatasetCategoryHelp = dsc.Key.Help,
                        DatasetElements = dsc.Select(e => new DatasetElementEditModel
                        {
                            DatasetElementId = e.DatasetElement.Id,
                            DatasetElementName = e.DatasetElement.ElementName,
                            DatasetElementDisplayName = String.IsNullOrWhiteSpace(e.FriendlyName) ? e.DatasetElement.ElementName : e.FriendlyName,
                            DatasetElementHelp = e.Help,
                            DatasetElementRequired = e.DatasetElement.Field.Mandatory,
                            DatasetElementDisplayed = true,
                            DatasetElementChronic = false,
                            DatasetElementType = e.DatasetElement.Field.FieldType.Description,
                            DatasetElementValue = datasetInstance.GetInstanceValue(e.DatasetElement),
                            DatasetElementSubs = e.DatasetElement.DatasetElementSubs.Select(es => new DatasetElementSubEditModel
                            {
                                DatasetElementSubId = es.Id,
                                DatasetElementSubName = es.ElementName,
                                DatasetElementSubRequired = es.Field.Mandatory,
                                DatasetElementSubType = es.Field.FieldType.Description//,
                                //DatasetElementSubValue = datasetInstance.GetInstanceSubValue(es)
                            }).ToArray()
                        })
                        .ToArray()
                    })
                    .ToArray();

                var selectTypeDatasetElements = instanceModel.DatasetCategories
                    .SelectMany(dc => dc.DatasetElements)
                    .Where(de => de.DatasetElementType == FieldTypes.Listbox.ToString()
                        || de.DatasetElementType == FieldTypes.DropDownList.ToString())
                    .ToArray();

                var yesNoDatasetElements = instanceModel.DatasetCategories
                    .SelectMany(dc => dc.DatasetElements)
                    .Where(de => de.DatasetElementType == FieldTypes.YesNo.ToString())
                    .ToArray();

                var datasetElementRepository = _unitOfWork.Repository<DatasetElement>();

                foreach (var element in selectTypeDatasetElements)
                {
                    var elementFieldValues = datasetElementRepository.Queryable()
                        .SingleOrDefault(de => de.Id == element.DatasetElementId)
                        .Field.FieldValues
                        .ToList();

                    var elementFieldValueList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
                    elementFieldValueList.AddRange(elementFieldValues.Select(ev => new SelectListItem { Value = ev.Value, Text = ev.Value, Selected = element.DatasetElementValue == ev.Value }));

                    ViewData.Add(element.DatasetElementName, elementFieldValueList.ToArray());
                }

                foreach (var element in yesNoDatasetElements)
                {
                    var yesNo = new[] { new SelectListItem { Value = "", Text = "" }, new SelectListItem { Value = "No", Text = "No" }, new SelectListItem { Value = "Yes", Text = "Yes" } };

                    var selectedYesNo = yesNo.SingleOrDefault(yn => yn.Value == element.DatasetElementValue);
                    if (selectedYesNo != null)
                    {
                        selectedYesNo.Selected = true;
                    }

                    ViewData.Add(element.DatasetElementName, yesNo);
                }
            }

            return View(instanceModel);
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult ViewDatasetElementTable(int id, int datasetInstanceId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewBag.DatasetInstanceId = datasetInstanceId;

            var datasetElement = _unitOfWork.Repository<DatasetElement>().Get(id);

            ViewBag.DatasetElementName = datasetElement.ElementName;
            ViewBag.DatasetElementId = datasetElement.Id;

            var datasetElementSubs = _unitOfWork.Repository<DatasetElementSub>()
                .Queryable()
                .Include(i => i.Field.FieldType)
                .Include(i => i.Field.FieldValues.Select(fv => fv.Field.FieldType))
                .Where(w => w.DatasetElement.Id == id)
                .Take(6)
                .ToList()
                .OrderBy(o => o.FieldOrder)
                .Select(des => new DatasetElementTableHeaderRowModel 
                {
                    DatasetElementSubId = des.Id,
                    DatasetElementSubName = des.ElementName
                })
                .ToArray();

            ViewBag.TableHeaderColumns = datasetElementSubs;

            var instanceSubValues = _unitOfWork.Repository<DatasetInstanceSubValue>()
                .Queryable()
                .Include(i => i.DatasetElementSub.DatasetElement.Field.FieldType)
                .Include(i2 => i2.DatasetElementSub.Field.FieldValues.Select(fv => fv.Field.FieldType))
                .Where(w => w.DatasetElementSub.DatasetElement.Id == id && w.DatasetInstanceValue.DatasetInstance.Id == datasetInstanceId)
                .ToList()
                .GroupBy(g => g.ContextValue)
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
                })
                .ToArray();

            return View(instanceSubValues);
        }

        [MvcUnitOfWork]
        public ActionResult DeleteDatasetInstanceSubValuesForDatasetElement(int id, int datasetInstanceId, int encounterId, Guid context)
        {
            var datasetInstanceSubValueRepository = _unitOfWork.Repository<DatasetInstanceSubValue>();
            var datasetElement = _unitOfWork.Repository<DatasetElement>().Get(id);

            var datasetInstance = _unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include(i => i.DatasetInstanceValues.Select(i2 => i2.DatasetInstanceSubValues.Select(i3 => i3.DatasetElementSub)))
                .Where(di => di.Id == datasetInstanceId)
                .SingleOrDefault();

            var instanceSubValues = datasetInstance.GetInstanceSubValues(datasetElement, context);

            foreach (var instanceSubValue in instanceSubValues)
            {
                datasetInstanceSubValueRepository.Delete(instanceSubValue);
            }

            _unitOfWork.Complete();

            return RedirectToAction("ViewDatasetElementTable", new { id, encounterId = encounterId, datasetInstanceId = datasetInstanceId });
        }
    }
}
