using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class ManageDatasetElementEdit : MainPageBase
    {
        private int _id;
        private DatasetElement _element;

        private int _sid;
        private DatasetElementSub _subElement;

        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3 };
        private FormMode _formMode = FormMode.ViewMode;

        private enum FormContext { ElementContext = 1, SubElementContext = 2 };
        private FormContext _formContext = FormContext.ElementContext;

        private string _sortIconUp = "fa fa-arrow-circle-up fa-2x";
        private string _sortIconDown = "fa fa-arrow-circle-down fa-2x";

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0) {
                    _element = GetDatasetElement(_id);
                }
                else
                {
                    _element = null;
                    _formMode = FormMode.AddMode;
                }
            }
            else
            {
                throw new Exception("id not passed as parameter");
            }

            if (Request.QueryString["sid"] != null)
            {
                _sid = Convert.ToInt32(Request.QueryString["sid"]);
                _formContext = FormContext.SubElementContext;
                if (_sid > 0) {
                    _subElement = GetDatasetElementSub(_sid);
                }
                else {
                    _subElement = null;
                    _formMode = FormMode.AddMode;
                }
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

            LoadDropDownList();
            RenderButtons();
            if (_formContext == FormContext.SubElementContext)
            {
                RenderMainElementForSub();
                if (_subElement != null)
                {
                    RenderSubElement();
                    RenderSubElementGroupValues();
                }
            }
            else
            {
                if (_element != null)
                {
                    RenderElement();
                    RenderCategories();
                }
            }
            ToggleView();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            divError.Visible = false;
            spnErrors.InnerHtml = "";

            if (!Page.IsPostBack)
            {
                Master.SetMenuActive("AdminDatasetElement");

                divAlpha.Visible = false;
                divNumeric.Visible = false;
            };

        }

        #region "Rendering"

        private void RenderButtons()
        {
            Button btn;
            HyperLink hyp;

            var url = "";

            switch (_formMode)
            {
                case FormMode.ViewMode:
                    // Values buttons
                    if (_formContext == FormContext.ElementContext)
                    {
                        if (_element.System == false)
                        {
                            hyp = new HyperLink()
                            {
                                ID = "btnEdit",
                                NavigateUrl = "ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString() + "&a=edit",
                                CssClass = "btn btn-primary",
                                Text = "Edit Element"
                            };
                            spnButtons.Controls.Add(hyp);

                            hyp = new HyperLink()
                            {
                                ID = "btnDelete ",
                                NavigateUrl = String.Format("/Admin/DeleteDatasetElement?id={0}&cancelRedirectUrl={1}", _element.Id.ToString(), "/Admin/ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString()),
                                CssClass = "btn btn-default",
                                Text = "Delete Element"
                            };
                            spnButtons.Controls.Add(hyp);
                        }

                        hyp = new HyperLink()
                        {
                            ID = "btnReturn",
                            NavigateUrl = "ManageDatasetElement.aspx",
                            CssClass = "btn btn-default",
                            Text = "Return"
                        };
                        spnButtons.Controls.Add(hyp);

                        switch (_element.Field.FieldType.Description)
                        {
                            case "Listbox":
                            case "DropDownList":
                                hyp = new HyperLink()
                                {
                                    ID = "btnAddValue",
                                    NavigateUrl = "#",
                                    CssClass = "btn btn-primary",
                                    Text = "Add Value"
                                };
                                hyp.Attributes.Add("data-toggle", "modal");
                                hyp.Attributes.Add("data-target", "#valueModal");
                                hyp.Attributes.Add("data-id", "0");
                                hyp.Attributes.Add("data-evt", "add");
                                hyp.Attributes.Add("data-typ", _element.Field.FieldType.Description);
                                hyp.Attributes.Add("data-value", "");
                                hyp.Attributes.Add("data-default", "Yes");
                                hyp.Attributes.Add("data-other", "No");
                                hyp.Attributes.Add("data-unknown", "No");
                                spnValueButtons.Controls.Add(hyp);

                                break;

                            case "Table":
                                hyp = new HyperLink()
                                {
                                    ID = "btnAddValue",
                                    NavigateUrl = "ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString() + "&sid=0",
                                    CssClass = "btn btn-primary",
                                    Text = "Add Element"
                                };
                                spnValueButtons.Controls.Add(hyp);

                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        hyp = new HyperLink()
                        {
                            ID = "btnEdit",
                            NavigateUrl = "ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString() + "&sid=" + _subElement.Id.ToString() + "&a=edit",
                            CssClass = "btn btn-primary",
                            Text = "Edit"
                        };
                        spnButtons.Controls.Add(hyp);

                        btn = new Button();
                        btn.ID = "btnDelete";
                        btn.CssClass = "btn btn-default";
                        btn.Text = "Delete";
                        btn.OnClientClick = "return confirm('Are you sure you would like to continue?');";
                        btn.Click += btnDelete_Click;
                        btn.Attributes.Add("formnovalidate", "formnovalidate");
                        spnButtons.Controls.Add(btn);

                        hyp = new HyperLink()
                        {
                            ID = "btnReturn",
                            NavigateUrl = "ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString(),
                            CssClass = "btn btn-default",
                            Text = "Return To Element"
                        };
                        spnButtons.Controls.Add(hyp);

                        switch (_subElement.Field.FieldType.Description)
                        {
                            case "Listbox":
                            case "DropDownList":
                                hyp = new HyperLink()
                                {
                                    ID = "btnAddValue",
                                    NavigateUrl = "#",
                                    CssClass = "btn btn-primary",
                                    Text = "Add Value"
                                };
                                hyp.Attributes.Add("data-toggle", "modal");
                                hyp.Attributes.Add("data-target", "#valueModal");
                                hyp.Attributes.Add("data-id", "0");
                                hyp.Attributes.Add("data-evt", "add");
                                hyp.Attributes.Add("data-typ", _subElement.Field.FieldType.Description);
                                hyp.Attributes.Add("data-value", "");
                                hyp.Attributes.Add("data-default", "Yes");
                                hyp.Attributes.Add("data-other", "No");
                                hyp.Attributes.Add("data-unknown", "No");

                                spnValueButtons.Controls.Add(hyp);

                                break;

                            default:
                                break;
                        }
                    }

                    break;

                case FormMode.EditMode:
                    btn = new Button();
                    btn.ID = "btnSave";
                    btn.CssClass = "btn btn-primary";
                    btn.Text = "Save";
                    btn.Click += btnSave_Click;
                    btn.Attributes.Add("formnovalidate", "formnovalidate");
                    spnButtons.Controls.Add(btn);

                    if (_formContext == FormContext.ElementContext)
                    {
                        hyp = new HyperLink()
                        {
                            ID = "btnCancel",
                            NavigateUrl = "ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString(),
                            CssClass = "btn btn-default",
                            Text = "Cancel"
                        };
                    }
                    else
                    {
                        hyp = new HyperLink()
                        {
                            ID = "btnCancel",
                            NavigateUrl = "ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString() + "&sid=" + _subElement.Id.ToString(),
                            CssClass = "btn btn-default",
                            Text = "Cancel"
                        };
                    }
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
                        NavigateUrl = "ManageDatasetElement.aspx",
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

        private void RenderElement()
        {
            divElementDetail.Visible = false;

            txtUID.Value = _element.Id.ToString();
            txtDescription.Value = _element.ElementName;
            txtOID.Value = _element.OID;
            txtDefaultValue.Value = _element.DefaultValue;
            ddlMandatory.Value = _element.Field.Mandatory ? "Yes" : "No";
            ddlSystem.Value = _element.System ? "Yes" : "No";
            ddlAnon.Value = _element.Field.Anonymise ? "Yes" : "No";
            ddlFieldType.SelectedValue = _element.Field.FieldType.Id.ToString();

            var rule = _element.GetRule(DatasetRuleType.ElementCanoOnlyLinkToSingleDataset);
            ddlSingleDatasetRule.SelectedValue = rule.RuleActive ? "Y" : "N";

            HideFieldTypePanels();

            switch (ddlFieldType.SelectedItem.Text)
            {
                case "AlphaNumericTextbox":
                    divAlpha.Visible = true;
                    txtMaxLength.Value = _element.Field.MaxLength.ToString();
                    RenderElementGroupValues();

                    break;

                case "NumericTextbox":
                    divNumeric.Visible = true;
                    txtDecimal.Value = _element.Field.Decimals.ToString();
                    txtMinimum.Value = _element.Field.MinSize.ToString();
                    txtMaximum.Value = _element.Field.MaxSize.ToString();
                    break;

                case "Listbox":
                    if(_formMode == FormMode.ViewMode)
                    {
                        wid_id_2.Visible = true;
                        divListBox.Visible = true;
                    }
                    RenderListValues();
                    RenderElementGroupValues();
                    
                    break;

                case "DropDownList":
                    if(_formMode == FormMode.ViewMode)
                    {
                        wid_id_2.Visible = true;
                        divDropDownList.Visible = true;
                    }
                    RenderListValues();
                    RenderElementGroupValues();
                    
                    break;

                case "Table":
                    if (_formMode == FormMode.ViewMode)
                    {
                        wid_id_2.Visible = true;
                        divTable.Visible = true;
                    }
                    RenderTableValues();

                    break;

                default:
                    break;
            }
        }

        private void RenderMainElementForSub()
        {
            divElementDetail.Visible = true;
            txtMainElementID.Value = _element.Id.ToString();
            txtMainElement.Value = _element.ElementName;
        }

        private void RenderSubElement()
        {
            txtUID.Value = _subElement.Id.ToString();
            txtDescription.Value = _subElement.ElementName;
            txtFriendlyName.Value = _subElement.FriendlyName;
            txtHelp.Value = _subElement.Help;
            txtOID.Value = _subElement.OID;
            txtDefaultValue.Value = _subElement.DefaultValue;
            ddlMandatory.Value = _subElement.Field.Mandatory ? "Yes" : "No";
            ddlSystem.Value = _subElement.System ? "Yes" : "No";
            ddlAnon.Value = _subElement.Field.Anonymise ? "Yes" : "No";
            ddlFieldType.SelectedValue = _subElement.Field.FieldType.Id.ToString();

            HideFieldTypePanels();
            divSubElement.Style["display"] = "block";

            switch (ddlFieldType.SelectedItem.Text)
            {
                case "AlphaNumericTextbox":
                    divAlpha.Visible = true;
                    txtMaxLength.Value = _subElement.Field.MaxLength.ToString();
                    break;

                case "NumericTextbox":
                    divNumeric.Visible = true;
                    txtDecimal.Value = _subElement.Field.Decimals.ToString();
                    txtMinimum.Value = _subElement.Field.MinSize.ToString();
                    txtMaximum.Value = _subElement.Field.MaxSize.ToString();
                    break;

                case "Listbox":
                    if (_formMode == FormMode.ViewMode)
                    {
                        wid_id_2.Visible = true;
                        divListBox.Visible = true;
                    }
                    RenderSubListValues();

                    break;

                case "DropDownList":
                    if (_formMode == FormMode.ViewMode)
                    {
                        wid_id_2.Visible = true;
                        divDropDownList.Visible = true;
                    }
                    RenderSubListValues();

                    break;

                default:
                    break;
            }
        }

        private void RenderListValues()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            foreach(FieldValue value in _element.Field.FieldValues.OrderBy(fv => fv.Id))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = value.Value;
                row.Cells.Add(cell);

                if(_element.Field.FieldType.Description == "DropDownList")
                {
                    cell = new TableCell();
                    cell.Text = value.Default ? "Yes" : "No";
                    row.Cells.Add(cell);
                }

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
                    Text = "Edit Value"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#valueModal");
                hyp.Attributes.Add("data-id", value.Id.ToString());
                hyp.Attributes.Add("data-evt", "edit");
                hyp.Attributes.Add("data-typ", _element.Field.FieldType.Description);
                hyp.Attributes.Add("data-value", value.Value);
                hyp.Attributes.Add("data-default", value.Default ? "Yes" : "No" );
                hyp.Attributes.Add("data-defaulto", _element.Field.HasDefaultValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-other", value.Other ? "Yes" : "No" );
                hyp.Attributes.Add("data-othero", _element.Field.HasOtherValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-unknown", value.Unknown ? "Yes" : "No");
                hyp.Attributes.Add("data-unknowno", _element.Field.HasUnknownValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-inuse", IsFieldValueInUse(value) ? "Yes" : "No");

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    Text = "Delete Value"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#valueModal");
                hyp.Attributes.Add("data-id", value.Id.ToString());
                hyp.Attributes.Add("data-evt", "delete");
                hyp.Attributes.Add("data-typ", _element.Field.FieldType.Description);
                hyp.Attributes.Add("data-value", value.Value);
                hyp.Attributes.Add("data-default", value.Default ? "Yes" : "No");
                hyp.Attributes.Add("data-defaulto", _element.Field.HasDefaultValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-other", value.Other ? "Yes" : "No");
                hyp.Attributes.Add("data-othero", _element.Field.HasOtherValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-unknown", value.Unknown ? "Yes" : "No");
                hyp.Attributes.Add("data-unknowno", _element.Field.HasUnknownValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-inuse", IsFieldValueInUse(value) ? "Yes" : "No");
                
                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                pnl.Controls.Add(ul);

                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                switch (_element.Field.FieldType.Description)
	            {
                    case "DropDownList":
                        dt_1.Rows.Add(row);
                        break;

                    case "Listbox":
                        dt_2.Rows.Add(row);
                        break;

                    default:
                        break;
	            }
            }
        }

        private void RenderSubListValues()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            foreach (FieldValue value in _subElement.Field.FieldValues)
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = value.Value;
                row.Cells.Add(cell);

                if (_subElement.Field.FieldType.Description == "DropDownList")
                {
                    cell = new TableCell();
                    cell.Text = value.Default ? "Yes" : "No";
                    row.Cells.Add(cell);
                }

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
                    Text = "Edit Value"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#valueModal");
                hyp.Attributes.Add("data-id", value.Id.ToString());
                hyp.Attributes.Add("data-evt", "edit");
                hyp.Attributes.Add("data-typ", _subElement.Field.FieldType.Description);
                hyp.Attributes.Add("data-value", value.Value);
                hyp.Attributes.Add("data-default", value.Default ? "Yes" : "No");
                hyp.Attributes.Add("data-defaulto", _subElement.Field.HasDefaultValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-other", value.Other ? "Yes" : "No");
                hyp.Attributes.Add("data-othero", _subElement.Field.HasOtherValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-unknown", value.Unknown ? "Yes" : "No");
                hyp.Attributes.Add("data-unknowno", _subElement.Field.HasUnknownValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-inuse", IsFieldValueInUse(value) ? "Yes" : "No");

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    Text = "Delete Value"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#valueModal");
                hyp.Attributes.Add("data-id", value.Id.ToString());
                hyp.Attributes.Add("data-evt", "delete");
                hyp.Attributes.Add("data-typ", _subElement.Field.FieldType.Description);
                hyp.Attributes.Add("data-value", value.Value);
                hyp.Attributes.Add("data-default", value.Default ? "Yes" : "No");
                hyp.Attributes.Add("data-defaulto", _subElement.Field.HasDefaultValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-other", value.Other ? "Yes" : "No");
                hyp.Attributes.Add("data-othero", _subElement.Field.HasOtherValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-unknown", value.Unknown ? "Yes" : "No");
                hyp.Attributes.Add("data-unknowno", _subElement.Field.HasUnknownValue(value) ? "Yes" : "No");
                hyp.Attributes.Add("data-inuse", IsFieldValueInUse(value) ? "Yes" : "No");

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                pnl.Controls.Add(ul);

                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                switch (_subElement.Field.FieldType.Description)
                {
                    case "DropDownList":
                        dt_1.Rows.Add(row);
                        break;

                    case "Listbox":
                        dt_2.Rows.Add(row);
                        break;

                    default:
                        break;
                }
            }
        }

        private void RenderTableValues()
        {
            TableRow row;
            TableCell cell;

            LinkButton sortbtn;

            HyperLink hyp;
            Label lbl;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_3.Rows)
            {
                if (temprow.Cells[0].Text != "Element")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete)
            {
                dt_3.Rows.Remove(temprow);
            }

            var i = 0;
            foreach (DatasetElementSub subElement in _element.DatasetElementSubs.Where(des => des.System == false).OrderBy(des => des.FieldOrder))
            {
                i++;
                row = new TableRow();

                cell = new TableCell();
                cell.Text = subElement.ElementName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = subElement.Field.FieldType.Description;
                row.Cells.Add(cell);

                // Sortable
                cell = new TableCell();
                if (i < _element.DatasetElementSubs.Where(des => des.System == false).Count())
                {
                    sortbtn = new LinkButton();
                    sortbtn.ID = "ed_" + subElement.Id;
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
                    sortbtn.ID = "eu_" + subElement.Id;
                    sortbtn.CssClass = _sortIconUp;
                    sortbtn.Click += btnSortElement_Click;
                    sortbtn.Style["color"] = "#3276B1";
                    sortbtn.Style["padding-left"] = "1px";
                    sortbtn.Style["padding-right"] = "1px";
                    cell.Controls.Add(sortbtn);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString() + "&sid=" + subElement.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Edit Element"
                };

                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                dt_3.Rows.Add(row);
            }
        }

        private void RenderCategories()
        {
            TableRow row;
            TableCell cell;

            if (_element.DatasetCategoryElements.Count > 0)
            {
                dt_4.Visible = true;
                foreach (DatasetCategoryElement category in _element.DatasetCategoryElements.OrderBy(dce => dce.DatasetCategory.Dataset.DatasetName).ThenBy(dce => dce.DatasetCategory.DatasetCategoryName))
                {
                    row = new TableRow();

                    cell = new TableCell();
                    cell.Text = category.DatasetCategory.Dataset.DatasetName;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Text = category.DatasetCategory.DatasetCategoryName;
                    row.Cells.Add(cell);

                    dt_4.Rows.Add(row);
                }
            }
            else
            {
                dt_4.Visible = false;
                divCategoryMessage.Visible = true;
            }
        }

        private void RenderElementGroupValues()
        {
            TableRow row;
            TableCell cell;

            var instanceValues = UnitOfWork.Repository<DatasetInstanceValue>().Queryable().Where(div => div.DatasetElement.Id == _element.Id).GroupBy(val => val.InstanceValue).Select(group => new { GroupValue = group.Key, Count = group.Count() }).OrderBy(x => x.GroupValue);
            if (instanceValues.Count() > 0)
            {
                dt_basic.Visible = true;
                foreach (var line in instanceValues)
                {
                    row = new TableRow();

                    cell = new TableCell();
                    cell.Text = line.GroupValue;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Text = line.Count.ToString();
                    row.Cells.Add(cell);

                    dt_basic.Rows.Add(row);
                }
            }
            else
            {
                dt_basic.Visible = false;
                divValueMessage.Visible = true;
            }
        }

        private void RenderSubElementGroupValues()
        {
            TableRow row;
            TableCell cell;

            var subValues = UnitOfWork.Repository<DatasetInstanceSubValue>().Queryable().Where(disv => disv.DatasetElementSub.Id == _subElement.Id);
            if (subValues.Count() > 0)
            {
                dt_basic.Visible = true;
                foreach (var line in subValues.GroupBy(val => val.InstanceValue).Select(group => new { GroupValue = group.Key, Count = group.Count() }).OrderBy(x => x.GroupValue))
                {
                    row = new TableRow();

                    cell = new TableCell();
                    cell.Text = line.GroupValue;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Text = line.Count.ToString();
                    row.Cells.Add(cell);

                    dt_basic.Rows.Add(row);
                }
            }
            else
            {
                dt_basic.Visible = false;
                divValueMessage.Visible = true;
            }
        }

        private void ToggleView()
        {
            switch (_formMode)
            {
                case FormMode.ViewMode:
                    txtDescription.Attributes.Add("readonly", "true");
                    txtDescription.Style.Add("background-color", "#EBEBE4");

                    txtFriendlyName.Attributes.Add("readonly", "true");
                    txtFriendlyName.Style.Add("background-color", "#EBEBE4");

                    txtHelp.Attributes.Add("readonly", "true");
                    txtHelp.Style.Add("background-color", "#EBEBE4");

                    txtOID.Attributes.Add("readonly", "true");
                    txtOID.Style.Add("background-color", "#EBEBE4");

                    txtDefaultValue.Attributes.Add("readonly", "true");
                    txtDefaultValue.Style.Add("background-color", "#EBEBE4");

                    ddlMandatory.Attributes.Add("readonly", "true");
                    ddlMandatory.Style.Add("background-color", "#EBEBE4");

                    ddlAnon.Attributes.Add("readonly", "true");
                    ddlAnon.Style.Add("background-color", "#EBEBE4");

                    ddlSystem.Attributes.Add("readonly", "true");
                    ddlSystem.Style.Add("background-color", "#EBEBE4");

                    ddlSingleDatasetRule.Attributes.Add("readonly", "true");
                    ddlSingleDatasetRule.Style.Add("background-color", "#EBEBE4");

                    ddlFieldType.Attributes.Add("readonly", "true");
                    ddlFieldType.Style.Add("background-color", "#EBEBE4");

                    txtMaxLength.Attributes.Add("readonly", "true");
                    txtMaxLength.Style.Add("background-color", "#EBEBE4");

                    txtMaximum.Attributes.Add("readonly", "true");
                    txtMaximum.Style.Add("background-color", "#EBEBE4");

                    txtMinimum.Attributes.Add("readonly", "true");
                    txtMinimum.Style.Add("background-color", "#EBEBE4");

                    txtDecimal.Attributes.Add("readonly", "true");
                    txtDecimal.Style.Add("background-color", "#EBEBE4");

                    liCategories.Visible = _formContext == FormContext.ElementContext ? true : false;
                    liValues.Visible = true;

                    if (_element.System)
                    {
                        divSystem.Style["display"] = "block";
                    }

                    break;

                case FormMode.EditMode:
                    wid_id_2.Visible = false;

                    ddlFieldType.Attributes.Add("readonly", "true");
                    ddlFieldType.Style.Add("background-color", "#EBEBE4");

                    liCategories.Visible = false;
                    liValues.Visible = false;

                    break;

                case FormMode.AddMode:
                    wid_id_2.Visible = false;

                    liCategories.Visible = false;
                    liValues.Visible = false;

                    break;

                default:
                    break;
            };
        }

        private void LoadDropDownList()
        {
            ListItem item;
            var fieldList = (from ft in UnitOfWork.Repository<FieldType>().Queryable() orderby ft.Description ascending select ft).ToList();

            item = new ListItem();
            item.Text = "-- UNDEFINED --";
            item.Value = "0";
            ddlFieldType.Items.Add(item);

            foreach (FieldType ft in fieldList)
            {
                if (_formContext == FormContext.SubElementContext && ft.Description == "Table")
                {
                    continue;
                }

                item = new ListItem();
                item.Text = ft.Description;
                item.Value = ft.Id.ToString();
                ddlFieldType.Items.Add(item);
            }

        }

        private void HideFieldTypePanels()
        {
            wid_id_2.Visible = false;

            divAlpha.Visible = false;
            divNumeric.Visible = false;
            divDropDownList.Visible = false;
            divListBox.Visible = false;
            divTable.Visible = false;
        }

        #endregion

        #region "Save Element"

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string err = "<ul>";

            // Validation
            if (_formMode == FormMode.AddMode)
            {
                if(ddlFieldType.SelectedItem.Value == "0") {
                    err += "<li>Please select a field type...</li>";
                }
                if (err != "<ul>")
                {
                    err += "</ul>";
                    divError.Visible = true;
                    spnErrors.InnerHtml = err;
                    return;
                }
            }

            if (_formContext == FormContext.ElementContext) {
                SaveElement();
            }
            else {
                SaveSubElement();
            }

            var url = String.Format("ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString());
            Response.Redirect(url);
        }

        private void SaveElement()
        {
            short temp;
            decimal tempd;

            if (_element == null)
            {
                // Prepare new element
                var elementType = GetDatasetElementType("Generic");
                var fieldType = GetFieldType(Convert.ToInt32(ddlFieldType.SelectedValue));
                var field = new Field()
                {
                    Anonymise = (ddlAnon.Value == "Yes"),
                    Mandatory = (ddlMandatory.Value == "Yes"),
                    FieldType = fieldType
                };
                var fieldTypeE = (FieldTypes)fieldType.Id;
                switch (fieldTypeE)
                {
                    case FieldTypes.AlphaNumericTextbox:
                        field.MaxLength = short.TryParse(txtMaxLength.Value, out temp) ? Convert.ToInt16(txtMaxLength.Value) : (short?)null;
                        break;

                    case FieldTypes.NumericTextbox:
                        field.Decimals = short.TryParse(txtDecimal.Value, out temp) ? Convert.ToInt16(txtDecimal.Value) : (short?)null;
                        field.MinSize = Decimal.TryParse(txtMinimum.Value, out tempd) ? Convert.ToDecimal(txtMinimum.Value) : (decimal?)null;
                        field.MaxSize = Decimal.TryParse(txtMaximum.Value, out tempd) ? Convert.ToDecimal(txtMaximum.Value) : (decimal?)null;
                        break;

                    default:
                        break;
                }

                _element = new DatasetElement { DatasetElementType = elementType, ElementName = txtDescription.Value, OID = txtOID.Value, DefaultValue = txtDefaultValue.Value, Field = field, System = (ddlSystem.Value == "Yes") };

                var rule = _element.GetRule(DatasetRuleType.ElementCanoOnlyLinkToSingleDataset);
                rule.RuleActive = (ddlSingleDatasetRule.SelectedValue == "Y");

                UnitOfWork.Repository<DatasetElement>().Save(_element);
            }
            else
            {
                // Prepare updated element
                _element.ElementName = txtDescription.Value;
                _element.OID = txtOID.Value;
                _element.DefaultValue = txtDefaultValue.Value;
                _element.System = (ddlSystem.Value == "Yes");
                _element.Field.Anonymise = (ddlAnon.Value == "Yes");
                _element.Field.Mandatory = (ddlMandatory.Value == "Yes");

                var fieldTypeE = (FieldTypes)_element.Field.FieldType.Id;
                switch (fieldTypeE)
                {
                    case FieldTypes.AlphaNumericTextbox:
                        _element.Field.MaxLength = short.TryParse(txtMaxLength.Value, out temp) ? Convert.ToInt16(txtMaxLength.Value) : (short?)null;
                        break;

                    case FieldTypes.NumericTextbox:
                        _element.Field.Decimals = short.TryParse(txtDecimal.Value, out temp) ? Convert.ToInt16(txtDecimal.Value) : (short?)null;
                        _element.Field.MinSize = Decimal.TryParse(txtMinimum.Value, out tempd) ? Convert.ToDecimal(txtMinimum.Value) : (decimal?)null;
                        _element.Field.MaxSize = Decimal.TryParse(txtMaximum.Value, out tempd) ? Convert.ToDecimal(txtMaximum.Value) : (decimal?)null;
                        break;

                    default:
                        break;
                }

                var rule = _element.GetRule(DatasetRuleType.ElementCanoOnlyLinkToSingleDataset);
                rule.RuleActive = (ddlSingleDatasetRule.SelectedValue == "Y");

                UnitOfWork.Repository<DatasetElement>().Update(_element);
            }

            if (_element.Field.Mandatory)
            {
                // Ensure element is linked to Required Information for each dataset it belongs to
                var datasets = _element.DatasetCategoryElements.Select(dce => dce.DatasetCategory.Dataset);
                foreach(Dataset dataset in datasets)
                {
                    // Ensure element is linked to Required Information category
                    var reqCat = UnitOfWork.Repository<DatasetCategory>().Queryable().SingleOrDefault(dc => dc.Dataset.Id == dataset.Id && dc.DatasetCategoryName == "Required Information");
                    if (reqCat != null)
                    {
                        var catEle = new DatasetCategoryElement() { Acute = false, Chronic = false, DatasetCategory = reqCat, DatasetElement = _element };
                        reqCat.DatasetCategoryElements.Add(catEle);

                        UnitOfWork.Repository<DatasetCategory>().Update(reqCat);
                    }
                }
            }
            else
            {
                // Ensure element is not linked to Required Information for each dataset it belongs to
                var elements = _element.DatasetCategoryElements.Where(dce => dce.DatasetCategory.DatasetCategoryName == "Required Information");
                ArrayList deleteCollection = new ArrayList();
                foreach (DatasetCategoryElement element in elements)
                {
                    deleteCollection.Add(element);
                }
                foreach (DatasetCategoryElement element in deleteCollection)
                {
                    _element.DatasetCategoryElements.Remove(element);
                    UnitOfWork.Repository<DatasetCategoryElement>().Delete(element);
                }
            }

            UnitOfWork.Complete();
        }

        private void SaveSubElement()
        {
            short temp;
            decimal tempd;

            if (_subElement == null)
            {
                // Prepare new element
                var fieldType = GetFieldType(Convert.ToInt32(ddlFieldType.SelectedValue));
                var field = new Field()
                {
                    Anonymise = (ddlAnon.Value == "Yes"),
                    Mandatory = (ddlMandatory.Value == "Yes"),
                    FieldType = fieldType
                };
                var fieldTypeE = (FieldTypes)fieldType.Id;
                switch (fieldTypeE)
                {
                    case FieldTypes.AlphaNumericTextbox:
                        field.MaxLength = short.TryParse(txtMaxLength.Value, out temp) ? Convert.ToInt16(txtMaxLength.Value) : (short?)null;
                        break;

                    case FieldTypes.NumericTextbox:
                        field.Decimals = short.TryParse(txtDecimal.Value, out temp) ? Convert.ToInt16(txtDecimal.Value) : (short?)null;
                        field.MinSize = Decimal.TryParse(txtMinimum.Value, out tempd) ? Convert.ToDecimal(txtMinimum.Value) : (decimal?)null;
                        field.MaxSize = Decimal.TryParse(txtMaximum.Value, out tempd) ? Convert.ToDecimal(txtMaximum.Value) : (decimal?)null;
                        break;

                    default:
                        break;
                }

                _subElement = new DatasetElementSub { DatasetElement = _element, ElementName = txtDescription.Value, FriendlyName = txtFriendlyName.Value, Help = txtHelp.Value, OID = txtOID.Value, DefaultValue = txtDefaultValue.Value, Field = field, FieldOrder = 1, System = (ddlSystem.Value == "Yes") };
                UnitOfWork.Repository<DatasetElementSub>().Save(_subElement);
            }
            else
            {
                // Prepare updated element
                _subElement.ElementName = txtDescription.Value;
                _subElement.FriendlyName = txtFriendlyName.Value;
                _subElement.Help = txtHelp.Value;
                _subElement.OID = txtOID.Value;
                _subElement.DefaultValue = txtDefaultValue.Value;
                _subElement.System = (ddlSystem.Value == "Yes");
                _subElement.Field.Anonymise = (ddlAnon.Value == "Yes");
                _subElement.Field.Mandatory = (ddlMandatory.Value == "Yes");

                var fieldTypeE = (FieldTypes)_subElement.Field.FieldType.Id;
                switch (fieldTypeE)
                {
                    case FieldTypes.AlphaNumericTextbox:
                        _subElement.Field.MaxLength = short.TryParse(txtMaxLength.Value, out temp) ? Convert.ToInt16(txtMaxLength.Value) : (short?)null;
                        break;

                    case FieldTypes.NumericTextbox:
                        _subElement.Field.Decimals = short.TryParse(txtDecimal.Value, out temp) ? Convert.ToInt16(txtDecimal.Value) : (short?)null;
                        _subElement.Field.MinSize = Decimal.TryParse(txtMinimum.Value, out tempd) ? Convert.ToDecimal(txtMinimum.Value) : (decimal?)null;
                        _subElement.Field.MaxSize = Decimal.TryParse(txtMaximum.Value, out tempd) ? Convert.ToDecimal(txtMaximum.Value) : (decimal?)null;
                        break;

                    default:
                        break;
                }

                UnitOfWork.Repository<DatasetElementSub>().Update(_subElement);
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
                var sub = UnitOfWork.Repository<DatasetElementSub>().Queryable().Single(des => des.Id == id);
                if (action == "eu")
                {
                    var prevSub = UnitOfWork.Repository<DatasetElementSub>().Queryable().OrderByDescending(des => des.FieldOrder).First(des => des.DatasetElement.Id == sub.DatasetElement.Id && des.FieldOrder < sub.FieldOrder);
                    sub.FieldOrder -= 1;
                    prevSub.FieldOrder += 1;

                    UnitOfWork.Repository<DatasetElementSub>().Update(sub);
                    UnitOfWork.Repository<DatasetElementSub>().Update(prevSub);
                }
                if (action == "ed")
                {
                    var nextSub = UnitOfWork.Repository<DatasetElementSub>().Queryable().OrderBy(des => des.FieldOrder).First(des => des.DatasetElement.Id == sub.DatasetElement.Id && des.FieldOrder > sub.FieldOrder);
                    sub.FieldOrder += 1;
                    nextSub.FieldOrder -= 1;

                    UnitOfWork.Repository<DatasetElementSub>().Update(sub);
                    UnitOfWork.Repository<DatasetElementSub>().Update(nextSub);
                }

                HttpCookie cookie = new HttpCookie("PopUpMessage");
                cookie.Value = "Element order set successfully";
                Response.Cookies.Add(cookie);
                Master.ShouldPopUpBeDisplayed();

                RenderTableValues();
            }
        }

        #endregion

        #region "Delete Element"
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (_formContext == FormContext.ElementContext) {
                DeleteElement();
            }
            else {
                DeleteSubElement();
            }
        }

        private void DeleteElement()
        {
            string err = "<ul>";

            // Validation
            if (_element.DatasetCategoryElements.Count > 0) 
            {
                err += "<li>Unable to delete...element is linked to categories</li>";
            }
            if (UnitOfWork.Repository<DatasetInstanceValue>().Queryable().Where(div => div.DatasetElement.Id == _element.Id).Any()) 
            {
                err += "<li>Unable to delete...element is linked to values</li>";
            }
            if (err != "<ul>")
            {
                err += "</ul>";
                divError.Visible = true;
                spnErrors.InnerHtml = err;
                return;
            }

            UnitOfWork.Repository<DatasetElement>().Delete(_element);
            UnitOfWork.Complete();

            var url = String.Format("ManageDatasetElement.aspx");
            Response.Redirect(url);
        }

        private void DeleteSubElement()
        {
            string err = "<ul>";

            // Validation

            if (UnitOfWork.Repository<DatasetInstanceSubValue>().Queryable().Where(disv => disv.DatasetElementSub.Id == _subElement.Id).Any()) 
            {
                err += "<li>Unable to delete...element is linked to existing values</li>";
            }
            if (err != "<ul>")
            {
                err += "</ul>";
                divError.Visible = true;
                spnErrors.InnerHtml = err;
                return;
            }

            UnitOfWork.Repository<DatasetElementSub>().Delete(_subElement);
            UnitOfWork.Complete();

            var url = String.Format("ManageDatasetElementEdit.aspx?id=" + _element.Id.ToString());
            Response.Redirect(url);
        }

        #endregion

        #region "Save Value"
        protected void btnSaveValue_Click(object sender, EventArgs e)
        {
            if (_formContext == FormContext.ElementContext) 
            {
                SaveElementValue();
            }
            else 
            {
                SaveSubElementValue();
            }
        }

        private void SaveElementValue()
        {
            FieldValue value = null;
            Boolean def;
            string err = "<ul>";

            if (txtValueUID.Value == "0")
            {
                def = (ddlValueDefault.SelectedValue == "Yes");
                if (_element.Field.FieldType.Description == "Listbox") 
                {
                    def = false;
                }

                value = new FieldValue { Field = _element.Field, Value = txtValue.Value, Default = def, Other = (ddlValueOther.SelectedValue == "Yes"), Unknown = (ddlValueUnknown.SelectedValue == "Yes") };
                UnitOfWork.Repository<FieldValue>().Save(value);
            }
            else
            {
                value = GetFieldValue(Convert.ToInt32(txtValueUID.Value));

                def = (ddlValueDefault.SelectedValue == "Yes");
                if (_element.Field.FieldType.Description == "Listbox") 
                {
                    def = false;
                }

                if (value != null)
                {
                    value.Default = def;
                    value.Other = (ddlValueOther.SelectedValue == "Yes");
                    value.Unknown = (ddlValueUnknown.SelectedValue == "Yes");

                    UnitOfWork.Repository<FieldValue>().Update(value);
                }
            }

            UnitOfWork.Complete();

            RenderListValues();
        }

        private void SaveSubElementValue()
        {
            FieldValue value = null;
            Boolean def;
            string err = "<ul>";

            if (txtValueUID.Value == "0")
            {
                def = (ddlValueDefault.SelectedValue == "Yes");
                if(_subElement.Field.FieldType.Description == "Listbox") {
                    def = false;
                }

                value = new FieldValue { Field = _subElement.Field, Value = txtValue.Value, Default = def, Other = (ddlValueOther.SelectedValue == "Yes"), Unknown = (ddlValueUnknown.SelectedValue == "Yes") };
                UnitOfWork.Repository<FieldValue>().Save(value);
            }
            else
            {
                value = GetFieldValue(Convert.ToInt32(txtValueUID.Value));

                def = (ddlValueDefault.SelectedValue == "Yes");
                if (_subElement.Field.FieldType.Description == "Listbox") {
                    def = false;
                }

                if (value != null)
                {
                    value.Default = def;
                    value.Other = (ddlValueOther.SelectedValue == "Yes");
                    value.Unknown = (ddlValueUnknown.SelectedValue == "Yes");

                    UnitOfWork.Repository<FieldValue>().Update(value);
                }
            }

            UnitOfWork.Complete();

            RenderSubListValues();
        }
        #endregion

        protected void btnDeleteValue_Click(object sender, EventArgs e)
        {
            FieldValue value = null;

            if (txtValueUID.Value != "0")
            {
                value = GetFieldValue(Convert.ToInt32(txtValueUID.Value));
            }

            if (value != null)
            {
                UnitOfWork.Repository<FieldValue>().Delete(value);
                UnitOfWork.Complete();

                RenderListValues();
            }
        }

        protected void SelectedIndexChanged(object sender, EventArgs e)
        {
            HideFieldTypePanels();

            switch (ddlFieldType.SelectedItem.Text)
            {
                case "-- UNDEFINED --":
                    break;

                case "AlphaNumericTextbox":
                    divAlpha.Visible = true;
                    break;

                case "NumericTextbox":
                    divNumeric.Visible = true;
                    break;

                default:
                    break;
            }
        }

        #region "EF"
        private DatasetElement GetDatasetElement(int id)
        {
            return UnitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.Id == id);
        }

        private DatasetElementSub GetDatasetElementSub(int sid)
        {
            return UnitOfWork.Repository<DatasetElementSub>().Queryable().SingleOrDefault(u => u.Id == sid);
        }

        private DatasetElementType GetDatasetElementType(string desc)
        {
            return UnitOfWork.Repository<DatasetElementType>().Queryable().SingleOrDefault(u => u.Description == desc);
        }

        private FieldType GetFieldType(int id)
        {
            return UnitOfWork.Repository<FieldType>().Queryable().SingleOrDefault(u => u.Id == id);
        }

        private FieldValue GetFieldValue(int id)
        {
            return UnitOfWork.Repository<FieldValue>().Queryable().SingleOrDefault(u => u.Id == id);
        }

        private bool IsFieldValueInUse(FieldValue value)
        {
            if (_formContext == FormContext.ElementContext)
            {
                return (UnitOfWork.Repository<DatasetInstanceValue>().Queryable().Count(u => u.DatasetElement.Id == _element.Id && u.InstanceValue == value.Value) > 0);
            }
            else
            {
                return (UnitOfWork.Repository<DatasetInstanceSubValue>().Queryable().Count(u => u.DatasetElementSub.Id == _subElement.Id && u.InstanceValue == value.Value) > 0);
            }
        }
        #endregion

    }
}