using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ManageConfig : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.MainMenu.SetActive("AdminConfig");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Configurations", SubTitle = "", Icon = "fa fa-windows fa-fw" });

            if (!Page.IsPostBack)
            {
                RenderItems();
            }
            else
            {
                divstatus.Visible = false;
            }
        }

        private void RenderItems()
        {
            TableRow row;
            TableCell cell;

            HyperLink storeHyp;

            List<ConfigType> allowAdmin = new List<ConfigType>() { ConfigType.AssessmentScale, ConfigType.E2BVersion, ConfigType.MedicationOnsetCheckPeriodWeeks, ConfigType.ReportInstanceNewAlertCount, ConfigType.WebServiceSubscriberList };

            // Loop through and render table
            foreach (var c in UnitOfWork.Repository<Config>().Queryable().OrderBy(c => c.ConfigType))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = c.ConfigType.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = c.ConfigValue;
                row.Cells.Add(cell);

                cell = new TableCell();
                if (allowAdmin.Contains(c.ConfigType))
                {
                    storeHyp = new HyperLink()
                    {
                        NavigateUrl = "/Config/EditConfig?cId=" + c.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "Edit Configuration"
                    };
                    cell.Controls.Add(storeHyp);
                }
                else
                {
                    cell.Text = string.Empty;
                }
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }
        }

    }
}