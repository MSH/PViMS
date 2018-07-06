using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using VPS.Common.Repositories;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class EncounterTypeController : BaseController
    {
        private static string CurrentMenuItem = "AdminEncounterType";

        private readonly IUnitOfWorkInt unitOfWork;

        public EncounterTypeController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: EncounterType
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [HttpGet]
        public ActionResult AddEncounterType()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var workPlans = unitOfWork.Repository<WorkPlan>()
                .Queryable()
                .ToList();

            var wpList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            wpList.AddRange(workPlans.Select(wp => new SelectListItem { Value = wp.Id.ToString(), Text = wp.Description }));

            ViewBag.WorkPlans = wpList;

            return View(new EncounterTypeAddModel());
        }

        [HttpPost]
        public ActionResult AddEncounterType(EncounterTypeAddModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (unitOfWork.Repository<EncounterType>().Queryable().Any(et => et.Description == model.Description))
            {
                ModelState.AddModelError("Description", "An encounter type with the specified description already exists.");
            }

            if (Regex.Matches(model.Description, @"[a-zA-Z ']").Count < model.Description.Length)
            {
                ModelState.AddModelError("Description", "Description contains invalid characters (Enter A-Z, a-z)");
            }

            if (!String.IsNullOrWhiteSpace(model.Help))
            {
                if (Regex.Matches(model.Help, @"[a-zA-Z ']").Count < model.Help.Length)
                {
                    ModelState.AddModelError("Help", "Help contains invalid characters (Enter A-Z, a-z)");
                }
            }

            if (ModelState.IsValid)
            {
                var encodedDescription = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.Description, false);
                var encodedHelp = !String.IsNullOrWhiteSpace(model.Help) ? System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.Help, false) : "";

                var newEncounterType = new EncounterType { Description = encodedDescription, Help = encodedHelp };

                var workPlan = unitOfWork.Repository<WorkPlan>().Queryable().SingleOrDefault(wp => wp.Id == model.WorkPlanId);

                var newEncounterTypeWorkPlan = new EncounterTypeWorkPlan { CohortGroup = null, EncounterType = newEncounterType, WorkPlan = workPlan };

                try
                {
                    unitOfWork.Repository<EncounterType>().Save(newEncounterType);
                    unitOfWork.Repository<EncounterTypeWorkPlan>().Save(newEncounterTypeWorkPlan);

                    return Redirect("/Admin/ManageEncounterType.aspx");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to add the encounter type: {0}", ex.Message));
                }
            }

            var workPlans = unitOfWork.Repository<WorkPlan>()
                .Queryable()
                .ToList();

            var wpList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            wpList.AddRange(workPlans.Select(wp => new SelectListItem { Value = wp.Id.ToString(), Text = wp.Description }));

            ViewBag.WorkPlans = wpList;

            return View(model);
        }

        [HttpGet]
        public ActionResult EditEncounterType(int etId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var encounterType = unitOfWork.Repository<EncounterType>().Queryable().SingleOrDefault(et => et.Id == etId);

            var workPlans = unitOfWork.Repository<WorkPlan>()
                .Queryable()
                .ToList();

            var wpList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            wpList.AddRange(workPlans.Select(wp => new SelectListItem { Value = wp.Id.ToString(), Text = wp.Description }));

            ViewBag.WorkPlans = wpList;

            var encounterTypeWorkPlan = unitOfWork.Repository<EncounterTypeWorkPlan>().Queryable().SingleOrDefault(etwp => etwp.EncounterType.Id == encounterType.Id);
            int workPlanId = 0;
            if(encounterTypeWorkPlan != null) {
                workPlanId = encounterTypeWorkPlan.WorkPlan.Id;
            }

            return View(new EncounterTypeEditModel { EncounterTypeId = encounterType.Id, Description = encounterType.Description, WorkPlanId = workPlanId, Help = encounterType.Help });
        }

        [HttpPost]
        public ActionResult EditEncounterType(EncounterTypeEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (unitOfWork.Repository<EncounterType>().Queryable().Any(et => et.Id != model.EncounterTypeId && et.Description == model.Description))
            {
                ModelState.AddModelError("Description", "Another encounter type with the specified description already exists.");
            }

            if (Regex.Matches(model.Description, @"[a-zA-Z ']").Count < model.Description.Length)
            {
                ModelState.AddModelError("Description", "Description contains invalid characters (Enter A-Z, a-z)");
            }

            if (!String.IsNullOrWhiteSpace(model.Help))
            {
                if (Regex.Matches(model.Help, @"[a-zA-Z ']").Count < model.Help.Length)
                {
                    ModelState.AddModelError("Help", "Help contains invalid characters (Enter A-Z, a-z)");
                }
            }

            if (ModelState.IsValid)
            {
                var encodedDescription = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.Description, false);
                var encodedHelp = !String.IsNullOrWhiteSpace(model.Help) ? System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.Help, false) : "";

                var encounterType = unitOfWork.Repository<EncounterType>().Queryable().SingleOrDefault(et => et.Id == model.EncounterTypeId);

                if (encounterType == null)
                {
                    ModelState.AddModelError("", "Unable to update the encounter type. The encounter type could not be found in the data store.");
                }

                try
                {
                    encounterType.Description = encodedDescription;
                    encounterType.Help = encodedHelp;

                    unitOfWork.Repository<EncounterType>().Update(encounterType);

                    var encounterTypeWorkPlan = unitOfWork.Repository<EncounterTypeWorkPlan>().Queryable().SingleOrDefault(etwp => etwp.EncounterType.Id == model.EncounterTypeId);

                    var workPlan = unitOfWork.Repository<WorkPlan>().Queryable().SingleOrDefault(wp => wp.Id == model.WorkPlanId);

                    if(workPlan == null)
                    {
                        if (encounterTypeWorkPlan != null) {
                            unitOfWork.Repository<EncounterTypeWorkPlan>().Delete(encounterTypeWorkPlan);
                        }
                    }
                    else
                    {
                        if (encounterTypeWorkPlan == null) {
                            encounterTypeWorkPlan = new EncounterTypeWorkPlan { CohortGroup = null, EncounterType = encounterType, WorkPlan = workPlan };
                            unitOfWork.Repository<EncounterTypeWorkPlan>().Save(encounterTypeWorkPlan);
                        }
                        else
                        {
                            encounterTypeWorkPlan.WorkPlan = workPlan;
                            unitOfWork.Repository<EncounterTypeWorkPlan>().Update(encounterTypeWorkPlan);
                        }
                    }

                    unitOfWork.Complete();

                    return Redirect("/Admin/ManageEncounterType.aspx");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to update the encounter type: {0}", ex.Message));
                }
            }

            var workPlans = unitOfWork.Repository<WorkPlan>()
                .Queryable()
                .ToList();

            var wpList = new List<SelectListItem> { { new SelectListItem { Value = "", Text = "" } } };
            wpList.AddRange(workPlans.Select(wp => new SelectListItem { Value = wp.Id.ToString(), Text = wp.Description }));

            ViewBag.WorkPlans = wpList;

            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteEncounterType(int etId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var encounterType = unitOfWork.Repository<EncounterType>().Get(etId);

            if (encounterType != null)
            {
                var encounterTypeInUse = unitOfWork.Repository<Encounter>().Queryable().Any(e => e.EncounterType.Id == etId);

                if (encounterTypeInUse)
                {
                    return View("Error", new HandleErrorInfo(new Exception("Unable to delete the EncounterType. It is being used by encounters."), "EncounterType", "DeleteEncounterType"));
                }
            }

            return Redirect("/Admin/ManageEncounterType.aspx");
        }
    }
}