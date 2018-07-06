using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using VPS.Common.Repositories;
using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;

using PVIMS.Web.Models;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using FrameworkCustomAttributeConfiguration = VPS.CustomAttributes.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using PVIMS.Web.ActionFilters;

namespace PVIMS.Web.Controllers
{
    public class PatientMedicationController : BaseController
    {
        private static string CurrentMenuItem = "EncounterView";

        private readonly IUnitOfWorkInt unitOfWork;
        private readonly IWorkFlowService _workflowService;

        public PatientMedicationController(IUnitOfWorkInt unitOfWork, IWorkFlowService workFlowService)
        {
            this.unitOfWork = unitOfWork;
            _workflowService = workFlowService;
        }

        // PatientMedicatio Offline view
        public ActionResult PatientMedicationView()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult AddPatientMedication(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var patient = unitOfWork.Repository<Patient>().Get(id);

            if (patient == null)
            {
                ViewBag.Entity = "Patient";
                return View("NotFound");
            }

            var model = new PatientMedicationAddModel { PatientId = id, PatientFullName = patient.FullName };

            var medications = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            medications.AddRange(unitOfWork.Repository<Medication>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.DrugName.Trim()
                })
                .OrderBy(c => c.Text)
                .ToArray());
            ViewBag.Medications = medications;

            var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == "PatientMedication")
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

            var selectiondataRepository = unitOfWork.Repository<SelectionDataItem>();

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
            ViewBag.DoseUnits = new[]
                {
                    new SelectListItem { Value = "", Text = "" },
                    new SelectListItem { Value = "Bq", Text = "becquerel" },
                    new SelectListItem { Value = "Ci", Text = "curie" },
                    new SelectListItem { Value = "{DF}", Text = "Dosage form" },
                    new SelectListItem { Value = "[drp]", Text = "drop" },
                    new SelectListItem { Value = "GBq", Text = "gigabecquerel" },
                    new SelectListItem { Value = "g", Text = "gram" },
                    new SelectListItem { Value = "[iU]", Text = "International unit" },
                    new SelectListItem { Value = "[iU]/kg", Text = "International unit/kilogram" },
                    new SelectListItem { Value = "kBq", Text = "killobecquerel" },
                    new SelectListItem { Value = "kg", Text = "kilogram" },
                    new SelectListItem { Value = "k[iU]", Text = "kilo-international unit" },
                    new SelectListItem { Value = "L", Text = "liter" },
                    new SelectListItem { Value = "MBq", Text = "megabecquerel" },
                    new SelectListItem { Value = "M[iU]", Text = "mega-international unit" },
                    new SelectListItem { Value = "uCi", Text = "microcurie" },
                    new SelectListItem { Value = "ug", Text = "microgram" },
                    new SelectListItem { Value = "ug/kg", Text = "microgram/kilogram" },
                    new SelectListItem { Value = "uL", Text = "microliter" },
                    new SelectListItem { Value = "mCi", Text = "millicurie" },
                    new SelectListItem { Value = "meq", Text = "milliequivalent" },
                    new SelectListItem { Value = "mg", Text = "milligram" },
                    new SelectListItem { Value = "mg/kg", Text = "milligram/kilogram" },
                    new SelectListItem { Value = "mg/m2", Text = "milligram/sq.meter" },
                    new SelectListItem { Value = "ug/m2", Text = "microgram/sq.meter" },
                    new SelectListItem { Value = "mL", Text = "milliliter" },
                    new SelectListItem { Value = "mmol", Text = "millimole" },
                    new SelectListItem { Value = "mol", Text = "mole" },
                    new SelectListItem { Value = "nCi", Text = "nanocurie" },
                    new SelectListItem { Value = "ng", Text = "nanogram" },
                    new SelectListItem { Value = "%", Text = "percent" },
                    new SelectListItem { Value = "pg", Text = "picogram" }
                };

            return View(model);
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult AddPatientMedication(PatientMedicationAddModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var patient = unitOfWork.Repository<Patient>().Get(model.PatientId);

            if (patient == null)
            {
                ViewBag.Entity = "Patient";
                return View("NotFound");
            }

            // Date validation
            if (model.StartDate != null)
            {
                if (model.StartDate > DateTime.Today)
                {
                    ModelState.AddModelError("StartDate", "Start Date should be before current date");
                }
                if (model.StartDate < patient.DateOfBirth)
                {
                    ModelState.AddModelError("StartDate", "Start Date should be after Date Of Birth");
                }
            }
            else
            {
                ModelState.AddModelError("StartDate", "Start Date is mandatory");
            }

            if (model.EndDate != null)
            {
                if (model.EndDate > DateTime.Today)
                {
                    ModelState.AddModelError("EndDate", "End Date should be before current date");
                }
                if (model.EndDate < model.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End Date should be after Start Date");
                }
            }

            if (ModelState.IsValid)
            {
                var medication = unitOfWork.Repository<Medication>().Get(model.MedicationId);

                if (medication == null)
                {
                    ViewBag.Entity = "Medication";
                    return View("NotFound");
                }
                if (medication != null)
                {
                    var startDate = Convert.ToDateTime(model.StartDate);
                    // Check medication overlapping - START DATE
                    if (checkStartDateAgainstStartDateWithNoEndDate(medication.Id, model.PatientId, startDate, 0))
                    {
                        ModelState.AddModelError("StartDate", "Duplication of medication. Please check start and end dates...");
                    }
                    else
                    {
                        if (checkStartDateWithinRange(medication.Id, model.PatientId, startDate, 0))
                        {
                            ModelState.AddModelError("StartDate", "Duplication of medication. Please check start and end dates...");
                        }
                        else
                        {
                            if (model.EndDate == null)
                            {
                                if (checkStartDateWithNoEndDateBeforeStart(medication.Id, model.PatientId, startDate, 0))
                                {
                                    ModelState.AddModelError("StartDate", "Duplication of medication. Please check start and end dates...");
                                }
                            }
                        }
                    }

                    // Check medication overlapping - END DATE
                    if(model.EndDate != null)
                    {
                        if (checkEndDateAgainstStartDateWithNoEndDate(medication.Id, model.PatientId, Convert.ToDateTime(model.EndDate), 0))
                        {
                            ModelState.AddModelError("EndDate", "Duplication of medication. Please check start and end dates...");
                        }
                        else
                        {
                            if (checkEndDateWithinRange(medication.Id, model.PatientId, Convert.ToDateTime(model.EndDate), 0))
                            {
                                ModelState.AddModelError("EndDate", "Duplication of medication. Please check start and end dates...");
                            }
                        }
                    }
                }
                try
                {
                    var patientMedication = new PatientMedication
                    {
                        Patient = patient,
                        DateStart = Convert.ToDateTime(model.StartDate),
                        DateEnd = model.EndDate,
                        Medication = medication,
                        Dose = model.Dose,
                        DoseFrequency = model.DoseFrequency,
                        DoseUnit = model.DoseUnit
                    };

                    if (model.CustomAttributes != null)
                    {
                        var patientMedicationExtended = (IExtendable)patientMedication;
                        var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(ca => ca.ExtendableTypeName == typeof(PatientMedication).Name).ToList();

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
                                            patientMedicationExtended.ValidateAndSetAttributeValue(attributeConfig, number, User.Identity.Name);
                                        }
                                        break;
                                    case "Selection":
                                        Int32 selection = 0;
                                        if (Int32.TryParse(model.CustomAttributes[i].Value, out selection))
                                        {
                                            patientMedicationExtended.ValidateAndSetAttributeValue(attributeConfig, selection, User.Identity.Name);
                                        }
                                        break;
                                    case "DateTime":
                                        DateTime parsedDate = DateTime.MinValue;
                                        if (DateTime.TryParse(model.CustomAttributes[i].Value, out parsedDate))
                                        {
                                            patientMedicationExtended.ValidateAndSetAttributeValue(attributeConfig, parsedDate, User.Identity.Name);
                                        }
                                        break;
                                    case "String":
                                    default:
                                        patientMedicationExtended.ValidateAndSetAttributeValue(attributeConfig, model.CustomAttributes[i].Value ?? string.Empty, User.Identity.Name);
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
                        unitOfWork.Repository<PatientMedication>().Save(patientMedication);
                        AddOrUpdateMedicationsToReportInstance(patientMedication);
                        unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Medication added successfully";
                        Response.Cookies.Add(cookie);

                        return Redirect("/Patient/PatientView.aspx?pid=" + patientMedication.Patient.Id.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("MedicationId", string.Format("Unable to add the Patient Medication: {0}", ex.Message));
                }
            }

            // Prepare custom attributes
            var cattributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientMedication).Name)
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

            var selectiondataRepository = unitOfWork.Repository<SelectionDataItem>();

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

            var medications = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            medications.AddRange(unitOfWork.Repository<Medication>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.DrugName.Trim()
                })
                .OrderBy(c => c.Text)
                .ToArray());
            ViewBag.Medications = medications;

            // Prepare drop downs
            ViewBag.DoseUnits = new[]
                {
                    new SelectListItem { Value = "", Text = "" },
                    new SelectListItem { Value = "Bq", Text = "becquerel" },
                    new SelectListItem { Value = "Ci", Text = "curie" },
                    new SelectListItem { Value = "{DF}", Text = "Dosage form" },
                    new SelectListItem { Value = "[drp]", Text = "drop" },
                    new SelectListItem { Value = "GBq", Text = "gigabecquerel" },
                    new SelectListItem { Value = "g", Text = "gram" },
                    new SelectListItem { Value = "[iU]", Text = "International unit" },
                    new SelectListItem { Value = "[iU]/kg", Text = "International unit/kilogram" },
                    new SelectListItem { Value = "kBq", Text = "killobecquerel" },
                    new SelectListItem { Value = "kg", Text = "kilogram" },
                    new SelectListItem { Value = "k[iU]", Text = "kilo-international unit" },
                    new SelectListItem { Value = "L", Text = "liter" },
                    new SelectListItem { Value = "MBq", Text = "megabecquerel" },
                    new SelectListItem { Value = "M[iU]", Text = "mega-international unit" },
                    new SelectListItem { Value = "uCi", Text = "microcurie" },
                    new SelectListItem { Value = "ug", Text = "microgram" },
                    new SelectListItem { Value = "ug/kg", Text = "microgram/kilogram" },
                    new SelectListItem { Value = "uL", Text = "microliter" },
                    new SelectListItem { Value = "mCi", Text = "millicurie" },
                    new SelectListItem { Value = "meq", Text = "milliequivalent" },
                    new SelectListItem { Value = "mg", Text = "milligram" },
                    new SelectListItem { Value = "mg/kg", Text = "milligram/kilogram" },
                    new SelectListItem { Value = "mg/m2", Text = "milligram/sq.meter" },
                    new SelectListItem { Value = "ug/m2", Text = "microgram/sq.meter" },
                    new SelectListItem { Value = "mL", Text = "milliliter" },
                    new SelectListItem { Value = "mmol", Text = "millimole" },
                    new SelectListItem { Value = "mol", Text = "mole" },
                    new SelectListItem { Value = "nCi", Text = "nanocurie" },
                    new SelectListItem { Value = "ng", Text = "nanogram" },
                    new SelectListItem { Value = "%", Text = "percent" },
                    new SelectListItem { Value = "pg", Text = "picogram" }
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
        public ActionResult EditPatientMedication(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var patientMedication = unitOfWork.Repository<PatientMedication>()
                .Queryable()
                .Include(i => i.Medication)
                .SingleOrDefault(p => p.Id == id);

            if (patientMedication == null)
            {
                ViewBag.Entity = "Patient Medication";
                return View("NotFound");
            }

            var model = new PatientMedicationEditModel
            {
                PatientFullName = patientMedication.Patient.FullName,
                PatientMedicationId = patientMedication.Id,
                StartDate = patientMedication.DateStart,
                EndDate = patientMedication.DateEnd,
                Medication = patientMedication.Medication != null ? patientMedication.Medication.DrugName : "",
                MedicationForm = patientMedication.Medication != null ? patientMedication.Medication.MedicationForm != null ? patientMedication.Medication.MedicationForm.Description : "" : "",
                Dose = patientMedication.Dose,
                DoseFrequency = patientMedication.DoseFrequency,
                DoseUnit = patientMedication.DoseUnit
            };

            ViewBag.Medications = unitOfWork.Repository<Medication>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.DrugName.Trim()
                })
                .ToArray();

            var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == "PatientMedication")
                .ToList();

            var extendable = (IExtendable)patientMedication;

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

            var selectiondataRepository = unitOfWork.Repository<SelectionDataItem>();

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
            ViewBag.DoseUnits = new[]
                {
                    new SelectListItem { Value = "", Text = "" },
                    new SelectListItem { Value = "Bq", Text = "becquerel" },
                    new SelectListItem { Value = "Ci", Text = "curie" },
                    new SelectListItem { Value = "{DF}", Text = "Dosage form" },
                    new SelectListItem { Value = "[drp]", Text = "drop" },
                    new SelectListItem { Value = "GBq", Text = "gigabecquerel" },
                    new SelectListItem { Value = "g", Text = "gram" },
                    new SelectListItem { Value = "[iU]", Text = "International unit" },
                    new SelectListItem { Value = "[iU]/kg", Text = "International unit/kilogram" },
                    new SelectListItem { Value = "kBq", Text = "killobecquerel" },
                    new SelectListItem { Value = "kg", Text = "kilogram" },
                    new SelectListItem { Value = "k[iU]", Text = "kilo-international unit" },
                    new SelectListItem { Value = "L", Text = "liter" },
                    new SelectListItem { Value = "MBq", Text = "megabecquerel" },
                    new SelectListItem { Value = "M[iU]", Text = "mega-international unit" },
                    new SelectListItem { Value = "uCi", Text = "microcurie" },
                    new SelectListItem { Value = "ug", Text = "microgram" },
                    new SelectListItem { Value = "ug/kg", Text = "microgram/kilogram" },
                    new SelectListItem { Value = "uL", Text = "microliter" },
                    new SelectListItem { Value = "mCi", Text = "millicurie" },
                    new SelectListItem { Value = "meq", Text = "milliequivalent" },
                    new SelectListItem { Value = "mg", Text = "milligram" },
                    new SelectListItem { Value = "mg/kg", Text = "milligram/kilogram" },
                    new SelectListItem { Value = "mg/m2", Text = "milligram/sq.meter" },
                    new SelectListItem { Value = "ug/m2", Text = "microgram/sq.meter" },
                    new SelectListItem { Value = "mL", Text = "milliliter" },
                    new SelectListItem { Value = "mmol", Text = "millimole" },
                    new SelectListItem { Value = "mol", Text = "mole" },
                    new SelectListItem { Value = "nCi", Text = "nanocurie" },
                    new SelectListItem { Value = "ng", Text = "nanogram" },
                    new SelectListItem { Value = "%", Text = "percent" },
                    new SelectListItem { Value = "pg", Text = "picogram" }
                };

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpPost]
        public ActionResult EditPatientMedication(PatientMedicationEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var patientMedication = unitOfWork.Repository<PatientMedication>().Get(model.PatientMedicationId);

            if (patientMedication == null)
            {
                ViewBag.Entity = "PatientMedication";
                return View("NotFound");
            }

            // Date validation
            if (model.StartDate < patientMedication.Patient.DateOfBirth)
            {
                ModelState.AddModelError("StartDate", "Start Date should be after Date Of Birth");
            }
            if (model.EndDate != null)
            {
                if (model.EndDate > DateTime.Today)
                {
                    ModelState.AddModelError("EndDate", "End Date should be before current date");
                }
                if (model.EndDate < model.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End Date should be after Start Date");
                }
            }

            if (ModelState.IsValid)
            {
                // Check medication overlapping - START DATE
                if (checkStartDateAgainstStartDateWithNoEndDate(patientMedication.Medication.Id, patientMedication.Patient.Id, model.StartDate, patientMedication.Id))
                {
                    ModelState.AddModelError("StartDate", "Duplication of medication. Please check start and end dates...");
                }
                else
                {
                    if (checkStartDateWithinRange(patientMedication.Medication.Id, patientMedication.Patient.Id, model.StartDate, patientMedication.Id))
                    {
                        ModelState.AddModelError("StartDate", "Duplication of medication. Please check start and end dates...");
                    }
                    else
                    {
                        if (model.EndDate == null)
                        {
                            if (checkStartDateWithNoEndDateBeforeStart(patientMedication.Medication.Id, patientMedication.Patient.Id, model.StartDate, patientMedication.Id))
                            {
                                ModelState.AddModelError("StartDate", "Duplication of medication. Please check start and end dates...");
                            }
                        }
                    }
                }

                // Check medication overlapping - END DATE
                if (model.EndDate != null)
                {
                    if (checkEndDateAgainstStartDateWithNoEndDate(patientMedication.Medication.Id, patientMedication.Patient.Id, Convert.ToDateTime(model.EndDate), patientMedication.Id))
                    {
                        ModelState.AddModelError("EndDate", "Duplication of medication. Please check start and end dates...");
                    }
                    else
                    {
                        if (checkEndDateWithinRange(patientMedication.Medication.Id, patientMedication.Patient.Id, Convert.ToDateTime(model.EndDate), patientMedication.Id))
                        {
                            ModelState.AddModelError("EndDate", "Duplication of medication. Please check start and end dates...");
                        }
                    }
                }
                try
                {
                    patientMedication.DateStart = model.StartDate;
                    patientMedication.DateEnd = model.EndDate;
                    patientMedication.Dose = model.Dose;
                    patientMedication.DoseFrequency = model.DoseFrequency;
                    patientMedication.DoseUnit = model.DoseUnit;

                    if (model.CustomAttributes != null)
                    {
                        var patientMedicationExtended = (IExtendable)patientMedication;
                        var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(ca => ca.ExtendableTypeName == typeof(PatientMedication).Name).ToList();

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
                                            patientMedicationExtended.ValidateAndSetAttributeValue(attributeConfig, number, User.Identity.Name);
                                        }
                                        break;
                                    case "Selection":
                                        Int32 selection = 0;
                                        if (Int32.TryParse(model.CustomAttributes[i].Value, out selection))
                                        {
                                            patientMedicationExtended.ValidateAndSetAttributeValue(attributeConfig, selection, User.Identity.Name);
                                        }
                                        break;
                                    case "DateTime":
                                        DateTime parsedDate = DateTime.MinValue;
                                        if (DateTime.TryParse(model.CustomAttributes[i].Value, out parsedDate))
                                        {
                                            patientMedicationExtended.ValidateAndSetAttributeValue(attributeConfig, parsedDate, User.Identity.Name);
                                        }
                                        break;
                                    case "String":
                                    default:
                                        patientMedicationExtended.ValidateAndSetAttributeValue(attributeConfig, model.CustomAttributes[i].Value ?? string.Empty, User.Identity.Name);
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
                        unitOfWork.Repository<PatientMedication>().Update(patientMedication);
                        AddOrUpdateMedicationsToReportInstance(patientMedication);

                        unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Medication updated successfully";
                        Response.Cookies.Add(cookie);

                        //if (returnUrl != string.Empty)
                        //{
                        //    return Redirect(returnUrl);
                        //}
                        return Redirect("/Patient/PatientView.aspx?pid=" + patientMedication.Patient.Id.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Medication", string.Format("Unable to update the Patient Medication: {0}", ex.Message));
                }
            }

            // Prepare custom attributes
            var cattributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientMedication).Name)
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

            var selectiondataRepository = unitOfWork.Repository<SelectionDataItem>();

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

            ViewBag.Medications = unitOfWork.Repository<Medication>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.DrugName
                })
                .ToArray();

            // Prepare drop downs
            ViewBag.DoseUnits = new[]
                {
                    new SelectListItem { Value = "", Text = "" },
                    new SelectListItem { Value = "Bq", Text = "becquerel" },
                    new SelectListItem { Value = "Ci", Text = "curie" },
                    new SelectListItem { Value = "{DF}", Text = "Dosage form" },
                    new SelectListItem { Value = "[drp]", Text = "drop" },
                    new SelectListItem { Value = "GBq", Text = "gigabecquerel" },
                    new SelectListItem { Value = "g", Text = "gram" },
                    new SelectListItem { Value = "[iU]", Text = "International unit" },
                    new SelectListItem { Value = "[iU]/kg", Text = "International unit/kilogram" },
                    new SelectListItem { Value = "kBq", Text = "killobecquerel" },
                    new SelectListItem { Value = "kg", Text = "kilogram" },
                    new SelectListItem { Value = "k[iU]", Text = "kilo-international unit" },
                    new SelectListItem { Value = "L", Text = "liter" },
                    new SelectListItem { Value = "MBq", Text = "megabecquerel" },
                    new SelectListItem { Value = "M[iU]", Text = "mega-international unit" },
                    new SelectListItem { Value = "uCi", Text = "microcurie" },
                    new SelectListItem { Value = "ug", Text = "microgram" },
                    new SelectListItem { Value = "ug/kg", Text = "microgram/kilogram" },
                    new SelectListItem { Value = "uL", Text = "microliter" },
                    new SelectListItem { Value = "mCi", Text = "millicurie" },
                    new SelectListItem { Value = "meq", Text = "milliequivalent" },
                    new SelectListItem { Value = "mg", Text = "milligram" },
                    new SelectListItem { Value = "mg/kg", Text = "milligram/kilogram" },
                    new SelectListItem { Value = "mg/m2", Text = "milligram/sq.meter" },
                    new SelectListItem { Value = "ug/m2", Text = "microgram/sq.meter" },
                    new SelectListItem { Value = "mL", Text = "milliliter" },
                    new SelectListItem { Value = "mmol", Text = "millimole" },
                    new SelectListItem { Value = "mol", Text = "mole" },
                    new SelectListItem { Value = "nCi", Text = "nanocurie" },
                    new SelectListItem { Value = "ng", Text = "nanogram" },
                    new SelectListItem { Value = "%", Text = "percent" },
                    new SelectListItem { Value = "pg", Text = "picogram" }
                };

            TempData["returnUrl"] = returnUrl;

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeletePatientMedication(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var patientMedication = unitOfWork.Repository<PatientMedication>()
                .Queryable()
                .Include(i => i.Medication)
                .SingleOrDefault(p => p.Id == id);

            if (patientMedication == null)
            {
                ViewBag.Entity = "Patient Medication";
                return View("NotFound");
            }

            // Prepare model
            var model = new PatientMedicationDeleteModel
            {
                PatientFullName = patientMedication.Patient.FullName,
                PatientMedicationId = patientMedication.Id,
                StartDate = patientMedication.DateStart,
                EndDate = patientMedication.DateEnd,
                Medication = patientMedication.Medication != null ? patientMedication.Medication.DrugName : "",
                Dose = patientMedication.Dose,
                DoseFrequency = patientMedication.DoseFrequency,
                DoseUnit = patientMedication.DoseUnit
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeletePatientMedication(PatientMedicationDeleteModel model)
        {
            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var patientMedicationRepository = unitOfWork.Repository<PatientMedication>();
            var patientMedication = patientMedicationRepository.Queryable()
                                                            .Include(x => x.Patient)
                                                            .FirstOrDefault(x => x.Id == model.PatientMedicationId);

            if (patientMedication != null)
            {
                var user = GetCurrentUser();

                if (user != null)
                {
                    if (ModelState.IsValid)
                    {
                        var reason = model.ArchiveReason ?? "** NO REASON SPECIFIED ** ";
                        patientMedication.Archived = true;
                        patientMedication.ArchivedDate = DateTime.Now;
                        patientMedication.ArchivedReason = reason;
                        patientMedication.AuditUser = user;
                        patientMedicationRepository.Update(patientMedication);
                        DeleteMedicationsFromReportInstance(patientMedication);
                        unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Medication deleted successfully";
                        Response.Cookies.Add(cookie);

                        //return Redirect(returnUrl);
                        return Redirect("/Patient/PatientView.aspx?pid=" + patientMedication.Patient.Id.ToString());
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

        private Boolean checkStartDateAgainstStartDateWithNoEndDate(int medication_id, int patient_id, DateTime startDate, int patientMedicationId)
        {
            List<PatientMedication> patientMedications;

            if(patientMedicationId > 0)
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientMedicationId && pce.Medication.Id == medication_id && pce.DateStart <= startDate && pce.DateEnd == null && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Medication.Id == medication_id && pce.DateStart <= startDate && pce.DateEnd == null && pce.Archived == false)
                        .ToList();
            }

            if (patientMedications.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private Boolean checkStartDateWithinRange(int medication_id, int patient_id, DateTime startDate, int patientMedicationId)
        {
            List<PatientMedication> patientMedications;

            if (patientMedicationId > 0)
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientMedicationId && pce.Medication.Id == medication_id && startDate >= pce.DateStart && startDate <= pce.DateEnd && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Medication.Id == medication_id && startDate >= pce.DateStart && startDate <= pce.DateEnd && pce.Archived == false)
                        .ToList();
            }

            if (patientMedications.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private Boolean checkStartDateWithNoEndDateBeforeStart(int medication_id, int patient_id, DateTime startDate, int patientMedicationId)
        {
            List<PatientMedication> patientMedications;

            if (patientMedicationId > 0)
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientMedicationId && pce.Medication.Id == medication_id && startDate < pce.DateStart && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Medication.Id == medication_id && startDate < pce.DateStart && pce.Archived == false)
                        .ToList();
            }

            if (patientMedications.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private Boolean checkEndDateAgainstStartDateWithNoEndDate(int medication_id, int patient_id, DateTime endDate, int patientMedicationId)
        {
            List<PatientMedication> patientMedications;

            if (patientMedicationId > 0)
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientMedicationId && pce.Medication.Id == medication_id && pce.DateStart <= endDate && pce.DateEnd == null && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Medication.Id == medication_id && pce.DateStart <= endDate && pce.DateEnd == null && pce.Archived == false)
                        .ToList();
            }

            if (patientMedications.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private Boolean checkEndDateWithinRange(int medication_id, int patient_id, DateTime endDate, int patientMedicationId)
        {
            List<PatientMedication> patientMedications;

            if (patientMedicationId > 0)
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientMedicationId && pce.Medication.Id == medication_id && endDate >= pce.DateStart && endDate <= pce.DateEnd && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientMedications = unitOfWork.Repository<PatientMedication>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Medication.Id == medication_id && endDate >= pce.DateStart && endDate <= pce.DateEnd && pce.Archived == false)
                        .ToList();
            }

            if (patientMedications.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        private User GetCurrentUser()
        {
            return unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
        }

        private void AddOrUpdateMedicationsToReportInstance(PatientMedication patientMedication)
        {
            var weeks = 0;
            var config = unitOfWork.Repository<Config>().Queryable().Where(c => c.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks).SingleOrDefault();
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    weeks = Convert.ToInt32(config.ConfigValue);
                }
            }

            // Manage modifications to report instance - if one exists
            IEnumerable<PatientClinicalEvent> events;
            if (patientMedication.DateEnd == null)
            {
                events = patientMedication.Patient.PatientClinicalEvents.Where(pce => pce.OnsetDate >= patientMedication.DateStart.AddDays(weeks * -7) && pce.Archived == false);
            }
            else
            {
                events = patientMedication.Patient.PatientClinicalEvents.Where(pce => pce.OnsetDate >= patientMedication.DateStart.AddDays(weeks * -7) && pce.OnsetDate <= Convert.ToDateTime(patientMedication.DateEnd).AddDays(weeks * 7) && pce.Archived == false);
            }

            // Prepare medications
            List<ReportInstanceMedicationListItem> instanceMedications = new List<ReportInstanceMedicationListItem>();
            var item = new ReportInstanceMedicationListItem()
            {
                MedicationIdentifier = patientMedication.DisplayName,
                ReportInstanceMedicationGuid = patientMedication.PatientMedicationGuid
            };
            instanceMedications.Add(item);

            foreach (var evt in events)
            {
                _workflowService.AddOrUpdateMedicationsForWorkFlowInstance(evt.PatientClinicalEventGuid, instanceMedications);
            }
        }

        private void DeleteMedicationsFromReportInstance(PatientMedication patientMedication)
        {
            // Manage modifications to report instance - if one exists
            IEnumerable<PatientClinicalEvent> events;
            if (patientMedication.DateEnd == null)
            {
                events = patientMedication.Patient.PatientClinicalEvents.Where(pce => pce.OnsetDate >= patientMedication.DateStart && pce.Archived == false);
            }
            else
            {
                events = patientMedication.Patient.PatientClinicalEvents.Where(pce => pce.OnsetDate >= patientMedication.DateStart && pce.OnsetDate <= patientMedication.DateEnd && pce.Archived == false);
            }

            // Prepare medications
            List<ReportInstanceMedicationListItem> instanceMedications = new List<ReportInstanceMedicationListItem>();
            var item = new ReportInstanceMedicationListItem()
            {
                MedicationIdentifier = patientMedication.DisplayName,
                ReportInstanceMedicationGuid = patientMedication.PatientMedicationGuid
            };
            instanceMedications.Add(item);

            foreach (var evt in events)
            {
                _workflowService.DeleteMedicationsFromWorkFlowInstance(evt.PatientClinicalEventGuid, instanceMedications);
            }
        }
    }
}