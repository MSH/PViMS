using System;
using System.Collections;
using System.Drawing;

using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
using PVIMS.Core.Services;

namespace PVIMS.Web
{
    public partial class ReportAdverseEventAnnual : MainPageBase
    {
        public IReportService _reportService { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            txtSearchFrom.Value = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            txtSearchTo.Value = DateTime.Today.ToString("yyyy-MM-dd");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("ReportAdverseEventa");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Report - Annual Adverse Events", SubTitle = "", Icon = "fa fa-bar-chart-o fa-fw" });
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            string searchfrom = txtSearchFrom.Value;
            string searchto = txtSearchTo.Value;

            bool err = false;

            divError.Visible = false;
            spnNoRows.Visible = false;
            spnRows.Visible = false;

            lblSearchFrom.Attributes.Remove("class");
            lblSearchFrom.Attributes.Add("class", "input");
            lblSearchTo.Attributes.Remove("class");
            lblSearchTo.Attributes.Add("class", "input");

            DateTime dttemp;

            if (!String.IsNullOrWhiteSpace(searchfrom) && !String.IsNullOrWhiteSpace(searchto))
            {
                if (DateTime.TryParse(searchfrom, out dttemp))
                {
                    dttemp = Convert.ToDateTime(searchfrom);
                    if (dttemp > DateTime.Today)
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From cannot be after current date";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From cannot be so far in the past";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblSearchFrom.Attributes.Remove("class");
                    lblSearchFrom.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Search From has an invalid date format";
                    lblSearchFrom.Controls.Add(errorMessageDiv);

                    err = true;
                }

                if (DateTime.TryParse(searchto, out dttemp))
                {
                    dttemp = Convert.ToDateTime(searchto);
                    if (dttemp > DateTime.Today)
                    {
                        lblSearchTo.Attributes.Remove("class");
                        lblSearchTo.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search To cannot be after current date";
                        lblSearchTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblSearchTo.Attributes.Remove("class");
                        lblSearchTo.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search To cannot be so far in the past";
                        lblSearchTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblSearchTo.Attributes.Remove("class");
                    lblSearchTo.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Search To has an invalid date format";
                    lblSearchTo.Controls.Add(errorMessageDiv);

                    err = true;
                }

                if (DateTime.TryParse(searchfrom, out dttemp) && DateTime.TryParse(searchto, out dttemp))
                {
                    if (Convert.ToDateTime(searchfrom) > Convert.ToDateTime(searchto))
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From must be before Search To";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        lblSearchTo.Attributes.Remove("class");
                        lblSearchTo.Attributes.Add("class", "input state-error");
                        errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search To must be after Search From";
                        lblSearchTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
            }

            if (err)
            {
                divError.Visible = true;
                return;
            };

            var results = _reportService.GetAdverseEventAnnualItems(Convert.ToDateTime(searchfrom), Convert.ToDateTime(searchto));

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

            TableRow row = null;
            TableCell cell = null;

            if (results.Count > 0)
            {
                int? storeYear = 0;
                var storeFacilityName = string.Empty;
                var storeMeddra = string.Empty;

                foreach (AdverseEventAnnualList item in results)
                {
                    if (storeYear != item.PeriodYear || storeFacilityName != item.FacilityName || storeMeddra != item.MedDraTerm)
                    {
                        row = new TableRow();

                        storeYear = item.PeriodYear;
                        storeFacilityName = item.FacilityName;
                        storeMeddra = item.MedDraTerm;

                        cell = new TableCell();
                        cell.Text = item.MedDraTerm;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = item.PeriodYear != null ? String.Format("Year {0}", item.PeriodYear) : "";
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = item.FacilityName;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = "0";
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = "0";
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = "0";
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = "0";
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = item.PatientCount.ToString();
                        row.Cells.Add(cell);

                        dt_basic.Rows.Add(row);
                    }

                    var i = GetColumnCount(item.SeverityGrade);
                    cell = row.Cells[i];
                    cell.Text = item.PatientCount.ToString();

                }
            }
        }

        #region "Export"

        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            // Populate report
            PopulateGrid();

            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var logoDirectory = String.Format("{0}\\img\\", System.AppDomain.CurrentDomain.BaseDirectory);

            string destName = string.Format("RAE_{0}.pdf", DateTime.Now.ToString("yyyyMMddhhmmsss"));
            string destFile = string.Format("{0}{1}", documentDirectory, destName);

            string logoName = string.Format("SIAPS_USAID_Horiz.png");
            string logoFile = string.Format("{0}{1}", logoDirectory, logoName);

            string fontFile = string.Format("{0}\\arial.ttf", System.AppDomain.CurrentDomain.BaseDirectory);

            var linePosition = 60;
            var columnPosition = 30;

            // Create document
            PdfDocument pdfDoc = new PdfDocument();
            XmlNode rootNode;
            XmlNode filterNode;
            XmlNode contentHeadNode;
            XmlNode contentNode;
            XmlNode contentValueNode;
            XmlAttribute attrib;
            XmlComment comment;

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
            pdfDoc.Info.Title = "Adverse Event Report for " + DateTime.Now.ToString("yyyy-MM-dd hh:MM");
            gfx.DrawString("Adverse Event Report for " + DateTime.Now.ToString("yyyy-MM-dd hh:MM"), fontb, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);

            // Write filter
            linePosition += 24;
            gfx.DrawString("Filter", fontb, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);
            linePosition += 24;
            gfx.DrawString("Range From : " + txtSearchFrom.Value, fontr, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);
            linePosition += 24;
            gfx.DrawString("Range To : " + txtSearchTo.Value, fontr, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);

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
                    
                    gfx.DrawString("Adverse Event Report (Page " + pageCount.ToString() + ")", fontb, XBrushes.Black, new XRect(columnPosition, linePosition, page.Width.Point, 20), XStringFormats.TopLeft);

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
            string destName = string.Format("RAE_{0}.xml", DateTime.Now.ToString("yyyyMMddhhmmsss"));
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

            rootNode = xmlDoc.CreateElement("PViMS_AdverseEventsReport", ns);
            attrib = xmlDoc.CreateAttribute("CreatedDate");
            attrib.InnerText = DateTime.Now.ToString("yyyy-MM-dd hh:MM");
            rootNode.Attributes.Append(attrib);

            // Write filter
            filterNode = xmlDoc.CreateElement("Filter", ns);
            attrib = xmlDoc.CreateAttribute("RangeFrom");
            attrib.InnerText = txtSearchFrom.Value;
            filterNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("RangeTo");
            attrib.InnerText = txtSearchTo.Value;
            filterNode.Attributes.Append(attrib);
            rootNode.AppendChild(filterNode);

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
            var ws = pck.Workbook.Worksheets.Add("Adverse Event Report");
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
            Response.AddHeader("content-disposition", String.Format("attachment;filename=ReportAdverseEvent_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm")));
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

        #region "EF"

        #endregion

        private int GetColumnCount(string grade)
        {
            switch (grade)
            {
                case "Grade 1":
                    return 2;

                case "Grade 2":
                    return 3;

                case "Grade 3":
                    return 4;

                case "Grade 4":
                    return 5;

                case "Grade 5":
                    return 6;

                default:
                    return 7;
            }
        }
    }
}