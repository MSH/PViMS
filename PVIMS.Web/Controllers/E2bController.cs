using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using VPS.Common.Repositories;
using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;

using PVIMS.Web.ActionFilters;
using PVIMS.Web.Models;

using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;

namespace PVIMS.Web.Controllers
{
    public class E2bController : BaseController
    {
        private static string ActiveCurrentMenuItem = "ActiveReporting";
        private static string SpontaneousCurrentMenuItem = "SpontaneousReporting";

        private readonly IUnitOfWorkInt unitOfWork;
        private readonly IWorkFlowService _workflowService;

        public IWorkFlowService _workFlowService { get; set; }

        public E2bController(IUnitOfWorkInt unitOfWork, IWorkFlowService workFlowService)
        {
            this.unitOfWork = unitOfWork;
            _workflowService = workFlowService;
        }

        // GET: Encounter
        [MvcUnitOfWork]
        public ActionResult Index()
        {
            ViewBag.MenuItem = ActiveCurrentMenuItem;

            var encounters = unitOfWork.Repository<Encounter>()
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
        public ActionResult AddE2bActive(Guid contextGuid)
        {
            ViewBag.MenuItem = ActiveCurrentMenuItem;

            var instanceModel = new DatasetInstanceModel { };

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = 0;

            DatasetInstance datasetInstance = null;

            // Determine which dataset to use
            var config = unitOfWork.Repository<Config>().Queryable().Where(c => c.ConfigType == ConfigType.E2BVersion).Single();
            var dsName = String.Format("{0}", config.ConfigValue);
            var dataset = unitOfWork.Repository<Dataset>()
                .Queryable()
                .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType")
                .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType")
                .SingleOrDefault(d => d.DatasetName == dsName);

            // Load source element
            var patientClinicalEvent = unitOfWork.Repository<PatientClinicalEvent>()
                .Queryable()
                .Include("Patient")
                .SingleOrDefault(p => p.PatientClinicalEventGuid == contextGuid);

            // Add activity and link E2B to new element
            var evt = _workflowService.ExecuteActivity(contextGuid, "E2BINITIATED", "AUTOMATION: E2B dataset created", null, "");

            if (dataset != null && patientClinicalEvent != null)
            {
                datasetInstance = dataset.CreateInstance(evt.Id, null);
                datasetInstance.Tag = "Active";
                unitOfWork.Repository<DatasetInstance>().Save(datasetInstance);

                // Default values
                if (config.ConfigValue.Contains("(R2)"))
                {
                    datasetInstance.InitialiseValues(datasetInstance.Tag, null, patientClinicalEvent);

                    SetInstanceValuesForActiveRelease2(datasetInstance, patientClinicalEvent);
                }
                if (config.ConfigValue.Contains("(R3)"))
                {
                    var term = _workFlowService.GetTerminologyMedDraForReportInstance(patientClinicalEvent.PatientClinicalEventGuid);

                    datasetInstance.InitialiseValues(datasetInstance.Tag, null, patientClinicalEvent);

                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.2 Batch Number"), "MSH.PViMS-B01000" + patientClinicalEvent.Id.ToString());
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.5 Date of Batch Transmission"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.1 Message Identifier"), "MSH.PViMS-B01000" + patientClinicalEvent.Id.ToString() + "-" + DateTime.Now.ToString("mmsss"));
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.4 Date of Message Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.1 Sender’s (case) Safety Report Unique Identifier"), String.Format("ZA-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), patientClinicalEvent.Id.ToString()));
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.2 Date of Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.8.1 Worldwide Unique Case Identification Number"), String.Format("ZA-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), patientClinicalEvent.Id.ToString()));
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.2.1b MedDRA Code for Reaction / Event"), term.DisplayName);
                }

                unitOfWork.Complete();
                instanceModel.DatasetInstanceId = datasetInstance.Id;
            }

            return Redirect("/E2b/ViewE2b?datasetInstanceId=" + datasetInstance.Id.ToString());
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult AddE2bSpontaneous(Guid contextGuid)
        {
            ViewBag.MenuItem = SpontaneousCurrentMenuItem;

            var instanceModel = new DatasetInstanceModel { };
            var currentUser = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == User.Identity.Name);

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = 0;

            DatasetInstance datasetInstance = null;

            var config = unitOfWork.Repository<Config>().Queryable().Where(c => c.ConfigType == ConfigType.E2BVersion).Single();
            var dsName = String.Format("{0}", config.ConfigValue);
            var dataset = unitOfWork.Repository<Dataset>()
                .Queryable()
                .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType")
                .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType")
                .SingleOrDefault(d => d.DatasetName == dsName);

            // Load source element
            var sourceInstance = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("Dataset")
                .SingleOrDefault(ds => ds.DatasetInstanceGuid == contextGuid);

            // Add activity and link E2B to new element
            var evt = _workflowService.ExecuteActivity(contextGuid, "E2BINITIATED", "AUTOMATION: E2B dataset created", null, "");

            if (dataset != null && sourceInstance != null)
            {
                datasetInstance = dataset.CreateInstance(evt.Id, null);
                datasetInstance.Tag = "Spontaneous";
                unitOfWork.Repository<DatasetInstance>().Save(datasetInstance);

                // Default values
                if (config.ConfigValue.Contains("(R2)"))
                {
                    datasetInstance.InitialiseValues(datasetInstance.Tag, sourceInstance, null);

                    SetInstanceValuesForSpontaneousRelease2(datasetInstance, sourceInstance, currentUser);
                }
                if (config.ConfigValue.Contains("(R3)"))
                {
                    datasetInstance.InitialiseValues(datasetInstance.Tag, sourceInstance, null);

                    SetInstanceValuesForSpontaneousRelease3(datasetInstance, sourceInstance, currentUser, sourceInstance.Id);
                }

                unitOfWork.Complete();
                instanceModel.DatasetInstanceId = datasetInstance.Id;
            }

            return Redirect("/E2b/ViewE2b?datasetInstanceId=" + datasetInstance.Id.ToString());
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult EditE2b(int datasetInstanceId)
        {
            ViewBag.MenuItem = ActiveCurrentMenuItem;

            var returnUrl = "/Analytical/ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219";
            TempData["returnUrl"] = returnUrl;
            ViewBag.ReturnUrl = returnUrl;

            var instanceModel = new DatasetInstanceModel { };

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = datasetInstanceId;

            var datasetInstance = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Id == datasetInstanceId).Single();

            if (datasetInstance != null)
            {
                if (datasetInstance.Tag == "Active")
                {
                    returnUrl = "/Analytical/ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219";
                    ViewBag.MenuItem = ActiveCurrentMenuItem;
                }
                else
                {
                    returnUrl = "/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986";
                    ViewBag.MenuItem = SpontaneousCurrentMenuItem;
                }
                TempData["returnUrl"] = returnUrl;
                ViewBag.ReturnUrl = returnUrl;

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
                            }).ToArray(),
                            TableHeaderColumns = e.DatasetElement.DatasetElementSubs.Where(es1 => es1.System == false).Take(6).OrderBy(es2 => es2.FieldOrder).Select(des => new DatasetElementTableHeaderRowModel
                            {
                                DatasetElementSubId = des.Id,
                                DatasetElementSubName = des.ElementName
                            }).ToArray(),
                            InstanceSubValues = e.DatasetElement.DatasetElementSubs.Count > 0 ? unitOfWork.Repository<DatasetInstanceValue>().Queryable().Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == e.DatasetElement.Id).DatasetInstanceSubValues.GroupBy(g => g.ContextValue).Select(disv => new DatasetInstanceSubValueGroupingModel
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
                            }).ToArray() : null })
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

                var datasetElementRepository = unitOfWork.Repository<DatasetElement>();

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
        public ActionResult EditE2b(DatasetInstanceModel model)
        {
            ViewBag.MenuItem = ActiveCurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            DatasetInstance datasetInstance = null;

            var datasetInstanceRepository = unitOfWork.Repository<DatasetInstance>();

            if (ModelState.IsValid)
            {
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

                var datasetElementIds = model.DatasetCategories.SelectMany(dc => dc.DatasetElements.Select(dse => dse.DatasetElementId)).ToArray();

                var datasetElements = unitOfWork.Repository<DatasetElement>()
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
                                    if (eleVal != string.Empty) {
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

                    unitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "E2B record updated successfully";
                    Response.Cookies.Add(cookie);

                    if (datasetInstance.Tag == "Active") {
                        return Redirect("/Analytical/ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219");
                    }
                    else {
                        return Redirect("/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986");
                    }
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
                        }).ToArray(),
                        TableHeaderColumns = e.DatasetElement.DatasetElementSubs.Take(6).OrderBy(es => es.FieldOrder).Select(des => new DatasetElementTableHeaderRowModel
                        {
                            DatasetElementSubId = des.Id,
                            DatasetElementSubName = des.ElementName
                        }).ToArray(),
                        InstanceSubValues = e.DatasetElement.DatasetElementSubs.Count > 0 ? unitOfWork.Repository<DatasetInstanceValue>().Queryable().Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == e.DatasetElement.Id).DatasetInstanceSubValues.GroupBy(g => g.ContextValue).Select(disv => new DatasetInstanceSubValueGroupingModel
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
                        }).ToArray() : null })
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

            var datasetElementRepository = unitOfWork.Repository<DatasetElement>();
            var datasetElementSubRepository = unitOfWork.Repository<DatasetElementSub>();

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
        public ActionResult ViewE2b(int datasetInstanceId)
        {
            ViewBag.MenuItem = ActiveCurrentMenuItem;

            var returnUrl = "/Analytical/ReportInstanceList.aspx";

            var instanceModel = new DatasetInstanceModel { };

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = datasetInstanceId;

            var datasetInstance = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Id == datasetInstanceId).Single();

            if (datasetInstance != null)
            {
                if (datasetInstance.Tag == "Active")
                {
                    returnUrl = "/Analytical/ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219";
                    ViewBag.MenuItem = ActiveCurrentMenuItem;
                }
                else
                {
                    returnUrl = "/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986";
                    ViewBag.MenuItem = SpontaneousCurrentMenuItem;
                }

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
                            }).ToArray(),
                            TableHeaderColumns = e.DatasetElement.DatasetElementSubs.Where(es1 => es1.System == false).Take(6).OrderBy(es2 => es2.FieldOrder).Select(des => new DatasetElementTableHeaderRowModel
                            {
                                DatasetElementSubId = des.Id,
                                DatasetElementSubName = des.ElementName
                            }).ToArray(),
                            InstanceSubValues = e.DatasetElement.DatasetElementSubs.Count > 0 ? unitOfWork.Repository<DatasetInstanceValue>().Queryable().Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == e.DatasetElement.Id).DatasetInstanceSubValues.GroupBy(g => g.ContextValue).Select(disv => new DatasetInstanceSubValueGroupingModel
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

                var datasetElementRepository = unitOfWork.Repository<DatasetElement>();

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

            TempData["returnUrl"] = returnUrl;
            ViewBag.ReturnUrl = returnUrl;

            return View(instanceModel);
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult ViewSpontaneous(int datasetInstanceId)
        {
            ViewBag.MenuItem = SpontaneousCurrentMenuItem;

            var returnUrl = "/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986";
            TempData["returnUrl"] = returnUrl;
            ViewBag.ReturnUrl = returnUrl;

            var instanceModel = new DatasetInstanceModel { };

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = 0;

            var datasetInstanceQuery = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Id == datasetInstanceId);

            var datasetInstance = datasetInstanceQuery.SingleOrDefault();

            if (datasetInstance != null)
            {
                instanceModel.DatasetInstanceId = datasetInstance.Id;

                var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories
                    .OrderBy(dc => dc.CategoryOrder)
                    .SelectMany(dc2 => dc2.DatasetCategoryElements)
                    .OrderBy(dce => dce.FieldOrder)
                    .GroupBy(dce2 => dce2.DatasetCategory)
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
                            InstanceSubValues = e.DatasetElement.DatasetElementSubs.Count > 0 ? unitOfWork.Repository<DatasetInstanceValue>().Queryable().Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == e.DatasetElement.Id).DatasetInstanceSubValues.GroupBy(g => g.ContextValue).Select(disv => new DatasetInstanceSubValueGroupingModel
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

                var datasetElementRepository = unitOfWork.Repository<DatasetElement>();

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
        public ActionResult EditSpontaneous(int datasetInstanceId)
        {
            ViewBag.MenuItem = SpontaneousCurrentMenuItem;

            var returnUrl = "/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986";
            //var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;
            ViewBag.ReturnUrl = returnUrl;

            var instanceModel = new DatasetInstanceModel { };

            instanceModel.DatasetCategories = new DatasetCategoryEditModel[0];
            instanceModel.DatasetInstanceId = datasetInstanceId;

            var datasetInstance = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Id == datasetInstanceId).Single();

            if (datasetInstance != null)
            {
                var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories
                    .OrderBy(dc => dc.CategoryOrder)
                    .SelectMany(dc2 => dc2.DatasetCategoryElements)
                    .OrderBy(dce => dce.FieldOrder)
                    .GroupBy(dce2 => dce2.DatasetCategory)
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
                            }).ToArray(),
                            TableHeaderColumns = e.DatasetElement.DatasetElementSubs.Where(es1 => es1.System == false).Take(6).OrderBy(es2 => es2.FieldOrder).Select(des => new DatasetElementTableHeaderRowModel
                            {
                                DatasetElementSubId = des.Id,
                                DatasetElementSubName = des.ElementName
                            }).ToArray(),
                            InstanceSubValues = e.DatasetElement.DatasetElementSubs.Count > 0 ? unitOfWork.Repository<DatasetInstanceValue>().Queryable().Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == e.DatasetElement.Id).DatasetInstanceSubValues.GroupBy(g => g.ContextValue).Select(disv => new DatasetInstanceSubValueGroupingModel
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

                var datasetElementRepository = unitOfWork.Repository<DatasetElement>();

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
            ViewBag.MenuItem = ActiveCurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            DatasetInstance datasetInstance = null;

            var datasetInstanceRepository = unitOfWork.Repository<DatasetInstance>();

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

                var datasetElements = unitOfWork.Repository<DatasetElement>()
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

                    var patientIdentifier = datasetInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Identification Number"));
                    var sourceIdentifier = datasetInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Description of reaction"));
                    _workFlowService.UpdateIdentifiersForWorkFlowInstance(datasetInstance.DatasetInstanceGuid, patientIdentifier, sourceIdentifier);

                    unitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Spontaneous record updated successfully";
                    Response.Cookies.Add(cookie);

                    return Redirect("/Analytical/ReportInstanceList.aspx?wuid=4096d0a3-45f7-4702-bda1-76aede41b986");
                }
                catch (DatasetFieldSetException dse)
                {
                    ModelState.AddModelError(dse.Key, dse.Message);
                }
            }

            var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories
                .OrderBy(dc => dc.CategoryOrder)
                .SelectMany(dc2 => dc2.DatasetCategoryElements)
                .OrderBy(dce => dce.FieldOrder)
                .GroupBy(dce2 => dce2.DatasetCategory)
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
                        }).ToArray(),
                        TableHeaderColumns = e.DatasetElement.DatasetElementSubs.Take(6).OrderBy(es => es.FieldOrder).Select(des => new DatasetElementTableHeaderRowModel
                        {
                            DatasetElementSubId = des.Id,
                            DatasetElementSubName = des.ElementName
                        }).ToArray(),
                        InstanceSubValues = e.DatasetElement.DatasetElementSubs.Count > 0 ? unitOfWork.Repository<DatasetInstanceValue>().Queryable().Single(div => div.DatasetInstance.Id == datasetInstance.Id && div.DatasetElement.Id == e.DatasetElement.Id).DatasetInstanceSubValues.GroupBy(g => g.ContextValue).Select(disv => new DatasetInstanceSubValueGroupingModel
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

            var selectTypeDatasetElements = model.DatasetCategories
                .SelectMany(dc => dc.DatasetElements)
                .Where(de => de.DatasetElementType == FieldTypes.Listbox.ToString()
                    || de.DatasetElementType == FieldTypes.DropDownList.ToString())
                .ToArray();

            var yesNoDatasetElements = model.DatasetCategories
                .SelectMany(dc => dc.DatasetElements)
                .Where(de => de.DatasetElementType == FieldTypes.YesNo.ToString())
                .ToArray();

            var datasetElementRepository = unitOfWork.Repository<DatasetElement>();
            var datasetElementSubRepository = unitOfWork.Repository<DatasetElementSub>();

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
        public ActionResult ViewDatasetElementTable(int id, int datasetInstanceId)
        {
            ViewBag.MenuItem = ActiveCurrentMenuItem;

            ViewBag.DatasetInstanceId = datasetInstanceId;

            var datasetElement = unitOfWork.Repository<DatasetElement>().Get(id);

            ViewBag.DatasetElementName = datasetElement.ElementName;
            ViewBag.DatasetElementId = datasetElement.Id;

            var datasetElementSubs = unitOfWork.Repository<DatasetElementSub>()
                .Queryable()
                .Include(i => i.Field.FieldType)
                .Include(i => i.Field.FieldValues.Select(fv => fv.Field.FieldType))
                .Where(w => w.DatasetElement.Id == id)
                .Take(7)
                .ToList()
                .OrderBy(o => o.FieldOrder)
                .Select(des => new DatasetElementTableHeaderRowModel
                {
                    DatasetElementSubId = des.Id,
                    DatasetElementSubName = des.ElementName
                })
                .ToArray();

            ViewBag.TableHeaderColumns = datasetElementSubs;

            var instanceSubValues = unitOfWork.Repository<DatasetInstanceSubValue>()
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
        public ActionResult DeleteDatasetInstanceSubValuesForDatasetElement(int id, int datasetInstanceId, Guid context)
        {
            ViewBag.MenuItem = ActiveCurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var datasetInstanceSubValueRepository = unitOfWork.Repository<DatasetInstanceSubValue>();
            var datasetElement = unitOfWork.Repository<DatasetElement>().Get(id);

            var datasetInstance = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include(i => i.DatasetInstanceValues.Select(i2 => i2.DatasetInstanceSubValues.Select(i3 => i3.DatasetElementSub)))
                .Where(di => di.Id == datasetInstanceId)
                .SingleOrDefault();

            var instanceSubValues = datasetInstance.GetInstanceSubValues(datasetElement, context);

            foreach (var instanceSubValue in instanceSubValues)
            {
                datasetInstanceSubValueRepository.Delete(instanceSubValue);
            }

            unitOfWork.Complete();

            HttpCookie cookie = new HttpCookie("PopUpMessage");
            cookie.Value = "Record deleted successfully";
            Response.Cookies.Add(cookie);

            if (String.IsNullOrEmpty(returnUrl)) { RedirectToAction("Index", "Home"); };

            return Redirect(returnUrl);
        }

        private void SetInstanceValuesForSpontaneousRelease2(DatasetInstance datasetInstance, DatasetInstance sourceInstance, User currentUser)
        {
            // ************************************* ichicsrmessageheader
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Message Header").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Message Number").DatasetElement, datasetInstance.Id.ToString("D8"));
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Message Header").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Message Date").DatasetElement, DateTime.Today.ToString("yyyyMMddhhmmss"));

            // ************************************* safetyreport
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Safety Report ID").DatasetElement, string.Format("PH-FDA-{0}", sourceInstance.Id.ToString("D5")));
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Transmission Date").DatasetElement, DateTime.Today.ToString("yyyyMMdd"));
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Report Type").DatasetElement, "1");

            var seriousReason = sourceInstance.GetInstanceValue(sourceInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction and Treatment").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction serious details").DatasetElement);
            if (!String.IsNullOrWhiteSpace(seriousReason))
            {
                var sd = "2=No";
                var slt = "2=No";
                var sh = "2=No";
                var sdi = "2=No";
                var sca = "2=No";
                var so = "2=No";

                switch (seriousReason)
                {
                    case "Resulted in death":
                        sd = "1=Yes";
                        break;

                    case "Is life-threatening":
                        slt = "1=Yes";
                        break;

                    case "Is a congenital anomaly/birth defect":
                        sca = "1=Yes";
                        break;

                    case "Requires hospitalization or longer stay in hospital":
                        sh = "1=Yes";
                        break;

                    case "Results in persistent or significant disability/incapacity (as per reporter's opinion)":
                        sdi = "1=Yes";
                        break;

                    case "Other medically important condition":
                        so = "1=Yes";
                        break;

                    default:
                        break;
                }

                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B4EA6CBF-2D9C-482D-918A-36ABB0C96EFA"), sd); //Seriousness Death
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "26C6F08E-B80B-411E-BFDC-0506FE102253"), slt); //Seriousness Life Threatening
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "837154A9-D088-41C6-A9E2-8A0231128496"), sh); //Seriousness Hospitalization
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DDEBDEC0-2A90-49C7-970E-B7855CFDF19D"), sdi); //Seriousness Disabling
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DF89C98B-1D2A-4C8E-A753-02E265841F4F"), sca); //Seriousness Congenital Anomaly
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "33A75547-EF1B-42FB-8768-CD6EC52B24F8"), so); //Seriousness Other
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), "1"); //Serious
            }
            else
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), "2"); //Serious
            }

            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Date report was first received").DatasetElement, sourceInstance.Created.ToString("yyyyMMdd"));
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Date of most recent info").DatasetElement, sourceInstance.Created.ToString("yyyyMMdd"));

            // ************************************* primarysource
            var fullName = sourceInstance.GetInstanceValue(sourceInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reporter Information").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Name").DatasetElement);
            if (!String.IsNullOrWhiteSpace(fullName))
            {
                if (fullName.Contains(" "))
                {
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Given Name").DatasetElement, fullName.Substring(0, fullName.IndexOf(" ")));
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Family Name").DatasetElement, fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1)));
                }
                else
                {
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Given Name").DatasetElement, fullName);
                }
            }

            // ************************************* sender
            var regAuth = unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.RegulatoryAuthority);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Type"), "2=Regulatory Authority");
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Organization"), regAuth.OrganisationName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Given Name"), regAuth.ContactFirstName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Family Name"), regAuth.ContactSurname);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Street Address"), regAuth.StreetAddress);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender City"), regAuth.City);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender State"), regAuth.State);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Postcode"), regAuth.PostCode);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Number"), regAuth.ContactNumber);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Country Code"), regAuth.CountryCode);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Email Address"), regAuth.ContactEmail);

            // ************************************* receiver
            var repAuth = unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.ReportingAuthority);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Type"), "5=WHO Collaborating Center for International Drug Monitoring");
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Organization"), repAuth.OrganisationName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Given Name"), repAuth.ContactFirstName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Family Name"), repAuth.ContactSurname);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Street Address"), repAuth.StreetAddress);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver City"), repAuth.City);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver State"), repAuth.State);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Postcode"), repAuth.PostCode);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel"), repAuth.ContactNumber);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel Country Code"), repAuth.CountryCode);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Email Address"), repAuth.ContactEmail);

            // ************************************* patient
            var dob = sourceInstance.GetInstanceValue(sourceInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient Information").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Date of Birth").DatasetElement);
            var onset = sourceInstance.GetInstanceValue(sourceInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction and Treatment").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction start date").DatasetElement);
            var recovery = sourceInstance.GetInstanceValue(sourceInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction and Treatment").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction date of recovery").DatasetElement);
            if (!String.IsNullOrWhiteSpace(dob))
            {
                datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Birthdate").DatasetElement, Convert.ToDateTime(dob).ToString("yyyyMMdd"));

                if (!String.IsNullOrWhiteSpace(onset))
                {
                    var age = (Convert.ToDateTime(onset) - Convert.ToDateTime(dob)).Days;
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Onset Age").DatasetElement, age.ToString());
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Onset Age Unit").DatasetElement, "804=Day");
                }
            }

            // ************************************* reaction
            var term = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "TerminologyMedDra"));
            var termOut = "NOT SET";
            if (!String.IsNullOrWhiteSpace(term))
            {
                var termid = Convert.ToInt32(term);
                termOut = unitOfWork.Repository<TerminologyMedDra>().Queryable().Single(u => u.Id == termid).DisplayName;
            };
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Reaction MedDRA LLT"), termOut);
            if (!String.IsNullOrWhiteSpace(onset)) { datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction Start Date").DatasetElement, Convert.ToDateTime(onset).ToString("yyyyMMdd")); };
            if (!String.IsNullOrWhiteSpace(onset) && !String.IsNullOrWhiteSpace(recovery))
            {
                var rduration = (Convert.ToDateTime(recovery) - Convert.ToDateTime(onset)).Days;
                datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction Duration").DatasetElement, rduration.ToString());
                datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction Duration Unit").DatasetElement, "804=Day");
            }

            // ************************************* drug
            var sourceProductElement = unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Product Information");
            var destinationProductElement = unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Medicinal Products");
            var sourceContexts = sourceInstance.GetInstanceSubValuesContext(sourceProductElement);
            foreach (Guid sourceContext in sourceContexts)
            {
                var drugItemValues = sourceInstance.GetInstanceSubValues(sourceProductElement, sourceContext);
                var drugName = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue;

                if (drugName != string.Empty)
                {
                    Guid? newContext = datasetInstance.GetContextForInstanceSubValue(destinationProductElement, destinationProductElement.DatasetElementSubs.SingleOrDefault(des => des.ElementName == "Medicinal Product"), drugName);
                    if (newContext != null)
                    {
                        var reportInstanceMedication = unitOfWork.Repository<ReportInstanceMedication>().Queryable().Single(x => x.ReportInstanceMedicationGuid == sourceContext);

                        if (reportInstanceMedication.WhoCausality != null)
                        {
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Source of Assessment"), "WHO Causality Scale", (Guid)newContext);
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), reportInstanceMedication.WhoCausality.ToLowerInvariant() == "ignored" ? "" : reportInstanceMedication.WhoCausality, (Guid)newContext);
                        }
                        else
                        {
                            if (reportInstanceMedication.NaranjoCausality != null)
                            {
                                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Source of Assessment"), "Naranjo Causality Scale", (Guid)newContext);
                                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), reportInstanceMedication.NaranjoCausality.ToLowerInvariant() == "ignored" ? "" : reportInstanceMedication.NaranjoCausality, (Guid)newContext);
                            }
                        }

                        var startValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Start Date");
                        var endValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug End Date");

                        if (startValue != null && endValue != null)
                        {
                            var rduration = (Convert.ToDateTime(endValue.InstanceValue) - Convert.ToDateTime(startValue.InstanceValue)).Days;
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration"), rduration.ToString(), (Guid)newContext);
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration Unit"), "804=Day", (Guid)newContext);
                        }

                        //        var characterValue =  drugItemValues.Single(div => div.DatasetElementSub.ElementName == "Product Suspected");
                        //        var character = characterValue != null ? characterValue.InstanceValue = "Yes" ? "1=Suspect" : "2=Concomitant" : "2=Concomitant";
                        //        datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Characterization"), character, newContext);
                    }
                }
            }
        }

        private void SetInstanceValuesForSpontaneousRelease3(DatasetInstance datasetInstance, DatasetInstance sourceInstance, User currentUser, int datasetInstanceId)
        {
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.2 Batch Number"), "MSH.PViMS-B01000" + datasetInstanceId.ToString());
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.5 Date of Batch Transmission"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.1 Message Identifier"), "MSH.PViMS-B01000" + datasetInstanceId.ToString() + "-" + DateTime.Now.ToString("mmsss"));
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.4 Date of Message Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.1 Sender’s (case) Safety Report Unique Identifier"), String.Format("US-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), datasetInstanceId.ToString()));
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.2 Date of Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.8.1 Worldwide Unique Case Identification Number"), String.Format("US-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), datasetInstanceId.ToString()));

            // Default remaining fields
            // C1 Identification
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.3 Type of Report"), "1=Spontaneous report");
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.4 Date Report Was First Received from Source"), sourceInstance.Created.ToString("yyyy-MM-dd"));

            // C2 Primary Source
            var fullName = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Reporter Name"));
            if (fullName != string.Empty)
            {
                if (fullName.Contains(" "))
                {
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.2 Reporter’s Given Name"), fullName.Substring(0, fullName.IndexOf(" ")));
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.4 Reporter’s Family Name"), fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1)));
                }
                else
                {
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.2 Reporter’s Given Name"), fullName);
                }
            }
            var place = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Reporter Place of Practise"));
            if (place != string.Empty)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.1 Reporter’s Organisation"), place);
            }
            var address = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Reporter Address"));
            if (address != string.Empty)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.3 Reporter’s Street"), address.Substring(0, 99));
            }
            var telNo = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Reporter Telephone Number"));
            if (telNo != string.Empty)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.7 Reporter’s Telephone"), telNo);
            }

            // C3 Sender
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.3.3 Sender’s Given Name"), currentUser.FirstName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.3.5 Sender’s Family Name"), currentUser.LastName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.4.8 Sender’s E-mail Address"), currentUser.Email);

            // D Patient
            var dob = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Date of Birth"));
            var onset = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Date of Onset"));
            if (dob != string.Empty)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.1 Date of Birth"), dob);

                if (onset != string.Empty)
                {
                    var age = (Convert.ToDateTime(onset) - Convert.ToDateTime(dob)).Days;
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.2a Age at Time of Onset of Reaction / Event"), age.ToString());
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.2bAge at Time of Onset of Reaction / Event (unit)"), "Day");
                }
            }
            var weight = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Weight (kg)"));
            if (weight != string.Empty)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.3 Body Weight (kg)"), weight);
            }
            var sex = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sex"));
            if (sex != string.Empty)
            {
                if (sex == "Male")
                {
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.5 Sex"), "1=Male");
                }
                if (sex == "Female")
                {
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.5 Sex"), "2=Female");
                }
            }
            var death = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Date of Death"));
            if (death != string.Empty)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.9.1 Date of Death"), death);
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.3.2a Results in Death"), "Yes");
            }

            // E Reaction
            var evnt = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "ADR Description"));
            if (evnt != string.Empty)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.1.1a Reaction / Event as Reported by the Primary Source in Native Language"), evnt);
            }

            var term = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "TerminologyMedDra"));
            var termOut = "NOT SET";
            if (term != string.Empty)
            {
                var termid = Convert.ToInt32(term);
                termOut = unitOfWork.Repository<TerminologyMedDra>().Queryable().Single(u => u.Id == termid).DisplayName;
            };
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.2.1b MedDRA Code for Reaction / Event"), termOut);

            if (onset != string.Empty)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.4 Date of Start of Reaction / Event"), onset);
            }

            var outcome = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "ADR Outcome"));
            if (outcome != string.Empty)
            {
                switch (outcome)
                {
                    case "Died - Drug may be contributory":
                    case "Died - Due to adverse reaction":
                    case "Died - Unrelated to drug":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "5=fatal");
                        break;

                    case "Not yet recovered":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "3=not recovered/not resolved/ongoing");
                        break;

                    case "Recovered":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "1=recovered/resolved");
                        break;

                    case "Uncertain outcome":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "0=unknown");
                        break;

                    default:
                        break;
                }
            }

            for (int i = 1; i <= 6; i++)
            {
                var drugId = 0;
                var elementName = "";
                var drugName = "";
                var tempi = 0;

                if (i < 4)
                {
                    drugId = i;
                    elementName = string.Format("Suspected Drug {0}", drugId);
                    drugName = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == elementName));

                    if (drugName != string.Empty)
                    {
                        // Create a new context
                        var context = Guid.NewGuid();

                        datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.1 Characterisation of Drug Role"), "1=Suspect", context);
                        datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.2.2 Medicinal Product Name as Reported by the Primary Source"), drugName, context);

                        elementName = string.Format("Suspected Drug {0} Dosage", drugId);
                        var dosage = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (dosage != string.Empty)
                        {
                            if (Int32.TryParse(dosage, out tempi))
                            {
                                datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1a Dose (number)"), dosage, context);
                            }
                        }
                        elementName = string.Format("Suspected Drug {0} Dosage Unit", drugId);
                        var dosageUnit = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (dosageUnit != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1b Dose (unit)"), dosageUnit, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Date Started", drugId);
                        var dateStarted = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (dateStarted != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.4 Date of Start of Drug"), dateStarted, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Date Stopped", drugId);
                        var dateStopped = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (dateStopped != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.5 Date of Last Administration"), dateStopped, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Batch Number", drugId);
                        var batch = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (batch != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.7 Batch / Lot Number"), batch, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Route", drugId);
                        var route = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (route != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.10.1 Route of Administration"), route, context);
                        }
                    }
                }
                else
                {
                    drugId = i - 3;
                    elementName = string.Format("Concomitant Drug {0}", drugId);
                    drugName = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == elementName));

                    if (drugName != string.Empty)
                    {
                        // Create a new context
                        var context = Guid.NewGuid();

                        datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.1 Characterisation of Drug Role"), "1=Suspect", context);
                        datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.2.2 Medicinal Product Name as Reported by the Primary Source"), drugName, context);

                        elementName = string.Format("Concomitant Drug {0} Dosage", drugId);
                        var dosage = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (dosage != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1a Dose (number)"), dosage, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Dosage Unit", drugId);
                        var dosageUnit = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (dosageUnit != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1b Dose (unit)"), dosageUnit, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Date Started", drugId);
                        var dateStarted = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (dateStarted != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.4 Date of Start of Drug"), dateStarted, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Date Stopped", drugId);
                        var dateStopped = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (dateStopped != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.5 Date of Last Administration"), dateStopped, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Batch Number", drugId);
                        var batch = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (batch != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.7 Batch / Lot Number"), batch, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Route", drugId);
                        var route = sourceInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == elementName));
                        if (route != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.10.1 Route of Administration"), route, context);
                        }
                    }
                }
            }
        }

        private void SetInstanceValuesForActiveRelease2(DatasetInstance datasetInstance, PatientClinicalEvent patientClinicalEvent)
        {
            var reportInstance = unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == patientClinicalEvent.PatientClinicalEventGuid);

            // ************************************* ichicsrmessageheader
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "7FF710CB-C08C-4C35-925E-484B983F2135"), datasetInstance.Id.ToString("D8")); // Message Number
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "693614B6-D5D5-457E-A03B-EAAFA66E6FBD"), DateTime.Today.ToString("yyyyMMddhhmmss")); // Message Date

            // ************************************* safetyreport
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "6799CAD0-2A65-48A5-8734-0090D7C2D85E"), string.Format("PH.FDA.{0}", reportInstance.Id.ToString("D6"))); //Safety Report ID
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "9C92D382-03AF-4A52-9A2F-04A46ADA0F7E"), DateTime.Today.ToString("yyyyMMdd")); //Transmission Date 
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "AE53FEB2-FF27-4CD5-AD54-C3FFED1490B5"), "2"); //Report Type

            IExtendable pcExtended = patientClinicalEvent;
            var objectValue = pcExtended.GetAttributeValue("Is the adverse event serious?");
            var serious = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(serious))
            {
                var selectionValue = unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Is the adverse event serious?" && sdi.SelectionKey == serious).Value;
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), selectionValue == "Yes" ? "1=Yes" : "2=No"); //Serious
            }

            objectValue = pcExtended.GetAttributeValue("Seriousness");
            var seriousReason = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(seriousReason) && serious == "1")
            {
                var selectionValue = unitOfWork.Repository<SelectionDataItem>().Queryable().Single(si => si.AttributeKey == "Seriousness" && si.SelectionKey == seriousReason).Value;

                var sd = "2=No";
                var slt = "2=No";
                var sh = "2=No";
                var sdi = "2=No";
                var sca = "2=No";
                var so = "2=No";

                switch (selectionValue)
                {
                    case "Death":
                        sd = "1=Yes";
                        break;

                    case "Life threatening":
                        slt = "1=Yes";
                        break;

                    case "A congenital anomaly or birth defect":
                        sca = "1=Yes";
                        break;

                    case "Initial or prolonged hospitalization":
                        sh = "1=Yes";
                        break;

                    case "Persistent or significant disability or incapacity":
                        sdi = "1=Yes";
                        break;

                    case "A medically important event":
                        so = "1=Yes";
                        break;

                    default:
                        break;
                }

                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B4EA6CBF-2D9C-482D-918A-36ABB0C96EFA"), sd); //Seriousness Death
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "26C6F08E-B80B-411E-BFDC-0506FE102253"), slt); //Seriousness Life Threatening
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "837154A9-D088-41C6-A9E2-8A0231128496"), sh); //Seriousness Hospitalization
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DDEBDEC0-2A90-49C7-970E-B7855CFDF19D"), sdi); //Seriousness Disabling
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DF89C98B-1D2A-4C8E-A753-02E265841F4F"), sca); //Seriousness Congenital Anomaly
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "33A75547-EF1B-42FB-8768-CD6EC52B24F8"), so); //Seriousness Other
            }

            objectValue = pcExtended.GetAttributeValue("Date of Report");
            var reportDate = objectValue != null ? objectValue.ToString() : "";
            DateTime tempdt;
            if (DateTime.TryParse(reportDate, out tempdt))
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "65ADEF15-961A-4558-B864-7832D276E0E3"), Convert.ToDateTime(reportDate).ToString("yyyyMMdd")); //Date report was first received
            }
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "A10C704D-BC1D-445E-B084-9426A91DB63B"), DateTime.Today.ToString("yyyyMMdd")); //Date of most recent info

            // ************************************* primarysource
            objectValue = pcExtended.GetAttributeValue("Full Name of Reporter");
            var fullName = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(fullName))
            {
                if (fullName.Contains(" "))
                {
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C35D5F5A-D539-4EEE-B080-FF384D5FBE08"), fullName.Substring(0, fullName.IndexOf(" "))); //Reporter Given Name
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F214C619-EE0E-433E-8F52-83469778E418"), fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1))); //Reporter Family Name
                }
                else
                {
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C35D5F5A-D539-4EEE-B080-FF384D5FBE08"), fullName); //Reporter Given Name
                }
            }

            objectValue = pcExtended.GetAttributeValue("Type of Reporter");
            var profession = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(profession))
            {
                var selectionValue = unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Type of Reporter" && sdi.SelectionKey == profession).Value;

                switch (selectionValue)
                {
                    case "Other health professional":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "3=Other Health Professional"); //Qualification
                        break;

                    case "Physician":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "1=Physician");
                        break;

                    case "Consumer or other non health professional":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "5=Consumer or other non health professional");
                        break;

                    case "Pharmacist":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "2=Pharmacist");
                        break;

                    case "Lawyer":
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "4=Lawyer");
                        break;

                    default:
                        break;
                }
            }

            // ************************************* sender
            var regAuth = unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.RegulatoryAuthority);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Type"), "2=Regulatory Authority");
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Organization"), regAuth.OrganisationName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Given Name"), regAuth.ContactFirstName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Family Name"), regAuth.ContactSurname);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Street Address"), regAuth.StreetAddress);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender City"), regAuth.City);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender State"), regAuth.State);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Postcode"), regAuth.PostCode);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Number"), regAuth.ContactNumber);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Country Code"), regAuth.CountryCode);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Email Address"), regAuth.ContactEmail);

            // ************************************* receiver
            var repAuth = unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.ReportingAuthority);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Type"), "5=WHO Collaborating Center for International Drug Monitoring");
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Organization"), repAuth.OrganisationName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Given Name"), repAuth.ContactFirstName);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Family Name"), repAuth.ContactSurname);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Street Address"), repAuth.StreetAddress);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver City"), repAuth.City);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver State"), repAuth.State);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Postcode"), repAuth.PostCode);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel"), repAuth.ContactNumber);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel Country Code"), repAuth.CountryCode);
            datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Email Address"), repAuth.ContactEmail);

            // ************************************* patient
            var init = String.Format("{0}{1}", patientClinicalEvent.Patient.FirstName.Substring(0, 1), patientClinicalEvent.Patient.Surname.Substring(0, 1));
            if (!String.IsNullOrWhiteSpace(init)) { datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "A0BEAB3A-0B0A-457E-B190-1B66FE60CA73"), init); }; //Patient Initial

            var dob = patientClinicalEvent.Patient.DateOfBirth;
            var onset = patientClinicalEvent.OnsetDate;
            var recovery = patientClinicalEvent.ResolutionDate;
            if (dob != null)
            {
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "4F71B7F4-4317-4680-B3A3-9C1C1F72AD6A"), Convert.ToDateTime(dob).ToString("yyyyMMdd")); //Patient Birthdate

                if (onset != null)
                {
                    var age = (Convert.ToDateTime(onset) - Convert.ToDateTime(dob)).Days;
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "E10C259B-DD2C-4F19-9D41-16FDDF9C5807"), age.ToString()); //Patient Onset Age
                    datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "CA9B94C2-E1EF-407B-87C3-181224AF637A"), "804=Day"); //Patient Onset Age Unit
                }
            }

            var encounter = unitOfWork.Repository<Encounter>().Queryable().FirstOrDefault(e => e.Patient.Id == patientClinicalEvent.Patient.Id && e.Archived == false & e.EncounterDate <= patientClinicalEvent.OnsetDate);
            if (encounter != null)
            {
                var encounterInstance = unitOfWork.Repository<DatasetInstance>().Queryable().SingleOrDefault(ds => ds.Dataset.DatasetName == "Chronic Treatment" && ds.ContextID == encounter.Id);
                if (encounterInstance != null)
                {
                    var weight = encounterInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Weight (kg)"));
                    if (!String.IsNullOrWhiteSpace(weight)) { datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "89A6E687-A220-4319-AAC1-AFBB55C81873"), weight); }; //Patient Weight

                    var height = encounterInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Height (cm)"));
                    if (!String.IsNullOrWhiteSpace(height)) { datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "40DAD435-8282-4B3E-B65E-3478FF55D028"), height); }; //Patient Height

                    var lmp = encounterInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Date of last menstrual period"));
                    if (!String.IsNullOrWhiteSpace(lmp)) { datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "93253F91-60D1-4161-AF1A-F3ABDD140CB9"), Convert.ToDateTime(lmp).ToString("yyyyMMdd")); }; //Patient Last Menstrual Date

                    var gest = encounterInstance.GetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Estimated gestation (weeks)"));
                    if (!String.IsNullOrWhiteSpace(gest))
                    {
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B6BE9689-B6B2-4FCF-8918-664AFC91A4E0"), gest); //Gestation Period
                        datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1F174413-2A1E-45BD-B5C4-0C8F5DFFBFF4"), "803=Week");  //Gestation Period Unit
                    };
                }
            }

            // ************************************* reaction
            var terminologyMedDra = _workFlowService.GetTerminologyMedDraForReportInstance(patientClinicalEvent.PatientClinicalEventGuid);
            var term = terminologyMedDra != null ? terminologyMedDra.DisplayName : "";
            if (!String.IsNullOrWhiteSpace(term)) { datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C8DD9A5E-BD9A-488D-8ABF-171271F5D370"), term); }; //Reaction MedDRA LLT
            if (onset != null) { datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1EAD9E11-60E6-4B27-9A4D-4B296B169E90"), Convert.ToDateTime(onset).ToString("yyyyMMdd")); }; //Reaction Start Date
            if (recovery != null) { datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "3A0F240E-8B36-48F6-9527-77E55F6E7CF1"), Convert.ToDateTime(recovery).ToString("yyyyMMdd")); }; // Reaction End Date
            if (onset != null && recovery != null)
            {
                var rduration = (Convert.ToDateTime(recovery) - Convert.ToDateTime(onset)).Days;
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "0712C664-2ADD-44C0-B8D5-B6E83FB01F42"), rduration.ToString()); //Reaction Duration
                datasetInstance.SetInstanceValue(unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F96E702D-DCC5-455A-AB45-CAEFF25BF82A"), "804=Day"); //Reaction Duration Unit
            }

            // ************************************* test
            var destinationTestElement = unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.DatasetElementGuid.ToString() == "693A2E8C-B041-46E7-8687-0A42E6B3C82E"); // Test History
            foreach (PatientLabTest labTest in patientClinicalEvent.Patient.PatientLabTests.Where(lt => lt.TestDate >= patientClinicalEvent.OnsetDate).OrderByDescending(lt => lt.TestDate))
            {
                var newContext = Guid.NewGuid();

                datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Date"), labTest.TestDate.ToString("yyyyMMdd"), (Guid)newContext);
                datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Name"), labTest.LabTest.Description, (Guid)newContext);

                var testResult = !String.IsNullOrWhiteSpace(labTest.LabValue) ? labTest.LabValue : !String.IsNullOrWhiteSpace(labTest.TestResult) ? labTest.TestResult : "";
                datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Result"), testResult, (Guid)newContext);

                var testUnit = labTest.TestUnit != null ? labTest.TestUnit.Description : "";
                if (!String.IsNullOrWhiteSpace(testUnit)) { datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Unit"), testUnit, (Guid)newContext); };

                var lowRange = labTest.ReferenceLower;
                if (!String.IsNullOrWhiteSpace(lowRange)) { datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Low Test Range"), lowRange, (Guid)newContext); };

                var highRange = labTest.ReferenceUpper;
                if (!String.IsNullOrWhiteSpace(highRange)) { datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "High Test Range"), highRange, (Guid)newContext); };
            }

            // ************************************* drug
            string[] validNaranjoCriteria = { "Possible", "Probable", "Definite" };
            string[] validWHOCriteria = { "Possible", "Probable", "Certain" };

            var destinationProductElement = unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.DatasetElementGuid.ToString() == "E033BDE8-EDC8-43FF-A6B0-DEA6D6FA581C"); // Medicinal Products
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                var newContext = Guid.NewGuid();

                var patientMedication = unitOfWork.Repository<PatientMedication>().Queryable().Single(pm => pm.PatientMedicationGuid == med.ReportInstanceMedicationGuid);
                IExtendable mcExtended = patientMedication;

                var character = "";
                character = (validNaranjoCriteria.Contains(med.NaranjoCausality) || validWHOCriteria.Contains(med.WhoCausality)) ? "1=Suspect" : "2=Concomitant";
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Characterization"), character, (Guid)newContext);

                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Medicinal Product"), patientMedication.Medication.DrugName, (Guid)newContext);

                objectValue = mcExtended.GetAttributeValue("Batch Number");
                var batchNumber = objectValue != null ? objectValue.ToString() : "";
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Batch Number"), batchNumber, (Guid)newContext);

                objectValue = mcExtended.GetAttributeValue("Comments");
                var comments = objectValue != null ? objectValue.ToString() : "";
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Additional Information"), comments, (Guid)newContext);

                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Dosage Text"), patientMedication.Dose, (Guid)newContext);

                var form = patientMedication != null ? patientMedication.Medication.MedicationForm != null ? patientMedication.Medication.MedicationForm.Description : "" : "";
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Dosage Form"), form, (Guid)newContext);

                var startdate = patientMedication.DateStart;
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Start Date"), startdate.ToString("yyyyMMdd"), (Guid)newContext);

                var enddate = patientMedication.DateEnd;
                if (enddate != null) { datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug End Date"), Convert.ToDateTime(enddate).ToString("yyyyMMdd"), (Guid)newContext); };

                if (startdate != null && enddate != null)
                {
                    var rduration = (Convert.ToDateTime(enddate) - Convert.ToDateTime(startdate)).Days;
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration"), rduration.ToString(), (Guid)newContext);
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration Unit"), "804=Day", (Guid)newContext);
                }

                var doseUnit = MapDoseUnitForActive(patientMedication.DoseUnit);
                if (!string.IsNullOrWhiteSpace(doseUnit)) { datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Structured Dosage Unit"), doseUnit, (Guid)newContext); };

                objectValue = mcExtended.GetAttributeValue("Clinician action taken with regard to medicine if related to AE");
                var drugAction = objectValue != null ? objectValue.ToString() : "";
                if (!string.IsNullOrWhiteSpace(drugAction)) { drugAction = MapDrugActionForActive(drugAction); };
                if (!string.IsNullOrWhiteSpace(drugAction)) { datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Action"), doseUnit, (Guid)newContext); };

                // Causality
                if (med.WhoCausality != null)
                {
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Source of Assessment"), "WHO Causality Scale", (Guid)newContext);
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), med.WhoCausality.ToLowerInvariant() == "ignored" ? "" : med.WhoCausality, (Guid)newContext);
                }
                else
                {
                    if (med.NaranjoCausality != null)
                    {
                        datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Source of Assessment"), "Naranjo Causality Scale", (Guid)newContext);
                        datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), med.NaranjoCausality.ToLowerInvariant() == "ignored" ? "" : med.NaranjoCausality, (Guid)newContext);
                    }
                }
            } // foreach (ReportInstanceMedication med in reportInstance.Medications)

        } // end of sub


        private string MapDoseUnitForActive(string doseUnit)
        {
            switch (doseUnit)
            {
                case "Bq":
                    return "014=Bq becquerel(s)";

                case "Ci":
                    return "018=Ci curie(s)";

                case "{DF}":
                    return "032=DF dosage form";

                case "[drp]":
                    return "031=Gtt drop(s)";

                case "GBq":
                    return "015=GBq gigabecquerel(s)";

                case "g":
                    return "002=G gram(s)";

                case "[iU]":
                    return "025=Iu international unit(s)";

                case "[iU]/kg":
                    return "028=iu/kg iu/kilogram";

                case "kBq":
                    return "017=Kbq kilobecquerel(s)";

                case "kg":
                    return "001=kg kilogram(s)";

                case "k[iU]":
                    return "026=Kiu iu(1000s)";

                case "L":
                    return "011=l litre(s)";

                case "MBq":
                    return "016=MBq megabecquerel(s)";

                case "M[iU]":
                    return "027=Miu iu(1,000,000s)";

                case "uCi":
                    return "020=uCi microcurie(s)";

                case "ug":
                    return "004=ug microgram(s)";

                case "ug/kg":
                    return "007=mg/kg milligram(s)/kilogram";

                case "uL":
                    return "013=ul microlitre(s)";

                case "mCi":
                    return "019=MCi millicurie(s)";

                case "meq":
                    return "029=Meq milliequivalent(s)";

                case "mg":
                    return "003=Mg milligram(s)";

                case "mg/kg":
                    return "007=mg/kg milligram(s)/kilogram";

                case "mg/m2":
                    return "009=mg/m 2 milligram(s)/sq. meter";

                case "ug/m2":
                    return "010=ug/ m 2 microgram(s)/ sq. Meter";

                case "mL":
                    return "012=ml millilitre(s)";

                case "mmol":
                    return "023=Mmol millimole(s)";

                case "mol":
                    return "022=Mol mole(s)";

                case "nCi":
                    return "021=NCi nanocurie(s)";

                case "ng":
                    return "005=ng nanogram(s)";

                case "%":
                    return "030=% percent";

                case "pg":
                    return "006=pg picogram(s)";

                default:
                    break;
            }

            return "";
        }

        private string MapDrugActionForActive(string drugAction)
        {
            if (!String.IsNullOrWhiteSpace(drugAction))
            {
                var selectionValue = unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Clinician action taken with regard to medicine if related to AE" && sdi.SelectionKey == drugAction).Value;

                switch (selectionValue)
                {
                    case "Dose not changed":
                        return "4=Dose not changed";

                    case "Dose reduced":
                        return "2=Dose reduced";

                    case "Drug interrupted":
                        return "5=Unknown";

                    case "Drug withdrawn":
                        return "1=Drug withdrawn";

                    case "Not applicable":
                        return "6=Not applicable";

                    default:
                        break;
                }
            } // if (!String.IsNullOrWhiteSpace(drugAction))

            return "";
        }
    }
}

