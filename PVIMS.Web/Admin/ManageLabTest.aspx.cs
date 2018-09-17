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
    public partial class ManageLabTest : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminLabTest");

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
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Loop through and render table
            foreach (var l in UnitOfWork.Repository<LabTest>().Queryable().OrderBy(l => l.Description))
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
                    Text = "Edit Test and Procedure"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#labtestModal");
                hyp.Attributes.Add("data-id", l.Id.ToString());
                hyp.Attributes.Add("data-evt", "edit");
                hyp.Attributes.Add("data-name", l.Description);

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    Text = "Delete Test and Procedure"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#labtestModal");
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
                Text = "Add Test and Procedure"
            };
            hyp.Attributes.Add("data-toggle", "modal");
            hyp.Attributes.Add("data-target", "#labtestModal");
            hyp.Attributes.Add("data-id", "0");
            hyp.Attributes.Add("data-evt", "add");
            hyp.Attributes.Add("data-name", "");
            spnbuttons.Controls.Add(hyp);
        }

        protected void btnSaveLabTest_Click(object sender, EventArgs e)
        {
            LabTest labtest = null;

            if (txtUID.Value == "0")
            {
                if (!IsLabTestUnique(txtDescription.Value, 0))
                {
                    divErrorDuplicate.Visible = true;

                    RenderItems();
                    return;
                }

                labtest = new LabTest { Description = txtDescription.Value, Active = true };

                UnitOfWork.Repository<LabTest>().Save(labtest);
            }
            else
            {
                labtest = GetLabTest(Convert.ToInt32(txtUID.Value));

                if (labtest != null)
                {
                    if (!IsLabTestUnique(txtDescription.Value, labtest.Id))
                    {
                        divErrorDuplicate.Visible = true;

                        RenderItems();
                        return;
                    }

                    labtest.Description = txtDescription.Value;

                    UnitOfWork.Repository<LabTest>().Update(labtest);
                }
            }

            UnitOfWork.Complete();

            RenderItems();
            divstatus.Visible = true;
        }

        protected void btnDeleteLabTest_Click(object sender, EventArgs e)
        {
            LabTest labtest = null;

            if (txtUID.Value != "0") {
                labtest = GetLabTest(Convert.ToInt32(txtUID.Value));
            }

            if (labtest != null)
            {
                UnitOfWork.Repository<LabTest>().Delete(labtest);
                UnitOfWork.Complete();

                RenderItems();
            }
        }

        #region "EF"

        private LabTest GetLabTest(int id)
        {
            return UnitOfWork.Repository<LabTest>().Queryable().SingleOrDefault(l => l.Id == id);
        }

        private bool IsLabTestUnique(string labtest, int id)
        {
            int count = 0;
            if (id > 0) {
                count = UnitOfWork.Repository<LabTest>().Queryable().Where(l => l.Description == labtest && l.Id != id).Count();
            }
            else {
                count = UnitOfWork.Repository<LabTest>().Queryable().Where(l => l.Description == labtest).Count();
            }
            return (count == 0);
        }

        #endregion

    }
}