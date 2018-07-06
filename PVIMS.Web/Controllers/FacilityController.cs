using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class FacilityController : BaseController
    {
        private static string CurrentMenuItem = "AdminFacility";

        private readonly IUnitOfWorkInt unitOfWork;

        public FacilityController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Facility
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var facilities = unitOfWork.Repository<Facility>().Queryable().OrderBy(f => f.FacilityName).ToList().Select(f => new FacilityListItem
            {
                FacilityId = f.Id,
                FacilityName = f.FacilityName,
                FacilityCode = f.FacilityCode,
                FacilityType = f.FacilityType.Description
            });

            ViewData.Model = facilities;

            return View();
        }

        [HttpGet]
        public ActionResult AddFacility()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewBag.FacilityTypes = unitOfWork.Repository<FacilityType>()
                .Queryable()
                .Select(ft => new SelectListItem { Value = ft.Id.ToString(), Text = ft.Description })
                .ToList();

            ViewBag.OrgUnits = unitOfWork.Repository<OrgUnit>()
                .Queryable()
                .Where(ou => ou.OrgUnitType.Description == "Region")
                .Select(ou => new SelectListItem { Value = ou.Id.ToString(), Text = ou.Name })
                .ToList();

            return View(new FacilityAddModel());
        }

        [HttpPost]
        public ActionResult AddFacility(FacilityAddModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewBag.FacilityTypes = unitOfWork.Repository<FacilityType>()
                .Queryable()
                .Select(ft => new SelectListItem { Value = ft.Id.ToString(), Text = ft.Description })
                .ToList();

            ViewBag.OrgUnits = unitOfWork.Repository<OrgUnit>()
                .Queryable()
                .Where(ou => ou.OrgUnitType.Description == "Region")
                .Select(ou => new SelectListItem { Value = ou.Id.ToString(), Text = ou.Name })
                .ToList();

            if (ModelState.IsValid)
            {
                if (unitOfWork.Repository<Facility>().Queryable().Any(f => f.FacilityName == model.FacilityName))
                {
                    ModelState.AddModelError("FacilityName", "A facility with the specified facility name already exists.");

                    return View(model);
                }

                if (Regex.Matches(model.FacilityName, @"[-a-zA-Z0-9. '()]").Count < model.FacilityName.Length)
                {
                    ModelState.AddModelError("FacilityName", "Facility name contains invalid characters (Enter A-Z, a-z, 0-9, space, period, apostrophe, round brackets)");

                    return View(model);
                }

                if (unitOfWork.Repository<Facility>().Queryable().Any(f => f.FacilityCode == model.FacilityCode))
                {
                    ModelState.AddModelError("FacilityCode", "A facility with the specified facility code already exists.");

                    return View(model);
                }

                if (Regex.Matches(model.FacilityCode, @"[-a-zA-Z0-9]").Count < model.FacilityCode.Length)
                {
                    ModelState.AddModelError("FacilityCode", "Facility code contains invalid characters (Enter A-Z, a-z, 0-9)");

                    return View(model);
                }

                if (!String.IsNullOrEmpty(model.TelNumber))
                {
                    if (Regex.Matches(model.TelNumber, @"[-a-zA-Z0-9]").Count < model.TelNumber.Length)
                    {
                        ModelState.AddModelError("TelNumber", "Telephone number contains invalid characters (Enter A-Z, a-z, 0-9)");

                        return View(model);
                    }
                }

                if (!String.IsNullOrEmpty(model.MobileNumber))
                {
                    if (Regex.Matches(model.MobileNumber, @"[-a-zA-Z0-9]").Count < model.MobileNumber.Length)
                    {
                        ModelState.AddModelError("MobileNumber", "Mobile number contains invalid characters (Enter A-Z, a-z, 0-9)");

                        return View(model);
                    }
                }

                if (!String.IsNullOrEmpty(model.FaxNumber))
                {
                    if (Regex.Matches(model.FaxNumber, @"[a-zA-Z0-9]").Count < model.FaxNumber.Length)
                    {
                        ModelState.AddModelError("FaxNumber", "Fax number contains invalid characters (Enter A-Z, a-z, 0-9)");

                        return View(model);
                    }
                }

                var encodedName = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.FacilityName, false);
                var encodedCode = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.FacilityCode, false);
                var encodedTelNumber = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.TelNumber, false);
                var encodedMobileNumber = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.MobileNumber, false);
                var encodedFaxNumber = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.FaxNumber, false);
                var facilityType = unitOfWork.Repository<FacilityType>().Queryable().SingleOrDefault(ft => ft.Id == model.FacilityTypeId);
                var orgUnit = unitOfWork.Repository<OrgUnit>().Queryable().SingleOrDefault(ou => ou.Id == model.OrgUnitId);

                var newFacility = new Facility 
                { 
                    FacilityName = encodedName,
                    FacilityCode = encodedCode,
                    FacilityType = facilityType,
                    OrgUnit = orgUnit,
                    FaxNumber = encodedFaxNumber,
                    MobileNumber = encodedMobileNumber,
                    TelNumber = encodedTelNumber 
                };

                try
                {
                    unitOfWork.Repository<Facility>().Save(newFacility);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to add the facility: {0}", ex.Message));
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult EditFacility(int fId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewBag.FacilityTypes = unitOfWork.Repository<FacilityType>()
                .Queryable()
                .Select(ft => new SelectListItem { Value = ft.Id.ToString(), Text = ft.Description })
                .ToList();

            ViewBag.OrgUnits = unitOfWork.Repository<OrgUnit>()
                .Queryable()
                .Where(ou => ou.OrgUnitType.Description == "Region")
                .Select(ou => new SelectListItem { Value = ou.Id.ToString(), Text = ou.Name })
                .ToList();

            var facility = unitOfWork.Repository<Facility>().Queryable().SingleOrDefault(f => f.Id == fId);

            return View(new FacilityEditModel { FacilityId = facility.Id, FacilityCode = facility.FacilityCode, FacilityName = facility.FacilityName, FacilityTypeId = facility.FacilityType.Id, OrgUnitId = facility.OrgUnit != null ? facility.OrgUnit.Id : 0, FaxNumber = facility.FaxNumber, MobileNumber = facility.MobileNumber, TelNumber = facility.TelNumber });
        }

        [HttpPost]
        public ActionResult EditFacility(FacilityEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewBag.FacilityTypes = unitOfWork.Repository<FacilityType>()
                .Queryable()
                .Select(ft => new SelectListItem { Value = ft.Id.ToString(), Text = ft.Description })
                .ToList();

            ViewBag.OrgUnits = unitOfWork.Repository<OrgUnit>()
                .Queryable()
                .Where(ou => ou.OrgUnitType.Description == "Region")
                .Select(ou => new SelectListItem { Value = ou.Id.ToString(), Text = ou.Name })
                .ToList();

            if (ModelState.IsValid)
            {
                if (unitOfWork.Repository<Facility>().Queryable().Any(f => f.Id != model.FacilityId && f.FacilityName == model.FacilityName))
                {
                    ModelState.AddModelError("FacilityName", "Another facility with the specified name already exists.");

                    return View(model);
                }

                if (Regex.Matches(model.FacilityName, @"[-a-zA-Z0-9. '()]").Count < model.FacilityName.Length)
                {
                    ModelState.AddModelError("FacilityName", "Facility name contains invalid characters (Enter A-Z, a-z, 0-9, space, period, apostrophe, round brackets)");

                    return View(model);
                }

                if (unitOfWork.Repository<Facility>().Queryable().Any(f => f.Id != model.FacilityId && f.FacilityCode == model.FacilityCode))
                {
                    ModelState.AddModelError("FacilityCode", "Another facility with the specified code already exists.");

                    return View(model);
                }

                if (Regex.Matches(model.FacilityCode, @"[-a-zA-Z0-9]").Count < model.FacilityCode.Length)
                {
                    ModelState.AddModelError("FacilityCode", "Facility code contains invalid characters (Enter A-Z, a-z, 0-9)");

                    return View(model);
                }

                if (!String.IsNullOrEmpty(model.TelNumber))
                {
                    if (Regex.Matches(model.TelNumber, @"[-a-zA-Z0-9]").Count < model.TelNumber.Length)
                    {
                        ModelState.AddModelError("TelNumber", "Telephone number contains invalid characters (Enter A-Z, a-z, 0-9)");

                        return View(model);
                    }
                }

                if (!String.IsNullOrEmpty(model.MobileNumber))
                {
                    if (Regex.Matches(model.MobileNumber, @"[-a-zA-Z0-9]").Count < model.MobileNumber.Length)
                    {
                        ModelState.AddModelError("MobileNumber", "Mobile number contains invalid characters (Enter A-Z, a-z, 0-9)");

                        return View(model);
                    }
                }

                if (!String.IsNullOrEmpty(model.FaxNumber))
                {
                    if (Regex.Matches(model.FaxNumber, @"[a-zA-Z0-9]").Count < model.FaxNumber.Length)
                    {
                        ModelState.AddModelError("FaxNumber", "Fax number contains invalid characters (Enter A-Z, a-z, 0-9)");

                        return View(model);
                    }
                }

                var encodedName = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.FacilityName, false);
                var encodedCode = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.FacilityCode, false);
                var encodedTelNumber = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.TelNumber, false);
                var encodedMobileNumber = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.MobileNumber, false);
                var encodedFaxNumber = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.FaxNumber, false);
                var facilityType = unitOfWork.Repository<FacilityType>().Queryable().SingleOrDefault(ft => ft.Id == model.FacilityTypeId);
                var orgUnit = unitOfWork.Repository<OrgUnit>().Queryable().SingleOrDefault(ou => ou.Id == model.OrgUnitId);

                var facility = unitOfWork.Repository<Facility>().Queryable().SingleOrDefault(f => f.Id == model.FacilityId);

                if (facility == null)
                {
                    ModelState.AddModelError("", "Unable to update the facility. The facility could not be found in the data store.");

                    return View(model);
                }

                try
                {
                    facility.FacilityCode = encodedCode;
                    facility.FacilityName = encodedName;
                    facility.FacilityType = facilityType;
                    facility.OrgUnit = orgUnit;
                    facility.FaxNumber = encodedFaxNumber;
                    facility.MobileNumber = encodedMobileNumber;
                    facility.TelNumber = encodedTelNumber;

                    unitOfWork.Repository<Facility>().Update(facility);
                    unitOfWork.Complete();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to update the facility: {0}", ex.Message));
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteFacility(int fId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var facility = unitOfWork.Repository<Facility>().Get(fId);

            if (facility != null)
            {
                var facilityInUse = (facility.PatientFacilities.Count > 0 || facility.UserFacilities.Count > 0);

                if (facilityInUse)
                {
                    return View("Error", new HandleErrorInfo(new Exception("Unable to delete the Facility. It is being used by a patient or user."), "Facility", "DeleteFacility"));
                }

                unitOfWork.Repository<Facility>().Delete(facility);
                unitOfWork.Complete();
            }

            return RedirectToAction("Index");
        }
    }
}