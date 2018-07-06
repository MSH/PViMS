using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using VPS.Common.Repositories;
using VPS.CustomAttributes;

using FrameworkCustomAttributeConfiguration = VPS.CustomAttributes.CustomAttributeConfiguration;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using PVIMS.Web.ActionFilters;

namespace PVIMS.Web.Controllers
{
    public class PatientLabTestController : BaseController
    {
        private static string CurrentMenuItem = "EncounterView";

        private readonly IUnitOfWorkInt unitOfWork;

        public PatientLabTestController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult PatientLabTestOffline()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult AddPatientLabTest(int id)
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

            var model = new PatientLabTestAddModel { PatientId = id, PatientFullName = patient.FullName, TestDate = DateTime.Today };

            var labTests = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labTests.AddRange(unitOfWork.Repository<LabTest>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .OrderBy(c => c.Text)
                .ToArray());
            ViewBag.LabTests = labTests;

            var labTestUnits = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labTestUnits.AddRange(unitOfWork.Repository<LabTestUnit>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.LabTestUnits = labTestUnits;

            var labResults = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labResults.AddRange(unitOfWork.Repository<LabResult>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Description,
                    Text = c.Description
                })
                .ToArray());
            ViewBag.TestResults = labResults;

            var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == "PatientLabTest")
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

            return View(model);
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult AddPatientLabTest(PatientLabTestAddModel model)
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

            if (model.TestDate < patient.DateOfBirth)
            {
                ModelState.AddModelError("TestDate", "Test Date should be after Date Of Birth");
            }

            if (ModelState.IsValid)
            {
                var labTest = unitOfWork.Repository<LabTest>().Get(model.LabTestId);

                if (labTest == null)
                {
                    ViewBag.Entity = "Lab Test";
                    return View("NotFound");
                }

                try
                {
                    var labTestUnit = unitOfWork.Repository<LabTestUnit>().Queryable().SingleOrDefault(lu => lu.Id == model.LabTestUnitId);
                    var patientLabTest = new PatientLabTest
                    {
                        Patient = patient,
                        LabTest = labTest,
                        TestDate = model.TestDate,
                        TestResult = model.TestResult,
                        LabValue = model.LabValue,
                        TestUnit = labTestUnit,
                        ReferenceLower = model.ReferenceLower,
                        ReferenceUpper = model.ReferenceUpper
                    };

                    if (model.CustomAttributes != null)
                    {
                        var patientLabTestExtended = (IExtendable)patientLabTest;
                        var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(ca => ca.ExtendableTypeName == typeof(PatientLabTest).Name).ToList();

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
                                            patientLabTestExtended.ValidateAndSetAttributeValue(attributeConfig, number, User.Identity.Name);
                                        }
                                        break;
                                    case "Selection":
                                        Int32 selection = 0;
                                        if (Int32.TryParse(model.CustomAttributes[i].Value, out selection))
                                        {
                                            patientLabTestExtended.ValidateAndSetAttributeValue(attributeConfig, selection, User.Identity.Name);
                                        }
                                        break;
                                    case "DateTime":
                                        DateTime parsedDate = DateTime.MinValue;
                                        if (DateTime.TryParse(model.CustomAttributes[i].Value, out parsedDate))
                                        {
                                            patientLabTestExtended.ValidateAndSetAttributeValue(attributeConfig, parsedDate, User.Identity.Name);
                                        }
                                        break;
                                    case "String":
                                    default:
                                        patientLabTestExtended.ValidateAndSetAttributeValue(attributeConfig, model.CustomAttributes[i].Value ?? string.Empty, User.Identity.Name);
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
                        unitOfWork.Repository<PatientLabTest>().Save(patientLabTest);
                        unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Test and procedure added successfully";
                        Response.Cookies.Add(cookie);

                        return Redirect("/Patient/PatientView.aspx?pid=" + patientLabTest.Patient.Id.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to add the Patient Lab Test: {0}", ex.Message));
                }
            }

            // Prepare custom attributes
            var cattributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientLabTest).Name)
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

            if (model.CustomAttributes != null)
            {
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
            }

            var labTests = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labTests.AddRange(unitOfWork.Repository<LabTest>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .OrderBy(c => c.Text)
                .ToArray());
            ViewBag.LabTests = labTests;

            var labTestUnits = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labTestUnits.AddRange(unitOfWork.Repository<LabTestUnit>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.LabTestUnits = labTestUnits;

            var labResults = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labResults.AddRange(unitOfWork.Repository<LabResult>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Description,
                    Text = c.Description
                })
                .ToArray());
            ViewBag.TestResults = labResults;

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
        public ActionResult EditPatientLabTest(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var patientLabTest = unitOfWork.Repository<PatientLabTest>()
                .Queryable()
                .Include(i => i.Patient)
                .Include(i => i.LabTest)
                .Include(i => i.TestUnit)
                .SingleOrDefault(p => p.Id == id);

            if (patientLabTest == null)
            {
                ViewBag.Entity = "Patient Lab Test";
                return View("NotFound");
            }

            var model = new PatientLabTestEditModel
            {
                PatientFullName = patientLabTest.Patient.FullName,
                PatientLabTestId = patientLabTest.Id,
                LabTest = patientLabTest.LabTest.Description,
                TestDate = patientLabTest.TestDate,
                TestResult = patientLabTest.TestResult,
                LabValue = patientLabTest.LabValue,
                LabTestUnitId = patientLabTest.TestUnit != null ?  patientLabTest.TestUnit.Id : 0,
                ReferenceLower = patientLabTest.ReferenceLower,
                ReferenceUpper = patientLabTest.ReferenceUpper
            };

            var labTestUnits = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labTestUnits.AddRange(unitOfWork.Repository<LabTestUnit>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.LabTestUnits = labTestUnits;

            var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == "PatientLabTest" )
                .ToList();

            var extendable = (IExtendable)patientLabTest;

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

            var labResults = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labResults.AddRange(unitOfWork.Repository<LabResult>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Description,
                    Text = c.Description
                })
                .ToArray());
            ViewBag.TestResults = labResults;

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpPost]
        public ActionResult EditPatientLabTest(PatientLabTestEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var patientLabTest = unitOfWork.Repository<PatientLabTest>().Queryable().Include(plt => plt.Patient).Include(plt => plt.LabTest).Include(plt => plt.TestUnit).Single(plt => plt.Id == model.PatientLabTestId);

            if (patientLabTest == null)
            {
                ViewBag.Entity = "PatientLabTest";
                return View("NotFound");
            }

            if (model.TestDate < patientLabTest.Patient.DateOfBirth)
            {
                ModelState.AddModelError("TestDate", "Test Date should be after Date Of Birth");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var labTestUnit = unitOfWork.Repository<LabTestUnit>().Queryable().SingleOrDefault(lu => lu.Id == model.LabTestUnitId);

                    patientLabTest.TestDate = model.TestDate;
                    patientLabTest.TestResult = model.TestResult;
                    patientLabTest.LabValue = model.LabValue;
                    patientLabTest.TestUnit = labTestUnit;
                    patientLabTest.ReferenceLower = model.ReferenceLower;
                    patientLabTest.ReferenceUpper = model.ReferenceUpper;

                    if (model.CustomAttributes != null)
                    {
                        var patientLabTestExtended = (IExtendable)patientLabTest;
                        var customAttributes = unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(ca => ca.ExtendableTypeName == typeof(PatientLabTest).Name).ToList();

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
                                            patientLabTestExtended.ValidateAndSetAttributeValue(attributeConfig, number, User.Identity.Name);
                                        }
                                        break;
                                    case "Selection":
                                        Int32 selection = 0;
                                        if (Int32.TryParse(model.CustomAttributes[i].Value, out selection))
                                        {
                                            patientLabTestExtended.ValidateAndSetAttributeValue(attributeConfig, selection, User.Identity.Name);
                                        }
                                        break;
                                    case "DateTime":
                                        DateTime parsedDate = DateTime.MinValue;
                                        if (DateTime.TryParse(model.CustomAttributes[i].Value, out parsedDate))
                                        {
                                            patientLabTestExtended.ValidateAndSetAttributeValue(attributeConfig, parsedDate, User.Identity.Name);
                                        }
                                        break;
                                    case "String":
                                    default:
                                        patientLabTestExtended.ValidateAndSetAttributeValue(attributeConfig, model.CustomAttributes[i].Value ?? string.Empty, User.Identity.Name);
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
                        unitOfWork.Repository<PatientLabTest>().Update(patientLabTest);
                        unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Test and procedure updated successfully";
                        Response.Cookies.Add(cookie);

                        //if (returnUrl != string.Empty)
                        //{
                        //    return Redirect(returnUrl);
                        //}
                        return Redirect("/Patient/PatientView.aspx?pid=" + patientLabTest.Patient.Id.ToString());
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    var err = string.Empty;
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            err += String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    ModelState.AddModelError("PatientLabTestId", string.Format("Unable to update the Patient Condition: {0}", err));
                }
            }

            // Prepare custom attributes
            var cattributes = unitOfWork.Repository<CustomAttributeConfiguration>()
                .Queryable()
                .Where(ca => ca.ExtendableTypeName == typeof(PatientLabTest).Name)
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

            if (model.CustomAttributes != null)
            {
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
            }

            ViewBag.LabTests = unitOfWork.Repository<LabTest>()
                .Queryable()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray();

            var labTestUnits = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labTestUnits.AddRange(unitOfWork.Repository<LabTestUnit>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Description
                })
                .ToArray());
            ViewBag.LabTestUnits = labTestUnits;

            var labResults = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            labResults.AddRange(unitOfWork.Repository<LabResult>()
                .Queryable()
                .OrderBy(c => c.Description)
                .Select(c => new SelectListItem
                {
                    Value = c.Description,
                    Text = c.Description
                })
                .ToArray());
            ViewBag.TestResults = labResults;

            TempData["returnUrl"] = returnUrl;

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeletePatientLabTest(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var patientLabTest = unitOfWork.Repository<PatientLabTest>()
                .Queryable()
                .Include(i => i.Patient)
                .Include(i => i.LabTest)
                .Include(i => i.TestUnit)
                .SingleOrDefault(p => p.Id == id);

            if (patientLabTest == null)
            {
                ViewBag.Entity = "Patient Lab Test";
                return View("NotFound");
            }

            // Prepare model
            var model = new PatientLabTestDeleteModel
            {
                PatientFullName = patientLabTest.Patient.FullName,
                PatientLabTestId = patientLabTest.Id,
                LabTest = patientLabTest.LabTest.Description,
                TestDate = patientLabTest.TestDate,
                TestResult = patientLabTest.TestResult,
                LabValue = patientLabTest.LabValue,
                LabTestUnit = patientLabTest.TestUnit != null ? patientLabTest.TestUnit.Description : "",
                ReferenceLower = patientLabTest.ReferenceLower,
                ReferenceUpper = patientLabTest.ReferenceUpper
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeletePatientLabTest(PatientLabTestDeleteModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var patientLabTestRepository = unitOfWork.Repository<PatientLabTest>();
            var patientLabTest = patientLabTestRepository.Queryable()
                                                            .Include(x => x.Patient)
                                                            .Include(x => x.LabTest)
                                                            .Include(x => x.TestUnit)
                                                            .FirstOrDefault(x => x.Id == model.PatientLabTestId);

            if (patientLabTest != null)
            {
                var user = GetCurrentUser();

                if (user != null)
                {
                    if (ModelState.IsValid)
                    {
                        var reason = model.ArchiveReason ?? "** NO REASON SPECIFIED ** ";
                        patientLabTest.Archived = true;
                        patientLabTest.ArchivedDate = DateTime.Now;
                        patientLabTest.ArchivedReason = reason;
                        patientLabTest.AuditUser = user;

                        try
                        {
                            patientLabTestRepository.Update(patientLabTest);
                            unitOfWork.Complete();
 
                            HttpCookie cookie = new HttpCookie("PopUpMessage");
                            cookie.Value = "Test and procedure deleted successfully";
                            Response.Cookies.Add(cookie);

                            //return Redirect(returnUrl);
                            return Redirect("/Patient/PatientView.aspx?pid=" + patientLabTest.Patient.Id.ToString());
                        }
                        catch (DbEntityValidationException ex)
                        {
                            var err = string.Empty;
                            foreach (var eve in ex.EntityValidationErrors)
                            {
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    err += String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            ModelState.AddModelError("ArchiveReason", string.Format("Unable to delete the Patient Condition: {0}", err));
                        }
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

        private User GetCurrentUser()
        {
            return unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
        }
    }
}