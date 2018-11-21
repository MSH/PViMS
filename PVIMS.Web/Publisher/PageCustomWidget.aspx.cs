using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security.AntiXss;

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using System.Xml;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Web
{
    public partial class PageCustomWidget : MainPageBase
    {
        private int _id;
        private MetaWidget _metaWidget;

        private int _pid;
        private MetaPage _metaPage;

        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3};
        private FormMode _formMode = FormMode.EditMode;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0) {
                    _metaWidget = UnitOfWork.Repository<MetaWidget>().Get(_id);

                    string action;
                    if (Request.QueryString["action"] != null)
                    {
                        action = Request.QueryString["action"];
                        switch (action)
                        {
                            case "edit":
                                _formMode = FormMode.EditMode;
                                Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Edit Widget", SubTitle = "", Icon = "fa fa-windows fa-fw" });

                                break;

                        } // switch (_action)
                    }
                    else
                    {
                        throw new Exception("action not passed as parameter");
                    }
                }
                else
                {
                    _metaWidget = null;
                    _formMode = FormMode.AddMode;
                    Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Add New Widget", SubTitle = "", Icon = "fa fa-windows fa-fw" });

                    if (Request.QueryString["pid"] != null)
                    {
                        _pid = Convert.ToInt32(Request.QueryString["pid"]);
                        if (_pid > 0)
                        {
                            _metaPage = UnitOfWork.Repository<MetaPage>().Get(_pid);
                        }
                        else
                        {
                            throw new Exception("pid not passed as parameter");
                        }
                    }
                }
            }
            else
            {
                throw new Exception("id not passed as parameter");
            }

            LoadDropDownList();
            RenderButtons();
            RenderPage();
            if (_metaWidget != null)
            {
                RenderWidget();
            }
            ToggleView();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Master.SetMenuActive("PublishAdmin");
            };
        }

        #region "Rendering"

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
                        NavigateUrl = "PageCustomWidget.aspx?id=" + _metaWidget.Id.ToString() + "&a=edit",
                        CssClass = "btn btn-primary",
                        Text = "Edit Widget"
                    };
                    spnButtons.Controls.Add(hyp);

                    hyp = new HyperLink()
                    {
                        ID = "btnReturn",
                        NavigateUrl = "PageViewer.aspx?guid=" + ((_metaPage == null) ? _metaWidget.MetaPage.metapage_guid.ToString() : _metaPage.metapage_guid.ToString()),
                        CssClass = "btn btn-default",
                        Text = "Return"
                    };
                    spnButtons.Controls.Add(hyp);

                    break;

                case FormMode.AddMode:
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
                        NavigateUrl = "PageViewer.aspx?guid=" + ((_metaPage == null) ? _metaWidget.MetaPage.metapage_guid.ToString() : _metaPage.metapage_guid.ToString()),
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

        private void RenderPage()
        {
            txtPageID.Value = _metaPage == null ? _metaWidget.MetaPage.Id.ToString() : _metaPage.Id.ToString();
            txtPageName.Value = _metaPage == null ? _metaWidget.MetaPage.PageName.ToString() : _metaPage.PageName.ToString();

            HideWidgetTypePanels();
        }

        private void RenderWidget()
        {
            txtUID.Value = _metaWidget.metawidget_guid.ToString();
            txtName.Value = _metaWidget.WidgetName;
            txtDefinition.Value = _metaWidget.WidgetDefinition;
            ddlWidgetType.SelectedValue = _metaWidget.WidgetType.Id.ToString();
            ddlWidgetStatus.SelectedValue = Convert.ToInt32(_metaWidget.WidgetStatus).ToString();
            txtWidgetLocation.Value = _metaWidget.WidgetLocation.ToString();
            ddlWidgetLocation.SelectedValue = _metaWidget.WidgetLocation.ToString();
            txtIcon.Value = _metaWidget.Icon;

            HideWidgetTypePanels();

            var widgetType = UnitOfWork.Repository<MetaWidgetType>().Get(Convert.ToInt32(ddlWidgetType.SelectedValue));
            var widgetTypeE = (MetaWidgetTypes)widgetType.Id;

            switch (widgetTypeE)
            {
                case MetaWidgetTypes.General:
                    divContentGeneral.Visible = true;
                    CKEditor1.Text = _metaWidget.Content;

                    break;

                case MetaWidgetTypes.SubItem:
                    divContentWiki.Visible = true;

                    var tabsWiki = PrepareTabsForWidget();
                    spnWikiList.Controls.Add(tabsWiki);

                    var tabsWikiDiv = new HtmlGenericControl("div");
                    tabsWikiDiv.Attributes.Add("class", "tab-content");

                    // Create the XmlDocument
                    XmlDocument wdoc = new XmlDocument();
                    wdoc.LoadXml(_metaWidget.Content);
                    XmlNode wroot = wdoc.DocumentElement;

                    // Loop through each listitem
                    var witem = 0;
                    foreach (XmlNode node in wroot.ChildNodes)
                    {
                        witem += 1;

                        tabsWikiDiv.Controls.Add(PreparePaneForWikiItem(witem, node, false));
                    }
                    witem += 1;
                    for (int i = witem; i <= 15; i++)
                    {
                        tabsWikiDiv.Controls.Add(PreparePaneForWikiItem(i, null, true));
                    }
                    spnWikiList.Controls.Add(tabsWikiDiv);

                    break;

                case MetaWidgetTypes.ItemList:
                    divContentItemList.Visible = true;

                    var tabsItems = PrepareTabsForWidget();
                    spnItemList.Controls.Add(tabsItems);

                    var tabsItemDiv = new HtmlGenericControl("div");
                    tabsItemDiv.Attributes.Add("class", "tab-content");

                    // Create the XmlDocument
                    XmlDocument idoc = new XmlDocument();
                    idoc.LoadXml(_metaWidget.Content);
                    XmlNode iroot = idoc.DocumentElement;

                    // Loop through each listitem
                    var iitem = 0;
                    foreach (XmlNode node in iroot.ChildNodes)
                    {
                        iitem += 1;

                        tabsItemDiv.Controls.Add(PreparePaneForContentItem(iitem, node, false));
                    }
                    iitem += 1;
                    for (int i = iitem; i <= 15; i++)
                    {
                        tabsItemDiv.Controls.Add(PreparePaneForContentItem(i, null, true));
                    }
                    spnItemList.Controls.Add(tabsItemDiv);

                    break;

                default:
                    break;
            }
        }

        private HtmlGenericControl PrepareTabsForWidget()
        {
            var tabUl = new HtmlGenericControl("ul");
            tabUl.Attributes.Add("class", "nav nav-tabs tabs-left bordered");

            var tabLi = new HtmlGenericControl("li");

            // Create the XmlDocument
            XmlDocument wdoc = new XmlDocument();
            wdoc.LoadXml(_metaWidget.Content);
            XmlNode wroot = wdoc.DocumentElement;

            // Loop through each listitem
            var witem = 0;
            foreach (XmlNode node in wroot.ChildNodes)
            {
                witem += 1;

                tabLi = new HtmlGenericControl("li");
                tabLi.ID = "li-" + witem.ToString();
                if(witem == 1)
                {
                    tabLi.Attributes.Add("class", "active");
                }
                tabLi.InnerHtml = String.Format(@"<a href=""#tab-{0}"" data-toggle=""tab""> Item {0} </a>", witem.ToString());
                tabUl.Controls.Add(tabLi);
            }
            witem += 1;
            for (int i = witem; i <= 15; i++)
            {
                tabLi = new HtmlGenericControl("li");
                tabLi.ID = "li-" + i.ToString();
                if (witem == 1)
                {
                    tabLi.Attributes.Add("class", "active");
                }
                tabLi.Style["display"] = "none";
                tabLi.InnerHtml = String.Format(@"<a href=""#tab-{0}"" data-toggle=""tab""> Item {0} </a>", i.ToString());
                tabUl.Controls.Add(tabLi);
            }

            return tabUl;
        }

        private HtmlGenericControl PreparePaneForWikiItem(int witem, XmlNode node, bool isBlank)
        {
            var tabPaneDiv = new HtmlGenericControl("div");
            tabPaneDiv.Attributes.Add("class", "tab-pane smart-form " + ((witem == 1) ? "active" : ""));
            tabPaneDiv.ID = "tab-" + witem.ToString();

            var title = isBlank ? "" : node.ChildNodes[0].InnerText;
            var subTitle = isBlank ? "" : node.ChildNodes[1].InnerText;
            var contentPage = isBlank ? "" : node.ChildNodes[2].InnerText;
            var modified = isBlank ? "" : node.ChildNodes[3] != null ? node.ChildNodes[3].InnerText : DateTime.Today.ToString("yyyy-MM-dd");

            var fieldset = new HtmlGenericControl("fieldset");

            List<ListItem> items = new List<ListItem>();
            //items.Add(new ListItem() { Value = "", Text = "-- PLEASE SELECT A PAGE --" });
            foreach (var metaPage in UnitOfWork.Repository<MetaPage>().Queryable().Where(mp => mp.Id != _metaWidget.MetaPage.Id).OrderByDescending(mp => mp.Id))
            {
                items.Add(new ListItem() { Value = metaPage.Id.ToString(), Text = metaPage.PageName, Selected = (contentPage == metaPage.Id.ToString()) });
            }

            fieldset.Controls.Add(PrepareRowValueForDate("col col-lg-3", "Modified Date", witem, 10, modified));
            fieldset.Controls.Add(PrepareRowValueForInputText("col col-lg-6", "Title", witem, 100, title));
            fieldset.Controls.Add(PrepareRowValueForInputText("col col-lg-9", "SubTitle", witem, 250, subTitle));
            fieldset.Controls.Add(PrepareRowValueForSelect("col col-lg-4", "PageContent", witem, items.ToArray()));
            fieldset.Controls.Add(PrepareRowForButtons(witem));

            tabPaneDiv.Controls.Add(fieldset);
            return tabPaneDiv;
        }

        private HtmlGenericControl PreparePaneForContentItem(int iitem, XmlNode node, bool isBlank)
        {
            var tabPaneDiv = new HtmlGenericControl("div");
            tabPaneDiv.Attributes.Add("class", "tab-pane smart-form " + ((iitem == 1) ? "active" : ""));
            tabPaneDiv.ID = "tab-" + iitem.ToString();

            var title = isBlank ? "" : node.ChildNodes[0].InnerText;
            var content = isBlank ? "" : node.ChildNodes[1].InnerText;

            var fieldset = new HtmlGenericControl("fieldset");

            fieldset.Controls.Add(PrepareRowValueForInputText("col col-lg-6", "Title", iitem, 100, title));
            fieldset.Controls.Add(PrepareRowValueForInputTextArea("col col-lg-12", "Content", iitem, content));
            fieldset.Controls.Add(PrepareRowForButtons(iitem));

            tabPaneDiv.Controls.Add(fieldset);
            return tabPaneDiv;
        }

        private HtmlGenericControl PrepareRowValueForDate(string sectionClass, string rowName, int witem, int maxLength, string value)
        {
            var rowDiv = new HtmlGenericControl("div");
            rowDiv.Attributes.Add("class", "row");
            var section = new HtmlGenericControl("section");
            section.Attributes.Add("class", sectionClass);

            var label = new HtmlGenericControl("label");
            label.Attributes.Add("class", "input");
            label.InnerText = rowName;

            var inputText = new HtmlInputText() { Name = rowName.ToLower() + "-" + witem.ToString(), ID = rowName.ToLower() + "-" + witem.ToString(), MaxLength = maxLength, Value = value };
            inputText.Disabled = true;
            inputText.Style.Add("background-color", "#EBEBE4");
            inputText.Attributes.Add("class", "form-control");
            label.Controls.Add(inputText);

            section.Controls.Add(label);
            rowDiv.Controls.Add(section);

            return rowDiv;
        }

        private HtmlGenericControl PrepareRowValueForInputText(string sectionClass, string rowName, int witem, int maxLength, string value)
        {
            var rowDiv = new HtmlGenericControl("div");
            rowDiv.Attributes.Add("class", "row");
            var section = new HtmlGenericControl("section");
            section.Attributes.Add("class", sectionClass);

            var label = new HtmlGenericControl("label");
            label.Attributes.Add("class", "input");
            label.InnerText = rowName;

            var inputText = new HtmlInputText() { Name = rowName.ToLower() + "-" + witem.ToString(), ID = rowName.ToLower() + "-" + witem.ToString(), MaxLength = maxLength, Value = value };
            if (_formMode == FormMode.ViewMode)
            {
                inputText.Disabled = true;
                inputText.Style.Add("background-color", "#EBEBE4");
            }
            inputText.Attributes.Add("class", "form-control");
            label.Controls.Add(inputText);

            section.Controls.Add(label);
            rowDiv.Controls.Add(section);

            return rowDiv;
        }

        private HtmlGenericControl PrepareRowValueForInputTextArea(string sectionClass, string rowName, int witem, string value)
        {
            var rowDiv = new HtmlGenericControl("div");
            rowDiv.Attributes.Add("class", "row");
            var section = new HtmlGenericControl("section");
            section.Attributes.Add("class", sectionClass);

            var label = new HtmlGenericControl("label");
            label.Attributes.Add("class", "input");
            label.InnerText = rowName;

            var textArea = new HtmlTextArea() { Name = rowName.ToLower() + "-" + witem.ToString(), ID = rowName.ToLower() + "-" + witem.ToString(), Rows = 15 };
            textArea.Attributes.Add("class", "form-control");
            textArea.InnerHtml = value;
            if (_formMode == FormMode.ViewMode)
            {
                textArea.Disabled = true;
                textArea.Style.Add("background-color", "#EBEBE4");
            }
            label.Controls.Add(textArea);

            section.Controls.Add(label);
            rowDiv.Controls.Add(section);

            return rowDiv;
        }

        private HtmlGenericControl PrepareRowValueForSelect(string sectionClass, string rowName, int witem, ListItem[] items)
        {
            var rowDiv = new HtmlGenericControl("div");
            rowDiv.Attributes.Add("class", "row");
            var section = new HtmlGenericControl("section");
            section.Attributes.Add("class", sectionClass);

            var label = new HtmlGenericControl("label");
            label.Attributes.Add("class", "input");
            label.InnerText = rowName;

            var select = new HtmlSelect() { Name = rowName.ToLower() + "-" + witem.ToString(), ID = rowName.ToLower() + "-" + witem.ToString() };
            select.Attributes.Add("class", "form-control");
            select.Items.AddRange(items);
            if (_formMode == FormMode.ViewMode)
            {
                select.Disabled = true;
                select.Style.Add("background-color", "#EBEBE4");
            }
            label.Controls.Add(select);

            var button = new HtmlButton() { InnerText = "Add New Page" };
            button.Attributes.Add("class", "btn btn-secondary btn-sm btn-add-page");
            label.Controls.Add(button);

            section.Controls.Add(label);
            rowDiv.Controls.Add(section);

            return rowDiv;
        }

        private HtmlGenericControl PrepareRowForButtons(int witem)
        {
            var footer = new HtmlGenericControl("footer");
            var button = new HtmlButton() { InnerText = "Add New Item", ID = "add-" + witem.ToString() };
            button.Attributes.Add("class", "btn btn-default btn-sm btn-add");
            footer.Controls.Add(button);
            button = new HtmlButton() { InnerText = "Remove Last Item", ID = "remove-" + witem.ToString() };
            button.Attributes.Add("class", "btn btn-default btn-sm btn-remove");
            footer.Controls.Add(button);

            return footer;
        }

        private void ToggleView()
        {
            switch (_formMode)
            {
                case FormMode.AddMode:
                    divWikiContent.Visible = false;

                    divWidgetLocation.Visible = _metaWidget != null ? _metaWidget.WidgetStatus == MetaWidgetStatus.Published : false;

                    break;

                case FormMode.ViewMode:
                    divMainContent.Visible = false;
                    divWikiContent.Visible = true;

                    CKEditor1.Attributes.Add("readonly", "true");
                    CKEditor1.Style.Add("background-color", "#EBEBE4");

                    divWidgetLocation.Visible = _metaWidget != null ? _metaWidget.WidgetStatus == MetaWidgetStatus.Published : false;

                    break;

                case FormMode.EditMode:
                    divMainContent.Visible = true;
                    divWikiContent.Visible = true;

                    ddlWidgetType.Attributes.Add("readonly", "true");
                    ddlWidgetType.Style.Add("background-color", "#EBEBE4");

                    divWidgetLocation.Visible = _metaWidget != null ? _metaWidget.WidgetStatus == MetaWidgetStatus.Published : false;

                    break;

                default:
                    break;
            };
        }

        private void LoadDropDownList()
        {
            ListItem item;
            var typeList = (from wt in UnitOfWork.Repository<MetaWidgetType>().Queryable() orderby wt.Description ascending select wt).ToList();

            ddlWidgetType.Items.Add(new ListItem() { Text = "-- UNDEFINED --", Value = "0" });
            foreach (MetaWidgetType wt in typeList)
            {
                item = new ListItem();
                item.Text = wt.Description;
                item.Value = wt.Id.ToString();
                ddlWidgetType.Items.Add(item);
            }

            var metaPage = (_metaPage == null) ? _metaWidget.MetaPage : _metaPage;
            ddlWidgetLocation.Items.Add(new ListItem() { Text = "-- No change --", Value = "0" });
            if (!metaPage.Widgets.Any(w => w.WidgetLocation == MetaWidgetLocation.TopLeft)) { ddlWidgetLocation.Items.Add(new ListItem() { Text = "Top Left", Value = "1" }); };
            if (!metaPage.Widgets.Any(w => w.WidgetLocation == MetaWidgetLocation.TopRight)) { ddlWidgetLocation.Items.Add(new ListItem() { Text = "Top Right", Value = "2" }); };
            if (!metaPage.Widgets.Any(w => w.WidgetLocation == MetaWidgetLocation.MiddleLeft)) { ddlWidgetLocation.Items.Add(new ListItem() { Text = "Middle Left", Value = "3" }); };
            if (!metaPage.Widgets.Any(w => w.WidgetLocation == MetaWidgetLocation.MiddleRight)) { ddlWidgetLocation.Items.Add(new ListItem() { Text = "Middle Right", Value = "4" }); };
            if (!metaPage.Widgets.Any(w => w.WidgetLocation == MetaWidgetLocation.BottomLeft)) { ddlWidgetLocation.Items.Add(new ListItem() { Text = "Bottom Left", Value = "5" }); };
            if (!metaPage.Widgets.Any(w => w.WidgetLocation == MetaWidgetLocation.BottomRight)) { ddlWidgetLocation.Items.Add(new ListItem() { Text = "Bottom Right", Value = "6" }); };
        }

        private void HideWidgetTypePanels()
        {
            divContentGeneral.Visible = false;
            divContentItemList.Visible = false;
            divContentWiki.Visible = false;
        }

        #endregion

        #region "Save Widget"

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Validation
            lblName.Attributes.Remove("class");
            lblName.Attributes.Add("class", "input");
            lblWidgetType.Attributes.Remove("class");
            lblWidgetType.Attributes.Add("class", "input");
            lblWidgetLocation.Attributes.Remove("class");
            lblWidgetLocation.Attributes.Add("class", "input");

            var err = false;

            if (String.IsNullOrWhiteSpace(txtName.Value))
            {
                lblName.Attributes.Remove("class");
                lblName.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Widget Name is required";
                lblName.Controls.Add(errorMessageDiv);

                err = true;
            }
            else
            {
                var widgetName = txtName.Value.Trim();
                if (Regex.Matches(widgetName, @"[a-zA-Z ']").Count < widgetName.Length)
                {
                    lblName.Attributes.Remove("class");
                    lblName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Widget Name contains invalid characters (Enter A-Z, a-z, space)";
                    lblName.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }
            if (ddlWidgetType.SelectedValue == "0")
            {
                lblWidgetType.Attributes.Remove("class");
                lblWidgetType.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Widget Type is required";
                lblWidgetType.Controls.Add(errorMessageDiv);

                err = true;
            }
            if (_formMode == FormMode.AddMode)
            {
                if (ddlWidgetStatus.SelectedValue == "1")
                {
                    lblWidgetStatus.Attributes.Remove("class");
                    lblWidgetStatus.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Widget Status must be unpublished for new widgets";
                    lblWidgetStatus.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }
            else
            {
                if (ddlWidgetStatus.SelectedValue == "1" && _metaWidget.WidgetLocation == MetaWidgetLocation.Unassigned && ddlWidgetLocation.SelectedValue == "0")
                {
                    lblWidgetLocation.Attributes.Remove("class");
                    lblWidgetLocation.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Widget Location is required";
                    lblWidgetLocation.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }

            if (err) { return; };

            string url = string.Empty;
            if (_formMode == FormMode.AddMode)
            {
                var encodedName = AntiXssEncoder.HtmlEncode(txtName.Value, false);
                var encodedDefinition = AntiXssEncoder.HtmlEncode(txtDefinition.Value, false);
                var content = string.Empty;

                var widgetType = UnitOfWork.Repository<MetaWidgetType>().Get(Convert.ToInt32(ddlWidgetType.SelectedValue));
                var widgetTypeE = (MetaWidgetTypes)widgetType.Id;

                switch (widgetTypeE)
                {
                    case MetaWidgetTypes.General:
                        content = "** PLEASE ENTER YOUR CONTENT HERE **";
                        break;

                    case MetaWidgetTypes.SubItem:
                    case MetaWidgetTypes.ItemList:
                        content = GetBaseTemplate(widgetTypeE);
                        break;

                    default:
                        break;
                }

                _metaWidget = new MetaWidget()
                {
                    Content = content,
                    MetaPage = _metaPage,
                    WidgetDefinition = encodedDefinition,
                    WidgetType = widgetType,
                    WidgetLocation = ddlWidgetStatus.SelectedValue == "1" ? (MetaWidgetLocation)Convert.ToInt32(ddlWidgetLocation.SelectedValue) : MetaWidgetLocation.Unassigned,
                    WidgetName = encodedName,
                    WidgetStatus = (MetaWidgetStatus)Convert.ToInt32(ddlWidgetStatus.SelectedValue),
                    Icon = txtIcon.Value
            };
                UnitOfWork.Repository<MetaWidget>().Save(_metaWidget);
                url = "PageCustomWidget.aspx?id=" + _metaWidget.Id + "&action=edit";
            }

            if (_formMode == FormMode.EditMode)
            {
                var content = string.Empty;

                var encodedName = AntiXssEncoder.HtmlEncode(txtName.Value, false);
                var encodedDefinition = AntiXssEncoder.HtmlEncode(txtDefinition.Value, false);

                var widgetType = UnitOfWork.Repository<MetaWidgetType>().Get(Convert.ToInt32(ddlWidgetType.SelectedValue));
                var widgetTypeE = (MetaWidgetTypes)widgetType.Id;

                switch (widgetTypeE)
                {
                    case MetaWidgetTypes.General:
                        content = CKEditor1.Text;
                        break;

                    case MetaWidgetTypes.SubItem:
                    case MetaWidgetTypes.ItemList:
                        content = GetContentFromWidget(widgetTypeE);
                        break;

                    default:
                        break;
                }

                _metaWidget.WidgetDefinition = encodedDefinition;
                _metaWidget.WidgetName = encodedName;
                _metaWidget.WidgetType = widgetType;
                _metaWidget.WidgetLocation = ddlWidgetStatus.SelectedValue == "1" ? ddlWidgetLocation.SelectedValue != "0" ? (MetaWidgetLocation)Convert.ToInt32(ddlWidgetLocation.SelectedValue) : _metaWidget.WidgetLocation : MetaWidgetLocation.Unassigned;
                _metaWidget.WidgetStatus = (MetaWidgetStatus)Convert.ToInt32(ddlWidgetStatus.SelectedValue);
                _metaWidget.Content = content;
                _metaWidget.Icon = txtIcon.Value;

                UnitOfWork.Repository<MetaWidget>().Update(_metaWidget);
                url = "PageViewer.aspx?guid=" + ((_metaPage == null) ? _metaWidget.MetaPage.metapage_guid.ToString() : _metaPage.metapage_guid.ToString());
            }

            UnitOfWork.Complete();

            HttpCookie cookie = new HttpCookie("PopUpMessage");
            cookie.Value = "Widget " + ((_formMode == FormMode.AddMode) ? "added" : "updated") + " successfully";
            Response.Cookies.Add(cookie);

            
            Response.Redirect(url);
        }

        private string GetContentFromWidget(MetaWidgetTypes widgetTypeE)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode parentNode = xmlDoc.CreateElement("WidgetList", "");

            // Get all item tabs
            var tabs = Controls.All().OfType<HtmlGenericControl>().Where(h => h.ID != null && (h.ID.StartsWith("tab-")));
            foreach(var tab in tabs)
            {
                XmlNode childNode = xmlDoc.CreateElement("ListItem", "");
                if (widgetTypeE == MetaWidgetTypes.ItemList)
                {
                    var title = tab.Controls.All().OfType<HtmlInputText>().First(h => h.ID != null && (h.ID.StartsWith("title-")));
                    var content = tab.Controls.All().OfType<HtmlTextArea>().First(h => h.ID != null && (h.ID.StartsWith("content-")));

                    if (title != null)
                    {
                        if(!String.IsNullOrWhiteSpace(title.Value))
                        {
                            XmlNode titleNode = xmlDoc.CreateElement("Title", "");
                            titleNode.InnerText = (title != null) ? title.Value : "";
                            XmlNode contentNode = xmlDoc.CreateElement("Content", "");
                            contentNode.InnerText = (content != null) ? content.InnerHtml : "";

                            childNode.AppendChild(titleNode);
                            childNode.AppendChild(contentNode);
                            parentNode.AppendChild(childNode);
                        }
                    }
                }
                if (widgetTypeE == MetaWidgetTypes.SubItem)
                {
                    var title = tab.Controls.All().OfType<HtmlInputText>().First(h => h.ID != null && (h.ID.StartsWith("title-")));
                    var subTitle = tab.Controls.All().OfType<HtmlInputText>().First(h => h.ID != null && (h.ID.StartsWith("subtitle-")));
                    var contentPage = tab.Controls.All().OfType<HtmlSelect>().First(h => h.ID != null && (h.ID.StartsWith("pagecontent-")));

                    if (title != null)
                    {
                        if (!String.IsNullOrWhiteSpace(title.Value))
                        {

                            XmlNode titleNode = xmlDoc.CreateElement("Title", "");
                            titleNode.InnerText = (title != null) ? title.Value : "";
                            XmlNode subTitleNode = xmlDoc.CreateElement("SubTitle", "");
                            subTitleNode.InnerText = (subTitle != null) ? subTitle.Value : "";
                            XmlNode contentPageNode = xmlDoc.CreateElement("ContentPage", "");
                            contentPageNode.InnerText = (contentPage != null) ? contentPage.Value : "";
                            XmlNode modifiedNode = xmlDoc.CreateElement("ModifiedDate", "");
                            modifiedNode.InnerText = DateTime.Today.ToString("yyyy-MM-dd");

                            childNode.AppendChild(titleNode);
                            childNode.AppendChild(subTitleNode);
                            childNode.AppendChild(contentPageNode);
                            childNode.AppendChild(modifiedNode);
                            parentNode.AppendChild(childNode);
                        }
                    }
                }
            }

            xmlDoc.AppendChild(parentNode);
            return xmlDoc.InnerXml;
        }

        private string GetBaseTemplate(MetaWidgetTypes widgetTypeE)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode parentNode = xmlDoc.CreateElement("WidgetList", "");
            XmlNode childNode = xmlDoc.CreateElement("ListItem", "");
            if (widgetTypeE == MetaWidgetTypes.ItemList)
            {
                XmlNode titleNode = xmlDoc.CreateElement("Title", "");
                titleNode.InnerText = "** PLEASE ADD TITLE HERE **";
                XmlNode contentNode = xmlDoc.CreateElement("Content", "");
                contentNode.InnerText = "** PLEASE ADD CONTENT HERE **";

                childNode.AppendChild(titleNode);
                childNode.AppendChild(contentNode);
                parentNode.AppendChild(childNode);
            }
            if (widgetTypeE == MetaWidgetTypes.SubItem)
            {
                XmlNode titleNode = xmlDoc.CreateElement("Title", "");
                titleNode.InnerText = "** PLEASE ADD TITLE HERE **";
                XmlNode subTitleNode = xmlDoc.CreateElement("SubTitle", "");
                subTitleNode.InnerText = "** PLEASE ADD SUB-TITLE HERE **";
                XmlNode contentPageNode = xmlDoc.CreateElement("ContentPage", "");
                contentPageNode.InnerText = _metaPage.Id.ToString();
                XmlNode modifiedNode = xmlDoc.CreateElement("ModifiedDate", "");
                modifiedNode.InnerText = DateTime.Today.ToString("yyyy-MM-dd");

                childNode.AppendChild(titleNode);
                childNode.AppendChild(subTitleNode);
                childNode.AppendChild(contentPageNode);
                childNode.AppendChild(modifiedNode);
                parentNode.AppendChild(childNode);
            }

            xmlDoc.AppendChild(parentNode);
            return xmlDoc.InnerXml;
        }

        #endregion

        protected void ddlWidgetStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            divWidgetLocation.Visible = (ddlWidgetStatus.SelectedValue == "1");
        }
    }

    public static class ControlExtensions
    {
        public static IEnumerable<T> GetAllControlsOfType<T>(this Control parent) where T : Control
        {
            var result = new List<T>();
            foreach (Control control in parent.Controls)
            {
                if (control is T)
                {
                    result.Add((T)control);
                }
                if (control.HasControls())
                {
                    result.AddRange(control.GetAllControlsOfType<T>());
                }
            }
            return result;
        }
    }

    public static class PageExtensions
    {
        public static IEnumerable<Control> All(this ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                foreach (Control grandChild in control.Controls.All())
                {
                    yield return grandChild;
                }
                yield return control;
            }
        }
    }
}