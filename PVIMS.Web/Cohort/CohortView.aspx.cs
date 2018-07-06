using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;

namespace PVIMS.Web
{
    public partial class CohortView : MainPageBase
    {
        private int _id;
        private CohortGroup _cohort;

        public IWorkFlowService _workFlowService { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                _id = Convert.ToInt32(Request.QueryString["id"]);
                _cohort = GetCohortGroup(_id);

                txtCohortName.Value = _cohort.CohortName;
                txtCohortCode.Value = _cohort.CohortCode;
            }
            else {
                throw new Exception("id not passed as parameter");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.MainMenu.SetActive("Cohort");

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
            foreach (var enrollment in _cohort.CohortGroupEnrolments.Where( x=>!x.Archived || !x.Patient.Archived))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = enrollment.Patient.FullName;
                row.Cells.Add(cell);

                var facility = enrollment.Patient.GetCurrentFacility();
                if(facility != null)
                {
                    cell = new TableCell();
                    cell.Text = facility.Facility.FacilityName + " " + facility.Facility.FacilityCode;
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = @"<span class=""label label-warning"">Not Set</span>";
                    row.Cells.Add(cell);
                }

                cell = new TableCell();
                if (enrollment.Patient.Age < 19) {
                    cell.Text = String.Format(@"{0} <span class=""badge bg-color-blueLight"">{1}</span>", enrollment.Patient.DateOfBirth != null ? Convert.ToDateTime(enrollment.Patient.DateOfBirth).ToString("yyyy-MM-dd") : "", enrollment.Patient.Age.ToString());
                }
                else
                {
                    cell.Text = String.Format(@"{0} <span class=""badge bg-color-blueDark"">{1}</span>", Convert.ToDateTime(enrollment.Patient.DateOfBirth).ToString("yyyy-MM-dd"), enrollment.Patient.Age.ToString());
                }
                row.Cells.Add(cell);

                var lastEncounter = enrollment.Patient.LastEncounterDate();
                cell = new TableCell();
                if (lastEncounter != null) {
                    cell.Text = Convert.ToDateTime(lastEncounter).ToString("yyyy-MM-dd");
                }
                else {
                    cell.Text = @"<span class=""label label-info"">No Encounters</span>";
                }
                row.Cells.Add(cell);


                var element = GetDatasetElement("Weight (kg)");
                if(element != null)
                {
                    cell = new TableCell();
                    cell.Text = GetCurrentValue(enrollment.Patient, element);
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }

                var adverse = _workFlowService.GetCurrentAdverseReaction(enrollment.Patient);
                if(adverse != null)
                {
                    cell = new TableCell();
                    cell.Text = adverse.MedDraTerm;
                    row.Cells.Add(cell);
                }
                else
                {
                    cell = new TableCell();
                    cell.Text = "NO CONFIRMED REACTIONS";
                    row.Cells.Add(cell);
                }

                cell = new TableCell();
                hyp = new HyperLink()
                {
                    NavigateUrl = "/Patient/PatientView.aspx?pid=" + enrollment.Patient.Id.ToString() + "&returnCohort=" + _id.ToString(),
                    CssClass = "btn btn-default",
                    Text = "View Patient"
                };
                cell.Controls.Add(hyp);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }
        }

        private string GetCurrentValue(Patient patient, DatasetElement element)
        {
            if (patient.Encounters.Count == 0) {
                return "NO VALUE";
            }
            else
            {
                var encounter = patient.GetCurrentEncounter();

                // Get Dataset Instance for encounter
                var instance = GetDatasetInstance(encounter.Id);
                if (instance != null)
                {
                    var value = instance.GetInstanceValue(element);
                    return value;
                }
                else {
                    return "NO VALUE";
                }
            }
        }

        #region "EF"

        private CohortGroup GetCohortGroup(int id)
        {
            return UnitOfWork.Repository<CohortGroup>().Queryable().SingleOrDefault(cg => cg.Id == id);
        }

        private DatasetInstance GetDatasetInstance(int contextId)
        {
            return UnitOfWork.Repository<DatasetInstance>().Queryable().SingleOrDefault(di => di.ContextID == contextId && di.Dataset.ContextType.Description == "Encounter");
        }

        private DatasetElement GetDatasetElement(string element)
        {
            return UnitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(de => de.ElementName == element);
        }

        #endregion

    }
}