using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class ActivityController : BaseController
    {
        private static string CurrentMenuItem = "ActiveReporting";

        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workflowService;

        public ActivityController(IUnitOfWorkInt unitOfWork, IWorkFlowService workFlowService)
        {
            _unitOfWork = unitOfWork;
            _workflowService = workFlowService;
        }

        public ActionResult Index(int reportInstanceId)
        {
            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.Id == reportInstanceId);

            ViewBag.MenuItem = string.Equals(reportInstance.WorkFlow.WorkFlowGuid.ToString(), "892F3305-7819-4F18-8A87-11CBA3AEE219", StringComparison.InvariantCultureIgnoreCase) ? "ActiveReporting" : "SpontaneousReporting";

            var returnUrl = "/Analytical/ReportInstanceList.aspx?wuid=" + reportInstance.WorkFlow.WorkFlowGuid.ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var activities = _unitOfWork.Repository<ActivityExecutionStatusEvent>().Queryable().Include(evt => evt.ExecutionStatus.Activity).Include(evt2 => evt2.EventCreatedBy).Where(evt => evt.ActivityInstance.ReportInstance.Id == reportInstanceId).OrderBy(evt => evt.EventDateTime).ToList().Select(a => new ActivityListItem
            {
                Activity = a.ExecutionStatus.Activity.QualifiedName,
                Comments = a.Comments,
                ExecutionBy = a.EventCreatedBy != null ? a.EventCreatedBy.FullName : string.Empty,
                ExecutionDate = a.EventDateTime.ToString("yyyy-MM-dd HH:mm"),
                ExecutionEvent = a.ExecutionStatus.Description,
                ReceiptDate = a.ContextDateTime != null ? Convert.ToDateTime(a.ContextDateTime).ToString("yyyy-MM-dd") : "",
                ReceiptCode = a.ContextCode,
                PatientSummaryFileId = a.Attachments.SingleOrDefault(att => att.Description == "PatientSummary") == null ? 0 : a.Attachments.SingleOrDefault(att => att.Description == "PatientSummary").Id,
                PatientExtractFileId = a.Attachments.SingleOrDefault(att => att.Description == "PatientExtract") == null ? 0 : a.Attachments.SingleOrDefault(att => att.Description == "PatientExtract").Id,
                E2bXmlFileId = a.Attachments.SingleOrDefault(att => att.Description == "E2b") == null ? 0 : a.Attachments.SingleOrDefault(att => att.Description == "E2b").Id
            });

            ViewData.Model = activities;

            return View();
        }

        [HttpGet]
        public ActionResult AddActivity(int activityInstanceId, int activityExecutionStatusId)
        {
            var returnUrl = (TempData["returnUrl"] ?? Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var activityInstance = _unitOfWork.Repository<ActivityInstance>().Queryable()
                .Include(ai => ai.ReportInstance.WorkFlow)
                .Single(ai => ai.Id == activityInstanceId);
            var newExecutionStatus = _unitOfWork.Repository<ActivityExecutionStatus>().Queryable().Single(aes => aes.Id == activityExecutionStatusId).Description;

            ViewBag.MenuItem = string.Equals(activityInstance.ReportInstance.WorkFlow.WorkFlowGuid.ToString(), "892F3305-7819-4F18-8A87-11CBA3AEE219", StringComparison.InvariantCultureIgnoreCase) ? "ActiveReporting" : "SpontaneousReporting";
            ViewBag.DisplayContext = newExecutionStatus == "E2BSUBMITTED";

            var model = new ActivityAddModel()
            {
                ActivityInstanceId = activityInstanceId,
                Comments = string.Empty,
                CurrentExecutionStatus = activityInstance.CurrentStatus.Description,
                NewExecutionStatus = newExecutionStatus,
                ContextCode = string.Empty,
                ContextDate = null
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddActivity(ActivityAddModel model)
        {

            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var activityInstance = _unitOfWork.Repository<ActivityInstance>().Queryable()
                .Include(ai => ai.ReportInstance.WorkFlow)
                .Single(ai => ai.Id == model.ActivityInstanceId);
            ViewBag.MenuItem = string.Equals(activityInstance.ReportInstance.WorkFlow.WorkFlowGuid.ToString(), "892F3305-7819-4F18-8A87-11CBA3AEE219", StringComparison.InvariantCultureIgnoreCase) ? "ActiveReporting" : "SpontaneousReporting";
            ViewBag.DisplayContext = model.NewExecutionStatus == "E2BSUBMITTED";

            if (ModelState.IsValid)
            {
                if(!String.IsNullOrWhiteSpace(model.Comments))
                {
                    if (Regex.Matches(model.Comments, @"[a-zA-Z0-9 ']").Count < model.Comments.Length)
                    {
                        ModelState.AddModelError("Comments", "Comments contains invalid characters (Enter A-Z, a-z, 0-9)");
                    }
                }
                if (!String.IsNullOrWhiteSpace(model.ContextCode))
                {
                    if (Regex.Matches(model.ContextCode, @"[a-zA-Z0-9 ']").Count < model.ContextCode.Length)
                    {
                        ModelState.AddModelError("ContextCode", "Receipt Code contains invalid characters (Enter A-Z, a-z, 0-9)");
                    }
                }

                var encodedComments = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.Comments, false);
                var encodedContextCode = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ContextCode, false);

                try
                {
                    if (ModelState.IsValid)
                    {
                        _workflowService.ExecuteActivity(activityInstance.ReportInstance.ContextGuid, model.NewExecutionStatus, encodedComments, model.ContextDate, encodedContextCode);

                        if(activityInstance.ReportInstance.WorkFlow.Description  == "New Active Surveilliance Report") { return Redirect("/Analytical/ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219"); };
                        return Redirect("/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Comments", string.Format("Unable to add the activity: {0}", ex.Message));
                }
            }

            return View(model);
        }

    }
}