using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;

using PVIMS.Web.Models;

using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using FrameworkCustomAttributeConfiguration = VPS.CustomAttributes.CustomAttributeConfiguration;

namespace PVIMS.Web
{
    public partial class PatientView : MainPageBase
    {
        private int _pid;
        private Patient _patient;
        private int _returnCohort;
        private List<PatientStatus> statuses;

        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3, ReadOnlyMode = 4 };
        private FormMode _formMode = FormMode.ViewMode;

        private User _user;

        // Global variables for patient save
        TerminologyMedDra _sourceTerm = null;
        DateTime _tempDOB = DateTime.MinValue;
        DateTime _tempConditionStartDate = DateTime.MinValue;
        DateTime _tempConditionEndDate = DateTime.MinValue;
        DateTime _tempEncounterDate = DateTime.MinValue;
        DateTime _tempCohortEnrollmentDate = DateTime.MinValue;
        string _url = string.Empty;

        public IWorkFlowService _workFlowService { get; set; }
        public ICustomAttributeService _attributeService { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            _user = UnitOfWork.Repository<User>().Queryable().Include(u => u.Facilities.Select(f => f.Facility)).SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);

            if (Request.QueryString["pid"] != null)
            {
                _pid = Convert.ToInt32(Request.QueryString["pid"]);
                if (_pid == 0)
                {
                    _patient = null;
                    _formMode = FormMode.AddMode;
                }
                else
                {
                    _patient = GetPatient(_pid);
                    if(_patient == null)
                    {
                        lblFirstName.Attributes.Remove("class");
                        lblFirstName.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Patient does not exist or has been archived";
                        lblFirstName.Controls.Add(errorMessageDiv);

                        diverror.Visible = true;

                        _formMode = FormMode.AddMode;
                    }
                    else
                    {
                        // Read only mode
                        if (!_user.HasFacility(_patient.GetCurrentFacility().Facility.Id))
                        {
                            _formMode = FormMode.ReadOnlyMode;
                        }

                        hrefDownloadAll.Attributes.Add("data-pid", _pid.ToString());
                    }
                }
            }
            else
            {
                throw new Exception("pid not passed as parameter");
            }

            if (Request.QueryString["returnCohort"] != null)
            {
                _returnCohort = Convert.ToInt32(Request.QueryString["returnCohort"]);
            }

            string action;
            if (Request.QueryString["a"] != null)
            {
                action = Request.QueryString["a"];
                switch (action)
                {
                    case "edit":
                        _formMode = FormMode.EditMode;
                        break;

                    case "refresh":
                        divstatus.Visible = false;
                        break;

                } // switch (_action)
            }

            RenderButtons();
            ToggleView();
            LoadCustom();
            LoadCustomCondition();

            txtEncounterDate.Value = DateTime.Today.ToString("yyyy-MM-dd");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Patient View " + (_patient != null ? "for " + _patient.FullName : ""), SubTitle = "", Icon = "fa fa-group fa-fw" });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            hidLastTab.Value = Request.Form[hidLastTab.UniqueID];
            if (!Page.IsPostBack)
            {
                Master.MainMenu.SetActive("PatientView");

                LoadDropDownList();
                LoadStatusDropDownList();

                if (_patient != null)
                {
                    RenderPatient();
                    RenderConditionGroups();
                    RenderReportingItems();
                    RenderGrids();
                    RenderEncounterGrids();
                    SetActive("tab3");
                }

                //ClientScript.GetPostBackEventReference(this, string.Empty);
            }
            else
            {
                divstatus.Visible = false;
            }
        }

        private void LoadCustom()
        {
            TableRow row;
            TableCell cell;
            DropDownList ddl;
            TextBox txt;
            Label lbl;

            string[] categories = { "Custom" };

            foreach (var con in UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(c => c.ExtendableTypeName == "Patient").ToList())
            {
                if (categories.Contains(con.Category))
                {
                    row = new TableRow();

                    cell = new TableCell();
                    cell.Text = con.AttributeKey;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    if (_formMode == FormMode.ViewMode)
                        cell.Text = String.Empty;
                    else
                    {
                        lbl = new Label();
                        lbl.ID = string.Format("lbl{0}", con.Id);

                        // Add mode so initialise value
                        switch (con.CustomAttributeType)
                        {
                            case CustomAttributeType.String:
                                lbl.Attributes.Add("class", "input");
                                if (con.IsRequired)
                                {
                                    var i = new HtmlGenericControl("i");
                                    i.Attributes.Add("class", "icon-append fa fa-asterisk");
                                    i.Attributes.Add("style", "color:red;");
                                    lbl.Controls.Add(i);
                                }
                                txt = new TextBox();
                                txt.ID = "txt" + con.Id;
                                txt.Attributes.Add("type", "text");
                                txt.Text = "";
                                if (con.StringMaxLength.HasValue)
                                {
                                    txt.Attributes.Add("maxlength", con.StringMaxLength.Value.ToString());
                                }
                                lbl.Controls.Add(txt);
                                break;
                            case CustomAttributeType.DateTime:
                                lbl.Attributes.Add("class", "input");
                                if (con.IsRequired)
                                {
                                    var i = new HtmlGenericControl("i");
                                    i.Attributes.Add("class", "icon-append fa fa-asterisk");
                                    i.Attributes.Add("style", "color:red;");
                                    lbl.Controls.Add(i);
                                }
                                txt = new TextBox();
                                txt.TextMode = TextBoxMode.Date;
                                if (con.FutureDateOnly)
                                {
                                    txt.Attributes.Add("min", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                                }
                                if (con.PastDateOnly)
                                {
                                    txt.Attributes.Add("max", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                                }
                                txt.ID = "txt" + con.Id;
                                txt.Text = "";
                                lbl.Controls.Add(txt);
                                break;
                            case CustomAttributeType.Numeric:
                                lbl.Attributes.Add("class", "input");
                                if (con.IsRequired)
                                {
                                    var i = new HtmlGenericControl("i");
                                    i.Attributes.Add("class", "icon-append fa fa-asterisk");
                                    i.Attributes.Add("style", "color:red;");
                                    lbl.Controls.Add(i);
                                }
                                txt = new TextBox();
                                txt.TextMode = TextBoxMode.Number;
                                if (con.NumericMinValue.HasValue)
                                {
                                    txt.Attributes.Add("min", con.NumericMinValue.Value.ToString());
                                }
                                if (con.NumericMaxValue.HasValue)
                                {
                                    txt.Attributes.Add("max", con.NumericMaxValue.Value.ToString());
                                }
                                txt.ID = "txt" + con.Id;
                                txt.Text = "";
                                lbl.Controls.Add(txt);
                                break;
                            case CustomAttributeType.Selection:
                                lbl.Attributes.Add("class", "input");
                                ddl = new DropDownList();
                                ddl.ID = "ddl" + con.Id;
                                ddl.CssClass = "form-control";
                                // Populate drop down list
                                foreach (var sdi in UnitOfWork.Repository<SelectionDataItem>().Queryable().Where(h => h.AttributeKey == con.AttributeKey).ToList())
                                    ddl.Items.Add(new ListItem(sdi.Value, sdi.SelectionKey));
                                lbl.Controls.Add(ddl);
                                break;
                        }

                        cell.Controls.Add(lbl);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);

                    dt_1.Rows.Add(row);
                }
            }
        }

        private void LoadCustomCondition()
        {
            TableRow row;
            TableCell cell;
            DropDownList ddl;
            TextBox txt;
            Label lbl;

            string[] categories = { "Custom" };

            foreach (var con in UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(c => c.ExtendableTypeName == "PatientCondition").ToList())
            {
                if (categories.Contains(con.Category))
                {
                    row = new TableRow();

                    cell = new TableCell();
                    cell.Text = con.AttributeKey;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    if (_formMode == FormMode.ViewMode)
                        cell.Text = String.Empty;
                    else
                    {
                        lbl = new Label();
                        lbl.ID = string.Format("lbl{0}", con.Id);

                        // Add mode so initialise value
                        switch (con.CustomAttributeType)
                        {
                            case CustomAttributeType.String:
                                lbl.Attributes.Add("class", "input");
                                if (con.IsRequired)
                                {
                                    var i = new HtmlGenericControl("i");
                                    i.Attributes.Add("class", "icon-append fa fa-asterisk");
                                    i.Attributes.Add("style", "color:red;");
                                    lbl.Controls.Add(i);
                                }
                                txt = new TextBox();
                                txt.ID = "txt" + con.Id;
                                txt.Attributes.Add("type", "text");
                                txt.Text = "";
                                if (con.StringMaxLength.HasValue)
                                {
                                    txt.Attributes.Add("maxlength", con.StringMaxLength.Value.ToString());
                                }
                                lbl.Controls.Add(txt);
                                break;
                            case CustomAttributeType.DateTime:
                                lbl.Attributes.Add("class", "input");
                                if (con.IsRequired)
                                {
                                    var i = new HtmlGenericControl("i");
                                    i.Attributes.Add("class", "icon-append fa fa-asterisk");
                                    i.Attributes.Add("style", "color:red;");
                                    lbl.Controls.Add(i);
                                }
                                txt = new TextBox();
                                txt.TextMode = TextBoxMode.Date;
                                if (con.FutureDateOnly)
                                {
                                    txt.Attributes.Add("min", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                                }
                                if (con.PastDateOnly)
                                {
                                    txt.Attributes.Add("max", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                                }
                                txt.ID = "txt" + con.Id;
                                txt.Text = "";
                                lbl.Controls.Add(txt);
                                break;
                            case CustomAttributeType.Numeric:
                                lbl.Attributes.Add("class", "input");
                                if (con.IsRequired)
                                {
                                    var i = new HtmlGenericControl("i");
                                    i.Attributes.Add("class", "icon-append fa fa-asterisk");
                                    i.Attributes.Add("style", "color:red;");
                                    lbl.Controls.Add(i);
                                }
                                txt = new TextBox();
                                txt.TextMode = TextBoxMode.Number;
                                if (con.NumericMinValue.HasValue)
                                {
                                    txt.Attributes.Add("min", con.NumericMinValue.Value.ToString());
                                }
                                if (con.NumericMaxValue.HasValue)
                                {
                                    txt.Attributes.Add("max", con.NumericMaxValue.Value.ToString());
                                }
                                txt.ID = "txt" + con.Id;
                                txt.Text = "";
                                lbl.Controls.Add(txt);
                                break;
                            case CustomAttributeType.Selection:
                                lbl.Attributes.Add("class", "input");
                                ddl = new DropDownList();
                                ddl.ID = "ddl" + con.Id;
                                ddl.CssClass = "form-control";
                                // Populate drop down list
                                foreach (var sdi in UnitOfWork.Repository<SelectionDataItem>().Queryable().Where(h => h.AttributeKey == con.AttributeKey).ToList())
                                    ddl.Items.Add(new ListItem(sdi.Value, sdi.SelectionKey));
                                lbl.Controls.Add(ddl);
                                break;
                        }

                        cell.Controls.Add(lbl);
                    }
                    row.Cells.Add(cell);
                    
                    dtConditionAttributes.Rows.Add(row);
                }
            }
        }

        #region "Rendering"

        private void LoadDropDownList()
        {
            ListItem item;
            var facilityList = (from f in UnitOfWork.Repository<Facility>().Queryable() orderby f.FacilityName ascending select f).ToList();

            item = new ListItem();
            item.Text = "";
            item.Value = "0";
            ddlFacility.Items.Add(item);

            foreach (Facility fac in facilityList)
            {
                if (_user.HasFacility(fac.Id))
                {
                    item = new ListItem();
                    item.Text = fac.FacilityName;
                    item.Value = fac.Id.ToString();
                    ddlFacility.Items.Add(item);
                }
            }

            if (ddlFacility.Items.Count == 1)
            {
                ddlFacility.Items.Clear();

                item = new ListItem();
                item.Text = "-- UNDEFINED --";
                item.Value = "0";
                ddlFacility.Items.Add(item);
            }

            var conditionList = (from c in UnitOfWork.Repository<Condition>().Queryable() select c).ToList();

            item = new ListItem();
            item.Text = "";
            item.Value = "0";
            ddlConditions.Items.Add(item);

            foreach (Condition c in conditionList)
            {
                item = new ListItem();
                item.Text = c.Description;
                item.Value = c.Id.ToString();
                ddlConditions.Items.Add(item);
            }
            if (ddlConditions.Items.Count > 0)
                ddlConditions.SelectedIndex = 0;

            var encounterTypeList = (from et in UnitOfWork.Repository<EncounterType>().Queryable() select et).ToList();
            foreach (EncounterType et in encounterTypeList)
            {
                item = new ListItem();
                item.Text = et.Description;
                item.Value = et.Id.ToString();
                ddlEncounterType.Items.Add(item);
            }
            if (ddlEncounterType.Items.Count > 0)
                ddlEncounterType.SelectedIndex = 0;

            var priorityList = (from p in UnitOfWork.Repository<Priority>().Queryable() select p).ToList();
            foreach (Priority p in priorityList)
            {
                item = new ListItem();
                item.Text = p.Description;
                item.Value = p.Id.ToString();
                ddlPriority.Items.Add(item);
            }
            if (ddlPriority.Items.Count > 0)
                ddlPriority.SelectedIndex = 0;

        }

        private void LoadStatusDropDownList()
        {
            ListItem item;
            statuses = (from f in UnitOfWork.Repository<PatientStatus>().Queryable() orderby f.Description ascending select f).ToList();

            foreach (PatientStatus status in statuses)
            {
                item = new ListItem();
                item.Text = status.Description;
                item.Value = status.Id.ToString();
                ddlStatus.Items.Add(item);
            }
        }

        private void RenderPatient()
        {
            txtUID.Value = _patient.Id.ToString();
            txtGUID.Value = _patient.PatientGuid.ToString();
            txtFirstName.Value = _patient.FirstName;
            txtMiddleName.Value = _patient.MiddleName;
            txtSurname.Value = _patient.Surname;
            txtDOB.Value = _patient.DateOfBirth != null ? Convert.ToDateTime(_patient.DateOfBirth).ToString("yyyy-MM-dd") : null;
            txtAge.Value = _patient.Age > 0 ? txtAge.Value = _patient.Age.ToString() : "";
            divAge.Visible = _patient.Age > 0 ? true : false;
            txtAgeGroup.Value = _patient.AgeGroup != "" ? txtAgeGroup.Value = _patient.AgeGroup : "";
            divAgeGroup.Visible = _patient.AgeGroup != "" ? true : false;

            ddlFacility.SelectedValue = "0";
            divFacilityEnrolled.Visible = false;

            var patientFacility = _patient.GetCurrentFacility();
            if (patientFacility != null)
            {
                ddlFacility.SelectedValue = patientFacility.Facility.Id.ToString();
                txtFacility.Value = patientFacility.Facility.FacilityName;
                txtEnrolledDate.Value = patientFacility.EnrolledDate.ToString("yyyy-MM-dd");
                divFacilityEnrolled.Visible = true;
            }

            CKEditor1.Text = _patient.Notes;

            // Audit
            txtCreated.Value = _patient.GetCreatedStamp();
            txtUpdated.Value = _patient.GetLastUpdatedStamp();

            RenderCustomVariables();
        }

        private void RenderConditionGroups()
        {
            int[] items = UnitOfWork.Repository<PatientCondition>()
                .Queryable()
                .Where(pc => pc.Patient.Id == _patient.Id && !pc.Archived && !pc.Patient.Archived)
                .Select(p => p.TerminologyMedDra.Id)
                .ToArray();

            List<ConditionGroupListItemModel> cgarray = new List<ConditionGroupListItemModel>();
            foreach (var cm in UnitOfWork.Repository<ConditionMedDra>().Queryable().Where(cm => items.Contains(cm.TerminologyMedDra.Id)).ToList())
            {
                var cgmod = new ConditionGroupListItemModel();
                cgmod.ConditionGroup = cm.Condition.Description;

                var tempCondition = cm.GetConditionForPatient(_patient);
                if (tempCondition != null)
                {
                    cgmod.Status = tempCondition.OutcomeDate != null ? "Case Closed" : "Case Open";
                    cgmod.PatientConditionId = tempCondition.Id;
                    cgmod.StartDate = tempCondition.DateStart;
                    cgmod.Detail = String.Format("{0} started on {1}", tempCondition.TerminologyMedDra.DisplayName, tempCondition.DateStart.ToString("yyyy-MM-dd"));

                    cgarray.Add(cgmod);
                }
            }

            foreach(var mod in cgarray.OrderByDescending(m => m.StartDate))
            {
                HtmlGenericControl newDiv = new HtmlGenericControl("div");
                HyperLink hyp = new HyperLink();

                hyp.NavigateUrl = "/PatientCondition/EditPatientCondition/" + mod.PatientConditionId;
                hyp.CssClass = "btn btn-default btn-sm";
                hyp.Text = String.Format("{0} {1}", mod.ConditionGroup, mod.Status);

                newDiv.Attributes.Add("style", "width:200px; color:black; padding:5px; text-align:left; border:solid; border-color:white; border-width:1px;");
                newDiv.Controls.Add(hyp);

                divConditionItems.Controls.Add(newDiv);

                newDiv = new HtmlGenericControl("div");
                newDiv.Attributes.Add("style", "color:black; padding:5px; text-align:left; border:solid; border-color:white; border-width:1px;");
                newDiv.InnerText = mod.Detail;

                divConditionItems.Controls.Add(newDiv);
            }
        }

        private void RenderReportingItems()
        {
            var items = _workFlowService.GetExecutionStatusEventsForPatientView(_patient);

            foreach (var evt in items)
            {
                HtmlGenericControl newDiv = new HtmlGenericControl("div");
                HyperLink hyp = new HyperLink();

                hyp.NavigateUrl = "/PatientClinicalEvent/EditPatientClinicalEvent/" + evt.PatientClinicalEvent.Id;
                hyp.CssClass = "btn btn-default btn-sm";
                hyp.Text = String.Format("{0}", evt.PatientClinicalEvent.SourceTerminologyMedDra.DisplayName);

                newDiv.Attributes.Add("style", "width:200px; color:black; padding:5px; text-align:left; border:solid; border-color:white; border-width:1px;");
                newDiv.Controls.Add(hyp);

                divReportingItems.Controls.Add(newDiv);

                foreach (var activityItem in evt.ActivityItems)
                {
                    newDiv = new HtmlGenericControl("div");
                    newDiv.Attributes.Add("style", "color:black; padding:5px; text-align:left; border:solid; border-color:white; border-width:1px;");
                    newDiv.InnerText = activityItem.Display;

                    divReportingItems.Controls.Add(newDiv);
                }
            }
        }

        private void RenderGrids()
        {
            RenderAppointments();
            RenderAttachments();
            RenderEncounters();
            RenderStatus();
            RenderCohorts();
        }

        private void RenderEncounterGrids()
        {
            RenderConditions();
            RenderClinicalEvents();
            RenderMedications();
            RenderLabTests();
        }

        private void SetActive(string tab)
        {
            liAppointments.Attributes["class"] = "";
            liAttachments.Attributes["class"] = "";
            liEncounters.Attributes["class"] = "";
            liStatus.Attributes["class"] = "";

            tab3.Attributes["class"] = "tab-pane";
            tab4.Attributes["class"] = "tab-pane";
            tab5.Attributes["class"] = "tab-pane";
            tab6.Attributes["class"] = "tab-pane";
            tab7.Attributes["class"] = "tab-pane";

            switch (tab)
            {
                case "tab3":
                    tab3.Attributes["class"] = "tab-pane active";
                    liAppointments.Attributes["class"] = "active";
                    break;

                case "tab4":
                    tab4.Attributes["class"] = "tab-pane active";
                    liAttachments.Attributes["class"] = "active";
                    break;

                case "tab5":
                    tab5.Attributes["class"] = "tab-pane active";
                    liEncounters.Attributes["class"] = "active";
                    break;

                case "tab6":
                    tab6.Attributes["class"] = "tab-pane active";
                    liStatus.Attributes["class"] = "active";
                    break;

                case "tab7":
                    tab7.Attributes["class"] = "tab-pane active";
                    liCohort.Attributes["class"] = "active";
                    break;

                default:
                    break;
            }
        }

        private void RenderStatus()
        {
            TableRow row;
            TableCell cell;

            string created = string.Empty;
            string updated = string.Empty;

            var delete = new ArrayList();
            foreach (TableRow temprow in dt_basic_6.Rows)
            {
                if (temprow.Cells[0].Text != "Effective Date")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_basic_6.Rows.Remove(temprow);
            }

            foreach (var status in _patient.PatientStatusHistories.Where(x => !x.Archived))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = status.EffectiveDate.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = status.PatientStatus != null ? status.PatientStatus.Description : "";
                row.Cells.Add(cell);

                created = status.GetCreatedStamp();
                updated = status.GetLastUpdatedStamp();

                cell = new TableCell();
                cell.Text = created;
                row.Cells.Add(cell);

                dt_basic_6.Rows.Add(row);
            }
        }

        private void RenderCohorts()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            var delete = new ArrayList();
            foreach (TableRow temprow in dt_basic_7.Rows)
            {
                if (temprow.Cells[0].Text != "Cohort")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_basic_7.Rows.Remove(temprow);
            }

            foreach (var cohort in UnitOfWork.Repository<CohortGroup>().Queryable().ToList())
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = cohort.DisplayName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = cohort.StartDate.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                // Is this patient enrolled
                var enrollment = _patient.GetCohortEnrolled(cohort);
                if (enrollment != null)
                {
                    cell = new TableCell();
                    cell.Text = enrollment.EnroledDate.ToString("yyyy-MM-dd");
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Text = enrollment.DeenroledDate.HasValue ?
                        enrollment.DeenroledDate.Value.ToString("yyyy-MM-dd") :
                        @"<span class=""label label-info"">Not De-enrolled</span>";
                    row.Cells.Add(cell);
                    cell = new TableCell();
                    if (enrollment.DeenroledDate.HasValue)
                    {
                        cell.Text = "";
                    }
                    else
                    {
                        if (_formMode != FormMode.ReadOnlyMode)
                        {
                            pnl = new Panel() { CssClass = "btn-group" };
                            btn = new HtmlGenericControl("button");
                            btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                            btn.Attributes.Add("data-toggle", "dropdown");
                            btn.Controls.Add(new Label() { Text = "Action " });
                            btn.Controls.Add(new Label() { CssClass = "caret" });
                            pnl.Controls.Add(btn);

                            ul = new HtmlGenericControl("ul");
                            ul.Attributes.Add("class", "dropdown-menu pull-right");

                            li = new HtmlGenericControl("li");
                            hyp = new HyperLink()
                            {
                                NavigateUrl = "#",
                                Text = "De-enroll"
                            };
                            hyp.Attributes.Add("data-toggle", "modal");
                            hyp.Attributes.Add("data-target", "#cohortModal-de-enrol");
                            hyp.Attributes.Add("data-id", enrollment.Id.ToString());
                            hyp.Attributes.Add("data-startdate", enrollment.EnroledDate.ToString("yyyy-MM-dd"));
                            hyp.Attributes.Add("data-cohort", cohort.DisplayName);
                            li.Controls.Add(hyp);
                            ul.Controls.Add(li);

                            li = new HtmlGenericControl("li");
                            hyp = new HyperLink()
                            {
                                NavigateUrl = "#",
                                Text = "Delete"
                            };
                            hyp.Attributes.Add("data-toggle", "modal");
                            hyp.Attributes.Add("data-target", "#cohortModal-remove");
                            hyp.Attributes.Add("data-id", enrollment.Id.ToString());
                            hyp.Attributes.Add("data-startdate", enrollment.EnroledDate.ToString("yyyy-MM-dd"));
                            hyp.Attributes.Add("data-cohort", cohort.DisplayName);
                            li.Controls.Add(hyp);
                            ul.Controls.Add(li);

                            pnl.Controls.Add(ul);

                            cell.Controls.Add(pnl);
                        }
                        else
                        {
                            cell.Text = "";
                        }
                    }

                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = @"<span class=""label label-info"">Not Enrolled</span>";
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Text = @"<span class=""label label-info"">Not De-enrolled</span>";
                    row.Cells.Add(cell);

                    if (_formMode != FormMode.ReadOnlyMode)
                    {
                        cell = new TableCell();

                        // Is patient eligible to be enrolled (check primary condition group)
                        List<Condition> conditions = new List<Condition>();
                        conditions.Add(cohort.Condition);
                        if(_patient.HasCondition(conditions))
                        {
                            hyp = new HyperLink()
                            {
                                NavigateUrl = "#",
                                CssClass = "btn btn-default",
                                Text = "Enroll"
                            };
                            hyp.Attributes.Add("data-toggle", "modal");
                            hyp.Attributes.Add("data-target", "#cohortModal");
                            hyp.Attributes.Add("data-id", cohort.Id.ToString());
                            hyp.Attributes.Add("data-cohort", cohort.DisplayName);
                            cell.Controls.Add(hyp);

                        }
                        else
                        {
                            cell.Text = @"<span class=""label label-info"">Not Eligible</span>";
                        }

                        row.Cells.Add(cell);
                    }
                    else
                    {
                        cell = new TableCell();
                        cell.Text = "";
                        row.Cells.Add(cell);
                    }
                }

                dt_basic_7.Rows.Add(row);
            }
        }

        private void RenderAppointments()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_2.Rows)
            {
                if (temprow.Cells[0].Text != "Date")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_2.Rows.Remove(temprow);
            }

            string status = string.Empty;
            string created = string.Empty;
            string updated = string.Empty;

            foreach (var app in _patient.Appointments.Where(x => !x.Archived).OrderBy(a => a.AppointmentDate))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = app.AppointmentDate.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = app.Reason;
                row.Cells.Add(cell);

                status = @"<span class=""label label-info"">Current</span>";
                if (app.DNA)
                {
                    status = @"<span class=""label label-warning"">Did Not Arrive</span>";
                }
                if (app.Cancelled)
                {
                    status = @"<span class=""label label-danger"">Cancelled</span>";
                }
                cell = new TableCell();
                cell.Text = status;
                row.Cells.Add(cell);

                created = app.GetCreatedStamp();
                updated = app.GetLastUpdatedStamp();

                cell = new TableCell();
                pnl = new Panel() { CssClass = "btn-group" };

                btn = new HtmlGenericControl("button");
                btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                btn.Attributes.Add("data-toggle", "dropdown");
                btn.Controls.Add(new Label() { Text = "Action " });
                btn.Controls.Add(new Label() { CssClass = "caret" });
                pnl.Controls.Add(btn);

                ul = new HtmlGenericControl("ul");
                ul.Attributes.Add("class", "dropdown-menu pull-right");

                li = new HtmlGenericControl("li");

                if (_formMode != FormMode.ReadOnlyMode)
                {
                    if (app.AppointmentDate > DateTime.Today.AddDays(-1))
                    {
                        hyp = new HyperLink()
                        {
                            NavigateUrl = string.Format("/Appointment/EditAppointment?id={0}&cancelRedirectUrl={1}", app.Id.ToString(), "/Patient/PatientView.aspx?pid=" + _patient.Id.ToString()),
                            Text = "Edit Appointment"
                        };
                        li.Controls.Add(hyp);
                        ul.Controls.Add(li);

                        li = new HtmlGenericControl("li");
                        hyp = new HyperLink()
                        {
                            NavigateUrl = "/Appointment/DeleteAppointment/" + app.Id.ToString(),
                            Text = "Delete Appointment"
                        };

                        li.Controls.Add(hyp);
                        ul.Controls.Add(li);
                    }

                    // DNA menu if necessary
                    if (app.AppointmentDate < DateTime.Today.AddDays(-3) && app.DNA == false && app.Cancelled == false)
                    {
                        li = new HtmlGenericControl("li");
                        hyp = new HyperLink()
                        {
                            NavigateUrl = "#",
                            Text = "Did Not Arrive"
                        };
                        hyp.Attributes.Add("data-toggle", "modal");
                        hyp.Attributes.Add("data-target", "#appointmentModal");
                        hyp.Attributes.Add("data-id", app.Id.ToString());
                        hyp.Attributes.Add("data-evt", "dna");
                        hyp.Attributes.Add("data-date", app.AppointmentDate.ToString("yyyy-MM-dd"));
                        hyp.Attributes.Add("data-reason", app.Reason);
                        hyp.Attributes.Add("data-cancelled", app.Cancelled ? "Yes" : "No");
                        hyp.Attributes.Add("data-cancelledreason", app.CancellationReason);
                        hyp.Attributes.Add("data-created", created);
                        hyp.Attributes.Add("data-updated", updated);
                        li.Controls.Add(hyp);
                        ul.Controls.Add(li);
                    }

                    pnl.Controls.Add(ul);

                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }

                dt_2.Rows.Add(row);
            }
        }

        private void RenderAttachments()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            var delete = new ArrayList();

            foreach (TableRow temprow in dt_3.Rows)
            {
                if (temprow.Cells[0].Text != "Type")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_3.Rows.Remove(temprow);
            }

            foreach (var att in _patient.Attachments.Where(x => !x.Archived).OrderByDescending(a => a.Created))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = att.AttachmentType.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = att.FileName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = att.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = String.Format("By {0} on {1}", att.CreatedBy != null ? att.CreatedBy.FirstName + ' ' + att.CreatedBy.LastName : "UNKNOWN", att.Created.ToString("yyyy-MM-dd HH:mm"));
                row.Cells.Add(cell);

                if (_formMode != FormMode.ReadOnlyMode)
                {
                    cell = new TableCell();
                    pnl = new Panel() { CssClass = "btn-group" };

                    btn = new HtmlGenericControl("button");
                    btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                    btn.Attributes.Add("data-toggle", "dropdown");
                    btn.Controls.Add(new Label() { Text = "Action " });
                    btn.Controls.Add(new Label() { CssClass = "caret" });
                    pnl.Controls.Add(btn);

                    ul = new HtmlGenericControl("ul");
                    ul.Attributes.Add("class", "dropdown-menu pull-right");

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/Patient/DeleteAttachment/" + att.Id.ToString(),
                        Text = "Delete Attachment"
                    };

                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "#",
                        Text = "Download Attachment"
                    };
                    hyp.Attributes.Add("data-attachmentid", att.Id.ToString());
                    hyp.Attributes.Add("data-filename", att.FileName);
                    hyp.Attributes.Add("onclick", "showDownloadNotification(this);");
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);

                    cell.Controls.Add(pnl);                    
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }

                dt_3.Rows.Add(row);
            }
        }

        private void RenderEncounters()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_4.Rows)
            {
                if (temprow.Cells[0].Text != "Date")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_4.Rows.Remove(temprow);
            }

            string status = string.Empty;
            string created = string.Empty;
            string updated = string.Empty;

            foreach (var enc in _patient.Encounters.Where(x => !x.Archived).OrderByDescending(x => x.EncounterDate))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = enc.EncounterDate.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = enc.EncounterType.Description;
                row.Cells.Add(cell);

                if (_formMode != FormMode.ReadOnlyMode)
                {
                    cell = new TableCell();
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/Encounter/ViewEncounter/" + enc.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "View Encounter"
                    };
                    cell.Controls.Add(hyp);
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }

                dt_4.Rows.Add(row);
            }
        }

        private void RenderCustomVariables()
        {
            IExtendable patientExtended = null;
            if (_pid > 0)
                patientExtended = _patient;

            TableRow row;
            TableCell attributeValueCell;
            TableCell lastUpdatedCell;
            DropDownList ddl;
            TextBox txt;
            SelectionDataItem tempSDI;
            string[] categories = { "Custom" };

            foreach (var con in UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(c => c.ExtendableTypeName == "Patient").ToList())
            {
                if (categories.Contains(con.Category))
                {
                    // Get the corresponding row
                    row = null;
                    foreach (TableRow tempRow in dt_1.Rows)
                        if (tempRow.Cells[0].Text == con.AttributeKey)
                            row = tempRow;

                    if (row != null)
                    {
                        lastUpdatedCell = row.Cells[2];
                        attributeValueCell = row.Cells[1];
                        if (_formMode == FormMode.ViewMode)
                        {
                            // If in view mode, then display current value in a normal text cell
                            var attribute = patientExtended.GetAttributeValue(con.AttributeKey);
                            if (attribute == null)
                                attributeValueCell.Text = String.Empty;
                            else
                            {
                                if (con.CustomAttributeType == CustomAttributeType.Selection)
                                {
                                    tempSDI = GetSelectionDataItem(con.AttributeKey, attribute.ToString());
                                    if (tempSDI != null)
                                        attributeValueCell.Text = tempSDI.Value;
                                    else
                                        attributeValueCell.Text = string.Empty;
                                }
                                else if (con.CustomAttributeType == CustomAttributeType.DateTime)
                                {
                                    DateTime dateTime;
                                    if (attribute != null && DateTime.TryParse(attribute.ToString(), out dateTime))
                                    {
                                        attributeValueCell.Text = dateTime.ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        attributeValueCell.Text = attribute.ToString();
                                    }
                                }
                                else
                                    attributeValueCell.Text = attribute.ToString();

                                if (attributeValueCell.Text != String.Empty) {
                                    lastUpdatedCell.Text = string.Format("By {0} on {1}", patientExtended.GetUpdatedByUser(con.AttributeKey), patientExtended.GetUpdatedDate(con.AttributeKey).ToString("yyyy-MM-dd"));
                                }
                                else {
                                    lastUpdatedCell.Text = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            if (_pid > 0)
                            {
                                var attribute = patientExtended.GetAttributeValue(con.AttributeKey);
                                var textBoxIndex = con.IsRequired ? 1 : 0;

                                // Edit mode so default value
                                switch (con.CustomAttributeType)
                                {
                                    case CustomAttributeType.String:
                                        txt = (TextBox)attributeValueCell.Controls[0].Controls[textBoxIndex];
                                        txt.Text = attribute != null ? attribute.ToString() : string.Empty;
                                        break;
                                    case CustomAttributeType.DateTime:
                                        txt = (TextBox)attributeValueCell.Controls[0].Controls[textBoxIndex];
                                        DateTime dateTime;
                                        if (attribute != null && DateTime.TryParse(attribute.ToString(), out dateTime))
                                        {
                                            txt.Text = dateTime.ToString("yyyy-MM-dd");
                                        }
                                        else
                                        {
                                            attributeValueCell.Text = string.Empty;
                                        }
                                        break;
                                    case CustomAttributeType.Numeric:
                                        txt = (TextBox)attributeValueCell.Controls[0].Controls[textBoxIndex];
                                        txt.Text = attribute != null ? attribute.ToString() : string.Empty;
                                        break;
                                    case CustomAttributeType.Selection:
                                        ddl = (DropDownList)attributeValueCell.Controls[0].Controls[0];
                                        ddl.SelectedValue = attribute != null ? attribute.ToString() : string.Empty;
                                        break;
                                }
                            }
                            else
                                // Add mode so initialise value
                                switch (con.CustomAttributeType)
                                {
                                    case CustomAttributeType.String:
                                        txt = (TextBox)attributeValueCell.Controls[0];
                                        txt.Text = "";
                                        break;
                                    case CustomAttributeType.DateTime:
                                        txt = (TextBox)attributeValueCell.Controls[0];
                                        txt.Text = "";
                                        break;
                                    case CustomAttributeType.Numeric:
                                        txt = (TextBox)attributeValueCell.Controls[0];
                                        txt.Text = "";
                                        break;
                                    case CustomAttributeType.Selection:
                                        ddl = (DropDownList)attributeValueCell.Controls[0];
                                        ddl.SelectedValue = "";
                                        break;
                                }
                        }
                    }
                }
            }
        }

        private void RenderConditions()
        {
            TableRow row;
            TableCell cell;
            HyperLink storeHyp;
            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_basic_8.Rows)
            {
                if (temprow.Cells[1].Text == "")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_basic_8.Rows.Remove(temprow);
            }

            foreach (var patientCondition in _patient.PatientConditions.Where(pc => !pc.Archived && !pc.Patient.Archived))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = patientCondition.TerminologyMedDra != null ? patientCondition.TerminologyMedDra.MedDraTerm.ToString() : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientCondition.DateStart != null ? ((DateTime)patientCondition.DateStart).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientCondition.OutcomeDate != null ? ((DateTime)patientCondition.OutcomeDate).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientCondition.Outcome != null ? patientCondition.Outcome.Description : "";
                row.Cells.Add(cell);

                if (_formMode != FormMode.ReadOnlyMode)
                {
                    cell = new TableCell();
                    pnl = new Panel() { CssClass = "btn-group" };
                    btn = new HtmlGenericControl("button");
                    btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                    btn.Attributes.Add("data-toggle", "dropdown");
                    btn.Controls.Add(new Label() { Text = "Action " });
                    btn.Controls.Add(new Label() { CssClass = "caret" });
                    pnl.Controls.Add(btn);

                    ul = new HtmlGenericControl("ul");
                    ul.Attributes.Add("class", "dropdown-menu pull-right");

                    li = new HtmlGenericControl("li");
                    storeHyp = new HyperLink()
                    {
                        NavigateUrl = "/PatientCondition/EditPatientCondition/" + patientCondition.Id.ToString(),
                        Text = "Edit Condition"
                    };

                    li.Controls.Add(storeHyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/PatientCondition/DeletePatientCondition/" + patientCondition.Id.ToString(),
                        Text = "Delete Condition"
                    };

                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);

                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }


                dt_basic_8.Rows.Add(row);
            }
        }

        private void RenderClinicalEvents()
        {
            TableRow row;
            TableCell cell;
            HyperLink storeHyp;
            HyperLink hyp;
            Panel pnl;

            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            IExtendable eventExtended;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_basic_9.Rows)
            {
                if (temprow.Cells[0].Text == "")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_basic_9.Rows.Remove(temprow);
            }

            foreach (var clinicalEvent in _patient.PatientClinicalEvents.Where(pc => !pc.Archived && !pc.Patient.Archived))
            {
                eventExtended = clinicalEvent;

                row = new TableRow();

                cell = new TableCell();
                cell.Text = clinicalEvent.SourceTerminologyMedDra.DisplayName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = clinicalEvent.OnsetDate != null ? ((DateTime)clinicalEvent.OnsetDate).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                var config = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Date of Report");
                cell = new TableCell();
                cell.Text = _attributeService.GetCustomAttributeValue(config, eventExtended);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = clinicalEvent.ResolutionDate != null ? ((DateTime)clinicalEvent.ResolutionDate).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                config = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Is the adverse event serious?");
                cell = new TableCell();
                cell.Text = _attributeService.GetCustomAttributeValue(config, eventExtended);
                row.Cells.Add(cell);

                if (_formMode != FormMode.ReadOnlyMode)
                {
                    cell = new TableCell();
                    pnl = new Panel() { CssClass = "btn-group" };
                    btn = new HtmlGenericControl("button");
                    btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                    btn.Attributes.Add("data-toggle", "dropdown");
                    btn.Controls.Add(new Label() { Text = "Action " });
                    btn.Controls.Add(new Label() { CssClass = "caret" });
                    pnl.Controls.Add(btn);

                    ul = new HtmlGenericControl("ul");
                    ul.Attributes.Add("class", "dropdown-menu pull-right");

                    li = new HtmlGenericControl("li");
                    storeHyp = new HyperLink()
                    {
                        NavigateUrl = "/PatientClinicalEvent/EditPatientClinicalEvent/" + clinicalEvent.Id.ToString(),
                        Text = "Edit Adverse Event"
                    };

                    li.Controls.Add(storeHyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/PatientClinicalEvent/DeletePatientClinicalEvent/" + clinicalEvent.Id.ToString(),
                        Text = "Delete Adverse Event"
                    };

                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);

                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }

                dt_basic_9.Rows.Add(row);
            }
        }

        private void RenderMedications()
        {
            TableRow row;
            TableCell cell;
            HyperLink storeHyp;
            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            IExtendable medExtended;
            var config = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Type of Indication");

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_basic_10.Rows)
            {
                if (temprow.Cells[0].Text == "")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_basic_10.Rows.Remove(temprow);
            }

            foreach (var patientMedication in _patient.PatientMedications.Where(pm => !pm.Archived && !pm.Patient.Archived))
            {
                medExtended = patientMedication;

                row = new TableRow();

                cell = new TableCell();
                cell.Text = patientMedication.Medication != null ? patientMedication.Medication.DrugName.ToString() : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientMedication.Dose;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientMedication.DoseUnit;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientMedication.DoseFrequency;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientMedication.DateStart != null ? ((DateTime)patientMedication.DateStart).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientMedication.DateEnd != null ? ((DateTime)patientMedication.DateEnd).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                var typeInd = "";
                if(medExtended.GetAttributeValue("Type of Indication") != null)
                {
                    var val = medExtended.GetAttributeValue("Type of Indication").ToString();
                    var selection = UnitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(s => s.AttributeKey == config.AttributeKey && s.SelectionKey == val);

                    typeInd = selection.Value;
                }
                cell.Text = typeInd;
                row.Cells.Add(cell);

                if (_formMode != FormMode.ReadOnlyMode)
                {
                    cell = new TableCell();
                    pnl = new Panel() { CssClass = "btn-group" };
                    btn = new HtmlGenericControl("button");
                    btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                    btn.Attributes.Add("data-toggle", "dropdown");
                    btn.Controls.Add(new Label() { Text = "Action " });
                    btn.Controls.Add(new Label() { CssClass = "caret" });
                    pnl.Controls.Add(btn);

                    ul = new HtmlGenericControl("ul");
                    ul.Attributes.Add("class", "dropdown-menu pull-right");

                    li = new HtmlGenericControl("li");
                    storeHyp = new HyperLink()
                    {
                        NavigateUrl = "/PatientMedication/EditPatientMedication/" + patientMedication.Id.ToString(),
                        Text = "Edit Medication"
                    };

                    li.Controls.Add(storeHyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/PatientMedication/DeletePatientMedication/" + patientMedication.Id.ToString(),
                        Text = "Delete Medication"
                    };

                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);

                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);
                }

                dt_basic_10.Rows.Add(row);
            }

            // Hide columns where necessary
            if (_formMode == FormMode.ReadOnlyMode)
            {
                dt_basic_10.HideColumn(7); // Action column
            }
            if(config == null)
            {
                dt_basic_10.HideColumn(6); // Type of indication column
            }
        }

        private void RenderLabTests()
        {
            TableRow row;
            TableCell cell;
            HyperLink storeHyp;
            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_basic_11.Rows)
            {
                if (temprow.Cells[0].Text == "")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_basic_11.Rows.Remove(temprow);
            }

            foreach (var patientLabTest in _patient.PatientLabTests.Where(pl => !pl.Archived && !pl.Patient.Archived))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text =patientLabTest.LabTest != null ? patientLabTest.LabTest.Description.ToString() : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientLabTest.TestDate != null ? ((DateTime)patientLabTest.TestDate).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientLabTest.TestResult;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientLabTest.LabValue;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientLabTest.TestUnit != null ? patientLabTest.TestUnit.Description.ToString() : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                var lower = !String.IsNullOrWhiteSpace(patientLabTest.ReferenceLower) ? string.Format("Lower: {0}", patientLabTest.ReferenceLower) : "";
                var upper = !String.IsNullOrWhiteSpace(patientLabTest.ReferenceUpper) ? string.Format("Upper: {0}", patientLabTest.ReferenceUpper) : "";
                cell.Text = String.Format("{0}{1}{2}", lower, !String.IsNullOrWhiteSpace(patientLabTest.ReferenceLower) ? "<br />" : "", upper);
                row.Cells.Add(cell);

                if (_formMode != FormMode.ReadOnlyMode)
                {
                    cell = new TableCell();
                    pnl = new Panel() { CssClass = "btn-group" };
                    btn = new HtmlGenericControl("button");
                    btn.Attributes.Add("class", "btn btn-default dropdown-toggle");
                    btn.Attributes.Add("data-toggle", "dropdown");
                    btn.Controls.Add(new Label() { Text = "Action " });
                    btn.Controls.Add(new Label() { CssClass = "caret" });
                    pnl.Controls.Add(btn);

                    ul = new HtmlGenericControl("ul");
                    ul.Attributes.Add("class", "dropdown-menu pull-right");

                    li = new HtmlGenericControl("li");
                    storeHyp = new HyperLink()
                    {
                        NavigateUrl = "/PatientLabTest/EditPatientLabTest/" + patientLabTest.Id.ToString(),
                        Text = "Edit Clinical Evaluation"
                    };

                    li.Controls.Add(storeHyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/PatientLabTest/DeletePatientLabTest/" + patientLabTest.Id.ToString(),
                        Text = "Delete Clinical Evaluation"
                    };

                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);

                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }

                dt_basic_11.Rows.Add(row);
            }
        }

        private void RenderButtons()
        {
            Button btn;
            HyperLink hyp;

            switch (_formMode)
            {
                case FormMode.ViewMode:
                    hyp = new HyperLink()
                    {
                        ID = "btnEdit",
                        NavigateUrl = "PatientView.aspx?pid=" + _patient.Id.ToString() + "&a=edit",
                        CssClass = "btn btn-primary",
                        Text = "Edit Patient"
                    };
                    spnButtons.Controls.Add(hyp);

                    if (HttpContext.Current.User.IsInRole("Clinician"))
                    {
                        hyp = new HyperLink
                        {
                            ID = "btnDelete",
                            NavigateUrl = String.Format("/Patient/DeletePatient?id={0}&cancelRedirectUrl={1}", _patient.Id.ToString(), "/Patient/PatientView.aspx?pid=" + _patient.Id.ToString()),
                            CssClass = "btn btn-default",
                            Text = "Delete Patient"
                        };
                        spnButtons.Controls.Add(hyp);

                    }

                    hyp = new HyperLink()
                    {
                        ID = "btnSearchPatient",
                        NavigateUrl = "PatientSearch.aspx",
                        CssClass = "btn btn-default",
                        Text = "Patient Search"
                    };
                    spnButtons.Controls.Add(hyp);

                    if (_returnCohort != null && _returnCohort > 0)
                    {
                        hyp = new HyperLink()
                        {
                            ID = "btnViewCohort",
                            NavigateUrl = "/Cohort/CohortView.aspx?id=" + _returnCohort.ToString(),
                            CssClass = "btn btn-default",
                            Text = "Return Cohort"
                        };
                        spnButtons.Controls.Add(hyp);
                    }
                 
                    hyp = new HyperLink()
                    {
                        ID = "btnAddAppointment",
                        NavigateUrl = string.Format("/Appointment/AddAppointment?pid={0}&cancelRedirectUrl={1}", _patient.Id.ToString(), "/Patient/PatientView.aspx?pid=" + _patient.Id.ToString()),
                        CssClass = "btn btn-default",
                        Text = "Add Appointment"
                    };
                    spnAppointmentbuttons.Controls.Add(hyp);

                    hyp = new HyperLink()
                    {
                        ID = "btnAddEncounter",
                        NavigateUrl = string.Format("/Encounter/AddEncounter?pid={0}&aid={1}&cancelRedirectUrl={2}", _patient.Id.ToString(), "0", "/Patient/PatientView.aspx?pid=" + _patient.Id.ToString()),
                        CssClass = "btn btn-default",
                        Text = "Add Encounter"
                    };
                    spnEncounterButtons.Controls.Add(hyp);

                    break;

                case FormMode.EditMode:
                    btn = new Button();
                    btn.ID = "btnSave";
                    btn.CssClass = "btn btn-primary";
                    btn.Text = "Save";
                    btn.Click += btnSave_Click;
                    spnButtons.Controls.Add(btn);

                    hyp = new HyperLink()
                    {
                        ID = "btnCancel",
                        NavigateUrl = "PatientView.aspx?pid=" + _patient.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "Cancel"
                    };
                    spnButtons.Controls.Add(hyp);

                    break;

                case FormMode.AddMode:
                    btn = new Button();
                    btn.ID = "btnSave";
                    btn.CssClass = "btn btn-primary";
                    btn.Text = "Save";
                    btn.Click += btnSave_Click;
                    spnAddButtons.Controls.Add(btn);

                    hyp = new HyperLink()
                    {
                        ID = "btnCancel",
                        NavigateUrl = "PatientSearch.aspx",
                        CssClass = "btn btn-default",
                        Text = "Cancel"
                    };
                    spnAddButtons.Controls.Add(hyp);

                    break;

                case FormMode.ReadOnlyMode:
                    // Hide attachment buttons
                    btnAddAttachment.Visible = false;
                    hrefDownloadAll.Visible = false;

                    hyp = new HyperLink()
                    {
                        ID = "btnSearchPatient",
                        NavigateUrl = "PatientSearch.aspx",
                        CssClass = "btn btn-default",
                        Text = "Patient Search"
                    };
                    spnButtons.Controls.Add(hyp);

                    break;

                default:
                    break;
            };

            spnButtons.Visible = true;

        }

        private void ToggleView()
        {
            switch (_formMode)
            {
                case FormMode.ViewMode:
                    // Identification
                    txtFirstName.Attributes.Add("readonly", "true");
                    txtFirstName.Style.Add("background-color", "#EBEBE4");

                    txtMiddleName.Attributes.Add("readonly", "true");
                    txtMiddleName.Style.Add("background-color", "#EBEBE4");

                    txtSurname.Attributes.Add("readonly", "true");
                    txtSurname.Style.Add("background-color", "#EBEBE4");

                    txtDOB.Attributes.Add("readonly", "true");
                    txtDOB.Style.Add("background-color", "#EBEBE4");

                    ddlFacility.Attributes.Add("readonly", "true");
                    ddlFacility.Style.Add("background-color", "#EBEBE4");

                    ddlFacility.Visible = true;
                    txtFacility.Visible = false;

                    CKEditor1.ReadOnly = true;

                    divPatientCondition.Visible = false;
                    divAdditional.Visible = true;

                    divReadonly.Visible = false;
                    divAddClinical.Visible = true;
                    divAttachment.Visible = true;

                    // Permissions
                    tab5.Visible = false;
                    liEncounters.Visible = false;
                    if (HttpContext.Current.User.IsInRole("DataCap") || HttpContext.Current.User.IsInRole("Clinician"))
                    {
                        tab5.Visible = true;
                        liEncounters.Visible = true;
                    }

                    break;

                case FormMode.EditMode:
                    divPatientCondition.Visible = false;
                    divAdditional.Visible = false;

                    ddlFacility.Visible = true;
                    txtFacility.Visible = false;

                    divReadonly.Visible = false;
                    divAddClinical.Visible = false;
                    divAttachment.Visible = false;

                    break;

                case FormMode.AddMode:
                    divIdentifyAudit.Visible = false;
                    divClinicalinfo.Visible = false;
                    divRightPane.Visible = false;
                    divAdditional.Visible = false;

                    divPatientCondition.Visible = true;

                    ddlFacility.Visible = true;
                    txtFacility.Visible = false;

                    divReadonly.Visible = false;
                    divAddClinical.Visible = false;
                    divAttachment.Visible = false;

                    break;

                case FormMode.ReadOnlyMode:
                    // Identification
                    txtFirstName.Attributes.Add("readonly", "true");
                    txtFirstName.Style.Add("background-color", "#EBEBE4");

                    txtMiddleName.Attributes.Add("readonly", "true");
                    txtMiddleName.Style.Add("background-color", "#EBEBE4");

                    txtSurname.Attributes.Add("readonly", "true");
                    txtSurname.Style.Add("background-color", "#EBEBE4");

                    txtDOB.Attributes.Add("readonly", "true");
                    txtDOB.Style.Add("background-color", "#EBEBE4");

                    ddlFacility.Visible = false;
                    txtFacility.Visible = true;

                    CKEditor1.ReadOnly = true;

                    divPatientCondition.Visible = false;
                    divAdditional.Visible = false;
                    divClinicalinfo.Visible = false;
                    divRightPane.Visible = false;

                    divReadonly.Visible = true;
                    divAddClinical.Visible = false;
                    divAttachment.Visible = false;

                    break;

                default:
                    break;
            };
        }

        #endregion

        #region "Events"

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // reset global variables
            _sourceTerm = null;
            _tempDOB = DateTime.MinValue;
            _tempConditionStartDate = DateTime.MinValue;
            _tempConditionEndDate = DateTime.MinValue;
            _tempEncounterDate = DateTime.MinValue;
            _tempCohortEnrollmentDate = DateTime.MinValue;
            _url = string.Empty;

            diverror.Visible = false;

            // Validation
            ValidatePatientInformation();
            ValidateCustomAttributes();
            ValidatePatientAddition();

            if (_formMode == FormMode.AddMode)
            {
                ValidatePatientAdditionCustomAttributes();
            }

            // if div set then do not continue
            if (diverror.Visible) { return; };

            // Save patient record
            DateTime tempdt;
            var saveMessage = ""; // Used to set pop up
            if (_patient == null)
            {   
                _patient = new Patient { PatientGuid = Guid.NewGuid() };
                _patient.FirstName = txtFirstName.Value;
                _patient.MiddleName = txtMiddleName.Value;
                _patient.Surname = txtSurname.Value;
                _patient.DateOfBirth = DateTime.TryParse(txtDOB.Value, out tempdt) ? Convert.ToDateTime(txtDOB.Value) : (DateTime?)null;
                _patient.Notes = CKEditor1.Text;

                var facility = GetFacility(Convert.ToInt32(ddlFacility.SelectedValue));
                var patientFacility = new PatientFacility { Patient = _patient, Facility = facility, EnrolledDate = DateTime.Today };
                _patient.PatientFacilities.Add(patientFacility);

                UnitOfWork.Repository<Patient>().Save(_patient);

                // once you have successfully saved patient  give patient a default active status
                PatientStatusHistory status = null;

                status = new PatientStatusHistory()
                {
                    Patient = _patient,
                    EffectiveDate = DateTime.Today,//set default date to today
                    Comments = "New Patient",
                    PatientStatus = UnitOfWork.Repository<PatientStatus>().Queryable().Single(u => u.Description == "Active")
                };

                UnitOfWork.Repository<PatientStatusHistory>().Save(status);
                saveMessage = "Patient record added successfully";
            }
            else
            {
                _patient.FirstName = txtFirstName.Value;
                _patient.MiddleName = txtMiddleName.Value;
                _patient.Surname = txtSurname.Value;
                _patient.DateOfBirth = DateTime.TryParse(txtDOB.Value, out tempdt) ? Convert.ToDateTime(txtDOB.Value) : (DateTime?)null;
                _patient.Notes = CKEditor1.Text;

                var facility = GetFacility(Convert.ToInt32(ddlFacility.SelectedValue));
                var patientFacility = _patient.GetCurrentFacility();

                if (patientFacility != null)
                {
                    if (facility != patientFacility.Facility)
                    {
                        patientFacility = new PatientFacility { Patient = _patient, Facility = facility, EnrolledDate = DateTime.Today };
                        _patient.PatientFacilities.Add(patientFacility);
                    }
                }
                else
                {
                    patientFacility = new PatientFacility { Patient = _patient, Facility = facility, EnrolledDate = DateTime.Today };
                    _patient.PatientFacilities.Add(patientFacility);
                }

                UnitOfWork.Repository<Patient>().Update(_patient);
                _url = String.Format("PatientView.aspx?pid=" + _patient.Id.ToString());
                saveMessage = "Patient record updated successfully";
            }

            // Save custom attributes
            SaveCustomAttributes();

            // Save additional info for new patients
            if (_formMode == FormMode.AddMode)
            {
                SavePatientAddition();
            }

            UnitOfWork.Complete();

            HttpCookie cookie = new HttpCookie("PopUpMessage");
            cookie.Value = saveMessage;
            Response.Cookies.Add(cookie);

            Response.Redirect(_url);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ArrayList deleteItems = new ArrayList();
            ArrayList deleteSubItems = new ArrayList();

            foreach (Appointment appointment in _patient.Appointments)
            {
                deleteItems.Add(appointment);
            }
            foreach (Appointment appointment in deleteItems)
            {
                _patient.Appointments.Remove(appointment);
                UnitOfWork.Repository<Appointment>().Delete(appointment);
            }

            deleteItems.Clear();
            foreach (Attachment attachment in _patient.Attachments)
            {
                deleteItems.Add(attachment);
            }
            foreach (Attachment attachment in deleteItems)
            {
                _patient.Attachments.Remove(attachment);
                UnitOfWork.Repository<Attachment>().Delete(attachment);
            }

            deleteItems.Clear();
            foreach (CohortGroupEnrolment enrolment in _patient.CohortEnrolments)
            {
                deleteItems.Add(enrolment);
            }
            foreach (CohortGroupEnrolment enrolment in deleteItems)
            {
                _patient.CohortEnrolments.Remove(enrolment);
                UnitOfWork.Repository<CohortGroupEnrolment>().Delete(enrolment);
            }

            deleteItems.Clear();
            foreach (Encounter encounter in _patient.Encounters)
            {
                deleteItems.Add(encounter);
            }
            foreach (Encounter encounter in deleteItems)
            {
                _patient.Encounters.Remove(encounter);
                UnitOfWork.Repository<Encounter>().Delete(encounter);
            }

            deleteItems.Clear();
            foreach (PatientClinicalEvent clinicalEvent in _patient.PatientClinicalEvents)
            {
                deleteItems.Add(clinicalEvent);
            }
            foreach (PatientClinicalEvent clinicalEvent in deleteItems)
            {
                _patient.PatientClinicalEvents.Remove(clinicalEvent);
                UnitOfWork.Repository<PatientClinicalEvent>().Delete(clinicalEvent);
            }

            deleteItems.Clear();
            foreach (PatientCondition condition in _patient.PatientConditions)
            {
                deleteItems.Add(condition);
            }
            foreach (PatientCondition condition in deleteItems)
            {
                _patient.PatientConditions.Remove(condition);
                UnitOfWork.Repository<PatientCondition>().Delete(condition);
            }

            deleteItems.Clear();
            foreach (PatientFacility facility in _patient.PatientFacilities)
            {
                deleteItems.Add(facility);
            }
            foreach (PatientFacility facility in deleteItems)
            {
                _patient.PatientFacilities.Remove(facility);
                UnitOfWork.Repository<PatientFacility>().Delete(facility);
            }

            deleteItems.Clear();
            foreach (PatientLabTest labTest in _patient.PatientLabTests)
            {
                deleteItems.Add(labTest);
            }
            foreach (PatientLabTest labTest in deleteItems)
            {
                _patient.PatientLabTests.Remove(labTest);
                UnitOfWork.Repository<PatientLabTest>().Delete(labTest);
            }

            deleteItems.Clear();
            foreach (PatientMedication medication in _patient.PatientMedications)
            {
                deleteItems.Add(medication);
            }
            foreach (PatientMedication medication in deleteItems)
            {
                _patient.PatientMedications.Remove(medication);
                UnitOfWork.Repository<PatientMedication>().Delete(medication);
            }

            deleteItems.Clear();
            foreach (PatientStatusHistory status in _patient.PatientStatusHistories)
            {
                deleteItems.Add(status);
            }
            foreach (PatientStatusHistory status in deleteItems)
            {
                _patient.PatientStatusHistories.Remove(status);
                UnitOfWork.Repository<PatientStatusHistory>().Delete(status);
            }

            UnitOfWork.Repository<Patient>().Delete(_patient);
            UnitOfWork.Complete();

            var url = String.Format("PatientSearch.aspx");
            Response.Redirect(url);
        }

        protected void btnSaveStatus_Click(object sender, EventArgs e)
        {

            PatientStatusHistory status = null;

            statuses = (from f in UnitOfWork.Repository<PatientStatus>().Queryable() orderby f.Description ascending select f).ToList();

            if (txtStatusUID.Value == "0")
            {
                status = new PatientStatusHistory()
                {
                    Patient = _patient,
                    EffectiveDate = Convert.ToDateTime(txtStatusDate.Value),
                    Comments = txtStatusDetails.Value,
                    PatientStatus = statuses.First(s => s.Id == int.Parse(ddlStatus.Value))
                };

                UnitOfWork.Repository<PatientStatusHistory>().Save(status);
            }
            else
            {
                status = GetPatientStatusHistory(Convert.ToInt32(txtStatusUID.Value));

                if (status != null)
                {
                    status.EffectiveDate = Convert.ToDateTime(txtStatusDate.Value);
                    status.Comments = txtStatusDetails.Value;
                    status.PatientStatus = statuses.First(s => s.Id == int.Parse(ddlStatus.Value));

                    UnitOfWork.Repository<PatientStatusHistory>().Update(status);
                }
            }

            UnitOfWork.Complete();

            RenderGrids();
            SetActive("tab3");

            //tab1.Attributes["class"] = "tab-pane smart-form";
        }

        protected void btnEnrolCohort_Click(object sender, EventArgs e)
        {
            CohortGroup cohort = null;
            CohortGroupEnrolment enrolment = null;

            if (txtCohortUID.Value != "0")
            {
                cohort = GetCohortGroup(Convert.ToInt32(txtCohortUID.Value));

                if (cohort != null)
                {
                    enrolment = new CohortGroupEnrolment()
                    {
                        CohortGroup = cohort,
                        EnroledDate = Convert.ToDateTime(txtCohortDateEnrolment.Value),
                        Patient = _patient
                    };

                    UnitOfWork.Repository<CohortGroupEnrolment>().Save(enrolment);
                    UnitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Patient enrolled into cohort successfully";
                    Response.Cookies.Add(cookie);
                    Master.ShouldPopUpBeDisplayed();
                }
            }

            RenderGrids();
            SetActive("tab7");

        }

        protected void btnDeleteStatus_Click(object sender, EventArgs e)
        {
            PatientStatusHistory status = null;

            if (txtStatusUID.Value != "0")
            {
                status = _patient.PatientStatusHistories.FirstOrDefault(f => f.Id == (Convert.ToInt32(txtStatusUID.Value)));
            }

            if (status != null)
            {
                const string reason = "** NO REASON SPECIFIED ** ";
                status.Archived = true;
                status.ArchivedReason = reason;
                status.AuditUser = GetCurrentUser();
                status.ArchivedDate = DateTime.Now;
                UnitOfWork.Repository<PatientStatusHistory>().Update(status);
                UnitOfWork.Complete();

                RenderGrids();
                SetActive("tab3");

            }
        }

        protected void btnSaveAppointment_Click(object sender, EventArgs e)
        {
            diverror2.Visible = false;
            string err = "<ul>";

            // Validation
            DateTime tempdt;
            DateTime todaydate = DateTime.Today;
            if (DateTime.TryParse(txtAppointmentDate.Value, out tempdt) == false)
            {
                err += "<li>Please ensure you have selected a valid appointment date...</li>";
            }
            else if (GetMonthDifference(tempdt, todaydate) > 12)
            {
                err += "<li>Appointment date must be within 12 months from today...</li>";



            }
            else
            {
                tempdt = Convert.ToDateTime(txtAppointmentDate.Value);
                if (tempdt < DateTime.Today)
                {
                    err += "<li>Appointment date is less than current date...</li>";
                }

                if (_patient.HasAppointment(Convert.ToInt32(txtAppointmentUID.Value), Convert.ToDateTime(txtAppointmentDate.Value)))
                {
                    err += "<li>Appointment already exists for the selected date...</li>";
                }
                var holiday = GetHoliday(tempdt);
                if (holiday != null)
                {
                    err += "<li>Appointment date falls on a holiday (" + holiday.Description + ") ...</li>";
                }
            }
            if (err != "<ul>")
            {
                err += "</ul>";
                diverror2.Visible = true;
                spnErrors.InnerHtml = err;

                this.ClientScript.RegisterStartupScript(this.GetType(),
                                                        "navigate",
                                                        "document.getElementById('diverror2').scrollIntoView();",
                                                        true);

                RenderGrids();
                SetActive("tab3");

                return;
            }

            Int32 temp;

            string reason = txtAppointmentReason.Value.Trim() == "" ? "** NO REASON SPECIFIED ** " : txtAppointmentReason.Value;

            Appointment appointment = null;

            if (txtAppointmentUID.Value == "0")
            {
                appointment = new Appointment(_patient)
                {
                    DNA = false,
                    AppointmentDate = Convert.ToDateTime(txtAppointmentDate.Value),
                    Reason = reason,
                    Cancelled = (ddlAppointmentCancelled.Value == "Yes"),
                    CancellationReason = txtAppointmentCancelledReason.Value
                };

                UnitOfWork.Repository<Appointment>().Save(appointment);
            }
            else
            {
                appointment = GetAppointment(Convert.ToInt32(txtAppointmentUID.Value));

                if (appointment != null)
                {
                    appointment.AppointmentDate = Convert.ToDateTime(txtAppointmentDate.Value);
                    appointment.Reason = reason;
                    appointment.Cancelled = (ddlAppointmentCancelled.Value == "Yes");
                    appointment.CancellationReason = txtAppointmentCancelledReason.Value;

                    UnitOfWork.Repository<Appointment>().Update(appointment);
                }
            }

            UnitOfWork.Complete();

            RenderGrids();
            SetActive("tab3");

            tab1.Attributes["class"] = "tab-pane active smart-form";

            this.ClientScript.RegisterStartupScript(this.GetType(),
                                                    "navigate",
                                                    "document.getElementById('divapptstatus').scrollIntoView();",
                                                    true);

        }

        protected void btnDNAAppointment_Click(object sender, EventArgs e)
        {
            Appointment appointment = null;

            if (txtAppointmentUID.Value != "0")
            {
                appointment = GetAppointment(Convert.ToInt32(txtAppointmentUID.Value));
            }

            if (appointment != null)
            {
                appointment.DNA = true;

                UnitOfWork.Repository<Appointment>().Update(appointment);
                UnitOfWork.Complete();

                RenderGrids();
                SetActive("tab3");
            }
        }

        protected void btnAddAttachment_Click(object sender, EventArgs e)
        {
            divErrorFileType.Visible = false;
            divErrorSize.Visible = false;
            divErrorDuplicate.Visible = false;
            if (filAttachment.Value == string.Empty)
            {
                RenderGrids();
                SetActive("tab4");

                return;
            }

            HttpFileCollection files = Request.Files;

            Attachment att;
            AttachmentType attType;

            foreach (string file in files)
            {
                HttpPostedFile tempFile = files[file];

                attType = GetAttachmentType(Path.GetExtension(tempFile.FileName));
                if (tempFile.ContentLength > 0)
                {
                    if (tempFile.ContentLength > 500000)
                    {
                        divErrorSize.Visible = true;
                        break;
                    }
                    if (attType == null)
                    {
                        divErrorFileType.Visible = true;
                        break;
                    }
                    if (HasFileName(tempFile.FileName))
                    {
                        divErrorDuplicate.Visible = true;
                        break;
                    }
                    if (tempFile.FileName.Length > 50)
                    {
                        divErrorFileName.Visible = true;
                        break;
                    }

                    BinaryReader rdr = new BinaryReader(tempFile.InputStream);
                    byte[] buffer = rdr.ReadBytes(tempFile.ContentLength);

                    // Create the attachment
                    att = new Attachment
                    {
                        Patient = _patient,
                        Description = txtFileDescription.Value,
                        FileName = tempFile.FileName,
                        AttachmentType = attType,
                        Size = tempFile.ContentLength,
                        Content = buffer
                    };

                    UnitOfWork.Repository<Attachment>().Save(att);
                    UnitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Attachment record saved successfully";
                    Response.Cookies.Add(cookie);
                    Master.ShouldPopUpBeDisplayed();
                }
            }

            RenderGrids();
            SetActive("tab4");

            txtFileDescription.Value = string.Empty;
        }

        protected void ddlConditions_SelectedIndexChanged(object sender, EventArgs e)
        {
            int conditionId = Convert.ToInt32(ddlConditions.SelectedItem.Value);
            var conditionMedDraList = (from cmd in UnitOfWork.Repository<ConditionMedDra>().Queryable()
                                        .Where(cmd => cmd.Condition.Id == conditionId)
                                       orderby cmd.TerminologyMedDra.MedDraTerm ascending
                                       select cmd).ToList();

            ddlConditionMedDras.Items.Clear();

            ListItem citem;
            if (conditionMedDraList.Count() > 0)
            {
                foreach (ConditionMedDra cmd in conditionMedDraList)
                {
                    citem = new ListItem();
                    citem.Text = cmd.TerminologyMedDra.MedDraTerm;
                    citem.Value = cmd.TerminologyMedDra.Id.ToString();
                    ddlConditionMedDras.Items.Add(citem);
                }
            }

            if (ddlConditions.SelectedItem.Text == string.Empty)
                divConditionDetails.Visible = false;
            else
                divConditionDetails.Visible = true;

            // Populate eligible cohorts
            ddlCohort.Items.Clear();

            var cohortList = (from coh in UnitOfWork.Repository<CohortGroup>().Queryable()
                                        .Where(coh => coh.Condition.Id == conditionId)
                                       orderby coh.CohortName ascending
                                       select coh).ToList();

            var cohortText = cohortList.Count > 0 ? "" : "-- no cohorts available --";
            ddlCohort.Items.Add(new ListItem() { Selected = true, Text = cohortText, Value = "0" });

            ListItem cgitem;
            foreach (CohortGroup cg in cohortList)
            {
                cgitem = new ListItem();
                cgitem.Text = cg.CohortName;
                cgitem.Value = cg.Id.ToString();
                ddlCohort.Items.Add(cgitem);
            }

            txtCohortDateEnrollmentPatientAdd.Value = "";

        }

        protected void ddlCohort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCohort.SelectedValue != "0")
            {
                txtCohortDateEnrollmentPatientAdd.Value = DateTime.Today.ToString("yyyy-MM-dd");
            }
            else
            {
                txtCohortDateEnrollmentPatientAdd.Value = string.Empty;
            }
        }

        protected void btnCohortDeenrol_Click(object sender, EventArgs e)
        {
            CohortGroupEnrolment enrolment = null;

            if (txtCohortUIDDeenrol.Value != "0")
            {
                enrolment = GetCohortGroupEnrolment(Convert.ToInt32(txtCohortUIDDeenrol.Value));

                if (enrolment != null)
                {
                    enrolment.DeenroledDate = Convert.ToDateTime(txtCohortDateDeenrolment.Value);
                    UnitOfWork.Repository<CohortGroupEnrolment>().Update(enrolment);
                    UnitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Patient de-enrolled from cohort successfully";
                    Response.Cookies.Add(cookie);
                    Master.ShouldPopUpBeDisplayed();
                }
            }

            RenderGrids();
            SetActive("tab7");
        }

        protected void btnCohortRemove_Click(object sender, EventArgs e)
        {
            if (txtCohortUIDRemove.Value != "0")
            {
                var enrolment = GetCohortGroupEnrolment(Convert.ToInt32(txtCohortUIDRemove.Value));

                if (enrolment != null)
                {
                    var reason = txtCohortRemoveReason.Value.Trim() == "" ? "** NO REASON SPECIFIED ** " : txtCohortRemoveReason.Value;
                    enrolment.Archived = true;
                    enrolment.ArchivedReason = reason;
                    enrolment.AuditUser = GetCurrentUser();
                    enrolment.ArchivedDate = DateTime.Now;
                    UnitOfWork.Repository<CohortGroupEnrolment>().Update(enrolment);
                    UnitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Patient deleted from cohort successfully";
                    Response.Cookies.Add(cookie);
                    Master.ShouldPopUpBeDisplayed();
                }
            }

            RenderGrids();
            SetActive("tab7");
        }

        #endregion

        #region "EF"

        private Patient GetPatient(int id)
        {
            return
                UnitOfWork.Repository<Patient>()
                    .Queryable()
                    .Include(p1 => p1.Attachments)
                    .Include(p2 => p2.Encounters.Select(p2a => p2a.EncounterType))
                    .Include(p3 => p3.PatientClinicalEvents)
                    .Include(p4 => p4.PatientConditions.Select(p4a => p4a.Outcome))
                    .Include(p5 => p5.PatientFacilities)
                    .Include(p6 => p6.PatientLabTests.Select(p6a => p6a.TestUnit))
                    .Include(p7 => p7.PatientLanguages)
                    .Include(p8 => p8.PatientMedications)
                    .Include(p9 => p9.PatientStatusHistories)
                    .SingleOrDefault(u => u.Id == id && !u.Archived);
        }

        private Appointment GetAppointment(int id)
        {
            return UnitOfWork.Repository<Appointment>()
                    .Queryable()
                    .SingleOrDefault(u => u.Id == id && !u.Archived);
        }

        private Holiday GetHoliday(DateTime date)
        {
            return UnitOfWork.Repository<Holiday>()
                    .Queryable()
                    .SingleOrDefault(h => h.HolidayDate == date);
        }

        private PatientStatusHistory GetPatientStatusHistory(int id)
        {
            return UnitOfWork.Repository<PatientStatusHistory>()
                    .Queryable()
                    .SingleOrDefault(u => u.Id == id && !u.Archived);
        }

        private AttachmentType GetAttachmentType(string extension)
        {
            return UnitOfWork.Repository<AttachmentType>()
                .Queryable()
                .SingleOrDefault(u => u.Key == extension.Replace(".", ""));
        }

        private Facility GetFacility(int id)
        {
            return UnitOfWork.Repository<Facility>()
                    .Queryable()
                    .SingleOrDefault(u => u.Id == id);
        }

        private PatientCondition GetPatientCondition(int id)
        {
            return UnitOfWork.Repository<PatientCondition>()
                    .Queryable()
                    .SingleOrDefault(u => u.Id == id && !u.Archived);
        }

        private SelectionDataItem GetSelectionDataItem(string attributeKey, string selectionKey)
        {
            return UnitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(u => u.AttributeKey == attributeKey && u.SelectionKey == selectionKey);
        }

        private CustomAttributeConfiguration GetCustomAttribute(string attributeKey)
        {
            return UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(u => u.AttributeKey.Trim() == attributeKey.Trim());
        }

        private bool HasFileName(string fileName)
        {
            return (UnitOfWork.Repository<Attachment>().Queryable().Any(a => a.Patient.Id == _patient.Id && a.FileName == fileName && !a.Archived));
        }

        private User GetCurrentUser()
        {
            return UnitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
        }

        private CohortGroup GetCohortGroup(int id)
        {
            return UnitOfWork.Repository<CohortGroup>().Queryable().SingleOrDefault(cg => cg.Id == id);
        }

        private CohortGroupEnrolment GetCohortGroupEnrolment(int id)
        {
            return UnitOfWork.Repository<CohortGroupEnrolment>().Queryable().SingleOrDefault(cg => cg.Id == id);
        }

        private Boolean CheckDuplicate(string currentvalue, int fieldID)
        {
            var where = _patient!= null ? " c.Id <> " + _patient.Id + " and " : "";
            var sqlQuery = string.Format("SELECT * from patient c WHERE Archived = 0 and {2} " +
               "c.CustomAttributesXmlSerialised.value('(/CustomAttributeSet/CustomStringAttribute/Value)[{0}]', 'varchar(50)') = '{1}' ", fieldID, currentvalue, where);

            var patientQuery = UnitOfWork.Repository<Patient>().ExecuteSql(sqlQuery);

            if (patientQuery.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        #endregion

        #region "Internal Functions"

        private void ValidatePatientInformation()
        {
            // Reset error indicators
            lblFirstName.Attributes.Remove("class");
            lblFirstName.Attributes.Add("class", "input");
            lblMiddleName.Attributes.Remove("class");
            lblMiddleName.Attributes.Add("class", "input");
            lblSurname.Attributes.Remove("class");
            lblSurname.Attributes.Add("class", "input");
            lblDOB.Attributes.Remove("class");
            lblDOB.Attributes.Add("class", "input");
            lblFacility.Attributes.Remove("class");
            lblFacility.Attributes.Add("class", "input");
            lblConditionStartDate.Attributes.Remove("class");
            lblConditionStartDate.Attributes.Add("class", "input");
            lblConditionGroup.Attributes.Remove("class");
            lblConditionGroup.Attributes.Add("class", "input");
            lblMedDraTerm.Attributes.Remove("class");
            lblMedDraTerm.Attributes.Add("class", "input");
            lblEncounterDate.Attributes.Remove("class");
            lblEncounterDate.Attributes.Add("class", "input");
            lblCohortEnrollmentDate.Attributes.Remove("class");
            lblCohortEnrollmentDate.Attributes.Add("class", "input");

            // Patient Information Validation
            if (txtFirstName.Value.Trim() == "")
            {
                lblFirstName.Attributes.Remove("class");
                lblFirstName.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "First Name is required";
                lblFirstName.Controls.Add(errorMessageDiv);

                diverror.Visible = true;
            }
            else
            {
                var firstName = txtFirstName.Value.Trim();
                if (Regex.Matches(firstName, @"[a-zA-Z ']").Count < firstName.Length)
                {
                    lblFirstName.Attributes.Remove("class");
                    lblFirstName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "First Name contains invalid characters (Enter A-Z, a-z, space)";
                    lblFirstName.Controls.Add(errorMessageDiv);

                    diverror.Visible = true;
                }
            }

            if (txtMiddleName.Value.Trim() != "")
            {
                var middle = txtMiddleName.Value.Trim();
                if (Regex.Matches(middle, @"[a-zA-Z ]").Count < middle.Length)
                {
                    lblMiddleName.Attributes.Remove("class");
                    lblMiddleName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Middle Name contains invalid characters (Enter A-Z, a-z, space)";
                    lblMiddleName.Controls.Add(errorMessageDiv);

                    diverror.Visible = true;
                }
            }

            if (txtSurname.Value.Trim() == "")
            {
                lblSurname.Attributes.Remove("class");
                lblSurname.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Last Name is required";
                lblSurname.Controls.Add(errorMessageDiv);

                diverror.Visible = true;
            }
            else
            {
                var surname = txtSurname.Value.Trim();
                if (Regex.Matches(surname, @"[a-zA-Z ]").Count < surname.Length)
                {
                    lblSurname.Attributes.Remove("class");
                    lblSurname.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Last Name contains invalid characters (Enter A-Z, a-z, space)";
                    lblSurname.Controls.Add(errorMessageDiv);

                    diverror.Visible = true;
                }
            }

            if (txtDOB.Value.Trim() != "")
            {
                if (DateTime.TryParse(txtDOB.Value, out _tempDOB))
                {
                    _tempDOB = Convert.ToDateTime(txtDOB.Value);
                    if (_tempDOB > DateTime.Today)
                    {
                        lblDOB.Attributes.Remove("class");
                        lblDOB.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date of Birth should be before current date";
                        lblDOB.Controls.Add(errorMessageDiv);

                        diverror.Visible = true;
                    }
                    if (_tempDOB < DateTime.Today.AddYears(-120))
                    {
                        lblDOB.Attributes.Remove("class");
                        lblDOB.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date of Birth cannot be so far in the past";
                        lblDOB.Controls.Add(errorMessageDiv);

                        diverror.Visible = true;
                    }
                }
                else
                {
                    lblDOB.Attributes.Remove("class");
                    lblDOB.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Date of Birth has an invalid date format";
                    lblDOB.Controls.Add(errorMessageDiv);

                    diverror.Visible = true;
                }
            }
            else
            {
                lblDOB.Attributes.Remove("class");
                lblDOB.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Date of Birth is required";
                lblDOB.Controls.Add(errorMessageDiv);

                diverror.Visible = true;
            }

            if (ddlFacility.SelectedValue == "0")
            {
                lblFacility.Attributes.Remove("class");
                lblFacility.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Facility is required";
                lblFacility.Controls.Add(errorMessageDiv);

                diverror.Visible = true;
            }
        }

        private void ValidatePatientAddition()
        {
            // Additional information validation for new patient addition
            if (_formMode == FormMode.AddMode)
            {
                // Check additional information
                if (ddlConditions.SelectedValue == "0")
                {
                    lblConditionGroup.Attributes.Remove("class");
                    lblConditionGroup.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Condition Group must be selected for a new patient";
                    lblConditionGroup.Controls.Add(errorMessageDiv);

                    diverror.Visible = true;
                }

                if (ddlConditionMedDras.SelectedItem != null) {
                    _sourceTerm = UnitOfWork.Repository<TerminologyMedDra>().Get(Convert.ToInt32(ddlConditionMedDras.SelectedItem.Value));
                }

                if (_sourceTerm == null)
                {
                    lblMedDraTerm.Attributes.Remove("class");
                    lblMedDraTerm.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "MedDRA Term must be selected for a new patient";
                    lblMedDraTerm.Controls.Add(errorMessageDiv);

                    diverror.Visible = true;
                }
                else
                {
                    // validate dates
                    if (ddlCohort.SelectedValue != "0")
                    {
                        if (txtCohortDateEnrollmentPatientAdd.Value.Trim() == "")
                        {
                            lblCohortEnrollmentDate.Attributes.Remove("class");
                            lblCohortEnrollmentDate.Attributes.Add("class", "input state-error");
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = "Cohort Enrollment Date must be specified if cohort selected";
                            lblCohortEnrollmentDate.Controls.Add(errorMessageDiv);

                            diverror.Visible = true;
                        }
                        else
                        {
                            if (DateTime.TryParse(txtCohortDateEnrollmentPatientAdd.Value, out _tempCohortEnrollmentDate))
                            {
                                _tempCohortEnrollmentDate = Convert.ToDateTime(txtCohortDateEnrollmentPatientAdd.Value);
                                if (_tempCohortEnrollmentDate > DateTime.Today)
                                {
                                    lblCohortEnrollmentDate.Attributes.Remove("class");
                                    lblCohortEnrollmentDate.Attributes.Add("class", "input state-error");
                                    var errorMessageDiv = new HtmlGenericControl("div");
                                    errorMessageDiv.Attributes.Add("class", "note note-error");
                                    errorMessageDiv.InnerText = "Cohort Enrollment Date should be before current date";
                                    lblCohortEnrollmentDate.Controls.Add(errorMessageDiv);

                                    diverror.Visible = true;
                                }
                                if (_tempDOB != DateTime.MinValue)
                                {
                                    if (_tempCohortEnrollmentDate < _tempDOB)
                                    {
                                        lblCohortEnrollmentDate.Attributes.Remove("class");
                                        lblCohortEnrollmentDate.Attributes.Add("class", "input state-error");
                                        var errorMessageDiv = new HtmlGenericControl("div");
                                        errorMessageDiv.Attributes.Add("class", "note note-error");
                                        errorMessageDiv.InnerText = "Cohort Enrollment Date should be after date of birth";
                                        lblCohortEnrollmentDate.Controls.Add(errorMessageDiv);

                                        diverror.Visible = true;
                                    }
                                }
                            }
                            else
                            {
                                lblCohortEnrollmentDate.Attributes.Remove("class");
                                lblCohortEnrollmentDate.Attributes.Add("class", "input state-error");
                                var errorMessageDiv = new HtmlGenericControl("div");
                                errorMessageDiv.Attributes.Add("class", "note note-error");
                                errorMessageDiv.InnerText = "Cohort Enrollment Date has an invalid date format";
                                lblCohortEnrollmentDate.Controls.Add(errorMessageDiv);

                                diverror.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        if (txtCohortDateEnrollmentPatientAdd.Value.Trim() != "")
                        {
                            lblCohortEnrollmentDate.Attributes.Remove("class");
                            lblCohortEnrollmentDate.Attributes.Add("class", "input state-error");
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = "Cohort Enrollment Date must not be specified if no cohort selected";
                            lblCohortEnrollmentDate.Controls.Add(errorMessageDiv);

                            diverror.Visible = true;
                        }
                    }

                    if (txtStartDate.Value.Trim() != "")
                    {
                        if (DateTime.TryParse(txtStartDate.Value, out _tempConditionStartDate))
                        {
                            _tempConditionStartDate = Convert.ToDateTime(txtStartDate.Value);
                            if (_tempConditionStartDate > DateTime.Today)
                            {
                                lblConditionStartDate.Attributes.Remove("class");
                                lblConditionStartDate.Attributes.Add("class", "input state-error");
                                var errorMessageDiv = new HtmlGenericControl("div");
                                errorMessageDiv.Attributes.Add("class", "note note-error");
                                errorMessageDiv.InnerText = "Condition Start Date should be before current date";
                                lblConditionStartDate.Controls.Add(errorMessageDiv);

                                diverror.Visible = true;
                            }
                            if (_tempDOB != DateTime.MinValue)
                            {
                                if (_tempConditionStartDate < _tempDOB)
                                {
                                    lblConditionStartDate.Attributes.Remove("class");
                                    lblConditionStartDate.Attributes.Add("class", "input state-error");
                                    var errorMessageDiv = new HtmlGenericControl("div");
                                    errorMessageDiv.Attributes.Add("class", "note note-error");
                                    errorMessageDiv.InnerText = "Condition Start Date should be after date of birth";
                                    lblConditionStartDate.Controls.Add(errorMessageDiv);

                                    diverror.Visible = true;
                                }
                            }
                        }
                        else
                        {
                            lblConditionStartDate.Attributes.Remove("class");
                            lblConditionStartDate.Attributes.Add("class", "input state-error");
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = "Condition Start Date has an invalid date format";
                            lblConditionStartDate.Controls.Add(errorMessageDiv);

                            diverror.Visible = true;
                        }
                    }
                    else
                    {
                        lblConditionStartDate.Attributes.Remove("class");
                        lblConditionStartDate.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Condition Start Date is required";
                        lblConditionStartDate.Controls.Add(errorMessageDiv);

                        diverror.Visible = true;
                    }
                    if (txtOutcomeDate.Value.Trim() != "")
                    {
                        if (DateTime.TryParse(txtOutcomeDate.Value, out _tempConditionEndDate))
                        {
                            _tempConditionEndDate = Convert.ToDateTime(txtOutcomeDate.Value);
                            if (_tempConditionEndDate > DateTime.Today)
                            {
                                lblConditionEndDate.Attributes.Remove("class");
                                lblConditionEndDate.Attributes.Add("class", "input state-error");
                                var errorMessageDiv = new HtmlGenericControl("div");
                                errorMessageDiv.Attributes.Add("class", "note note-error");
                                errorMessageDiv.InnerText = "Condition End Date should be before current date";
                                lblConditionEndDate.Controls.Add(errorMessageDiv);

                                diverror.Visible = true;
                            }

                            if (_tempConditionStartDate != DateTime.MinValue)
                            {
                                if (_tempConditionEndDate < _tempConditionStartDate)
                                {
                                    lblConditionEndDate.Attributes.Remove("class");
                                    lblConditionEndDate.Attributes.Add("class", "input state-error");
                                    var errorMessageDiv = new HtmlGenericControl("div");
                                    errorMessageDiv.Attributes.Add("class", "note note-error");
                                    errorMessageDiv.InnerText = "Condition End Date should be after Start Date";
                                    lblConditionEndDate.Controls.Add(errorMessageDiv);

                                    diverror.Visible = true;
                                }
                            }
                        }
                        else
                        {
                            lblConditionEndDate.Attributes.Remove("class");
                            lblConditionEndDate.Attributes.Add("class", "input state-error");
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = "Condition End Date has an invalid date format";
                            lblConditionEndDate.Controls.Add(errorMessageDiv);

                            diverror.Visible = true;
                        }
                    }
                }
                if (txtEncounterDate.Value.Trim() != "")
                {
                    if (DateTime.TryParse(txtEncounterDate.Value, out _tempEncounterDate))
                    {
                        _tempEncounterDate = Convert.ToDateTime(txtEncounterDate.Value);
                        if (_tempEncounterDate > DateTime.Today)
                        {
                            lblEncounterDate.Attributes.Remove("class");
                            lblEncounterDate.Attributes.Add("class", "input state-error");
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = "Encounter Date should be before current date";
                            lblEncounterDate.Controls.Add(errorMessageDiv);

                            diverror.Visible = true;
                        }
                    }
                    else
                    {
                        lblEncounterDate.Attributes.Remove("class");
                        lblEncounterDate.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Encounter Date has an invalid date format";
                        lblEncounterDate.Controls.Add(errorMessageDiv);

                        diverror.Visible = true;
                    }
                }
                else
                {
                    lblEncounterDate.Attributes.Remove("class");
                    lblEncounterDate.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Encounter Date is required";
                    lblEncounterDate.Controls.Add(errorMessageDiv);

                    diverror.Visible = true;
                }
            }
        }

        private void ValidateCustomAttributes()
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;

            string val; // field value
            string valf; // field text 

            int textBoxIndex = 0;

            IExtendable patientExtended = null;
            patientExtended = _patient;
            
            TextBox customAttributeValueTextBox;
            DropDownList ddl;

            foreach (TableRow row in dt_1.Rows)
            {
                if (row is TableHeaderRow) continue;

                // Get attribute
                con = GetCustomAttribute(row.Cells[0].Text);
                var customAttributeLabel = row.Cells[1].Controls[0] as Label;

                if (customAttributeLabel != null)
                {
                    // Clear error messages
                    customAttributeLabel.Attributes.Remove("class");
                    if (con.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        customAttributeLabel.Attributes.Add("class", "select");
                    }
                    else
                    {
                        customAttributeLabel.Attributes.Add("class", "input");
                    }
                }

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                textBoxIndex = con.IsRequired ? 1 : 0;

                // Get value
                val = "";
                valf = "";
                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.String:
                    case CustomAttributeType.DateTime:
                    case CustomAttributeType.Numeric:
                        customAttributeValueTextBox = (TextBox)row.Cells[1].Controls[0].Controls[textBoxIndex];
                        val = customAttributeValueTextBox.Text;
                        valf = "";
                        break;
                    case CustomAttributeType.Selection:
                        ddl = (DropDownList)row.Cells[1].Controls[0].Controls[0];
                        val = ddl.SelectedValue;
                        valf = ddl.SelectedItem.Text;
                        break;
                }

                if (string.IsNullOrWhiteSpace(val) && con.IsRequired)
                {
                    if (customAttributeLabel != null)
                    {
                        customAttributeLabel.Attributes.Remove("class");
                        if (con.CustomAttributeType == CustomAttributeType.Selection)
                        {
                            customAttributeLabel.Attributes.Add("class", "select state-error");
                        }
                        else
                        {
                            customAttributeLabel.Attributes.Add("class", "input state-error");
                        }
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = string.Format("{0} is required", con.AttributeKey);
                        customAttributeLabel.Controls.Add(errorMessageDiv);
                    }

                    diverror.Visible = true;
                }

                if (!string.IsNullOrWhiteSpace(val) && con.AttributeKey == "Medical Record Number") //validate facility number first
                {
                    if (Regex.Matches(val, @"[a-zA-Z0-9-]").Count < val.Length)
                    {
                        if (customAttributeLabel != null)
                        {
                            customAttributeLabel.Attributes.Remove("class");
                            if (con.CustomAttributeType == CustomAttributeType.Selection)
                            {
                                customAttributeLabel.Attributes.Add("class", "select state-error");
                            }
                            else
                            {
                                customAttributeLabel.Attributes.Add("class", "input state-error");
                            }
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = string.Format("{0} contains invalid characters (Enter A-Z, a-z, 0-9, -)", "Medical Record Number");
                            customAttributeLabel.Controls.Add(errorMessageDiv);
                        }

                        diverror.Visible = true;
                    }
                    else
                    {
                        if (CheckDuplicate(val, 1)) //check if it is not duplicate returns true if it is a duplicate facility nuber is key element number 1
                        {
                            if (customAttributeLabel != null)
                            {
                                customAttributeLabel.Attributes.Remove("class");
                                customAttributeLabel.Attributes.Add("class", "input state-error");
                                var errorMessageDiv = new HtmlGenericControl("div");
                                errorMessageDiv.Attributes.Add("class", "note note-error");
                                errorMessageDiv.InnerText = string.Format("{0} already exists.", "Medical Record Number");
                                customAttributeLabel.Controls.Add(errorMessageDiv);
                            }

                            diverror.Visible = true;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(val) && con.AttributeKey == "Patient Identity Number") // check id number
                {
                    if (Regex.Matches(val, @"[a-zA-Z0-9-]").Count < val.Length)
                    {
                        if (customAttributeLabel != null)
                        {
                            customAttributeLabel.Attributes.Remove("class");
                            if (con.CustomAttributeType == CustomAttributeType.Selection)
                            {
                                customAttributeLabel.Attributes.Add("class", "select state-error");
                            }
                            else
                            {
                                customAttributeLabel.Attributes.Add("class", "input state-error");
                            }
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = string.Format("{0} contains invalid characters (Enter A-Z, a-z, 0-9, -)", con.AttributeKey);
                            customAttributeLabel.Controls.Add(errorMessageDiv);
                        }

                        diverror.Visible = true;
                    }
                    else
                    {
                        if (CheckDuplicate(val, 2)) //check if it is not duplicate returns true if it is a duplicate identity number is key element 2
                        {
                            if (customAttributeLabel != null)
                            {
                                customAttributeLabel.Attributes.Remove("class");
                                customAttributeLabel.Attributes.Add("class", "input state-error");
                                var errorMessageDiv = new HtmlGenericControl("div");
                                errorMessageDiv.Attributes.Add("class", "note note-error");
                                errorMessageDiv.InnerText = string.Format("{0} already exists.", "Patient Identity Number");
                                customAttributeLabel.Controls.Add(errorMessageDiv);
                            }

                            diverror.Visible = true;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(val) && con.AttributeKey == "Patient Contact Number")
                {
                    if (!Regex.IsMatch(val, @"^[0-9+]*$"))
                    {
                        if (customAttributeLabel != null)
                        {
                            customAttributeLabel.Attributes.Remove("class");
                            if (con.CustomAttributeType == CustomAttributeType.Selection)
                            {
                                customAttributeLabel.Attributes.Add("class", "select state-error");
                            }
                            else
                            {
                                customAttributeLabel.Attributes.Add("class", "input state-error");
                            }
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = string.Format("{0} contains invalid characters (Enter 0-9, +)", con.AttributeKey);
                            customAttributeLabel.Controls.Add(errorMessageDiv);
                        }

                        diverror.Visible = true;
                    }
                }
            }
        }

        private void ValidatePatientAdditionCustomAttributes()
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;

            string val; // field value
            string valf; // field text 
            string vala; // current attribute value

            int textBoxIndex = 0;

            TextBox customAttributeValueTextBox;
            DropDownList ddl;

            foreach (TableRow row in dtConditionAttributes.Rows)
            {
                if (row is TableHeaderRow) continue;

                // Get attribute
                con = GetCustomAttribute(row.Cells[0].Text);
                var customAttributeLabel = row.Cells[1].Controls[0] as Label;

                if (customAttributeLabel != null)
                {
                    // Clear error messages
                    customAttributeLabel.Attributes.Remove("class");
                    if (con.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        customAttributeLabel.Attributes.Add("class", "select");
                    }
                    else
                    {
                        customAttributeLabel.Attributes.Add("class", "input");
                    }
                }

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                textBoxIndex = con.IsRequired ? 1 : 0;

                // Get value
                val = "";
                valf = "";
                vala = "";

                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.String:
                    case CustomAttributeType.DateTime:
                    case CustomAttributeType.Numeric:
                        customAttributeValueTextBox = (TextBox)row.Cells[1].Controls[0].Controls[textBoxIndex];
                        val = customAttributeValueTextBox.Text;
                        valf = "";
                        break;
                    case CustomAttributeType.Selection:
                        ddl = (DropDownList)row.Cells[1].Controls[0].Controls[0];
                        val = ddl.SelectedValue;
                        valf = ddl.SelectedItem.Text;
                        break;
                }

                if (string.IsNullOrWhiteSpace(val) && con.IsRequired)
                {
                    if (customAttributeLabel != null)
                    {
                        customAttributeLabel.Attributes.Remove("class");
                        if (con.CustomAttributeType == CustomAttributeType.Selection)
                        {
                            customAttributeLabel.Attributes.Add("class", "select state-error");
                        }
                        else
                        {
                            customAttributeLabel.Attributes.Add("class", "input state-error");
                        }
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = string.Format("{0} is required", con.AttributeKey);
                        customAttributeLabel.Controls.Add(errorMessageDiv);

                    }

                    diverror.Visible = true;
                }
            }
        }

        private void SaveCustomAttributes()
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;

            int textBoxIndex = 0;
            
            string val; // field value
            string valf; // field text 
            string vala; // current attribute value
            
            TextBox customAttributeValueTextBox;
            DropDownList ddl;
            
            IExtendable patientExtended = null;
            patientExtended = _patient;
            PatientCondition patientCondition = null;
            IExtendable patientConditionExtended = null;

            foreach (TableRow row in dt_1.Rows)
            {
                if (row is TableHeaderRow) continue;

                // Get attribute
                con = GetCustomAttribute(row.Cells[0].Text);
                var customAttributeLabel = row.Cells[1].Controls[0] as Label;

                if (customAttributeLabel != null)
                {
                    // Clear error messages
                    customAttributeLabel.Attributes.Remove("class");
                    if (con.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        customAttributeLabel.Attributes.Add("class", "select");
                    }
                    else
                    {
                        customAttributeLabel.Attributes.Add("class", "input");
                    }
                }

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                textBoxIndex = con.IsRequired ? 1 : 0;

                // Get value
                val = "";
                valf = "";
                vala = "";
                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.String:
                    case CustomAttributeType.DateTime:
                    case CustomAttributeType.Numeric:
                        customAttributeValueTextBox = (TextBox)row.Cells[1].Controls[0].Controls[textBoxIndex];
                        val = customAttributeValueTextBox.Text;
                        valf = "";
                        break;
                    case CustomAttributeType.Selection:
                        ddl = (DropDownList)row.Cells[1].Controls[0].Controls[0];
                        val = ddl.SelectedValue;
                        valf = ddl.SelectedItem.Text;
                        break;
                }

                var cattr = patientExtended.GetAttributeValue(row.Cells[0].Text);

                if (cattr != null)
                    vala = cattr.ToString();

                if (vala != val)
                {
                    customAttributeLabel.Attributes.Remove("class");
                    customAttributeLabel.Attributes.Add("class", "input");

                    // Store new value
                    try
                    {
                        switch (con.CustomAttributeType)
                        {
                            case CustomAttributeType.Numeric:
                                patientExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Decimal.Parse(val),
                                    GetCurrentUser().UserName);
                                break;
                            case CustomAttributeType.String:
                                patientExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, val, GetCurrentUser().UserName);
                                break;
                            case CustomAttributeType.Selection:
                                patientExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Int32.Parse(val),
                                    GetCurrentUser().UserName);
                                break;
                            case CustomAttributeType.DateTime:
                                if (!string.IsNullOrWhiteSpace(val))
                                {
                                    patientExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, DateTime.Parse(val), GetCurrentUser().UserName);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (CustomAttributeValidationException validationException)
                    {
                        if (customAttributeLabel != null)
                        {
                            customAttributeLabel.Attributes.Remove("class");
                            if (con.CustomAttributeType == CustomAttributeType.Selection)
                            {
                                customAttributeLabel.Attributes.Add("class", "select state-error");
                            }
                            else
                            {
                                customAttributeLabel.Attributes.Add("class", "input state-error");
                            }
                            var errorMessageDiv = new HtmlGenericControl("div");
                            errorMessageDiv.Attributes.Add("class", "note note-error");
                            errorMessageDiv.InnerText = validationException.Message;
                            customAttributeLabel.Controls.Add(errorMessageDiv);
                        }

                        continue;
                    }
                }
            }
        }

        private void SavePatientAddition()
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;

            string val; // field value
            string valf; // field text 
            string vala; // current attribute value

            int textBoxIndex = 0;

            PatientCondition patientCondition = null;
            IExtendable patientConditionExtended = null;

            TextBox customAttributeValueTextBox;
            DropDownList ddl;

            // Add new encounter
            var etid = Convert.ToInt32(ddlEncounterType.SelectedValue);
            var pid = Convert.ToInt32(ddlPriority.SelectedValue);
            var encounterType = UnitOfWork.Repository<EncounterType>().Queryable().SingleOrDefault(et => et.Id == etid);
            var priority = UnitOfWork.Repository<Priority>().Queryable().SingleOrDefault(p => p.Id == pid);
            var currentUser = UnitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == User.Identity.Name);

            var newEncounter = new Encounter(_patient)
            {
                EncounterType = encounterType,
                Priority = priority,
                EncounterDate = Convert.ToDateTime(txtEncounterDate.Value)
            };

            UnitOfWork.Repository<Encounter>().Save(newEncounter);
            _url = String.Format("/Encounter/ViewEncounter/" + newEncounter.Id.ToString());

            var encounterTypeWorkPlan = UnitOfWork.Repository<EncounterTypeWorkPlan>()
                .Queryable()
                .Include("WorkPlan.Dataset")
                .Where(et => et.EncounterType.Id == encounterType.Id)
                .SingleOrDefault();

            if (encounterTypeWorkPlan != null)
            {
                // Create a new instance
                var dataset = UnitOfWork.Repository<Dataset>()
                    .Queryable()
                    .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType")
                    .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType")
                    .SingleOrDefault(d => d.Id == encounterTypeWorkPlan.WorkPlan.Dataset.Id);

                if (dataset != null)
                {
                    var datasetInstance = dataset.CreateInstance(newEncounter.Id, encounterTypeWorkPlan);

                    UnitOfWork.Repository<DatasetInstance>().Save(datasetInstance);
                }
            }

            // Add new condition
            patientCondition = new PatientCondition
            {
                Patient = _patient,

                TerminologyMedDra = _sourceTerm,
                Comments = txtComments.Text,
                DateStart = Convert.ToDateTime(txtStartDate.Value),
            };

            if (txtOutcomeDate.Value != "")
                patientCondition.OutcomeDate = Convert.ToDateTime(txtOutcomeDate.Value);
            UnitOfWork.Repository<PatientCondition>().Save(patientCondition);

            patientConditionExtended = (IExtendable)patientCondition;

            foreach (TableRow row in dtConditionAttributes.Rows)
            {
                if (row is TableHeaderRow) continue;

                // Get attribute
                con = GetCustomAttribute(row.Cells[0].Text);
                var customAttributeLabel = row.Cells[1].Controls[0] as Label;

                if (customAttributeLabel != null)
                {
                    // Clear error messages
                    customAttributeLabel.Attributes.Remove("class");
                    if (con.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        customAttributeLabel.Attributes.Add("class", "select");
                    }
                    else
                    {
                        customAttributeLabel.Attributes.Add("class", "input");
                    }
                }

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                textBoxIndex = con.IsRequired ? 1 : 0;

                // Get value
                val = "";
                valf = "";
                vala = "";
                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.String:
                    case CustomAttributeType.DateTime:
                    case CustomAttributeType.Numeric:
                        customAttributeValueTextBox = (TextBox)row.Cells[1].Controls[0].Controls[textBoxIndex];
                        val = customAttributeValueTextBox.Text;
                        valf = "";
                        break;
                    case CustomAttributeType.Selection:
                        ddl = (DropDownList)row.Cells[1].Controls[0].Controls[0];
                        val = ddl.SelectedValue;
                        valf = ddl.SelectedItem.Text;
                        break;
                }

                var cattr = patientConditionExtended.GetAttributeValue(row.Cells[0].Text);

                // Store new value
                try
                {
                    switch (con.CustomAttributeType)
                    {
                        case CustomAttributeType.Numeric:
                            patientConditionExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Decimal.Parse(val),
                                GetCurrentUser().UserName);
                            break;
                        case CustomAttributeType.String:
                            patientConditionExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, val, GetCurrentUser().UserName);
                            break;
                        case CustomAttributeType.Selection:
                            patientConditionExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Int32.Parse(val),
                                GetCurrentUser().UserName);
                            break;
                        case CustomAttributeType.DateTime:
                            if (!string.IsNullOrWhiteSpace(val))
                            {
                                patientConditionExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, DateTime.Parse(val), GetCurrentUser().UserName);
                            }
                            break;
                        default:
                            break;
                    }

                    UnitOfWork.Repository<PatientCondition>().Update(patientCondition);
                }
                catch (CustomAttributeValidationException validationException)
                {
                    if (customAttributeLabel != null)
                    {
                        customAttributeLabel.Attributes.Remove("class");
                        if (con.CustomAttributeType == CustomAttributeType.Selection)
                        {
                            customAttributeLabel.Attributes.Add("class", "select state-error");
                        }
                        else
                        {
                            customAttributeLabel.Attributes.Add("class", "input state-error");
                        }
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = validationException.Message;
                        customAttributeLabel.Controls.Add(errorMessageDiv);
                    }

                    continue;
                }
            }

            // Enrol into cohort
            if (ddlCohort.SelectedValue != "0")
            {
                CohortGroup cohort = null;
                CohortGroupEnrolment enrolment = null;

                cohort = GetCohortGroup(Convert.ToInt32(ddlCohort.SelectedValue));

                if (cohort != null)
                {
                    enrolment = new CohortGroupEnrolment()
                    {
                        CohortGroup = cohort,
                        EnroledDate = Convert.ToDateTime(txtCohortDateEnrollmentPatientAdd.Value),
                        Patient = _patient
                    };

                    UnitOfWork.Repository<CohortGroupEnrolment>().Save(enrolment);
                }
            }
        }

        #endregion

    }
}
