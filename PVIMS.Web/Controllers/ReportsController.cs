using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Core.Utilities;
using PVIMS.Core.ValueTypes;

using PVIMS.Web.ActionFilters;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class ReportsController : BaseController
    {
        private static string CurrentMenuItem = "ReportList";

        private readonly IUnitOfWorkInt _unitOfWork;

        public ReportsController(IUnitOfWorkInt unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;
            ViewBag.PopUpMessage = PreparePopUpMessage();

            var reports = _unitOfWork.Repository<MetaReport>().Queryable()
                .OrderBy(mr => mr.ReportName).ToList().Select(mr => new MetaReportItem
            {
                    Definition = mr.ReportDefinition,
                    GUID = mr.metareport_guid.ToString(),
                    ReportName = mr.ReportName,
                    MetaReportId = mr.Id,
                    Status = mr.ReportStatus.ToString()
            });

            ViewData.Model = reports;

            return View();
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult CustomiseReport(int metaReportId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaReport = _unitOfWork.Repository<MetaReport>()
                .Queryable()
                .SingleOrDefault(r => r.Id == metaReportId);

            // Prepare model
            var model = new CustomiseReportModel
            {
                MetaReportId = metaReportId,
                ReportName = metaReport != null ? metaReport.ReportName : string.Empty,
                ReportDefinition = metaReport != null ? metaReport.ReportDefinition : string.Empty,
                ReportStatus = metaReport != null ? Convert.ToInt32(metaReport.ReportStatus) : 2,
                AllowDeletion = System.Web.HttpContext.Current.User.IsInRole(Constants.Role.ReporterAdministrator)
            };

            if(metaReport != null)
            {
                ExtractMetaDataForCustom(metaReport, model);
            }

            ViewBag.CoreEntities = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "", Selected = true } };
            ViewBag.CoreEntities.AddRange(_unitOfWork.Repository<MetaTable>().Queryable()
                .OrderBy(mt => mt.FriendlyName).Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.FriendlyName }).ToList());

            ViewBag.ReportTypes = new[]
                {
                    new SelectListItem { Value = "0", Text = "" },
                    new SelectListItem { Value = "1", Text = "List", Selected = true },
                    new SelectListItem { Value = "2", Text = "Summary" }
                };

            ViewBag.ReportStatuses = new[]
                {
                    new SelectListItem { Value = "0", Text = "" },
                    new SelectListItem { Value = "2", Text = "Unpublished", Selected = true },
                    new SelectListItem { Value = "1", Text = "Published" }
                };

            return View(model);
        }

        [HttpPost]
        public ActionResult CustomiseReport(CustomiseReportModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaReport = _unitOfWork.Repository<MetaReport>()
                .Queryable()
                .SingleOrDefault(r => r.Id == model.MetaReportId);

            if (metaReport == null)
            {
                metaReport = new MetaReport();
            }

            ValidateDefinition(model);

            if (ModelState.IsValid)
            {
                SaveDefinition(model, metaReport);

                HttpCookie cookie = new HttpCookie("PopUpMessage");
                cookie.Value = "Report saved successfully";
                Response.Cookies.Add(cookie);

                return Redirect("/Reports/Index");
            }

            ViewBag.CoreEntities = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "", Selected = true } };
            ViewBag.CoreEntities.AddRange(_unitOfWork.Repository<MetaTable>().Queryable()
                .OrderBy(mt => mt.FriendlyName).Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.FriendlyName }).ToList());

            ViewBag.ReportTypes = new[]
                {
                    new SelectListItem { Value = "0", Text = "" },
                    new SelectListItem { Value = "1", Text = "List", Selected = true },
                    new SelectListItem { Value = "2", Text = "Summary" }
                };

            ViewBag.ReportStatuses = new[]
                {
                    new SelectListItem { Value = "0", Text = "" },
                    new SelectListItem { Value = "2", Text = "Unpublished", Selected = true },
                    new SelectListItem { Value = "1", Text = "Published" }
                };

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeleteMetaReport(int metaReportId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaReport = _unitOfWork.Repository<MetaReport>()
                .Queryable()
                .SingleOrDefault(r => r.Id == metaReportId);

            if (metaReport == null)
            {
                ViewBag.Entity = "Meta Report";
                return View("NotFound");
            }

            // Prepare model
            var model = new MetaReportDeleteModel
            {
                MetaReportId = metaReport.Id,
                ReportDefinition = Server.HtmlDecode(metaReport.ReportDefinition),
                ReportName = Server.HtmlDecode(metaReport.ReportName)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteMetaReport(MetaReportDeleteModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaReport = _unitOfWork.Repository<MetaReport>()
                .Queryable()
                .SingleOrDefault(r => r.Id == model.MetaReportId);

            if (metaReport != null)
            {
                if (ModelState.IsValid)
                {
                    if (metaReport.IsSystem)
                    {
                        ModelState.AddModelError("ReportName", "SYSTEM REPORT. Unable to delete.");
                    }

                    if (ModelState.IsValid)
                    {
                        _unitOfWork.Repository<MetaReport>().Delete(metaReport);
                        _unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Report deleted successfully";
                        Response.Cookies.Add(cookie);

                        return Redirect("/Reports/Index");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ReportAttributeItem(long metaReportId, Models.ViewType viewType)
        {
            ViewBag.MenuItem = CurrentMenuItem;
            ViewBag.PopUpMessage = PreparePopUpMessage();

            var metaReport = _unitOfWork.Repository<MetaReport>()
                .Queryable()
                .SingleOrDefault(r => r.Id == metaReportId);

            var model = new MetaReportAttributeModel()
            {
                MetaReportId = metaReport.Id,
                ReportDefinition = metaReport.ReportDefinition,
                ReportName = metaReport.ReportName,
                ViewType = viewType
            };

            if (metaReport != null)
            {
                ExtractMetaDataForAttribute(metaReport, model);

                IOrderedQueryable<MetaColumn> metaColumns = _unitOfWork.Repository<MetaColumn>().Queryable()
                    .Where(mc => mc.Table.Id == model.CoreEntity)
                    .OrderBy(mc => mc.ColumnName);
                List<SelectListItem> listMetaColumns = new List<SelectListItem>();

                // Add new item to relevant list
                switch (model.ViewType)
                {
                    case Models.ViewType.List:
                        foreach (MetaColumn metaColumn in metaColumns)
                        {
                            // ensure not selected
                            if (!model.ListItems.Any(li => li.MetaColumnId == metaColumn.Id))
                            {
                                listMetaColumns.Add(new SelectListItem() { Text = metaColumn.ColumnName, Value = metaColumn.Id.ToString() });
                            }
                        }
                        if (listMetaColumns.Count == 0)
                        {
                            listMetaColumns.Add(new SelectListItem() { Text = "-- ALL COLUMNS ASSIGNED --", Value = "0" });
                        }

                        break;
                    case Models.ViewType.Summary:
                        foreach (MetaColumn metaColumn in metaColumns)
                        {
                            // ensure not selected
                            if (!model.StratifyItems.Any(li => li.MetaColumnId == metaColumn.Id))
                            {
                                listMetaColumns.Add(new SelectListItem() { Text = metaColumn.ColumnName, Value = metaColumn.Id.ToString() });
                            }
                        }
                        if (listMetaColumns.Count == 0)
                        {
                            listMetaColumns.Add(new SelectListItem() { Text = "-- ALL COLUMNS ASSIGNED --", Value = "0" });
                        }

                        break;
                    case Models.ViewType.Filter:
                        if(metaColumns.Count() > 0)
                        {
                            listMetaColumns.Add(new SelectListItem() { Text = "-- Please select a column --", Value = "0" });
                            foreach (MetaColumn metaColumn in metaColumns)
                            {
                                // ensure not selected
                                if (!model.FilterItems.Any(li => li.MetaColumnId == metaColumn.Id))
                                {
                                    listMetaColumns.Add(new SelectListItem() { Text = metaColumn.ColumnName, Value = metaColumn.Id.ToString() });
                                }
                            }
                        }
                        if (listMetaColumns.Count == 0)
                        {
                            listMetaColumns.Add(new SelectListItem() { Text = "-- ALL COLUMNS ASSIGNED --", Value = "0" });
                        }

                        break;
                    default:
                        break;
                }

                ViewBag.MetaColumns = listMetaColumns;
            }

            ViewBag.Relationships = new[]
                {
                    new SelectListItem { Value = "And", Text = "And", Selected = true },
                    new SelectListItem { Value = "Or", Text = "Or" }
                };

            ViewBag.Operators = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Please select an operator --", Selected = true }
                };

            ViewData.Model = model;

            return View();
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult ReportAttributeItem(MetaReportAttributeModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            switch (model.ViewType)
            {
                case Models.ViewType.List:
                    if (!String.IsNullOrWhiteSpace(model.DisplayForList))
                    {
                        if (Regex.Matches(model.DisplayForList, @"[a-zA-Z ']").Count < model.DisplayForList.Length)
                        {
                            ModelState.AddModelError("DisplayForList", "Display contains invalid characters(Enter A-Z, a-z, space).");
                        }
                    }

                    break;

                case Models.ViewType.Summary:
                    if (!String.IsNullOrWhiteSpace(model.DisplayForSummary))
                    {
                        if (Regex.Matches(model.DisplayForSummary, @"[a-zA-Z ']").Count < model.ReportName.Length)
                        {
                            ModelState.AddModelError("DisplayForSummary", "Display contains invalid characters(Enter A-Z, a-z, space).");
                        }
                    }

                    break;

                case Models.ViewType.Filter:
                    if (model.MetaColumnForFilterId == 0)
                    {
                        ModelState.AddModelError("MetaColumnForFilterId", "Column must be selected.");
                    }
                    if (String.IsNullOrWhiteSpace(model.Operator))
                    {
                        ModelState.AddModelError("Operator", "Operator must be selected.");
                    }
                    if (String.IsNullOrWhiteSpace(model.Relation))
                    {
                        ModelState.AddModelError("Relation", "Relation must be selected.");
                    }

                    break;

                default:
                    break;
            }

            if (ModelState.IsValid)
            {
                var metaReport = _unitOfWork.Repository<MetaReport>()
                    .Queryable()
                    .SingleOrDefault(r => r.Id == model.MetaReportId);

                ExtractMetaDataForAttribute(metaReport, model);

                // Add new item to relevant list
                switch (model.ViewType)
                {
                    case Models.ViewType.List:
                        if (model.MetaColumnForListId > 0)
                        {
                            MetaReportAttributeModel.ListItem list = new MetaReportAttributeModel.ListItem();
                            list.MetaColumnId = model.MetaColumnForListId;
                            list.AttributeName = _unitOfWork.Repository<MetaColumn>().Queryable().Single(mc => mc.Id == model.MetaColumnForListId).ColumnName;
                            list.DisplayName = String.IsNullOrWhiteSpace(model.DisplayForList) ? list.AttributeName : model.DisplayForList;
                            model.ListItems.Add(list);
                        }

                        // Now save final definition
                        SaveDefinitionForAttribute(model, metaReport);

                        break;
                    case Models.ViewType.Summary:
                        if (model.MetaColumnForSummaryId > 0)
                        {
                            MetaReportAttributeModel.ListItem strat = new MetaReportAttributeModel.ListItem();
                            strat.MetaColumnId = model.MetaColumnForSummaryId;
                            strat.AttributeName = _unitOfWork.Repository<MetaColumn>().Queryable().Single(mc => mc.Id == model.MetaColumnForSummaryId).ColumnName;
                            strat.DisplayName = String.IsNullOrWhiteSpace(model.DisplayForSummary) ? strat.AttributeName : model.DisplayForSummary;
                            model.StratifyItems.Add(strat);
                        }

                        // Now save final definition
                        SaveDefinitionForAttribute(model, metaReport);

                        break;
                    case Models.ViewType.Filter:
                        if (model.MetaColumnForFilterId > 0)
                        {
                            MetaReportAttributeModel.FilterItem filter = new MetaReportAttributeModel.FilterItem();
                            filter.MetaColumnId = model.MetaColumnForFilterId;
                            filter.AttributeName = _unitOfWork.Repository<MetaColumn>().Queryable().Single(mc => mc.Id == model.MetaColumnForFilterId).ColumnName;
                            filter.Operator = model.Operator.ToString();
                            filter.Relation = model.Relation.ToString();
                            model.FilterItems.Add(filter);
                        }

                        // Now save final definition
                        SaveDefinitionForAttribute(model, metaReport);

                        break;
                    default:
                        break;
                }

                HttpCookie cookie = new HttpCookie("PopUpMessage");
                cookie.Value = "Column added successfully";
                Response.Cookies.Add(cookie);

                return Redirect("/Reports/ReportAttributeItem?metaReportId=" + model.MetaReportId.ToString() + "&viewType=" + model.ViewType.ToString());
            }

            IOrderedQueryable<MetaColumn> metaColumns = _unitOfWork.Repository<MetaColumn>().Queryable()
                .Where(mc => mc.Table.Id == model.CoreEntity)
                .OrderBy(mc => mc.ColumnName);
            List<SelectListItem> listMetaColumns = new List<SelectListItem>();

            // Add new item to relevant list
            switch (model.ViewType)
            {
                case Models.ViewType.List:
                    foreach (MetaColumn metaColumn in metaColumns)
                    {
                        // ensure not selected
                        if (!model.ListItems.Any(li => li.MetaColumnId == metaColumn.Id))
                        {
                            listMetaColumns.Add(new SelectListItem() { Text = metaColumn.ColumnName, Value = metaColumn.Id.ToString() });
                        }
                    }
                    if (listMetaColumns.Count == 0)
                    {
                        listMetaColumns.Add(new SelectListItem() { Text = "-- ALL COLUMNS ASSIGNED --", Value = "0" });
                    }

                    break;
                case Models.ViewType.Summary:
                    foreach (MetaColumn metaColumn in metaColumns)
                    {
                        // ensure not selected
                        if (!model.StratifyItems.Any(li => li.MetaColumnId == metaColumn.Id))
                        {
                            listMetaColumns.Add(new SelectListItem() { Text = metaColumn.ColumnName, Value = metaColumn.Id.ToString() });
                        }
                    }
                    if (listMetaColumns.Count == 0)
                    {
                        listMetaColumns.Add(new SelectListItem() { Text = "-- ALL COLUMNS ASSIGNED --", Value = "0" });
                    }

                    break;
                case Models.ViewType.Filter:
                    if (metaColumns.Count() > 0)
                    {
                        listMetaColumns.Add(new SelectListItem() { Text = "-- Please select a column --", Value = "0" });
                        foreach (MetaColumn metaColumn in metaColumns)
                        {
                            // ensure not selected
                            if (!model.FilterItems.Any(li => li.MetaColumnId == metaColumn.Id))
                            {
                                listMetaColumns.Add(new SelectListItem() { Text = metaColumn.ColumnName, Value = metaColumn.Id.ToString() });
                            }
                        }
                    }
                    if (listMetaColumns.Count == 0)
                    {
                        listMetaColumns.Add(new SelectListItem() { Text = "-- ALL COLUMNS ASSIGNED --", Value = "0" });
                    }

                    break;
                default:
                    break;
            }

            ViewBag.MetaColumns = listMetaColumns;

            ViewBag.Relationships = new[]
                {
                    new SelectListItem { Value = "And", Text = "And", Selected = true },
                    new SelectListItem { Value = "Or", Text = "Or" }
                };

            ViewBag.Operators = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Please select an operator --", Selected = true }
                };

            return View(model);
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult RemoveMetaColumn(long metaReportId, long metaColumnId, Models.ViewType viewType)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var metaReport = _unitOfWork.Repository<MetaReport>()
                .Queryable()
                .SingleOrDefault(r => r.Id == metaReportId);

            var model = new MetaReportAttributeModel()
            {
                MetaReportId = metaReport.Id,
                ReportDefinition = metaReport.ReportDefinition,
                ReportName = metaReport.ReportName,
                ViewType = viewType
            };

            ExtractMetaDataForAttribute(metaReport, model);

            XmlDocument meta = new XmlDocument();
            meta.LoadXml(metaReport.MetaDefinition);

            // Get node to be removed
            XmlNode rootNode = meta.SelectSingleNode("//MetaReport");
            XmlNode typeNode = null;
            XmlNode removeNode = null;
            switch (viewType)
            {
                case Models.ViewType.List:
                    typeNode = rootNode.SelectSingleNode("//List");
                    removeNode = typeNode.SelectSingleNode(String.Format("ListItem[@MetaColumnId='{0}']", metaColumnId));
                    typeNode.RemoveChild(removeNode);

                    break;
                case Models.ViewType.Summary:
                    typeNode = rootNode.SelectSingleNode("//Summary");
                    removeNode = typeNode.SelectSingleNode(String.Format("SummaryItem[@MetaColumnId='{0}']", metaColumnId));
                    typeNode.RemoveChild(removeNode);

                    break;
                case Models.ViewType.Filter:
                    typeNode = rootNode.SelectSingleNode("//Filter");
                    removeNode = typeNode.SelectSingleNode(String.Format("FilterItem[@MetaColumnId='{0}']", metaColumnId));
                    typeNode.RemoveChild(removeNode);

                    break;
                default:
                    break;
            }

            metaReport.MetaDefinition = meta.InnerXml;

            string sql = string.Empty;
            if (model.ReportType == 2)
            {
                sql = PrepareSummaryQueryForPublication(model);
            }
            else
            {
                sql = PrepareListQueryForPublication(model);
            }
            metaReport.SQLDefinition = sql;

            _unitOfWork.Repository<MetaReport>().Update(metaReport);
            _unitOfWork.Complete();

            HttpCookie cookie = new HttpCookie("PopUpMessage");
            cookie.Value = "Column removed successfully";
            Response.Cookies.Add(cookie);

            return Redirect("/Reports/ReportAttributeItem?metaReportId=" + metaReportId.ToString() + "&viewType=" + viewType.ToString());
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult GetOperatorList(long metaColumnId)
        {
            // Get the attribute and check what type of element it is
            var metaColumn = _unitOfWork.Repository<MetaColumn>()
                .Queryable()
                .Single(mc => mc.Id == metaColumnId);

            List<SelectListItem> operatorList = new List<SelectListItem>();

            switch ((MetaColumnTypes)metaColumn.ColumnType.Id)
            {
                case MetaColumnTypes.tbigint:
                case MetaColumnTypes.tint:
                case MetaColumnTypes.tdecimal:
                case MetaColumnTypes.tsmallint:
                case MetaColumnTypes.ttinyint:
                    operatorList.Add(new SelectListItem() { Text = "Equals", Value = "=" });
                    operatorList.Add(new SelectListItem() { Text = "Not Equals", Value = "<>" });
                    operatorList.Add(new SelectListItem() { Text = "Greater Than", Value = ">" });
                    operatorList.Add(new SelectListItem() { Text = "Less Than", Value = "<" });
                    operatorList.Add(new SelectListItem() { Text = "GreaterEqual Than", Value = ">=" });
                    operatorList.Add(new SelectListItem() { Text = "LessEqual Than", Value = "<=" });
                    operatorList.Add(new SelectListItem() { Text = "Between", Value = "between" });

                    break;

                case MetaColumnTypes.tchar:
                case MetaColumnTypes.tnchar:
                case MetaColumnTypes.tnvarchar:
                case MetaColumnTypes.tvarchar:
                    if (String.IsNullOrEmpty(metaColumn.Range))
                    {
                        operatorList.Add(new SelectListItem() { Text = "Equals", Value = "=" });
                        operatorList.Add(new SelectListItem() { Text = "Not Equals", Value = "<>" });
                    }
                    else
                    {
                        operatorList.Add(new SelectListItem() { Text = "Equals", Value = "=" });
                        operatorList.Add(new SelectListItem() { Text = "Not Equals", Value = "<>" });
                        operatorList.Add(new SelectListItem() { Text = "In", Value = "in" });
                    }

                    break;

                case MetaColumnTypes.tdate:
                case MetaColumnTypes.tdatetime:
                    operatorList.Add(new SelectListItem() { Text = "Equals", Value = "=" });
                    operatorList.Add(new SelectListItem() { Text = "Not Equals", Value = "<>" });
                    operatorList.Add(new SelectListItem() { Text = "Greater Than", Value = ">" });
                    operatorList.Add(new SelectListItem() { Text = "Less Than", Value = "<" });
                    operatorList.Add(new SelectListItem() { Text = "GreaterEqual Than", Value = ">=" });
                    operatorList.Add(new SelectListItem() { Text = "LessEqual Than", Value = "<=" });

                    operatorList.Add(new SelectListItem() { Text = "Between", Value = "between" });
                    break;

                default:
                    break;
            }

            return Json(operatorList.ToList(), JsonRequestBehavior.AllowGet);
        }

        private void ExtractMetaDataForCustom(MetaReport metaReport, CustomiseReportModel model)
        {
            int tempi;

            XmlDocument meta = new XmlDocument();
            meta.LoadXml(metaReport.MetaDefinition);

            // Unpack structures
            XmlNode rootNode = meta.SelectSingleNode("//MetaReport");
            XmlAttribute typeAttr = rootNode.Attributes["Type"];
            XmlAttribute entityAttr = rootNode.Attributes["CoreEntity"];

            model.CoreEntity = entityAttr != null ? int.TryParse(entityAttr.Value, out tempi) ? Convert.ToInt32(entityAttr.Value) : 0 : 0;
            model.ReportType = typeAttr != null ? int.TryParse(typeAttr.Value, out tempi) ? Convert.ToInt32(typeAttr.Value) : 0 : 0;
            model.ViewType = model.ReportType == 1 ? Models.ViewType.List : Models.ViewType.Summary ;
        }

        private void ExtractMetaDataForAttribute(MetaReport metaReport, MetaReportAttributeModel model)
        {
            int tempi;

            XmlDocument meta = new XmlDocument();
            meta.LoadXml(metaReport.MetaDefinition);

            // Unpack structures
            XmlNode rootNode = meta.SelectSingleNode("//MetaReport");
            XmlAttribute typeAttr = rootNode.Attributes["Type"];
            XmlAttribute entityAttr = rootNode.Attributes["CoreEntity"];

            var coreId = entityAttr != null ? int.TryParse(entityAttr.Value, out tempi) ? Convert.ToInt32(entityAttr.Value) : 0 : 0;
            model.CoreEntity = coreId;
            model.CoreEntityDisplay = _unitOfWork.Repository<MetaTable>().Queryable().Single(mt => mt.Id == coreId).TableName;
            var typeId = typeAttr != null ? int.TryParse(typeAttr.Value, out tempi) ? Convert.ToInt32(typeAttr.Value) : 0 : 0;
            model.ReportType = typeId;
            model.ReportTypeDisplay = typeId == 1 ? "List" : "Summary";

            XmlNode mainNode;

            // List or summary
            if(typeId == 1)
            {
                mainNode = rootNode.SelectSingleNode("//List");
                if (mainNode != null)
                {
                    foreach (XmlNode subNode in mainNode.ChildNodes)
                    {
                        MetaReportAttributeModel.ListItem list = new MetaReportAttributeModel.ListItem();
                        list.MetaColumnId = Convert.ToInt32(subNode.Attributes["MetaColumnId"].Value);
                        list.AttributeName = subNode.Attributes.GetNamedItem("AttributeName").Value;
                        list.DisplayName = subNode.Attributes.GetNamedItem("DisplayName").Value;
                        model.ListItems.Add(list);
                    }
                }
            }
            else
            {
                mainNode = rootNode.SelectSingleNode("//Summary");
                if (mainNode != null)
                {
                    foreach (XmlNode subNode in mainNode.ChildNodes)
                    {
                        MetaReportAttributeModel.ListItem strat = new MetaReportAttributeModel.ListItem();
                        strat.MetaColumnId = Convert.ToInt32(subNode.Attributes["MetaColumnId"].Value);
                        strat.AttributeName = subNode.Attributes.GetNamedItem("AttributeName").Value;
                        strat.DisplayName = subNode.Attributes.GetNamedItem("DisplayName").Value;
                        model.StratifyItems.Add(strat);
                    }
                }
            }

            // filter
            mainNode = rootNode.SelectSingleNode("//Filter");
            if (mainNode != null)
            {
                foreach (XmlNode subNode in mainNode.ChildNodes)
                {
                    MetaReportAttributeModel.FilterItem filter = new MetaReportAttributeModel.FilterItem();
                    filter.MetaColumnId = Convert.ToInt32(subNode.Attributes["MetaColumnId"].Value);
                    filter.AttributeName = subNode.Attributes.GetNamedItem("AttributeName").Value;
                    filter.Operator = subNode.Attributes.GetNamedItem("Operator").Value;
                    filter.Relation = subNode.Attributes.GetNamedItem("Relation").Value;
                    model.FilterItems.Add(filter);
                }
            }
        }

        private void ValidateDefinition(CustomiseReportModel model)
        {
            if (!String.IsNullOrWhiteSpace(model.ReportName))
            {
                if (Regex.Matches(model.ReportName, @"[a-zA-Z ']").Count < model.ReportName.Length)
                {
                    ModelState.AddModelError("ReportName", "Report Name contains invalid characters(Enter A-Z, a-z, space).");
                }
            }
            if(!String.IsNullOrWhiteSpace(model.ReportDefinition))
            {
                if (Regex.Matches(model.ReportDefinition, @"[a-zA-Z'!,]").Count < model.ReportDefinition.Length)
                {
                    ModelState.AddModelError("ReportName", "Definition contains invalid characters (Enter A-Z, a-z, '!,).");
                }
            }
            if (model.ReportType == 0)
            {
                ModelState.AddModelError("ReportType", "Please select a valid report type.");
            }
            if (model.CoreEntity == 0)
            {
                ModelState.AddModelError("CoreEntity", "Please select a valid core entity.");
            }
            if (model.ReportStatus == 0)
            {
                ModelState.AddModelError("ReportStatus", "Please select a valid report status.");
            }

        }

        private void SaveDefinition(CustomiseReportModel model, MetaReport metaReport)
        {
            try
            {
                // Prepare XML
                XmlDocument meta = new XmlDocument();

                var ns = ""; // urn:pvims-org:v3

                XmlNode rootNode = null;
                XmlAttribute attrib;

                XmlDeclaration xmlDeclaration = meta.CreateXmlDeclaration("1.0", "UTF-8", null);
                meta.AppendChild(xmlDeclaration);

                rootNode = meta.CreateElement("MetaReport", ns);

                attrib = meta.CreateAttribute("Type");
                attrib.InnerText = model.ReportType.ToString();
                rootNode.Attributes.Append(attrib);

                attrib = meta.CreateAttribute("CoreEntity");
                attrib.InnerText = model.CoreEntity.ToString();
                rootNode.Attributes.Append(attrib);

                meta.AppendChild(rootNode);

                var encodedName = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ReportName, false);
                var encodedDefinition = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ReportDefinition, false);

                metaReport.MetaDefinition = meta.InnerXml;
                metaReport.ReportDefinition = encodedDefinition;
                metaReport.ReportName = encodedName;
                metaReport.ReportStatus = (MetaReportStatus)model.ReportStatus;

                if (model.MetaReportId == 0)
                {
                    _unitOfWork.Repository<MetaReport>().Save(metaReport);
                }
                else
                {
                    _unitOfWork.Repository<MetaReport>().Update(metaReport);
                }
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ReportName", String.Format("<li>ERROR: {0}...</li>", ex.Message));
            }
        }

        private void SaveDefinitionForAttribute(MetaReportAttributeModel model, MetaReport metaReport)
        {
            try
            {
                // Prepare XML
                XmlDocument meta = new XmlDocument();

                var ns = ""; // urn:pvims-org:v3

                XmlNode rootNode = null;
                XmlNode mainNode = null;
                XmlNode subNode = null;
                XmlAttribute attrib;

                XmlDeclaration xmlDeclaration = meta.CreateXmlDeclaration("1.0", "UTF-8", null);
                meta.AppendChild(xmlDeclaration);

                rootNode = meta.CreateElement("MetaReport", ns);

                attrib = meta.CreateAttribute("Type");
                attrib.InnerText = model.ReportType.ToString();
                rootNode.Attributes.Append(attrib);

                attrib = meta.CreateAttribute("CoreEntity");
                attrib.InnerText = model.CoreEntity.ToString();
                rootNode.Attributes.Append(attrib);

                if (model.ReportType == 2)
                {
                    mainNode = meta.CreateElement("Summary", ns);

                    foreach (MetaReportAttributeModel.ListItem strat in model.StratifyItems)
                    {
                        subNode = meta.CreateElement("SummaryItem", ns);
                        attrib = meta.CreateAttribute("MetaColumnId");
                        attrib.InnerText = strat.MetaColumnId.ToString();
                        subNode.Attributes.Append(attrib);

                        attrib = meta.CreateAttribute("DisplayName");
                        attrib.InnerText = strat.DisplayName;
                        subNode.Attributes.Append(attrib);

                        attrib = meta.CreateAttribute("AttributeName");
                        attrib.InnerText = strat.AttributeName;
                        subNode.Attributes.Append(attrib);

                        mainNode.AppendChild(subNode);
                    }

                    rootNode.AppendChild(mainNode);
                }
                else
                {
                    mainNode = meta.CreateElement("List", ns);

                    foreach (MetaReportAttributeModel.ListItem list in model.ListItems)
                    {
                        subNode = meta.CreateElement("ListItem", ns);
                        attrib = meta.CreateAttribute("MetaColumnId");
                        attrib.InnerText = list.MetaColumnId.ToString();
                        subNode.Attributes.Append(attrib);

                        attrib = meta.CreateAttribute("DisplayName");
                        attrib.InnerText = list.DisplayName;
                        subNode.Attributes.Append(attrib);

                        attrib = meta.CreateAttribute("AttributeName");
                        attrib.InnerText = list.AttributeName;
                        subNode.Attributes.Append(attrib);

                        mainNode.AppendChild(subNode);
                    }

                    rootNode.AppendChild(mainNode);
                }

                mainNode = meta.CreateElement("Filter", ns);

                foreach (MetaReportAttributeModel.FilterItem filter in model.FilterItems)
                {
                    subNode = meta.CreateElement("FilterItem", ns);
                    attrib = meta.CreateAttribute("MetaColumnId");
                    attrib.InnerText = filter.MetaColumnId.ToString();
                    subNode.Attributes.Append(attrib);

                    attrib = meta.CreateAttribute("AttributeName");
                    attrib.InnerText = filter.AttributeName;
                    subNode.Attributes.Append(attrib);

                    attrib = meta.CreateAttribute("Operator");
                    attrib.InnerText = filter.Operator;
                    subNode.Attributes.Append(attrib);

                    attrib = meta.CreateAttribute("Relation");
                    attrib.InnerText = filter.Relation;
                    subNode.Attributes.Append(attrib);

                    mainNode.AppendChild(subNode);
                }

                rootNode.AppendChild(mainNode);
                meta.AppendChild(rootNode);

                metaReport.MetaDefinition = meta.InnerXml;

                string sql = string.Empty;
                if (model.ReportType == 2)
                {
                    sql = PrepareSummaryQueryForPublication(model);
                }
                else
                {
                    sql = PrepareListQueryForPublication(model);
                }
                metaReport.SQLDefinition = sql;

                _unitOfWork.Repository<MetaReport>().Update(metaReport);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ReportName", String.Format("<li>ERROR: {0}...</li>", ex.Message));
            }
        }

        private string PrepareListQueryForPublication(MetaReportAttributeModel model)
        {
            string sql = "";

            string fcriteria = ""; // from
            string jcriteria = ""; // joins
            string scriteria = ""; // selects
            string ocriteria = ""; // orders
            string wcriteria = ""; // wheres

            var metaTable = _unitOfWork.Repository<MetaTable>()
                .Queryable()
                .SingleOrDefault(mt => mt.Id == model.CoreEntity);

            // FROM
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Core:
                    fcriteria = "[Meta" + metaTable.TableName + "] P";
                    break;

                case MetaTableTypes.CoreChild:
                    fcriteria = "[Meta" + metaTable.TableName + "] C";
                    break;

                case MetaTableTypes.Child:
                    fcriteria = "[Meta" + metaTable.TableName + "] P";
                    break;

                case MetaTableTypes.History:
                    fcriteria = "[Meta" + metaTable.TableName + "] C";
                    break;

                default:
                    break;
            }

            // JOINS
            MetaDependency metaDependency;
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Child:
                case MetaTableTypes.Core:
                    // do nothing
                    break;

                case MetaTableTypes.History:
                case MetaTableTypes.CoreChild:
                    // get parent
                    metaDependency = _unitOfWork.Repository<MetaDependency>()
                        .Queryable()
                        .SingleOrDefault(md => md.ReferenceTable.Id == model.CoreEntity);

                    jcriteria += String.Format(" LEFT JOIN [Meta{0}] P ON P.{1} = C.{2} ", metaDependency.ParentTable.TableName, metaDependency.ParentColumnName, metaDependency.ReferenceColumnName);

                    break;
            }

            // FIELDS
            var fc = 0;
            foreach (MetaReportAttributeModel.ListItem list in model.ListItems)
            {
                fc += 1;

                scriteria += "cast(" + list.AttributeName + " as varchar)" + " as 'Col" + fc.ToString() + "', ";
                ocriteria += list.AttributeName + ", ";
            }
            scriteria = !String.IsNullOrWhiteSpace(scriteria) ? scriteria.Substring(0, scriteria.Length - 2) : "";
            ocriteria = !String.IsNullOrWhiteSpace(ocriteria) ? ocriteria.Substring(0, ocriteria.Length - 2) : "";

            // FILTERS
            fc = 0;
            foreach (MetaReportAttributeModel.FilterItem filter in model.FilterItems)
            {
                fc += 1;
                wcriteria += String.Format("{0} ({1} {2} %{3})", filter.Relation, filter.AttributeName, filter.Operator, fc.ToString());
            }

            sql = String.Format(@"
                select {0} 
                    from {3} 
                            {1}
                    where 1 = 1 {4}
                            ORDER BY {2}
                ", scriteria, jcriteria, ocriteria, fcriteria, wcriteria);

            return sql;
        }

        private string PrepareSummaryQueryForPublication(MetaReportAttributeModel model)
        {
            string sql = "";

            string fcriteria = ""; // from
            string jcriteria = ""; // joins
            string scriteria = ""; // selects
            string gcriteria = ""; // groups
            string ocriteria = ""; // orders
            string wcriteria = ""; // wheres

            var metaTable = _unitOfWork.Repository<MetaTable>()
                .Queryable()
                .SingleOrDefault(mt => mt.Id == model.CoreEntity);

            // FROM
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Core:
                    fcriteria = "[Meta" + metaTable.TableName + "] P";
                    break;

                case MetaTableTypes.CoreChild:
                    fcriteria = "[Meta" + metaTable.TableName + "] C";
                    break;

                case MetaTableTypes.Child:
                    fcriteria = "[Meta" + metaTable.TableName + "] P";
                    break;

                case MetaTableTypes.History:
                    fcriteria = "[Meta" + metaTable.TableName + "] C";
                    break;

                default:
                    break;
            }

            // JOINS
            MetaDependency metaDependency;
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Child:
                case MetaTableTypes.Core:
                    // do nothing
                    break;

                case MetaTableTypes.History:
                case MetaTableTypes.CoreChild:
                    // get parent
                    metaDependency = _unitOfWork.Repository<MetaDependency>()
                        .Queryable()
                        .SingleOrDefault(md => md.ReferenceTable.Id == model.CoreEntity);

                    jcriteria += String.Format(" LEFT JOIN [Meta{0}] P ON P.{1} = C.{2} ", metaDependency.ParentTable.TableName, metaDependency.ParentColumnName, metaDependency.ReferenceColumnName);

                    break;
            }

            // FIELDS
            var fc = 0;
            foreach (MetaReportAttributeModel.ListItem strat in model.StratifyItems)
            {
                fc += 1;

                scriteria += "cast(" + strat.AttributeName + " as varchar)" + " as 'Col" + fc.ToString() + "', ";
                gcriteria += strat.AttributeName + ", ";
                ocriteria += strat.AttributeName + ", ";
            }

            scriteria = !String.IsNullOrWhiteSpace(scriteria) ? scriteria.Substring(0, scriteria.Length - 2) : "";
            gcriteria = !String.IsNullOrWhiteSpace(gcriteria) ? gcriteria.Substring(0, ocriteria.Length - 2) : "";
            ocriteria = !String.IsNullOrWhiteSpace(ocriteria) ? ocriteria.Substring(0, ocriteria.Length - 2) : "";

            // FILTERS
            var filc = 0;
            foreach (MetaReportAttributeModel.FilterItem filter in model.FilterItems)
            {
                filc += 1;
                wcriteria += String.Format("{0} ({1} {2} %{3})", filter.Relation, filter.AttributeName, filter.Operator, filc.ToString());
            }

            sql = String.Format(@"
                select {0}, CAST(COUNT(*) as varchar) AS Col{6}
                    from {4} 
                            {1}
                    where 1 = 1 {5}
                            GROUP BY {2}
                            ORDER BY {3}
                ", scriteria, jcriteria, gcriteria, ocriteria, fcriteria, wcriteria, (fc + 1).ToString());

            return sql;
        }
    }

}