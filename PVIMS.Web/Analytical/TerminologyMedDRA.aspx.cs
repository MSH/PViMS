using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;

namespace PVIMS.Web
{
    public partial class TerminologyMedDRA : MainPageBase
    {
        private enum FormMode { ActiveMode = 1, SpontaneousMode = 2 };
        private FormMode _formMode = FormMode.ActiveMode;

        private int _rid;
        private ReportInstance _reportInstance;

        private PatientClinicalEvent _clinicalEvent;
        private DatasetInstance _instance;

        public IInfrastructureService _infrastuctureService { get; set; }
        public IWorkFlowService _workflowService { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "MedDRA Terminology", SubTitle = "", Icon = "fa fa-dashboard fa-fw", MetaPageId = 0 });

            _clinicalEvent = null;
            _instance = null;

            if (Request.QueryString["rid"] != null)
            {
                _rid = Convert.ToInt32(Request.QueryString["rid"]);
                if (_rid > 0) 
                {
                    _reportInstance = UnitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.Id == _rid);

                    if(_reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
                    {
                        Master.MainMenu.SetActive("ActiveReporting");

                        _clinicalEvent = UnitOfWork.Repository<PatientClinicalEvent>().Queryable().Include(pce => pce.Patient).SingleOrDefault(pce => pce.PatientClinicalEventGuid == _reportInstance.ContextGuid);
                        _formMode = FormMode.ActiveMode;
                    }
                    else
                    {
                        Master.MainMenu.SetActive("SpontaneousReporting");

                        _instance = UnitOfWork.Repository<DatasetInstance>().Queryable().SingleOrDefault(di => di.DatasetInstanceGuid == _reportInstance.ContextGuid);
                        _formMode = FormMode.SpontaneousMode;
                    }
                }
                else
                {
                    throw new Exception("rid parameter not passed");
                }
            }

            LoadCommonItems();
            RenderButtons();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) {
                ddlSOC.Items.Add("---");
                foreach(var term in UnitOfWork.Repository<TerminologyMedDra>().Queryable().Where(u => u.MedDraTermType == "SOC").ToList()) {
                    ddlSOC.Items.Add(new ListItem() { Text = term.MedDraTerm, Value = term.Id.ToString() });
                }
                if (ddlSOC.Items.Count > 0) {
                    ddlSOC.SelectedIndex = 0;
                }

                if(_formMode == FormMode.ActiveMode)
                {
                    lblVerbatimLabel.InnerHtml = "<b> Facility Level Verbatim Description </b>";
                    lblVerbatim.InnerText = _clinicalEvent.SourceDescription;
                    if (_clinicalEvent.SourceTerminologyMedDra != null) { lblSource.InnerText = _clinicalEvent.SourceTerminologyMedDra.DisplayName; };
                }
                else
                {
                    lblVerbatimLabel.InnerHtml = "<b> Reporter Verbatim Description </b>";
                    var source = _instance.GetInstanceValue(UnitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Description of reaction"));
                    if (source != string.Empty)
                    {
                        lblVerbatim.InnerText = source;
                    };
                    divSource.Style["display"] = "none";
                }
            }
        }

        private void RenderButtons()
        {
            Button btn;
            HyperLink hyp;

            btn = new Button();
            btn.ID = "btnSave";
            btn.CssClass = "btn btn-primary";
            btn.Text = "Save";
            btn.Click += btnSave_Click;
            btn.Attributes.Add("formnovalidate", "formnovalidate");
            spnButtons.Controls.Add(btn);

            if(_formMode == FormMode.ActiveMode)
            {
                hyp = new HyperLink()
                {
                    ID = "btnReturn",
                    NavigateUrl = "/Analytical/ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219",
                    CssClass = "btn btn-default",
                    Text = "Return"
                };
            }
            else
            {
                hyp = new HyperLink()
                {
                    ID = "btnReturn",
                    NavigateUrl = "/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986",
                    CssClass = "btn btn-default",
                    Text = "Return"
                };
            }
            spnButtons.Controls.Add(hyp); 
            
            spnButtons.Visible = true;
        }

        private void LoadCommonItems()
        {
            ListItem listItem;

            var items = UnitOfWork.Repository<TerminologyMedDra>().Queryable().Where(tm => tm.Common == true).OrderBy(tm => tm.MedDraTerm).ToList();

            foreach(TerminologyMedDra item in items)
            {
                listItem = new ListItem()
                {
                    Text = item.MedDraTerm,
                    Value = item.Id.ToString()
                };
                lstCommon.Items.Add(listItem);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblError.Attributes.Remove("class");
            lblError.Attributes.Add("class", "input");

            var terminologyId = 0;
            if(tab1.Attributes["class"] == "tab-pane active smart-form")
            {
                if (lstCommon.SelectedIndex > -1) {
                    terminologyId = Convert.ToInt32(lstCommon.SelectedItem.Value);
                }
            }
            if (tab2.Attributes["class"] == "tab-pane active smart-form")
            {
                if (lstLLT.SelectedIndex > -1) {
                    terminologyId = Convert.ToInt32(lstLLT.SelectedItem.Value);
                }
                else
                {
                    if (ddlPT.SelectedValue != "" && ddlPT.SelectedValue != "---") {
                        terminologyId = Convert.ToInt32(ddlPT.SelectedValue);
                    }
                    else
                    {
                        if (ddlHLT.SelectedValue != "" && ddlHLT.SelectedValue != "---") {
                            terminologyId = Convert.ToInt32(ddlHLT.SelectedValue);
                        }
                        else
                        {
                            if (ddlHLGT.SelectedValue != "" && ddlHLGT.SelectedValue != "---") {
                                terminologyId = Convert.ToInt32(ddlHLGT.SelectedValue);
                            }
                            else
                            {
                                if (ddlSOC.SelectedValue != "" && ddlSOC.SelectedValue != "---") {
                                    terminologyId = Convert.ToInt32(ddlSOC.SelectedValue);
                                }
                            }
                        }
                    }
                }
            }
            if (tab3.Attributes["class"] == "tab-pane active smart-form")
            {
                if (lstTermResult.SelectedIndex > -1) {
                    terminologyId = Convert.ToInt32(lstTermResult.SelectedItem.Value);
                }
            }
            if (tab4.Attributes["class"] == "tab-pane active smart-form")
            {
                if (lstCodeResult.SelectedIndex > -1) {
                    terminologyId = Convert.ToInt32(lstCodeResult.SelectedItem.Value);
                }
            }
            if (terminologyId == 0) { return; };

            var term = GetTerminologyMedDraById(terminologyId);

            _reportInstance.TerminologyMedDra = term;

            try
            {
                UnitOfWork.Repository<ReportInstance>().Update(_reportInstance);
                UnitOfWork.Complete();

                _workflowService.ExecuteActivity(_reportInstance.ContextGuid, "MEDDRASET", "AUTOMATION: MedDRA Term set", null, "");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        lblError.InnerHtml += String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            HttpCookie cookie = new HttpCookie("PopUpMessage");
            cookie.Value = "MedDRA terminology set successfully";
            Response.Cookies.Add(cookie);
            Master.ShouldPopUpBeDisplayed();

            spnButtons.Controls[1].Visible = false;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lstTermResult.Items.Clear();

            bool err = false;

            lblTerm.Attributes.Remove("class");
            lblTerm.Attributes.Add("class", "input");

            if (!String.IsNullOrWhiteSpace(txtTerm.Value))
            {
                if (Regex.Matches(txtTerm.Value, @"[a-zA-Z ]").Count < txtTerm.Value.Length)
                {
                    lblTerm.Attributes.Remove("class");
                    lblTerm.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Search term contains invalid characters (Enter A-Z, a-z, space)";
                    lblTerm.Controls.Add(errorMessageDiv);

                    err = true;
                }
                else
                {
                    if (txtTerm.Value.Trim().Length < 3)
                    {
                        lblTerm.Attributes.Remove("class");
                        lblTerm.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search term must contain at least 3 characters";
                        lblTerm.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
            }
            else
            {
                lblTerm.Attributes.Remove("class");
                lblTerm.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Search term must be entered";
                lblTerm.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (err)
            {
                SetActive("tab3");
                return;
            };

            var terms = UnitOfWork.Repository<TerminologyMedDra>().Queryable().Where(u => u.MedDraTerm.Contains(txtTerm.Value) && u.MedDraTermType == ddlTermType.SelectedValue).OrderBy(tm => tm.MedDraTerm).ToList();

            foreach (var term in terms) {
                lstTermResult.Items.Add(new ListItem() { Text = term.MedDraTerm, Value = term.Id.ToString() });
            }
            if(terms.Count == 0)
            {
                lblTerm.Attributes.Remove("class");
                lblTerm.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "No results found";
                lblTerm.Controls.Add(errorMessageDiv);
            }
            else
            {
                lblTerm.Attributes.Remove("class");
                lblTerm.Attributes.Add("class", "input state-error");
                var messageDiv = new HtmlGenericControl("div");
                messageDiv.Attributes.Add("class", "note note-success");
                messageDiv.InnerText = terms.Count.ToString() + " result(s) found";
                lblTerm.Controls.Add(messageDiv);
            }

            SetActive("tab3");
        }

        protected void btnSearchCode_Click(object sender, EventArgs e)
        {
            lstCodeResult.Items.Clear();

            bool err = false;

            lblCode.Attributes.Remove("class");
            lblCode.Attributes.Add("class", "input");

            if (!String.IsNullOrWhiteSpace(txtCode.Value))
            {
                if (Regex.Matches(txtCode.Value, @"[0-9]").Count < txtCode.Value.Length)
                {
                    lblCode.Attributes.Remove("class");
                    lblCode.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Search code contains invalid characters (Enter 0-9)";
                    lblCode.Controls.Add(errorMessageDiv);

                    err = true;
                }
                else
                {
                    if (txtCode.Value.Trim().Length < 4)
                    {
                        lblCode.Attributes.Remove("class");
                        lblCode.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search code must contain at least 4 numerics";
                        lblCode.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
            }
            else
            {
                lblCode.Attributes.Remove("class");
                lblCode.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Search code must be entered";
                lblCode.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (err)
            {
                SetActive("tab4");
                return;
            };

            var terms = UnitOfWork.Repository<TerminologyMedDra>().Queryable().Where(u => u.MedDraCode.Contains(txtCode.Value)).OrderBy(tm => tm.MedDraTerm).ToList();

            foreach (var term in terms) {
                lstCodeResult.Items.Add(new ListItem() { Text = term.MedDraTerm, Value = term.Id.ToString() });
            }
            if(terms.Count == 0)
            {
                lblCode.Attributes.Remove("class");
                lblCode.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "No results found";
                lblCode.Controls.Add(errorMessageDiv);
            }
            else
            {
                lblCode.Attributes.Remove("class");
                lblCode.Attributes.Add("class", "input state-error");
                var messageDiv = new HtmlGenericControl("div");
                messageDiv.Attributes.Add("class", "note note-success");
                messageDiv.InnerText = terms.Count.ToString() + " result(s) found";
                lblCode.Controls.Add(messageDiv);
            }

            SetActive("tab4");
        }

        #region "EF"

        private TerminologyMedDra GetTerminologyMedDraByName(string name, string termType)
        {
            return UnitOfWork.Repository<TerminologyMedDra>().Queryable().FirstOrDefault(u => u.MedDraTerm == name && u.MedDraTermType == termType);
        }

        private TerminologyMedDra GetTerminologyMedDraById(int id)
        {
            return UnitOfWork.Repository<TerminologyMedDra>().Queryable().SingleOrDefault(u => u.Id == id);
        }

        #endregion

        protected void ddlSOC_SelectedIndexChanged(object sender, EventArgs e)
        {
            divHLGT.Style["display"] = "none";
            divHLT.Style["display"] = "none";
            divPT.Style["display"] = "none";
            divLLT.Style["display"] = "none";

            if (ddlSOC.SelectedItem.Text == "---") { return; };

            divHLGT.Style["display"] = "block";
            ddlHLGT.Items.Clear();
            ddlHLGT.Items.Add("---");
            ddlHLGT.SelectedIndex = 0;

            ddlHLT.Items.Clear();
            ddlHLT.Items.Add("---");
            ddlHLT.SelectedIndex = 0;

            ddlPT.Items.Clear();
            ddlPT.Items.Add("---");
            ddlPT.SelectedIndex = 0;

            lstLLT.Items.Clear();

            var term = GetTerminologyMedDraByName(ddlSOC.SelectedItem.Text, "SOC");

            if(term != null)
            {
                foreach (var child in term.Children) {
                    ddlHLGT.Items.Add(new ListItem() { Text = child.MedDraTerm, Value = child.Id.ToString() });
                }
                if(ddlHLGT.Items.Count > 1) {
                    ddlHLGT.SelectedIndex = 0;
                }
            }

            SetActive("tab2");
        }

        protected void ddlHLGT_SelectedIndexChanged(object sender, EventArgs e)
        {
            divHLT.Style["display"] = "none";
            divPT.Style["display"] = "none";
            divLLT.Style["display"] = "none";

            if (ddlHLGT.SelectedItem.Text == "---") { return; };

            divHLT.Style["display"] = "block";
            ddlHLT.Items.Clear();
            ddlHLT.Items.Add("---");
            ddlHLT.SelectedIndex = 0;

            ddlPT.Items.Clear();
            ddlPT.Items.Add("---");
            ddlPT.SelectedIndex = 0;

            lstLLT.Items.Clear();

            var term = GetTerminologyMedDraByName(ddlHLGT.SelectedItem.Text, "HLGT");

            if (term != null)
            {
                foreach (var child in term.Children) {
                    ddlHLT.Items.Add(new ListItem() { Text = child.MedDraTerm, Value = child.Id.ToString() });
                }
                if (ddlHLT.Items.Count > 1) {
                    ddlHLT.SelectedIndex = 0;
                }
            }

            SetActive("tab2");
        }

        protected void ddlHLT_SelectedIndexChanged(object sender, EventArgs e)
        {
            divPT.Style["display"] = "none";
            divLLT.Style["display"] = "none";

            if (ddlHLT.SelectedItem.Text == "---") { return; };

            divPT.Style["display"] = "block";
            ddlPT.Items.Clear();
            ddlPT.Items.Add("---");
            ddlPT.SelectedIndex = 0;

            lstLLT.Items.Clear();

            var term = GetTerminologyMedDraByName(ddlHLT.SelectedItem.Text, "HLT");

            if (term != null)
            {
                foreach (var child in term.Children) {
                    ddlPT.Items.Add(new ListItem() { Text = child.MedDraTerm, Value = child.Id.ToString() });
                }
                if (ddlPT.Items.Count > 1) {
                    ddlPT.SelectedIndex = 0;
                }
            }

            SetActive("tab2");
        }

        protected void ddlPT_SelectedIndexChanged(object sender, EventArgs e)
        {
            divLLT.Style["display"] = "none";

            if (ddlPT.SelectedItem.Text == "---") { return; };

            divLLT.Style["display"] = "block";
            lstLLT.Items.Clear();

            var term = GetTerminologyMedDraByName(ddlPT.SelectedItem.Text, "PT");

            if (term != null)
            {
                foreach (var child in term.Children) {
                    lstLLT.Items.Add(new ListItem() { Text = child.MedDraTerm, Value = child.Id.ToString() });
                }
            }

            SetActive("tab2");
        }

        private void SetActive(string tab)
        {
            liCommon.Attributes["class"] = "";
            liList.Attributes["class"] = "";
            liMTerm.Attributes["class"] = "";
            liMCode.Attributes["class"] = "";

            tab1.Attributes["class"] = "tab-pane smart-form";
            tab2.Attributes["class"] = "tab-pane smart-form";
            tab3.Attributes["class"] = "tab-pane smart-form";
            tab4.Attributes["class"] = "tab-pane smart-form";

            switch (tab)
            {
                case "tab1":
                    tab1.Attributes["class"] = "tab-pane active smart-form";
                    liCommon.Attributes["class"] = "active";
                    break;

                case "tab2":
                    tab2.Attributes["class"] = "tab-pane active smart-form";
                    liList.Attributes["class"] = "active";
                    break;

                case "tab3":
                    tab3.Attributes["class"] = "tab-pane active smart-form";
                    liMTerm.Attributes["class"] = "active";
                    break;

                case "tab4":
                    tab4.Attributes["class"] = "tab-pane active smart-form";
                    liMCode.Attributes["class"] = "active";
                    break;

                default:
                    break;
            }
        }
    }
}