using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

using System.Web;
using System.Web.Security.AntiXss;

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class PageCustom : MainPageBase
    {
        private int _id;
        private MetaPage _metaPage;

        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3 };
        private FormMode _formMode = FormMode.EditMode;

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Manage Page", SubTitle = "", Icon = "fa fa-windows fa-fw" });

            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0)
                {
                    _metaPage = UnitOfWork.Repository<MetaPage>().Queryable().SingleOrDefault(u => u.Id == _id);
                }
                else
                {
                    _metaPage = null;
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
                Master.MainMenu.SetActive("PublishAdmin");

                if (_metaPage != null)
                {
                    RenderPage();
                }
            };
            RenderGrids();
        }

        #region "Rendering"

        private void RenderPage()
        {
            txtUID.Value = _metaPage.metapage_guid.ToString();
            txtName.Value = _metaPage.PageName;
            txtDefinition.Value = _metaPage.PageDefinition;
            txtBreadcrumb.Value = _metaPage.Breadcrumb;
            ddlVisible.Value = _metaPage.IsVisible ? "Yes" : "No";
        }

        private void RenderGrids()
        {
            RenderWidgets();
        }

        private void RenderWidgets()
        {
            if (_metaPage == null) { return; };

            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_1.Rows)
            {
                if (temprow.Cells[0].Text != "Widget Name")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) 
            {
                dt_1.Rows.Remove(temprow);
            }

            var i = 0;
            var max = _metaPage.Widgets.Count();
            foreach (var widget in _metaPage.Widgets.OrderBy(c => c.WidgetLocation))
            {
                i++;
                row = new TableRow();

                cell = new TableCell();
                cell.Text = widget.WidgetName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = widget.WidgetDefinition;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = widget.WidgetType.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = widget.WidgetLocation.ToString();
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
                    NavigateUrl = "PageCustomWidget.aspx?Id=" + widget.Id,
                    Text = "Edit Widget"
                };

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "PageCustomWidget?Id=" + widget.Id,
                    Text = "Delete Widget"
                };
                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                pnl.Controls.Add(ul);
                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                dt_1.Rows.Add(row);
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
                        NavigateUrl = "PageCustom.aspx?id=" + _metaPage.Id.ToString() + "&a=edit",
                        CssClass = "btn btn-primary",
                        Text = "Edit Page"
                    };
                    spnButtons.Controls.Add(hyp);

                    hyp = new HyperLink()
                    {
                        ID = "btnReturn",
                        NavigateUrl = "PageList.aspx",
                        CssClass = "btn btn-default",
                        Text = "Return"
                    };
                    spnButtons.Controls.Add(hyp);

                    hyp = new HyperLink()
                    {
                        ID = "btnAdd",
                        NavigateUrl = "PageCustomWidget.aspx?pid=" + _metaPage.Id.ToString() + "&id=0",
                        CssClass = "btn btn-default",
                        Text = "Add Widget"
                    };
                    spnWidgetButtons.Controls.Add(hyp);

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
                        NavigateUrl = "PageViewer.aspx?guid=" + _metaPage.metapage_guid.ToString(),
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
                        NavigateUrl = "PageViewer.aspx?guid=a63e9f29-a22f-43df-87a0-d0c8dec50548",
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

                    txtDefinition.Attributes.Add("readonly", "true");
                    txtDefinition.Style.Add("background-color", "#EBEBE4");

                    txtBreadcrumb.Attributes.Add("readonly", "true");
                    txtBreadcrumb.Style.Add("background-color", "#EBEBE4");

                    ddlVisible.Attributes.Add("readonly", "true");
                    ddlVisible.Style.Add("background-color", "#EBEBE4");

                    divWidget.Style["display"] = "block";

                    break;

                case FormMode.EditMode:

                    divWidget.Style["display"] = "none";

                    break;

                case FormMode.AddMode:

                    divUnique.Visible = false;
                    divWidget.Style["display"] = "none";

                    break;

                default:
                    break;
            };
        }

        #endregion

        #region "Save"
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblName.Attributes.Remove("class");
            lblName.Attributes.Add("class", "input");

            var err = false;

            if (String.IsNullOrWhiteSpace(txtName.Value))
            {
                lblName.Attributes.Remove("class");
                lblName.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Page Name is required";
                lblName.Controls.Add(errorMessageDiv);

                err = true;
            }
            else
            {
                var pageName = txtName.Value.Trim();
                if (Regex.Matches(pageName, @"[a-zA-Z ']").Count < pageName.Length)
                {
                    lblName.Attributes.Remove("class");
                    lblName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Page Name contains invalid characters (Enter A-Z, a-z, space)";
                    lblName.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }

            if(err) { return; };

            SavePage();

            var url = String.Format("PageCustom.aspx?id=" + _metaPage.Id.ToString());
            Response.Redirect(url);
        }

        private void SavePage()
        {
            var encodedName = AntiXssEncoder.HtmlEncode(txtName.Value, false);
            var encodedDefinition = AntiXssEncoder.HtmlEncode(txtDefinition.Value, false);
            var encodedBreadcrumb = AntiXssEncoder.HtmlEncode(txtBreadcrumb.Value, false);

            if (_metaPage == null)
            {
                // Prepare new page
                _metaPage = new MetaPage { Breadcrumb = encodedBreadcrumb, IsSystem = false, MetaDefinition = "", PageDefinition = encodedDefinition, PageName = encodedName, metapage_guid = Guid.NewGuid(), IsVisible = (ddlVisible.Value == "Yes") };

                UnitOfWork.Repository<MetaPage>().Save(_metaPage);
            }
            else
            {
                // Prepare updated page
                _metaPage.PageName = encodedName;
                _metaPage.PageDefinition = encodedDefinition;
                _metaPage.Breadcrumb = encodedBreadcrumb;
                _metaPage.IsVisible = (ddlVisible.Value == "Yes");

                UnitOfWork.Repository<MetaPage>().Update(_metaPage);
            }

            UnitOfWork.Complete();

            HttpCookie cookie = new HttpCookie("PopUpMessage");
            cookie.Value = String.Format("Page {0} successfully", _formMode == FormMode.AddMode ? "added" : "updated");
            Response.Cookies.Add(cookie);

            Response.Redirect("PageViewer.aspx?guid=" + _metaPage.metapage_guid.ToString());

        }

        #endregion

        #region "EF"

        private bool IsPageUnique(string pageName, int id)
        {
            int count = 0;
            if (id > 0)
            {
                count = UnitOfWork.Repository<MetaPage>().Queryable().Where(mp => mp.PageName == pageName && mp.Id != id).Count();
            }
            else
            {
                count = UnitOfWork.Repository<MetaPage>().Queryable().Where(mp => mp.PageName == pageName).Count();
            }
            return (count == 0);
        }

        #endregion
    }
}
