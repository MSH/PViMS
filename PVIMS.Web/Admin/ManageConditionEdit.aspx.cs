using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ManageConditionEdit : MainPageBase
    {
        private int _id;
        private Condition _condition;

        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3 };
        private FormMode _formMode = FormMode.ViewMode;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0) {
                    _condition = GetCondition(_id);
                }
                else
                {
                    _condition = null;
                    _formMode = FormMode.AddMode;
                }
            }
            else {
                throw new Exception("id not passed as parameter");
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

                } // switch (_action)
            }

            RenderButtons();
            ToggleView();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            divError.Visible = false;
            spnErrors.InnerHtml = "";
            divMError.Visible = false;
            spnMError.InnerHtml = "";

            Master.SetMenuActive("AdminCondition");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Condition Groups", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

            if (!Page.IsPostBack)
            {

                if (_condition != null)
                {
                    RenderCondition();
                    LoadLabDropDownList();
                    LoadMedicationDropDownList();
                    RenderGrids();
                }
            }
            else
            {
                if (_condition != null) {
                    RenderGrids();
                }
            }

        }

        #region "Rendering"

        private void RenderCondition()
        {
            txtUID.Value = _condition.Id.ToString();
            txtDescription.Value = _condition.Description;
            ddlChronic.Value = _condition.Chronic ? "Yes" : "No";
        }
        
        private void RenderGrids()
        {
            RenderLabs();
            RenderMedications();
            RenderMedDras();
        }

        private void RenderLabs()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Label lbl;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_1.Rows)
            {
                if (temprow.Cells[0].Text != "Lab Test")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) {
                dt_1.Rows.Remove(temprow);
            }

            foreach (var lab in _condition.ConditionLabTests.OrderBy(c => c.LabTest.Description))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = lab.LabTest.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    CssClass = "btn btn-default",
                    Text = "Delete Lab Test"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#itemModal");
                hyp.Attributes.Add("data-id", lab.Id.ToString());
                hyp.Attributes.Add("data-type", "lab");
                hyp.Attributes.Add("data-name", lab.LabTest.Description);
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                dt_1.Rows.Add(row);
            }
        }

        private void RenderMedDras()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Label lbl;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_3.Rows)
            {
                if (temprow.Cells[0].Text != "MedDRA Term")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) {
                dt_3.Rows.Remove(temprow);
            }

            foreach (var med in _condition.ConditionMedDras.OrderBy(c => c.TerminologyMedDra.MedDraTerm))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = med.TerminologyMedDra.MedDraTerm;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    CssClass = "btn btn-default",
                    Text = "Delete Terminology"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#itemModal");
                hyp.Attributes.Add("data-id", med.Id.ToString());
                hyp.Attributes.Add("data-type", "meddra");
                hyp.Attributes.Add("data-name", med.TerminologyMedDra.MedDraTerm);
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                dt_3.Rows.Add(row);
            }
        }

        private void RenderMedications()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Label lbl;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_2.Rows)
            {
                if (temprow.Cells[0].Text != "Medication")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) {
                dt_2.Rows.Remove(temprow);
            }

            foreach (var med in _condition.ConditionMedications.OrderBy(c => c.Medication.DrugName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = med.Medication.DrugName;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    CssClass = "btn btn-default",
                    Text = "Delete Medication"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#itemModal");
                hyp.Attributes.Add("data-id", med.Id.ToString());
                hyp.Attributes.Add("data-type", "med");
                hyp.Attributes.Add("data-name", med.Medication.DrugName);
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                dt_2.Rows.Add(row);
            }
        }

        private void RenderButtons()
        {
            Button btn;
            HyperLink hyp;

            var url = "";

            switch (_formMode)
            {
                case FormMode.ViewMode:
                    hyp = new HyperLink()
                    {
                        ID = "btnEdit",
                        NavigateUrl = "ManageConditionEdit.aspx?id=" + _condition.Id.ToString() + "&a=edit",
                        CssClass = "btn btn-primary",
                        Text = "Edit"
                    };
                    spnButtons.Controls.Add(hyp);

                    hyp = new HyperLink()
                    {
                        ID = "btnReturn",
                        NavigateUrl = "ManageCondition.aspx",
                        CssClass = "btn btn-default",
                        Text = "Return"
                    };
                    spnButtons.Controls.Add(hyp);

                    break;

                case FormMode.EditMode:
                    btn = new Button();
                    btn.ID = "btnSave";
                    btn.CssClass = "btn btn-primary";
                    btn.Text = "Save";
                    btn.Click += btnSave_Click;
                    btn.Attributes.Add("formnovalidate", "formnovalidate");
                    spnButtons.Controls.Add(btn);

                    hyp = new HyperLink()
                    {
                        ID = "btnCancel",
                        NavigateUrl = "ManageConditionEdit.aspx?id=" + _condition.Id.ToString(),
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
                    spnButtons.Controls.Add(btn);

                    hyp = new HyperLink()
                    {
                        ID = "btnCancel",
                        NavigateUrl = "ManageCondition.aspx",
                        CssClass = "btn btn-default",
                        Text = "Cancel"
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
                    txtDescription.Attributes.Add("readonly", "true");
                    txtDescription.Style.Add("background-color", "#EBEBE4");
                    ddlChronic.Attributes.Add("readonly", "true");
                    ddlChronic.Style.Add("background-color", "#EBEBE4");

                    btnAddLabTest.Visible = true;
                    btnAddMedication.Visible = true;

                    liLabs.Visible = true;
                    liMedications.Visible = true;

                    break;

                case FormMode.EditMode:

                    btnAddLabTest.Visible = false;
                    btnAddMedication.Visible = false;

                    liLabs.Visible = false;
                    liMedications.Visible = false;

                    break;

                case FormMode.AddMode:

                    btnAddLabTest.Visible = false;
                    btnAddMedication.Visible = false;

                    liLabs.Visible = false;
                    liMedications.Visible = false;

                    break;

                default:
                    break;
            };
        }

        private void LoadLabDropDownList()
        {
            ListItem item;

            foreach (LabTest lab in UnitOfWork.Repository<LabTest>().Queryable().OrderBy(lt => lt.Description))
            {
                item = new ListItem();
                item.Text = lab.Description;
                item.Value = lab.Id.ToString();
                ddlLabTest.Items.Add(item);
            }
        }

        private void LoadMedicationDropDownList()
        {
            ListItem item;

            foreach (Medication med in UnitOfWork.Repository<Medication>().Queryable().OrderBy(m => m.DrugName))
            {
                item = new ListItem();
                item.Text = med.DrugName;
                item.Value = med.Id.ToString();
                ddlMedication.Items.Add(item);
            }
        }

        private void SetActive(string tab)
        {
            liCondition.Attributes["class"] = "";
            liLabs.Attributes["class"] = "";
            liMedications.Attributes["class"] = "";
            liMedDras.Attributes["class"] = "";

            tab1.Attributes["class"] = "tab-pane smart-form";
            tab2.Attributes["class"] = "tab-pane";
            tab3.Attributes["class"] = "tab-pane";
            tab4.Attributes["class"] = "tab-pane";

            switch (tab)
            {
                case "tab1":
                    tab1.Attributes["class"] = "tab-pane smart-form active";
                    liCondition.Attributes["class"] = "active";
                    break;

                case "tab2":
                    tab2.Attributes["class"] = "tab-pane active";
                    liLabs.Attributes["class"] = "active";
                    break;

                case "tab3":
                    tab3.Attributes["class"] = "tab-pane active";
                    liMedications.Attributes["class"] = "active";
                    break;

                case "tab4":
                    tab4.Attributes["class"] = "tab-pane active";
                    liMedDras.Attributes["class"] = "active";
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region "Save"
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string err = "<ul>";

            // Validation
            if (_formMode == FormMode.AddMode)
            {
                if (!IsConditionUnique(txtDescription.Value, 0)) {
                    err += "<li>Condition with same name already added...</li>";
                }

                if (Regex.Matches(txtDescription.Value, @"[a-zA-Z ']").Count < txtDescription.Value.Length) {
                    err += "<li>Condition contains invalid characters (Enter A-Z, a-z)...</li>";
                }

                if (err != "<ul>")
                {
                    err += "</ul>";
                    divError.Visible = true;
                    spnErrors.InnerHtml = err;
                    return;
                }
            }
            else
            {
                if (!IsConditionUnique(txtDescription.Value, _condition.Id)) {
                    err += "<li>Condition with same name already added...</li>";
                }

                if (Regex.Matches(txtDescription.Value, @"[a-zA-Z ']").Count < txtDescription.Value.Length) {
                    err += "<li>Condition contains invalid characters (Enter A-Z, a-z)...</li>";
                }

                if (err != "<ul>")
                {
                    err += "</ul>";
                    divError.Visible = true;
                    spnErrors.InnerHtml = err;
                    return;
                }
            }

            SaveCondition();

            var url = String.Format("ManageConditionEdit.aspx?id=" + _condition.Id.ToString());
            Response.Redirect(url);
        }

        private void SaveCondition()
        {
            var encodedDescription = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(txtDescription.Value, false);

            if (_condition == null)
            {
                // Prepare new condition
                _condition = new Condition { Description = encodedDescription, Chronic = (ddlChronic.Value == "Yes") };
                UnitOfWork.Repository<Condition>().Save(_condition);
            }
            else
            {
                // Prepare updated dataset
                _condition.Description = encodedDescription;
                _condition.Chronic = (ddlChronic.Value == "Yes");

                UnitOfWork.Repository<Condition>().Update(_condition);
            }
        }

        #endregion

        #region "Events"

        protected void btnAddLabTest_Click(object sender, EventArgs e)
        {
            if (ddlLabTest.Items.Count == 0) { SetActive("tab2");  return; };
            if (ddlLabTest.SelectedValue == "0") { SetActive("tab2"); return; };

            // Save lab test
            var labtest = GetLabTest(Convert.ToInt32(ddlLabTest.SelectedValue));

            var conlab = new ConditionLabTest
            {
                Condition = _condition,
                LabTest = labtest
            };

            UnitOfWork.Repository<ConditionLabTest>().Save(conlab);
            UnitOfWork.Complete();

            RenderGrids();
            SetActive("tab2");
        }

        protected void btnAddMedication_Click(object sender, EventArgs e)
        {
            if (ddlMedication.Items.Count == 0) { SetActive("tab3"); return; };
            if (ddlMedication.SelectedValue == "0") { SetActive("tab3"); return; };

            // Save element
            var medication = GetMedication(Convert.ToInt32(ddlMedication.SelectedValue));

            var conmed = new ConditionMedication 
            {
                Condition = _condition,
                Medication = medication
            };

            UnitOfWork.Repository<ConditionMedication>().Save(conmed);
            UnitOfWork.Complete();

            RenderGrids();
            SetActive("tab3"); 
        }

        protected void btnAddMedDra_Click(object sender, EventArgs e)
        {
            string err = "<ul>";

            if (lstTermResult.SelectedIndex == -1) { SetActive("tab4"); return; };

            // Save element
            var terminologyId = Convert.ToInt32(lstTermResult.SelectedItem.Value);
            var terminology = GetTerminology(terminologyId);

            if(terminology.ConditionMedDras.Count > 0)
            {
                SetActive("tab4");
                err += "<li>Terminology already added to a condition...</li>";
            }

            if (err != "<ul>")
            {
                err += "</ul>";
                divMError.Visible = true;
                spnMError.InnerHtml = err;
                return;
            }

            var conmed = new ConditionMedDra
            {
                Condition = _condition,
                TerminologyMedDra = terminology
            };

            UnitOfWork.Repository<ConditionMedDra>().Save(conmed);
            UnitOfWork.Complete();

            RenderGrids();
            SetActive("tab4");
        }

        protected void btnDeleteItem_Click(object sender, EventArgs e)
        {
            switch (txtItemType.Value)
	        {
                case "lab":
                    if (txtItemUID.Value != "0")
                    {
                        var conditionlabtest = GetConditionLabTest(Convert.ToInt32(txtItemUID.Value));

                        _condition.ConditionLabTests.Remove(conditionlabtest);
                        UnitOfWork.Complete();

                        RenderGrids();
                        SetActive("tab2");
                    }
                    break;

                case "med":
                    if (txtItemUID.Value != "0")
                    {
                        var conditionmedication = GetConditionMedication(Convert.ToInt32(txtItemUID.Value));

                        _condition.ConditionMedications.Remove(conditionmedication);
                        UnitOfWork.Complete();

                        RenderGrids();
                        SetActive("tab3");
                    }
                    break;

                case "meddra":
                    if (txtItemUID.Value != "0")
                    {
                        var conditionmeddra = GetConditionMedDra(Convert.ToInt32(txtItemUID.Value));

                        UnitOfWork.Repository<ConditionMedDra>().Delete(conditionmeddra);
                        _condition.ConditionMedDras.Remove(conditionmeddra);
                        UnitOfWork.Complete();

                        RenderGrids();
                        SetActive("tab4");
                    }
                    break;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lstTermResult.Items.Clear();

            var encodedTerm = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(txtTerm.Value, false);

            var terms = UnitOfWork.Repository<TerminologyMedDra>().Queryable().Where(u => u.MedDraTerm.Contains(encodedTerm) && u.MedDraTermType == "LLT").OrderBy(tm => tm.MedDraTerm).ToList();

            foreach (var term in terms)
            {
                lstTermResult.Items.Add(new ListItem() { Text = term.MedDraTerm, Value = term.Id.ToString() });
            }

            SetActive("tab4");
        }

        #endregion

        #region "EF"

        private Condition GetCondition(int id)
        {
            return UnitOfWork.Repository<Condition>().Queryable().SingleOrDefault(c => c.Id == id);
        }

        private LabTest GetLabTest(int id)
        {
            return UnitOfWork.Repository<LabTest>().Queryable().SingleOrDefault(lt => lt.Id == id);
        }

        private Medication GetMedication(int id)
        {
            return UnitOfWork.Repository<Medication>().Queryable().SingleOrDefault(m => m.Id == id);
        }

        private TerminologyMedDra GetTerminology(int id)
        {
            return UnitOfWork.Repository<TerminologyMedDra>().Queryable().SingleOrDefault(m => m.Id == id);
        }

        private ConditionLabTest GetConditionLabTest(int id)
        {
            return UnitOfWork.Repository<ConditionLabTest>().Queryable().SingleOrDefault(clt => clt.Id == id);
        }

        private ConditionMedication GetConditionMedication(int id)
        {
            return UnitOfWork.Repository<ConditionMedication>().Queryable().SingleOrDefault(cm => cm.Id == id);
        }

        private ConditionMedDra GetConditionMedDra(int id)
        {
            return UnitOfWork.Repository<ConditionMedDra>().Queryable().SingleOrDefault(cm => cm.Id == id);
        }

        private bool IsConditionUnique(string condition, int id)
        {
            int count = 0;
            if (id > 0) {
                count = UnitOfWork.Repository<Condition>().Queryable().Where(c => c.Description == condition && c.Id != id).Count();
            }
            else {
                count = UnitOfWork.Repository<Condition>().Queryable().Where(c => c.Description == condition).Count();
            }
            return (count == 0);
        }

        #endregion

    }
}