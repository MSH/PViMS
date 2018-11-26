using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;

namespace PVIMS.Web
{
    public partial class CohortView : MainPageBase
    {
        private int _id;
        private CohortGroup _cohort;
        private CohortSummary _cohortSummary;

        public IPatientService _patientService { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                _cohort = GetCohortGroup(_id);

                _cohortSummary = new CohortSummary()
                {
                    CohortGroupId = _id
                };

                txtCohortName.Value = _cohort.CohortName;
                txtCohortCode.Value = _cohort.CohortCode;
                txtCohortCondition.Value = _cohort.Condition != null ? _cohort.Condition.Description : "";
            }
            else {
                throw new Exception("id not passed as parameter");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Cohort View", SubTitle = "", Icon = "fa fa-cogs fa-fw" });
            Master.SetMenuActive("CohortView");

            if (!Page.IsPostBack) {
                RenderEnrolments();
            }
        }

        private void RenderEnrolments()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;

            // Loop through and render table
            foreach (var enrollment in _cohort.CohortGroupEnrolments.Where( x=>!x.Archived && !x.Patient.Archived))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = enrollment.Patient.FullName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = enrollment.Patient.CurrentFacilityName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = String.Format(@"{0} <span class=""{2}"">{1}</span>", enrollment.Patient.DateOfBirth != null ? Convert.ToDateTime(enrollment.Patient.DateOfBirth).ToString("yyyy-MM-dd") : "", enrollment.Patient.Age.ToString(), enrollment.Patient.Age < 19 ? "badge bg-color-blueLight" : "badge bg-color-blueDark");
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = enrollment.Patient.LatestEncounterDate.HasValue ? Convert.ToDateTime(enrollment.Patient.LatestEncounterDate).ToString("yyyy-MM-dd") : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = _patientService.GetCurrentElementValueForPatient(enrollment.Patient.Id, "Chronic Treatment", "Weight (kg)").Value;
                row.Cells.Add(cell);

                var patientEventSummary = enrollment.Patient.GetEventSummary();

                cell = new TableCell();
                cell.Text = patientEventSummary.NonSeriesEventCount.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = patientEventSummary.SeriesEventCount.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "/Patient/PatientView.aspx?pid=" + enrollment.Patient.Id.ToString() + "&returnCohort=" + _id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "View Patient"
                };
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                _cohortSummary.PatientCount += 1;
                _cohortSummary.SeriesEventCount += patientEventSummary.SeriesEventCount;
                _cohortSummary.NonSeriesEventCount += patientEventSummary.NonSeriesEventCount;

                dt_basic.Rows.Add(row);
            }

            txtNonSeriousCount.Value = _cohortSummary.NonSeriesEventCount.ToString();
            txtSeriousCount.Value = _cohortSummary.SeriesEventCount.ToString();
            txtPatientCount.Value = _cohortSummary.PatientCount.ToString();
        }

        #region "EF"

        private CohortGroup GetCohortGroup(int id)
        {
            return UnitOfWork.Repository<CohortGroup>().Queryable().SingleOrDefault(cg => cg.Id == id);
        }

        private DatasetElement GetDatasetElement(string element)
        {
            return UnitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(de => de.ElementName == element);
        }

        #endregion

    }
}