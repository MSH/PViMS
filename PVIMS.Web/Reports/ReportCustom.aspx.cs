using OfficeOpenXml;
using OfficeOpenXml.Style;

using System;
using System.Collections;
using System.Collections.Generic;

using System.Data;
using System.Data.Common;

using System.Drawing;
using System.IO;
using System.Linq;

using System.Text;
using System.Text.RegularExpressions;

using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Xml;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Entities.EF;

using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;

namespace PVIMS.Web
{
    public partial class ReportCustom : MainPageBase
    {
        private PVIMSDbContext _db = new PVIMSDbContext();

        private List<StratifyStructure> _strats = new List<StratifyStructure>();
        private List<FilterStructure> _filters = new List<FilterStructure>();
        private List<ListStructure> _lists = new List<ListStructure>();

        private StringBuilder _summary = new StringBuilder();

        private MetaReport _metaReport;
        private int _id = 0;

        enum ReportType
        {
            None,
            Summary,
            Listing
        }
        private ReportType _reportType;

        private string _sql;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["strats"] != null)
            {
                _strats = (List<StratifyStructure>)Session["strats"];
                RenderStrats();
            }
            if (Session["filters"] != null)
            {
                _filters = (List<FilterStructure>)Session["filters"];
                RenderFilters();
            }
            if (Session["lists"] != null)
            {
                _lists = (List<ListStructure>)Session["lists"];
                RenderLists();
            }

            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0)
                {
                    _metaReport = _db.MetaReports.Single(mr => mr.Id == _id);

                    RenderButtons();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("ReportList");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Custom Reports", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

            if (!Page.IsPostBack)
            {
                LoadEntities();
            }

            _reportType = ddlType.SelectedValue == "" ? ReportType.None : ddlType.SelectedValue == "Summary" ? ReportType.Summary : ReportType.Listing;
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (_db != null)
                _db.Dispose();
        }

        #region "Preparation"

        private void LoadEntities()
        {
            var items = _db.MetaTables
                .AsQueryable()
                .OrderBy(mt => mt.Id)
                .Select(s => new ListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.TableName
                })
                .ToArray();
            ddlEntity.Items.AddRange(items);
        }

        private void InitialiseEntityElements()
        {
            ddlElement.Items.Clear();
            ddlListElement.Items.Clear();
            ddlFilterElement.Items.Clear();
            ddlFilterOperator.Items.Clear();

            ListItem listItem = new ListItem() { Text = "-- Not Selected --", Value = "" };
            ddlElement.Items.Add(listItem);

            listItem = new ListItem() { Text = "-- Not Selected --", Value = "" };
            ddlListElement.Items.Add(listItem);

            ListItem elementListItem = new ListItem() { Text = "-- Not Selected --", Value = "" };
            ddlFilterElement.Items.Add(elementListItem);

            ListItem[] listItems;
            ListItem operatorListItem = new ListItem() { Text = "-- Not Selected --", Value = "" };
            ddlFilterOperator.Items.Add(operatorListItem);

            lblFilterNumericValue.Visible = true;
            lblFilterNumericRange.Visible = false;
            lblFilterTextValue.Visible = false;
            lblFilterDateValue.Visible = false;
            lblFilterDateRange.Visible = false;
            lblFilterSelectValue.Visible = false;
            lblFilterInValue.Visible = false;

            txtFilterNumericValue.Value = "0";

            MetaDependency metaDependency;

            var id = Convert.ToInt32(ddlEntity.SelectedValue);
            var metaTable = _db.MetaTables.Single(mt => mt.Id == id);

            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Child:
                case MetaTableTypes.Core:
                    var items = _db.MetaColumns
                        .AsQueryable()
                        .Where(mc => mc.Table.Id == metaTable.Id)
                        .OrderBy(mc => mc.Id)
                        .Select(s => new ListItem
                        {
                            Value = s.Id.ToString(),
                            Text = "P." + s.ColumnName,
                            Selected = false
                        })
                        .ToArray();

                    ddlElement.Items.AddRange(items);
                    ddlListElement.Items.AddRange(items);
                    ddlFilterElement.Items.AddRange(items);

                    break;

                case MetaTableTypes.CoreChild:
                    // get parent
                    metaDependency = _db.MetaDependencies.Single(md => md.ReferenceTable.Id == id);

                    var parentItems = _db.MetaColumns
                        .AsQueryable()
                        .Where(mc => mc.Table.Id == metaDependency.ParentTable.Id)
                        .OrderBy(mc => mc.Id)
                        .Select(s => new ListItem
                        {
                            Value = s.Id.ToString(),
                            Text = "P." + s.ColumnName,
                            Selected = false
                        })
                        .ToArray();

                    var childItems = _db.MetaColumns
                        .AsQueryable()
                        .Where(mc => mc.Table.Id == metaTable.Id)
                        .OrderBy(mc => mc.Id)
                        .Select(s => new ListItem
                        {
                            Value = s.Id.ToString(),
                            Text = "C." + s.ColumnName,
                            Selected = false
                        })
                        .ToArray();

                    ddlElement.Items.AddRange(parentItems);
                    ddlElement.Items.AddRange(childItems);

                    ddlListElement.Items.AddRange(parentItems);
                    ddlListElement.Items.AddRange(childItems);

                    ddlFilterElement.Items.AddRange(parentItems);
                    ddlFilterElement.Items.AddRange(childItems);

                    break;

                case MetaTableTypes.History:
                    // get parent
                    metaDependency = _db.MetaDependencies.Single(md => md.ReferenceTable.Id == id);

                    var historyItems = _db.MetaColumns
                        .AsQueryable()
                        .Where(mc => mc.Table.Id == metaDependency.ParentTable.Id)
                        .OrderBy(mc => mc.Id)
                        .Select(s => new ListItem
                        {
                            Value = s.Id.ToString(),
                            Text = "P." + s.ColumnName,
                            Selected = false
                        })
                        .ToArray();

                    ListItem elementlistItem = new ListItem()
                    {
                        Value = "99999",
                        Text = metaTable.TableName + "." + "CurrentValue",
                        Selected = false
                    };

                    ddlElement.Items.AddRange(historyItems);
                    ddlElement.Items.Add(elementlistItem);

                    ddlListElement.Items.AddRange(historyItems);
                    ddlListElement.Items.Add(elementlistItem);

                    ddlFilterElement.Items.AddRange(historyItems);
                    ddlFilterElement.Items.Add(elementlistItem);

                    break;

                default:
                     break;
            }
        }

        private void RenderStrats()
        {
            var index = 0;

            ImageButton ib;
            Label lbl;

            tblStratify.Rows.Clear();

            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell() { InnerHtml = "<b>Stratification</b>" };
            row.Cells.Add(cell);
            tblStratify.Rows.Add(row);

            foreach (StratifyStructure strat in _strats)
            {
                var display = String.Format("   [{0}] AS '{1}'", strat.AttributeName, strat.DisplayName);

                ib = new ImageButton() { ID = "ibDeleteStrat" + index.ToString(), AlternateText = "Delete", ImageAlign = System.Web.UI.WebControls.ImageAlign.Middle, ImageUrl = "/img/delete.jpg", CommandArgument = index.ToString(), CausesValidation = false };
                ib.Command += ImageButtonStrat_OnCommand;

                lbl = new Label() { Text = display };

                row = new HtmlTableRow();
                cell = new HtmlTableCell();

                cell.Controls.Add(ib);
                cell.Controls.Add(lbl);

                row.Cells.Add(cell);
                tblStratify.Rows.Add(row);

                index += 1;
            }
        }

        private void RenderLists()
        {
            var index = 0;

            ImageButton ib;
            Label lbl;

            tblList.Rows.Clear();

            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell() { InnerHtml = "<b>List</b>" };
            row.Cells.Add(cell);
            tblList.Rows.Add(row);

            foreach (ListStructure list in _lists)
            {
                var display = String.Format("   [{0}] AS '{1}'", list.AttributeName, list.DisplayName);

                ib = new ImageButton() { ID = "ibDeleteList" + index.ToString(), AlternateText = "Delete", ImageAlign = System.Web.UI.WebControls.ImageAlign.Middle, ImageUrl = "/img/delete.jpg", CommandArgument = index.ToString(), CausesValidation = false };
                ib.Command += ImageButtonList_OnCommand;

                lbl = new Label() { Text = display };

                row = new HtmlTableRow();
                cell = new HtmlTableCell();

                cell.Controls.Add(ib);
                cell.Controls.Add(lbl);

                row.Cells.Add(cell);
                tblList.Rows.Add(row);

                index += 1;
            }
        }

        private void RenderFilters()
        {
            var index = 0;

            ImageButton ib;
            Label lbl;

            tblFilter.Rows.Clear();

            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell() { InnerHtml = "<b>Filter</b>", Width = "100px" };
            row.Cells.Add(cell);
            tblFilter.Rows.Add(row);

            foreach (FilterStructure filter in _filters)
            {
                var display = String.Format("   {0} ([{1}] {2} {3})", filter.Relation, filter.AttributeName, filter.Operator, filter.FilterValue);

                ib = new ImageButton() { ID = "ibDeleteFilter" + index.ToString(), AlternateText = "Delete", ImageAlign = System.Web.UI.WebControls.ImageAlign.Middle, ImageUrl = "/img/delete.jpg", CommandArgument = index.ToString(), CausesValidation = false };
                ib.Command += ImageButtonFilter_OnCommand;

                lbl = new Label() { Text = display };

                row = new HtmlTableRow();
                cell = new HtmlTableCell();

                cell.Controls.Add(ib);
                cell.Controls.Add(lbl);

                row.Cells.Add(cell);
                tblFilter.Rows.Add(row);

                index += 1;
            }

            ddlFilterRelation.Items[1].Selected = false;
            if (_filters.Count > 0)
            {
                ddlFilterRelation.Items[0].Enabled = true;
                ddlFilterRelation.Items[1].Enabled = true;

                ddlFilterRelation.Items[0].Selected = true;
            }
            else
            {
                ddlFilterRelation.Items[0].Enabled = true;
                ddlFilterRelation.Items[1].Enabled = false;

                ddlFilterRelation.Items[0].Selected = true;
            }
        }

        private void RenderButtons()
        {
            HyperLink hyp;

            hyp = new HyperLink()
            {
                //NavigateUrl = "/FileDownload/DownloadReportMeta?Id=" + au.Id.ToString(),
                NavigateUrl = "/FileDownload/DownloadReportMeta?Id=0",
                CssClass = "btn btn-default",
                Text = "View Meta"
            };
            spnButtons.Controls.Add(hyp);

            hyp = new HyperLink()
            {
                //NavigateUrl = "/FileDownload/DownloadReportMeta?Id=" + au.Id.ToString(),
                NavigateUrl = "/FileDownload/DownloadReportSQL?Id=0",
                CssClass = "btn btn-default",
                Text = "View SQL"
            };
            spnButtons.Controls.Add(hyp);

            spnButtons.Visible = true;

        }

        private bool FilterContainsAttribute(MetaColumn attribute)
        {
            var res = _filters.SingleOrDefault(fs => fs.AttributeName == attribute.ColumnName);

            return (res != null);
        }

        private bool StratContainsAttribute(MetaColumn attribute)
        {
            var res = _strats.SingleOrDefault(fs => fs.AttributeName == attribute.ColumnName);

            return (res != null);
        }

        private bool ListContainsAttribute(MetaColumn attribute)
        {
            var res = _lists.SingleOrDefault(fs => fs.AttributeName == attribute.ColumnName);

            return (res != null);
        }

        #endregion

        #region "Execution"

        private void Search()
        {
            if (_reportType == ReportType.Summary && _strats.Count == 0) { return; }
            if (_reportType == ReportType.Listing && _lists.Count == 0) { return; }

            var summary = "<ul>";

            _sql = "";

            if (_reportType == ReportType.Summary) { PrepareSummaryQueryForExecution(); };
            if (_reportType == ReportType.Listing) { PrepareListQueryForExecution(); };

            if (_sql != "")
            {
                try
                {
                    DataSet ds = GetDatasetForSql(_sql);

                    // Bind to gridview
                    gvOutput.DataSource = ds.Tables[0];
                    gvOutput.DataBind();
                }
                catch (Exception ex)
                {
                    summary += String.Format("<li>ERROR: {0}...</li>", ex.Message);
                }
                finally
                {
                    summary += String.Format("<li>INFO: SUCCESSFUL. {0} row(s) returned...</li>", gvOutput.Rows.Count.ToString());
                }
                            
            }
            summary += "</ul>";
            spnSummary.InnerHtml = summary;
        }

        private void Publish()
        {
            _summary.Clear();
            _summary.Append("<ul>");

            if (ValidDefinition())
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
                    attrib.InnerText = ddlType.SelectedItem.Text;
                    rootNode.Attributes.Append(attrib);
                    attrib = meta.CreateAttribute("CoreEntity");
                    attrib.InnerText = ddlEntity.SelectedItem.Text;
                    rootNode.Attributes.Append(attrib);

                    if (ddlType.SelectedValue == "Summary")
                    {
                        mainNode = meta.CreateElement("Summary", ns);

                        foreach (StratifyStructure strat in _strats)
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

                        foreach (ListStructure list in _lists)
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

                    foreach (FilterStructure filter in _filters)
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

                        attrib = meta.CreateAttribute("Value");
                        attrib.InnerText = filter.FilterValue;
                        subNode.Attributes.Append(attrib);

                        mainNode.AppendChild(subNode);
                    }

                    rootNode.AppendChild(mainNode);
                    meta.AppendChild(rootNode);

                    if (_metaReport == null)
                    {
                        _metaReport = new MetaReport()
                        {
                            metareport_guid = System.Guid.NewGuid(),
                            Breadcrumb = "** NOT DEFINED **",
                            IsSystem = false
                        };
                        _db.MetaReports.Add(_metaReport);
                    }
                    _metaReport.MetaDefinition = meta.InnerXml;
                    _metaReport.ReportDefinition = txtDefinition.Value;
                    _metaReport.ReportName = txtReportName.Value;
                    _metaReport.SQLDefinition = _sql;

                    _db.SaveChanges();

                    _summary.Append("<li>INFO: SUCCESSFUL. Report published...</li>");
                }
                catch (Exception ex)
                {
                    _summary.AppendFormat("<li>ERROR: {0}...</li>", ex.Message);
                }
            }

            _summary.Append("</ul>");
            spnSummary.InnerHtml = _summary.ToString();
        }

        private void PrepareSummaryQueryForExecution()
        {
            string fcriteria = ""; // from
            string jcriteria = ""; // joins
            string scriteria = ""; // selects
            string gcriteria = ""; // groups
            string ocriteria = ""; // orders
            string wcriteria = ""; // wheres

            var id = Convert.ToInt32(ddlEntity.SelectedValue);
            var metaTable = _db.MetaTables.Single(mt => mt.Id == id);

            // FROM
            switch ((MetaTableTypes)metaTable.TableType.Id) 
            {
                case MetaTableTypes.Core:
                    fcriteria = "Meta" + metaTable.TableName + " P";
                    break;

                case MetaTableTypes.CoreChild:
                    fcriteria = "Meta" + metaTable.TableName + " C";
                    break;

                case MetaTableTypes.Child:
                    fcriteria = "Meta" + metaTable.TableName + " P";
                    break;

                case MetaTableTypes.History:
                    fcriteria = "Meta" + metaTable.TableName + " C";
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
                    metaDependency = _db.MetaDependencies.Single(md => md.ReferenceTable.Id == id);

                    jcriteria += String.Format(" LEFT JOIN Meta{0} P ON P.{1} = C.{2} ", metaDependency.ParentTable.TableName, metaDependency.ParentColumnName, metaDependency.ReferenceColumnName );

                    break;
            }

            // FIELDS
            foreach (StratifyStructure strat in _strats)
            {
                scriteria += strat.AttributeName + " as '" + strat.DisplayName + "', ";
                gcriteria += strat.AttributeName + ", ";
                ocriteria += strat.AttributeName + ", ";
            }
            scriteria = scriteria.Substring(0, scriteria.Length - 2);
            gcriteria = gcriteria.Substring(0, gcriteria.Length - 2);
            ocriteria = ocriteria.Substring(0, ocriteria.Length - 2);

            // FILTERS
            foreach (FilterStructure filter in _filters)
            {
                wcriteria += String.Format("{0} ({1} {2} {3})", filter.Relation, filter.AttributeName, filter.Operator, filter.FilterValue);
            }

            _sql = String.Format(@"
                select {0}, COUNT(*) AS Value
                    from {4} 
                            {1}
                    where 1 = 1 {5}
                            GROUP BY {2}
                            ORDER BY {3}
                ", scriteria, jcriteria, gcriteria, ocriteria, fcriteria, wcriteria);
        }

        private void PrepareSummaryQueryForPublication()
        {
            string fcriteria = ""; // from
            string jcriteria = ""; // joins
            string scriteria = ""; // selects
            string gcriteria = ""; // groups
            string ocriteria = ""; // orders
            string wcriteria = ""; // wheres

            var id = Convert.ToInt32(ddlEntity.SelectedValue);
            var metaTable = _db.MetaTables.Single(mt => mt.Id == id);

            // FROM
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Core:
                    fcriteria = "Meta" + metaTable.TableName + " P";
                    break;

                case MetaTableTypes.CoreChild:
                    fcriteria = "Meta" + metaTable.TableName + " C";
                    break;

                case MetaTableTypes.Child:
                    fcriteria = "Meta" + metaTable.TableName + " P";
                    break;

                case MetaTableTypes.History:
                    fcriteria = "Meta" + metaTable.TableName + " C";
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
                    metaDependency = _db.MetaDependencies.Single(md => md.ReferenceTable.Id == id);

                    jcriteria += String.Format(" LEFT JOIN Meta{0} P ON P.{1} = C.{2} ", metaDependency.ParentTable.TableName, metaDependency.ParentColumnName, metaDependency.ReferenceColumnName);

                    break;
            }

            // FIELDS
            var fc = 0;
            foreach (StratifyStructure strat in _strats)
            {
                fc+=1;

                scriteria += strat.AttributeName + " as 'Col" + fc.ToString() + "', ";
                gcriteria += strat.AttributeName + ", ";
                ocriteria += strat.AttributeName + ", ";
            }
            scriteria = scriteria.Substring(0, scriteria.Length - 2);
            gcriteria = gcriteria.Substring(0, gcriteria.Length - 2);
            ocriteria = ocriteria.Substring(0, ocriteria.Length - 2);

            // FILTERS
            var filc = 0;
            foreach (FilterStructure filter in _filters)
            {
                filc+=1;
                wcriteria += String.Format("{0} ({1} {2} %{3})", filter.Relation, filter.AttributeName, filter.Operator, filc.ToString() );
            }

            _sql = String.Format(@"
                select {0}, COUNT(*) AS Col{6}
                    from {4} 
                            {1}
                    where 1 = 1 {5}
                            GROUP BY {2}
                            ORDER BY {3}
                ", scriteria, jcriteria, gcriteria, ocriteria, fcriteria, wcriteria, (fc + 1).ToString());
        }

        private void PrepareListQueryForExecution()
        {
            string fcriteria = ""; // from
            string jcriteria = ""; // joins
            string scriteria = ""; // selects
            string gcriteria = ""; // groups
            string ocriteria = ""; // orders
            string wcriteria = ""; // wheres

            var id = Convert.ToInt32(ddlEntity.SelectedValue);
            var metaTable = _db.MetaTables.Single(mt => mt.Id == id);

            // FROM
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Core:
                    fcriteria = "Meta" + metaTable.TableName + " P";
                    break;

                case MetaTableTypes.CoreChild:
                    fcriteria = "Meta" + metaTable.TableName + " C";
                    break;

                case MetaTableTypes.Child:
                    fcriteria = "Meta" + metaTable.TableName + " P";
                    break;

                case MetaTableTypes.History:
                    fcriteria = "Meta" + metaTable.TableName + " C";
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
                    metaDependency = _db.MetaDependencies.Single(md => md.ReferenceTable.Id == id);

                    jcriteria += String.Format(" LEFT JOIN Meta{0} P ON P.{1} = C.{2} ", metaDependency.ParentTable.TableName, metaDependency.ParentColumnName, metaDependency.ReferenceColumnName);

                    break;
            }

            // FIELDS
            foreach (ListStructure list in _lists)
            {
                scriteria += list.AttributeName + " as '" + list.DisplayName + "', ";
                ocriteria += list.AttributeName + ", ";
            }
            scriteria = scriteria.Substring(0, scriteria.Length - 2);
            ocriteria = ocriteria.Substring(0, ocriteria.Length - 2);

            // FILTERS
            foreach (FilterStructure filter in _filters)
            {
                wcriteria += String.Format("{0} ({1} {2} {3})", filter.Relation, filter.AttributeName, filter.Operator, filter.FilterValue);
            }

            _sql = String.Format(@"
                select {0} 
                    from {3} 
                            {1}
                    where 1 = 1 {4}
                            ORDER BY {2}
                ", scriteria, jcriteria, ocriteria, fcriteria, wcriteria);
        }

        private void PrepareListQueryForPublication()
        {
            string fcriteria = ""; // from
            string jcriteria = ""; // joins
            string scriteria = ""; // selects
            string gcriteria = ""; // groups
            string ocriteria = ""; // orders
            string wcriteria = ""; // wheres

            var id = Convert.ToInt32(ddlEntity.SelectedValue);
            var metaTable = _db.MetaTables.Single(mt => mt.Id == id);

            // FROM
            switch ((MetaTableTypes)metaTable.TableType.Id)
            {
                case MetaTableTypes.Core:
                    fcriteria = "Meta" + metaTable.TableName + " P";
                    break;

                case MetaTableTypes.CoreChild:
                    fcriteria = "Meta" + metaTable.TableName + " C";
                    break;

                case MetaTableTypes.Child:
                    fcriteria = "Meta" + metaTable.TableName + " P";
                    break;

                case MetaTableTypes.History:
                    fcriteria = "Meta" + metaTable.TableName + " C";
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
                    metaDependency = _db.MetaDependencies.Single(md => md.ReferenceTable.Id == id);

                    jcriteria += String.Format(" LEFT JOIN Meta{0} P ON P.{1} = C.{2} ", metaDependency.ParentTable.TableName, metaDependency.ParentColumnName, metaDependency.ReferenceColumnName);

                    break;
            }

            // FIELDS
            var fc = 0;
            foreach (ListStructure list in _lists)
            {
                fc+=1;

                scriteria += list.AttributeName + " as 'Col" + fcriteria.ToString() + "', ";
                ocriteria += list.AttributeName + ", ";
            }
            scriteria = scriteria.Substring(0, scriteria.Length - 2);
            ocriteria = ocriteria.Substring(0, ocriteria.Length - 2);

            // FILTERS
            fc = 0;
            foreach (FilterStructure filter in _filters)
            {
                fc+=1;
                wcriteria += String.Format("{0} ({1} {2} %{3})", filter.Relation, filter.AttributeName, filter.Operator, fc.ToString());
            }

            _sql = String.Format(@"
                select {0} 
                    from {3} 
                            {1}
                    where 1 = 1 {4}
                            ORDER BY {2}
                ", scriteria, jcriteria, ocriteria, fcriteria, wcriteria);
        }

        private DataSet GetDatasetForSql(string sql)
        {
            PVIMSDbContext db = new PVIMSDbContext();
            DbConnection conn = db.Database.Connection;
            //DbDataReader reader;
            DataSet dataSet1 = new DataSet();
            DbDataAdapter adapter;

            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLClient");
            ConnectionState initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open();  // open connection if not already open
                using (DbCommand cmd = conn.CreateCommand())
                {
                    //adapter = new DbDataAdapter(sql, conn);
                    adapter = factory.CreateDataAdapter();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 180;
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dataSet1);
                    //reader = cmd.Execute();
                }
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close(); // only close connection if not initially open
            }
            //return dataSet1.GetXml();
            db.Dispose();
            return dataSet1;
        }

        #endregion

        #region "Export"

        protected void btnExportXml_Click(object sender, EventArgs e)
        {
            if (Request.Form["orgUnitIds"] == null) { return; }

            Search();

            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var ns = ""; // 

            string contentXml = string.Empty;
            string destName = string.Format("RC_{0}.xml", DateTime.Now.ToString("yyyyMMddhhmmsss"));
            string destFile = string.Format("{0}{1}", documentDirectory, destName);

            // Create document
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode;
            XmlNode filterNode;
            XmlNode contentHeadNode;
            XmlNode contentNode;
            XmlNode contentValueNode;
            XmlAttribute attrib;
            XmlComment comment;

            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);

            rootNode = xmlDoc.CreateElement("MISSA_CustomReport", ns);
            attrib = xmlDoc.CreateAttribute("CreatedDate");
            attrib.InnerText = DateTime.Now.ToString("yyyy-MM-dd hh:MM");
            rootNode.Attributes.Append(attrib);

            // Write filter
            filterNode = xmlDoc.CreateElement("Filter", ns);
            attrib = xmlDoc.CreateAttribute("StratifyBy");
            //attrib.InnerText = ddlOrgUnitType.Text;
            filterNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Criteria");
            //attrib.InnerText = ddlCriteria.Text;
            filterNode.Attributes.Append(attrib);
            rootNode.AppendChild(filterNode);

            // Write content
            var rowCount = 0;
            var cellCount = 0;

            contentHeadNode = xmlDoc.CreateElement("Content", ns);
            attrib = xmlDoc.CreateAttribute("RowCount");
            attrib.InnerText = (gvOutput.Rows.Count - 1).ToString();

            ArrayList headerArray = new ArrayList();

            // Establish header
            foreach (TableCell cell in gvOutput.HeaderRow.Cells)
            {
                if (!cell.Text.Contains("Action"))
                {
                    cellCount += 1;
                    headerArray.Add(cell.Text);
                }
            }

            foreach (TableRow row in gvOutput.Rows)
            {
                rowCount += 1;
                cellCount = 0;
                contentNode = xmlDoc.CreateElement("Row", ns);

                foreach (TableCell cell in row.Cells)
                {
                    if (!cell.Text.Contains("&nbsp;"))
                    {
                        cellCount += 1;

                        var nodeName = Regex.Replace(headerArray[cellCount - 1].ToString().Replace(" ", ""), "[^0-9a-zA-Z]+", "");
                        contentValueNode = xmlDoc.CreateElement(nodeName, ns);
                        contentValueNode.InnerText = cell.Text;

                        contentNode.AppendChild(contentValueNode);
                    }
                }
                if (contentNode.ChildNodes.Count > 0)
                {
                    contentHeadNode.AppendChild(contentNode);
                }
            }
            rootNode.AppendChild(contentHeadNode);
            xmlDoc.AppendChild(rootNode);

            contentXml = FormatXML(xmlDoc);
            WriteXML(destFile, contentXml);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/xml";
            Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", destName));
            Response.Charset = "";
            this.EnableViewState = false;

            Response.WriteFile(destFile);
            Response.End();
        }

        protected void btnExportXls_Click(object sender, EventArgs e)
        {
            if (Request.Form["orgUnitIds"] == null) { return; }

            Search();

            // Create XLS
            var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Custom Report");
            ws.View.ShowGridLines = true;

            // Write content
            var rowCount = 0;
            var cellCount = 0;

            // Establish header
            rowCount += 1;
            foreach (TableCell cell in gvOutput.HeaderRow.Cells)
            {
                if (!cell.Text.Contains("Action"))
                {
                    cellCount += 1;
                    ws.Cells[GetExcelColumnName(cellCount) + rowCount].Value = cell.Text;
                }
            }

            foreach (TableRow row in gvOutput.Rows)
            {
                rowCount += 1;
                cellCount = 0;

                foreach (TableCell cell in row.Cells)
                {
                    if (!cell.Text.Contains("&nbsp;"))
                    {
                        cellCount += 1;
                        ws.Cells[GetExcelColumnName(cellCount) + rowCount].Value = cell.Text;
                    }
                }
            }

            //format row
            using (var r = ws.Cells["A1:" + GetExcelColumnName(cellCount) + rowCount])
            {
                r.Style.Font.SetFromFont(new Font("Calibri", 10, FontStyle.Regular));
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                r.AutoFitColumns();
            }
            //Lock cells
            using (var r = ws.Cells["A1:" + GetExcelColumnName(cellCount) + rowCount])
                r.Style.Locked = true;

            // Add borders
            FormatAsBorder(ref ws, "A1:" + GetExcelColumnName(cellCount) + rowCount);

            // Format header
            FormatAsHeader(ref ws, "A1:" + GetExcelColumnName(cellCount) + "1", false, ExcelHorizontalAlignment.Left);

            ws.Protection.IsProtected = true;
            ws.Protection.AllowAutoFilter = false;
            ws.Protection.AllowDeleteColumns = false;
            ws.Protection.AllowDeleteRows = false;
            ws.Protection.AllowEditObject = false;
            ws.Protection.AllowEditScenarios = false;
            ws.Protection.AllowFormatCells = false;
            ws.Protection.AllowFormatColumns = false;
            ws.Protection.AllowFormatRows = false;
            ws.Protection.AllowInsertColumns = false;
            ws.Protection.AllowInsertHyperlinks = false;
            ws.Protection.AllowInsertRows = false;
            ws.Protection.AllowPivotTables = false;
            ws.Protection.AllowSelectLockedCells = false;
            ws.Protection.AllowSelectUnlockedCells = true;
            ws.Protection.AllowSort = false;
            ws.Protection.SetPassword("2323434weasddsfdfd!@ESDDS");

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", String.Format("attachment;filename=ReportEnrollment_{0}.xlsx", DateTime.Now.ToString("yyyyMMddhhmm")));
            Response.Charset = "";
            this.EnableViewState = false;

            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        private void FormatAsHeader(ref ExcelWorksheet ws, string range, bool usePattern, ExcelHorizontalAlignment alignment)
        {
            //Formatting - Bold
            using (var r = ws.Cells[range])
            {
                r.Style.Font.SetFromFont(new Font("Calibri", 10, FontStyle.Bold));
                r.Style.HorizontalAlignment = alignment;
                if (usePattern)
                {
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(23, 55, 93));
                    r.Style.Font.Color.SetColor(Color.White);
                }
                else
                {
                    r.Style.Font.Color.SetColor(Color.Black);
                }
            }
        }

        private void FormatAsBorder(ref ExcelWorksheet ws, string range)
        {
            //Formatting - Editable
            using (var r = ws.Cells[range])
            {
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
        }

        private void WriteXML(string xmlFileName, string xmlText)
        {
            string line = "********************************************************************";

            // Write the string to a file.
            StreamWriter file = new System.IO.StreamWriter(xmlFileName, false, Encoding.UTF8);

            file.Write(xmlText);

            file.Close();
            file = null;
        }

        #endregion

        #region "Events"

        protected void btnAddList_Click(object sender, EventArgs e)
        {
            if (ddlListElement.SelectedValue == "") { return; }

            var id = Convert.ToInt32(ddlListElement.SelectedValue);
            MetaColumn mc = _db.MetaColumns.Single(c => c.Id == id);

            var val = "";
            if (mc != null)
            {
                if (!ListContainsAttribute(mc))
                {
                    ListStructure list = new ListStructure();

                    list.MetaColumnId = mc.Id;
                    list.AttributeName = ddlListElement.SelectedItem.Text;
                    list.DisplayName = String.IsNullOrEmpty(txtListDisplayName.Value) ? ddlListElement.SelectedItem.Text : txtListDisplayName.Value;

                    _lists.Add(list);
                }
            }

            Session["lists"] = _lists;
            RenderLists();

            InitialiseEntityElements();
        }

        protected void btnAddStratify_Click(object sender, EventArgs e)
        {
            if (ddlElement.SelectedValue == "") { return; }

            var id = Convert.ToInt32(ddlElement.SelectedValue);
            MetaColumn metaColumn = _db.MetaColumns.Single(mc => mc.Id == id);

            var val = "";
            if (metaColumn != null)
            {
                if (!StratContainsAttribute(metaColumn))
                {
                    StratifyStructure strat = new StratifyStructure();

                    strat.MetaColumnId = metaColumn.Id;
                    strat.AttributeName = ddlElement.SelectedItem.Text;
                    strat.DisplayName = String.IsNullOrEmpty(txtDisplayName.Value) ? metaColumn.ColumnName : txtDisplayName.Value;

                    _strats.Add(strat);
                }
            }

            Session["strats"] = _strats;
            RenderStrats();

            InitialiseEntityElements();
        }

        protected void btnAddFilter_Click(object sender, EventArgs e)
        {
            if (ddlFilterElement.SelectedValue == "") { return; }
            if (ddlFilterOperator.SelectedValue == "") { return; }

            // Get the attribute and check what type of element it is
            var id = Convert.ToInt32(ddlFilterElement.SelectedValue);
            MetaColumn metaColumn = _db.MetaColumns.Single(mc => mc.Id == id);

            var val = "";
            if (metaColumn != null)
            {
                switch ((MetaColumnTypes)metaColumn.ColumnType.Id) 
                {
                    case MetaColumnTypes.tbigint:
                    case MetaColumnTypes.tint:
                    case MetaColumnTypes.tdecimal:
                    case MetaColumnTypes.tsmallint:
                    case MetaColumnTypes.ttinyint:

                        switch (ddlFilterOperator.SelectedValue)
                        {
                            case "=":
                            case "<>":
                            case ">":
                            case "<":
                            case ">=":
                            case "<=":
                                val = txtFilterNumericValue.Value;

                                break;

                            case "between":
                                val = String.Format(" {0} and {1} ", txtFilterNumericFrom.Value, txtFilterNumericTo.Value);

                                break;

                            default:
                                break;
                        }

                        break;

                    case MetaColumnTypes.tchar:
                    case MetaColumnTypes.tnchar:
                    case MetaColumnTypes.tnvarchar:
                    case MetaColumnTypes.tvarchar:

                        if(String.IsNullOrEmpty(metaColumn.Range))
                        {
                            switch (ddlFilterOperator.SelectedValue)
                            {
                                case "=":
                                case "<>":
                                    val = "'" + txtFilterTextValue.Value + "'";

                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (ddlFilterOperator.SelectedValue)
                            {
                                case "=":
                                case "<>":
                                    val = "'" + ddlFilterSelect.SelectedItem.Text + "'";

                                    break;

                                case "in":
                                    val = "(";
                                    foreach (ListItem li in lbFilterSelect.Items)
                                    {
                                        if (li.Selected)
                                        {
                                            val += "'" + li.Text + "', ";
                                        }
                                    }
                                    if (val != "(")
                                    {
                                        val = val.Substring(0, val.Length - 2);
                                    }
                                    val += ")";

                                    break;

                                default:
                                    break;
                            }
                        }

                        break;

                    case MetaColumnTypes.tdate:
                    case MetaColumnTypes.tdatetime:

                        switch (ddlFilterOperator.SelectedValue)
                        {
                            case "=":
                            case "<>":
                            case ">":
                            case "<":
                            case ">=":
                            case "<=":
                                val = "'" + txtFilterDateValue.Value + "'";

                                break;

                            case "between":
                                val = String.Format(" '{0}' and '{1}' ", txtFilterDateFrom.Value, txtFilterDateTo.Value);

                                break;

                            default:
                                break;
                        }

                        break;

                    default:
                        break;
                }
            }

            if (val.Trim() != "")
            {
                if (!FilterContainsAttribute(metaColumn))
                {
                    FilterStructure filter = new FilterStructure();

                    filter.MetaColumnId = metaColumn.Id;
                    filter.Relation = ddlFilterRelation.SelectedValue;
                    filter.AttributeName = ddlFilterElement.SelectedItem.Text;
                    filter.Operator = ddlFilterOperator.SelectedValue;
                    filter.FilterValue = val;

                    _filters.Add(filter);
                }
            }

            Session["filters"] = _filters;
            RenderFilters();

            InitialiseEntityElements();
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            divEntity.Visible = false;
            divStratify.Visible = false;
            divList.Visible = false;
            divFilter.Visible = false;

            _reportType = ReportType.None;

            switch (ddlType.SelectedValue)
            {
                case "Summary":
                    divEntity.Visible = true;
                    divStratify.Visible = true;
                    divFilter.Visible = true;

                    _reportType = ReportType.Summary;

                    break;

                case "List":
                    divEntity.Visible = true;
                    divList.Visible = true;
                    divFilter.Visible = true;

                    _reportType = ReportType.Listing;

                    break;

                default:
                    break;
            }
        }

        protected void ddlEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEntity.SelectedValue == "") { return; }

            // Initialise custom components
            _strats.Clear();
            Session["strats"] = null;
            RenderStrats();

            _filters.Clear();
            Session["filters"] = null;
            RenderFilters();

            InitialiseEntityElements();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Search();
        }
        
        protected void btnPublish_Click(object sender, EventArgs e)
        {
            Publish();
        }

        protected void ddlFilterElement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlFilterElement.SelectedValue == "") { return; }

            // Get the attribute and check what type of element it is
            var id = Convert.ToInt32(ddlFilterElement.SelectedValue);
            MetaColumn metaColumn = _db.MetaColumns.Single(mc => mc.Id == id);

            if (metaColumn != null)
            {
                ddlFilterOperator.Items.Clear();
                ListItem[] listItems;
                ListItem listItem = new ListItem() { Text = "-- Not Selected --", Value = "" };
                ddlFilterOperator.Items.Add(listItem);

                switch ((MetaColumnTypes)metaColumn.ColumnType.Id)
                {
                    case MetaColumnTypes.tbigint:
                    case MetaColumnTypes.tint:
                    case MetaColumnTypes.tdecimal:
                    case MetaColumnTypes.tsmallint:
                    case MetaColumnTypes.ttinyint:
                        listItems = new ListItem[] { new ListItem() { Text = "Equals", Value = "=" }, new ListItem() { Text = "Not Equals", Value = "<>" }, new ListItem() { Text = "Greater Than", Value = ">" }, new ListItem() { Text = "Less Than", Value = "<" }, new ListItem() { Text = "GreaterEqual Than", Value = ">=" }, new ListItem() { Text = "LessEqual Than", Value = "<=" }, new ListItem() { Text = "Between", Value = "between" } };
                        ddlFilterOperator.Items.AddRange(listItems);

                        break;

                    case MetaColumnTypes.tchar:
                    case MetaColumnTypes.tnchar:
                    case MetaColumnTypes.tnvarchar:
                    case MetaColumnTypes.tvarchar:
                        if (String.IsNullOrEmpty(metaColumn.Range))
                        {
                            listItems = new ListItem[] { new ListItem() { Text = "Equals", Value = "=" }, new ListItem() { Text = "Not Equals", Value = "<>" } };
                            ddlFilterOperator.Items.AddRange(listItems);
                        }
                        else
                        {
                            listItems = new ListItem[] { new ListItem() { Text = "Equals", Value = "=" }, new ListItem() { Text = "Not Equals", Value = "<>" }, new ListItem() { Text = "In", Value = "in" } };
                            ddlFilterOperator.Items.AddRange(listItems);
                        }
                        break;

                    case MetaColumnTypes.tdate:
                    case MetaColumnTypes.tdatetime:
                        listItems = new ListItem[] { new ListItem() { Text = "Equals", Value = "=" }, new ListItem() { Text = "Not Equals", Value = "<>" }, new ListItem() { Text = "Greater Than", Value = ">" }, new ListItem() { Text = "Less Than", Value = "<" }, new ListItem() { Text = "GreaterEqual Than", Value = ">=" }, new ListItem() { Text = "LessEqual Than", Value = "<=" }, new ListItem() { Text = "Between", Value = "between" } };
                        ddlFilterOperator.Items.AddRange(listItems);
                        break;

                    default:
                        break;
                }
            }
        }

        protected void ddlFilterOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlFilterElement.SelectedValue == "") { return; }
            if (ddlFilterOperator.SelectedValue == "") { return; }

            // Get the attribute and check what type of element it is
            var id = Convert.ToInt32(ddlFilterElement.SelectedValue);
            MetaColumn metaColumn = _db.MetaColumns.Single(mc => mc.Id == id);

            if (metaColumn != null)
            {
                lblFilterNumericValue.Visible = false;
                lblFilterNumericRange.Visible = false;
                lblFilterTextValue.Visible = false;
                lblFilterDateValue.Visible = false;
                lblFilterDateRange.Visible = false;
                lblFilterSelectValue.Visible = false;
                lblFilterInValue.Visible = false;

                switch ((MetaColumnTypes)metaColumn.ColumnType.Id)
                {
                    case MetaColumnTypes.tbigint:
                    case MetaColumnTypes.tint:
                    case MetaColumnTypes.tdecimal:
                    case MetaColumnTypes.tsmallint:
                    case MetaColumnTypes.ttinyint:

                        switch (ddlFilterOperator.SelectedValue)
                        {
                            case "=":
                            case "<>":
                            case ">":
                            case "<":
                            case ">=":
                            case "<=":
                                lblFilterNumericValue.Visible = true;
                                txtFilterNumericValue.Value = "0";

                                break;

                            case "between":
                                lblFilterNumericRange.Visible = true;
                                txtFilterNumericFrom.Value = "0";
                                txtFilterNumericTo.Value = "0";

                                break;

                            default:
                                break;
                        }

                        break;

                    case MetaColumnTypes.tchar:
                    case MetaColumnTypes.tnchar:
                    case MetaColumnTypes.tnvarchar:
                    case MetaColumnTypes.tvarchar:

                        if (String.IsNullOrEmpty(metaColumn.Range))
                        {
                            switch (ddlFilterOperator.SelectedValue)
                            {
                                case "=":
                                case "<>":
                                    lblFilterTextValue.Visible = true;
                                    txtFilterTextValue.Value = "";

                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            ListItem[] values = null;
                            // extract values : SOURCE
                            if (metaColumn.Range.Contains("SOURCE:"))
                            {
                                var sources = metaColumn.Range.Replace("SOURCE:", "").Split('.');
                                switch (sources[0])
                                {
                                    case "EncounterType":
                                        values = _db.EncounterTypes.OrderBy(et => et.Description).Select(s => new ListItem
                                        {
                                            Value = s.Description,
                                            Text = s.Description
                                        })
                                        .ToArray();

                                        break;

                                    case "Facility":
                                        values = _db.Facilities.OrderBy(f => f.FacilityName).Select(s => new ListItem
                                        {
                                            Value = s.FacilityName,
                                            Text = s.FacilityName
                                        })
                                        .ToArray();

                                        break;

                                    case "CohortGroup":
                                        values = _db.CohortGroups.OrderBy(cg => cg.CohortName).Select(s => new ListItem
                                        {
                                            Value = s.CohortName,
                                            Text = s.CohortName
                                        })
                                        .ToArray();

                                        break;

                                    case "LabTestUnit":
                                        values = _db.LabTestUnits.OrderBy(ltu => ltu.Description).Select(s => new ListItem
                                        {
                                            Value = s.Description,
                                            Text = s.Description
                                        })
                                        .ToArray();

                                        break;

                                    case "LabTest":
                                        values = _db.LabTests.OrderBy(lt => lt.Description).Select(s => new ListItem
                                        {
                                            Value = s.Description,
                                            Text = s.Description
                                        })
                                        .ToArray();

                                        break;

                                    case "Outcome":
                                        values = _db.Outcomes.OrderBy(o => o.Description).Select(s => new ListItem
                                        {
                                            Value = s.Description,
                                            Text = s.Description
                                        })
                                        .ToArray();

                                        break;

                                    case "Medication":
                                        values = _db.Medications.OrderBy(m => m.DrugName).Select(s => new ListItem
                                        {
                                            Value = s.DrugName,
                                            Text = s.DrugName
                                        })
                                        .ToArray();

                                        break;

                                    case "User":
                                        values = _db.Users.OrderBy(u => u.UserName).Select(s => new ListItem
                                        {
                                            Value = s.UserName,
                                            Text = s.UserName
                                        })
                                        .ToArray();

                                        break;

                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                values = metaColumn.Range.Split(',').Select(s => new ListItem
                                    {
                                        Value = s,
                                        Text = s
                                    })
                                    .ToArray();
                            }

                            switch (ddlFilterOperator.SelectedValue)
                            {
                                case "=":
                                case "<>":
                                    lblFilterSelectValue.Visible = true;
                                    ddlFilterSelect.Items.Clear();

                                    ddlFilterSelect.Items.AddRange(values);

                                    break;

                                case "in":
                                    lblFilterInValue.Visible = true;
                                    lbFilterSelect.Items.Clear();

                                    lbFilterSelect.Items.AddRange(values);

                                    break;

                                default:
                                    break;
                            }
                        }

                        break;

                    case MetaColumnTypes.tdate:
                    case MetaColumnTypes.tdatetime:

                        switch (ddlFilterOperator.SelectedValue)
                        {
                            case "=":
                            case "<>":
                            case ">":
                            case "<":
                            case ">=":
                            case "<=":
                                lblFilterDateValue.Visible = true;
                                txtFilterDateValue.Value = DateTime.Today.ToString("yyyy-MM-dd");

                                break;

                            case "between":
                                lblFilterDateRange.Visible = true;
                                txtFilterDateFrom.Value = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                                txtFilterDateTo.Value = DateTime.Today.ToString("yyyy-MM-dd");

                                break;

                            default:
                                break;
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        void ImageButtonList_OnCommand(object sender, CommandEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;

            var index = Convert.ToInt32(btn.CommandArgument);
            _lists.RemoveAt(index);

            Session["lists"] = _lists;
            RenderLists();
        }

        void ImageButtonStrat_OnCommand(object sender, CommandEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;

            var index = Convert.ToInt32(btn.CommandArgument);
            _strats.RemoveAt(index);

            Session["strats"] = _strats;
            RenderStrats();
        }

        void ImageButtonFilter_OnCommand(object sender, CommandEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;

            var index = Convert.ToInt32(btn.CommandArgument);
            _filters.RemoveAt(index);

            Session["filters"] = _filters;
            RenderFilters();
        }

        #endregion

        #region "Internal"

        private bool ValidDefinition()
        {
            bool valid = true;

            lblReportName.Attributes.Remove("class");
            lblReportName.Attributes.Add("class", "input");
            lblDefinition.Attributes.Remove("class");
            lblDefinition.Attributes.Add("class", "input");
            lblReportType.Attributes.Remove("class");
            lblReportType.Attributes.Add("class", "input");
            lblEntity.Attributes.Remove("class");
            lblEntity.Attributes.Add("class", "input");

            if (!String.IsNullOrWhiteSpace(txtReportName.Value))
            {
                if (Regex.Matches(txtReportName.Value, @"[a-zA-Z']").Count < txtReportName.Value.Length)
                {
                    lblReportName.Attributes.Remove("class");
                    lblReportName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Report Name contains invalid characters (Enter A-Z, a-z)";
                    lblReportName.Controls.Add(errorMessageDiv);

                    valid = false;
                }
            }
            else
            {
                lblReportName.Attributes.Remove("class");
                lblReportName.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Report Name must be entered";
                lblReportName.Controls.Add(errorMessageDiv);

                valid = false;
            }

            if (!String.IsNullOrWhiteSpace(txtDefinition.Value))
            {
                if (Regex.Matches(txtDefinition.Value, @"[a-zA-Z'!,]").Count < txtDefinition.Value.Length)
                {
                    lblDefinition.Attributes.Remove("class");
                    lblDefinition.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Definition contains invalid characters (Enter A-Z, a-z, '!,)";
                    lblDefinition.Controls.Add(errorMessageDiv);

                    valid = false;
                }
            }

            if (ddlType.SelectedValue == "")
            {
                lblReportType.Attributes.Remove("class");
                lblReportType.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Report Type must be selected";
                lblReportType.Controls.Add(errorMessageDiv);

                valid = false;
            }

            if (ddlEntity.SelectedValue == "")
            {
                lblEntity.Attributes.Remove("class");
                lblEntity.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Entity must be selected";
                lblEntity.Controls.Add(errorMessageDiv);

                valid = false;
            }

            if (_reportType == ReportType.Summary && _strats.Count == 0) { valid = false; }
            if (_reportType == ReportType.Listing && _lists.Count == 0) { valid = false; }

            // Prepare SQL
            _sql = "";

            if (_reportType == ReportType.Summary) { PrepareSummaryQueryForPublication(); };
            if (_reportType == ReportType.Listing) { PrepareListQueryForPublication(); };

            if (_sql == "") { valid = false; }

            return valid;
        }

        static public string FormatXML(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        #endregion

    }

    public class ListStructure
    {
        public int MetaColumnId { get; set; }
        public string AttributeName { get; set; }
        public string DisplayName { get; set; }
    }

    public class StratifyStructure
    {
        public int MetaColumnId { get; set; }
        public string AttributeName { get; set; }
        public string DisplayName { get; set; }
    }

    public class FilterStructure
    {
        public int MetaColumnId { get; set; }
        public string Relation { get; set; }
        public string AttributeName { get; set; }
        public string Operator { get; set; }
        public string FilterValue { get; set; }
    }

}