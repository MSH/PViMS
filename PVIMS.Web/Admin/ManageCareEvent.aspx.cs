using System;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ManageCareEvent : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminCareEvent");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Care Events", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

            if (!Page.IsPostBack)
            {
                RenderItems();
            }
        }

        private void RenderItems()
        {
            TableRow row;
            TableCell cell;

            Panel pnl;
            HyperLink hyp;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            string workplans;

            // Loop through and render table
            foreach (var c in UnitOfWork.Repository<CareEvent>().Queryable().OrderBy(c => c.Description))
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
                    Text = "View Work Plans"
                };
                hyp.Attributes.Add("data-toggle", "popover");
                hyp.Attributes.Add("data-placement", "top");
                hyp.Attributes.Add("data-original-title", "Work Plans");

                workplans = "";
                if (c.WorkPlanCareEvents.Count == 0) {
                    workplans += string.Format(@"<span class=""label label-danger"">{0}</span>", @"No work plans configured");
                }
                else
                {
                    workplans += "<ul>";
                    foreach (var wp in c.WorkPlanCareEvents)
                    {
                        workplans += string.Format("<li>{0}</li>", wp.WorkPlan.Description);
                    }
                    workplans += "</ul>";
                }
                hyp.Attributes.Add("data-content", workplans);
                hyp.Attributes.Add("data-html", "true");
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                cell = new TableCell();
                // Delete menu if necessary
                if (c.WorkPlanCareEvents.Count == 0)
                {
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
                        NavigateUrl = "/CareEvent/EditCareEvent?ceId=" + c.Id.ToString(),
                        Text = "Edit Care Event"
                    };

                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/CareEvent/DeleteCareEvent?ceId=" + c.Id.ToString(),
                        Text = "Delete Care Event"
                    };
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    pnl.Controls.Add(ul);
                }
                else 
                {
                    pnl = new Panel() { CssClass = "btn-group" };
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/CareEvent/EditCareEvent?ceId=" + c.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "Edit Care Event"
                    };

                    pnl.Controls.Add(hyp);
                }

                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }

            // Add button
            hyp = new HyperLink()
            {
                ID = "btnAdd",
                NavigateUrl = "/CareEvent/AddCareEvent",
                CssClass = "btn btn-primary",
                Text = "Add Care Event"
            };
            spnbuttons.Controls.Add(hyp);
        }

        #region "EF"

        private CareEvent GetCareEvent(int id)
        {
            return UnitOfWork.Repository<CareEvent>().Queryable().SingleOrDefault(u => u.Id == id);
        }

        #endregion
    }
}