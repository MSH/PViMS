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
    public class CohortController : BaseController
    {
        private static string CurrentMenuItem = "Cohort";

        private readonly IUnitOfWorkInt unitOfWork;

        public CohortController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Cohort
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [HttpGet]
        public ActionResult AddCohort()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var conditions = unitOfWork.Repository<Condition>()
                .Queryable()
                .ToList();

            var cgList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            cgList.AddRange(conditions.Select(cg => new SelectListItem { Value = cg.Id.ToString(), Text = cg.Description }));

            ViewBag.Conditions = cgList;

            return View(new CohortAddModel() { } );
        }

        [HttpPost]
        public ActionResult AddCohort(CohortAddModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (ModelState.IsValid)
            {
                if (unitOfWork.Repository<CohortGroup>().Queryable().Any(cg => cg.CohortName == model.CohortName))
                {
                    ModelState.AddModelError("CohortName", "A cohort with the specified name already exists.");
                }

                if (Regex.Matches(model.CohortName, @"[a-zA-Z0-9 ']").Count < model.CohortName.Length)
                {
                    ModelState.AddModelError("CohortName", "Cohort name contains invalid characters (Enter A-Z, a-z, 0-9)");
                }

                if (unitOfWork.Repository<CohortGroup>().Queryable().Any(cg => cg.CohortCode == model.CohortCode))
                {
                    ModelState.AddModelError("CohortCode", "A cohort with the specified code already exists.");
                }

                if (Regex.Matches(model.CohortCode, @"[-a-zA-Z0-9 ']").Count < model.CohortCode.Length)
                {
                    ModelState.AddModelError("CohortCode", "Cohort code contains invalid characters (Enter -, A-Z, a-z, 0-9)");
                }

                if (model.StartDate != null)
                {
                    if (model.StartDate > DateTime.Today)
                    {
                        ModelState.AddModelError("StartDate", "Start Date should be before current date");
                    }
                }
                else
                {
                    ModelState.AddModelError("StartDate", "Start Date is mandatory");
                }

                if (model.FinishDate != null)
                {
                    if (model.FinishDate > DateTime.Today.AddYears(20))
                    {
                        ModelState.AddModelError("FinishDate", "End Date should be within 20 years");
                    }
                    if (model.FinishDate < model.StartDate)
                    {
                        ModelState.AddModelError("FinishDate", "End Date should be after Start Date");
                    }
                }

                var encodedName = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.CohortName, false);
                var encodedCode = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.CohortCode, false);
                var condition = unitOfWork.Repository<Condition>().Queryable().SingleOrDefault(cg => cg.Id == model.ConditionId);

                var newCohort = new CohortGroup 
                { 
                    CohortName = encodedName,
                    CohortCode = encodedCode,
                    Condition = condition,
                    FinishDate = model.FinishDate,
                    StartDate = Convert.ToDateTime(model.StartDate),
                    MaxEnrolment = 0,
                    MinEnrolment = 0,
                    LastPatientNo = 0
                };

                try
                {
                    if (ModelState.IsValid)
                    {
                        unitOfWork.Repository<CohortGroup>().Save(newCohort);

                        return Redirect("/Cohort/CohortSearch.aspx");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CohortName", string.Format("Unable to add the cohort: {0}", ex.Message));
                }
            }

            var conditions = unitOfWork.Repository<Condition>()
                .Queryable()
                .ToList();

            var cgList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            cgList.AddRange(conditions.Select(cg => new SelectListItem { Value = cg.Id.ToString(), Text = cg.Description }));

            ViewBag.Conditions = cgList;

            return View(model);
        }

        [HttpGet]
        public ActionResult EditCohort(int cgId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var cohortGroup = unitOfWork.Repository<CohortGroup>().Queryable().SingleOrDefault(cg => cg.Id == cgId);

            var conditions = unitOfWork.Repository<Condition>()
                .Queryable()
                .ToList();

            var cgList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            cgList.AddRange(conditions.Select(cg => new SelectListItem { Value = cg.Id.ToString(), Text = cg.Description }));

            ViewBag.Conditions = cgList;

            return View(new CohortEditModel { CohortId = cohortGroup.Id, CohortName = cohortGroup.CohortName, CohortCode = cohortGroup.CohortCode, StartDate = cohortGroup.StartDate, FinishDate = cohortGroup.FinishDate, ConditionId = cohortGroup.Condition != null ? cohortGroup.Condition.Id : 0 });
        }

        [HttpPost]
        public ActionResult EditCohort(CohortEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (ModelState.IsValid)
            {
                if (unitOfWork.Repository<CohortGroup>().Queryable().Any(cg => cg.Id != model.CohortId && cg.CohortName == model.CohortName))
                {
                    ModelState.AddModelError("CohortName", "A cohort with the specified name already exists.");
                }

                if (Regex.Matches(model.CohortName, @"[a-zA-Z0-9 ']").Count < model.CohortName.Length)
                {
                    ModelState.AddModelError("CohortName", "Cohort name contains invalid characters (Enter A-Z, a-z, 0-9)");
                }

                if (unitOfWork.Repository<CohortGroup>().Queryable().Any(cg => cg.Id != model.CohortId && cg.CohortCode == model.CohortCode))
                {
                    ModelState.AddModelError("CohortCode", "A cohort with the specified code already exists.");
                }

                if (Regex.Matches(model.CohortCode, @"[-a-zA-Z0-9 ']").Count < model.CohortCode.Length)
                {
                    ModelState.AddModelError("CohortCode", "Cohort code contains invalid characters (Enter -, A-Z, a-z, 0-9)");
                }

                if (model.FinishDate != null)
                {
                    if (model.FinishDate > DateTime.Today.AddYears(20))
                    {
                        ModelState.AddModelError("FinishDate", "End Date should be within 20 years");
                    }
                    if (model.FinishDate < model.StartDate)
                    {
                        ModelState.AddModelError("FinishDate", "End Date should be after Start Date");
                    }
                }

                var encodedName = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.CohortName, false);
                var encodedCode = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.CohortCode, false);
                var condition = unitOfWork.Repository<Condition>().Queryable().SingleOrDefault(cg => cg.Id == model.ConditionId);

                var cohortGroup = unitOfWork.Repository<CohortGroup>().Queryable().SingleOrDefault(cg => cg.Id == model.CohortId);

                if (cohortGroup == null)
                {
                    ModelState.AddModelError("CohortName", "Unable to update the cohort. The cohort could not be found in the data store.");
                }

                try
                {
                    cohortGroup.CohortName = encodedName;
                    cohortGroup.CohortCode = encodedCode;
                    cohortGroup.StartDate = model.StartDate;
                    cohortGroup.FinishDate = model.FinishDate;
                    cohortGroup.Condition = condition;

                    if (ModelState.IsValid)
                    {
                        unitOfWork.Repository<CohortGroup>().Update(cohortGroup);
                        unitOfWork.Complete();

                        return Redirect("/Cohort/CohortSearch.aspx");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CohortName", string.Format("Unable to update the cohort: {0}", ex.Message));
                }
            }

            var conditions = unitOfWork.Repository<Condition>()
                .Queryable()
                .ToList();

            var cgList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            cgList.AddRange(conditions.Select(cg => new SelectListItem { Value = cg.Id.ToString(), Text = cg.Description }));

            ViewBag.Conditions = cgList;

            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteCohort(int cgId)
        {
            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            ViewBag.MenuItem = CurrentMenuItem;

            var cohortGroup = unitOfWork.Repository<CohortGroup>().Queryable().SingleOrDefault(cg => cg.Id == cgId);

            if (cohortGroup != null)
            {
                var cohortGroupInUse = unitOfWork.Repository<CohortGroupEnrolment>().Queryable().Any(cge => cge.CohortGroup.Id == cgId);

                if (cohortGroupInUse)
                {
                    return View("CohortName", new HandleErrorInfo(new Exception("Unable to delete the Cohort as it is currently in use."), "Cohort", "DeleteCohort"));
                }

                unitOfWork.Repository<CohortGroup>().Delete(cohortGroup);
                unitOfWork.Complete();

                HttpCookie cookie = new HttpCookie("PopUpMessage");
                cookie.Value = "Cohort record deleted successfully";
                Response.Cookies.Add(cookie);
            }

            return Redirect("/Cohort/CohortSearch.aspx");
        }
    }
}