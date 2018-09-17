using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.Entity;
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
    public partial class ManageGrading : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminGrading");

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

            // Loop through and render table
            foreach (var grad in UnitOfWork.Repository<MedDRAScale>().Queryable().Include("GradingScale").Include("TerminologyMedDra").OrderBy(g => g.TerminologyMedDra.MedDraTerm).ThenBy(g => g.GradingScale.Value))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = grad.TerminologyMedDra.MedDraTerm;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = grad.GradingScale.Value;
                row.Cells.Add(cell);

                cell = new TableCell();
                pnl = new Panel() { CssClass = "btn-group" };
                hyp = new HyperLink()
                {
                    NavigateUrl = "ManageGradingEdit.aspx?id=" + grad.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Edit Scale Grading"
                };
                pnl.Controls.Add(hyp);

                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }

            // Add button
            hyp = new HyperLink()
            {
                ID = "btnAdd",
                NavigateUrl = "ManageGradingEdit.aspx?id=0",
                CssClass = "btn btn-primary",
                Text = "Add Scale Grading"
            };
            spnbuttons.Controls.Add(hyp);
        }

    }
}