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
    public partial class ManageEncounterType : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.MainMenu.SetActive("AdminEncounterType");

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
            HyperLink storeHyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Loop through and render table
            foreach (var e in UnitOfWork.Repository<EncounterType>().Queryable().OrderBy(e => e.Description))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = e.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = e.Help;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    CssClass = "btn btn-default",
                    Text = "View Work Plans"
                };
                hyp.Attributes.Add("data-toggle", "popover");
                hyp.Attributes.Add("data-placement", "top");
                hyp.Attributes.Add("data-original-title", "Work Plans");

                var workPlans = "";
                if (e.EncounterTypeWorkPlans.Count == 0)
                {
                    workPlans += string.Format(@"<span class=""label label-danger"">{0}</span>", @"No work plans configured");
                }
                else
                {
                    workPlans += "<ul>";
                    foreach (var wp in e.EncounterTypeWorkPlans)
                    {
                        workPlans += string.Format("<li>{0}</li>", wp.WorkPlan.Description);
                    }
                    workPlans += "</ul>";
                }
                hyp.Attributes.Add("data-content", workPlans);
                hyp.Attributes.Add("data-html", "true");
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                cell = new TableCell();
                pnl = new Panel() { CssClass = "btn-group" };
                storeHyp = new HyperLink()
                {
                    NavigateUrl = "/EncounterType/EditEncounterType?etId=" + e.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Edit Encounter Type"
                };
                pnl.Controls.Add(storeHyp);

                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }

            // Add button
            hyp = new HyperLink()
            {
                ID = "btnAdd",
                NavigateUrl = "/EncounterType/AddEncounterType",
                CssClass = "btn btn-primary",
                Text = "Add Encounter Type"
            };
            spnbuttons.Controls.Add(hyp);
        }

    }
}