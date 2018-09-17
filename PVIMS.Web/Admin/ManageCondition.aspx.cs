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
    public partial class ManageCondition : MainPageBase
    {
        private readonly PVIMSDbContext _db = new PVIMSDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminCondition");

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
            Label lbl;

            string labtests;
            string medications;
            string terminologies;

            // Loop through and render table
            foreach (var c in UnitOfWork.Repository<Condition>().Queryable().Include("ConditionLabTests.LabTest").Include("ConditionMedications.Medication").Include("ConditionMedDras.TerminologyMedDra").OrderBy(c => c.Description))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = c.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    CssClass = "btn btn-default",
                    Text = "View Terminologies"
                };
                hyp.Attributes.Add("data-toggle", "popover");
                hyp.Attributes.Add("data-placement", "top");
                hyp.Attributes.Add("data-original-title", "Terminologies");

                terminologies = "";
                if (c.ConditionMedDras.Count == 0) {
                    terminologies += string.Format(@"<span class=""label label-danger"">{0}</span>", @"No terminologies configured");
                }
                else
                {
                    terminologies += "<ul>";
                    foreach (var cmd in c.ConditionMedDras) {
                        terminologies += string.Format("<li>{0}</li>", cmd.TerminologyMedDra.MedDraTerm);
                    }
                    terminologies += "</ul>";
                }
                hyp.Attributes.Add("data-content", terminologies);
                hyp.Attributes.Add("data-html", "true");
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                cell = new TableCell();
                pnl = new Panel() { CssClass = "btn-group" };
                hyp = new HyperLink()
                {
                    NavigateUrl = "ManageConditionEdit.aspx?id=" + c.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Edit Condition Group"
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
                NavigateUrl = "ManageConditionEdit.aspx?id=0",
                CssClass = "btn btn-primary",
                Text = "Add Condition Group"
            };
            spnbuttons.Controls.Add(hyp);
        }

    }
}