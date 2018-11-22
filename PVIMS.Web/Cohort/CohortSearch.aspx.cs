using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class CohortSearch : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Cohorts", SubTitle = "", Icon = "fa fa-cogs fa-fw" });
            Master.SetMenuActive("CohortView");

            if (!Page.IsPostBack) 
            {
                RenderCohorts();
            }
            else 
            {
                divstatus.Visible = false;
            }
        }

        private void RenderCohorts()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Loop through and render table
            foreach (var coh in UnitOfWork.Repository<CohortGroup>().Queryable().OrderBy(f => f.CohortName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = coh.Id.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = coh.CohortName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = coh.CohortCode;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = coh.Condition != null ? coh.Condition.Description : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = coh.CohortGroupEnrolments.Count.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = coh.StartDate.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = Convert.ToDateTime(coh.FinishDate).ToString("yyyy-MM-dd") != DateTime.MaxValue.ToString("yyyy-MM-dd") ? Convert.ToDateTime(coh.FinishDate).ToString("yyyy-MM-dd") : @"<span class=""label label-info""></span>";
                row.Cells.Add(cell);

                if (HttpContext.Current.User.IsInRole("Admin"))
                {
                    cell = new TableCell();
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
                        NavigateUrl = "CohortView.aspx?id=" + coh.Id.ToString(),
                        Text = "View Cohort"
                    };

                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/Cohort/EditCohort?cgId=" + coh.Id.ToString(),
                        Text = "Edit Cohort"
                    };
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);

                    if(coh.CohortGroupEnrolments.Count == 0)
                    {
                        li = new HtmlGenericControl("li");
                        hyp = new HyperLink()
                        {
                            NavigateUrl = "/Cohort/DeleteCohort?cgId=" + coh.Id.ToString(),
                            Text = "Delete Cohort"
                        };
                        li.Controls.Add(hyp);
                        ul.Controls.Add(li);
                    }

                    pnl.Controls.Add(ul);
                    cell.Controls.Add(pnl);
                }
                else
                {
                    cell = new TableCell();
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "CohortView.aspx?id=" + coh.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "View Cohort"
                    };
                    cell.Controls.Add(hyp);
                }
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }

            // Add button
            if (HttpContext.Current.User.IsInRole("Admin"))
            {
                hyp = new HyperLink()
                {
                    ID = "btnAdd",
                    NavigateUrl = "/Cohort/AddCohort",
                    CssClass = "btn btn-primary",
                    Text = "Add Cohort"
                };
                spnbuttons.Controls.Add(hyp);
            }
        }

    }
}