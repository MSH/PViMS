using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using VPS.Common.Repositories;
using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Entities.EF;

using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;

namespace PVIMS.Web
{
    public partial class EncounterSearch : MainPageBase
    {
        private User _user;

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Encounter Search", SubTitle = "", Icon = "fa fa-file-text-o fa-fw" });

            txtSearchFrom.Value = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            txtSearchTo.Value = DateTime.Today.ToString("yyyy-MM-dd");
            
            LoadCustomDropDownList();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = UnitOfWork.Repository<User>().Queryable().Include(u => u.Facilities).SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);

            //EnsureBookmarkableUri();

            Master.SetMenuActive("EncounterView");

            if (!Page.IsPostBack)
            {
                LoadDropDownList();
            }

            //ddlFacility.SelectedValue = Request["facility"];
            //ddlCriteria.SelectedValue = Request["criteria"];
            //txtUniqueID.Value = Request["uid"];
            //txtFirstName.Value = Request["fname"];
            //txtSurname.Value = Request["sname"];
            
            //DateTime temp;
            //txtSearchFrom.Value = DateTime.TryParse(Request["searchfrom"], out temp) ? Convert.ToDateTime(Request["searchfrom"]).ToString("yyyy-MM-dd") : DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            //txtSearchTo.Value = DateTime.TryParse(Request["searchto"], out temp) ? Convert.ToDateTime(Request["searchto"]).ToString("yyyy-MM-dd") : DateTime.Today.ToString("yyyy-MM-dd");

            //if (!String.IsNullOrWhiteSpace(Request["facility"]))
            //{
            //    LoadDataForSearchCriteria();
            //}
        }

        private void EnsureBookmarkableUri()
        {
            if (Page.IsPostBack)
            {
                Response.Redirect(Request.Url.AbsolutePath + "?facility=" + ddlFacility.SelectedValue + "&uid=" + txtUniqueID.Value + "&fname=" + txtFirstName.Value + "&sname=" + txtSurname.Value + "&searchfrom=" + txtSearchFrom.Value + "&searchto=" + txtSearchTo.Value + "&criteria=" + ddlCriteria.SelectedValue);
            }
        }

        private void LoadDataForSearchCriteria()
        {
            this.btnSubmit_Click(null, EventArgs.Empty);
        }

        private void LoadDropDownList()
        {
            ListItem item;
            var facilityList = (from f in UnitOfWork.Repository<Facility>().Queryable() orderby f.FacilityName ascending select f).ToList();

            foreach (Facility fac in facilityList)
            {
                if(_user.HasFacility(fac.Id))
                {
                    item = new ListItem();
                    item.Text = fac.FacilityName;
                    item.Value = fac.Id.ToString();
                    ddlFacility.Items.Add(item);
                }
            }

        }

        private void LoadCustomDropDownList()
        {
            divCustomSearch.Visible = false;

            ListItem item;
            var customList = (from c in UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable() where c.IsSearchable == true && c.ExtendableTypeName == "Patient" orderby c.AttributeKey ascending select c).ToList();

            foreach (CustomAttributeConfiguration ca in customList)
            {
                item = new ListItem();
                item.Text = ca.AttributeKey;
                item.Value = ca.Id.ToString();
                ddlCustomAttribute.Items.Add(item);

                RenderCustomValue(ca);
            }

            if (customList.Count() > 0)
            {
                divCustomSearch.Visible = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            switch (ddlCriteria.SelectedValue)
            {
                case "1": // All encounter
                    HandleEncounter();
                    break;

                case "2": // All appointment
                case "3": // Appointments with missed encounter
                case "4": // Appointments with Did Not Arrive Status
                case "5": // Appointments with encounter
                    HandleAppointment();
                    break;

                default:
                    break;
            }

        }

        private string Escape(string uri)
        {
            return uri.Replace("&", "%26").Replace("=", "%3D").Replace("?", "%3F");
        }

        protected void ddlCustomAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(ddlCustomAttribute.SelectedValue);
            if (id == 0)
            {
                divCustomValue.Visible = false;
                return;
            };

            divCustomValue.Visible = true;
            var con = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.Id == id);
        }

        private void HandleEncounter()
        {
            TableRow row;
            TableCell cell;

            //string facility = Request.QueryString["facility"];
            //string uid = Request.QueryString["uid"];
            //string fname = Request.QueryString["fname"];
            //string sname = Request.QueryString["sname"];
            //string searchfrom = Request.QueryString["searchfrom"];
            //string searchto = Request.QueryString["searchto"];
            //string criteria = Request.QueryString["criteria"];

            string facility = ddlFacility.Text;
            int facilityId;
            int.TryParse(ddlFacility.SelectedValue, out facilityId);
            string uid = txtUniqueID.Value;
            string fname = txtFirstName.Value;
            string sname = txtSurname.Value;
            string searchfrom = txtSearchFrom.Value;
            string searchto = txtSearchTo.Value;
            string criteria = ddlCriteria.Text;

            // Get custom attribute search value
            string custom = "";
            string path = "";
            var id = Convert.ToInt32(ddlCustomAttribute.SelectedValue);
            if (id > 0)
            {
                DropDownList ddl;
                TextBox txt;

                var con = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.Id == id);
                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.String:
                        txt = (TextBox)spnCustomValue.Controls[0].Controls[0];
                        custom = txt.Text;
                        path = @"CustomStringAttribute";
                        break;

                    case CustomAttributeType.DateTime:
                        txt = (TextBox)spnCustomValue.Controls[0].Controls[0];
                        custom = txt.Text;
                        path = @"CustomStringAttribute";
                        break;

                    case CustomAttributeType.Numeric:
                        txt = (TextBox)spnCustomValue.Controls[0].Controls[0];
                        custom = txt.Text;
                        path = @"CustomStringAttribute";
                        break;

                    case CustomAttributeType.Selection:
                        ddl = (DropDownList)spnCustomValue.Controls[0].Controls[0];
                        custom = ddl.SelectedValue;
                        path = @"CustomSelectionAttribute";
                        break;
                }
            };

            bool err = false;

            divError.Visible = false;
            spnNoRows.Visible = false;
            spnRows.Visible = false;

            lblUniqueID.Attributes.Remove("class");
            lblUniqueID.Attributes.Add("class", "input");
            lblSearchFrom.Attributes.Remove("class");
            lblSearchFrom.Attributes.Add("class", "input");
            lblSearchTo.Attributes.Remove("class");
            lblSearchTo.Attributes.Add("class", "input");
            lblFirstName.Attributes.Remove("class");
            lblFirstName.Attributes.Add("class", "input");
            lblSurname.Attributes.Remove("class");
            lblSurname.Attributes.Add("class", "input");
            lblFacility.Attributes.Remove("class");
            lblFacility.Attributes.Add("class", "input");

            DateTime dttemp;

            // Validation
            if (facility == "0" && String.IsNullOrWhiteSpace(uid) && String.IsNullOrWhiteSpace(fname) && String.IsNullOrWhiteSpace(sname) && (String.IsNullOrWhiteSpace(searchfrom) || String.IsNullOrWhiteSpace(searchto)))
            {
                lblUniqueID.Attributes.Remove("class");
                lblUniqueID.Attributes.Add("class", "input state-error");
                lblSearchFrom.Attributes.Remove("class");
                lblSearchFrom.Attributes.Add("class", "input state-error");
                lblSearchTo.Attributes.Remove("class");
                lblSearchTo.Attributes.Add("class", "input state-error");
                lblFirstName.Attributes.Remove("class");
                lblFirstName.Attributes.Add("class", "input state-error");
                lblSurname.Attributes.Remove("class");
                lblSurname.Attributes.Add("class", "input state-error");
                lblFacility.Attributes.Remove("class");
                lblFacility.Attributes.Add("class", "input state-error");

                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "At least one search criteria must be selected";
                lblUniqueID.Controls.Add(errorMessageDiv);

                err = true;
            }

            int temp;
            if (uid != "" && !int.TryParse(uid, out temp))
            {
                lblUniqueID.Attributes.Remove("class");
                lblUniqueID.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Unique ID must be numeric";
                lblUniqueID.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (!String.IsNullOrWhiteSpace(searchfrom) && !String.IsNullOrWhiteSpace(searchto))
            {
                if (DateTime.TryParse(searchfrom, out dttemp))
                {
                    dttemp = Convert.ToDateTime(searchfrom);
                    if (dttemp > DateTime.Today)
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From cannot be after current date";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From cannot be so far in the past";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblSearchFrom.Attributes.Remove("class");
                    lblSearchFrom.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Search From has an invalid date format";
                    lblSearchFrom.Controls.Add(errorMessageDiv);

                    err = true;
                }

                if (DateTime.TryParse(searchto, out dttemp))
                {
                    dttemp = Convert.ToDateTime(searchto);
                    if (dttemp > DateTime.Today)
                    {
                        lblSearchTo.Attributes.Remove("class");
                        lblSearchTo.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search To cannot be after current date";
                        lblSearchTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblSearchTo.Attributes.Remove("class");
                        lblSearchTo.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search To cannot be so far in the past";
                        lblSearchTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblSearchTo.Attributes.Remove("class");
                    lblSearchTo.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Search To has an invalid date format";
                    lblSearchTo.Controls.Add(errorMessageDiv);

                    err = true;
                }

                if (DateTime.TryParse(searchfrom, out dttemp) && DateTime.TryParse(searchto, out dttemp))
                {
                    if (Convert.ToDateTime(searchfrom) > Convert.ToDateTime(searchto))
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From must be before Search To";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        lblSearchTo.Attributes.Remove("class");
                        lblSearchTo.Attributes.Add("class", "input state-error");
                        errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search To must be after Search From";
                        lblSearchTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(fname))
            {
                if (Regex.Matches(fname, @"[a-zA-Z']").Count < fname.Length)
                {
                    lblFirstName.Attributes.Remove("class");
                    lblFirstName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "First Name contains invalid characters (Enter A-Z, a-z)";
                    lblFirstName.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }
            if (!String.IsNullOrWhiteSpace(sname))
            {
                if (Regex.Matches(sname, @"[a-zA-Z]").Count < sname.Length)
                {
                    lblSurname.Attributes.Remove("class");
                    lblSurname.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Last Name contains invalid characters (Enter A-Z, a-z)";
                    lblSurname.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }
            if (_user.Facilities.Count == 0) 
            {
                lblFacility.Attributes.Remove("class");
                lblFacility.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "User must have view permissions to at least one facility";
                lblFacility.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (err)
            {
                divError.Visible = true;
                return;
            };

            var encounterQuery = UnitOfWork.Repository<Encounter>().Queryable().Where( x=>!x.Archived && !x.Patient.Archived);

            if (!String.IsNullOrWhiteSpace(facility))
            {
                if (facility != "All Facilities") {
                    encounterQuery = encounterQuery.Where(ef => ef.Patient.PatientFacilities.OrderByDescending(pf1 => pf1.EnrolledDate).Take(1).Any(f => f.Facility.Id == facilityId));
                }
            };

            if (!String.IsNullOrWhiteSpace(uid))
            {
                var tid = Convert.ToInt32(uid);
                encounterQuery = encounterQuery.Where(ef => ef.Patient.Id == tid);
            }

            if (!String.IsNullOrWhiteSpace(fname))
            {
                if(fname.Length < 3)
                {
                    encounterQuery = encounterQuery.Where(ef => ef.Patient.FirstName.StartsWith(fname.Trim()));
                }
                else
                {
                    encounterQuery = encounterQuery.Where(ef => ef.Patient.FirstName.Contains(fname.Trim()));
                }
            }

            if (!String.IsNullOrWhiteSpace(sname))
            {
                if (sname.Length < 3)
                {
                    encounterQuery = encounterQuery.Where(ef => ef.Patient.Surname.StartsWith(sname.Trim()));
                }
                else
                {
                    encounterQuery = encounterQuery.Where(ef => ef.Patient.Surname.Contains(sname.Trim()));
                }
            }

            if (!String.IsNullOrWhiteSpace(searchfrom) && !String.IsNullOrWhiteSpace(searchto))
            {
                var tsearchfrom = Convert.ToDateTime(searchfrom);
                var tsearchto = Convert.ToDateTime(searchto);
                encounterQuery = encounterQuery.Where(ef => ef.EncounterDate >= tsearchfrom && ef.EncounterDate <= tsearchto);
            }

            IEnumerable<Encounter> encounters = encounterQuery.ToList();

            // XML query on the custom attribute
            if (!String.IsNullOrWhiteSpace(custom))
            {
                encounters = encounters.Where(p => XElement.Parse(p.Patient.CustomAttributesXmlSerialised).Descendants(path).Descendants("Value").First().Value == custom);
            };

            // Loop through and render table
            string status = "";
            foreach (var enc in encounters)
            {
                var href = "/Encounter/ViewEncounter/" + enc.Id.ToString();

                row = new TableRow();

                cell = new TableCell();
                cell.Text = enc.Patient.FirstName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = enc.Patient.Surname;
                row.Cells.Add(cell);

                cell = new TableCell();
                var patientFacility = enc.Patient.GetCurrentFacility();
                cell.Text = patientFacility != null ? patientFacility.Facility.FacilityName : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = enc.EncounterType.Description;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = enc.EncounterDate.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                var action = @"<a class=""btn btn-default"" href=""" + href + @""">View Encounter</a>";

                cell = new TableCell();
                cell.Text = action;
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

        private void HandleAppointment()
        {
            TableRow row;
            TableCell cell;

            Panel pnl;
            HyperLink hyp;

            HtmlGenericControl ul;
            HtmlGenericControl li;

            //string facility = Request.QueryString["facility"];
            //string uid = Request.QueryString["uid"];
            //string fname = Request.QueryString["fname"];
            //string sname = Request.QueryString["sname"];
            //string searchfrom = Request.QueryString["searchfrom"];
            //string searchto = Request.QueryString["searchto"];
            //string criteria = Request.QueryString["criteria"];

            string facility = ddlFacility.Text;
            int facilityId;
            int.TryParse(ddlFacility.SelectedValue, out facilityId);
            string uid = txtUniqueID.Value;
            string fname = txtFirstName.Value;
            string sname = txtSurname.Value;
            string searchfrom = txtSearchFrom.Value;
            string searchto = txtSearchTo.Value;
            string criteria = ddlCriteria.Text;

            // Get custom attribute search value
            string custom = "";
            string path = "";
            var id = Convert.ToInt32(ddlCustomAttribute.SelectedValue);
            if (id > 0)
            {
                DropDownList ddl;
                TextBox txt;

                var con = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.Id == id);
                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.String:
                        txt = (TextBox)spnCustomValue.Controls[0].Controls[0];
                        custom = txt.Text;
                        path = @"CustomStringAttribute";
                        break;

                    case CustomAttributeType.DateTime:
                        txt = (TextBox)spnCustomValue.Controls[0].Controls[0];
                        custom = txt.Text;
                        path = @"CustomStringAttribute";
                        break;

                    case CustomAttributeType.Numeric:
                        txt = (TextBox)spnCustomValue.Controls[0].Controls[0];
                        custom = txt.Text;
                        path = @"CustomStringAttribute";
                        break;

                    case CustomAttributeType.Selection:
                        ddl = (DropDownList)spnCustomValue.Controls[0].Controls[0];
                        custom = ddl.SelectedValue;
                        path = @"CustomSelectionAttribute";
                        break;
                }
            };

            bool err = false;

            divError.Visible = false;
            spnNoRows.Visible = false;
            spnRows.Visible = false;

            lblUniqueID.Attributes.Remove("class");
            lblUniqueID.Attributes.Add("class", "input");
            lblSearchFrom.Attributes.Remove("class");
            lblSearchFrom.Attributes.Add("class", "input");
            lblSearchTo.Attributes.Remove("class");
            lblSearchTo.Attributes.Add("class", "input");
            lblFirstName.Attributes.Remove("class");
            lblFirstName.Attributes.Add("class", "input");
            lblSurname.Attributes.Remove("class");
            lblSurname.Attributes.Add("class", "input");
            lblFacility.Attributes.Remove("class");
            lblFacility.Attributes.Add("class", "input");

            DateTime dttemp;

            // Validation
            if (facility == "0" && String.IsNullOrWhiteSpace(uid) && String.IsNullOrWhiteSpace(fname) && String.IsNullOrWhiteSpace(sname) && (String.IsNullOrWhiteSpace(searchfrom) || String.IsNullOrWhiteSpace(searchto))) 
            {
                lblUniqueID.Attributes.Remove("class");
                lblUniqueID.Attributes.Add("class", "input state-error");
                lblSearchFrom.Attributes.Remove("class");
                lblSearchFrom.Attributes.Add("class", "input state-error");
                lblSearchTo.Attributes.Remove("class");
                lblSearchTo.Attributes.Add("class", "input state-error");
                lblFirstName.Attributes.Remove("class");
                lblFirstName.Attributes.Add("class", "input state-error");
                lblSurname.Attributes.Remove("class");
                lblSurname.Attributes.Add("class", "input state-error");
                lblFacility.Attributes.Remove("class");
                lblFacility.Attributes.Add("class", "input state-error");

                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "At least one search criteria must be selected";
                lblUniqueID.Controls.Add(errorMessageDiv);

                err = true;
            }

            int temp;
            if (uid != "" && !int.TryParse(uid, out temp))
            {
                lblUniqueID.Attributes.Remove("class");
                lblUniqueID.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "Unique ID must be numeric";
                lblUniqueID.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (!String.IsNullOrWhiteSpace(searchfrom) && !String.IsNullOrWhiteSpace(searchto))
            {
                if (DateTime.TryParse(searchfrom, out dttemp))
                {
                    dttemp = Convert.ToDateTime(searchfrom);
                    if (dttemp > DateTime.Today)
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From cannot be after current date";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From cannot be so far in the past";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblSearchFrom.Attributes.Remove("class");
                    lblSearchFrom.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Search From has an invalid date format";
                    lblSearchFrom.Controls.Add(errorMessageDiv);

                    err = true;
                }

                if (DateTime.TryParse(searchto, out dttemp))
                {
                    dttemp = Convert.ToDateTime(searchto);
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblSearchTo.Attributes.Remove("class");
                        lblSearchTo.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search To cannot be so far in the past";
                        lblSearchTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblSearchTo.Attributes.Remove("class");
                    lblSearchTo.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Search To has an invalid date format";
                    lblSearchTo.Controls.Add(errorMessageDiv);

                    err = true;
                }

                if (DateTime.TryParse(searchfrom, out dttemp) && DateTime.TryParse(searchto, out dttemp))
                {
                    if (Convert.ToDateTime(searchfrom) > Convert.ToDateTime(searchto))
                    {
                        lblSearchFrom.Attributes.Remove("class");
                        lblSearchFrom.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search From must be before Search To";
                        lblSearchFrom.Controls.Add(errorMessageDiv);

                        lblSearchTo.Attributes.Remove("class");
                        lblSearchTo.Attributes.Add("class", "input state-error");
                        errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Search To must be after Search From";
                        lblSearchTo.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(fname))
            {
                if (fname.Length < 3)
                {
                    lblFirstName.Attributes.Remove("class");
                    lblFirstName.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "First Name must be at least 3 characters long";
                    lblFirstName.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }
            if (!String.IsNullOrWhiteSpace(sname))
            {
                if (sname.Length < 3)
                {
                    lblSurname.Attributes.Remove("class");
                    lblSurname.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Last Name must be at least 3 characters long";
                    lblSurname.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }

            if (_user.Facilities.Count == 0) 
            {
                lblFacility.Attributes.Remove("class");
                lblFacility.Attributes.Add("class", "input state-error");
                var errorMessageDiv = new HtmlGenericControl("div");
                errorMessageDiv.Attributes.Add("class", "note note-error");
                errorMessageDiv.InnerText = "User must have view permissions to at least one facility";
                lblFacility.Controls.Add(errorMessageDiv);

                err = true;
            }

            if (err) 
            {
                divError.Visible = true;
                return; 
            };

            var appointmentQuery = UnitOfWork.Repository<Appointment>().Queryable().Where( x=>!x.Archived);

            if (!String.IsNullOrWhiteSpace(facility))
            {
                if (facility != "0") {
                    appointmentQuery = appointmentQuery.Where(af => af.Patient.PatientFacilities.Any(f => f.Facility.FacilityName.StartsWith(facility)));
                }
            };

            if (!String.IsNullOrWhiteSpace(uid))
            {
                var tid = Convert.ToInt32(uid);
                appointmentQuery = appointmentQuery.Where(af => af.Patient.Id == tid);
            }

            if (!String.IsNullOrWhiteSpace(fname)) {
                appointmentQuery = appointmentQuery.Where(af => af.Patient.FirstName.StartsWith(fname));
            }

            if (!String.IsNullOrWhiteSpace(sname)) {
                appointmentQuery = appointmentQuery.Where(af => af.Patient.Surname.StartsWith(sname));
            }

            if (!String.IsNullOrWhiteSpace(searchfrom) && !String.IsNullOrWhiteSpace(searchto))
            {
                var tsearchfrom = Convert.ToDateTime(searchfrom);
                var tsearchto = Convert.ToDateTime(searchto);
                appointmentQuery = appointmentQuery.Where(af => af.AppointmentDate >= tsearchfrom && af.AppointmentDate <= tsearchto);
            }

            IEnumerable<Appointment> appointments = appointmentQuery.ToList();

            // XML query on the custom attribute
            if (!String.IsNullOrWhiteSpace(custom))
            {
                appointments = appointments.Where(p => XElement.Parse(p.Patient.CustomAttributesXmlSerialised).Descendants(path).Descendants("Value").First().Value == custom);
            };

            // Loop through and render table
            string status = "";
            string created = string.Empty;
            string updated = string.Empty;
            var rowCount = 0;
            foreach (var appt in appointments)
            {
                row = new TableRow();
                pnl = new Panel();

                cell = new TableCell();
                cell.Text = appt.Id.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = appt.Patient.FirstName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = appt.Patient.Surname;
                row.Cells.Add(cell);

                cell = new TableCell();
                var patientFacility = appt.Patient.GetCurrentFacility();
                cell.Text = patientFacility != null ? patientFacility.Facility.FacilityName : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "N/A";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = appt.AppointmentDate.ToString("yyyy-MM-dd");
                row.Cells.Add(cell);

                var encounter = appt.GetEncounter();
                if(encounter == null)
                {
                    if (appt.AppointmentDate.AddDays(3) < DateTime.Today) {
                        if(!appt.DNA)
                        {
                            status = @"<span class=""label label-info"">MISSED</span>";

                            string[] validCriteria = { "2", "3" };
                            row.Visible = validCriteria.Contains(ddlCriteria.SelectedValue) ? true : false;

                            created = appt.GetCreatedStamp();
                            updated = appt.GetLastUpdatedStamp();

                            pnl = new Panel() { CssClass = "btn-group" };
                            hyp = new HyperLink()
                            {
                                NavigateUrl = "/Patient/PatientView.aspx?pid=" + appt.Patient.Id.ToString(),
                                CssClass = "btn btn-default",
                                Text = "View Patient"
                            };
                            pnl.Controls.Add(hyp);

                            hyp = new HyperLink()
                            {
                                NavigateUrl = "javascript:void(0);",
                                CssClass = "btn btn-default dropdown-toggle",
                            };
                            hyp.Attributes.Add("data-toggle", "dropdown");
                            hyp.Controls.Add(new Label() { CssClass = "caret" });
                            pnl.Controls.Add(hyp);

                            ul = new HtmlGenericControl("ul");
                            ul.Attributes.Add("class", "dropdown-menu");

                            li = new HtmlGenericControl("li");
                            hyp = new HyperLink()
                            {
                                NavigateUrl = "#",
                                Text = "Did Not Arrive"
                            };
                            hyp.Attributes.Add("data-toggle", "modal");
                            hyp.Attributes.Add("data-target", "#appointmentModal");
                            hyp.Attributes.Add("data-id", appt.Id.ToString());
                            hyp.Attributes.Add("data-evt", "dna");
                            hyp.Attributes.Add("data-date", appt.AppointmentDate.ToString("yyyy-MM-dd"));
                            hyp.Attributes.Add("data-reason", appt.Reason);
                            hyp.Attributes.Add("data-cancelled", appt.Cancelled ? "Yes" : "No");
                            hyp.Attributes.Add("data-cancelledreason", appt.CancellationReason);
                            hyp.Attributes.Add("data-created", created);
                            hyp.Attributes.Add("data-updated", updated);
                            li.Controls.Add(hyp);
                            ul.Controls.Add(li);

                            pnl.Controls.Add(ul);
                        }
                        else
                        {
                            status = @"<span class=""label label-info"">Did Not Arrive</span>";

                            string[] validCriteria = { "2", "4" };
                            row.Visible = validCriteria.Contains(ddlCriteria.SelectedValue) ? true : false;
                        }
                    }
                    else 
                    {
                        status = @"<span class=""label label-info"">Appointment</span>";

                        string[] validCriteria = { "2" };
                        row.Visible = validCriteria.Contains(ddlCriteria.SelectedValue) ? true : false;

                        pnl = new Panel() { CssClass = "btn-group" };
                        hyp = new HyperLink()
                        {
                            NavigateUrl = "/Patient/PatientView.aspx?pid=" + appt.Patient.Id.ToString(),
                            CssClass = "btn btn-default",
                            Text = "View Patient"
                        };
                        pnl.Controls.Add(hyp);
                    }
                }
                else 
                {
                    status = @"<span class=""label label-success"">Appointment Met</span>";

                    string[] validCriteria = { "2", "5" };
                    row.Visible = validCriteria.Contains(ddlCriteria.SelectedValue) ? true : false;

                    pnl = new Panel();
                    hyp = new HyperLink()
                    {
                        NavigateUrl = "/Encounter/ViewEncounter/" + encounter.Id.ToString(),
                        CssClass = "btn btn-default",
                        Text = "View Encounter"
                    };
                    pnl.Controls.Add(hyp);
                }

                cell = new TableCell();
                cell.Text = status;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Controls.Add(pnl);
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
                if (row.Visible) { rowCount +=1 ;};
            }

            if (rowCount == 0)
            {
                spnNoRows.InnerText = "No matching records found...";
                spnNoRows.Visible = true;
                spnRows.Visible = false;
            }
            else
            {
                spnRows.InnerText = rowCount.ToString() + " row(s) matching criteria found...";
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

                UnitOfWork.Repository<Appointment>().Update(appointment);
                UnitOfWork.Complete();
                btnSubmit_Click(null, null);
            }
        }

        private Appointment GetAppointment(int id)
        {
            return UnitOfWork.Repository<Appointment>()
                    .Queryable()
                    .Include("Patient")
                    .SingleOrDefault(u => u.Id == id);
        }

        private void RenderCustomValue(CustomAttributeConfiguration con)
        {
            if (con == null) { return; };

            DropDownList ddl;
            TextBox txt;
            Label lbl;
            SelectionDataItem tempSDI;

            lbl = new Label();
            lbl.ID = string.Format("lbl{0}", con.Id);
            lbl.Attributes.Add("class", "input");

            // Add mode so initialise value
            switch (con.CustomAttributeType)
            {
                case CustomAttributeType.String:
                    txt = new TextBox();
                    txt.ID = "txt" + con.Id;
                    txt.Attributes.Add("type", "text");
                    txt.Text = "";
                    if (con.StringMaxLength.HasValue)
                    {
                        txt.Attributes.Add("maxlength", con.StringMaxLength.Value.ToString());
                    }
                    lbl.Controls.Add(txt);
                    break;

                case CustomAttributeType.DateTime:
                    txt = new TextBox();
                    txt.TextMode = TextBoxMode.Date;
                    if (con.FutureDateOnly)
                    {
                        txt.Attributes.Add("min", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                    }
                    if (con.PastDateOnly)
                    {
                        txt.Attributes.Add("max", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                    }
                    txt.ID = "txt" + con.Id;
                    txt.Text = "";
                    lbl.Controls.Add(txt);
                    break;

                case CustomAttributeType.Numeric:
                    txt = new TextBox();
                    txt.TextMode = TextBoxMode.Number;
                    if (con.NumericMinValue.HasValue)
                    {
                        txt.Attributes.Add("min", con.NumericMinValue.Value.ToString());
                    }
                    if (con.NumericMaxValue.HasValue)
                    {
                        txt.Attributes.Add("max", con.NumericMaxValue.Value.ToString());
                    }
                    txt.ID = "txt" + con.Id;
                    txt.Text = "";
                    lbl.Controls.Add(txt);
                    break;

                case CustomAttributeType.Selection:
                    ddl = new DropDownList();
                    ddl.ID = "ddl" + con.Id;
                    ddl.CssClass = "form-control";
                    // Populate drop down list
                    ddl.Items.Add(new ListItem("", ""));
                    foreach (var sdi in UnitOfWork.Repository<SelectionDataItem>().Queryable().Where(h => h.AttributeKey == con.AttributeKey).ToList())
                        ddl.Items.Add(new ListItem(sdi.Value, sdi.SelectionKey));
                    lbl.Controls.Add(ddl);
                    break;
            }

            lblCustomValue.Visible = true;
            spnCustomValue.Controls.Add(lbl);
        }

    }
}