using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class ManageWorkPlanEdit : MainPageBase
    {
        private int _id;
        private WorkPlan _workPlan;

        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3 };
        private FormMode _formMode = FormMode.ViewMode;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0) {
                    _workPlan = UnitOfWork.Repository<WorkPlan>().Queryable().Include(wp => wp.Dataset).SingleOrDefault(u => u.Id == _id); ;
                }
                else
                {
                    _workPlan = null;
                    _formMode = FormMode.AddMode;
                }
            }
            else
            {
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

            if (!Page.IsPostBack)
            {
                Master.SetMenuActive("AdminWorkPlan");

                LoadDatasetDropDownList();
                if (_workPlan != null)
                {
                    LoadCareEventDropDownList();
                    LoadWorkPlanCareEventDropDownList();

                    RenderWorkPlan();
                    RenderGrids();
                }
                else
                {
                    ddlDataset.Visible = true;
                    txtDataset.Visible = false;
                }
            };

        }

        #region "Rendering"

        private void RenderWorkPlan()
        {
            txtUID.Value = _workPlan.Id.ToString();
            txtName.Value = _workPlan.Description;

            ddlDataset.SelectedValue = _workPlan.Dataset != null ? _workPlan.Dataset.Id.ToString() : "0";
            txtDataset.Value = _workPlan.Dataset != null ? _workPlan.Dataset.DatasetName : "";
        }

        private void RenderGrids()
        {
            RenderCareEvents();
        }

        private void RenderCareEvents()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            HyperLink storeHyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_1.Rows)
            {
                if (temprow.Cells[0].Text != "Care Event")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) {
                dt_1.Rows.Remove(temprow);
            }

            foreach (var ce in _workPlan.WorkPlanCareEvents.OrderBy(ce => ce.CareEvent.Description))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = ce.CareEvent.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = ce.WorkPlanCareEventDatasetCategories.Count.ToString();
                row.Cells.Add(cell);

                if (ce.WorkPlanCareEventDatasetCategories.Count == 0)
                {
                    cell = new TableCell();
                    pnl = new Panel() { CssClass = "btn-group" };
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "#",
                        Text = "Delete Care Event",
                        CssClass = "btn btn-default"
                    };
                    hyp.Attributes.Add("data-toggle", "modal");
                    hyp.Attributes.Add("data-target", "#careModal");
                    hyp.Attributes.Add("data-id", ce.Id.ToString());
                    hyp.Attributes.Add("data-evt", "delete");
                    hyp.Attributes.Add("data-name", ce.CareEvent.Description);
                    pnl.Controls.Add(hyp);

                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }

                dt_1.Rows.Add(row);
            }
        }

        private void RenderCategories()
        {
            if (_workPlan.WorkPlanCareEvents.Count == 0) { return; };

            TableRow row;
            TableCell cell;

            HyperLink hyp;
            HyperLink storeHyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_2.Rows)
            {
                if (temprow.Cells[0].Text != "Category")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) {
                dt_2.Rows.Remove(temprow);
            }

            var id = Convert.ToInt32(ddlWorkPlanCareEvent.SelectedValue);
            var workPlanCareEvent = UnitOfWork.Repository<WorkPlanCareEvent>().Queryable().Single(wpce => wpce.Id == id);

            string elements;

            foreach (WorkPlanCareEventDatasetCategory dsCat in workPlanCareEvent.WorkPlanCareEventDatasetCategories)
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = dsCat.DatasetCategory.DatasetCategoryName;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    CssClass = "btn btn-default",
                    Text = "View Elements"
                };
                hyp.Attributes.Add("data-toggle", "popover");
                hyp.Attributes.Add("data-placement", "top");
                hyp.Attributes.Add("data-original-title", "Dataset Elements");

                elements = "";
                if (dsCat.DatasetCategory.DatasetCategoryElements.Count == 0) {
                    elements += string.Format(@"<span class=""label label-danger"">{0}</span>", @"No elements configured");
                }
                else
                {
                    elements += "<ul>";
                    foreach (var ele in dsCat.DatasetCategory.DatasetCategoryElements) {
                        elements += string.Format("<li>{0}</li>", ele.DatasetElement.ElementName);
                    }
                    elements += "</ul>";
                }
                hyp.Attributes.Add("data-content", elements);
                hyp.Attributes.Add("data-html", "true");
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                cell = new TableCell();
                pnl = new Panel() { CssClass = "btn-group" };
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    Text = "Delete Category",
                    CssClass = "btn btn-default"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#categoryModal");
                hyp.Attributes.Add("data-id", dsCat.Id.ToString());
                hyp.Attributes.Add("data-evt", "delete");
                hyp.Attributes.Add("data-name", dsCat.DatasetCategory.DatasetCategoryName);
                pnl.Controls.Add(hyp);

                cell.Controls.Add(pnl);
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
                        NavigateUrl = "ManageWorkPlanEdit.aspx?id=" + _workPlan.Id.ToString() + "&a=edit",
                        CssClass = "btn btn-primary",
                        Text = "Edit"
                    };
                    spnButtons.Controls.Add(hyp);

                    hyp = new HyperLink()
                    {
                        ID = "btnReturn",
                        NavigateUrl = "ManageWorkPlan.aspx",
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
                        NavigateUrl = "ManageWorkPlanEdit.aspx?id=" + _workPlan.Id.ToString(),
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
                        NavigateUrl = "ManageWorkPlan.aspx",
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
            var allocated = _workPlan != null ? _workPlan.Dataset != null : false;

            switch (_formMode)
            {
                case FormMode.ViewMode:
                    txtName.Attributes.Add("readonly", "true");
                    txtName.Style.Add("background-color", "#EBEBE4");

                    ddlDataset.Attributes.Add("readonly", "true");
                    ddlDataset.Style.Add("background-color", "#EBEBE4");

                    btnAddCareEvent.Visible = true;
                    btnAddCategory.Visible = true;

                    ddlDataset.Visible = !allocated;
                    txtDataset.Visible = allocated;

                    break;

                case FormMode.EditMode:

                    btnAddCareEvent.Visible = false;
                    btnAddCategory.Visible = false;

                    ddlDataset.Visible = !allocated;
                    txtDataset.Visible = allocated;

                    break;

                case FormMode.AddMode:

                    btnAddCareEvent.Visible = false;
                    btnAddCategory.Visible = false;

                    break;

                default:
                    break;
            };
        }

        private void LoadDatasetDropDownList()
        {
            ListItem item;

            List<Dataset> datasets = UnitOfWork.Repository<Dataset>().Queryable().OrderBy(ds => ds.DatasetName).ToList();

            if (datasets.Count == 0)
            {
                item = new ListItem();
                item.Text = "-- NO DATASETS --";
                item.Value = "0";
                ddlCategory.Items.Add(item);
            }
            else
            {
                item = new ListItem();
                item.Text = "";
                item.Value = "0";
                ddlDataset.Items.Add(item);

                foreach (Dataset ds in datasets)
                {
                    item = new ListItem();
                    item.Text = ds.DatasetName;
                    item.Value = ds.Id.ToString();
                    ddlDataset.Items.Add(item);
                }
            }
        }

        private void LoadCareEventDropDownList()
        {
            ListItem item;

            List<int> ignoreEvents = new List<int>();
            foreach(WorkPlanCareEvent wpce in _workPlan.WorkPlanCareEvents) {
                ignoreEvents.Add(wpce.CareEvent.Id);
            }

            List<CareEvent> careEvents = UnitOfWork.Repository<CareEvent>().Queryable().Where(ce => !ignoreEvents.Contains(ce.Id)).OrderBy(ce => ce.Description).ToList();

            ddlCareEvent.Items.Clear();

            if (careEvents.Count == 0)
            {
                item = new ListItem();
                item.Text = "-- NO CARE EVENTS --";
                item.Value = "0";
                ddlCareEvent.Items.Add(item);
            }
            else
            {
                foreach (CareEvent ce in careEvents)
                {
                    item = new ListItem();
                    item.Text = ce.Description;
                    item.Value = ce.Id.ToString();
                    ddlCareEvent.Items.Add(item);
                }
            }
        }

        private void LoadWorkPlanCareEventDropDownList()
        {
            ListItem item;

            ddlWorkPlanCareEvent.Items.Clear();

            if (_workPlan.WorkPlanCareEvents.Count == 0)
            {
                item = new ListItem();
                item.Text = "-- NO CARE EVENTS --";
                item.Value = "0";
                ddlWorkPlanCareEvent.Items.Add(item);
            }
            else
            {
                foreach (WorkPlanCareEvent wpce in _workPlan.WorkPlanCareEvents)
                {
                    item = new ListItem();
                    item.Text = wpce.CareEvent.Description;
                    item.Value = wpce.Id.ToString();
                    ddlWorkPlanCareEvent.Items.Add(item);
                }
                LoadCategoryDropDownList();
            }
        }

        private void LoadCategoryDropDownList()
        {
            ListItem item;

            ddlCategory.Items.Clear();

            if (ddlWorkPlanCareEvent.SelectedValue == "0") { RenderGrids(); return; };

            var id = Convert.ToInt32(ddlWorkPlanCareEvent.SelectedValue);
            var workPlanCareEvent = UnitOfWork.Repository<WorkPlanCareEvent>().Queryable().Single(wpce => wpce.Id == id);

            if (_workPlan.Dataset == null) { return; };

            List<int> ignoreCategories = new List<int>();
            foreach (WorkPlanCareEventDatasetCategory dsCat in workPlanCareEvent.WorkPlanCareEventDatasetCategories) {
                ignoreCategories.Add(dsCat.DatasetCategory.Id);
            }

            List<DatasetCategory> datasetCategories = UnitOfWork.Repository<DatasetCategory>().Queryable().Where(dc => dc.Dataset.Id == _workPlan.Dataset.Id && !ignoreCategories.Contains(dc.Id)).OrderBy(dc => dc.DatasetCategoryName).ToList();

            if (datasetCategories.Count == 0)
            {
                item = new ListItem();
                item.Text = "-- NO CATEGORIES --";
                item.Value = "0";
                ddlCategory.Items.Add(item);
            }
            else
            {
                foreach (DatasetCategory dsCat in datasetCategories)
                {
                    item = new ListItem();
                    item.Text = dsCat.DatasetCategoryName;
                    item.Value = dsCat.Id.ToString();
                    ddlCategory.Items.Add(item);
                }
            }
            RenderCategories();
        }

        #endregion

        #region "Save"

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string err = "<ul>";

            // Validation
            if (Regex.Matches(txtName.Value, @"[a-zA-Z ']").Count < txtName.Value.Length) {
                err += "<li>Work plan name contains invalid characters (Enter A-Z, a-z)...</li>";
            }

            if (_formMode == FormMode.AddMode)
            {
                if (!IsWorkPlanUnique(txtName.Value, 0)) {
                    err += "<li>Work plan with same name already added...</li>";
                }

            }
            else
            {
                if (!IsWorkPlanUnique(txtName.Value, _workPlan.Id)) {
                    err += "<li>Work plan with same name already added...</li>";
                }
            }

            if (err != "<ul>")
            {
                err += "</ul>";
                divError.Visible = true;
                spnErrors.InnerHtml = err;
                return;
            }

            SaveWorkPlan();

            var url = String.Format("ManageWorkPlanEdit.aspx?id=" + _workPlan.Id.ToString());
            Response.Redirect(url);
        }

        private void SaveWorkPlan()
        {
            var id = Convert.ToInt32(ddlDataset.SelectedValue);
            var dataset = ddlDataset.SelectedValue != "0" ? UnitOfWork.Repository<Dataset>().Queryable().Single(ds => ds.Id == id) : null;

            var encodedName = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(txtName.Value, false);

            if (_workPlan == null)
            {
                // Prepare new work plan
                _workPlan = new WorkPlan { Dataset = dataset, Description = encodedName };
                UnitOfWork.Repository<WorkPlan>().Save(_workPlan);
            }
            else
            {
                // Prepare updated work plan
                _workPlan.Description = encodedName;
                _workPlan.Dataset = dataset;

                UnitOfWork.Repository<WorkPlan>().Update(_workPlan);
            }
        }

        #endregion

        #region "Events"

        protected void btnAddCareEvent_Click(object sender, EventArgs e)
        {
            if (ddlCareEvent.SelectedValue == "0")
            {
                RenderGrids();
                return;
            }

            var id = Convert.ToInt32(ddlCareEvent.SelectedValue);
            var careEvent = UnitOfWork.Repository<CareEvent>().Queryable().SingleOrDefault(ce => ce.Id == id);

            // Save care event
            var wpce = new WorkPlanCareEvent
            {
                Order = (short)GetNextCareEventOrder(),
                Active = true,
                CareEvent = careEvent,
                WorkPlan = _workPlan
            };

            UnitOfWork.Repository<WorkPlanCareEvent>().Save(wpce);
            UnitOfWork.Complete();

            RenderGrids();
            LoadCareEventDropDownList();
            LoadWorkPlanCareEventDropDownList();
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            if (ddlCategory.SelectedValue == "0") { return; };

            var id = Convert.ToInt32(ddlWorkPlanCareEvent.SelectedValue);
            var workPlanCareEvent = UnitOfWork.Repository<WorkPlanCareEvent>().Queryable().SingleOrDefault(ce => ce.Id == id);

            id = Convert.ToInt32(ddlCategory.SelectedValue);
            var datasetCategory = UnitOfWork.Repository<DatasetCategory>().Queryable().SingleOrDefault(ds => ds.Id == id);

            var wpcedsc = new WorkPlanCareEventDatasetCategory
            {
                DatasetCategory = datasetCategory,
                WorkPlanCareEvent = workPlanCareEvent
            };

            UnitOfWork.Repository<WorkPlanCareEventDatasetCategory>().Save(wpcedsc);
            UnitOfWork.Complete();

            RenderGrids();
            RenderWorkPlan();
            LoadCategoryDropDownList();
        }

        protected void btnDeleteCare_Click(object sender, EventArgs e)
        {
            if (txtCareEventUID.Value != "0")
            {
                var id = Convert.ToInt32(txtCareEventUID.Value);
                var workPlanCareEvent = UnitOfWork.Repository<WorkPlanCareEvent>().Queryable().SingleOrDefault(u => u.Id == id);

                UnitOfWork.Repository<WorkPlanCareEvent>().Delete(workPlanCareEvent);
                _workPlan.WorkPlanCareEvents.Remove(workPlanCareEvent);

                UnitOfWork.Complete();
            }

            RenderGrids();
            LoadCareEventDropDownList();
            LoadWorkPlanCareEventDropDownList();
        }

        protected void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (txtCategoryUID.Value != "0")
            {
                var id = Convert.ToInt32(ddlWorkPlanCareEvent.SelectedValue);
                var workPlanCareEvent = UnitOfWork.Repository<WorkPlanCareEvent>().Queryable().SingleOrDefault(u => u.Id == id);

                id = Convert.ToInt32(txtCategoryUID.Value);
                var workPlanDatasetCategory = UnitOfWork.Repository<WorkPlanCareEventDatasetCategory>().Queryable().SingleOrDefault(wpds => wpds.Id == id);

                UnitOfWork.Repository<WorkPlanCareEventDatasetCategory>().Delete(workPlanDatasetCategory);
                workPlanCareEvent.WorkPlanCareEventDatasetCategories.Remove(workPlanDatasetCategory);

                UnitOfWork.Complete();
            }

            RenderGrids();
            RenderWorkPlan();
            LoadCategoryDropDownList();
        }

        protected void ddlWorkPlanCareEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlWorkPlanCareEvent.SelectedValue == "0") { return; };

            RenderGrids();
            LoadCategoryDropDownList();
        }

        #endregion

        #region "EF"

        private int GetNextCareEventOrder()
        {
            int order = 0;

            if (_workPlan.WorkPlanCareEvents.Count == 0) {
                return 1;
            }
            else {
                return UnitOfWork.Repository<WorkPlanCareEvent>().Queryable().Where(ce => ce.WorkPlan.Id == _workPlan.Id).OrderByDescending(ce => ce.Order).First().Order + 1;
            }
            return 0;
        }

        private bool IsWorkPlanUnique(string workPlan, int id)
        {
            int count = 0;
            if (id > 0) {
                count = UnitOfWork.Repository<WorkPlan>().Queryable().Where(wp => wp.Description == workPlan && wp.Id != id).Count();
            }
            else {
                count = UnitOfWork.Repository<WorkPlan>().Queryable().Where(wp => wp.Description == workPlan).Count();
            }
            return (count == 0);
        }

        #endregion

    }
}