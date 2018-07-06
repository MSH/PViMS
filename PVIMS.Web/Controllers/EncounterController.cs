using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using VPS.Common.Repositories;
using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using PVIMS.Core.Exceptions;

using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;

using PVIMS.Web.ActionFilters;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class EncounterController : BaseController
    {
        private static string CurrentMenuItem = "EncounterView";

        private readonly IUnitOfWorkInt unitOfWork;
        private readonly IPatientService _patientService;
        private readonly ICustomAttributeService _attributeService;

        public EncounterController(IUnitOfWorkInt unitOfWork, IPatientService patientService, ICustomAttributeService attributeService)
        {
            this.unitOfWork = unitOfWork;

            _patientService = patientService;
            _attributeService = attributeService;
        }

        // GET: Encounter
        [MvcUnitOfWork]
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

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

        public ActionResult EncounterSearch()
        {
            return View();
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult AddEncounter(int pid, int aid, string cancelRedirectUrl)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            TempData["cancelRedirectUrl"] = cancelRedirectUrl;
            ViewBag.CancelRedirectUrl = cancelRedirectUrl;

            Patient patient = pid > 0 ? unitOfWork.Repository<Patient>().Get(pid) : null;
            Appointment appointment = aid > 0 ? unitOfWork.Repository<Appointment>().Get(aid) : null;

            EncounterAddModel encounterAddModel = null;

            string patientName = "";
            if (patient != null)
            {
                patientName = patient.FullName;
                encounterAddModel = new EncounterAddModel { PatientId = patient.Id, AppointmentId = 0, EncounterNotes = "", EncounterDate = DateTime.Today };
            }
            if (appointment != null)
            {
                patientName = appointment.Patient.FullName;
                encounterAddModel = new EncounterAddModel { PatientId = 0, AppointmentId = appointment.Id, EncounterNotes = appointment.Reason, EncounterDate = DateTime.Today };
            }

            ViewBag.EncounterTypes = unitOfWork.Repository<EncounterType>()
                .Queryable()
                .Select(et => new SelectListItem { Value = et.Id.ToString(), Text = et.Description })
                .ToList();

            ViewBag.Priorities = unitOfWork.Repository<Priority>()
                .Queryable()
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Description })
                .ToList();

            ViewBag.PatientName = patientName;
            ViewBag.CancelRedirectUrl = cancelRedirectUrl;

            return View(encounterAddModel);
        }

        public ActionResult AddEncounterOffline()
        {
            return View();
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult AddEncounter(EncounterAddModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var cancelRedirectUrl = (TempData["cancelRedirectUrl"] ?? string.Empty).ToString();
            ViewBag.CancelRedirectUrl = cancelRedirectUrl;

            string patientName = "";

            Patient patient = model.PatientId > 0 ? unitOfWork.Repository<Patient>().Get(model.PatientId) : null;
            Appointment appointment = model.AppointmentId > 0 ? unitOfWork.Repository<Appointment>().Get(model.AppointmentId) : null;

            if (patient == null) patient = appointment.Patient;

            var encounter = unitOfWork.Repository<Encounter>()
                .Queryable()
                .Include(p => p.Patient)
                .SingleOrDefault(e => e.Patient.Id == patient.Id && e.EncounterDate == model.EncounterDate && !e.Archived);

            if(encounter != null)
            {
                ModelState.AddModelError("EncounterDate", "Patient already has an encounter for this date. Please select a new date.");
            }
            if (model.EncounterDate > DateTime.Today)
            {
                ModelState.AddModelError("EncounterDate", "Encounter Date should be before current date");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    patientName = patient.FullName;

                    var encounterType = unitOfWork.Repository<EncounterType>().Queryable().SingleOrDefault(et => et.Id == model.EncounterTypeId);
                    var priority = unitOfWork.Repository<Priority>().Queryable().SingleOrDefault(p => p.Id == model.PriorityId);
                    var currentUser = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == User.Identity.Name);

                    var newEncounter = new Encounter(patient)
                    {
                        EncounterType = encounterType,
                        Priority = priority,
                        EncounterDate = model.EncounterDate,
                        Notes = model.EncounterNotes
                    };

                    unitOfWork.Repository<Encounter>().Save(newEncounter);

                    var encounterTypeWorkPlan = unitOfWork.Repository<EncounterTypeWorkPlan>()
                        .Queryable()
                        .Include("WorkPlan.Dataset")
                        .Where(et => et.EncounterType.Id == model.EncounterTypeId)
                        .SingleOrDefault();

                    if (encounterTypeWorkPlan != null)
                    {
                        // Create a new instance
                        var dataset = unitOfWork.Repository<Dataset>()
                            .Queryable()
                            .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType")
                            .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType")
                            .SingleOrDefault(d => d.Id == encounterTypeWorkPlan.WorkPlan.Dataset.Id);

                        if (dataset != null)
                        {
                            var datasetInstance = dataset.CreateInstance(newEncounter.Id, encounterTypeWorkPlan);

                            unitOfWork.Repository<DatasetInstance>().Save(datasetInstance);
                        }
                    }

                    return RedirectToAction("ViewEncounter/" + newEncounter.Id.ToString());
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to add the encounter: {0}", ex.Message));
                }
            }

            ViewBag.EncounterTypes = unitOfWork.Repository<EncounterType>()
                .Queryable()
                .Select(et => new SelectListItem { Value = et.Id.ToString(), Text = et.Description })
                .ToList();

            ViewBag.Priorities = unitOfWork.Repository<Priority>()
                .Queryable()
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Description })
                .ToList();

            ViewBag.PatientName = patientName;

            return View(model);

        }

        [HttpGet]
        public ActionResult viewEncounterOffline()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult ViewEncounter(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var encounter = unitOfWork.Repository<Encounter>()
                .Queryable()
                .Include(p => p.Patient)
                .SingleOrDefault(e => e.Id == id && !e.Archived && !e.Patient.Archived);

            if (encounter == null)
            {
                ViewBag.Entity = "Encounter";
                return View("NotFound");
            }

            TempData["returnUrl"] = null;

            ViewBag.PopUpMessage = PreparePopUpMessage();

            var patientUrl = "/Patient/PatientView.aspx?pid=" + encounter.Patient.Id.ToString();
            TempData["patientUrl"] = patientUrl;
            ViewBag.PatientUrl = patientUrl;
            var patientCohortGroups = unitOfWork.Repository<CohortGroupEnrolment>()
                .Queryable()
                .Where(cgr => cgr.Patient.Id == encounter.Patient.Id && !cgr.Archived && !cgr.Patient.Archived)
                .Select(cg => cg.CohortGroup.Id)
                .ToArray();

            var encounterViewModel = new EncounterViewModel
            {
                EncounterId = encounter.Id,
                PatientId = encounter.Patient.Id,
                PatientFullName = encounter.Patient.FullName,
                EncounterType = encounter.EncounterType.Description,
                EncounterDate = encounter.EncounterDate.ToString("yyyy-MM-dd"),
                EncounterNotes = encounter.Notes
            };

            encounterViewModel.DatasetCategories = new DatasetCategoryViewModel[0];
            encounterViewModel.DatasetInstanceId = 0;

            var datasetInstanceQuery = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Dataset.ContextType.Id == (int)ContextTypes.Encounter 
                    && di.ContextID == id 
                    && di.EncounterTypeWorkPlan.EncounterType.Id == encounter.EncounterType.Id);

            if (patientCohortGroups.Any())
            {
                datasetInstanceQuery.Where(diq => diq.EncounterTypeWorkPlan.CohortGroup == null || patientCohortGroups.Contains(diq.EncounterTypeWorkPlan.CohortGroup.Id));
            }

            var datasetInstance = datasetInstanceQuery.SingleOrDefault();

            if (datasetInstance != null)
            {
                encounterViewModel.DatasetInstanceId = datasetInstance.Id;

                var groupedDatasetCategories = datasetInstance.Dataset.DatasetCategories
                    .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                encounterViewModel.DatasetCategories = groupedDatasetCategories
                    .Select(dsc => new DatasetCategoryViewModel
                    {
                        DatasetCategoryId = dsc.Key.Id,
                        DatasetCategoryName = dsc.Key.DatasetCategoryName,
                        DatasetCategoryDisplayed = ShouldCategoryBeDisplayed(ref encounter, dsc.Key.Id),
                        DatasetElements = dsc.Select(e => new DatasetElementViewModel
                        {
                            DatasetElementId = e.DatasetElement.Id,
                            DatasetElementName = e.DatasetElement.ElementName,
                            DatasetElementDisplayName = String.IsNullOrWhiteSpace(e.FriendlyName) ? e.DatasetElement.ElementName : e.FriendlyName,
                            DatasetElementHelp = e.Help,
                            DatasetElementDisplayed = ShouldElementBeDisplayed(ref encounter, ref e),
                            DatasetElementChronic = IsElementChronic(ref encounter, ref e),
                            DatasetElementSystem = e.DatasetElement.System,
                            DatasetElementType = e.DatasetElement.Field.FieldType.Description,
                            DatasetElementValue = GetElementValue(ref datasetInstance, ref e, ref encounter),
                            DatasetElementSubs = e.DatasetElement.DatasetElementSubs.Select(es => new DatasetElementSubViewModel
                            {
                                DatasetElementSubId = es.Id,
                                DatasetElementSubName = es.ElementName,
                                DatasetElementSubType = es.Field.FieldType.Description
                            }).ToArray()
                        })
                        .ToArray()
                    })
                    .ToArray();
            }

            encounterViewModel.PatientConditions = unitOfWork.Repository<PatientCondition>()
                .Queryable()
                .Where(pc => pc.Patient.Id == encounter.Patient.Id && !pc.Archived && !pc.Patient.Archived)
                .ToArray()
                .Select(p => new PatientConditionListItemModel
                {
                    PatientConditionId = p.Id,
                    TerminologyMedDRA = p.TerminologyMedDra.MedDraTerm,
                    DateStart = p.DateStart,
                    DateEnd = p.OutcomeDate
                })
                .ToArray();

            var reportConfig = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Date of Report");
            var seriousConfig = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Is the adverse event serious?");
            encounterViewModel.PatientClinicalEvents = unitOfWork.Repository<PatientClinicalEvent>()
                .Queryable()
                .OrderBy(pce => pce.OnsetDate)
                .Where(pce => pce.Patient.Id == encounter.Patient.Id && !pce.Archived && !pce.Patient.Archived)
                .ToArray()
                .Select(c => new PatientClinicalEventListItemModel
                {
                    PatientClinicalEventId = c.Id,
                    SourceTerminologyMedDRA = c.SourceTerminologyMedDra.DisplayName,
                    OnsetDate = c.OnsetDate,
                    ReportedDate = _attributeService.GetCustomAttributeValue(reportConfig, (IExtendable)c),
                    ResolutionDate = c.ResolutionDate,
                    IsSerious  = _attributeService.GetCustomAttributeValue(seriousConfig, (IExtendable)c),
                })
                .ToArray();

            var indicationConfig = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Type of Indication");
            encounterViewModel.PatientMedications = unitOfWork.Repository<PatientMedication>()
                .Queryable()
                .Where(pm => pm.Patient.Id == encounter.Patient.Id && !pm.Archived && !pm.Patient.Archived)
                .ToArray()
                .Select(p => new PatientMedicationListItemModel
                {
                    PatientMedicationId = p.Id,
                    DrugName = p.Medication.FullName,
                    StartDate = p.DateStart,
                    EndDate = p.DateEnd,
                    Dose = p.Dose,
                    DoseFrequency = p.DoseFrequency,
                    DoseUnit = p.DoseUnit,
                    IndicationType = _attributeService.GetCustomAttributeValue(indicationConfig, (IExtendable)p)
                })
                .ToArray();

            encounterViewModel.PatientLabTests = unitOfWork.Repository<PatientLabTest>()
                .Queryable()
                .Include(pl => pl.LabTest)
                .Include(pl => pl.TestUnit)
                .Where(pm => pm.Patient.Id == encounter.Patient.Id && !pm.Archived && !pm.Patient.Archived)
                .ToArray()
                .Select(p => new PatientLabTestListItemModel
                {
                    PatientLabTestId = p.Id,
                    TestName = p.LabTest != null ? p.LabTest.Description : "",
                    TestDate = p.TestDate,
                    TestResult = p.TestResult,
                    TestUnit = p.TestUnit != null ? p.TestUnit.Description : "",
                    LabValue = p.LabValue,
                    Range = String.Format("{0}{1}{2}", !String.IsNullOrWhiteSpace(p.ReferenceLower) ? string.Format("Lower: {0}", p.ReferenceLower) : "", !String.IsNullOrWhiteSpace(p.ReferenceLower) ? "<br />" : "", !String.IsNullOrWhiteSpace(p.ReferenceUpper) ? string.Format("Upper: {0}", p.ReferenceUpper) : "")
                })
                .ToArray();

            int[] items = unitOfWork.Repository<PatientCondition>()
                .Queryable()
                .Where(pc => pc.Patient.Id == encounter.Patient.Id && !pc.Archived && !pc.Patient.Archived)
                .Select(p => p.TerminologyMedDra.Id)
                .ToArray();

            List<ConditionGroupListItemModel> cgarray = new List<ConditionGroupListItemModel>();
            foreach(var cm in unitOfWork.Repository<ConditionMedDra>().Queryable().Where(cm => items.Contains(cm.TerminologyMedDra.Id)).ToList())
            {
                var cgmod = new ConditionGroupListItemModel();
                cgmod.ConditionGroup = cm.Condition.Description;
                var tempCondition = cm.GetConditionForEncounter(encounter);
                if(tempCondition != null)
                {
                    cgmod.Status = tempCondition.OutcomeDate != null ? "Case Closed" : "Case Open";
                    cgmod.PatientConditionId = tempCondition.Id;
                    cgmod.StartDate = tempCondition.DateStart;
                    cgmod.Detail = String.Format("{0} started on {1}", tempCondition.TerminologyMedDra.DisplayName, tempCondition.DateStart.ToString("yyyy-MM-dd"));

                    cgarray.Add(cgmod);
                }
            }

            ConditionGroupListItemModel[] cgitems = (ConditionGroupListItemModel[])cgarray.OrderByDescending(m => m.StartDate).ToArray();
            encounterViewModel.ConditionGroups = cgitems;

            var weightElement = unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(de => de.ElementName == "Weight (kg)" && de.DatasetCategoryElements.Any(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment"));
            if(weightElement != null)
            {
                var weightModel = _patientService.GetElementValuesForPatient(encounter.Patient, weightElement, 5);
                encounterViewModel.InstanceValue = weightModel;
            };

            return View(encounterViewModel);
        }

        public ActionResult DeleteEncounter()
        {
            return PartialView("_DeleteEncounter");
        }

        public ActionResult DeletePatientCondition()
        {
            return PartialView("_DeletePatientCondition");
        }

        public ActionResult DeletePatientClinicalEvent()
        {
            return PartialView("_DeletePatientClinicalEvent");
        }

        public ActionResult DeletePatientMedication()
        {
            return PartialView("_DeletePatientMedication");
        }

        public ActionResult DeletePatientClinicalEvaluation()
        {
            return PartialView("_DeletePatientClinicalEvaluation");
        }

        [HttpPost]
        public JsonResult DeleteEncounter(DeleteDTO model)
        {
            if (!ModelState.IsValid) return Json(new {success = false}, JsonRequestBehavior.AllowGet);
            var encounterRepository = unitOfWork.Repository<Encounter>();
            var encounter = encounterRepository.Get(model.Id);
            var user = GetCurrentUser();
            if (user == null) return Json(new {success = false}, JsonRequestBehavior.AllowGet);
            
            var reason = model.Reason ?? "** NO REASON SPECIFIED ** ";
            var archiveDate = DateTime.Now;

            foreach (var attachment in encounter.Attachments.Where( x=>x.Archived))
            {
                attachment.Archived = true;
                attachment.ArchivedDate = archiveDate;
                attachment.ArchivedReason = reason;
                attachment.AuditUser = user;
                unitOfWork.Repository<Attachment>().Update(attachment);
            }

            foreach (var patientClinicalEvent in encounter.PatientClinicalEvents.Where(x => x.Archived))
            {
                patientClinicalEvent.Archived = true;
                patientClinicalEvent.ArchivedDate = archiveDate;
                patientClinicalEvent.ArchivedReason = reason;
                patientClinicalEvent.AuditUser = user;
                unitOfWork.Repository<PatientClinicalEvent>().Update(patientClinicalEvent);
            }

            encounter.Archived = true;
            encounter.ArchivedDate = archiveDate;
            encounter.ArchivedReason = reason;
            encounter.AuditUser = user;
            unitOfWork.Repository<Encounter>().Update(encounter);
            unitOfWork.Complete();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [MvcUnitOfWork]
        public ActionResult EditEncounter(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var encounter = unitOfWork.Repository<Encounter>()
                .Queryable()
                .Include(p => p.Patient)
                .SingleOrDefault(e => e.Id == id && !e.Archived && !e.Patient.Archived);

            if (encounter == null)
            {
                ViewBag.Entity = "Encounter";
                return View("NotFound");
            }

            var patientCohortGroups = unitOfWork.Repository<CohortGroupEnrolment>()
                .Queryable()
                .Where(cgr => cgr.Patient.Id == encounter.Patient.Id && !cgr.Archived && !cgr.Patient.Archived)
                .Select(cg => cg.CohortGroup.Id)
                .ToArray();

            var encounterEditModel = new EncounterEditModel
            {
                EncounterId = encounter.Id,
                PatientFullName = encounter.Patient.FullName,
                EncounterType = encounter.EncounterType.Description,
                EncounterDate = encounter.EncounterDate,
                EncounterNotes = encounter.Notes
            };

            encounterEditModel.DatasetCategories = new DatasetCategoryEditModel[0];
            encounterEditModel.DatasetInstanceId = 0;

            var datasetInstanceQuery = unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Dataset.ContextType.Id == (int)ContextTypes.Encounter
                    && di.ContextID == id
                    && di.EncounterTypeWorkPlan.EncounterType.Id == encounter.EncounterType.Id);

            if (patientCohortGroups.Any())
            {
                datasetInstanceQuery.Where(diq => diq.EncounterTypeWorkPlan.CohortGroup == null || patientCohortGroups.Contains(diq.EncounterTypeWorkPlan.CohortGroup.Id));
            }

            var datasetInstance = datasetInstanceQuery.SingleOrDefault();

            if (datasetInstance != null)
            {
                encounterEditModel.DatasetInstanceId = datasetInstance.Id;

                var groupedDatasetCategoryElements = datasetInstance.Dataset.DatasetCategories
                    .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                encounterEditModel.DatasetCategories = groupedDatasetCategoryElements
                    .Select(dsc => new DatasetCategoryEditModel
                    {
                        DatasetCategoryId = dsc.Key.Id,
                        DatasetCategoryDisplayName = String.IsNullOrWhiteSpace(dsc.Key.FriendlyName) ? dsc.Key.DatasetCategoryName : dsc.Key.FriendlyName,
                        DatasetCategoryHelp = dsc.Key.Help,
                        DatasetCategoryDisplayed = ShouldCategoryBeDisplayed(ref encounter, dsc.Key.Id),
                        DatasetElements = dsc.Select(e => new DatasetElementEditModel
                        {
                            DatasetElementId = e.DatasetElement.Id,
                            DatasetElementName = e.DatasetElement.ElementName,
                            DatasetElementDisplayName = String.IsNullOrWhiteSpace(e.FriendlyName) ? e.DatasetElement.ElementName : e.FriendlyName,
                            DatasetElementHelp = e.Help,
                            DatasetElementRequired = e.DatasetElement.Field.Mandatory,
                            DatasetElementDisplayed = ShouldElementBeDisplayed(ref encounter, ref e),
                            DatasetElementChronic = IsElementChronic(ref encounter, ref e),
                            DatasetElementSystem = e.DatasetElement.System,
                            DatasetElementType = e.DatasetElement.Field.FieldType.Description,
                            DatasetElementValue = GetElementValue(ref datasetInstance, ref e, ref encounter),
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
            }

            var selectTypeDatasetElements = encounterEditModel.DatasetCategories
                .SelectMany(dc => dc.DatasetElements)
                .Where(de => de.DatasetElementType == FieldTypes.Listbox.ToString()
                    || de.DatasetElementType == FieldTypes.DropDownList.ToString())
                .ToArray();

            var yesNoDatasetElements = encounterEditModel.DatasetCategories
                .SelectMany(dc => dc.DatasetElements)
                .Where(de => de.DatasetElementType == FieldTypes.YesNo.ToString())
                .ToArray();

            var datasetElementRepository = unitOfWork.Repository<DatasetElement>();

            foreach (var element in selectTypeDatasetElements)
            {
                var elementFieldValues = datasetElementRepository.Queryable()
                    .SingleOrDefault(de => de.Id == element.DatasetElementId)
                    .Field.FieldValues.OrderBy(fv => fv.Value)
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

            return View(encounterEditModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MvcUnitOfWork]
        public ActionResult EditEncounter(EncounterEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var encounterRepository = unitOfWork.Repository<Encounter>();
            var datasetInstanceRepository = unitOfWork.Repository<DatasetInstance>();

            var encounter = encounterRepository.Get(model.EncounterId);

            if (encounter == null)
            {
                ViewBag.Entity = "Encounter";
                return View("NotFound");
            }

            DatasetInstance datasetInstance = datasetInstanceRepository
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

            DateTime dttemp;

            for (int i = 0; i < model.DatasetCategories.Length; i++)
            {
                for (int j = 0; j < model.DatasetCategories[i].DatasetElements.Length; j++)
                {
                    if (model.DatasetCategories[i].DatasetElements[j].DatasetElementType == "Date")
                    {
                        if (model.DatasetCategories[i].DatasetElements[j].DatasetElementValue != null && model.DatasetCategories[i].DatasetElements[j].DatasetElementValue != "")
                        {
                            if(!DateTime.TryParse(model.DatasetCategories[i].DatasetElements[j].DatasetElementValue, out dttemp))
                            {
                                ModelState.AddModelError("", String.Format("{0} has an invalid date format", model.DatasetCategories[i].DatasetElements[j].DatasetElementName));
                            }
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                encounter.Notes = model.EncounterNotes;
                encounterRepository.Update(encounter);

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
                                datasetInstance.SetInstanceValue(datasetElements[model.DatasetCategories[i].DatasetElements[j].DatasetElementId], model.DatasetCategories[i].DatasetElements[j].DatasetElementValue);
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

                    return RedirectToAction("ViewEncounter", new { id = model.EncounterId });
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
                    DatasetCategoryDisplayed = ShouldCategoryBeDisplayed(ref encounter, dsc.Key.Id),
                    DatasetElements = dsc.Select(e => new DatasetElementEditModel
                    {
                        DatasetElementId = e.DatasetElement.Id,
                        DatasetElementName = e.DatasetElement.ElementName,
                        DatasetElementDisplayName = String.IsNullOrWhiteSpace(e.FriendlyName) ? e.DatasetElement.ElementName : e.FriendlyName,
                        DatasetElementHelp = e.Help,
                        DatasetElementRequired = e.DatasetElement.Field.Mandatory,
                        DatasetElementDisplayed = ShouldElementBeDisplayed(ref encounter, ref e),
                        DatasetElementChronic = IsElementChronic(ref encounter, ref e),
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
        public ActionResult ViewDatasetElementTable(int id, int encounterId, int datasetInstanceId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewBag.EncounterId = encounterId;
            ViewBag.DatasetInstanceId = datasetInstanceId;

            var datasetElement = unitOfWork.Repository<DatasetElement>().Get(id);

            ViewBag.DatasetElementName = datasetElement.ElementName;
            ViewBag.DatasetElementId = datasetElement.Id;

            var datasetElementSubs = unitOfWork.Repository<DatasetElementSub>()
                .Queryable()
                .Include(i => i.Field.FieldType)
                .Include(i => i.Field.FieldValues.Select(fv => fv.Field.FieldType))
                .Where(w => w.DatasetElement.Id == id)
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
                .Where(w => w.DatasetElementSub.DatasetElement.Id == id)
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

            return RedirectToAction("ViewDatasetElementTable", new { id, encounterId = encounterId, datasetInstanceId = datasetInstanceId });
        }

        private bool ShouldElementBeDisplayed(ref Encounter encounter, ref DatasetCategoryElement catElement)
        {
            bool display = true;

            //// ACUTE VS CHRONIC
            //if (encounter.EncounterType.Chronic == false) {
            //    // Encounter type is acute then element must have acute selected
            //    display = catElement.Acute ? true : false;
            //}
            //else
            //{
            //    // Encounter type is chronic then element must have chronic selected and patient must have condition
            //    if(catElement.Chronic)
            //    {
            //        List<Condition> conditions = new List<Condition>();
            //        foreach (var con in catElement.Conditions) {
            //            conditions.Add(con.Condition);
            //        }
            //        // Does patient have chronic condition
            //        if(encounter.Patient.HasCondition(conditions)) {
            //            display = true;
            //        }
            //        else {
            //            display = false;
            //        }
            //    }
            //    else {
            //        display = false;
            //    }
            //}

            if (catElement.Chronic)
            {
                List<Condition> conditions = new List<Condition>();
                foreach (var con in catElement.Conditions) {
                    conditions.Add(con.Condition);
                }
                // Does patient have chronic condition
                if (!encounter.Patient.HasCondition(conditions)) {
                    display = false;
                }
            }

            if(display)
            {
                IExtendable patient = encounter.Patient;

                var genderKey = patient.GetAttributeValue("Gender") != null ? patient.GetAttributeValue("Gender").ToString() : "";
                var genderItem = unitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(s => s.AttributeKey == "Gender" && s.SelectionKey == genderKey);
                var gender = "";

                if(genderItem != null) {
                    gender = genderItem.Value;
                }

                // Check dependencies - Gender
                if(gender == "Male")
                {
                    string[] ignore = { "Pregnancy Status", "Date of last menstrual period", "Estimated gestation (weeks)", "Breastfeeding mother" };

                    if(ignore.Contains(catElement.DatasetElement.ElementName)) {
                        display = false;
                    }
                }
            }
            
            return display;
        }

        private bool ShouldCategoryBeDisplayed(ref Encounter encounter, int id)
        {
            var cat = unitOfWork.Repository<DatasetCategory>().Get(id);

            bool display = true;

            // Encounter type is chronic then category must have chronic selected and patient must have condition
            if (cat.Chronic)
            {
                List<Condition> conditions = new List<Condition>();
                foreach (var con in cat.Conditions) {
                    conditions.Add(con.Condition);
                }
                // Does patient have chronic condition
                if (!encounter.Patient.HasCondition(conditions)) {
                    display = false;
                }
            }

            return display;
        }

        private bool IsElementChronic(ref Encounter encounter, ref DatasetCategoryElement catElement)
        {
            // Encounter type is chronic then element must have chronic selected and patient must have condition
            if (catElement.Chronic)
            {
                List<Condition> conditions = new List<Condition>();
                foreach (var con in catElement.Conditions)
                {
                    conditions.Add(con.Condition);
                }
                // Does patient have chronic condition
                if (encounter.Patient.HasCondition(conditions)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }

            return false;
        }

        private string GetElementValue(ref DatasetInstance datasetInstance, ref DatasetCategoryElement catElement, ref Encounter encounter)
        {
            var val = "";
            switch (catElement.DatasetElement.ElementName)
	        {
                case "BMI":
                    val = CalculateBMI(ref datasetInstance);
                    break;

                //case "Condition MedDra Terminology":
                //    if(catElement.DatasetCategory.Conditions.Count != 1) {
                //        val = "Unknown";
                //    }
                //    else
                //    {
                //        var pc = encounter.Patient.GetConditionForGroupAndDate(catElement.DatasetCategory.Conditions.First().Condition, encounter.EncounterDate);
                //        val = pc != null ? pc.TerminologyMedDra.DisplayName : "Unknown";
                //    }
                    
                //    break;

                //case "Condition Start Date":
                //    if(catElement.DatasetCategory.Conditions.Count != 1) {
                //        val = "Unknown";
                //    }
                //    else
                //    {
                //        var pc = encounter.Patient.GetConditionForGroupAndDate(catElement.DatasetCategory.Conditions.First().Condition, encounter.EncounterDate);
                //        val = pc != null ? pc.DateStart.ToString("yyyy-MM-dd") : "Unknown";
                //    }

                //    break;

                default:
                    val = datasetInstance.GetInstanceValue(catElement.DatasetElement);
                    break;
	        }
            return val;
        }

        private string CalculateBMI(ref DatasetInstance instance)
        {
            var weightElement = unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(de => de.ElementName == "Weight (kg)" && de.DatasetCategoryElements.Any(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment"));
            var heightElement = unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(de => de.ElementName == "Height (cm)" && de.DatasetCategoryElements.Any(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment"));

            if(weightElement == null || heightElement == null )  { return "Unable to calculate"; };

            var tempWeight = instance.GetInstanceValue(weightElement);
            var tempHeight = instance.GetInstanceValue(heightElement);

            if (String.IsNullOrWhiteSpace(tempWeight) || String.IsNullOrWhiteSpace(tempHeight)) { return "Unable to calculate"; };

            double BMI = Math.Round((Convert.ToDouble(tempWeight) / ((Convert.ToDouble(tempHeight) / 100.0) * (Convert.ToDouble(tempHeight) / 100.0))), 2);

            return BMI.ToString();
        }

        private User GetCurrentUser()
        {
            return unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
        }
    }
}