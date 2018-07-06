using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

using System.Web;
using System.Web.Security.AntiXss;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class ManageDatasetEdit : MainPageBase
    {
        private int _id;
        private Dataset _dataset;

        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3 };
        private FormMode _formMode = FormMode.ViewMode;

        private string _sortIconUp = "fa fa-arrow-circle-up fa-2x";
        private string _sortIconDown = "fa fa-arrow-circle-down fa-2x";

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0) {
                    _dataset = GetDataset(_id);
                }
                else
                {
                    _dataset = null;
                    _formMode = FormMode.AddMode;
                }
            }
            else
            {
                throw new Exception("id not passed as parameter");
            }

            string action = string.Empty;
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
                Master.MainMenu.SetActive("AdminDataset");

                LoadCategoryDropDownList();
                LoadElementDropDownList();
                LoadConditionListBox();

                if (_dataset != null)
                {
                    RenderDataset();
                }
            };
            RenderGrids();
        }

        #region "Rendering"

        private void RenderDataset()
        {
            txtUID.Value = _dataset.Id.ToString();
            txtName.Value = _dataset.DatasetName;
            txtHelp.Value = _dataset.Help;
            txtContextType.Value = _dataset.ContextType.Description;

            var rule = _dataset.GetRule(DatasetRuleType.MandatoryFieldsProminent);
            ddlMandatoryFieldRule.SelectedValue = rule.RuleActive ? "Y" : "N";

            // Audit
            txtCreated.Value = _dataset.GetCreatedStamp();
            txtUpdated.Value = _dataset.GetLastUpdatedStamp();
        }

        private void RenderGrids()
        {
            RenderCategories();
            RenderElements();
        }

        private void RenderCategories()
        {
            TableRow row;
            TableCell cell;

            LinkButton sortbtn;

            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_1.Rows)
            {
                if (temprow.Cells[0].Text != "Category")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) 
            {
                dt_1.Rows.Remove(temprow);
            }

            var i = 0;
            var max = _dataset.DatasetCategories.Where(c => c.System == false).Count();
            foreach (var cat in _dataset.DatasetCategories.Where(c => c.System == false).OrderBy(c => c.CategoryOrder))
            {
                i++;
                row = new TableRow();

                cell = new TableCell();
                cell.Text = cat.DatasetCategoryName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = cat.DatasetCategoryElements.Count.ToString();
                row.Cells.Add(cell);

                // Sortable
                cell = new TableCell();
                if (i < max)
                {
                    sortbtn = new LinkButton();
                    sortbtn.ID = "cd_" + cat.Id;
                    sortbtn.CssClass = _sortIconDown;
                    sortbtn.Click += btnSortCategory_Click;
                    sortbtn.Style["color"] = "#3276B1";
                    sortbtn.Style["padding-left"] = "1px";
                    sortbtn.Style["padding-right"] = "1px";
                    cell.Controls.Add(sortbtn);
                }

                if (i > 1)
                {
                    sortbtn = new LinkButton();
                    sortbtn.ID = "cu_" + cat.Id;
                    sortbtn.CssClass = _sortIconUp;
                    sortbtn.Click += btnSortCategory_Click;
                    sortbtn.Style["color"] = "#3276B1";
                    sortbtn.Style["padding-left"] = "1px";
                    sortbtn.Style["padding-right"] = "1px";
                    cell.Controls.Add(sortbtn);
                }
                row.Cells.Add(cell);

                var conditions = "";
                foreach (var con in cat.Conditions) {
                    conditions += con.Condition.Id.ToString() + ",";
                }
                if (conditions != "") {
                    conditions = conditions.Substring(0, conditions.Length - 1);
                }

                if (cat.DatasetCategoryElements.Count == 0)
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
                        NavigateUrl = "#",
                        Text = "Edit Category"
                    };

                    hyp.Attributes.Add("data-toggle", "modal");
                    hyp.Attributes.Add("data-target", "#categoryModal");
                    hyp.Attributes.Add("data-id", cat.Id.ToString());
                    hyp.Attributes.Add("data-evt", "edit");
                    hyp.Attributes.Add("data-name", cat.DatasetCategoryName);
                    hyp.Attributes.Add("data-friendlyname", cat.FriendlyName);
                    hyp.Attributes.Add("data-help", cat.Help);
                    hyp.Attributes.Add("data-evt", "edit");
                    hyp.Attributes.Add("data-acute", cat.Acute ? "Yes" : "No");
                    hyp.Attributes.Add("data-chronic", cat.Chronic ? "Yes" : "No");
                    hyp.Attributes.Add("data-conditions", conditions);
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "#",
                        Text = "Delete Category"
                    };
                    hyp.Attributes.Add("data-toggle", "modal");
                    hyp.Attributes.Add("data-target", "#categoryModal");
                    hyp.Attributes.Add("data-id", cat.Id.ToString());
                    hyp.Attributes.Add("data-evt", "delete");
                    hyp.Attributes.Add("data-name", cat.DatasetCategoryName);
                    hyp.Attributes.Add("data-acute", cat.Acute ? "Yes" : "No");
                    hyp.Attributes.Add("data-chronic", cat.Chronic ? "Yes" : "No");
                    hyp.Attributes.Add("data-conditions", conditions);
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);

                    cell.Controls.Add(pnl);
                }
                else
                {
                    cell = new TableCell();
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "#",
                        CssClass = "btn btn-default",
                        Text = "Edit Category"
                    };
                    hyp.Attributes.Add("data-toggle", "modal");
                    hyp.Attributes.Add("data-target", "#categoryModal");
                    hyp.Attributes.Add("data-id", cat.Id.ToString());
                    hyp.Attributes.Add("data-evt", "edit");
                    hyp.Attributes.Add("data-name", cat.DatasetCategoryName);
                    hyp.Attributes.Add("data-friendlyname", cat.FriendlyName);
                    hyp.Attributes.Add("data-help", cat.Help);
                    hyp.Attributes.Add("data-acute", cat.Acute ? "Yes" : "No");
                    hyp.Attributes.Add("data-chronic", cat.Chronic ? "Yes" : "No");
                    hyp.Attributes.Add("data-conditions", conditions);
                    cell.Controls.Add(hyp);
                }

                row.Cells.Add(cell);

                dt_1.Rows.Add(row);
            }
        }

        private void RenderElements()
        {
            if (_dataset.DatasetCategories.Count == 0) { return; };

            TableRow row;
            TableCell cell;

            LinkButton sortbtn;

            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_2.Rows)
            {
                if (temprow.Cells[0].Text != "Element")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) 
            {
                dt_2.Rows.Remove(temprow);
            }

            var datasetCategory = GetDatasetCategory(Convert.ToInt32(ddlCategory.SelectedValue));

            if(datasetCategory.DatasetCategoryElements.Count > 0)
            {
                var i = 0;
                foreach (var ele in datasetCategory.DatasetCategoryElements.OrderBy(e => e.FieldOrder))
                {
                    i++;
                    row = new TableRow();

                    cell = new TableCell();
                    cell.Text = ele.DatasetElement.ElementName;
                    row.Cells.Add(cell);

                    // Sortable
                    cell = new TableCell();
                    if (i < datasetCategory.DatasetCategoryElements.Count)
                    {
                        sortbtn = new LinkButton();
                        sortbtn.ID = "ed_" + ele.Id;
                        sortbtn.CssClass = _sortIconDown;
                        sortbtn.Click += btnSortElement_Click;
                        sortbtn.Style["color"] = "#3276B1";
                        sortbtn.Style["padding-left"] = "1px";
                        sortbtn.Style["padding-right"] = "1px";
                        cell.Controls.Add(sortbtn);
                    }

                    if (i > 1)
                    {
                        sortbtn = new LinkButton();
                        sortbtn.ID = "eu_" + ele.Id;
                        sortbtn.CssClass = _sortIconUp;
                        sortbtn.Click += btnSortElement_Click;
                        sortbtn.Style["color"] = "#3276B1";
                        sortbtn.Style["padding-left"] = "1px";
                        sortbtn.Style["padding-right"] = "1px";
                        cell.Controls.Add(sortbtn);
                    }
                    row.Cells.Add(cell);

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
                        NavigateUrl = "#",
                        Text = "Edit Element"
                    };

                    var conditions = "";
                    foreach (var con in ele.Conditions) {
                        conditions += con.Condition.Id.ToString() + ",";
                    }
                    if (conditions != "") {
                        conditions = conditions.Substring(0, conditions.Length - 1);
                    }

                    hyp.Attributes.Add("data-toggle", "modal");
                    hyp.Attributes.Add("data-target", "#elementModal");
                    hyp.Attributes.Add("data-id", ele.Id.ToString());
                    hyp.Attributes.Add("data-name", ele.DatasetElement.ElementName);
                    hyp.Attributes.Add("data-friendlyname", ele.FriendlyName);
                    hyp.Attributes.Add("data-help", ele.Help);
                    hyp.Attributes.Add("data-evt", "edit");
                    hyp.Attributes.Add("data-acute", ele.Acute ? "Yes" : "No");
                    hyp.Attributes.Add("data-chronic", ele.Chronic ? "Yes" : "No");
                    hyp.Attributes.Add("data-conditions", conditions);
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "#",
                        Text = "Delete Element"
                    };
                    hyp.Attributes.Add("data-toggle", "modal");
                    hyp.Attributes.Add("data-target", "#elementModal");
                    hyp.Attributes.Add("data-id", ele.Id.ToString());
                    hyp.Attributes.Add("data-name", ele.DatasetElement.ElementName);

                    hyp.Attributes.Add("data-evt", "delete");
                    hyp.Attributes.Add("data-acute", ele.Acute ? "Yes" : "No");
                    hyp.Attributes.Add("data-chronic", ele.Chronic ? "Yes" : "No");
                    hyp.Attributes.Add("data-warning", (ele.SourceMappings.Count > 0 || ele.DestinationMappings.Count > 0) ? "Yes" : "No");
                    hyp.Attributes.Add("data-conditions", conditions);
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);

                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);

                    dt_2.Rows.Add(row);
                }
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
                        NavigateUrl = "ManageDatasetEdit.aspx?id=" + _dataset.Id.ToString() + "&a=edit",
                        CssClass = "btn btn-primary",
                        Text = "Edit"
                    };
                    spnButtons.Controls.Add(hyp);

                    hyp = new HyperLink()
                    {
                        ID = "btnReturn",
                        NavigateUrl = "ManageDataset.aspx",
                        CssClass = "btn btn-default",
                        Text = "Return"
                    };
                    spnButtons.Controls.Add(hyp);

                    hyp = new HyperLink()
                    {
                        ID = "btnDownloadSQLScript",
                        NavigateUrl = "/FileDownload/DownloadDatasetScript?datasetId=" + _dataset.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "SQL Script"
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
                        NavigateUrl = "ManageDatasetEdit.aspx?id=" + _dataset.Id.ToString(),
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
                        NavigateUrl = "ManageDataset.aspx",
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
                    txtName.Attributes.Add("readonly", "true");
                    txtName.Style.Add("background-color", "#EBEBE4");

                    txtHelp.Attributes.Add("readonly", "true");
                    txtHelp.Style.Add("background-color", "#EBEBE4");

                    ddlMandatoryFieldRule.Attributes.Add("readonly", "true");
                    ddlMandatoryFieldRule.Style.Add("background-color", "#EBEBE4");

                    btnAddCategory.Visible = true;
                    btnAddElement.Visible = true;

                    break;

                case FormMode.EditMode:

                    btnAddCategory.Visible = false;
                    btnAddElement.Visible = false;

                    break;

                case FormMode.AddMode:

                    divAudit.Visible = false;
                    divContext.Visible = false;
                    divUnique.Visible = false;

                    btnAddCategory.Visible = false;
                    btnAddElement.Visible = false;

                    break;

                default:
                    break;
            };
        }

        private void LoadCategoryDropDownList()
        {
            ListItem item;

            ddlCategory.Items.Clear();
            if(_dataset.DatasetCategories.Count == 0)
            {
                item = new ListItem();
                item.Text = "-- NO CATEGORIES --";
                item.Value = "0";
                ddlCategory.Items.Add(item);
            }
            else
            {
                foreach (DatasetCategory cat in _dataset.DatasetCategories.Where(c => c.System == false).OrderBy(c => c.CategoryOrder))
                {
                    item = new ListItem();
                    item.Text = cat.DatasetCategoryName;
                    item.Value = cat.Id.ToString();
                    ddlCategory.Items.Add(item);
                }
            }
        }

        private void LoadElementDropDownList()
        {
            if(ddlCategory.Items.Count == 0) { return; };
            if(ddlCategory.SelectedValue == "") { return; };
            
            ListItem item;

            // Only allow adding of elements that have not been linked to this dataset yet or to any system based datasets
            var ids = UnitOfWork.Repository<Dataset>().Queryable().Where(d => d.IsSystem == true && d.Id != _dataset.Id).Select(d => d.Id).ToList();
            ids.Add(_dataset.Id);

            ddlElement.Items.Clear();
            foreach (DatasetElement ele in UnitOfWork.Repository<DatasetElement>().Queryable().Where(de => !de.DatasetCategoryElements.Any(dce => ids.Contains(dce.DatasetCategory.Dataset.Id))).OrderBy(de => de.ElementName))
            {
                item = new ListItem();
                item.Text = ele.ElementName;
                item.Value = ele.Id.ToString();
                ddlElement.Items.Add(item);
            }
            if(ddlElement.Items.Count == 0)
            {
                item = new ListItem();
                item.Text = "-- No elements available --";
                item.Value = "0";
                ddlElement.Items.Add(item);
            }
        }

        private void LoadConditionListBox()
        {
            ListItem item;

            foreach (Condition con in UnitOfWork.Repository<Condition>().Queryable().Where(c => c.Chronic == true).OrderBy(c => c.Description))
            {
                item = new ListItem();
                item.Text = con.Description;
                item.Value = con.Id.ToString();
                lbCondition.Items.Add(item);

                item = new ListItem();
                item.Text = con.Description;
                item.Value = con.Id.ToString();
                lbCatCondition.Items.Add(item);
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
                if (!IsDatasetUnique(txtName.Value, 0)) {
                    err += "<li>Dataset with same name already added...</li>";
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
                if (!IsDatasetUnique(txtName.Value, _dataset.Id)) {
                    err += "<li>Dataset with same name already added...</li>";
                }

                if (err != "<ul>")
                {
                    err += "</ul>";
                    divError.Visible = true;
                    spnErrors.InnerHtml = err;
                    return;
                }
            }

            SaveDataset();

            var url = String.Format("ManageDatasetEdit.aspx?id=" + _dataset.Id.ToString());
            Response.Redirect(url);
        }

        private void SaveDataset()
        {
            short temp;
            decimal tempd;

            if (_dataset == null)
            {
                // Prepare new dataset
                var contextType = GetContextType("Encounter");

                _dataset = new Dataset { DatasetName = txtName.Value, Active = true, ContextType = contextType, Help = txtHelp.Value };
                UnitOfWork.Repository<Dataset>().Save(_dataset);
            }
            else
            {
                // Prepare updated dataset
                _dataset.DatasetName = txtName.Value;
                _dataset.Help = txtHelp.Value;

                UnitOfWork.Repository<Dataset>().Update(_dataset);
            }

            var rule = _dataset.GetRule(DatasetRuleType.MandatoryFieldsProminent);
            rule.RuleActive = (ddlMandatoryFieldRule.SelectedValue == "Y");
            UnitOfWork.Repository<DatasetRule>().Update(rule);

            // Ensure this dataset has a corresponding required information system category
            SetRequiredCategory();
        }

        private void SetRequiredCategory()
        {
            var reqCat = UnitOfWork.Repository<DatasetCategory>().Queryable().SingleOrDefault(dc => dc.Dataset.Id == _dataset.Id && dc.DatasetCategoryName == "Required Information");
            if(reqCat == null)
            {
                // Save category
                reqCat = new DatasetCategory
                {
                    CategoryOrder = 0,
                    Dataset = _dataset,
                    DatasetCategoryName = "Required Information",
                    FriendlyName = "Required Information",
                    Public = false,
                    System = true,
                    Acute = false,
                    Chronic = false
                };

                foreach (DatasetCategory idc in _dataset.DatasetCategories)
                {
                    foreach (DatasetCategoryElement idce in idc.DatasetCategoryElements.Where(i => i.DatasetElement.Field.Mandatory))
                    {
                        var catEle = new DatasetCategoryElement() { Acute = false, Chronic = false, DatasetCategory = reqCat, DatasetElement = idce.DatasetElement,  };
                        reqCat.DatasetCategoryElements.Add(catEle);
                    }
                }

                UnitOfWork.Repository<DatasetCategory>().Save(reqCat);
            }
        }

        #endregion

        #region "Events"

        protected void btnSortCategory_Click(object sender, EventArgs e)
        {
            int tempi;
            var btn = (LinkButton)sender;
            string[] stringSeparators = new string[] {"_"};
            string[] ida = btn.ID.Split(stringSeparators, StringSplitOptions.None);
            var action = ida[0];
            var id = Int32.TryParse(ida[1], out tempi) ? Convert.ToInt32(ida[1]) : 0;
            if (id > 0)
            {
                var cat = UnitOfWork.Repository<DatasetCategory>().Queryable().Single(dc => dc.Id == id);
                if(action == "cu")
                {
                    var prevCat = UnitOfWork.Repository<DatasetCategory>().Queryable().OrderByDescending(dc => dc.CategoryOrder).First(dc => dc.Dataset.Id == cat.Dataset.Id && dc.CategoryOrder < cat.CategoryOrder);
                    cat.CategoryOrder -= 1;
                    prevCat.CategoryOrder += 1;

                    UnitOfWork.Repository<DatasetCategory>().Update(cat);
                    UnitOfWork.Repository<DatasetCategory>().Update(prevCat);
                }
                if (action == "cd")
                {
                    var nextCat = UnitOfWork.Repository<DatasetCategory>().Queryable().OrderBy(dc => dc.CategoryOrder).First(dc => dc.Dataset.Id == cat.Dataset.Id && dc.CategoryOrder > cat.CategoryOrder);
                    cat.CategoryOrder += 1;
                    nextCat.CategoryOrder -= 1;

                    UnitOfWork.Repository<DatasetCategory>().Update(cat);
                    UnitOfWork.Repository<DatasetCategory>().Update(nextCat);
                }
                
                HttpCookie cookie = new HttpCookie("PopUpMessage");
                cookie.Value = "Category order set successfully";
                Response.Cookies.Add(cookie);
                Master.ShouldPopUpBeDisplayed();

                LoadCategoryDropDownList();
                LoadElementDropDownList();
                RenderGrids();
            }
        }

        protected void btnSortElement_Click(object sender, EventArgs e)
        {
            int tempi;
            var btn = (LinkButton)sender;
            string[] stringSeparators = new string[] { "_" };
            string[] ida = btn.ID.Split(stringSeparators, StringSplitOptions.None);
            var action = ida[0];
            var id = Int32.TryParse(ida[1], out tempi) ? Convert.ToInt32(ida[1]) : 0;
            if (id > 0)
            {
                var ele = UnitOfWork.Repository<DatasetCategoryElement>().Queryable().Single(dce => dce.Id == id);
                if (action == "eu")
                {
                    var prevEle = UnitOfWork.Repository<DatasetCategoryElement>().Queryable().OrderByDescending(dce => dce.FieldOrder).First(dce => dce.DatasetCategory.Id == ele.DatasetCategory.Id && dce.FieldOrder < ele.FieldOrder);
                    ele.FieldOrder -= 1;
                    prevEle.FieldOrder += 1;

                    UnitOfWork.Repository<DatasetCategoryElement>().Update(ele);
                    UnitOfWork.Repository<DatasetCategoryElement>().Update(prevEle);
                }
                if (action == "ed")
                {
                    var nextEle = UnitOfWork.Repository<DatasetCategoryElement>().Queryable().OrderBy(dce => dce.FieldOrder).First(dce => dce.DatasetCategory.Id == ele.DatasetCategory.Id && dce.FieldOrder > ele.FieldOrder);
                    ele.FieldOrder += 1;
                    nextEle.FieldOrder -= 1;

                    UnitOfWork.Repository<DatasetCategoryElement>().Update(ele);
                    UnitOfWork.Repository<DatasetCategoryElement>().Update(nextEle);
                }

                HttpCookie cookie = new HttpCookie("PopUpMessage");
                cookie.Value = "Element order set successfully";
                Response.Cookies.Add(cookie);
                Master.ShouldPopUpBeDisplayed();

                RenderGrids();
            }
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            lblCategoryName.Attributes.Remove("class");
            lblCategoryName.Attributes.Add("class", "input");

            if (String.IsNullOrEmpty(txtCategoryName.Value))
            {
                lblCategoryName.Attributes.Remove("class");
                lblCategoryName.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Please enter a category name";
                lblCategoryName.Controls.Add(errorMessageDiv);

                RenderGrids();
                return;
            }

            if (Regex.Matches(txtCategoryName.Value, @"[a-zA-Z ']").Count < txtCategoryName.Value.Length)
            {
                lblCategoryName.Attributes.Remove("class");
                lblCategoryName.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Category name contains invalid characters (Enter A-Z, a-z)";
                lblCategoryName.Controls.Add(errorMessageDiv);

                RenderGrids();
                return;
            }

            // Ensure unique
            if (!IsCategoryUnique(txtCategoryName.Value, 0)) 
            {
                lblCategoryName.Attributes.Remove("class");
                lblCategoryName.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Duplicate category name. Please specity again.";
                lblCategoryName.Controls.Add(errorMessageDiv);

                RenderGrids();
                return;
            }

            var encodedName = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(txtCategoryName.Value, false);

            // Save category
            var cat = new DatasetCategory
            {
                CategoryOrder = (short)GetNextCategoryOrder(),
                Dataset = _dataset,
                DatasetCategoryName = encodedName 
            };

            UnitOfWork.Repository<DatasetCategory>().Save(cat);
            UnitOfWork.Complete();

            RenderGrids();
        }

        protected void btnAddElement_Click(object sender, EventArgs e)
        {
            if(ddlCategory.SelectedValue == "0") { return; };
            if(ddlElement.SelectedValue == "0") { return; };

            // Save element
            var datasetCategory = GetDatasetCategory(Convert.ToInt32(ddlCategory.SelectedValue));
            var datasetElement = GetDatasetElement(Convert.ToInt32(ddlElement.SelectedValue));

            var ele = new DatasetCategoryElement
            {
                DatasetCategory = datasetCategory,
                DatasetElement = datasetElement,
                FieldOrder = (short)GetNextElementOrder(datasetCategory)
            };

            UnitOfWork.Repository<DatasetCategoryElement>().Save(ele);

            if(datasetElement.Field.Mandatory)
            {
                // Ensure element is linked to Required Information category
                var reqCat = UnitOfWork.Repository<DatasetCategory>().Queryable().SingleOrDefault(dc => dc.Dataset.Id == _dataset.Id && dc.DatasetCategoryName == "Required Information");
                if(reqCat != null)
                {
                    var catEle = new DatasetCategoryElement() { Acute = false, Chronic = false, DatasetCategory = reqCat, DatasetElement = datasetElement };
                    reqCat.DatasetCategoryElements.Add(catEle);

                    UnitOfWork.Repository<DatasetCategory>().Update(reqCat);
                }
            }

            RenderGrids();
        }

        protected void btnSaveCategory_Click(object sender, EventArgs e)
        {
            txtCategoryName.Value = "";

            if (txtCategoryUID.Value != "0")
            {
                var datasetCategory = GetDatasetCategory(Convert.ToInt32(txtCategoryUID.Value));

                // Validation
                if (String.IsNullOrEmpty(txtMCategory.Value))
                {
                    lblCategoryName.Attributes.Remove("class");
                    lblCategoryName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Please enter a category name";
                    lblCategoryName.Controls.Add(errorMessageDiv);

                    RenderGrids();
                    return;
                }

                if (!IsCategoryUnique(txtMCategory.Value, datasetCategory.Id))
                {
                    lblCategoryName.Attributes.Remove("class");
                    lblCategoryName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Duplicate category name. Please specity again.";
                    lblCategoryName.Controls.Add(errorMessageDiv);

                    RenderGrids();
                    return;
                }

                var encodedName = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(txtMCategory.Value, false);

                datasetCategory.DatasetCategoryName = encodedName;
                datasetCategory.Acute = (ddlCatAcute.Value == "Yes");
                datasetCategory.Chronic = (ddlCatChronic.Value == "Yes");
                datasetCategory.FriendlyName = txtMCategoryFriendlyName.Value;
                datasetCategory.Help = txtMCategoryHelp.Value;

                // Now handle conditions
                ArrayList deleteCollection = new ArrayList();
                foreach (DatasetCategoryCondition con in datasetCategory.Conditions)
                {
                    var query_del = from ListItem item in lbCatCondition.Items where Convert.ToInt32(item.Value) == con.Condition.Id select item;
                    foreach (ListItem item in query_del)
                    {
                        if (!item.Selected) {
                            deleteCollection.Add(con);
                        }
                    }
                }
                ArrayList addCollection = new ArrayList();
                var query_add = from ListItem item in lbCatCondition.Items where item.Selected == true select item;
                foreach (ListItem item in query_add)
                {
                    if (!datasetCategory.Conditions.Any(c => c.Condition.Id == Convert.ToInt32(item.Value))) {
                        addCollection.Add(Convert.ToInt32(item.Value));
                    }
                }

                foreach (DatasetCategoryCondition con in deleteCollection)
                {
                    datasetCategory.Conditions.Remove(con);
                    UnitOfWork.Repository<DatasetCategoryCondition>().Delete(con);
                }
                foreach (Int32 id in addCollection)
                {
                    var con = new DatasetCategoryCondition() { Condition = GetCondition(id), DatasetCategory = datasetCategory };
                    datasetCategory.Conditions.Add(con);
                }

                UnitOfWork.Repository<DatasetCategory>().Update(datasetCategory);
                UnitOfWork.Complete();
            }

            RenderGrids();
        }

        protected void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (txtCategoryUID.Value != "0")
            {
                var datasetCategory = GetDatasetCategory(Convert.ToInt32(txtCategoryUID.Value));

                _dataset.DatasetCategories.Remove(datasetCategory);
                UnitOfWork.Complete();
            }

            RenderGrids();
        }

        protected void btnSaveElement_Click(object sender, EventArgs e)
        {
            if (txtElementUID.Value != "0")
            {
                var datasetCategoryElement = GetDatasetCategoryElement(Convert.ToInt32(txtElementUID.Value));

                datasetCategoryElement.Acute = (ddlAcute.Value == "Yes");
                datasetCategoryElement.Chronic = (ddlChronic.Value == "Yes");
                datasetCategoryElement.FriendlyName = txtMElementFriendlyName.Value;
                datasetCategoryElement.Help = txtMElementHelp.Value;

                // Now handle conditions
                ArrayList deleteCollection = new ArrayList();
                foreach (DatasetCategoryElementCondition con in datasetCategoryElement.Conditions)
                {
                    var query_del = from ListItem item in lbCondition.Items where Convert.ToInt32(item.Value) == con.Condition.Id select item;
                    foreach (ListItem item in query_del)
                    {
                        if(!item.Selected) {
                            deleteCollection.Add(con);
                        }
                    }
                }
                ArrayList addCollection = new ArrayList();
                var query_add = from ListItem item in lbCondition.Items where item.Selected == true select item;
                foreach (ListItem item in query_add)
                {
                    if (!datasetCategoryElement.Conditions.Any(c => c.Condition.Id == Convert.ToInt32(item.Value))) {
                        addCollection.Add(Convert.ToInt32(item.Value));
                    }
                }

                foreach (DatasetCategoryElementCondition con in deleteCollection) 
                {
                    datasetCategoryElement.Conditions.Remove(con);
                    UnitOfWork.Repository<DatasetCategoryElementCondition>().Delete(con);
                }
                foreach (Int32 id in addCollection)
                {
                    var con = new DatasetCategoryElementCondition() { Condition = GetCondition(id), DatasetCategoryElement = datasetCategoryElement };
                    datasetCategoryElement.Conditions.Add(con);
                }
                UnitOfWork.Repository<DatasetCategoryElement>().Update(datasetCategoryElement);

                UnitOfWork.Complete();
            }

            RenderGrids();
        }

        protected void btnDeleteElement_Click(object sender, EventArgs e)
        {
            if (txtElementUID.Value != "0")
            {
                var datasetCategory = GetDatasetCategory(Convert.ToInt32(ddlCategory.SelectedValue));
                var datasetCategoryElement = GetDatasetCategoryElement(Convert.ToInt32(txtElementUID.Value));

                datasetCategory.DatasetCategoryElements.Remove(datasetCategoryElement);
                UnitOfWork.Repository<DatasetCategoryElement>().Delete(datasetCategoryElement);
                UnitOfWork.Complete();
            }

            RenderGrids();
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategory.SelectedValue == "0") { return; };

            LoadElementDropDownList();
            RenderGrids();
        }

        #endregion

        #region "EF"

        private Condition GetCondition(int id)
        {
            return UnitOfWork.Repository<Condition>().Queryable().SingleOrDefault(c => c.Id == id);
        }

        private Dataset GetDataset(int id)
        {
            return UnitOfWork.Repository<Dataset>().Queryable().SingleOrDefault(u => u.Id == id);
        }

        private DatasetCategory GetDatasetCategory(int id)
        {
            return UnitOfWork.Repository<DatasetCategory>().Queryable().Include("DatasetCategoryElements").SingleOrDefault(u => u.Id == id);
        }

        private DatasetElement GetDatasetElement(int id)
        {
            return UnitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.Id == id);
        }

        private DatasetCategoryElement GetDatasetCategoryElement(int id)
        {
            return UnitOfWork.Repository<DatasetCategoryElement>().Queryable().SingleOrDefault(u => u.Id == id);
        }

        private ContextType GetContextType(string type)
        {
            return UnitOfWork.Repository<ContextType>().Queryable().SingleOrDefault(u => u.Description == type);
        }

        private bool IsCategoryUnique(string cat, int id)
        {
            int count = 0;
            if (id > 0) {
                count = UnitOfWork.Repository<DatasetCategory>().Queryable().Where(dc => dc.Dataset.Id == _dataset.Id && dc.DatasetCategoryName == cat && dc.Id != id).Count();
            }
            else {
                count = UnitOfWork.Repository<DatasetCategory>().Queryable().Where(dc => dc.Dataset.Id == _dataset.Id && dc.DatasetCategoryName == cat).Count();
            }
            return (count == 0);
        }

        private int GetNextCategoryOrder()
        {
            int order = 0;

            if (_dataset.DatasetCategories.Count == 0) {
                return 1;
            }
            else {
                return UnitOfWork.Repository<DatasetCategory>().Queryable().Where(dc => dc.Dataset.Id == _dataset.Id).OrderByDescending(dc => dc.CategoryOrder).First().CategoryOrder + 1;
            }
        }

        private int GetNextElementOrder(DatasetCategory cat)
        {
            int order = 0;

            if (cat.DatasetCategoryElements.Count == 0)
            {
                return 1;
            }
            else
            {
                return cat.DatasetCategoryElements.OrderByDescending(de => de.FieldOrder).First().FieldOrder + 1;
            }
        }

        private bool IsDatasetUnique(string dataset, int id)
        {
            int count = 0;
            if (id > 0) {
                count = UnitOfWork.Repository<Dataset>().Queryable().Where(ds => ds.DatasetName == dataset && ds.Id != id).Count();
            }
            else {
                count = UnitOfWork.Repository<Dataset>().Queryable().Where(ds => ds.DatasetName == dataset).Count();
            }
            return (count == 0);
        }

        #endregion

    }
}
