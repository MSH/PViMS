using System;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ManageHoliday : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminHoliday");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Public Holidays", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

            if (!Page.IsPostBack) {
                LoadDropDownList();
                RenderHolidays();
            }
        }

        private void LoadDropDownList()
        {
            ListItem item;

            for (int i = 0; i <= 3; i++)
            {
                item = new ListItem();
                item.Text = DateTime.Today.AddYears(i * -1).Year.ToString();
                item.Value = DateTime.Today.AddYears(i * -1).Year.ToString();
                ddlCriteria.Items.Add(item);
            }
            ddlCriteria.SelectedIndex = 0;
        }

        protected void ddlCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderHolidays();
        }

        protected void btnSaveHoliday_Click(object sender, EventArgs e)
        {
            Holiday holiday = null;

            if (txtHolidayUID.Value == "0")
            {
                holiday = new Holiday { HolidayDate = Convert.ToDateTime(txtHolidayDate.Value), Description = txtHolidayReason.Value };

                UnitOfWork.Repository<Holiday>().Save(holiday);
            }
            else {
                holiday = GetHoliday(Convert.ToInt32(txtHolidayUID.Value));

                if (holiday != null)
                {
                    holiday.HolidayDate = Convert.ToDateTime(txtHolidayDate.Value);
                    holiday.Description = txtHolidayReason.Value;

                    UnitOfWork.Repository<Holiday>().Update(holiday);
                }
            }

            UnitOfWork.Complete();

            RenderHolidays();
        }

        protected void btnDeleteHoliday_Click(object sender, EventArgs e)
        {
            Holiday holiday = null;

            if (txtHolidayUID.Value != "0") {
                holiday = GetHoliday(Convert.ToInt32(txtHolidayUID.Value));
            }

            if (holiday != null)
            {
                UnitOfWork.Repository<Holiday>().Delete(holiday);
                UnitOfWork.Complete();

                RenderHolidays();
            }
        }

        private void RenderHolidays()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Loop through and render table
            int searchYear = Convert.ToInt32(ddlCriteria.SelectedValue);
            foreach (var h in UnitOfWork.Repository<Holiday>().Queryable().Where(h => h.HolidayDate.Year == searchYear).OrderByDescending(h => h.HolidayDate))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = h.HolidayDate.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = h.Description;
                row.Cells.Add(cell);

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
                    NavigateUrl = "#",
                    Text = "Edit Holiday"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#holidayModal");
                hyp.Attributes.Add("data-id", h.Id.ToString());
                hyp.Attributes.Add("data-evt", "edit");
                hyp.Attributes.Add("data-date", h.HolidayDate.ToString("yyyy-MM-dd"));
                hyp.Attributes.Add("data-reason", h.Description);

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                // Delete menu if necessary
                if (h.HolidayDate > DateTime.Today)
                {
                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "#",
                        Text = "Delete Holiday"
                    };
                    hyp.Attributes.Add("data-toggle", "modal");
                    hyp.Attributes.Add("data-target", "#holidayModal");
                    hyp.Attributes.Add("data-id", h.Id.ToString());
                    hyp.Attributes.Add("data-evt", "delete");
                    hyp.Attributes.Add("data-date", h.HolidayDate.ToString("yyyy-MM-dd"));
                    hyp.Attributes.Add("data-reason", h.Description);
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);
                }

                pnl.Controls.Add(ul);

                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }

            // Add button
            hyp = new HyperLink()
            {
                ID = "btnAdd",
                NavigateUrl = "#",
                CssClass = "btn btn-primary",
                Text = "Add Holiday"
            };
            hyp.Attributes.Add("data-toggle", "modal");
            hyp.Attributes.Add("data-target", "#holidayModal");
            hyp.Attributes.Add("data-id", "0");
            hyp.Attributes.Add("data-evt", "add");
            hyp.Attributes.Add("data-date", DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
            hyp.Attributes.Add("data-reason", "");
            spnHolidaybuttons.Controls.Add(hyp);
        }

        #region "EF"

        private Holiday GetHoliday(int id)
        {
            return UnitOfWork.Repository<Holiday>().Queryable().SingleOrDefault(h => h.Id == id);
        }

        #endregion
    }
}