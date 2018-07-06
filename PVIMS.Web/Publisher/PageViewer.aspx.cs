using System;
using System.Linq;

using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Web
{
    public partial class PageViewer : MainPageBase
    {
        private MetaPage _metaPage;

        private Guid _guid;
        private Guid _ruid;

        private bool _isPublisher = false;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.IsInRole("PublisherAdmin")) { _isPublisher = true; }

            if (Request.QueryString["guid"] != null)
            {
                _guid = Guid.Parse(Request.QueryString["guid"]);
                _metaPage = UnitOfWork.Repository<MetaPage>().Queryable().Single(mp => mp.metapage_guid == _guid);

                // Prepare report
                RenderHeader();

                // Render unpublished widgets
                if (_isPublisher)
                {
                    var widgetCount = 12;
                    var div = new HtmlGenericControl("div");
                    foreach (MetaWidget uwidget in _metaPage.Widgets.Where(w => w.WidgetStatus == MetaWidgetStatus.Unpublished))
                    {
                        if(widgetCount == 12)
                        {
                            if (div.Controls.Count > 0) { spnUnpublished.Controls.Add(div); };

                            // add new row
                            div = new HtmlGenericControl("div");
                            div.Attributes.Add("class", "row");
                            widgetCount = 0;
                        }
                        widgetCount += 1;

                        var section = RenderUnpublishedWidget(uwidget);
                        div.Controls.Add(section);
                    }
                    if (div.Controls.Count > 0) { spnUnpublished.Controls.Add(div); };
                }

                // Render top widgets
                var widget = _metaPage.Widgets.FirstOrDefault(w => w.WidgetStatus == MetaWidgetStatus.Published && w.WidgetLocation == MetaWidgetLocation.TopLeft);
                RenderWidget(widget, MetaWidgetLocation.TopLeft, 1);
                widget = _metaPage.Widgets.FirstOrDefault(w => w.WidgetStatus == MetaWidgetStatus.Published && w.WidgetLocation == MetaWidgetLocation.TopRight);
                RenderWidget(widget, MetaWidgetLocation.TopRight, 2);

                // Render middle widgets
                widget = _metaPage.Widgets.FirstOrDefault(w => w.WidgetStatus == MetaWidgetStatus.Published && w.WidgetLocation == MetaWidgetLocation.MiddleLeft);
                RenderWidget(widget, MetaWidgetLocation.MiddleLeft, 3);
                widget = _metaPage.Widgets.FirstOrDefault(w => w.WidgetStatus == MetaWidgetStatus.Published && w.WidgetLocation == MetaWidgetLocation.MiddleRight);
                RenderWidget(widget, MetaWidgetLocation.MiddleRight, 4);

                // Render bottom widgets
                widget = _metaPage.Widgets.FirstOrDefault(w => w.WidgetStatus == MetaWidgetStatus.Published && w.WidgetLocation == MetaWidgetLocation.BottomLeft);
                RenderWidget(widget, MetaWidgetLocation.BottomLeft, 5);
                widget = _metaPage.Widgets.FirstOrDefault(w => w.WidgetStatus == MetaWidgetStatus.Published && w.WidgetLocation == MetaWidgetLocation.BottomRight);
                RenderWidget(widget, MetaWidgetLocation.BottomRight, 6);
            }
            else
            {
                throw new Exception("guid not passed as parameter");
            }
            divReturn.Visible = false;
            if (Request.QueryString["ruid"] != null)
            {
                _ruid = Guid.Parse(Request.QueryString["ruid"]);
                HyperLink hyp = new HyperLink()
                {
                    ID = "btnReturn",
                    NavigateUrl = "PageViewer.aspx?guid=" + _ruid.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Return"
                };
                spnButtons.Controls.Add(hyp);
                divReturn.Visible = true;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Master.MainMenu.SetActive("ReportOutstandingVisit");
        }

        #region "Rendering"

        private void RenderHeader()
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = _metaPage.PageName, SubTitle = "", Icon = "fa fa-files-o fa-fw", MetaPageId = _isPublisher ? _metaPage.Id : 0 });
        }

        private void RenderWidget(MetaWidget metaWidget, MetaWidgetLocation widgetLocation, int widgetCount)
        {
            var article = new HtmlGenericControl("article");
            article.Attributes.Add("class", "col-sm-12 col-md-12 col-lg-12");

            var mainDiv = (metaWidget != null) ? PrepareArticleOutlineForWidget(metaWidget, widgetCount) : PrepareArticlePlaceholderWidget();
            if (_isPublisher && metaWidget != null)
            {
                var editDiv = PrepareArticleEditForWidget(metaWidget, true);
                mainDiv.Controls.Add(editDiv);
            }

            article.Controls.Add(mainDiv);

            if (widgetLocation == Core.ValueTypes.MetaWidgetLocation.TopLeft)
            {
                spnTopLeft.Controls.Add(article);
            }
            if (widgetLocation == Core.ValueTypes.MetaWidgetLocation.TopRight)
            {
                spnTopRight.Controls.Add(article);
            }
            if (widgetLocation == Core.ValueTypes.MetaWidgetLocation.MiddleLeft)
            {
                spnMiddleLeft.Controls.Add(article);
            }
            if (widgetLocation == Core.ValueTypes.MetaWidgetLocation.MiddleRight)
            {
                spnMiddleRight.Controls.Add(article);
            }
            if (widgetLocation == Core.ValueTypes.MetaWidgetLocation.BottomLeft)
            {
                spnBottomLeft.Controls.Add(article);
            }
            if (widgetLocation == Core.ValueTypes.MetaWidgetLocation.BottomRight)
            {
                spnBottomRight.Controls.Add(article);
            }
        }

        private HtmlGenericControl RenderUnpublishedWidget(MetaWidget metaWidget)
        {
            var section = new HtmlGenericControl("section");
            section.Attributes.Add("class", "well col col-md-2");

            var header = new HtmlGenericControl("h3");
            header.InnerHtml = metaWidget.WidgetName;
            section.Controls.Add(header);

            var editDiv = PrepareArticleEditForWidget(metaWidget, false);
            section.Controls.Add(editDiv);

            return section;
        }

        private HtmlGenericControl PrepareArticleOutlineForWidget(MetaWidget metaWidget, int widgetCount)
        {
            var mainDiv = new HtmlGenericControl("div");
            mainDiv.Attributes.Add("class", "well");

            var widgetTypeE = (MetaWidgetTypes)metaWidget.WidgetType.Id;

            // Header
            if(widgetTypeE != MetaWidgetTypes.Wiki)
            {
                var header = new HtmlGenericControl("h2");
                header.InnerHtml = String.Format(@"<i class=""fa {0} text-muted""></i>&nbsp;&nbsp;{1}", metaWidget.Icon, metaWidget.WidgetName);
                mainDiv.Controls.Add(header);
            }

            // Content
            var contentDiv = new HtmlGenericControl("div");
            switch (widgetTypeE)
            {
                case MetaWidgetTypes.General:
                    contentDiv.InnerHtml = metaWidget.Content;

                    break;

                case MetaWidgetTypes.Wiki:
                    var tableDiv = new HtmlGenericControl("table");
                    tableDiv.Attributes.Add("class", "table table-striped table-forum");

                    // Add header row
                    var headerRowDiv = new HtmlGenericControl("thead");
                    headerRowDiv.InnerHtml = String.Format(@"<tr><th class=""text-center"" style=""width: 40px; ""><i class=""fa {0} fa-2x text-muted""></i></th><th>{1} </th><th class=""text-center hidden-xs hidden-sm"" style=""width: 100px;"">Updated</th></tr>", metaWidget.Icon, metaWidget.WidgetName);
                    tableDiv.Controls.Add(headerRowDiv);

                    var bodyRowDiv = new HtmlGenericControl("tbody");

                    // Create the XmlDocument
                    XmlDocument wdoc = new XmlDocument();
                    wdoc.LoadXml(metaWidget.Content);
                    XmlNode wroot = wdoc.DocumentElement;

                    // Loop through each listitem
                    var witem = 0;
                    foreach (XmlNode node in wroot.ChildNodes)
                    {
                        witem += (widgetCount * 100) + 1;
                        var title = node.ChildNodes[0].InnerText;
                        var subTitle = node.ChildNodes[1].InnerText;
                        var contentPage = node.ChildNodes[2].InnerText;
                        var modified = node.ChildNodes[3] != null ? node.ChildNodes[3].InnerText : DateTime.Today.ToString("yyyy-MM-dd");

                        if (!String.IsNullOrWhiteSpace(title))
                        {
                            bodyRowDiv.Controls.Add(PrepareWikiItemForArticle(title, subTitle, Convert.ToInt32(contentPage), modified, witem, ref contentDiv));
                        }
                    }
                    tableDiv.Controls.Add(bodyRowDiv);

                    contentDiv.Controls.Add(tableDiv);

                    break;

                case MetaWidgetTypes.ItemList:
                    contentDiv.Attributes.Add("class", "panel-group smart-accordion-default");
                    contentDiv.ID = "accordion-" + metaWidget.Id.ToString();
                    
                    // Create the XmlDocument
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(metaWidget.Content);
                    XmlNode root = doc.DocumentElement;

                    // Loop through each listitem
                    var item = 0;
                    foreach(XmlNode node in root.ChildNodes)
                    {
                        item += (widgetCount * 100) + 1;
                        var title = node.ChildNodes[0].InnerText;
                        var content = node.ChildNodes[1].InnerText;

                        contentDiv.Controls.Add(PrepareListItemForArticle(title, content, item, ref contentDiv, item == (widgetCount * 100) + 1));
                    }
                    
                    break;

                default:
                    break;
            }
            mainDiv.Controls.Add(contentDiv);

            return mainDiv;
        }

        private HtmlGenericControl PrepareArticleEditForWidget(MetaWidget metaWidget, bool published)
        {
            var rowDiv = new HtmlGenericControl("div");
            rowDiv.Attributes.Add("class", "row");

            var editDiv = new HtmlGenericControl("div");
            editDiv.Attributes.Add("class", published ? "col-md-3 col-md-offset-9" : "col-md-12");

            var hyp = new HyperLink()
            {
                NavigateUrl = "PageCustomWidget.aspx?Id=" + metaWidget.Id + "&action=35EF6F4A-1CF6-4F94-B92A-145AC57D1135",
                CssClass = "btn btn-primary btn-sm",
                Text = "Configure"
            };
            editDiv.Controls.Add(hyp);
            if (!published)
            {
                hyp = new HyperLink()
                {
                    NavigateUrl = "/Publisher/MoveMetaWidget?metaWidgetId=" + metaWidget.Id,
                    CssClass = "btn btn-primary btn-sm",
                    Text = "Move"
                };
            };
            editDiv.Controls.Add(hyp);
            hyp = new HyperLink()
            {
                NavigateUrl = "/Publisher/DeleteMetaWidget?metaWidgetId=" + metaWidget.Id,
                CssClass = "btn btn-default btn-sm",
                Text = "Delete"
            };
            editDiv.Controls.Add(hyp);
            rowDiv.Controls.Add(editDiv);

            return rowDiv;
        }

        private HtmlGenericControl PrepareArticlePlaceholderWidget()
        {
            var mainDiv = new HtmlGenericControl("div");
            mainDiv.Attributes.Add("class", "jarviswidget");
            mainDiv.Attributes.Add("data-widget-colorbutton", "false");
            mainDiv.Attributes.Add("data-widget-editbutton", "false");

            return mainDiv;
        }

        private HtmlGenericControl PrepareListItemForArticle(string title, string content, int item, ref HtmlGenericControl contentDiv, bool first)
        {
            var contentItemDiv = new HtmlGenericControl("div");
            contentItemDiv.Attributes.Add("class", "panel panel-default");

            var headingDiv = new HtmlGenericControl("div");
            headingDiv.Attributes.Add("class", "panel-heading");
            headingDiv.InnerHtml = String.Format(@"<h4 class=""panel-title""><a data-toggle=""collapse"" data-parent=""#{0}"" href=""#collapse-{1}"" {3}> <i class=""fa fa-fw fa-plus-circle txt-color-green""></i> <i class=""fa fa-fw fa-minus-circle txt-color-red""></i> {2}  </a></h4>", contentDiv.ID, item.ToString(), title, first ? "" : @" class=""collapsed""");
            contentItemDiv.Controls.Add(headingDiv);

            var detailDiv = new HtmlGenericControl("div");
            detailDiv.ID = "collapse-" + item.ToString();
            detailDiv.Attributes.Add("class", first ? "panel-collapse collapse in" : "panel-collapse collapse");
            detailDiv.InnerHtml = String.Format(@"<div class=""panel-body"">{0}</div>", content);
            contentItemDiv.Controls.Add(detailDiv);

            return contentItemDiv;
        }

        private HtmlGenericControl PrepareWikiItemForArticle(string title, string subTitle, int contentPage, string modified, int item, ref HtmlGenericControl contentDiv)
        {
            var metaPage = UnitOfWork.Repository<MetaPage>().Queryable().Single(mp => mp.Id == contentPage);
            var rowDiv = new HtmlGenericControl("tr");

            rowDiv.InnerHtml = String.Format(@"<td colspan=""2""><h4><a href=""PageViewer.aspx?guid={0}&ruid={1}"">{2}</a><small>{3}</small></h4></td><td class=""text-center hidden-xs hidden-sm"">{4}</td>", metaPage.metapage_guid.ToString(), _metaPage.metapage_guid.ToString(), title, subTitle, modified);

            return rowDiv;
        }

        #endregion

        #region "Internal"

        #endregion

    }
}
