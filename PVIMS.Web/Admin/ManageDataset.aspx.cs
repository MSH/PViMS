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
    public partial class ManageDataset : MainPageBase
    {
        private readonly PVIMSDbContext _db = new PVIMSDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.MainMenu.SetActive("AdminDataset");

            if (!Page.IsPostBack)
            {
                RenderItems();
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (_db != null)
                _db.Dispose();
        }

        private void RenderItems()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;

            string created;
            string updated;

            // Loop through and render table
            foreach (var d in UnitOfWork.Repository<Dataset>().Queryable().OrderBy(d => d.DatasetName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = d.DatasetName;
                row.Cells.Add(cell);

                // Audit
                created = String.Format("Created by {0} on {1} ...", "UNKNOWN", d.Created.ToString("yyyy-MM-dd"));
                if (d.LastUpdated != null) {
                    updated = String.Format("Updated by {0} on {1} ...", "UNKNOWN", Convert.ToDateTime(d.LastUpdated).ToString("yyyy-MM-dd"));
                }
                else {
                    updated = "NOT UPDATED";
                }

                cell = new TableCell();
                cell.Text = created;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = updated;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = d.ContextType.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = d.Active ? "Yes" : "No";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = d.Help;
                row.Cells.Add(cell);

                cell = new TableCell();
                pnl = new Panel() { CssClass = "btn-group" };
                hyp = new HyperLink()
                {
                    NavigateUrl = "ManageDatasetEdit.aspx?id=" + d.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Edit Dataset"
                };
                pnl.Controls.Add(hyp);

                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "";
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }

            // Add button
            hyp = new HyperLink()
            {
                ID = "btnAdd",
                NavigateUrl = "ManageDatasetEdit.aspx?id=0",
                CssClass = "btn btn-primary",
                Text = "Add Dataset"
            };
            spnbuttons.Controls.Add(hyp);
        }

    }
}