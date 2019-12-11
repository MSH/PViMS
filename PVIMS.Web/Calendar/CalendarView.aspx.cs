using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class CalendarView : MainPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Appointments", SubTitle = "", Icon = "fa fa-calendar fa-fw" });
                Master.SetMenuActive("CalendarView");

                // reset context 
                var user = UnitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
                if(user.CurrentContext != "0")
                {
                    user.CurrentContext = "0";
                    UnitOfWork.Repository<User>().Update(user);

                    var url = @"\Calendar\CalendarView.aspx";
                    Response.Redirect(url);
                }

                txtCurrentDate.Value = DateTime.Today.ToString("yyyy-MM-dd");
                RenderAppointments();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DateTime dttemp;

            lblCurrentDate.Attributes.Remove("class");
            lblCurrentDate.Attributes.Add("class", "input");

            bool err = false;
            divError.Visible = false;

            if (DateTime.TryParse(txtCurrentDate.Value, out dttemp))
            {
                dttemp = Convert.ToDateTime(txtCurrentDate.Value);
                if (dttemp > DateTime.Today.AddYears(10)) 
                {
                    lblCurrentDate.Attributes.Remove("class");
                    lblCurrentDate.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Appointment Date cannot be more than 10 years in the future";
                    lblCurrentDate.Controls.Add(errorMessageDiv);

                    err = true;
                }
                if (dttemp < DateTime.Today.AddYears(-120)) 
                {
                    lblCurrentDate.Attributes.Remove("class");
                    lblCurrentDate.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Appointment Date cannot be so far in the past";
                    lblCurrentDate.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }
            else 
            {
                lblCurrentDate.Attributes.Remove("class");
                lblCurrentDate.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Appointment Date has an invalid date format";
                lblCurrentDate.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (err)
            {
                divError.Visible = true;
                return;
            };

            dt_basic.Visible = true;
            dt_basic_2.Visible = false;
            RenderAppointments();

            // Check if holiday
            var holiday = GetHoliday(Convert.ToDateTime(txtCurrentDate.Value));
            if (holiday != null)
            {
                divHoliday.Visible = true;
                divHolidayMessage.InnerText = "Public Holiday ~ " + holiday.Description;
            }
        }

        private void RenderAppointments()
        {
            TableRow row;
            TableCell cell;

            HyperLink hyp;
            Panel pnl;
            Label lbl;
            HtmlGenericControl btn;
            HtmlGenericControl ul;
            HtmlGenericControl li;

            Encounter encounter = null;

            DateTime searchDate = Convert.ToDateTime(txtCurrentDate.Value);
            string activity = string.Empty;
            string created = string.Empty;
            string updated = string.Empty;

            foreach (var app in UnitOfWork.Repository<Appointment>().Queryable().Include("Patient").Where(a => a.AppointmentDate == searchDate && a.Cancelled == false && !a.Archived).ToList())
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = app.Patient.FullName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = app.Reason;
                row.Cells.Add(cell);

                encounter = null;
                if(app.DNA) {
                    activity = @"Appointment has been marked as <span class=""label label-warning"">Did Not Arrive</span>";
                }
                else
                {
                    encounter = app.GetEncounter();
                    if (encounter == null) {
                        activity = @"Patient has not arrived yet...";
                    }
                    else {
                        activity = @"Patient arrived on " + encounter.EncounterDate.ToString("yyyy-MM-dd");
                    }
                }
                cell = new TableCell();
                cell.Text = activity;
                row.Cells.Add(cell);

                created = String.Format("Created by {0} on {1} ...", "UNKNOWN", app.Created.ToString("yyyy-MM-dd"));
                if (app.LastUpdated != null) {
                    updated = String.Format("Updated by {0} on {1} ...", "UNKNOWN", Convert.ToDateTime(app.LastUpdated).ToString("yyyy-MM-dd"));
                }
                else {
                    updated = "NOT UPDATED";
                }

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
                    NavigateUrl = "/Patient/PatientView.aspx?pid=" + app.Patient.Id.ToString(),
                    Text = "View Patient"
                };

                li.Controls.Add(hyp);
                ul.Controls.Add(li);

                // Check in if necessary
                encounter = app.Patient.GetEncounterForAppointment(app);
                if(encounter != null)
                {
                    li = new HtmlGenericControl("li");
                    var verEncounterHyperLink = new HyperLink()
                    {
                        NavigateUrl = string.Format("/Encounter/ViewEncounter/{0}", encounter.Id.ToString()),
                        Text = "View Encounter"
                    };
                    li.Controls.Add(verEncounterHyperLink);
                    ul.Controls.Add(li);
                }
                else
                {
                    li = new HtmlGenericControl("li");
                    var addEncounterHyperLink = new HyperLink()
                    {
                        NavigateUrl = string.Format("/Encounter/AddEncounter?pid={0}&aid={1}&cancelRedirectUrl={2}", "0", app.Id.ToString(), "/Calendar/CalendarView.aspx"),
                        Text = "Open Encounter"
                    };
                    li.Controls.Add(addEncounterHyperLink);
                    ul.Controls.Add(li);
                }

                // DNA menu if necessary
                if (app.AppointmentDate < DateTime.Today.AddDays(-3) && app.DNA == false && app.Cancelled == false && encounter == null)
                {
                    li = new HtmlGenericControl("li");
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "#",
                        Text = "Did Not Arrive"
                    };
                    hyp.Attributes.Add("data-toggle", "modal");
                    hyp.Attributes.Add("data-target", "#appointmentModal");
                    hyp.Attributes.Add("data-id", app.Id.ToString());
                    hyp.Attributes.Add("data-evt", "dna");
                    hyp.Attributes.Add("data-date", app.AppointmentDate.ToString("yyyy-MM-dd"));
                    hyp.Attributes.Add("data-reason", app.Reason);
                    hyp.Attributes.Add("data-cancelled", app.Cancelled ? "Yes" : "No");
                    hyp.Attributes.Add("data-cancelledreason", app.CancellationReason);
                    hyp.Attributes.Add("data-created", created);
                    hyp.Attributes.Add("data-updated", updated);
                    li.Controls.Add(hyp);
                    ul.Controls.Add(li);
                }

                pnl.Controls.Add(ul);

                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
            }

            if (dt_basic.Rows.Count == 1)
            {
                spnNoRows.InnerText = "No matching records found...";
                spnNoRows.Visible = true;
                spnRows.Visible = false;
            }
            else
            {
                spnRows.InnerText = (dt_basic.Rows.Count - 1).ToString() + " row(s) matching criteria found...";
                spnRows.Visible = true;
                spnNoRows.Visible = false;
            }
        }

        protected void btnDNAAppointment_Click(object sender, EventArgs e)
        {
            Appointment appointment = null;

            if (txtAppointmentUID.Value != "0")
            {
                appointment = GetAppointment(Convert.ToInt32(txtAppointmentUID.Value));
            }

            if (appointment != null)
            {
                appointment.DNA = true;
                UnitOfWork.Complete();

                RenderAppointments();
            }
        }

        #region "EF"

        private Appointment GetAppointment(int id)
        {
            return UnitOfWork.Repository<Appointment>().Queryable().Include("Patient").SingleOrDefault(u => u.Id == id);
        }

        private Holiday GetHoliday(DateTime searchDate)
        {
            return UnitOfWork.Repository<Holiday>().Queryable().SingleOrDefault(u => u.HolidayDate == searchDate);
        }

        #endregion
    }
}