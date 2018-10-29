﻿using System;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ReportList : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("ReportList");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Custom Reports", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

            if (!Page.IsPostBack) {
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
            foreach (var rep in UnitOfWork.Repository<MetaReport>().Queryable().OrderBy(mr => mr.ReportName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = rep.metareport_guid.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = rep.ReportName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = rep.ReportDefinition;
                row.Cells.Add(cell);

                if(rep.IsSystem)
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
                        NavigateUrl = "ManageDatasetEdit.aspx?id=" + rep.Id.ToString(),
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
                NavigateUrl = "ReportCustom.aspx?id=0",
                CssClass = "btn btn-primary",
                Text = "Add Report"
            };
            spnbuttons.Controls.Add(hyp);
        }

    }
}