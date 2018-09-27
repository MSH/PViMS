using System;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web.UI;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class ManageDatasetElement : MainPageBase
    {
        private readonly PVIMSDbContext _db = new PVIMSDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminDatasetElement");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Dataset Elements", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

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

            string categories;

            // Loop through and render table
            foreach (var d in UnitOfWork.Repository<DatasetElement>().Queryable().Include("DatasetCategoryElements.DatasetCategory.Dataset").OrderBy(d => d.ElementName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = d.ElementName;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    CssClass = "btn btn-default",
                    Text = "View Categories"
                };
                hyp.Attributes.Add("data-toggle", "popover");
                hyp.Attributes.Add("data-placement", "top");
                hyp.Attributes.Add("data-original-title", "Categories");

                categories = "";
                if (d.DatasetCategoryElements.Count == 0) {
                    categories += string.Format(@"<span class=""label label-danger"">{0}</span>", @"No categories configured");
                }
                else
                {
                    categories += "<ul>";
                    foreach (var dce in d.DatasetCategoryElements)
                    {
                        categories += string.Format("<li>{0} ({1})</li>", dce.DatasetCategory.Dataset.DatasetName, dce.DatasetCategory.DatasetCategoryName != null ? dce.DatasetCategory.DatasetCategoryName : "");
                    }
                    categories += "</ul>";
                }
                hyp.Attributes.Add("data-content", categories);
                hyp.Attributes.Add("data-html", "true");
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = d.Field.FieldType.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = d.Field.Mandatory ? "Yes" : "No";
                row.Cells.Add(cell);

                cell = new TableCell();
                pnl = new Panel() { CssClass = "btn-group" };
                hyp = new HyperLink()
                {
                    NavigateUrl = "ManageDatasetElementEdit.aspx?id=" + d.Id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "Edit Element"
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
                NavigateUrl = "ManageDatasetElementEdit.aspx?id=0",
                CssClass = "btn btn-primary",
                Text = "Add Element"
            };
            spnbuttons.Controls.Add(hyp);
        }

    }
}