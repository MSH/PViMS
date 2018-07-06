using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VPS.Common.Repositories;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    public class DeploymentController : BaseController
    {
        private static string CurrentMenuItem = "";

        private readonly IUnitOfWorkInt _unitOfWork;

        public DeploymentController(IUnitOfWorkInt unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Deployment
        public ActionResult Index()
        {
            var isFirstRun = IsFirstRun();
            if (isFirstRun)
            {
                ExecuteScripts();                
            }

            var deploymentSummary = new DeploymentSummaryViewModel
            {
                CreatedTables = GetCreatedTables(),
                PendingPostDeploymentScripts = GetPendingPostDeploymentScripts(),
                ExecutedPostDeploymentScripts = GetExecutedPostDeploymentScripts(),
                IsFirstRun = isFirstRun,
            };
            return View(deploymentSummary);
        }

        public ActionResult ExecutePendingScripts()
        {
            ExecuteScripts();
            return RedirectToAction("Index");
        }

        public void ExecuteScripts()
        {
            var scriptsFolder = Server.MapPath("~/PostDeploymentScripts/");

            var postDeploymentRepository = _unitOfWork.Repository<PostDeployment>();
            var pendingPostDeploymentScripts = postDeploymentRepository.Queryable()
                                                                    .Where(x => !x.RunDate.HasValue)
                                                                    .OrderBy(u => u.RunRank)
                                                                    .ToList();
             
            foreach (var pendingPostDeploymentScript in pendingPostDeploymentScripts)
            {
                try
                {
                    var fullPath = Path.Combine(scriptsFolder, pendingPostDeploymentScript.ScriptFileName);
                    if (!System.IO.File.Exists(fullPath))
                    {
                        pendingPostDeploymentScript.StatusCode = 404;
                        pendingPostDeploymentScript.StatusMessage = "File not found";
                    }
                    else
                    {
                        postDeploymentRepository.ExecuteSqlCommand(System.IO.File.ReadAllText(fullPath));
                        pendingPostDeploymentScript.StatusCode = 200;
                    }
                }
                catch (Exception exception)
                {
                    pendingPostDeploymentScript.StatusCode = 501;
                    pendingPostDeploymentScript.StatusMessage = exception.Message;
                }
                finally
                {
                    pendingPostDeploymentScript.RunDate = DateTime.Now;
                    postDeploymentRepository.Update(pendingPostDeploymentScript);
                    _unitOfWork.Complete();
                }

            }

        }
        public IEnumerable<string> GetCreatedTables()
        {
            var createdTables = new List<string>
            {
                "Role",
                "User",
                "AttachmentType",
                "Priority",
                "CareEvent",
                "DatasetElementType",
                "LabTestUnit",
                "LabTest",
                "CustomAttribute",
                "SelectDataItem",
                "FieldType",
                "FacilityType",
                "ContextType",
                "EncounterType",
                "Status",
                "MetaType",
                "PostDeploymentScript",
            };
            return createdTables;
        }

        public IEnumerable<PostDeploymentScriptViewModel> GetPendingPostDeploymentScripts()
        {
            var pendingPostDeploymentScripts = _unitOfWork.Repository<PostDeployment>()
                .Queryable()
                .Where(x => !x.RunDate.HasValue)
                .Select(u => new PostDeploymentScriptViewModel
                {
                    Id = u.Id,
                    ScriptGuid = u.ScriptGuid,
                    ScriptFileName = u.ScriptFileName,
                    ScriptDescription = u.ScriptDescription,
                    RunRank = u.RunRank,
                }).OrderBy(u => u.RunRank);
            return pendingPostDeploymentScripts;
        }

        public IEnumerable<PostDeploymentScriptViewModel> GetExecutedPostDeploymentScripts()
        {
            var pendingPostDeploymentScripts = _unitOfWork.Repository<PostDeployment>()
                .Queryable()
                .Where(x => x.RunDate.HasValue)
                .OrderBy(u => u.RunRank)
                .Select(u => new PostDeploymentScriptViewModel
                {
                    Id = u.Id,
                    ScriptGuid = u.ScriptGuid,
                    ScriptFileName = u.ScriptFileName,
                    ScriptDescription = u.ScriptDescription,
                    RunDate = u.RunDate.Value,
                    StatusCode = u.StatusCode,
                    StatusMessage = u.StatusMessage,
                });
            return pendingPostDeploymentScripts;
        }

        public bool IsFirstRun()
        {
            var isFirstRun = _unitOfWork.Repository<PostDeployment>()
                .Queryable()
                .All(x => !x.RunDate.HasValue);
            return isFirstRun;
        }
    }
}