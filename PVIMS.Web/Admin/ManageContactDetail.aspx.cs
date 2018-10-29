using System;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ManageContactDetail : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminContact");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Contact Details", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

            if (!Page.IsPostBack) {
                RenderItems();
            }
            else {
                divstatus.Visible = false;
            }
        }

        private void RenderItems()
        {
            TableRow row;
            TableCell cell;

            HyperLink storeHyp;

            // Loop through and render table
            foreach (var c in UnitOfWork.Repository<SiteContactDetail>().Queryable().OrderBy(c => c.ContactType))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = c.ContactType.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = c.ContactFirstName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = c.ContactSurname;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = c.City;
                row.Cells.Add(cell);

                cell = new TableCell();
                storeHyp = new HyperLink()
                {
                    NavigateUrl = "/ContactDetail/EditContactDetail?cId=" + c.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Edit Detail"
                };

                cell.Controls.Add(storeHyp);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }
        }

    }
}