using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using VPS.Common.Repositories;
using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;

using PVIMS.Web.ActionFilters;
using PVIMS.Web.Models;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using FrameworkCustomAttributeConfiguration = VPS.CustomAttributes.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using PVIMS.Core.Models;

namespace PVIMS.Web.Controllers
{
    public class PatientClinicalEventController : BaseController
    {
        private static string CurrentMenuItem = "EncounterView";

        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workflowService;

        public PatientClinicalEventController(IUnitOfWorkInt unitOfWork, IWorkFlowService workFlowService)
        {
            _unitOfWork = unitOfWork;
            _workflowService = workFlowService;
        }

        [HttpGet]
        public ActionResult PatientClinicalEventOffline()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult AddPatientClinicalEvent(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var patient = _unitOfWork.Repository<Patient>().Get(id);

            if (patient == null)
            {
                ViewBag.Entity = "Patient";
                return View("NotFound");
            }

            // Prepare model
            var model = new PatientClincalEventAddModel { PatientId = id, PatientFullName = patient.FullName };

            // Prepare custom attributes
            var customAttributes = _unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientClinicalEvent).Name)
                .ToList();

            model.CustomAttributes = customAttributes.Select(c => new CustomAttributeAddModel
            {
                Name = c.AttributeKey,
                Detail = c.AttributeDetail == null ? "" : "(" + c.AttributeDetail + ")",
                Type = c.CustomAttributeType.ToString(),
                IsRequired = c.IsRequired,
                StringMaxLength = c.StringMaxLength,
                NumericMinValue = c.NumericMinValue,
                NumericMaxValue = c.NumericMaxValue,
                PastDateOnly = c.PastDateOnly,
                FutureDateOnly = c.FutureDateOnly
            })
            .ToArray();

            var selectiondataRepository = _unitOfWork.Repository<SelectionDataItem>();

            foreach (var selectCustomAttribute in customAttributes.Where(c => c.CustomAttributeType == VPS.CustomAttributes.CustomAttributeType.Selection))
            {
                ViewData[selectCustomAttribute.AttributeKey] = selectiondataRepository
                    .Queryable()
                    .Where(sd => sd.AttributeKey == selectCustomAttribute.AttributeKey)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SelectionKey,
                        Text = s.Value
                    })
                    .ToArray();
            }

            // Prepare drop downs
            ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                };

            // Prepare blank results
            ViewBag.TermResults = new[] { new SelectListItem { Value = "", Text = "" } };

            return View(model);
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult AddPatientClinicalEvent(PatientClincalEventAddModel model, string button)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            // Prepare entities
            var patient = _unitOfWork.Repository<Patient>().Get(model.PatientId);

            if (patient == null)
            {
                ViewBag.Entity = "Patient";
                return View("NotFound");
            }

            if (button != "Search")
            {
                if (model.SourceDescription == null)
                {
                    ModelState.AddModelError("SourceDescription", "Event Description is mandatory");
                }

                if (model.OnsetDate == null)
                {
                    ModelState.AddModelError("OnsetDate", "Onset Date is mandatory");
                }
                else
                {
                    if (model.OnsetDate > DateTime.Today)
                    {
                        ModelState.AddModelError("OnsetDate", "Onset Date should be before current date");
                    }
                    if (model.OnsetDate < patient.DateOfBirth)
                    {
                        ModelState.AddModelError("OnsetDate", "Onset Date should be after Date Of Birth");
                    }
                }

                if (model.ResolutionDate != null)
                {
                    if (model.ResolutionDate > DateTime.Today)
                    {
                        ModelState.AddModelError("ResolutionDate", "Resolution Date should be before current date");
                    }
                    if (model.ResolutionDate < model.OnsetDate)
                    {
                        ModelState.AddModelError("ResolutionDate", "Resolution Date should be after Onset Date");
                    }
                    if (model.ResolutionDate < patient.DateOfBirth)
                    {
                        ModelState.AddModelError("ResolutionDate", "Resolution Date should be after Date Of Birth");
                    }
                }

                DateTime tempDT;
                if(model.CustomAttributes[8].Value == "3") // Death
                {
                    if(DateTime.TryParse(model.CustomAttributes[11].Value, out tempDT))
                    {
                        var dateOfDeath = Convert.ToDateTime(model.CustomAttributes[11].Value);
                        if (dateOfDeath > DateTime.Today)
                        {
                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 11), string.Format("{0} should be before current date.", model.CustomAttributes[11].Name));
                        }
                        if (dateOfDeath < patient.DateOfBirth)
                        {
                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 11), string.Format("{0} should be after Date Of Birth.", model.CustomAttributes[11].Name));
                        }
                    }
                }

                if (model.CustomAttributes[8].Value == "4") // Hospitalisation
                {
                    DateTime admissionDate = DateTime.MinValue;
                    DateTime dischargeDate = DateTime.MinValue;

                    if (DateTime.TryParse(model.CustomAttributes[9].Value, out tempDT))
                    {
                        admissionDate = Convert.ToDateTime(model.CustomAttributes[9].Value);
                        if (admissionDate > DateTime.Today)
                        {
                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 9), string.Format("{0} should be before current date.", model.CustomAttributes[9].Name));
                        }
                        if (admissionDate < patient.DateOfBirth)
                        {
                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 9), string.Format("{0} should be after Date Of Birth.", model.CustomAttributes[9].Name));
                        }
                    }
                    if (DateTime.TryParse(model.CustomAttributes[10].Value, out tempDT))
                    {
                        dischargeDate = Convert.ToDateTime(model.CustomAttributes[10].Value);
                        if (dischargeDate > DateTime.Today)
                        {
                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 10), string.Format("{0} should be before current date.", model.CustomAttributes[10].Name));
                        }
                        if (dischargeDate < patient.DateOfBirth)
                        {
                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 10), string.Format("{0} should be after Date Of Birth.", model.CustomAttributes[10].Name));
                        }
                    }
                    if(admissionDate > DateTime.MinValue && dischargeDate > DateTime.MinValue)
                    {
                        if (admissionDate > dischargeDate)
                        {
                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 9), string.Format("{0} should be before {1}.", model.CustomAttributes[9].Name, model.CustomAttributes[10].Name));
                        }
                    }
                }

            }

            if (ModelState.IsValid)
            {
                // Which postback
                switch (button)
                {
                    case "Search":
                        // Prepare results from search
                        ViewBag.TermResults = _unitOfWork.Repository<TerminologyMedDra>()
                            .Queryable()
                            .Where(t => t.MedDraTermType == model.TermType && t.MedDraTerm.Contains(model.FindTerm))
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.MedDraTerm
                            })
                            .OrderBy(c => c.Text)
                            .ToArray();

                        break;

                    case "Submit":
                        TerminologyMedDra sourceTerm = null;
                        if (model.TermResult != null)
                        {
                            sourceTerm = _unitOfWork.Repository<TerminologyMedDra>().Get(Convert.ToInt32(model.TermResult));
                        }

                        if (sourceTerm == null)
                        {
                            ModelState.AddModelError("CustomError", "Please ensure a source term has been selected...");
                        }
                        else //validate source term
                        {
                            if(model.OnsetDate != null)
                            {
                                // Check clinical event overlapping - ONSET DATE
                                if (checkOnsetDateAgainstOnsetDateWithNoResolutionDate(sourceTerm.Id, patient.Id, Convert.ToDateTime(model.OnsetDate), 0))
                                {
                                    ModelState.AddModelError("OnsetDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                }
                                else
                                {
                                    if (checkOnsetDateWithinRange(sourceTerm.Id, patient.Id, Convert.ToDateTime(model.OnsetDate), 0))
                                    {
                                        ModelState.AddModelError("OnsetDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                    }
                                    else
                                    {
                                        if (model.ResolutionDate == null)
                                        {
                                            if (checkOnsetDateWithNoResolutionDateBeforeOnset(sourceTerm.Id, patient.Id, Convert.ToDateTime(model.OnsetDate), 0))
                                            {
                                                ModelState.AddModelError("OnsetDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                            }
                                        }
                                    }
                                }
                            }

                            // Check clinical event overlapping - RESOLUTION DATE
                            if (model.ResolutionDate != null)
                            {
                                if (checkResolutionDateAgainstOnsetDateWithNoResolutionDate(sourceTerm.Id, patient.Id, Convert.ToDateTime(model.ResolutionDate), 0))
                                {
                                    ModelState.AddModelError("ResolutionDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                }
                                else
                                {
                                    if (checkResolutionDateWithinRange(sourceTerm.Id, patient.Id, Convert.ToDateTime(model.ResolutionDate), 0))
                                    {
                                        ModelState.AddModelError("ResolutionDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                    }
                                }
                            }
                        }

                        //finally save
                        try
                        {
                            var patientClinicalEvent = new PatientClinicalEvent
                            {
                                SourceDescription = model.SourceDescription,
                                SourceTerminologyMedDra = sourceTerm,
                                OnsetDate = model.OnsetDate,
                                ResolutionDate = model.ResolutionDate,
                                Patient = patient
                            };

                            if (model.CustomAttributes != null)
                            {
                                var patientClinicalEventExtended = (IExtendable)patientClinicalEvent;
                                var customAttributes = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(ca => ca.ExtendableTypeName == typeof(PatientClinicalEvent).Name).ToList();

                                for (int i = 0; i < model.CustomAttributes.Length; i++)
                                {
                                    var attributeConfig = GetFrameworkCustomAttributeConfig(customAttributes, model.CustomAttributes[i].Name);

                                    // If there is not custom attribute configured with this name, ignore.
                                    if (attributeConfig == null)
                                    {
                                        continue;
                                    }

                                    try
                                    {
                                        if (attributeConfig.IsRequired && string.IsNullOrWhiteSpace(model.CustomAttributes[i].Value))
                                        {
                                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", i), string.Format("{0} is required.", model.CustomAttributes[i].Name));
                                            continue;
                                        }

                                        switch (model.CustomAttributes[i].Type)
                                        {
                                            case "Numeric":
                                                decimal number = 0M;
                                                if (decimal.TryParse(model.CustomAttributes[i].Value, out number))
                                                {
                                                    patientClinicalEventExtended.ValidateAndSetAttributeValue(attributeConfig, number, User.Identity.Name);
                                                }
                                                break;
                                            case "Selection":
                                                Int32 selection = 0;
                                                if (Int32.TryParse(model.CustomAttributes[i].Value, out selection))
                                                {
                                                    patientClinicalEventExtended.ValidateAndSetAttributeValue(attributeConfig, selection, User.Identity.Name);
                                                }
                                                break;
                                            case "DateTime":
                                                DateTime parsedDate = DateTime.MinValue;
                                                if (DateTime.TryParse(model.CustomAttributes[i].Value, out parsedDate))
                                                {
                                                    patientClinicalEventExtended.ValidateAndSetAttributeValue(attributeConfig, parsedDate, User.Identity.Name);
                                                }
                                                break;
                                            case "String":
                                            default:
                                                patientClinicalEventExtended.ValidateAndSetAttributeValue(attributeConfig, model.CustomAttributes[i].Value ?? string.Empty, User.Identity.Name);
                                                break;
                                        }

                                    }
                                    catch (CustomAttributeValidationException ve)
                                    {
                                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", i), ve.Message);
                                        continue;
                                    }
                                }
                            }

                            if (ModelState.IsValid)
                            {
                                _unitOfWork.Repository<PatientClinicalEvent>().Save(patientClinicalEvent);

                                // Instantiate new instance of work flow
                                _workflowService.CreateWorkFlowInstance("New Active Surveilliance Report", patientClinicalEvent.PatientClinicalEventGuid, patient.FullName, patientClinicalEvent.SourceTerminologyMedDra.DisplayName );

                                var weeks = 0;
                                var config = _unitOfWork.Repository<Config>().Queryable().Where(c => c.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks).SingleOrDefault();
                                if (config != null)
                                {
                                    if (!String.IsNullOrEmpty(config.ConfigValue))
                                    {
                                        weeks = Convert.ToInt32(config.ConfigValue);
                                    }
                                }

                                // Prepare medications
                                List<ReportInstanceMedicationListItem> medications = new List<ReportInstanceMedicationListItem>();
                                foreach (var med in patient.PatientMedications.Where(m => m.Archived == false && (m.DateEnd == null && m.DateStart.AddDays(weeks * -7) <= patientClinicalEvent.OnsetDate) || (m.DateEnd != null && m.DateStart.AddDays(weeks * -7) <= patientClinicalEvent.OnsetDate && Convert.ToDateTime(m.DateEnd).AddDays(weeks * 7) >= patientClinicalEvent.OnsetDate)).OrderBy(m => m.Medication.DrugName))
                                {
                                    var item = new ReportInstanceMedicationListItem()
                                    {
                                        MedicationIdentifier = med.DisplayName,
                                        ReportInstanceMedicationGuid = med.PatientMedicationGuid
                                    };
                                    medications.Add(item);
                                }
                                _workflowService.AddOrUpdateMedicationsForWorkFlowInstance(patientClinicalEvent.PatientClinicalEventGuid, medications);

                                _unitOfWork.Complete();

                                HttpCookie cookie = new HttpCookie("PopUpMessage");
                                cookie.Value = "Adverse event added successfully";
                                Response.Cookies.Add(cookie);

                                return Redirect("/Patient/PatientView.aspx?pid=" + patientClinicalEvent.Patient.Id.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("GenericError", string.Format("Unable to add the Patient Clinical Event: {0}", ex.Message));
                        }

                        // Prepare blank results
                        if (!String.IsNullOrWhiteSpace(model.FindTerm))
                        {
                            ViewBag.TermResults = _unitOfWork.Repository<TerminologyMedDra>()
                                .Queryable()
                                .Where(t => t.MedDraTermType == model.TermType && t.MedDraTerm.Contains(model.FindTerm))
                                .Select(c => new SelectListItem
                                {
                                    Value = c.Id.ToString(),
                                    Text = c.MedDraTerm
                                })
                                .OrderBy(c => c.Text)
                                .ToArray();
                        }
                        else
                        {
                            // Prepare blank results
                            ViewBag.TermResults = new[] { new SelectListItem { Value = "", Text = "" } };
                        }

                        break;

                    default:
                        break;
                }
            }
            else
            {
                // Prepare results from search
                if (!String.IsNullOrWhiteSpace(model.FindTerm))
                {
                    ViewBag.TermResults = _unitOfWork.Repository<TerminologyMedDra>()
                        .Queryable()
                        .Where(t => t.MedDraTermType == model.TermType && t.MedDraTerm.Contains(model.FindTerm))
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.MedDraTerm
                        })
                        .OrderBy(c => c.Text)
                        .ToArray();
                }
                else
                {
                    // Prepare blank results
                    ViewBag.TermResults = new[] { new SelectListItem { Value = "", Text = "" } };
                }
            }

            // Prepare custom attributes
            var cattributes = _unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientClinicalEvent).Name)
                .ToList();

            model.CustomAttributes = cattributes.Select(c => new CustomAttributeAddModel
            {
                Name = c.AttributeKey,
                Detail = c.AttributeDetail == null ? "" : "(" + c.AttributeDetail + ")",
                Type = c.CustomAttributeType.ToString(),
                IsRequired = c.IsRequired,
                StringMaxLength = c.StringMaxLength,
                NumericMinValue = c.NumericMinValue,
                NumericMaxValue = c.NumericMaxValue,
                PastDateOnly = c.PastDateOnly,
                FutureDateOnly = c.FutureDateOnly
            })
            .ToArray();

            // Load custom attributes
            var selectiondataRepository = _unitOfWork.Repository<SelectionDataItem>();

            foreach (var selectCustomAttribute in model.CustomAttributes.Where(c => c.Type == VPS.CustomAttributes.CustomAttributeType.Selection.ToString()))
            {
                ViewData[selectCustomAttribute.Name] = selectiondataRepository
                    .Queryable()
                    .Where(sd => sd.AttributeKey == selectCustomAttribute.Name)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SelectionKey,
                        Text = s.Value
                    })
                    .ToArray();
            }

            // Prepare drop downs
            ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                };

            TempData["returnUrl"] = returnUrl;

            return View(model);
        }

        private FrameworkCustomAttributeConfiguration GetFrameworkCustomAttributeConfig(List<CustomAttributeConfiguration> customAttributes, string customAttributeKey)
        {
            var localCustomAttributeConfig = customAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttributeKey);

            return new FrameworkCustomAttributeConfiguration
            {
                AttributeKey = localCustomAttributeConfig.AttributeKey,
                AttributeDetail = localCustomAttributeConfig.AttributeDetail,
                CustomAttributeType = localCustomAttributeConfig.CustomAttributeType,
                IsRequired = localCustomAttributeConfig.IsRequired,
                StringMaxLength = localCustomAttributeConfig.StringMaxLength,
                NumericMinValue = localCustomAttributeConfig.NumericMinValue,
                NumericMaxValue = localCustomAttributeConfig.NumericMaxValue,
                FutureDateOnly = localCustomAttributeConfig.FutureDateOnly,
                PastDateOnly = localCustomAttributeConfig.PastDateOnly
            };
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult EditPatientClinicalEvent(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            // Prepare clinical event
            var patientClinicalEvent = _unitOfWork.Repository<PatientClinicalEvent>()
                .Queryable()
                .Include(i1 => i1.Patient)
                .SingleOrDefault(p => p.Id == id);

            if (patientClinicalEvent == null)
            {
                ViewBag.Entity = "Patient Clinical Event";
                return View("NotFound");
            }

            string duration = "";

            if(patientClinicalEvent.OnsetDate != null && patientClinicalEvent.ResolutionDate != null)
            {
                System.DateTime onsetDate = Convert.ToDateTime(patientClinicalEvent.OnsetDate);
                System.DateTime resDate = Convert.ToDateTime(patientClinicalEvent.ResolutionDate);

                duration = string.Format("{0} day(s)", (resDate - onsetDate).TotalDays);
            }

            // Prepare model
            var model = new PatientClinicalEventEditModel
            {
                SourceDescription = patientClinicalEvent.SourceDescription,
                PatientFullName = patientClinicalEvent.Patient.FullName,
                PatientClinicalEventId = patientClinicalEvent.Id,
                SourceTerminologyMedDRA = patientClinicalEvent.SourceTerminologyMedDra.DisplayName,
                OnsetDate = patientClinicalEvent.OnsetDate,
                ResolutionDate = patientClinicalEvent.ResolutionDate,
                EventDuration = duration,
                EventAgeGroup = patientClinicalEvent.AgeGroup,
                MeddraSearch = "none"
            };

            // Prepare custom attributes
            var customAttributes = _unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == "PatientClinicalEvent")
                .ToList();

            var extendable = (IExtendable)patientClinicalEvent;

            model.CustomAttributes = customAttributes.Select(c => new CustomAttributeEditModel
            {
                Name = c.AttributeKey,
                Detail = c.AttributeDetail == null ? "" : "(" + c.AttributeDetail + ")",
                Type = c.CustomAttributeType.ToString(),
                Value = GetCustomAttributeVale(extendable, c),
                IsRequired = c.IsRequired,
                StringMaxLength = c.StringMaxLength,
                NumericMinValue = c.NumericMinValue,
                NumericMaxValue = c.NumericMaxValue,
                PastDateOnly = c.PastDateOnly,
                FutureDateOnly = c.FutureDateOnly
            })
           .ToArray();

            model.ReportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == patientClinicalEvent.PatientClinicalEventGuid);
            model.ReportingItems = _workflowService.GetExecutionStatusEventsForEventView(patientClinicalEvent);

            //prepare my dropdown list
            var selectiondataRepository = _unitOfWork.Repository<SelectionDataItem>();

            foreach (var selectCustomAttribute in customAttributes.Where(c => c.CustomAttributeType == VPS.CustomAttributes.CustomAttributeType.Selection))
            {
                ViewData[selectCustomAttribute.AttributeKey] = selectiondataRepository
                    .Queryable()
                    .Where(sd => sd.AttributeKey == selectCustomAttribute.AttributeKey)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SelectionKey,
                        Text = s.Value,

                    })
                    .ToArray();
            }

            //var sqlQuery = ("select * from CustomAttributeConfiguration c where c.ExtendableTypeName='PatientClinicalEvent' and c.CustomAttributeType=3");
            //var selectMethodQuery = new SelectList(unitOfWork.Repository<CustomAttributeConfiguration>().ExecuteSql(sqlQuery));
            // Prepare drop downs
            ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                };

            //Prepare blank results
            ViewBag.TermResults = new[] { new SelectListItem { Value = "", Text = "" } };
            //ViewBag.SelectionDropDown =new se ViewData[selectCustomAttribute.AttributeKey];
            return View(model);
        }

        [MvcUnitOfWork]
        [HttpPost]
        public ActionResult EditPatientClinicalEvent(PatientClinicalEventEditModel model, string button)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            model.MeddraSearch = "none";

            // Prepare entities
            var patientClinicalEvent = _unitOfWork.Repository<PatientClinicalEvent>().Queryable()
                .Include(i1 => i1.Patient).Single(i2 => i2.Id == model.PatientClinicalEventId);

            if (patientClinicalEvent == null)
            {
                ViewBag.Entity = "PatientClinicalEvent";
                return View("NotFound");
            }

            if (model.OnsetDate != null)
            {
                if (model.OnsetDate > DateTime.Today)
                {
                    ModelState.AddModelError("OnsetDate", "Onset Date should be before current date");
                }
                if (model.OnsetDate < patientClinicalEvent.Patient.DateOfBirth)
                {
                    ModelState.AddModelError("OnsetDate", "Onset Date should be after Date Of Birth");
                }
            }
            if (model.ResolutionDate != null)
            {
                if (model.ResolutionDate > DateTime.Today)
                {
                    ModelState.AddModelError("ResolutionDate", "Resolution Date should be before current date");
                }
                if (model.ResolutionDate < model.OnsetDate)
                {
                    ModelState.AddModelError("ResolutionDate", "Resolution Date should be after Onset Date");
                }
                if (model.ResolutionDate < patientClinicalEvent.Patient.DateOfBirth)
                {
                    ModelState.AddModelError("ResolutionDate", "Resolution Date should be after Date Of Birth");
                }
            }
            DateTime tempDT;
            if (model.CustomAttributes[8].Value == "3") // Death
            {
                if (DateTime.TryParse(model.CustomAttributes[11].Value, out tempDT))
                {
                    var dateOfDeath = Convert.ToDateTime(model.CustomAttributes[11].Value);
                    if (dateOfDeath > DateTime.Today)
                    {
                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 11), string.Format("{0} should be before current date.", model.CustomAttributes[11].Name));
                    }
                    if (dateOfDeath < patientClinicalEvent.Patient.DateOfBirth)
                    {
                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 11), string.Format("{0} should be after Date Of Birth.", model.CustomAttributes[11].Name));
                    }
                }
            }

            if (model.CustomAttributes[8].Value == "4") // Hospitalisation
            {
                DateTime admissionDate = DateTime.MinValue;
                DateTime dischargeDate = DateTime.MinValue;

                if (DateTime.TryParse(model.CustomAttributes[9].Value, out tempDT))
                {
                    admissionDate = Convert.ToDateTime(model.CustomAttributes[9].Value);
                    if (admissionDate > DateTime.Today)
                    {
                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 9), string.Format("{0} should be before current date.", model.CustomAttributes[9].Name));
                    }
                    if (admissionDate < patientClinicalEvent.Patient.DateOfBirth)
                    {
                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 9), string.Format("{0} should be after Date Of Birth.", model.CustomAttributes[9].Name));
                    }
                }
                if (DateTime.TryParse(model.CustomAttributes[10].Value, out tempDT))
                {
                    dischargeDate = Convert.ToDateTime(model.CustomAttributes[10].Value);
                    if (dischargeDate > DateTime.Today)
                    {
                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 10), string.Format("{0} should be before current date.", model.CustomAttributes[10].Name));
                    }
                    if (dischargeDate < patientClinicalEvent.Patient.DateOfBirth)
                    {
                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 10), string.Format("{0} should be after Date Of Birth.", model.CustomAttributes[10].Name));
                    }
                }
                if (admissionDate > DateTime.MinValue && dischargeDate > DateTime.MinValue)
                {
                    if (admissionDate > dischargeDate)
                    {
                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", 9), string.Format("{0} should be before {1}.", model.CustomAttributes[9].Name, model.CustomAttributes[10].Name));
                    }
                }
            }

            if (ModelState.IsValid)
            {
                // Which postback
                switch (button)
                {
                    case "Search":
                        // Prepare results from search
                        ViewBag.TermResults = _unitOfWork.Repository<TerminologyMedDra>()
                            .Queryable()
                            .Where(t => t.MedDraTermType == model.TermType && t.MedDraTerm.Contains(model.FindTerm))
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.MedDraTerm
                            })
                            .OrderBy(c => c.Text)
                            .ToArray();

                        model.MeddraSearch = "block";

                        break;

                    case "Submit":
                        TerminologyMedDra sourceTerm = null;
                        if (model.TermResult != null)
                        {
                            if (Convert.ToInt32(model.TermResult) != default(int))
                            {
                                sourceTerm = _unitOfWork.Repository<TerminologyMedDra>().Get(Convert.ToInt32(model.TermResult));
                            }
                        }
                        else {
                            sourceTerm = patientClinicalEvent.SourceTerminologyMedDra;
                        }

                        if (sourceTerm != null)
                        {
                            if (model.OnsetDate != null)
                            {
                                // Check clinical event overlapping - ONSET DATE
                                if (checkOnsetDateAgainstOnsetDateWithNoResolutionDate(sourceTerm.Id, patientClinicalEvent.Patient.Id, Convert.ToDateTime(model.OnsetDate), patientClinicalEvent.Id))
                                {
                                    ModelState.AddModelError("OnsetDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                }
                                else
                                {
                                    if (checkOnsetDateWithinRange(sourceTerm.Id, patientClinicalEvent.Patient.Id, Convert.ToDateTime(model.OnsetDate), patientClinicalEvent.Id))
                                    {
                                        ModelState.AddModelError("OnsetDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                    }
                                    else
                                    {
                                        if (model.ResolutionDate == null)
                                        {
                                            if (checkOnsetDateWithNoResolutionDateBeforeOnset(sourceTerm.Id, patientClinicalEvent.Patient.Id, Convert.ToDateTime(model.OnsetDate), patientClinicalEvent.Id))
                                            {
                                                ModelState.AddModelError("OnsetDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                            }
                                        }
                                    }
                                }
                            }

                            // Check clinical event overlapping - RESOLUTION DATE
                            if (model.ResolutionDate != null)
                            {
                                if (checkResolutionDateAgainstOnsetDateWithNoResolutionDate(sourceTerm.Id, patientClinicalEvent.Patient.Id, Convert.ToDateTime(model.ResolutionDate), patientClinicalEvent.Id))
                                {
                                    ModelState.AddModelError("ResolutionDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                }
                                else
                                {
                                    if (checkResolutionDateWithinRange(sourceTerm.Id, patientClinicalEvent.Patient.Id, Convert.ToDateTime(model.ResolutionDate), patientClinicalEvent.Id))
                                    {
                                        ModelState.AddModelError("ResolutionDate", "Duplication of adverse event. Please check onset and resolution dates...");
                                    }
                                }
                            }
                        }
                        try
                        {
                            if (sourceTerm != null) { patientClinicalEvent.SourceTerminologyMedDra = sourceTerm; };
                            patientClinicalEvent.SourceDescription = model.SourceDescription;
                            patientClinicalEvent.OnsetDate = model.OnsetDate;
                            patientClinicalEvent.ResolutionDate = model.ResolutionDate;

                            if (model.CustomAttributes != null)
                            {
                                var patientClinicalEventExtended = (IExtendable)patientClinicalEvent;
                                var customAttributes = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(ca => ca.ExtendableTypeName == typeof(PatientClinicalEvent).Name).ToList();

                                for (int i = 0; i < model.CustomAttributes.Length; i++)
                                {
                                    var attributeConfig = GetFrameworkCustomAttributeConfig(customAttributes, model.CustomAttributes[i].Name);

                                    // If there is not custom attribute configured with this name, ignore.
                                    if (attributeConfig == null)
                                    {
                                        continue;
                                    }

                                    try
                                    {
                                        if (attributeConfig.IsRequired && string.IsNullOrWhiteSpace(model.CustomAttributes[i].Value))
                                        {
                                            ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", i), string.Format("{0} is required.", model.CustomAttributes[i].Name));
                                            continue;
                                        }

                                        switch (model.CustomAttributes[i].Type)
                                        {
                                            case "Numeric":
                                                decimal number = 0M;
                                                if (decimal.TryParse(model.CustomAttributes[i].Value, out number))
                                                {
                                                    patientClinicalEventExtended.ValidateAndSetAttributeValue(attributeConfig, number, User.Identity.Name);
                                                }
                                                break;
                                            case "Selection":
                                                Int32 selection = 0;
                                                if (Int32.TryParse(model.CustomAttributes[i].Value, out selection))
                                                {
                                                    patientClinicalEventExtended.ValidateAndSetAttributeValue(attributeConfig, selection, User.Identity.Name);
                                                }
                                                break;
                                            case "DateTime":
                                                DateTime parsedDate = DateTime.MinValue;
                                                if (DateTime.TryParse(model.CustomAttributes[i].Value, out parsedDate))
                                                {
                                                    patientClinicalEventExtended.ValidateAndSetAttributeValue(attributeConfig, parsedDate, User.Identity.Name);
                                                }
                                                break;
                                            case "String":
                                            default:
                                                patientClinicalEventExtended.ValidateAndSetAttributeValue(attributeConfig, model.CustomAttributes[i].Value ?? string.Empty, User.Identity.Name);
                                                break;
                                        }

                                    }
                                    catch (CustomAttributeValidationException ve)
                                    {
                                        ModelState.AddModelError(string.Format("CustomAttributes[{0}].Value", i), ve.Message);
                                        continue;
                                    }
                                }
                            }

                            if (ModelState.IsValid)
                            {
                                _unitOfWork.Repository<PatientClinicalEvent>().Update(patientClinicalEvent);
                                _workflowService.UpdateIdentifiersForWorkFlowInstance(patientClinicalEvent.PatientClinicalEventGuid, patientClinicalEvent.Patient.FullName, patientClinicalEvent.SourceTerminologyMedDra.DisplayName);

                                _unitOfWork.Complete();

                                HttpCookie cookie = new HttpCookie("PopUpMessage");
                                cookie.Value = "Adverse event updated successfully";
                                Response.Cookies.Add(cookie);

                                //if (returnUrl != string.Empty)
                                //{
                                //    return Redirect(returnUrl);
                                //}
                                return Redirect("/Patient/PatientView.aspx?pid=" + patientClinicalEvent.Patient.Id.ToString());
                            }
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            ModelState.AddModelError("GenericError", string.Format("Unable to update the Patient Condition: {0}", e.Message));
                        }

                        // Prepare blank results
                        ViewBag.TermResults = new[] { new SelectListItem { Value = "", Text = "" } };

                        break;

                    default:
                        break;
                }
            }
            else
            {
                // Prepare results from search
                if (!String.IsNullOrWhiteSpace(model.FindTerm))
                {
                    ViewBag.TermResults = _unitOfWork.Repository<TerminologyMedDra>()
                        .Queryable()
                        .Where(t => t.MedDraTermType == model.TermType && t.MedDraTerm.Contains(model.FindTerm))
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.MedDraTerm
                        })
                        .OrderBy(c => c.Text)
                        .ToArray();
                }
                else
                {
                    // Prepare blank results
                    ViewBag.TermResults = new[] { new SelectListItem { Value = "", Text = "" } };
                }
            }

            // Prepare custom attributes
            var cattributes = _unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientClinicalEvent).Name)
                .ToList();

            model.CustomAttributes = cattributes.Select(c => new CustomAttributeEditModel
            {
                Name = c.AttributeKey,
                Detail = c.AttributeDetail == null ? "" : "(" + c.AttributeDetail + ")",
                Type = c.CustomAttributeType.ToString(),
                IsRequired = c.IsRequired,
                StringMaxLength = c.StringMaxLength,
                NumericMinValue = c.NumericMinValue,
                NumericMaxValue = c.NumericMaxValue,
                PastDateOnly = c.PastDateOnly,
                FutureDateOnly = c.FutureDateOnly
            })
            .ToArray();

            // Load custom attributes
            var selectiondataRepository = _unitOfWork.Repository<SelectionDataItem>();

            foreach (var selectCustomAttribute in model.CustomAttributes.Where(c => c.Type == VPS.CustomAttributes.CustomAttributeType.Selection.ToString()))
            {
                ViewData[selectCustomAttribute.Name] = selectiondataRepository
                    .Queryable()
                    .Where(sd => sd.AttributeKey == selectCustomAttribute.Name)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SelectionKey,
                        Text = s.Value
                    })
                    .ToArray();
            }

            // Prepare drop downs
            ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                };

            TempData["returnUrl"] = returnUrl;

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeletePatientClinicalEvent(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            // Prepare clinical event
            var patientClinicalEvent = _unitOfWork.Repository<PatientClinicalEvent>()
                .Queryable()
                .Include(i1 => i1.Patient)
                .SingleOrDefault(p => p.Id == id);

            if (patientClinicalEvent == null)
            {
                ViewBag.Entity = "Patient Clinical Event";
                return View("NotFound");
            }

            string duration = "";

            if (patientClinicalEvent.OnsetDate != null && patientClinicalEvent.ResolutionDate != null)
            {
                System.DateTime onsetDate = Convert.ToDateTime(patientClinicalEvent.OnsetDate);
                System.DateTime resDate = Convert.ToDateTime(patientClinicalEvent.ResolutionDate);

                duration = string.Format("{0} day(s)", (resDate - onsetDate).TotalDays);
            }

            // Prepare model
            var model = new PatientClinicalEventDeleteModel
            {
                SourceDescription = patientClinicalEvent.SourceDescription,
                PatientFullName = patientClinicalEvent.Patient.FullName,
                PatientClinicalEventId = patientClinicalEvent.Id,
                SourceTerminologyMedDRA = patientClinicalEvent.SourceTerminologyMedDra.DisplayName,
                OnsetDate = patientClinicalEvent.OnsetDate,
                ResolutionDate = patientClinicalEvent.ResolutionDate,
                EventDuration = duration,
                EventAgeGroup = patientClinicalEvent.AgeGroup
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeletePatientClinicalEvent(PatientClinicalEventDeleteModel model)
        {
            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var patientClinicalEventRepository = _unitOfWork.Repository<PatientClinicalEvent>();
            var patientClinicalEvent = patientClinicalEventRepository.Queryable()
                                                                    .Include(x => x.Patient)
                                                                    .FirstOrDefault(x => x.Id == model.PatientClinicalEventId);
            if (patientClinicalEvent != null)
            {
                var user = GetCurrentUser();

                if (user != null)
                {
                    if (ModelState.IsValid)
                    {
                        var reason = model.ArchiveReason ?? "** NO REASON SPECIFIED ** ";
                        patientClinicalEvent.Archived = true;
                        patientClinicalEvent.ArchivedDate = DateTime.Now;
                        patientClinicalEvent.ArchivedReason = reason;
                        patientClinicalEvent.AuditUser = user;
                        patientClinicalEventRepository.Update(patientClinicalEvent);
                        _unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Adverse event record deleted successfully";
                        Response.Cookies.Add(cookie);

                        //return Redirect(returnUrl);
                        return Redirect("/Patient/PatientView.aspx?pid=" + patientClinicalEvent.Patient.Id.ToString());
                    }
                }
            }

            TempData["returnUrl"] = returnUrl;

            return View(model);
        }

        private string GetCustomAttributeVale(IExtendable extendable, CustomAttributeConfiguration customAttribute)
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

        private Boolean checkOnsetDateAgainstOnsetDateWithNoResolutionDate(int sourceTerm_id, int patient_id, DateTime onsetDate, int patientClinicalEventId)
        {
            List<PatientClinicalEvent> patientClinicalEvents;

            if (patientClinicalEventId > 0)
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientClinicalEventId && pce.SourceTerminologyMedDra.Id == sourceTerm_id && pce.OnsetDate <= onsetDate && pce.ResolutionDate == null && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.SourceTerminologyMedDra.Id == sourceTerm_id && pce.OnsetDate <= onsetDate && pce.ResolutionDate == null && pce.Archived == false)
                        .ToList();
            }

            if (patientClinicalEvents.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private Boolean checkOnsetDateWithinRange(int sourceTerm_id, int patient_id, DateTime onsetDate, int patientClinicalEventId)
        {
            List<PatientClinicalEvent> patientClinicalEvents;

            if (patientClinicalEventId > 0)
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientClinicalEventId && pce.SourceTerminologyMedDra.Id == sourceTerm_id && onsetDate >= pce.OnsetDate && onsetDate <= pce.ResolutionDate && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.SourceTerminologyMedDra.Id == sourceTerm_id && onsetDate >= pce.OnsetDate && onsetDate <= pce.ResolutionDate && pce.Archived == false)
                        .ToList();
            }

            if (patientClinicalEvents.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private Boolean checkOnsetDateWithNoResolutionDateBeforeOnset(int sourceTerm_id, int patient_id, DateTime onsetDate, int patientClinicalEventId)
        {
            List<PatientClinicalEvent> patientClinicalEvents;

            if (patientClinicalEventId > 0)
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientClinicalEventId && pce.SourceTerminologyMedDra.Id == sourceTerm_id && onsetDate < pce.OnsetDate && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.SourceTerminologyMedDra.Id == sourceTerm_id && onsetDate < pce.OnsetDate && pce.Archived == false)
                        .ToList();
            }

            if (patientClinicalEvents.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private Boolean checkResolutionDateAgainstOnsetDateWithNoResolutionDate(int sourceTerm_id, int patient_id, DateTime resolutionDate, int patientClinicalEventId)
        {
            List<PatientClinicalEvent> patientClinicalEvents;

            if (patientClinicalEventId > 0)
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientClinicalEventId && pce.SourceTerminologyMedDra.Id == sourceTerm_id && pce.OnsetDate <= resolutionDate && pce.ResolutionDate == null && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.SourceTerminologyMedDra.Id == sourceTerm_id && pce.OnsetDate <= resolutionDate && pce.ResolutionDate == null && pce.Archived == false)
                        .ToList();
            }

            if (patientClinicalEvents.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private Boolean checkResolutionDateWithinRange(int sourceTerm_id, int patient_id, DateTime resolutionDate, int patientClinicalEventId)
        {
            List<PatientClinicalEvent> patientClinicalEvents;

            if (patientClinicalEventId > 0)
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientClinicalEventId && pce.SourceTerminologyMedDra.Id == sourceTerm_id && resolutionDate >= pce.OnsetDate && resolutionDate <= pce.ResolutionDate && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientClinicalEvents = _unitOfWork.Repository<PatientClinicalEvent>()
                        .Queryable()
                        .OrderBy(pce => pce.OnsetDate)
                        .Where(pce => pce.Patient.Id == patient_id && pce.SourceTerminologyMedDra.Id == sourceTerm_id && resolutionDate >= pce.OnsetDate && resolutionDate <= pce.ResolutionDate && pce.Archived == false)
                        .ToList();
            }

            if (patientClinicalEvents.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private User GetCurrentUser()
        {
            return _unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
        }

    }
}