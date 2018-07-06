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
using PVIMS.Web.Models;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using FrameworkCustomAttributeConfiguration = VPS.CustomAttributes.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using PVIMS.Web.ActionFilters;

namespace PVIMS.Web.Controllers
{
    public class PatientConditionController : BaseController
    {
        private static string CurrentMenuItem = "EncounterView";

        private readonly IUnitOfWorkInt unitOfWork;

        public PatientConditionController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // offline view
        [HttpGet]
        public ActionResult PatientConditionView()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult AddPatientCondition(int id)
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

            var model = new PatientConditionAddModel { PatientId = id, PatientFullName = patient.FullName };

            ViewBag.Conditions = unitOfWork.Repository<Condition>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .OrderBy(c => c.Text)
                .ToArray();

            var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == "PatientCondition")
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

            var outcomes = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            outcomes.AddRange(unitOfWork.Repository<Outcome>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.Outcomes = outcomes;

            var treatmentOutcomes = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            treatmentOutcomes.AddRange(unitOfWork.Repository<TreatmentOutcome>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.TreatmentOutcomes = treatmentOutcomes;

            return View(model);
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult AddPatientCondition(PatientConditionAddModel model, string button)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (button == null) { button = "Submit"; };

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            // Prepare entities
            var patient = unitOfWork.Repository<Patient>().Get(model.PatientId);

            if (patient == null)
            {
                ViewBag.Entity = "Patient";
                return View("NotFound");
            }

            var outcome = unitOfWork.Repository<Outcome>().Get(model.OutcomeId);
            var treatmentOutcome = unitOfWork.Repository<TreatmentOutcome>().Get(model.TreatmentOutcomeId);

            // Date validation
            TerminologyMedDra sourceTerm = null;

            if (button != "Search")
            {
                if (button == "Submit")
                {
                    if (model.TermResult != null)
                    {
                        sourceTerm = unitOfWork.Repository<TerminologyMedDra>().Get(Convert.ToInt32(model.TermResult));
                    }
                    if (sourceTerm == null)
                    {
                        ModelState.AddModelError("CustomError", "Please ensure a source term has been selected...");
                    }
                }

                if (model.StartDate != null)
                {
                    if (model.StartDate > DateTime.Today)
                    {
                        ModelState.AddModelError("StartDate", "Condition Start Date should be before current date");
                    }
                    if (model.StartDate < patient.DateOfBirth)
                    {
                        ModelState.AddModelError("StartDate", "Condition Start Date should be after Date Of Birth");
                    }
                }
                else
                {
                    ModelState.AddModelError("StartDate", "Condition Start Date is mandatory");
                }
                if (model.EndDate != null)
                {
                    if (model.EndDate > DateTime.Today)
                    {
                        ModelState.AddModelError("EndDate", "Condition Outcome Date should be before current date");
                    }
                    if (model.StartDate != null)
                    {
                        if (model.EndDate < model.StartDate)
                        {
                            ModelState.AddModelError("EndDate", "Condition Outcome Date should be after Start Date");
                        }
                    }
                }
                else
                {
                    if(outcome != null)
                    {
                        ModelState.AddModelError("EndDate", "Condition Outcome Date is mandatory if Condition Outcome is set");
                    }
                }
                if (outcome != null || treatmentOutcome != null)
                {
                    var outcomeVal = outcome != null ? outcome.Description : "";
                    var treatmentOutcomeVal = treatmentOutcome != null ? treatmentOutcome.Description : "";

                    if (outcomeVal == "Fatal" && treatmentOutcomeVal != "Died")
                    {
                        ModelState.AddModelError("TreatmentOutcomeId", "Treatment Outcome not consistent with Condition Outcome");
                    }
                    if (outcomeVal != "Fatal" && treatmentOutcomeVal == "Died")
                    {
                        ModelState.AddModelError("OutcomeId", "Condition Outcome not consistent with Treatment Outcome");
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
                        ViewBag.TermResults = unitOfWork.Repository<TerminologyMedDra>()
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
                        // validate source term, check condition overlapping - START DATE
                        if (model.StartDate != null)
                        {
                            if (checkStartDateAgainstStartDateWithNoEndDate(sourceTerm.Id, model.PatientId, Convert.ToDateTime(model.StartDate), 0))
                            {
                                ModelState.AddModelError("StartDate", "Duplication of condition. Please check condition start and outcome dates...");
                            }
                            else
                            {
                                if (checkStartDateWithinRange(sourceTerm.Id, model.PatientId, Convert.ToDateTime(model.StartDate), 0))
                                {
                                    ModelState.AddModelError("StartDate", "Duplication of condition. Please check condition start and outcome dates...");
                                }
                                else
                                {
                                    if (model.EndDate == null)
                                    {
                                        if (checkStartDateWithNoEndDateBeforeStart(sourceTerm.Id, model.PatientId, Convert.ToDateTime(model.StartDate), 0))
                                        {
                                            ModelState.AddModelError("StartDate", "Duplication of condition. Please check condition start and outcome dates...");
                                        }
                                    }
                                }
                            }
                        }

                        // Check condition overlapping - END DATE
                        if (model.EndDate != null)
                        {
                            if (checkEndDateAgainstStartDateWithNoEndDate(sourceTerm.Id, model.PatientId, Convert.ToDateTime(model.EndDate), 0))
                            {
                                ModelState.AddModelError("EndDate", "Duplication of condition. Please check condition start and outcome dates...");
                            }
                            else
                            {
                                if (checkEndDateWithinRange(sourceTerm.Id, model.PatientId, Convert.ToDateTime(model.EndDate), 0))
                                {
                                    ModelState.AddModelError("EndDate", "Duplication of condition. Please check condition start and outcome dates...");
                                }
                            }
                        }

                        //finally save
                        try
                        {
                            var patientCondition = new PatientCondition
                            {
                                Patient = patient,
                                TerminologyMedDra = sourceTerm,
                                Comments = model.Comments,
                                DateStart = Convert.ToDateTime(model.StartDate),
                                OutcomeDate = model.EndDate,
                                Outcome = outcome,
                                TreatmentOutcome = treatmentOutcome
                            };

                            if (model.CustomAttributes != null)
                            {
                                var patientConditionExtended = (IExtendable)patientCondition;
                                var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(ca => ca.ExtendableTypeName == typeof(PatientCondition).Name).ToList();

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
                                                    patientConditionExtended.ValidateAndSetAttributeValue(attributeConfig, number, User.Identity.Name);
                                                }
                                                break;
                                            case "Selection":
                                                Int32 selection = 0;
                                                if (Int32.TryParse(model.CustomAttributes[i].Value, out selection))
                                                {
                                                    patientConditionExtended.ValidateAndSetAttributeValue(attributeConfig, selection, User.Identity.Name);
                                                }
                                                break;
                                            case "DateTime":
                                                DateTime parsedDate = DateTime.MinValue;
                                                if (DateTime.TryParse(model.CustomAttributes[i].Value, out parsedDate))
                                                {
                                                    patientConditionExtended.ValidateAndSetAttributeValue(attributeConfig, parsedDate, User.Identity.Name);
                                                }
                                                break;
                                            case "String":
                                            default:
                                                patientConditionExtended.ValidateAndSetAttributeValue(attributeConfig, model.CustomAttributes[i].Value ?? string.Empty, User.Identity.Name);
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
                                unitOfWork.Repository<PatientCondition>().Save(patientCondition);
                                unitOfWork.Complete();

                                // If successfully updated check if  and person has died
                                if(outcome != null)
                                {
                                    if (outcome.Description == "Fatal")
                                    {
                                        var currentStatus = patient.GetCurrentStatus();
                                        if (currentStatus.PatientStatus.Description != "Died")
                                        {
                                            // set patient status to deceased in patient history
                                            var user = GetCurrentUser();

                                            PatientStatusHistory status = new PatientStatusHistory()
                                            {
                                                Patient = patientCondition.Patient,
                                                EffectiveDate = patientCondition.OutcomeDate ?? DateTime.Now,   //set effective date to  outcome date have set it to  use todays day if null but this will not happen as  autosetToDeceased will only become true when an end date is supplied first
                                                Comments = String.Format("Marked as Died through Patient Condition ({0}) by {1}", patientCondition.TerminologyMedDra.DisplayName, user.FullName),
                                                PatientStatus = unitOfWork.Repository<PatientStatus>().Queryable().Single(ps => ps.Description == "Died")
                                            };

                                            unitOfWork.Repository<PatientStatusHistory>().Save(status);
                                        }
                                    }
                                }
                                HttpCookie cookie = new HttpCookie("PopUpMessage");
                                cookie.Value = "Condition added successfully";
                                Response.Cookies.Add(cookie);

                                return Redirect("/Patient/PatientView.aspx?pid=" + patientCondition.Patient.Id.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", string.Format("Unable to add the Patient Condition: {0}", ex.Message));
                        }

                        // Prepare blank results
                        if (!String.IsNullOrWhiteSpace(model.FindTerm))
                        {
                            ViewBag.TermResults = unitOfWork.Repository<TerminologyMedDra>()
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
                    ViewBag.TermResults = unitOfWork.Repository<TerminologyMedDra>()
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
            var cattributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientCondition).Name)
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

            // Prepare drop downs
            ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                };

            var outcomes = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            outcomes.AddRange(unitOfWork.Repository<Outcome>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.Outcomes = outcomes;

            var treatmentOutcomes = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            treatmentOutcomes.AddRange(unitOfWork.Repository<TreatmentOutcome>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.TreatmentOutcomes = treatmentOutcomes;

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
        public ActionResult EditPatientCondition(int id)
        {

            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var patientCondition = unitOfWork.Repository<PatientCondition>()
                .Queryable()
                .Include(i => i.TerminologyMedDra)
                .SingleOrDefault(p => p.Id == id);

            if (patientCondition == null)
            {
                ViewBag.Entity = "Patient Condition";
                return View("NotFound");
            }

            var model = new PatientConditionEditModel
            {
                PatientFullName = patientCondition.Patient.FullName,
                PatientConditionId = patientCondition.Id,
                TerminologyMedDRA = patientCondition.TerminologyMedDra.DisplayName,
                StartDate = patientCondition.DateStart,
                EndDate = patientCondition.OutcomeDate,
                Comments = patientCondition.Comments,
                MeddraSearch = "none",
                OutcomeId = patientCondition.Outcome != null ? patientCondition.Outcome.Id : 0,
                TreatmentOutcomeId = patientCondition.TreatmentOutcome != null ? patientCondition.TreatmentOutcome.Id : 0,
                PreviousOutcomeId = patientCondition.Outcome != null ? patientCondition.Outcome.Id : 0
            };

            ViewBag.Conditions = unitOfWork.Repository<Condition>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray();

            var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == "PatientCondition")
                .ToList();

            var extendable = (IExtendable)patientCondition;

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

            var outcomes = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            outcomes.AddRange(unitOfWork.Repository<Outcome>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.Outcomes = outcomes;

            var treatmentOutcomes = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            treatmentOutcomes.AddRange(unitOfWork.Repository<TreatmentOutcome>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.TreatmentOutcomes = treatmentOutcomes;

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpPost]
        public ActionResult EditPatientCondition(PatientConditionEditModel model, string button)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (button == null) { button = "Submit"; };

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            model.MeddraSearch = "none";

            // Prepare entities
            var patientCondition = unitOfWork.Repository<PatientCondition>().Queryable().Include(i1 => i1.Patient).Single(i2 => i2.Id == model.PatientConditionId);

            if (patientCondition == null)
            {
                ViewBag.Entity = "PatientCondition";
                return View("NotFound");
            }

            var outcome = unitOfWork.Repository<Outcome>().Get(model.OutcomeId);
            var treatmentOutcome = unitOfWork.Repository<TreatmentOutcome>().Get(model.TreatmentOutcomeId);

            // Date validation
            TerminologyMedDra sourceTerm = null;

            if (button != "Search")
            {
                if (button == "Submit")
                {
                    if (model.TermResult != null)
                    {
                        if (Convert.ToInt32(model.TermResult) != default(int))
                        {
                            sourceTerm = unitOfWork.Repository<TerminologyMedDra>().Get(Convert.ToInt32(model.TermResult));
                        }
                    }
                    else
                    {
                        sourceTerm = patientCondition.TerminologyMedDra;
                    }
                }

                if (model.StartDate != null)
                {
                    if (model.StartDate > DateTime.Today)
                    {
                        ModelState.AddModelError("StartDate", "Start Date should be before current date");
                    }
                    if (model.StartDate < patientCondition.Patient.DateOfBirth)
                    {
                        ModelState.AddModelError("StartDate", "Condition Start Date should be after Date Of Birth");
                    }
                }
                else
                {
                    ModelState.AddModelError("StartDate", "Condition Start Date is mandatory");
                }

                if (model.EndDate != null)
                {
                    if (model.EndDate > DateTime.Today)
                    {
                        ModelState.AddModelError("EndDate", "Condition Outcome Date should be before current date");
                    }
                    if (model.StartDate != null)
                    {
                        if (model.EndDate < model.StartDate)
                        {
                            ModelState.AddModelError("EndDate", "Condition Outcome Date should be after Start Date");
                        }
                    }
                }
                else
                {
                    if (outcome != null)
                    {
                        ModelState.AddModelError("EndDate", "Condition Outcome Date is mandatory if Outcome is set");
                    }
                }
                if (outcome != null || treatmentOutcome != null)
                {
                    var outcomeVal = outcome != null ? outcome.Description : "";
                    var treatmentOutcomeVal = treatmentOutcome != null ? treatmentOutcome.Description : "";

                    if (outcomeVal == "Fatal" && treatmentOutcomeVal != "Died")
                    {
                        ModelState.AddModelError("TreatmentOutcomeId", "Treatment Outcome not consistent with Condition Outcome");
                    }
                    if (outcomeVal != "Fatal" && treatmentOutcomeVal == "Died")
                    {
                        ModelState.AddModelError("OutcomeId", "Condition Outcome not consistent with Treatment Outcome");
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
                        ViewBag.TermResults = unitOfWork.Repository<TerminologyMedDra>()
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
                        if (sourceTerm != null)
                        {
                            if (model.StartDate != null)
                            {
                                // Check condition overlapping - START DATE
                                if (checkStartDateAgainstStartDateWithNoEndDate(sourceTerm.Id, patientCondition.Patient.Id, Convert.ToDateTime(model.StartDate), patientCondition.Id))
                                {
                                    ModelState.AddModelError("StartDate", "Duplication of condition. Please check condition start and outcome dates...");
                                }
                                else
                                {
                                    if (checkStartDateWithinRange(sourceTerm.Id, patientCondition.Patient.Id, Convert.ToDateTime(model.StartDate), patientCondition.Id))
                                    {
                                        ModelState.AddModelError("StartDate", "Duplication of condition. Please check condition start and outcome dates...");
                                    }
                                    else
                                    {
                                        if (model.EndDate == null)
                                        {
                                            if (checkStartDateWithNoEndDateBeforeStart(sourceTerm.Id, patientCondition.Patient.Id, Convert.ToDateTime(model.StartDate), patientCondition.Id))
                                            {
                                                ModelState.AddModelError("StartDate", "Duplication of condition. Please check condition start and outcome dates...");
                                            }
                                        }
                                    }
                                }
                            }

                            // Check condition overlapping - END DATE
                            if (model.EndDate != null)
                            {
                                if (checkEndDateAgainstStartDateWithNoEndDate(sourceTerm.Id, patientCondition.Patient.Id, Convert.ToDateTime(model.EndDate), patientCondition.Id))
                                {
                                    ModelState.AddModelError("EndDate", "Duplication of condition. Please check condition start and outcome dates...");
                                }
                                else
                                {
                                    if (checkEndDateWithinRange(sourceTerm.Id, patientCondition.Patient.Id, Convert.ToDateTime(model.EndDate), patientCondition.Id))
                                    {
                                        ModelState.AddModelError("EndDate", "Duplication of condition. Please check condition start and outcome dates...");
                                    }
                                }
                            }
                        }

                        try
                        {
                            if (sourceTerm != null) { patientCondition.TerminologyMedDra = sourceTerm; };

                            patientCondition.TreatmentOutcome = treatmentOutcome;
                            patientCondition.Outcome = outcome;
                            patientCondition.Comments = model.Comments;
                            patientCondition.DateStart = model.StartDate;
                            patientCondition.OutcomeDate = model.EndDate;

                            if (model.CustomAttributes != null)
                            {
                                var patientConditionExtended = (IExtendable)patientCondition;
                                var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(ca => ca.ExtendableTypeName == typeof(PatientCondition).Name).ToList();

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
                                                    patientConditionExtended.ValidateAndSetAttributeValue(attributeConfig, number, User.Identity.Name);
                                                }
                                                break;
                                            case "Selection":
                                                Int32 selection = 0;
                                                if (Int32.TryParse(model.CustomAttributes[i].Value, out selection))
                                                {
                                                    patientConditionExtended.ValidateAndSetAttributeValue(attributeConfig, selection, User.Identity.Name);
                                                }
                                                break;
                                            case "DateTime":
                                                DateTime parsedDate = DateTime.MinValue;
                                                if (DateTime.TryParse(model.CustomAttributes[i].Value, out parsedDate))
                                                {
                                                    patientConditionExtended.ValidateAndSetAttributeValue(attributeConfig, parsedDate, User.Identity.Name);
                                                }
                                                break;
                                            case "String":
                                            default:
                                                patientConditionExtended.ValidateAndSetAttributeValue(attributeConfig, model.CustomAttributes[i].Value ?? string.Empty, User.Identity.Name);
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
                                unitOfWork.Repository<PatientCondition>().Update(patientCondition);
                                unitOfWork.Complete();

                                // If successfully updated check if  and person has died
                                if(outcome != null)
                                {
                                    if (outcome.Description == "Fatal")
                                    {
                                        var currentStatus = patientCondition.Patient.GetCurrentStatus();
                                        if (currentStatus.PatientStatus.Description != "Died")
                                        {
                                            // set patient status to deceased in patient history
                                            var user = GetCurrentUser();

                                            PatientStatusHistory status = new PatientStatusHistory()
                                            {
                                                Patient = patientCondition.Patient,
                                                EffectiveDate = patientCondition.OutcomeDate ?? DateTime.Now,   //set effective date to  outcome date have set it to  use todays day if null but this will not happen as  autosetToDeceased will only become true when an end date is supplied first
                                                Comments = String.Format("Marked as Died through Patient Condition ({0}) by {1}", patientCondition.TerminologyMedDra.DisplayName, user.FullName),
                                                PatientStatus = unitOfWork.Repository<PatientStatus>().Queryable().Single(ps => ps.Description == "Died")
                                            };

                                            unitOfWork.Repository<PatientStatusHistory>().Save(status);
                                        }
                                    }
                                }

                                HttpCookie cookie = new HttpCookie("PopUpMessage");
                                cookie.Value = "Condition updated successfully";
                                Response.Cookies.Add(cookie);

                                //if (returnUrl != string.Empty)
                                //{
                                //    return Redirect(returnUrl);
                                //}
                                return Redirect("/Patient/PatientView.aspx?pid=" + patientCondition.Patient.Id.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("CustomError", string.Format("Unable to update the Patient Condition: {0}", ex.Message));
                        }
                        //catch (DbEntityValidationException e)
                        //{
                        //    foreach (var eve in e.EntityValidationErrors)
                        //    {
                        //        //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        //        //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        //        var a = eve.Entry.Entity.GetType().Name;
                        //        foreach (var ve in eve.ValidationErrors)
                        //        {
                        //            var b = ve.PropertyName;
                        //            var c = ve.ErrorMessage;
                        //        }

                        //    }
                        //    throw;
                        //}

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
                    ViewBag.TermResults = unitOfWork.Repository<TerminologyMedDra>()
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
            var cattributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientCondition).Name)
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

            // Prepare drop downs
            ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                };

            var outcomes = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            outcomes.AddRange(unitOfWork.Repository<Outcome>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.Outcomes = outcomes;

            var treatmentOutcomes = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            treatmentOutcomes.AddRange(unitOfWork.Repository<TreatmentOutcome>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.TreatmentOutcomes = treatmentOutcomes;

            TempData["returnUrl"] = returnUrl;

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeletePatientCondition(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var patientCondition = unitOfWork.Repository<PatientCondition>()
                .Queryable()
                .Include(i => i.TerminologyMedDra)
                .SingleOrDefault(p => p.Id == id);

            if (patientCondition == null)
            {
                ViewBag.Entity = "Patient Condition";
                return View("NotFound");
            }

            // Prepare model
            var model = new PatientConditionDeleteModel
            {
                PatientFullName = patientCondition.Patient.FullName,
                PatientConditionId = patientCondition.Id,
                TerminologyMedDRA = patientCondition.TerminologyMedDra.DisplayName,
                StartDate = patientCondition.DateStart,
                EndDate = patientCondition.OutcomeDate,
                ConditionOutcome = patientCondition.Outcome != null ? patientCondition.Outcome.Description : "",
                TreatmentOutcome = patientCondition.TreatmentOutcome != null ? patientCondition.TreatmentOutcome.Description : ""
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeletePatientCondition(PatientConditionDeleteModel model)
        {
            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var patientConditionRepository = unitOfWork.Repository<PatientCondition>();
            var patientCondition = patientConditionRepository.Queryable()
                                                            .Include(x => x.Patient)
                                                            .FirstOrDefault(x => x.Id == model.PatientConditionId);

            if (patientCondition != null)
            {
                var user = GetCurrentUser();

                if (user != null)
                {
                    if (ModelState.IsValid)
                    {
                        var reason = model.ArchiveReason ?? "** NO REASON SPECIFIED ** ";
                        patientCondition.Archived = true;
                        patientCondition.ArchivedDate = DateTime.Now;
                        patientCondition.ArchivedReason = reason;
                        patientCondition.AuditUser = user;
                        patientConditionRepository.Update(patientCondition);
                        unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Condition deleted successfully";
                        Response.Cookies.Add(cookie);

                        //return Redirect(returnUrl);
                        return Redirect("/Patient/PatientView.aspx?pid=" + patientCondition.Patient.Id.ToString());
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

        private Boolean checkStartDateAgainstStartDateWithNoEndDate(int condition_id, int patient_id, DateTime startDate, int patientConditionId)
        {
            List<PatientCondition> patientConditions;

            if (patientConditionId > 0)
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientConditionId && pce.TerminologyMedDra.Id == condition_id && pce.DateStart <= startDate && pce.OutcomeDate == null && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.TerminologyMedDra.Id == condition_id && pce.DateStart <= startDate && pce.OutcomeDate == null && pce.Archived == false)
                        .ToList();
            }

            if (patientConditions.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean checkStartDateWithinRange(int condition_id, int patient_id, DateTime startDate, int patientConditionId)
        {
            List<PatientCondition> patientConditions;

            if (patientConditionId > 0)
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientConditionId && pce.TerminologyMedDra.Id == condition_id && startDate >= pce.DateStart && startDate <= pce.OutcomeDate && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.TerminologyMedDra.Id == condition_id && startDate >= pce.DateStart && startDate <= pce.OutcomeDate && pce.Archived == false)
                        .ToList();
            }

            if (patientConditions.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean checkStartDateWithNoEndDateBeforeStart(int condition_id, int patient_id, DateTime startDate, int patientConditionId)
        {
            List<PatientCondition> patientConditions;

            if (patientConditionId > 0)
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientConditionId && pce.TerminologyMedDra.Id == condition_id && startDate < pce.DateStart && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.TerminologyMedDra.Id == condition_id && startDate < pce.DateStart && pce.Archived == false)
                        .ToList();
            }

            if (patientConditions.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean checkEndDateAgainstStartDateWithNoEndDate(int condition_id, int patient_id, DateTime endDate, int patientConditionId)
        {
            List<PatientCondition> patientConditions;

            if (patientConditionId > 0)
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientConditionId && pce.TerminologyMedDra.Id == condition_id && pce.DateStart <= endDate && pce.OutcomeDate == null && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.TerminologyMedDra.Id == condition_id && pce.DateStart <= endDate && pce.OutcomeDate == null && pce.Archived == false)
                        .ToList();
            }

            if (patientConditions.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean checkEndDateWithinRange(int condition_id, int patient_id, DateTime endDate, int patientConditionId)
        {
            List<PatientCondition> patientConditions;

            if (patientConditionId > 0)
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.Id != patientConditionId && pce.TerminologyMedDra.Id == condition_id && endDate >= pce.DateStart && endDate <= pce.OutcomeDate && pce.Archived == false)
                        .ToList();
            }
            else
            {
                patientConditions = unitOfWork.Repository<PatientCondition>()
                        .Queryable()
                        .OrderBy(pce => pce.DateStart)
                        .Where(pce => pce.Patient.Id == patient_id && pce.TerminologyMedDra.Id == condition_id && endDate >= pce.DateStart && endDate <= pce.OutcomeDate && pce.Archived == false)
                        .ToList();
            }

            if (patientConditions.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private User GetCurrentUser()
        {
            return unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
        }
    }
}