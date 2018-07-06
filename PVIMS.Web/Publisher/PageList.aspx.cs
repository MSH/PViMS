using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using VPS.Common.Repositories;

using PVIMS.Core;
using PVIMS.Core.Entities;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class PageList : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.MainMenu.SetActive("PublishAdmin");

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
            foreach (var pg in UnitOfWork.Repository<MetaPage>().Queryable().OrderBy(mp => mp.PageName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = pg.metapage_guid.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = pg.PageName;
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
                        NavigateUrl = "PageCustom.aspx?id=" + pg.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "Customise"
                    };
                    pnl.Controls.Add(hyp);
                    cell.Controls.Add(pnl);
                    row.Cells.Add(cell);
                }

                dt_basic.Rows.Add(row);
            }

            // Add button
            hyp = new HyperLink()
            {
                ID = "btnAdd",
                NavigateUrl = "PageCustom.aspx?Id=0",
                CssClass = "btn btn-primary",
                Text = "Add Page"
            };
            spnbuttons.Controls.Add(hyp);
        }

    }
}