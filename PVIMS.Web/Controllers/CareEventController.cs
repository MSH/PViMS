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
    public class CareEventController : BaseController
    {
        private static string CurrentMenuItem = "AdminCareEvent";

        private readonly IUnitOfWorkInt unitOfWork;

        public CareEventController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: CareEvent
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [HttpGet]
        public ActionResult AddCareEvent()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View(new CareEventAddModel());
        }

        [HttpPost]
        public ActionResult AddCareEvent(CareEventAddModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (ModelState.IsValid)
            {
                if (unitOfWork.Repository<CareEvent>().Queryable().Any(ce => ce.Description == model.Description))
                {
                    ModelState.AddModelError("Description", "A care event with the specified description already exists.");

                    return View(model);
                }

                if (Regex.Matches(model.Description, @"[a-zA-Z ']").Count < model.Description.Length)
                {
                    ModelState.AddModelError("Description", "Description contains invalid characters (Enter A-Z, a-z)");

                    return View(model);
                }

                var encodedDescription = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.Description, false);
                var newCareEvent = new CareEvent { Description = encodedDescription };

                try
                {
                    unitOfWork.Repository<CareEvent>().Save(newCareEvent);

                    return Redirect("/Admin/ManageCareEvent.aspx");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to add the care event: {0}", ex.Message));
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult EditCareEvent(int ceId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var careEvent = unitOfWork.Repository<CareEvent>().Queryable().SingleOrDefault(ce => ce.Id == ceId);

            return View(new CareEventEditModel { CareEventId = careEvent.Id, Description = careEvent.Description });
        }

        [HttpPost]
        public ActionResult EditCareEvent(CareEventEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (ModelState.IsValid)
            {
                if (unitOfWork.Repository<CareEvent>().Queryable().Any(ce => ce.Id != model.CareEventId && ce.Description == model.Description))
                {
                    ModelState.AddModelError("Description", "Another care event with the specified description already exists.");

                    return View(model);
                }

                if (Regex.Matches(model.Description, @"[a-zA-Z ']").Count < model.Description.Length)
                {
                    ModelState.AddModelError("Description", "Description contains invalid characters (Enter A-Z, a-z)");

                    return View(model);
                }

                var careEvent = unitOfWork.Repository<CareEvent>().Queryable().SingleOrDefault(ce => ce.Id == model.CareEventId);

                if (careEvent == null)
                {
                    ModelState.AddModelError("", "Unable to update the care event. The care event could not be found in the data store.");

                    return View(model);
                }

                try
                {
                    var encodedDescription = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.Description, false);
                    careEvent.Description = encodedDescription;

                    unitOfWork.Repository<CareEvent>().Update(careEvent);
                    unitOfWork.Complete();

                    return Redirect("/Admin/ManageCareEvent.aspx");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to update the care event: {0}", ex.Message));
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteCareEvent(int ceId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var careEvent = unitOfWork.Repository<CareEvent>().Get(ceId);

            if (careEvent != null)
            {
                var careEventInUse = unitOfWork.Repository<WorkPlanCareEvent>().Queryable().Any(w => w.CareEvent.Id == ceId);

                if (careEventInUse)
                {
                    return View("Error", new HandleErrorInfo(new Exception("Unable to delete the Care Event. It is being used by a work plan."), "CareEvent", "DeleteCareEvent"));
                }

                unitOfWork.Repository<CareEvent>().Delete(careEvent);
                unitOfWork.Complete();
            }

            return Redirect("/Admin/ManageCareEvent.aspx");
        }
    }
}