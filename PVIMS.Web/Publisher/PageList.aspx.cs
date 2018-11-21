using System;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class PageList : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("PublishAdminList");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Page List", SubTitle = "", Icon = "fa fa-windows fa-fw" });

            if (!Page.IsPostBack)
            {
                RenderItems();
            }
        }

        private void RenderItems()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;

            // Loop through and render table
            foreach (var pg in UnitOfWork.Repository<MetaPage>().Queryable().Where(mp => mp.IsVisible == false).OrderBy(mp => mp.PageName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = pg.PageName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = pg.metapage_guid.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = pg.PageDefinition;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = pg.MetaDefinition;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = pg.Breadcrumb;
                row.Cells.Add(cell);

                if(pg.IsSystem)
                {
                    cell = new TableCell();
                    cell.Text = @"<span class=""label label-warning"">System</span>";
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    pnl = new Panel() { CssClass = "btn-group" };
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "PageViewer.aspx?guid=" + pg.metapage_guid.ToString(),
                        CssClass = "btn btn-default",
                        Text = "Customise"
                    };
                    pnl.Controls.Add(hyp);
                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);
                }

                dt_basic.Rows.Add(row);
            }

        }

    }
}