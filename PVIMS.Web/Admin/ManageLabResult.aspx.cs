using System;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ManageLabResult : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminLabResult");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Test Results", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

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

            HyperLink hyp;
            Panel pnl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Loop through and render table
            foreach (var l in UnitOfWork.Repository<LabResult>().Queryable().OrderBy(l => l.Description))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = l.Description;
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
                    Text = "Edit Test Result"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#labresultModal");
                hyp.Attributes.Add("data-id", l.Id.ToString());
                hyp.Attributes.Add("data-evt", "edit");
                hyp.Attributes.Add("data-name", l.Description);

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    Text = "Delete Test Result"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#labresultModal");
                hyp.Attributes.Add("data-id", l.Id.ToString());
                hyp.Attributes.Add("data-evt", "delete");
                hyp.Attributes.Add("data-name", l.Description);
                li.Controls.Add(hyp);
                ul.Controls.Add(li);

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
                Text = "Add Test Result"
            };
            hyp.Attributes.Add("data-toggle", "modal");
            hyp.Attributes.Add("data-target", "#labresultModal");
            hyp.Attributes.Add("data-id", "0");
            hyp.Attributes.Add("data-evt", "add");
            hyp.Attributes.Add("data-name", "");
            spnbuttons.Controls.Add(hyp);
        }

        protected void btnSaveLabResult_Click(object sender, EventArgs e)
        {
            LabResult labresult = null;

            if (txtUID.Value == "0")
            {
                if (!IsLabResultUnique(txtDescription.Value, 0))
                {
                    divErrorDuplicate.Visible = true;

                    RenderItems();
                    return;
                }

                labresult = new LabResult { Description = txtDescription.Value };

                UnitOfWork.Repository<LabResult>().Save(labresult);
            }
            else
            {
                labresult = GetLabResult(Convert.ToInt32(txtUID.Value));

                if (labresult != null)
                {
                    if (!IsLabResultUnique(txtDescription.Value, labresult.Id))
                    {
                        divErrorDuplicate.Visible = true;

                        RenderItems();
                        return;
                    }

                    labresult.Description = txtDescription.Value;

                    UnitOfWork.Repository<LabResult>().Update(labresult);
                }
            }

            UnitOfWork.Complete();

            RenderItems();
            divstatus.Visible = true;
        }

        protected void btnDeleteLabResult_Click(object sender, EventArgs e)
        {
            LabResult labresult = null;

            if (txtUID.Value != "0") {
                labresult = GetLabResult(Convert.ToInt32(txtUID.Value));
            }

            if (labresult != null)
            {
                UnitOfWork.Repository<LabResult>().Delete(labresult);
                UnitOfWork.Complete();

                RenderItems();
            }
        }

        #region "EF"

        private LabResult GetLabResult(int id)
        {
            return UnitOfWork.Repository<LabResult>().Queryable().SingleOrDefault(l => l.Id == id);
        }

        private bool IsLabResultUnique(string labresult, int id)
        {
            int count = 0;
            if (id > 0) {
                count = UnitOfWork.Repository<LabResult>().Queryable().Where(l => l.Description == labresult && l.Id != id).Count();
            }
            else {
                count = UnitOfWork.Repository<LabResult>().Queryable().Where(l => l.Description == labresult).Count();
            }
            return (count == 0);
        }

        #endregion

    }
}