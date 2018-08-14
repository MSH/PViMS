using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using VPS.CustomAttributes;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ReportInstanceList : MainPageBase
    {
        private User _user;
        private int _id = 0;
        private WorkFlow _workFlow;
        private string _configValue = "";

        protected void Page_Init(object sender, EventArgs e)
        {
            txtSearchFrom.Value = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
            txtSearchTo.Value = DateTime.Today.ToString("yyyy-MM-dd");

            if (Request.QueryString["wuid"] != null)
            {
                Guid wuid = Guid.Parse(Request.QueryString["wuid"]);
                _workFlow = UnitOfWork.Repository<WorkFlow>().Queryable().Single(wf => wf.WorkFlowGuid == wuid);

                PreparePageForWorkFlow();
            }
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = UnitOfWork.Repository<User>().Queryable().Include(u => u.Facilities).SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);

            if (_id > 0)
            {
                LoadExistingEvent();
            }
            else
            {
                if (!Page.IsPostBack) { btnSubmit_Click(null, null); };
            }
        }

        private void PreparePageForWorkFlow()
        {
            if (_workFlow.Description == "New Active Surveilliance Report")
            {
                Master.SetMenuActive("ActiveReporting");
                Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Active Reports", SubTitle = "", Icon = "fa fa-bar-chart-o fa-fw", MetaPageId = 0 });
            }
            else
            {
                Master.SetMenuActive("SpontaneousReporting");
                Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Spontaneous Reports", SubTitle = "", Icon = "fa fa-bar-chart-o fa-fw", MetaPageId = 0 });
            }

            ListItem listItem = new ListItem()
            {
                Text = "All Reports",
                Value = "0"
            };
            ddlReportCriteria.Items.Add(listItem);
            foreach (var activity in _workFlow.Activities.OrderBy(a => a.Id))
            {
                listItem = new ListItem()
                {
                    Text = activity.QualifiedName,
                    Value = activity.QualifiedName
                };
                ddlReportCriteria.Items.Add(listItem);
            }
            ddlReportCriteria.SelectedIndex = 0;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool err = false;

            spnNoRows.Visible = false;
            spnRows.Visible = false;

            lblReportCriteria.Attributes.Remove("class");
            lblReportCriteria.Attributes.Add("class", "input");
            lblSearchFrom.Attributes.Remove("class");
            lblSearchFrom.Attributes.Add("class", "input");
            lblSearchTo.Attributes.Remove("class");
            lblSearchTo.Attributes.Add("class", "input");

            DateTime dttemp;

            if (!String.IsNullOrWhiteSpace(txtSearchFrom.Value) && !String.IsNullOrWhiteSpace(txtSearchTo.Value))
            {
                if (DateTime.TryParse(txtSearchFrom.Value, out dttemp))
                {
                    dttemp = Convert.ToDateTime(txtSearchFrom.Value);
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

                if (DateTime.TryParse(txtSearchTo.Value, out dttemp))
                {
                    dttemp = Convert.ToDateTime(txtSearchTo.Value);
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

                if (DateTime.TryParse(txtSearchFrom.Value, out dttemp) && DateTime.TryParse(txtSearchTo.Value, out dttemp))
                {
                    if (Convert.ToDateTime(txtSearchFrom.Value) > Convert.ToDateTime(txtSearchTo.Value))
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
            else
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

            if (err) { return; };

            var tsearchfrom = Convert.ToDateTime(txtSearchFrom.Value);
            var tsearchto = Convert.ToDateTime(txtSearchTo.Value).AddDays(1);
            IQueryable<ReportInstance> instanceQuery;

            if(ddlReportCriteria.SelectedValue == "0")
            {
                instanceQuery = UnitOfWork.Repository<ReportInstance>().Queryable().Include(ri => ri.WorkFlow).Include(ri => ri.Activities.Select(a => a.CurrentStatus)).Where(ri => ri.WorkFlow.Description == _workFlow.Description && ri.Created >= tsearchfrom && ri.Created <= tsearchto);
            }
            else
            {
                instanceQuery = UnitOfWork.Repository<ReportInstance>().Queryable().Include(ri => ri.WorkFlow).Include(ri => ri.Activities.Select(a => a.CurrentStatus)).Where(ri => ri.WorkFlow.Description == _workFlow.Description && ri.Created >= tsearchfrom && ri.Created <= tsearchto && ri.Activities.Any(a => a.QualifiedName == ddlReportCriteria.SelectedValue && a.Current == true));
            }

            // ConfigType.AssessmentScale
            _configValue = UnitOfWork.Repository<Config>().Queryable().Single(c => c.ConfigType == ConfigType.AssessmentScale).ConfigValue;

            foreach (var instance in instanceQuery.ToList())
            {
                PopulateGrid(instance);
            }

            if (dt_basic.Rows.Count == 1)
            {
                spnNoRows.InnerText = "No matching records found...";
                spnNoRows.Visible = true;
                spnRows.Visible = false;
            }
            else
            {
                spnRows.InnerText = (dt_basic.Rows.Count - 1).ToString() + " row(s) matching criteria found...";
                spnRows.Visible = true;
                spnNoRows.Visible = false;
            }
        }

        private string Escape(string uri)
        {
            return uri.Replace("&", "%26").Replace("=", "%3D").Replace("?", "%3F");
        }

        private void LoadExistingEvent()
        {
            spnNoRows.Visible = false;
            spnRows.Visible = false;

            // ConfigType.AssessmentScale
            _configValue = UnitOfWork.Repository<Config>().Queryable().Single(c => c.ConfigType == ConfigType.AssessmentScale).ConfigValue;

            var instance = UnitOfWork.Repository<ReportInstance>().Queryable().Include("WorkFlow").Single(ri => ri.Id == _id);
            PopulateGrid(instance);

            if (dt_basic.Rows.Count == 1)
            {
                spnNoRows.InnerText = "No matching records found...";
                spnNoRows.Visible = true;
                spnRows.Visible = false;
            }
            else
            {
                spnRows.InnerText = (dt_basic.Rows.Count - 1).ToString() + " row(s) matching criteria found...";
                spnRows.Visible = true;
                spnNoRows.Visible = false;
            }
        }

        private void PopulateGrid(ReportInstance instance)
        {
            TableRow row;
            TableCell cell;

            string[] validNaranjoCriteria = { "Possible", "Probable", "Definite" };
            string[] validWHOCriteria = { "Possible", "Probable", "Certain" };

            row = new TableRow();

            cell = new TableCell();
            cell.Text = instance.Created.ToString("yyyy-MM-dd HH:mm");
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = instance.Identifier;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = instance.PatientIdentifier;
            row.Cells.Add(cell);

            // Create an array of unique meds allocated to patient which are unassigned
            var causality = "";
            foreach (var medication in instance.Medications)
            {
                var whoOutput = medication.WhoCausality != null ? String.Format("<span class=\"badge txt-color-white bg-color-darken pull-right\" style=\"padding:5px;width:100px;\"> {0} </span>", medication.WhoCausality) : "";
                var naranjoOutput = medication.NaranjoCausality != null ? String.Format("<span class=\"badge txt-color-white bg-color-red pull-right\" style=\"padding:5px;width:100px;\"> {0} </span>", medication.NaranjoCausality) : "";

                causality += "<div class=\"row\" style=\"padding-left:10px; padding-right:10px; padding-top:5px; padding-bottom:5px;\">";
                causality += String.Format("<span style=\"width:100px;\"> {0} </span> {1} {2}", medication.MedicationIdentifier, naranjoOutput, whoOutput);
                causality += "</div>";
            };

            cell = new TableCell();
            cell.Text = causality;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = instance.SourceIdentifier;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = instance.TerminologyMedDra != null ? instance.TerminologyMedDra.MedDraTerm : "<span class=\"label label-info\">NOT SET</span>";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = instance.DisplayStatus;
            row.Cells.Add(cell);

            cell = new TableCell();
            if (instance.CurrentActivity.QualifiedName == "Confirm Report Data") { cell.Controls.Add(PrepareMenuItemsPanelForVerification(instance)); };
            if (instance.CurrentActivity.QualifiedName == "Set MedDRA and Causality") { cell.Controls.Add(PrepareMenuItemsPanelForTerminology(instance)); };
            if (instance.CurrentActivity.QualifiedName == "Extract E2B") { cell.Controls.Add(PrepareMenuItemsPanelForExtract(instance)); };
            row.Cells.Add(cell);

            dt_basic.Rows.Add(row);
        }

        private Panel PrepareMenuItemsPanelForVerification(ReportInstance reportInstance)
        {
            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;

            pnl = new Panel() { CssClass = "btn-group" };

            btn = new HtmlGenericControl("button");
            btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
            btn.Attributes.Add("data-toggle", "dropdown");
            btn.Controls.Add(new Label() { Text = "Action " });
            btn.Controls.Add(new Label() { CssClass = "caret" });
            pnl.Controls.Add(btn);

            ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("class", "dropdown-menu pull-right");

            PrepareHistoryMenuItems(ul, reportInstance);

            var currentActivityInstance = reportInstance.Activities.Single(a => a.Current == true);
            if(currentActivityInstance.CurrentStatus.Description == "UNCONFIRMED")
            {
                PrepareUnconfirmedMenuItems(ul, reportInstance, currentActivityInstance);
            }

            PreparePatientMenuItems(ul, reportInstance);

            pnl.Controls.Add(ul);

            return pnl;
        }

        private Panel PrepareMenuItemsPanelForTerminology(ReportInstance reportInstance)
        {
            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;

            pnl = new Panel() { CssClass = "btn-group" };

            btn = new HtmlGenericControl("button");
            btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
            btn.Attributes.Add("data-toggle", "dropdown");
            btn.Controls.Add(new Label() { Text = "Action " });
            btn.Controls.Add(new Label() { CssClass = "caret" });
            pnl.Controls.Add(btn);

            ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("class", "dropdown-menu pull-right");

            PrepareHistoryMenuItems(ul, reportInstance);
            PrepareMedDRAMenuItems(ul, reportInstance);

            var currentActivityInstance = reportInstance.Activities.Single(a => a.Current == true);
            if (currentActivityInstance.CurrentStatus.Description != "NOTSET")
            {
                PrepareCausalityMenuItems(ul, reportInstance, currentActivityInstance);
            }

            PreparePatientMenuItems(ul, reportInstance);

            pnl.Controls.Add(ul);

            return pnl;
        }

        private Panel PrepareMenuItemsPanelForExtract(ReportInstance reportInstance)
        {
            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;

            pnl = new Panel() { CssClass = "btn-group" };

            btn = new HtmlGenericControl("button");
            btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
            btn.Attributes.Add("data-toggle", "dropdown");
            btn.Controls.Add(new Label() { Text = "Action " });
            btn.Controls.Add(new Label() { CssClass = "caret" });
            pnl.Controls.Add(btn);

            ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("class", "dropdown-menu pull-right");

            PrepareHistoryMenuItems(ul, reportInstance);

            var currentActivityInstance = reportInstance.Activities.Single(a => a.Current == true);
            if (currentActivityInstance.CurrentStatus.Description == "NOTGENERATED" || currentActivityInstance.CurrentStatus.Description == "E2BSUBMITTED")
            {
                PrepareE2BInitiateMenuItems(ul, reportInstance);
            }
            if (currentActivityInstance.CurrentStatus.Description == "E2BINITIATED")
            {
                PrepareE2BGenerateMenuItems(ul, reportInstance);
            }
            if (currentActivityInstance.CurrentStatus.Description == "E2BGENERATED")
            {
                PrepareE2BSubmissionMenuItems(ul, reportInstance);
            }

            PreparePatientMenuItems(ul, reportInstance);

            pnl.Controls.Add(ul);

            return pnl;
        }

        private void PrepareCausalityMenuItems(HtmlGenericControl ul, ReportInstance reportInstance, ActivityInstance currentActivityInstance)
        {
            HyperLink hyp;
            HtmlGenericControl li;

            if (_configValue == "Both Scales" || _configValue == "WHO Scale")
            {
                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "CausalityWHO.aspx?rid=" + reportInstance.Id.ToString(),
                    Text = "WHO Causality"
                };
                li.Controls.Add(hyp);
                ul.Controls.Add(li);
            }

            if (_configValue == "Both Scales" || _configValue == "Naranjo Scale")
            {
                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "CausalityNaranjo.aspx?rid=" + reportInstance.Id.ToString(),
                    Text = "Naranjo Causality"
                };
                li.Controls.Add(hyp);
                ul.Controls.Add(li);
            }

            li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "divider");
            ul.Controls.Add(li);

            var causalityExecutionStatus = UnitOfWork.Repository<Activity>().Queryable().Single(a => a.QualifiedName == currentActivityInstance.QualifiedName && a.WorkFlow.Id == reportInstance.WorkFlow.Id).ExecutionStatuses.Single(es => es.Description == "CAUSALITYSET");
            li = new HtmlGenericControl("li");
            hyp = new HyperLink()
            {
                NavigateUrl = "/Activity/AddActivity?activityInstanceId=" + currentActivityInstance.Id.ToString() + "&activityExecutionStatusId=" + causalityExecutionStatus.Id.ToString(),
                Text = "Confirm Causality Set"
            };
            li.Controls.Add(hyp);
            ul.Controls.Add(li);

            li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "divider");
            ul.Controls.Add(li);
        }

        private void PrepareE2BInitiateMenuItems(HtmlGenericControl ul, ReportInstance reportInstance)
        {
            HyperLink hyp;
            HtmlGenericControl li;

            string url = string.Empty;

            if (_workFlow.Description == "New Active Surveilliance Report")
            {
                var evt = UnitOfWork.Repository<PatientClinicalEvent>().Queryable().Single(pce => pce.PatientClinicalEventGuid == reportInstance.ContextGuid);
                url = "/E2b/AddE2bActive?contextGuid=" + evt.PatientClinicalEventGuid.ToString();
            }
            else
            {
                var sourceInstance = UnitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);
                url = "/E2b/AddE2bSpontaneous?contextGuid=" + sourceInstance.DatasetInstanceGuid.ToString();
            }

            var config = UnitOfWork.Repository<Config>().Queryable().Where(c => c.ConfigType == ConfigType.E2BVersion).SingleOrDefault();
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = url,
                        Text = "Create E2B"
                    };
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);
                }
            }

            li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "divider");
            ul.Controls.Add(li);

        }

        private void PrepareE2BGenerateMenuItems(HtmlGenericControl ul, ReportInstance reportInstance)
        {
            HyperLink hyp;
            HtmlGenericControl li;

            DatasetInstance datasetInstance = null;

            var currentActivityInstance = reportInstance.Activities.Single(a => a.Current == true);
            var evt = currentActivityInstance.ExecutionEvents.OrderByDescending(ee => ee.EventDateTime).First(ee => ee.ExecutionStatus.Id == currentActivityInstance.CurrentStatus.Id);

            var tag = (_workFlow.Description == "New Active Surveilliance Report") ? "Active" : "Spontaneous";

            datasetInstance
                = UnitOfWork.Repository<DatasetInstance>()
            .Queryable()
            .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
            .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
            .Where(di => di.Tag == tag
                && di.ContextID == evt.Id).SingleOrDefault();

            if (datasetInstance != null)
            {
                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "/E2b/EditE2b?datasetInstanceId=" + datasetInstance.Id.ToString(),
                    Text = "Update E2B"
                };
                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                var e2bgenExecutionStatus = UnitOfWork.Repository<Activity>().Queryable().Single(a => a.QualifiedName == reportInstance.CurrentActivity.QualifiedName && a.WorkFlow.Id == reportInstance.WorkFlow.Id).ExecutionStatuses.Single(es => es.Description == "E2BGENERATED");
                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "/Activity/AddActivity?activityInstanceId=" + reportInstance.CurrentActivity.Id.ToString() + "&activityExecutionStatusId=" + e2bgenExecutionStatus.Id.ToString(),
                    Text = "Prepare Report for E2B Submission"
                };
                li.Controls.Add(hyp);
                ul.Controls.Add(li);
            }

            li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "divider");
            ul.Controls.Add(li);

        }

        private void PrepareE2BSubmissionMenuItems(HtmlGenericControl ul, ReportInstance reportInstance)
        {
            HyperLink hyp;
            HtmlGenericControl li;

            var e2bSubExecutionStatus = UnitOfWork.Repository<Activity>().Queryable().Single(a => a.QualifiedName == reportInstance.CurrentActivity.QualifiedName && a.WorkFlow.Id == reportInstance.WorkFlow.Id).ExecutionStatuses.Single(es => es.Description == "E2BSUBMITTED");
            li = new HtmlGenericControl("li");
            hyp = new HyperLink()
            {
                NavigateUrl = "/Activity/AddActivity?activityInstanceId=" + reportInstance.CurrentActivity.Id.ToString() + "&activityExecutionStatusId=" + e2bSubExecutionStatus.Id.ToString(),
                Text = "Confirm E2B Submission"
            };
            li.Controls.Add(hyp);
            ul.Controls.Add(li);

            li = new HtmlGenericControl("li");
            var e2batt = reportInstance.CurrentActivity.ExecutionEvents.OrderByDescending(ee => ee.EventDateTime).First(ee => ee.ExecutionStatus.Description == "E2BGENERATED").Attachments.SingleOrDefault(att => att.Description == "E2b");
            if (e2batt != null)
            {
                hyp = new HyperLink()
                {
                    NavigateUrl = "/FileDownload/DownloadSingleAttachment?attid=" + e2batt.Id,
                    Text = "Download XML"
                };
                li.Controls.Add(hyp);
                ul.Controls.Add(li);
            }

            li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "divider");
            ul.Controls.Add(li);
        }

        private void PrepareHistoryMenuItems(HtmlGenericControl ul, ReportInstance reportInstance)
        {
            HyperLink hyp;
            HtmlGenericControl li;

            li = new HtmlGenericControl("li");
            hyp = new HyperLink()
            {
                NavigateUrl = "/Activity/Index?reportInstanceId=" + reportInstance.Id.ToString(),
                Text = "View Activity History"
            };
            li.Controls.Add(hyp);
            ul.Controls.Add(li);

            li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "divider");
            ul.Controls.Add(li);
        }

        private void PrepareMedDRAMenuItems(HtmlGenericControl ul, ReportInstance reportInstance)
        {
            HyperLink hyp;
            HtmlGenericControl li;

            li = new HtmlGenericControl("li");
            hyp = new HyperLink()
            {
                NavigateUrl = "TerminologyMedDRA.aspx?rid=" + reportInstance.Id.ToString(),
                Text = "Set Terminology"
            };
            li.Controls.Add(hyp);
            ul.Controls.Add(li);

            li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "divider");
            ul.Controls.Add(li);
        }

        private void PrepareUnconfirmedMenuItems(HtmlGenericControl ul, ReportInstance reportInstance, ActivityInstance currentActivityInstance)
        {
            HyperLink hyp;
            HtmlGenericControl li;

            var confirmExecutionStatus = UnitOfWork.Repository<Activity>().Queryable().Single(a => a.QualifiedName == currentActivityInstance.QualifiedName && a.WorkFlow.Id == reportInstance.WorkFlow.Id).ExecutionStatuses.Single(es => es.Description == "CONFIRMED");
            li = new HtmlGenericControl("li");
            hyp = new HyperLink()
            {
                NavigateUrl = "/Activity/AddActivity?activityInstanceId=" + currentActivityInstance.Id.ToString() + "&activityExecutionStatusId=" + confirmExecutionStatus.Id.ToString(),
                Text = "Confirm Report"
            };
            li.Controls.Add(hyp);
            ul.Controls.Add(li);

            var deleteExecutionStatus = UnitOfWork.Repository<Activity>().Queryable().Single(a => a.QualifiedName == currentActivityInstance.QualifiedName && a.WorkFlow.Id == reportInstance.WorkFlow.Id).ExecutionStatuses.Single(es => es.Description == "DELETED");
            li = new HtmlGenericControl("li");
            hyp = new HyperLink()
            {
                NavigateUrl = "/Activity/AddActivity?activityInstanceId=" + currentActivityInstance.Id.ToString() + "&activityExecutionStatusId=" + deleteExecutionStatus.Id.ToString(),
                Text = "Delete Report"
            };
            li.Controls.Add(hyp);
            ul.Controls.Add(li);

            li = new HtmlGenericControl("li");
            li.Attributes.Add("class", "divider");
            ul.Controls.Add(li);
        }

        private void PreparePatientMenuItems(HtmlGenericControl ul, ReportInstance reportInstance)
        {
            HyperLink hyp;
            HtmlGenericControl li;

            if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
            {
                var evt = UnitOfWork.Repository<PatientClinicalEvent>().Queryable().Single(pce => pce.PatientClinicalEventGuid == reportInstance.ContextGuid);
                var extendable = (IExtendable)evt;
                var extendableValue = extendable.GetAttributeValue("Is the adverse event serious?");
                var isSerious = extendableValue != null ? extendableValue.ToString() == "1" ? true : false : false;

                if (HttpContext.Current.User.IsInRole("RegClerk") || HttpContext.Current.User.IsInRole("DataCap") || HttpContext.Current.User.IsInRole("Clinician"))
                {
                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/Patient/PatientView.aspx?pId=" + evt.Patient.Id.ToString(),
                        Text = "View Patient"
                    };
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);
                }

                li = new HtmlGenericControl("li");
                if (isSerious)
                {
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/FileDownload/DownloadPatientSummaryForActiveReport?contextGuid=" + evt.PatientClinicalEventGuid.ToString() + "&isSerious=true",
                        Text = "View SAE Report"
                    };
                    li.Controls.Add(hyp);
                }
                else
                {
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/FileDownload/DownloadPatientSummaryForActiveReport?contextGuid=" + evt.PatientClinicalEventGuid.ToString() + "&isSerious=false",
                        Text = "View Patient Summary"
                    };
                    li.Controls.Add(hyp);
                }
                ul.Controls.Add(li);
            }
            else
            {
                var datasetInstance = UnitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);
                var isSerious = datasetInstance.GetInstanceValue(UnitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "302C07C9-B0E0-46AB-9EF8-5D5C2F756BF1"));

                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "/E2b/EditSpontaneous?datasetInstanceId=" + datasetInstance.Id.ToString(),
                    Text = "Update Report"
                };
                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                li = new HtmlGenericControl("li");
                if(!String.IsNullOrWhiteSpace(isSerious))
                {
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/FileDownload/DownloadPatientSummaryForSpontaneousReport?contextGuid=" + datasetInstance.DatasetInstanceGuid.ToString() + "&isSerious=true",
                        Text = "View SAE Report"
                    };
                    li.Controls.Add(hyp);
                }
                else
                {
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/FileDownload/DownloadPatientSummaryForSpontaneousReport?contextGuid=" + datasetInstance.DatasetInstanceGuid.ToString() + "&isSerious=false",
                        Text = "View Patient Summary"
                    };
                    li.Controls.Add(hyp);
                }
                ul.Controls.Add(li);
            }
        }

    }
}