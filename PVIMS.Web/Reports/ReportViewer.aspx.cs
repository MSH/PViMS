using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using System.Data;
using System.Data.SqlClient;

using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class ReportViewer : MainPageBase
    {
        private PVIMSDbContext _db = new PVIMSDbContext();

        private MetaReport _metaReport;
        private int _id = 0;

        private List<StratifyStructure> _strats = new List<StratifyStructure>();
        private List<FilterStructure> _filters = new List<FilterStructure>();
        private List<ListStructure> _lists = new List<ListStructure>();

        private bool _isPublisher = false;

        enum ReportType
        {
            None,
            Summary,
            Listing
        }
        private ReportType _reportType = ReportType.None;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.IsInRole("ReporterAdmin")) { _isPublisher = true; }

            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0)
                {
                    _metaReport = _db.MetaReports.Single(mr => mr.Id == _id);

                    // Prepare report
                    PrepareStructures();

                    RenderFilters();
                    RenderColumns();
                }
            }
            else
            {
                throw new Exception("id not passed as parameter");
            }

            //txtSearchFrom.Value = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            //txtSearchTo.Value = DateTime.Today.ToString("yyyy-MM-dd");

            //ListItem item;
            //var facilityList = (from f in UnitOfWork.Repository<Facility>().Queryable() orderby f.FacilityName ascending select f).ToList();

            //foreach (Facility fac in facilityList)
            //{
            //    item = new ListItem();
            //    item.Text = fac.FacilityName;
            //    item.Value = fac.Id.ToString();
            //    ddlFacility.Items.Add(item);
            //}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.MainMenu.SetActive("ReportAdmin");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Custom Reports", SubTitle = _metaReport.ReportName, Icon = "fa fa-file-text-o fa-fw", MetaReportId = _isPublisher ? _metaReport.Id : 0 });
        }

        #region "Preparation"

        private void PrepareStructures()
        {
            var ns = ""; // urn:pvims-org:v3

            XmlDocument meta = new XmlDocument();
            meta.LoadXml(_metaReport.MetaDefinition);

            // Unpack structures
            XmlNode rootNode = meta.SelectSingleNode("//MetaReport");
            XmlAttribute attr = rootNode.Attributes["Type"];
            XmlNode mainNode;

            if (attr.Value == "Summary")
            {
                _reportType = ReportType.Summary;

                mainNode = rootNode.SelectSingleNode("//Summary");
                if (mainNode != null)
                {
                    foreach (XmlNode subNode in mainNode.ChildNodes)
                    {
                        StratifyStructure strat = new StratifyStructure();
                        strat.MetaColumnId = Convert.ToInt32(subNode.Attributes["MetaColumnId"].Value);
                        strat.AttributeName = subNode.Attributes["AttributeName"].Value;
                        strat.DisplayName = subNode.Attributes["DisplayName"].Value;
                        _strats.Add(strat);
                    }
                }
            }
            else
            {
                _reportType = ReportType.Listing;

                mainNode = rootNode.SelectSingleNode("//List");
                if(mainNode != null)
                {
                    foreach (XmlNode subNode in mainNode.ChildNodes)
                    {
                        ListStructure list = new ListStructure();
                        list.MetaColumnId = Convert.ToInt32(subNode.Attributes["MetaColumnId"].Value);
                        list.AttributeName = subNode.Attributes.GetNamedItem("AttributeName").Value;
                        list.DisplayName = subNode.Attributes.GetNamedItem("DisplayName").Value;
                        _lists.Add(list);
                    }
                }
            }

            // filter
            mainNode = rootNode.SelectSingleNode("//Filter");
            if (mainNode != null)
            {
                foreach (XmlNode subNode in mainNode.ChildNodes)
                {
                    FilterStructure filter = new FilterStructure();
                    filter.MetaColumnId = Convert.ToInt32(subNode.Attributes["MetaColumnId"].Value);
                    filter.AttributeName = subNode.Attributes.GetNamedItem("AttributeName").Value;
                    filter.Operator = subNode.Attributes.GetNamedItem("Operator").Value;
                    filter.Relation = subNode.Attributes.GetNamedItem("Relation").Value;
                    _filters.Add(filter);
                }
            }
        }

        private void RenderFilters()
        {
            if (_filters.Count == 0)
            {
                HtmlGenericControl span = new HtmlGenericControl("span");
                span.Attributes.Add("class", "label");
                span.Attributes.Add("style", "padding:5px; background-color:#F1F1F1; text-align:center;");
                span.InnerText = "NO FILTERS CONFIGURED";

                spnFilter.Controls.Add(span);
            }
            else
            {
                var secCount = 6;
                HtmlGenericControl row = null;

                foreach (FilterStructure filter in _filters)
                {
                    secCount += 1;
                    if (secCount > 5)
                    {
                        if (row != null) { spnFilter.Controls.Add(row); }
                        secCount = 0;
                        row = new HtmlGenericControl("div");
                        row.Attributes.Add("class", "row");
                    }

                    // Get the attribute and check what type of element it is
                    MetaColumn metaColumn = _db.MetaColumns.Single(mc => mc.Id == filter.MetaColumnId);

                    if (metaColumn != null)
                    {
                        HtmlGenericControl sec = new HtmlGenericControl("section");
                        sec.Attributes.Add("class", "col col-2");

                        Label lbl = new Label() { CssClass = "input" };
                        Label lblFriendlyName = new Label() { CssClass = "label", Text = filter.AttributeName };
                        lbl.Controls.Add(lblFriendlyName);

                        Label lblAnd = new Label() { CssClass = "label", Text = " and " };

                        switch ((MetaColumnTypes)metaColumn.ColumnType.Id)
                        {
                            case MetaColumnTypes.tbigint:
                            case MetaColumnTypes.tint:
                            case MetaColumnTypes.tdecimal:
                            case MetaColumnTypes.tsmallint:
                            case MetaColumnTypes.ttinyint:

                                switch (filter.Operator)
                                {
                                    case "=":
                                    case "<>":
                                    case ">":
                                    case "<":
                                    case ">=":
                                    case "<=":
                                        TextBox txtFilterNumericValue = new TextBox() { TextMode = TextBoxMode.Number, CssClass = "form-control", Text = "0" };
                                        lbl.Controls.Add(txtFilterNumericValue);

                                        break;

                                    case "between":
                                        TextBox txtFilterNumericFrom = new TextBox() { TextMode = TextBoxMode.Number, CssClass = "form-control", Text = "0" };
                                        lbl.Controls.Add(txtFilterNumericFrom);
                                        lbl.Controls.Add(lblAnd);
                                        TextBox txtFilterNumericTo = new TextBox() { TextMode = TextBoxMode.Number, CssClass = "form-control", Text = "0" };
                                        lbl.Controls.Add(txtFilterNumericTo);

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
                                    switch (filter.Operator)
                                    {
                                        case "=":
                                        case "<>":
                                            TextBox txtFilterTextValue = new TextBox() { CssClass = "form-control", Text = "" };
                                            lbl.Controls.Add(txtFilterTextValue);

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
                                            case "OrgUnit":
                                                values = _db.OrgUnits.OrderBy(f => f.Name).Select(s => new ListItem
                                                {
                                                    Value = s.Name,
                                                    Text = s.Name
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

                                    switch (filter.Operator)
                                    {
                                        case "=":
                                        case "<>":
                                            DropDownList ddlSelectValue = new DropDownList() { CssClass = "form-control" };
                                            ddlSelectValue.Items.AddRange(values);
                                            lbl.Controls.Add(ddlSelectValue);

                                            break;

                                        case "in":
                                            ListBox lbInValue = new ListBox() { CssClass = "form-control", Rows = 6, SelectionMode = ListSelectionMode.Multiple };
                                            lbInValue.Attributes.Add("style", "color:black; height:100px;");
                                            lbInValue.Items.AddRange(values);
                                            lbl.Controls.Add(lbInValue);

                                            break;

                                        default:
                                            break;
                                    }
                                }

                                break;

                            case MetaColumnTypes.tdate:
                            case MetaColumnTypes.tdatetime:

                                switch (filter.Operator)
                                {
                                    case "=":
                                    case "<>":
                                    case ">":
                                    case "<":
                                    case ">=":
                                    case "<=":
                                        TextBox txtFilterDateValue = new TextBox() { CssClass = "form-control datepicker" };
                                        txtFilterDateValue.Attributes.Add("placeholder", "yyyy-mm-dd");
                                        txtFilterDateValue.Text = DateTime.Today.ToString("yyyy-MM-dd");
                                        lbl.Controls.Add(txtFilterDateValue);

                                        break;

                                    case "between":
                                        TextBox txtFilterDateFrom = new TextBox() { CssClass = "form-control datepicker" };
                                        txtFilterDateFrom.Attributes.Add("placeholder", "yyyy-mm-dd");
                                        txtFilterDateFrom.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                                        lbl.Controls.Add(txtFilterDateFrom);
                                        lbl.Controls.Add(lblAnd);
                                        TextBox txtFilterDateTo = new TextBox() { CssClass = "form-control datepicker" };
                                        txtFilterDateTo.Attributes.Add("placeholder", "yyyy-mm-dd");
                                        txtFilterDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
                                        lbl.Controls.Add(txtFilterDateTo);

                                        break;

                                    default:
                                        break;
                                }

                                break;

                            default:
                                break;
                        } // switch ((MetaColumnTypes)metaColumn.ColumnType.Id)

                        sec.Controls.Add(lbl);
                        row.Controls.Add(sec);

                    } // if (metaColumn != null)
                } // foreach (FilterStructure filter in _filters)

                if (row.Controls.Count > 0) { spnFilter.Controls.Add(row); }
            }
        }

        private void RenderColumns()
        {
            var i = 0;
            if (_reportType == ReportType.Listing)
            {
                foreach (ListStructure list in _lists)
                {
                    dt_basic.SetColumn(i, list.DisplayName, new Unit(20));
                    i++;
                }
                for (int x = i; x <= 20; x++)
                {
                    dt_basic.HideColumn(x);
                }
            }
            else
            {
                foreach (StratifyStructure strat in _strats)
                {
                    dt_basic.SetColumn(i, strat.DisplayName, new Unit(20));
                    i++;
                }

                // Cater for summary count
                dt_basic.SetColumn(i, "Value", new Unit(20));
                i++;

                for (int x = i; x <= 20; x++)
                {
                    dt_basic.HideColumn(x);
                }
            }

        }

        #endregion

        #region "Internal"

        #endregion

        #region "Execution"

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            //            string searchfrom = txtSearchFrom.Value;
            //            string searchto = txtSearchTo.Value;

            bool err = false;

            divError.Visible = false;
            spnNoRows.Visible = false;
            spnRows.Visible = false;

            //            lblSearchFrom.Attributes.Remove("class");
            //            lblSearchFrom.Attributes.Add("class", "input");
            //            lblSearchTo.Attributes.Remove("class");
            //            lblSearchTo.Attributes.Add("class", "input");

            //            DateTime dttemp;

            //            if (!String.IsNullOrWhiteSpace(searchfrom) && !String.IsNullOrWhiteSpace(searchto))
            //            {
            //                if (DateTime.TryParse(searchfrom, out dttemp))
            //                {
            //                    dttemp = Convert.ToDateTime(searchfrom);
            //                    if (dttemp > DateTime.Today)
            //                    {
            //                        lblSearchFrom.Attributes.Remove("class");
            //                        lblSearchFrom.Attributes.Add("class", "input state-error");
            //                        var errorMessageDiv = new HtmlGenericControl("div");
            //                        errorMessageDiv.Attributes.Add("class", "note note-error");
            //                        errorMessageDiv.InnerText = "Search From cannot be after current date";
            //                        lblSearchFrom.Controls.Add(errorMessageDiv);

            //                        err = true;
            //                    }
            //                    if (dttemp < DateTime.Today.AddYears(-120))
            //                    {
            //                        lblSearchFrom.Attributes.Remove("class");
            //                        lblSearchFrom.Attributes.Add("class", "input state-error");
            //                        var errorMessageDiv = new HtmlGenericControl("div");
            //                        errorMessageDiv.Attributes.Add("class", "note note-error");
            //                        errorMessageDiv.InnerText = "Search From cannot be so far in the past";
            //                        lblSearchFrom.Controls.Add(errorMessageDiv);

            //                        err = true;
            //                    }
            //                }
            //                else
            //                {
            //                    lblSearchFrom.Attributes.Remove("class");
            //                    lblSearchFrom.Attributes.Add("class", "input state-error");
            //                    var errorMessageDiv = new HtmlGenericControl("div");
            //                    errorMessageDiv.Attributes.Add("class", "note note-error");
            //                    errorMessageDiv.InnerText = "Search From has an invalid date format";
            //                    lblSearchFrom.Controls.Add(errorMessageDiv);

            //                    err = true;
            //                }

            //                if (DateTime.TryParse(searchto, out dttemp))
            //                {
            //                    dttemp = Convert.ToDateTime(searchto);
            //                    if (dttemp > DateTime.Today)
            //                    {
            //                        lblSearchTo.Attributes.Remove("class");
            //                        lblSearchTo.Attributes.Add("class", "input state-error");
            //                        var errorMessageDiv = new HtmlGenericControl("div");
            //                        errorMessageDiv.Attributes.Add("class", "note note-error");
            //                        errorMessageDiv.InnerText = "Search To cannot be after current date";
            //                        lblSearchTo.Controls.Add(errorMessageDiv);

            //                        err = true;
            //                    }
            //                    if (dttemp < DateTime.Today.AddYears(-120))
            //                    {
            //                        lblSearchTo.Attributes.Remove("class");
            //                        lblSearchTo.Attributes.Add("class", "input state-error");
            //                        var errorMessageDiv = new HtmlGenericControl("div");
            //                        errorMessageDiv.Attributes.Add("class", "note note-error");
            //                        errorMessageDiv.InnerText = "Search To cannot be so far in the past";
            //                        lblSearchTo.Controls.Add(errorMessageDiv);

            //                        err = true;
            //                    }
            //                }
            //                else
            //                {
            //                    lblSearchTo.Attributes.Remove("class");
            //                    lblSearchTo.Attributes.Add("class", "input state-error");
            //                    var errorMessageDiv = new HtmlGenericControl("div");
            //                    errorMessageDiv.Attributes.Add("class", "note note-error");
            //                    errorMessageDiv.InnerText = "Search To has an invalid date format";
            //                    lblSearchTo.Controls.Add(errorMessageDiv);

            //                    err = true;
            //                }

            //                if (DateTime.TryParse(searchfrom, out dttemp) && DateTime.TryParse(searchto, out dttemp))
            //                {
            //                    if (Convert.ToDateTime(searchfrom) > Convert.ToDateTime(searchto))
            //                    {
            //                        lblSearchFrom.Attributes.Remove("class");
            //                        lblSearchFrom.Attributes.Add("class", "input state-error");
            //                        var errorMessageDiv = new HtmlGenericControl("div");
            //                        errorMessageDiv.Attributes.Add("class", "note note-error");
            //                        errorMessageDiv.InnerText = "Search From must be before Search To";
            //                        lblSearchFrom.Controls.Add(errorMessageDiv);

            //                        lblSearchTo.Attributes.Remove("class");
            //                        lblSearchTo.Attributes.Add("class", "input state-error");
            //                        errorMessageDiv = new HtmlGenericControl("div");
            //                        errorMessageDiv.Attributes.Add("class", "note note-error");
            //                        errorMessageDiv.InnerText = "Search To must be after Search From";
            //                        lblSearchTo.Controls.Add(errorMessageDiv);

            //                        err = true;
            //                    }
            //                }
            //            }

            //            if (err)
            //            {
            //                divError.Visible = true;
            //                return;
            //            };

            //            string where = ddlFacility.SelectedValue != "" ? " AND pf.Facility_Id = " + ddlFacility.SelectedValue : "";

            string sql = _metaReport.SQLDefinition;

            // Modify SQL to use filter values
            var fc = 0;
            foreach (FilterStructure filter in _filters)
            {
                fc += 1;

                sql = sql.Replace("%" + fc.ToString(), filter.FilterValue);
            }

            SqlParameter[] parameters = new SqlParameter[0];
            var results = UnitOfWork.Repository<CustomReportList>().ExecuteSql(sql, parameters);

            if (results.Count == 0)
            {
                spnNoRows.InnerText = "No matching records found...";
                spnNoRows.Visible = true;
                spnRows.Visible = false;
            }
            else
            {
                spnRows.InnerText = results.Count.ToString() + " row(s) matching criteria found...";
                spnRows.Visible = true;
                spnNoRows.Visible = false;
            }

            TableRow row;
            TableCell cell;

            if (results.Count > 0)
            {
                foreach (CustomReportList item in results)
                {
                    row = new TableRow();
                    var i = 0;
                    if (_reportType == ReportType.Listing)
                    {
                        foreach (ListStructure list in _lists)
                        {
                            cell = new TableCell();

                            i++;
                            if (i == 1)
                            {
                                cell.Text = item.Col1;
                            }
                            if (i == 2)
                            {
                                cell.Text = item.Col2;
                            }
                            if (i == 3)
                            {
                                cell.Text = item.Col3;
                            }
                            if (i == 4)
                            {
                                cell.Text = item.Col4;
                            }
                            if (i == 5)
                            {
                                cell.Text = item.Col5;
                            }
                            if (i == 6)
                            {
                                cell.Text = item.Col6;
                            }
                            if (i == 7)
                            {
                                cell.Text = item.Col7;
                            }
                            if (i == 8)
                            {
                                cell.Text = item.Col8;
                            }
                            if (i == 9)
                            {
                                cell.Text = item.Col9;
                            }
                            if (i == 10)
                            {
                                cell.Text = item.Col10;
                            }
                            row.Cells.Add(cell);
                        }
                    }
                    else
                    {
                        foreach (StratifyStructure strat in _strats)
                        {
                            cell = new TableCell();

                            i++;
                            if (i == 1)
                            {
                                cell.Text = item.Col1;
                            }
                            if (i == 2)
                            {
                                cell.Text = item.Col2;
                            }
                            if (i == 3)
                            {
                                cell.Text = item.Col3;
                            }
                            if (i == 4)
                            {
                                cell.Text = item.Col4;
                            }
                            if (i == 5)
                            {
                                cell.Text = item.Col5;
                            }
                            if (i == 6)
                            {
                                cell.Text = item.Col6;
                            }
                            if (i == 7)
                            {
                                cell.Text = item.Col7;
                            }
                            if (i == 8)
                            {
                                cell.Text = item.Col8;
                            }
                            if (i == 9)
                            {
                                cell.Text = item.Col9;
                            }
                            if (i == 10)
                            {
                                cell.Text = item.Col10;
                            }
                            row.Cells.Add(cell);
                        }
                        cell = new TableCell();

                        i++;
                        if (i == 1)
                        {
                            cell.Text = item.Col1;
                        }
                        if (i == 2)
                        {
                            cell.Text = item.Col2;
                        }
                        if (i == 3)
                        {
                            cell.Text = item.Col3;
                        }
                        if (i == 4)
                        {
                            cell.Text = item.Col4;
                        }
                        if (i == 5)
                        {
                            cell.Text = item.Col5;
                        }
                        if (i == 6)
                        {
                            cell.Text = item.Col6;
                        }
                        if (i == 7)
                        {
                            cell.Text = item.Col7;
                        }
                        if (i == 8)
                        {
                            cell.Text = item.Col8;
                        }
                        if (i == 9)
                        {
                            cell.Text = item.Col9;
                        }
                        if (i == 10)
                        {
                            cell.Text = item.Col10;
                        }
                        row.Cells.Add(cell);

                    }

                    dt_basic.Rows.Add(row);
                }
            }
        }

        #endregion

        #region "Export"

        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            // Populate report
            PopulateGrid();

            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var logoDirectory = String.Format("{0}\\img\\", System.AppDomain.CurrentDomain.BaseDirectory);

            string destName = string.Format("ROV_{0}.pdf", DateTime.Now.ToString("yyyyMMddhhmmsss"));
            string destFile = string.Format("{0}{1}", documentDirectory, destName);

            string logoName = string.Format("SIAPS_USAID_Horiz.jpg");
            string logoFile = string.Format("{0}{1}", logoDirectory, logoName);

            string fontFile = string.Format("{0}\\arial.ttf", System.AppDomain.CurrentDomain.BaseDirectory);

            var linePosition = 60;
            var columnPosition = 30;

            // Create document
            PdfDocument pdfDoc = new PdfDocument();

            // Create a new page
            PdfPage page = pdfDoc.AddPage();
            page.Orientation = PageOrientation.Landscape;

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XPen pen = new XPen(XColor.FromArgb(255, 0, 0));

            // Logo
            XImage image = XImage.FromFile(logoFile);
            gfx.DrawImage(image, 10, 10);

            // Create a new font
            Uri fontUri = new Uri(fontFile);
            try
            {
                XPrivateFontCollection.Global.Add(fontUri, "#Arial");
            }
            catch
            {
            }

            XFont fontb = new XFont("Calibri", 10, XFontStyle.Bold | XFontStyle.Underline);
            XFont fontr = new XFont("Calibri", 10, XFontStyle.Regular);

            // Write header
            pdfDoc.Info.Title = "Outstanding Visit Report for " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            gfx.DrawString("Outstanding Visit Report for " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), fontb, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);

            // Write filter
            linePosition += 24;
            //gfx.DrawString("Range From : " + txtSearchFrom.Value, fontr, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);
            linePosition += 24;
            //gfx.DrawString("Range To : " + txtSearchTo.Value, fontr, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);

            // Write content
            var pageCount = 1;
            var rowCount = 0;
            var cellCount = 0;

            ArrayList headerArray = new ArrayList();
            ArrayList widthArray = new ArrayList();

            foreach (TableRow row in dt_basic.Rows)
            {
                rowCount += 1;
                cellCount = 0;

                linePosition += 24;
                columnPosition = 30;

                if (linePosition >= 480)
                {
                    pageCount += 1;

                    page = pdfDoc.AddPage();
                    page.Orientation = PageOrientation.Landscape;

                    linePosition = 60;

                    gfx = XGraphics.FromPdfPage(page);
                    tf = new XTextFormatter(gfx);

                    // Logo
                    gfx.DrawImage(image, 10, 10);

                    gfx.DrawString("Outstanding Visit Report (Page " + pageCount.ToString() + ")", fontb, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);

                    linePosition += 24;

                    /// rewrite column headers
                    foreach (var header in headerArray)
                    {
                        cellCount += 1;
                        var width = Convert.ToInt32(widthArray[cellCount - 1]);

                        gfx.DrawString(header.ToString(), fontb, XBrushes.Black, new XRect(columnPosition, linePosition, width, 20), XStringFormats.TopLeft);
                        columnPosition += width;
                    }

                    columnPosition = 30;
                    linePosition += 24;
                    cellCount = 0;
                }

                foreach (TableCell cell in row.Cells)
                {
                    int[] ignore = { };

                    if (!ignore.Contains(cellCount))
                    {
                        cellCount += 1;

                        if (rowCount == 1)
                        {
                            widthArray.Add((int)cell.Width.Value * 5);
                            headerArray.Add(cell.Text);

                            gfx.DrawString(cell.Text, fontb, XBrushes.Black, new XRect(columnPosition, linePosition, cell.Width.Value * 5, 20), XStringFormats.TopLeft);
                            columnPosition += (int)cell.Width.Value * 5;
                        }
                        else
                        {
                            var width = Convert.ToInt32(widthArray[cellCount - 1]);

                            tf.DrawString(cell.Text, fontr, XBrushes.Black, new XRect(columnPosition, linePosition, width, 20), XStringFormats.TopLeft);
                            columnPosition += width;
                        }
                    }
                }
            }

            pdfDoc.Save(destFile);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", destName));
            Response.Charset = "";
            this.EnableViewState = false;

            Response.WriteFile(destFile);
            Response.End();
        }

        protected void btnExportXml_Click(object sender, EventArgs e)
        {
            // Populate report
            PopulateGrid();

            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var ns = ""; // urn:pvims-org:v3

            string contentXml = string.Empty;
            string destName = string.Format("ROV_{0}.xml", DateTime.Now.ToString("yyyyMMddhhmmsss"));
            string destFile = string.Format("{0}{1}", documentDirectory, destName);

            // Create document
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode;
            XmlNode filterNode;
            XmlNode contentHeadNode;
            XmlNode contentNode;
            XmlNode contentValueNode;
            XmlAttribute attrib;

            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);

            rootNode = xmlDoc.CreateElement("PViMS_OutstandingVisitReport", ns);
            attrib = xmlDoc.CreateAttribute("CreatedDate");
            attrib.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            rootNode.Attributes.Append(attrib);

            // Write filter
            //filterNode = xmlDoc.CreateElement("Filter", ns);
            //attrib = xmlDoc.CreateAttribute("RangeFrom");
            //attrib.InnerText = txtSearchFrom.Value;
            //filterNode.Attributes.Append(attrib);
            //attrib = xmlDoc.CreateAttribute("RangeTo");
            //attrib.InnerText = txtSearchTo.Value;
            //filterNode.Attributes.Append(attrib); rootNode.AppendChild(filterNode);

            // Write content
            var rowCount = 0;
            var cellCount = 0;

            contentHeadNode = xmlDoc.CreateElement("Content", ns);
            attrib = xmlDoc.CreateAttribute("RowCount");
            attrib.InnerText = (dt_basic.Rows.Count - 1).ToString();

            ArrayList headerArray = new ArrayList();

            foreach (TableRow row in dt_basic.Rows)
            {
                rowCount += 1;
                cellCount = 0;
                contentNode = xmlDoc.CreateElement("Row", ns);

                foreach (TableCell cell in row.Cells)
                {
                    int[] ignore = { };

                    if (!ignore.Contains(cellCount))
                    {
                        cellCount += 1;

                        if (rowCount == 1)
                        {
                            headerArray.Add(cell.Text);
                        }
                        else
                        {
                            var nodeName = Regex.Replace(headerArray[cellCount - 1].ToString().Replace(" ", ""), "[^0-9a-zA-Z]+", "");
                            contentValueNode = xmlDoc.CreateElement(nodeName, ns);
                            contentValueNode.InnerText = cell.Text;

                            contentNode.AppendChild(contentValueNode);
                        }
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
            // Populate report
            PopulateGrid();

            // Create XLS
            var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Outstanding Visit Report");
            ws.View.ShowGridLines = true;

            // Write content
            var rowCount = 0;
            var cellCount = 0;

            foreach (TableRow row in dt_basic.Rows)
            {
                rowCount += 1;
                cellCount = 0;

                foreach (TableCell cell in row.Cells)
                {
                    int[] ignore = { };

                    if (!ignore.Contains(cellCount))
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
            Response.AddHeader("content-disposition", String.Format("attachment;filename=ReportOutstandingVisit_{0}.xlsx", DateTime.Now.ToString("yyyyMMddhhmm")));
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

        private string FormatXML(XmlDocument doc)
        {
            MemoryStream memoryStream = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.Encoding = new UTF8Encoding(false);
            using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
            {
                doc.Save(writer);
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray());
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
