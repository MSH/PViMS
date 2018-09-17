using System;
using System.Collections;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class Analyser : MainPageBase
    {
        private int _pid;
        private Patient _patient;
        private int _aid;
        private DataTable _factors;

        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3 };
        private FormMode _formMode = FormMode.ViewMode;

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Analyser", SubTitle = "", Icon = "fa fa-dashboard fa-fw", MetaPageId = 0 });

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Factor" });
            dt.Columns.Add(new DataColumn() { ColumnName = "Option" });
            ViewState["Factors"] = dt;

            PopulateFactors();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtSearchFrom.Value = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                txtSearchTo.Value = DateTime.Today.ToString("yyyy-MM-dd");

                var conditions = UnitOfWork.Repository<Condition>().Queryable().OrderByDescending(cg => cg.Description).ToList();
                foreach (var condition in conditions)
                {
                    ListItem listItem = new ListItem() { Text = condition.Description, Value = condition.Id.ToString() };
                    ddlCondition.Items.Add(listItem);
                }

                var cohorts = UnitOfWork.Repository<CohortGroup>().Queryable().OrderBy(cg => cg.CohortName).ToList();
                foreach (var cohort in cohorts)
                {
                    ListItem listItem = new ListItem() { Text = cohort.CohortName, Value = cohort.Id.ToString() };
                    ddlCohort.Items.Add(listItem);
                }

                divChart1Empty.Visible = true;
                divChart2Empty.Visible = true;
                divChart1.Visible = false;
                divChart2.Visible = false;

                User user = UnitOfWork.Repository<User>().Queryable().Single(u => u.UserName == HttpContext.Current.User.Identity.Name);
                divDownload.Visible = user.AllowDatasetDownload;
            }
            if (ViewState["Factors"] != null) 
            { 
                _factors = (DataTable)ViewState["Factors"];
                RenderFactors();
            };

            Master.SetMenuActive("AnalyserView");
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            divTerms.Visible = false;
            try
            {
                var results = UnitOfWork.Repository<ContingencyAnalysisList>().ExecuteSql("GenerateContingencyAnalysis @StartDate, @FinishDate, @TermID, @ReferenceRegimenID, @IncludeRiskFactor, @RateByCount, @DebugPatientList, @DebugPatientListExposed, @RiskFactorXml", new SqlParameter("@StartDate", txtSearchFrom.Value), new SqlParameter("@FinishDate", txtSearchTo.Value), new SqlParameter("@TermID", "0"), new SqlParameter("@ReferenceRegimenID", "0"), new SqlParameter("@IncludeRiskFactor", "False"), new SqlParameter("@RateByCount", "False"), new SqlParameter("@DebugPatientList", "False"), new SqlParameter("@DebugPatientListExposed", "False"), new SqlParameter("@RiskFactorXml", ""));

                spnRows.InnerText = (results.Count).ToString() + " row(s) matching criteria found...";
                divRows.Visible = true;

                if (results.Count > 0)
                {
                    ListItem item;
                    ddlReactions.Items.Clear();
                    item = new ListItem();
                    item.Text = "";
                    item.Value = "0";
                    ddlReactions.Items.Add(item);
                    foreach (ContingencyAnalysisList analysisItem in results)
                    {
                        item = new ListItem();
                        item.Text = analysisItem.MeddraTerm;
                        item.Value = analysisItem.TerminologyMeddra_Id.ToString();
                        ddlReactions.Items.Add(item);
                    }
                    divTerms.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblCustomError.Attributes.Remove("class");
                lblCustomError.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = ex.Message;
                lblCustomError.Controls.Add(errorMessageDiv);
            }
        }

        #region "Events"

        protected void btnAnalyse_Click(object sender, EventArgs e)
        {
            bool err = false;

            string searchfrom = txtSearchFrom.Value;
            string searchto = txtSearchTo.Value;

            lblCondition.Attributes.Remove("class");
            lblCondition.Attributes.Add("class", "input");
            lblCohort.Attributes.Remove("class");
            lblCohort.Attributes.Add("class", "input");
            lblSearchFrom.Attributes.Remove("class");
            lblSearchFrom.Attributes.Add("class", "input");
            lblSearchTo.Attributes.Remove("class");
            lblSearchTo.Attributes.Add("class", "input");

            DateTime dttemp;

            if (ddlCondition.SelectedValue == "0" && ddlCohort.SelectedValue == "0")
            {
                lblCondition.Attributes.Remove("class");
                lblCondition.Attributes.Add("class", "input state-error");
                lblCohort.Attributes.Remove("class");
                lblCohort.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Primary Condition Group Risk Factor or Cohort must be selected";
                lblCondition.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (ddlCondition.SelectedValue != "0" && ddlCohort.SelectedValue != "0")
            {
                lblCondition.Attributes.Remove("class");
                lblCondition.Attributes.Add("class", "input state-error");
                lblCohort.Attributes.Remove("class");
                lblCohort.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Primary Condition Group Risk Factor or Cohort must be selected";
                lblCondition.Controls.Add(errorMessageDiv);

                err = true;
            }

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
                return;
            };

            divTerms.Visible = false;

            // prepare risk factor xml
            string riskFactorXml = "";
            string includeRiskFactor = "False";

            XmlDocument xmlDoc = new XmlDocument();
            if (_factors.Rows.Count > 0)
            {
                XmlNode rootNode = xmlDoc.CreateElement("Factors", "");

                foreach (DataRow dtRow in _factors.Rows)
                {
                    XmlNode factorNode = xmlDoc.CreateElement("Factor", "");

                    XmlNode factorChildNode = xmlDoc.CreateElement("Name", "");
                    factorChildNode.InnerText = dtRow[0].ToString();
                    factorNode.AppendChild(factorChildNode);

                    factorChildNode = xmlDoc.CreateElement("Option", "");
                    factorChildNode.InnerText = dtRow[1].ToString();
                    factorNode.AppendChild(factorChildNode);

                    rootNode.AppendChild(factorNode);
                }

                xmlDoc.AppendChild(rootNode);

                riskFactorXml = xmlDoc.InnerXml;
                includeRiskFactor = "True";
            }

            try
            {
                var results = UnitOfWork.Repository<ContingencyAnalysisList>().ExecuteSql("spGenerateAnalysis @ConditionId, @CohortId, @StartDate, @FinishDate, @TermID, @IncludeRiskFactor, @RateByCount, @DebugPatientList, @RiskFactorXml, @DebugMode", new SqlParameter("@ConditionId", ddlCondition.SelectedValue), new SqlParameter("@CohortId", ddlCohort.SelectedValue), new SqlParameter("@StartDate", txtSearchFrom.Value), new SqlParameter("@FinishDate", txtSearchTo.Value), new SqlParameter("@TermID", "0"), new SqlParameter("@IncludeRiskFactor", includeRiskFactor), new SqlParameter("@RateByCount", "True"), new SqlParameter("@DebugPatientList", "False"), new SqlParameter("@RiskFactorXml", riskFactorXml), new SqlParameter("@DebugMode", "False"));

                spnRows.InnerText = (results.Count).ToString() + " row(s) matching criteria found...";
                divRows.Visible = true;

                if (results.Count > 0)
                {
                    ListItem item;
                    ddlReactions.Items.Clear();
                    item = new ListItem();
                    item.Text = "-- Please select a reaction --";
                    item.Value = "0";
                    ddlReactions.Items.Add(item);
                    foreach (ContingencyAnalysisList analysisItem in results)
                    {
                        item = new ListItem();
                        item.Text = analysisItem.MeddraTerm;
                        item.Value = analysisItem.TerminologyMeddra_Id.ToString();
                        ddlReactions.Items.Add(item);
                    }
                    divTerms.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblCustomError.Attributes.Remove("class");
                lblCustomError.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = ex.Message;
                lblCustomError.Controls.Add(errorMessageDiv);
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            bool found = false;
            var delete = new ArrayList();
            foreach (DataRow temprow in _factors.Rows)
            {
                if (temprow[0].ToString() == ((Button)sender).ToolTip)
                {
                    delete.Add(temprow);
                    break;
                }
            }
            foreach (DataRow temprow in delete)
            {
                _factors.Rows.Remove(temprow);
            }
            RenderFactors();
        }

        protected void btnAddFactor_Click(object sender, EventArgs e)
        {
            if (ddlRiskFactor.SelectedValue == "0" || ddlFactorOption.SelectedValue == "0")
            {
                return;
            }

            // Has item been added already
            bool found = false;
            foreach (TableRow temprow in dt_2.Rows)
            {
                if (temprow.Cells[0].Text != "Factor")
                {
                    if (ddlRiskFactor.SelectedItem.Text == temprow.Cells[0].Text)
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                var dtRow = _factors.NewRow();

                dtRow[0] = ddlRiskFactor.SelectedItem.Text;
                dtRow[1] = ddlFactorOption.SelectedItem.Text;

                _factors.Rows.Add(dtRow);
                ViewState["Factors"] = _factors;
                RenderFactors();
            }
        }

        protected void ddlCohort_SelectedIndexChanged(object sender, EventArgs e)
        {
            divCohort.Visible = false;

            if (ddlCohort.SelectedValue == "0") { return; };

            var id = Convert.ToInt32(ddlCohort.SelectedValue);
            var cohort = UnitOfWork.Repository<CohortGroup>().Queryable().SingleOrDefault(cg => cg.Id == id);
            if (cohort == null) { return; };

            divCohort.Visible = true;
            string cohortDetail = "<ul>";

            cohortDetail += String.Format("<li>Primary Condition Group: <b>{0}</b></li>", cohort.Condition != null ? cohort.Condition.Description : "Not defined");
            cohortDetail += String.Format("<li>Date Range: <b>{0} - {1}</b></li>", cohort.StartDate.ToString("yyyy-MM-dd"), cohort.FinishDate != null ? Convert.ToDateTime(cohort.FinishDate).ToString("yyyy-MM-dd") : "No End Date");
            cohortDetail += String.Format("<li>Number Patients: <b>{0}</b></li>", cohort.CohortGroupEnrolments.Count.ToString());

            cohortDetail += "</ul>";
            spnCohort.InnerHtml = cohortDetail;

            SetActiveCriteria("tab0");
        }

        protected void ddlReactions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlReactions.SelectedValue == "0") { return; };

            RenderResults();
            RenderPatients();
        }

        protected void ddlRiskFactor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRiskFactor.SelectedValue == "0") { return; };

            var id = Convert.ToInt32(ddlRiskFactor.SelectedValue);
            var factor = UnitOfWork.Repository<RiskFactor>().Queryable().SingleOrDefault(rf => rf.Id == id);
            if (factor == null) { return; };

            ddlFactorOption.Items.Clear();
            ddlFactorOption.Items.Add(new ListItem() { Text = "-- Please select an option --", Value = "0" });

            foreach(RiskFactorOption option in factor.Options)
            {
                ddlFactorOption.Items.Add(new ListItem() { Text = option.Display, Value = option.Id.ToString() });
            }

            SetActiveCriteria("tab2");
        }

        #endregion

        #region "Render"

        private void SetActiveCriteria(string tab)
        {
            liCondition.Attributes["class"] = "";
            liCriteriaRange.Attributes["class"] = "";
            liRiskFactors.Attributes["class"] = "";

            tab0.Attributes["class"] = "tab-pane smart-form";
            tab1.Attributes["class"] = "tab-pane smart-form";
            tab2.Attributes["class"] = "tab-pane";

            switch (tab)
            {
                case "tab0":
                    tab0.Attributes["class"] = "tab-pane active smart-form";
                    liCondition.Attributes["class"] = "active";
                    break;

                case "tab1":
                    tab1.Attributes["class"] = "tab-pane active smart-form";
                    liCriteriaRange.Attributes["class"] = "active";
                    break;

                case "tab2":
                    tab2.Attributes["class"] = "tab-pane active";
                    liRiskFactors.Attributes["class"] = "active";
                    break;

                default:
                    break;
            }
        }

        private void PopulateFactors()
        {
            foreach (RiskFactor factor in UnitOfWork.Repository<RiskFactor>().Queryable().Where(rf => rf.Active == true).OrderBy(rf => rf.FactorName))
            {
                ddlRiskFactor.Items.Add(new ListItem() { Value = factor.Id.ToString(), Text = factor.Display });
            }
        }

        private void RenderFactors()
        {
            TableRow row;
            TableCell cell;
            Button btn;

            var count = 0;

            var delete = new ArrayList();
            foreach (TableRow temprow in dt_2.Rows)
            {
                if (temprow.Cells[0].Text != "Factor")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_2.Rows.Remove(temprow);
            }

            foreach (DataRow dtRow in _factors.Rows)
            {
                count += 1;
                row = new TableRow();

                cell = new TableCell();
                cell.Text = dtRow[0].ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = dtRow[1].ToString();
                row.Cells.Add(cell);

                btn = new Button();
                btn.ID = "btnRemove_" + count.ToString();
                btn.CssClass = "btn btn-default";
                btn.Text = "Remove";
                btn.Click += btnRemove_Click;
                btn.ToolTip = dtRow[0].ToString();
                cell = new TableCell();
                cell.Controls.Add(btn);
                row.Cells.Add(cell);

                dt_2.Rows.Add(row);
            }
        }

        private void RenderResults()
        {
            TableRow row;
            TableCell cell;

            Button btn;

            // prepare risk factor xml
            string riskFactorXml = "";
            string includeRiskFactor = "False";

            XmlDocument xmlDoc = new XmlDocument();
            if (_factors.Rows.Count > 0)
            {
                XmlNode rootNode = xmlDoc.CreateElement("Factors", "");

                foreach (DataRow dtRow in _factors.Rows)
                {
                    XmlNode factorNode = xmlDoc.CreateElement("Factor", "");

                    XmlNode factorChildNode = xmlDoc.CreateElement("Name", "");
                    factorChildNode.InnerText = dtRow[0].ToString();
                    factorNode.AppendChild(factorChildNode);

                    factorChildNode = xmlDoc.CreateElement("Option", "");
                    factorChildNode.InnerText = dtRow[1].ToString();
                    factorNode.AppendChild(factorChildNode);

                    rootNode.AppendChild(factorNode);
                }

                xmlDoc.AppendChild(rootNode);

                riskFactorXml = xmlDoc.InnerXml;
                includeRiskFactor = "True";
            }

            try
            {
                var results = UnitOfWork.Repository<ContingencyAnalysisItem>().ExecuteSql("spGenerateAnalysis @ConditionId, @CohortId, @StartDate, @FinishDate, @TermID, @IncludeRiskFactor, @RateByCount, @DebugPatientList, @RiskFactorXml, @DebugMode", new SqlParameter("@ConditionId", ddlCondition.SelectedValue), new SqlParameter("@CohortId", ddlCohort.SelectedValue), new SqlParameter("@StartDate", txtSearchFrom.Value), new SqlParameter("@FinishDate", txtSearchTo.Value), new SqlParameter("@TermID", ddlReactions.SelectedValue), new SqlParameter("@IncludeRiskFactor", includeRiskFactor), new SqlParameter("@RateByCount", "True"), new SqlParameter("@DebugPatientList", "False"), new SqlParameter("@RiskFactorXml", riskFactorXml), new SqlParameter("@DebugMode", "False"));

                if (results.Count > 0)
                {
                    divChart1Empty.Visible = false;
                    divChart2Empty.Visible = false;

                    divChart1.Visible = true;
                    divChart2.Visible = true;

                    chtAdverse.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                    chtAdverse.ChartAreas[0].AxisX.Interval = 1;
                    chtAdverse.ChartAreas[0].AxisY.Interval = 1;

                    chtRelative.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                    chtRelative.ChartAreas[0].AxisX.Interval = 1;

                    var count = 0;

                    foreach (ContingencyAnalysisItem analysisItem in results)
                    {
                        count += 1;
                        row = new TableRow();

                        cell = new TableCell();
                        cell.Text = analysisItem.Drug;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.ExposedCases.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.ExposedNonCases.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.ExposedPopulation.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.ExposedIncidenceRate.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.NonExposedCases.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.NonExposedNonCases.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.NonExposedPopulation.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.NonExposedIncidenceRate.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.UnadjustedRelativeRisk > 1 ? @"<span class=""badge bg-color-red pull-right"">" + analysisItem.UnadjustedRelativeRisk.ToString() + "</span>" : analysisItem.UnadjustedRelativeRisk.ToString();
                        row.Cells.Add(cell);

                        if (_factors.Rows.Count > 0)
                        {
                            cell = new TableCell();
                            cell.Text = analysisItem.AdjustedRelativeRisk > 1 ? @"<span class=""badge bg-color-red pull-right"">" + analysisItem.AdjustedRelativeRisk.ToString() + "</span>" : analysisItem.AdjustedRelativeRisk.ToString();
                            row.Cells.Add(cell);
                        }
                        else
                        {
                            cell = new TableCell();
                            cell.Text = "N/A";
                            row.Cells.Add(cell);
                        }

                        cell = new TableCell();
                        cell.Text = String.Format("{0} ~ {1}", analysisItem.ConfidenceIntervalLow.ToString(), analysisItem.ConfidenceIntervalHigh.ToString());
                        row.Cells.Add(cell);

                        dt_1.Rows.Add(row);

                        // Update chart
                        chtAdverse.Series["AdverseEvents"].Points.AddXY(analysisItem.Drug, analysisItem.ExposedCases);
                        chtRelative.Series["RelativeRisks"].Points.AddXY(analysisItem.Drug, analysisItem.UnadjustedRelativeRisk);

                        chtAdverse.Series["AdverseEvents"]["DrawingStyle"] = "Cylinder";
                        chtRelative.Series["RelativeRisks"]["DrawingStyle"] = "Cylinder";
                    }
                }
                else
                {
                    divChart1Empty.Visible = true;
                    divChart2Empty.Visible = true;
                    divChart1.Visible = false;
                    divChart2.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblCustomError.Attributes.Remove("class");
                lblCustomError.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = ex.Message;
                lblCustomError.Controls.Add(errorMessageDiv);
            }
        }

        private void RenderPatients()
        {
            TableRow row;
            TableCell cell;

            Button btn;

            // prepare risk factor xml
            string riskFactorXml = "";
            string includeRiskFactor = "False";

            XmlDocument xmlDoc = new XmlDocument();
            if (_factors.Rows.Count > 0)
            {
                XmlNode rootNode = xmlDoc.CreateElement("Factors", "");

                foreach (DataRow dtRow in _factors.Rows)
                {
                    XmlNode factorNode = xmlDoc.CreateElement("Factor", "");

                    XmlNode factorChildNode = xmlDoc.CreateElement("Name", "");
                    factorChildNode.InnerText = dtRow[0].ToString();
                    factorNode.AppendChild(factorChildNode);

                    factorChildNode = xmlDoc.CreateElement("Option", "");
                    factorChildNode.InnerText = dtRow[1].ToString();
                    factorNode.AppendChild(factorChildNode);

                    rootNode.AppendChild(factorNode);
                }

                xmlDoc.AppendChild(rootNode);

                riskFactorXml = xmlDoc.InnerXml;
                includeRiskFactor = "True";
            }

            try
            {
                var results = UnitOfWork.Repository<ContingencyAnalysisPatient>().ExecuteSql("spGenerateAnalysis @ConditionId, @CohortId, @StartDate, @FinishDate, @TermID, @IncludeRiskFactor, @RateByCount, @DebugPatientList, @RiskFactorXml, @DebugMode", new SqlParameter("@ConditionId", ddlCondition.SelectedValue), new SqlParameter("@CohortId", ddlCohort.SelectedValue), new SqlParameter("@StartDate", txtSearchFrom.Value), new SqlParameter("@FinishDate", txtSearchTo.Value), new SqlParameter("@TermID", ddlReactions.SelectedValue), new SqlParameter("@IncludeRiskFactor", includeRiskFactor), new SqlParameter("@RateByCount", "True"), new SqlParameter("@DebugPatientList", "True"), new SqlParameter("@RiskFactorXml", riskFactorXml), new SqlParameter("@DebugMode", "False"));

                if (results.Count > 0)
                {
                    var count = 0;

                    foreach (ContingencyAnalysisPatient analysisItem in results)
                    {
                        count += 1;
                        row = new TableRow();

                        cell = new TableCell();
                        cell.Text = analysisItem.PatientName;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.Drug;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.StartDate;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.FinishDate;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.DaysContributed.ToString();
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.ADR == 1 ? "Yes" : "No";
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.RiskFactor;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.RiskFactorOption;
                        row.Cells.Add(cell);

                        cell = new TableCell();
                        cell.Text = analysisItem.FactorMet;
                        row.Cells.Add(cell);

                        dt_basic.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                lblCustomError.Attributes.Remove("class");
                lblCustomError.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = ex.Message;
                lblCustomError.Controls.Add(errorMessageDiv);
            }
        }

        #endregion

    }
}