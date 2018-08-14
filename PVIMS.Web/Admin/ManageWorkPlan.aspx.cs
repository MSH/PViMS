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
    public partial class ManageWorkPlan : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminWorkPlan");

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
            Label lbl;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Loop through and render table
            foreach (var w in UnitOfWork.Repository<WorkPlan>().Queryable().OrderBy(w => w.Description))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = w.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = w.Dataset.DatasetName;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "ManageWorkPlanEdit.aspx?id=" + w.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Edit Work Plan"
                };

                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }

            // Add button
            hyp = new HyperLink()
            {
                ID = "btnAdd",
                NavigateUrl = "ManageWorkPlanEdit.aspx?id=0",
                CssClass = "btn btn-primary",
                Text = "Add Work Plan"
            };
            spnbuttons.Controls.Add(hyp);
        }

    }
}