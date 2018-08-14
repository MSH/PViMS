using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
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
    public partial class PatientSearch : MainPageBase
    {
        private User _user;

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Patient Search", SubTitle = "", Icon = "fa fa-group fa-fw" });

            string action;
            if (Request.QueryString["a"] != null)
            {
                action = Request.QueryString["a"];
                switch (action)
                {
                    case "refresh":
                        divDelete.Visible = true;
                        break;

                } // switch (_action)
            }

            if (!Page.IsPostBack)
            {
                LoadFacilityDropDownList();
            }
            LoadCustomDropDownList();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = UnitOfWork.Repository<User>().Queryable().Include(u => u.Facilities).SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
            //EnsureBookmarkableUri();

            Master.SetMenuActive("PatientView");

            if (Page.IsPostBack)
            {
                divDelete.Visible = false;
            }

            //ddlFacility.Text = Request["facility"];
            //txtUniqueID.Value = Request["uid"];
            //txtFirstName.Value = Request["fname"];
            //txtSurname.Value = Request["sname"];

            //if (!String.IsNullOrWhiteSpace(Request["facility"])) {
            //    LoadDataForSearchCriteria();
            //}
        }

        private void EnsureBookmarkableUri()
        {
            if (Page.IsPostBack)
            {
                Response.Redirect(Request.Url.AbsolutePath + "?&facility=" + ddlFacility.SelectedItem.Text + "&uid=" + txtUniqueID.Value + "&fname=" + txtFirstName.Value + "&sname=" + txtSurname.Value);
            }
        }

        private void LoadDataForSearchCriteria()
        {
            this.btnSubmit_Click(null, EventArgs.Empty);
        }

        private void LoadFacilityDropDownList()
        {
            ListItem item;
            var facilityList = (from f in UnitOfWork.Repository<Facility>().Queryable() orderby f.FacilityName ascending select f).ToList();

            foreach (Facility fac in facilityList)
            {
                item = new ListItem();
                item.Text = fac.FacilityName;
                item.Value = fac.Id.ToString();
                ddlFacility.Items.Add(item);
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

            if (customList.Count() > 0) {
                divCustomSearch.Visible = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            TableRow row;
            TableCell cell;

            string action;

            //string facility = Request.QueryString["facility"];
            //string uid = Request.QueryString["uid"];
            //string fname = Request.QueryString["fname"];
            //string sname = Request.QueryString["sname"];

            string facility = ddlFacility.Text;
            int facilityId;
            int.TryParse(ddlFacility.SelectedValue, out facilityId);
            string uid = txtUniqueID.Value;
            string fname = txtFirstName.Value.Trim();
            string sname = txtSurname.Value.Trim();
            string dob = txtDateBirth.Value.Trim();

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
            DateTime dttemp;

            divError.Visible = false;
            spnNoRows.Visible = false;
            spnRows.Visible = false;

            lblUniqueID.Attributes.Remove("class");
            lblUniqueID.Attributes.Add("class", "input");
            lblFirstName.Attributes.Remove("class");
            lblFirstName.Attributes.Add("class", "input");
            lblSurname.Attributes.Remove("class");
            lblSurname.Attributes.Add("class", "input");
            lblFacility.Attributes.Remove("class");
            lblFacility.Attributes.Add("class", "input");
            lblDOB.Attributes.Remove("class");
            lblDOB.Attributes.Add("class", "input");

            // Validation
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

            if (!String.IsNullOrWhiteSpace(dob))
            {
                if (DateTime.TryParse(dob, out dttemp))
                {
                    dttemp = Convert.ToDateTime(dob);
                    if (dttemp > DateTime.Today)
                    {
                        lblDOB.Attributes.Remove("class");
                        lblDOB.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date of Birth should be before current date";
                        lblDOB.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                    if (dttemp < DateTime.Today.AddYears(-120))
                    {
                        lblDOB.Attributes.Remove("class");
                        lblDOB.Attributes.Add("class", "input state-error");
                        var errorMessageDiv = new HtmlGenericControl("div");
                        errorMessageDiv.Attributes.Add("class", "note note-error");
                        errorMessageDiv.InnerText = "Date of Birth cannot be so far in the past";
                        lblDOB.Controls.Add(errorMessageDiv);

                        err = true;
                    }
                }
                else
                {
                    lblDOB.Attributes.Remove("class");
                    lblDOB.Attributes.Add("class", "input state-error");
                    var errorMessageDiv = new HtmlGenericControl("div");
                    errorMessageDiv.Attributes.Add("class", "note note-error");
                    errorMessageDiv.InnerText = "Date of Birth has an invalid date format";
                    lblDOB.Controls.Add(errorMessageDiv);

                    err = true;
                }
            }

            if (err)
            {
                divError.Visible = true;
                return;
            };

            var patientQuery = UnitOfWork.Repository<Patient>().Queryable().Where( x=>!x.Archived);

            if (!String.IsNullOrWhiteSpace(facility))
            {
                if (facility != "All Facilities") {
                    patientQuery = patientQuery.Where(pf => pf.PatientFacilities.OrderByDescending(pf1 => pf1.EnrolledDate).Take(1).Any(f => f.Facility.Id == facilityId));
                }
            }

            if (!String.IsNullOrWhiteSpace(uid))
            {
                var tid = Convert.ToInt32(uid);
                patientQuery = patientQuery.Where(p => p.Id == tid);
            }

            if (!String.IsNullOrWhiteSpace(fname))
            {
                if (fname.Length < 3)
                {
                    patientQuery = patientQuery.Where(p => p.FirstName.StartsWith(fname.Trim()));
                }
                else
                {
                    patientQuery = patientQuery.Where(p => p.FirstName.Contains(fname.Trim()));
                }
            }

            if (!String.IsNullOrWhiteSpace(sname))
            {
                if (sname.Length < 3)
                {
                    patientQuery = patientQuery.Where(p => p.Surname.StartsWith(sname.Trim()));
                }
                else
                {
                    patientQuery = patientQuery.Where(p => p.Surname.Contains(sname.Trim()));
                }
            }

            if (!String.IsNullOrWhiteSpace(dob))
            {
                dttemp = Convert.ToDateTime(dob);
                patientQuery = patientQuery.Where(p => p.DateOfBirth == dttemp);
            }

            IEnumerable<Patient> patients = patientQuery.ToList();

            // XML query on the custom attribute
            if (!String.IsNullOrWhiteSpace(custom)) {
                patients = patients.Where(p => XElement.Parse(p.CustomAttributesXmlSerialised).Descendants(path).Descendants("Value").First().Value == custom);
            }

            // Loop through and render table
            DateTime? lastEncounter = null;
            var rowCount = 0;
            foreach (var p in patients)
            {
                var href = "PatientView.aspx?pid=" + p.Id.ToString();

                row = new TableRow();

                cell = new TableCell();
                cell.Text = p.Id.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = p.FirstName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = p.Surname;
                row.Cells.Add(cell);

                cell = new TableCell();
                var patientFacility = p.GetCurrentFacility();
                cell.Text = patientFacility != null ? patientFacility.Facility.FacilityName : "";
                row.Cells.Add(cell);

                IExtendable pExtended = null;
                pExtended = p;
                var attribute = pExtended.GetAttributeValue("Medical Record Number");

                cell = new TableCell();
                cell.Text = attribute != null ? attribute.ToString() : "";
                row.Cells.Add(cell);

                cell = new TableCell();
                if (p.Age < 19)
                {
                    cell.Text = String.Format(@"{0} <span class=""badge bg-color-blueLight"">{1}</span>", p.DateOfBirth != null ? Convert.ToDateTime(p.DateOfBirth).ToString("yyyy-MM-dd") : "", p.Age.ToString());
                }
                else
                {
                    cell.Text = String.Format(@"{0} <span class=""badge bg-color-blueDark"">{1}</span>", Convert.ToDateTime(p.DateOfBirth).ToString("yyyy-MM-dd"), p.Age.ToString());
                }
                row.Cells.Add(cell);

                lastEncounter = p.LastEncounterDate();
                cell = new TableCell();
                if (lastEncounter != null)
                {
                    cell.Text = Convert.ToDateTime(lastEncounter).ToString("yyyy-MM-dd");
                }
                else
                {
                    cell.Text = @"<span class=""label label-warning"">No Encounters</span>";
                }
                row.Cells.Add(cell);

                action = @"<div class=""btn-group""><a class=""btn btn-default"" href=""" + href + @""">View Patient</a></div><!-- /btn-group -->";

                cell = new TableCell();
                cell.Text = action;
                row.Cells.Add(cell);

                dt_basic.Rows.Add(row);
                rowCount += 1;
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

            // Add button
            HyperLink hyp = new HyperLink()
            {
                ID = "btnAdd",
                NavigateUrl = "PatientView.aspx?pid=0",
                CssClass = "btn btn-default",
                Text = "Add Patient"
            };
            spnButtons.Controls.Add(hyp);
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