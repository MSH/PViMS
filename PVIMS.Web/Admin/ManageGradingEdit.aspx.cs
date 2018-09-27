using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public partial class ManageGradingEdit : MainPageBase
    {
        private int _id;
        private MedDRAScale _scale;

        private enum FormMode { EditMode = 1, AddMode = 2 };
        private FormMode _formMode = FormMode.EditMode;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                if (_id > 0) {
                    _scale = UnitOfWork.Repository<MedDRAScale>().Queryable().SingleOrDefault(s => s.Id == _id);
                }
                else
                {
                    _scale = null;
                    _formMode = FormMode.AddMode;
                }
            }
            else {
                throw new Exception("id not passed as parameter");
            }

            LoadScaleDropDownList();
            RenderButtons();
            ToggleView();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            divError.Visible = false;
            spnErrors.InnerHtml = "";

            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Scale Gradings", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaPageId = 0 });
            Master.SetMenuActive("AdminGrading");

            if (!Page.IsPostBack)
            {
                if (_scale != null) {
                    RenderScale();
                }
            }
        }

        #region "Rendering"

        private void RenderScale()
        {
            txtUID.Value = _scale.Id.ToString();
            txtScale.Value = _scale.GradingScale.Value;
            txtMeddraTerm.Value = _scale.TerminologyMedDra.MedDraTerm;

            RenderGrades();
        }

        private void RenderGrades()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Label lbl;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            // Clear existing rows as function can be called second timewithout reload
            var delete = new ArrayList();
            foreach (TableRow temprow in dt_1.Rows)
            {
                if (temprow.Cells[0].Text != "Grade")
                    delete.Add(temprow);
            }
            foreach (TableRow temprow in delete) {
                dt_1.Rows.Remove(temprow);
            }

            foreach (var grade in _scale.Grades.OrderBy(c => c.Grade))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = grade.Grade;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = grade.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    CssClass = "btn btn-default",
                    Text = "Edit Grade"
                };
                hyp.Attributes.Add("data-toggle", "modal");
                hyp.Attributes.Add("data-target", "#itemModal");
                hyp.Attributes.Add("data-id", grade.Id.ToString());
                hyp.Attributes.Add("data-desc", grade.Description);
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                dt_1.Rows.Add(row);
            }
        }

        private void RenderButtons()
        {
            Button btn;
            HyperLink hyp;

            var url = "";

            switch (_formMode)
            {
                case FormMode.EditMode:
                    spnButtons.Controls.Clear();

                    break;

                case FormMode.AddMode:
                    btn = new Button();
                    btn.ID = "btnSave";
                    btn.CssClass = "btn btn-primary";
                    btn.Text = "Save";
                    btn.Click += btnSave_Click;
                    spnButtons.Controls.Add(btn);

                    hyp = new HyperLink()
                    {
                        ID = "btnCancel",
                        NavigateUrl = "ManageGrading.aspx",
                        CssClass = "btn btn-default",
                        Text = "Cancel"
                    };
                    spnButtons.Controls.Add(hyp);

                    break;

                default:
                    break;
            };

            spnButtons.Visible = true;

        }

        private void ToggleView()
        {
            switch (_formMode)
            {
                case FormMode.EditMode:
                    divGrades.Visible = true;
                    divAdd.Visible = false;
                    divView.Visible = true;

                    break;

                case FormMode.AddMode:
                    divGrades.Visible = false;
                    divAdd.Visible = true;
                    divView.Visible = false;

                    break;

                default:
                    break;
            };
        }

        private void LoadScaleDropDownList()
        {
            ListItem item;

            foreach (SelectionDataItem sel in UnitOfWork.Repository<SelectionDataItem>().Queryable().Where(s => s.AttributeKey == "Severity Grading Scale").OrderBy(s => s.Id))
            {
                item = new ListItem();
                item.Text = sel.Value;
                item.Value = sel.Id.ToString();
                ddlScale.Items.Add(item);
            }
        }

        #endregion

        #region "Events"

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string err = "<ul>";

            if (lstTermResult.SelectedIndex == -1) {
                err += "<li>Please select an adverse event...</li>";
            }

            if (err != "<ul>")
            {
                err += "</ul>";
                divError.Visible = true;
                spnErrors.InnerHtml = err;
                return;
            }

            var id = Convert.ToInt32(lstTermResult.SelectedItem.Value);
            TerminologyMedDra sourceTerm = UnitOfWork.Repository<TerminologyMedDra>().Get(id);

            // Validation
            if (_formMode == FormMode.AddMode)
            {

                if (!IsScaleUnique(sourceTerm, Convert.ToInt32(ddlScale.Value)))
                {
                    err += "<li>Scale has already been added for this adverse event...</li>";
                }

                if (err != "<ul>")
                {
                    err += "</ul>";
                    divError.Visible = true;
                    spnErrors.InnerHtml = err;
                    return;
                }

                if (_scale == null)
                {
                    id = Convert.ToInt32(ddlScale.Value);
                    var scale = UnitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(s => s.Id == id);

                    _scale = new MedDRAScale { TerminologyMedDra = sourceTerm, GradingScale = scale };
                    UnitOfWork.Repository<MedDRAScale>().Save(_scale);

                    for (var i = 1; i < 6; i++)
                    {
                        var grading = new MedDRAGrading() { Scale = _scale, Grade = "Grade " + i.ToString(), Description = "** NOT DEFINED **" };
                        UnitOfWork.Repository<MedDRAGrading>().Save(grading);
                    }
                }
            }

            var url = String.Format("ManageGradingEdit.aspx?id=" + _scale.Id.ToString());
            Response.Redirect(url);
        }

        protected void btnSaveItem_Click(object sender, EventArgs e)
        {
            MedDRAGrading grade = null;

            if (txtItemUID.Value != "0")
            {
                var id = Convert.ToInt32(txtItemUID.Value);
                grade = UnitOfWork.Repository<MedDRAGrading>().Queryable().SingleOrDefault(mg => mg.Id == id);

                if (grade != null)
                {
                    grade.Description = txtDescription.Value;

                    UnitOfWork.Repository<MedDRAGrading>().Update(grade);
                    UnitOfWork.Complete();

                    RenderGrades();
                    divstatus.Visible = true;
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lstTermResult.Items.Clear();

            var terms = UnitOfWork.Repository<TerminologyMedDra>().Queryable().Where(u => u.MedDraTerm.Contains(txtTerm.Value) && u.MedDraTermType == ddlTermType.SelectedValue).ToList();

            foreach (var term in terms)
            {
                lstTermResult.Items.Add(new ListItem() { Text = term.MedDraTerm, Value = term.Id.ToString() });
            }
        }

        #endregion

        #region "EF"

        private bool IsScaleUnique(TerminologyMedDra term, int scaleId)
        {
            int count = 0;
            count = UnitOfWork.Repository<MedDRAScale>().Queryable().Where(s => s.TerminologyMedDra.Id == term.Id && s.GradingScale.Id == scaleId).Count();
            return (count == 0);
        }

        #endregion

    }
}