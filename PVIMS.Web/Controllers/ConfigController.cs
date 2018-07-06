using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class ConfigController : BaseController
    {
        private static string CurrentMenuItem = "AdminConfig";

        private readonly IUnitOfWorkInt unitOfWork;

        public ConfigController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Config
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [HttpGet]
        public ActionResult EditConfig(int cId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var config = unitOfWork.Repository<Config>().Queryable().SingleOrDefault(c => c.Id == cId);

            ViewBag.ConfigValue1Items = new[]
                {
                    new SelectListItem { Value = "E2B(R2) ICH Report", Text = "E2B(R2) ICH Report", Selected = true },
                    new SelectListItem { Value = "E2B(R3) ICH Report", Text = "E2B(R3) ICH Report" }
                };

            ViewBag.ConfigValue3Items = new[]
                {
                    new SelectListItem { Value = "Both Scales", Text = "Both Scales", Selected = true },
                    new SelectListItem { Value = "WHO Scale", Text = "WHO Scale" },
                    new SelectListItem { Value = "Naranjo Scale", Text = "Naranjo Scale" }
                };

            var configValue1 = "";
            var configValue2 = "";
            var configValue3 = "";
            var configValue4 = "";
            var configValue5 = "";

            switch (config.ConfigType)
            {
                case ConfigType.E2BVersion:
                    configValue1 = config.ConfigValue;
                    break;
                case ConfigType.WebServiceSubscriberList:
                    configValue2 = config.ConfigValue;
                    break;
                case ConfigType.AssessmentScale:
                    configValue3 = config.ConfigValue;
                    break;
                case ConfigType.ReportInstanceNewAlertCount:
                    configValue4 = config.ConfigValue;
                    break;
                case ConfigType.MedicationOnsetCheckPeriodWeeks:
                    configValue5 = config.ConfigValue;
                    break;
                default:
                    break;
            }

            return View(new ConfigEditModel { ConfigId = config.Id, ConfigType = config.ConfigType.ToString(), ConfigValue1 = configValue1, ConfigValue2 = configValue2, ConfigValue3 = configValue3, ConfigValue4 = configValue4, ConfigValue5 = configValue5 });
        }

        [HttpPost]
        public ActionResult EditConfig(ConfigEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (ModelState.IsValid)
            {
                if(!String.IsNullOrEmpty(model.ConfigValue2))
                {
                    if (Regex.Matches(model.ConfigValue2, @"[-a-zA-Z0-9, ']").Count < model.ConfigValue2.Length)
                    {
                        ModelState.AddModelError("ConfigValue2", "Value contains invalid characters (Enter A-Z, a-z, 0-9, comma, space, hyphen)");

                        return View(model);
                    }
                }
                if (!String.IsNullOrEmpty(model.ConfigValue4))
                {
                    if (Regex.Matches(model.ConfigValue4, @"[0-9]").Count < model.ConfigValue4.Length)
                    {
                        ModelState.AddModelError("ConfigValue4", "Value contains invalid characters (Enter 0-9)");

                        return View(model);
                    }
                }
                if (!String.IsNullOrEmpty(model.ConfigValue5))
                {
                    if (Regex.Matches(model.ConfigValue5, @"[0-9]").Count < model.ConfigValue5.Length)
                    {
                        ModelState.AddModelError("ConfigValue5", "Value contains invalid characters (Enter 0-9)");

                        return View(model);
                    }
                }

                var config = unitOfWork.Repository<Config>().Queryable().SingleOrDefault(c => c.Id == model.ConfigId);

                if (config == null)
                {
                    ModelState.AddModelError("ConfigType", "Unable to update the configuration. The configuration could not be found in the data store.");

                    return View(model);
                }

                try
                {
                    string encodedValue = "";
                    switch (config.ConfigType)
                    {
                        case ConfigType.E2BVersion:
                            encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ConfigValue1, false);;
                            break;
                        case ConfigType.WebServiceSubscriberList:
                            encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ConfigValue2, false);;
                            break;
                        case ConfigType.AssessmentScale:
                            encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ConfigValue3, false);;
                            break;
                        case ConfigType.ReportInstanceNewAlertCount:
                            encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ConfigValue4, false); ;
                            break;
                        case ConfigType.MedicationOnsetCheckPeriodWeeks:
                            encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ConfigValue5, false); ;
                            break;
                        default:
                            break;
                    }
                    config.ConfigValue = encodedValue;

                    unitOfWork.Repository<Config>().Update(config);
                    unitOfWork.Complete();

                    return Redirect("/Admin/ManageConfig.aspx");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to update the configuration: {0}", ex.Message));
                }
            }

            ViewBag.ConfigValue1Items = new[]
                {
                    new SelectListItem { Value = "E2B(R2) ICH Report", Text = "E2B(R2) ICH Report", Selected = true },
                    new SelectListItem { Value = "E2B(R3) ICH Report", Text = "E2B(R3) ICH Report" }
                };

            ViewBag.ConfigValue3Items = new[]
                {
                    new SelectListItem { Value = "Both Scales", Text = "Both Scales", Selected = true },
                    new SelectListItem { Value = "WHO Scale", Text = "WHO Scale" },
                    new SelectListItem { Value = "Naranjo Scale", Text = "Naranjo Scale" }
                };

            return View(model);
        }

    }
}