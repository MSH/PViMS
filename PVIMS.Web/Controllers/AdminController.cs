using Ionic.Zip;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

using VPS.Common.Repositories;
using VPS.Common.Utilities;

using PVIMS.Core.Services;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private static string CurrentMenuItem = "AdminMedDRA";
        private string _summary = "<ul>";
        private string _subDirectory = "";
        private string _mainDirectory = "";

        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMedDraService _meddraService;
        private readonly IInfrastructureService _infrastuctureService;

        public AdminController(IUnitOfWorkInt unitOfWork, IMedDraService meddraService, IInfrastructureService infrastuctureService)
        {
            Check.IsNotNull(meddraService, "meddraService may not be null");

            _unitOfWork = unitOfWork;
            _meddraService = meddraService;
            _infrastuctureService = infrastuctureService;
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: MedDRA
        [HttpGet]
        public ActionResult ManageMedDRA()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var configValue = _unitOfWork.Repository<Config>().Queryable().Single(c => c.ConfigType == ConfigType.MedDRAVersion).ConfigValue;
            var version = !String.IsNullOrEmpty(configValue) ? configValue : "Not Set";

            var model = new MedDRAListModel { CurrentVersion = version };

            model.ListItems = _unitOfWork.Repository<TerminologyMedDra>()
                .Queryable().Include("Parent")
                .Where(tm => tm.MedDraTermType == "SOC")
                .ToArray()
                .Select(tm => new MedDRAListItemModel
                {
                    Code = tm.MedDraCode, 
                    MedDRAId = tm.Id,
                    ParentTerm = tm.Parent != null ? tm.Parent.DisplayName : "",
                    Term = tm.MedDraTerm,
                    TermType = tm.MedDraTermType,
                    Version = tm.MedDraVersion
                })
                .ToArray();

            // Prepare drop downs
            ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "ALL", Text = "All Classes" },
                    new SelectListItem { Value = "SOC", Text = "System Organ Class", Selected = true },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term" }
                };

            return View(model);
        }

        [HttpPost]
        public ActionResult ManageMedDRA(MedDRAListModel model, string button)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (button == "Search")
            {
                if (!String.IsNullOrWhiteSpace(model.FindTerm))
                {
                    if (Regex.Matches(model.FindTerm, @"[a-zA-Z]").Count < model.FindTerm.Length)
                    {
                        ModelState.AddModelError("FindTerm", "Term contains invalid characters (Enter A-Z, a-z)");
                    }
                    if(model.FindTerm.Length < 4)
                    {
                        ModelState.AddModelError("FindTerm", "Term must be at least 4 characters");
                    }
                }
                else
                {
                    ModelState.AddModelError("FindTerm", "Term must be entered");
                }

                if(ModelState.IsValid)
                {
                    if (model.TermType == "All")
                    {
                        model.ListItems = _unitOfWork.Repository<TerminologyMedDra>()
                            .Queryable().Include("Parent")
                            .Where(tm => tm.MedDraTerm.Contains(model.FindTerm))
                            .ToArray()
                            .Select(tm => new MedDRAListItemModel
                            {
                                Code = tm.MedDraCode,
                                MedDRAId = tm.Id,
                                ParentTerm = tm.Parent != null ? tm.Parent.DisplayName : "",
                                Term = tm.MedDraTerm,
                                TermType = tm.MedDraTermType,
                                Version = tm.MedDraVersion
                            })
                            .ToArray();
                    }
                    else
                    {
                        model.ListItems = _unitOfWork.Repository<TerminologyMedDra>()
                            .Queryable().Include("Parent")
                            .Where(tm => tm.MedDraTermType == model.TermType && tm.MedDraTerm.Contains(model.FindTerm))
                            .ToArray()
                            .Select(tm => new MedDRAListItemModel
                            {
                                Code = tm.MedDraCode,
                                MedDRAId = tm.Id,
                                ParentTerm = tm.Parent != null ? tm.Parent.DisplayName : "",
                                Term = tm.MedDraTerm,
                                TermType = tm.MedDraTermType,
                                Version = tm.MedDraVersion
                            })
                            .ToArray();
                    }

                    // Prepare drop downs
                    ViewBag.TermTypes = new[]
                    {
                        new SelectListItem { Value = "ALL", Text = "All Classes" },
                        new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                        new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                        new SelectListItem { Value = "HLT", Text = "High Level Term" },
                        new SelectListItem { Value = "PT", Text = "Preferred Term" },
                        new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                    };

                    return View(model);
                }
            }
            else // Import
            {
                string fileName = Path.GetFileName(model.InputFile.FileName);

                bool err = false;

                /*****************************************
                * Validation
                *****************************************/
                if (Path.GetExtension(fileName).ToLower() != ".zip")
                {
                    _summary += String.Format("<li>ERROR: File is not of type ZIP...</li>");
                    err = true;
                }
                if (Path.GetFileNameWithoutExtension(fileName).ToLower() != "medascii")
                {
                    _summary += String.Format("<li>ERROR: File name incorrect...</li>");
                    err = true;
                }

                if (err == false)
                {
                    try
                    {
                        var generatedDate = DateTime.Now.ToString("med_yyyyMMddhhmmss");

                        _mainDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
                        _subDirectory = String.Format("{0}\\Temp\\{1}\\", System.AppDomain.CurrentDomain.BaseDirectory, generatedDate);

                        // Create folder for purposes of storing zip file content
                        model.InputFile.SaveAs(_mainDirectory + fileName);
                        _summary += String.Format("<li>INFO: Zip file uploaded ...</li>");

                        // create a sub directory for the decompression
                        System.IO.Directory.CreateDirectory(_subDirectory);

                        // Now uncompress file
                        var zip = new ZipFile(_mainDirectory + fileName);
                        zip.ExtractAll(_subDirectory);
                        zip = null;
                        _summary += String.Format("<li>INFO: Zip file extracted to {0}...</li>", generatedDate);

                        _summary += _meddraService.ValidateSourceData(fileName, _subDirectory);
                        _summary += _meddraService.ImportSourceData(fileName, _subDirectory);
                    }
                    catch (Exception ex)
                    {
                        _summary += String.Format("<li>ERROR: {0}...</li>", ex.Message);
                        err = true;
                    }
                }

                _summary += "</ul>";
                model.Summary = _summary;

                model.ListItems = _unitOfWork.Repository<TerminologyMedDra>()
                    .Queryable().Include("Parent")
                    .Where(tm => tm.MedDraTerm.Contains(model.FindTerm))
                    .ToArray()
                    .Select(tm => new MedDRAListItemModel
                    {
                        Code = tm.MedDraCode,
                        MedDRAId = tm.Id,
                        ParentTerm = tm.Parent != null ? tm.Parent.DisplayName : "",
                        Term = tm.MedDraTerm,
                        TermType = tm.MedDraTermType,
                        Version = tm.MedDraVersion
                    })
                    .ToArray();

                // Prepare drop downs
                ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "ALL", Text = "All Classes" },
                    new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                };

                return View(model);
            }

            model.ListItems = _unitOfWork.Repository<TerminologyMedDra>()
                .Queryable().Include("Parent")
                .Where(tm => tm.MedDraTermType == "SOC")
                .ToArray()
                .Select(tm => new MedDRAListItemModel
                {
                    Code = tm.MedDraCode,
                    MedDRAId = tm.Id,
                    ParentTerm = tm.Parent != null ? tm.Parent.DisplayName : "",
                    Term = tm.MedDraTerm,
                    TermType = tm.MedDraTermType,
                    Version = tm.MedDraVersion
                })
                .ToArray();

            // Prepare drop downs
            ViewBag.TermTypes = new[]
                {
                    new SelectListItem { Value = "ALL", Text = "All Classes" },
                    new SelectListItem { Value = "SOC", Text = "System Organ Class" },
                    new SelectListItem { Value = "HLGT", Text = "High Level Group Term" },
                    new SelectListItem { Value = "HLT", Text = "High Level Term" },
                    new SelectListItem { Value = "PT", Text = "Preferred Term" },
                    new SelectListItem { Value = "LLT", Text = "Lowest Level Term", Selected = true }
                };

            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteDatasetElement(long id, string cancelRedirectUrl)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            TempData["cancelRedirectUrl"] = cancelRedirectUrl;
            ViewBag.CancelRedirectUrl = cancelRedirectUrl;

            var element = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(de => de.Id == id);
            var allowDelete = (_infrastuctureService.HasAssociatedData(element) == false);

            DatasetElementDeleteModel model = new DatasetElementDeleteModel() { DatasetElementId = element.Id, DatasetElementName = element.ElementName, AllowDelete = allowDelete };

            ViewData.Model = model;

            ViewBag.Id = id;
            ViewBag.AlertMessage = allowDelete == false ? "Unable to delete this record as it has associated referential data...." : "You are about to delete this record. This action is not reversible....";

            return View();
        }

        [HttpPost]
        public ActionResult DeleteDatasetElement(DatasetElementDeleteModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var cancelRedirectUrl = (TempData["cancelRedirectUrl"] ?? string.Empty).ToString();
            ViewBag.cancelRedirectUrl = cancelRedirectUrl;

            ArrayList errors = new ArrayList();

            if (ModelState.IsValid)
            {
                var element = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(ds => ds.Id == model.DatasetElementId);

                try
                {
                    ArrayList delete = new ArrayList();
                    delete.AddRange(element.DatasetRules.ToArray());
                    foreach (DatasetRule rule in delete)
                    {
                        _unitOfWork.Repository<DatasetRule>().Delete(rule);
                    }
                    _unitOfWork.Repository<Field>().Delete(element.Field);
                    _unitOfWork.Repository<DatasetElement>().Delete(element);
                    _unitOfWork.Complete();

                }
                catch (DbUpdateException ex)
                {
                    errors.Add("Unable to delete element. " + ex.Message);
                }
                if (errors.Count == 0)
                {
                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Dataset element deleted successfully";
                    Response.Cookies.Add(cookie);

                    return Redirect("/Admin/ManageDatasetElement.aspx");
                }
                else
                {
                    AddErrors(errors);
                }
            }

            return View(model);
        }

        private void AddErrors(ArrayList errors)
        {
            foreach (string error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}