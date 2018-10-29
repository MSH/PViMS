using System;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class ManageMedicine : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminMedicine");
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Medicines", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });

            if (!Page.IsPostBack) 
            {
                LoadDropDownList();
                RenderItems();
            }
            else {
                divstatus.Visible = false;
            }
        }

        protected void btnSaveMedication_Click(object sender, EventArgs e)
        {
            Medication medication = null;
            MedicationForm form = null;
            int selectedFormId = 0;

            if (txtUID.Value == "0")
            {
                if (!IsMedicationUnique(txtDrugName.Value, Convert.ToInt32(txtPackSize.Value), txtStrength.Value, 0))
                {
                    divErrorDuplicate.Visible = true;

                    RenderItems();
                    return;
                }

                if (Int32.TryParse(ddlMedicationForm.SelectedValue, out selectedFormId)) {
                    form = GetMedicationForm(selectedFormId);
                }

                medication = new Medication { DrugName = txtDrugName.Value, Active = true, PackSize = Convert.ToInt32(txtPackSize.Value), Strength = txtStrength.Value, CatalogNo = txtCatalog.Value, MedicationForm = form };

                UnitOfWork.Repository<Medication>().Save(medication);
            }
            else
            {
                medication = GetMedication(Convert.ToInt32(txtUID.Value));

                if (medication != null)
                {
                    if (!IsMedicationUnique(txtDrugName.Value, Convert.ToInt32(txtPackSize.Value), txtStrength.Value, medication.Id))
                    {
                        divErrorDuplicate.Visible = true;

                        RenderItems();
                        return;
                    }

                    medication.DrugName = txtDrugName.Value;
                    medication.PackSize = Convert.ToInt32(txtPackSize.Value);
                    medication.Strength = txtStrength.Value;
                    medication.CatalogNo = txtCatalog.Value;
                    medication.MedicationForm = GetMedicationForm(Convert.ToInt32(ddlMedicationForm.SelectedValue));

                    UnitOfWork.Repository<Medication>().Update(medication);
                }
            }

            UnitOfWork.Complete();

            RenderItems();
            divstatus.Visible = true;
        }

        protected void btnDeleteMedication_Click(object sender, EventArgs e)
        {
            Medication medication = null;

            if (txtUID.Value != "0") {
                medication = GetMedication(Convert.ToInt32(txtUID.Value));
            }

            if (medication != null)
            {
                UnitOfWork.Repository<Medication>().Delete(medication);
                UnitOfWork.Complete();

                RenderItems();
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
            foreach (var m in UnitOfWork.Repository<Medication>().Queryable().OrderBy(m => m.DrugName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = m.DrugName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = m.PackSize.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = m.Strength;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = m.CatalogNo;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = m.MedicationForm != null ? m.MedicationForm.Description : " ** NOT ASSIGNED ** ";
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
                    Text = "Edit Medication"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#medicationModal");
                hyp.Attributes.Add("data-id", m.Id.ToString());
                hyp.Attributes.Add("data-evt", "edit");
                hyp.Attributes.Add("data-drugname", m.DrugName);
                hyp.Attributes.Add("data-packsize", m.PackSize.ToString());
                hyp.Attributes.Add("data-strength", m.Strength);
                hyp.Attributes.Add("data-catalog", m.CatalogNo);
                hyp.Attributes.Add("data-form", m.MedicationForm != null ? m.MedicationForm.Id.ToString() : "0");

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                li = new HtmlGenericControl("li");
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    Text = "Delete Medication"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#medicationModal");
                hyp.Attributes.Add("data-id", m.Id.ToString());
                hyp.Attributes.Add("data-evt", "delete");
                hyp.Attributes.Add("data-drugname", m.DrugName);
                hyp.Attributes.Add("data-packsize", m.PackSize.ToString());
                hyp.Attributes.Add("data-strength", m.Strength);
                hyp.Attributes.Add("data-catalog", m.CatalogNo);
                hyp.Attributes.Add("data-form", m.MedicationForm != null ? m.MedicationForm.Id.ToString() : "0");
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
                Text = "Add Medication"
            };
            hyp.Attributes.Add("data-toggle", "modal");
            hyp.Attributes.Add("data-target", "#medicationModal");
            hyp.Attributes.Add("data-id", "0");
            hyp.Attributes.Add("data-evt", "add");
            hyp.Attributes.Add("data-drugname", "");
            hyp.Attributes.Add("data-packsize", "0");
            hyp.Attributes.Add("data-strength", "");
            hyp.Attributes.Add("data-catalog", "");
            hyp.Attributes.Add("data-form", "0");

            spnbuttons.Controls.Add(hyp);
        }

        private void LoadDropDownList()
        {
            ListItem item;
            var formList = (from mf in UnitOfWork.Repository<MedicationForm>().Queryable() orderby mf.Description ascending select mf).ToList();

            item = new ListItem();
            item.Text = " ";
            item.Value = "0";
            ddlMedicationForm.Items.Add(item);

            foreach (MedicationForm form in formList)
            {
                item = new ListItem();
                item.Text = form.Description;
                item.Value = form.Id.ToString();
                ddlMedicationForm.Items.Add(item);
            }
        }

        #region "EF"

        private Medication GetMedication(int id)
        {
            return UnitOfWork.Repository<Medication>().Queryable().SingleOrDefault(m => m.Id == id);
        }

        private MedicationForm GetMedicationForm(int id)
        {
            return UnitOfWork.Repository<MedicationForm>().Queryable().SingleOrDefault(mf => mf.Id == id);
        }

        private bool IsMedicationUnique(string drugname, int packsize, string strength, int id)
        {
            int count = 0;
            if (id > 0) {
                count = UnitOfWork.Repository<Medication>().Queryable().Where(m => m.DrugName == drugname && m.PackSize == packsize && m.Strength == strength && m.Id != id).Count();
            }
            else {
                count = UnitOfWork.Repository<Medication>().Queryable().Where(m => m.DrugName == drugname && m.PackSize == packsize && m.Strength == strength).Count();
            }
            return (count == 0);
        }

        #endregion
    }
}