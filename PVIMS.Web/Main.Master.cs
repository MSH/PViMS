using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using VPS.Common.Repositories;

using PVIMS.Core;
using PVIMS.Core.Entities;
using PVIMS.Entities.EF;
using PVIMS.Web.Models;

namespace PVIMS.Web
{
    public partial class Main : MasterPage
    {
        public IUnitOfWorkInt UnitOfWork { get; set; }

        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        public Main()
        {

        }

        public Menu MainMenu
        {
            get { return Menu; }
        }

        public User User
        {
            get
            {
                return UserContext.Current.User;
            }
        }

        public void ShouldPopUpBeDisplayed()
        {
            var popUpMessage = "";
            if (Request.Cookies["PopUpMessage"] != null)
            {
                popUpMessage = Convert.ToString(Request.Cookies["PopUpMessage"].Value);
            }
            if (Request.QueryString["PopUpMessage"] != null && !Page.IsPostBack) //Only process message if not a postback
            {
                popUpMessage = Convert.ToString(Request.QueryString["PopUpMessage"]);
            }
            if (!string.IsNullOrWhiteSpace(popUpMessage))
            {
                string script = String.Format(@"
                    $(document).ready(function () {{
                        $.bigBox({{
                            title: ""Action completed..."",
                            content: ""{0}"",
                            color: ""#3276B1"",
                            timeout: 4000,
                            icon : ""fa fa-bell swing animated""
                        }});
                    }});
                    ", popUpMessage);

                this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "RegisterClientScriptBlock", script, true);

                Response.Cookies["PopUpMessage"].Expires = DateTime.Now.AddDays(-1); ;
            }
        }

        public void SetPageHeader(PageHeaderDetail detail)
        {
            divEdit.Visible = false;
            spnPageTitle.InnerHtml = String.Format(@"<h1 class=""page-title txt-color-blueDark""><i class=""{0}""></i>{1}</h1>", detail.Icon, detail.Title);
            if(detail.MetaPageId > 0)
            {
                divEdit.Visible = true;
                hrefEdit.HRef = "/Publisher/PageCustom.aspx?id=" + detail.MetaPageId;
                hrefDelete.HRef = "/Publisher/DeleteMetaPage?metaPageId=" + detail.MetaPageId;
                hrefAdd.HRef = "/Publisher/PageCustomWidget.aspx?Id=0&pid=" + detail.MetaPageId;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            hrefClinical.Style["color"] = "white";
            hrefAnalytical.Style["color"] = "white";
            hrefReporting.Style["color"] = "white";
            hrefPublisher.Style["color"] = "white";

            if (User != null)
            {
                // Highlight selected portal
                switch (User.CurrentContext)
                {
                    case "Clinical":
                        hrefClinical.Style["color"] = "orange";
                        break;

                    case "Analytical":
                        hrefAnalytical.Style["color"] = "orange";
                        break;

                    case "Reports":
                        hrefReporting.Style["color"] = "orange";
                        break;

                    case "Info":
                        hrefPublisher.Style["color"] = "orange";
                        break;

                    default:
                        break;
                }
                lblPortalName.Text = User.CurrentContext + " Portal";
                lblPortalName.Style["color"] = "orange";

                // Portal roles
                hrefAnalytical.Visible = HttpContext.Current.User.IsInRole("Analyst");
                hrefReporting.Visible = HttpContext.Current.User.IsInRole("Reporter");
                hrefPublisher.Visible = HttpContext.Current.User.IsInRole("Publisher");

                PVIMSDbContext db = new PVIMSDbContext();

                // pepare facilities
                User localUser = db.Users.Include(u => u.Facilities.Select(f => f.Facility)).SingleOrDefault(u => u.UserName == User.UserName);
                
                db = null;

                string facilities = "<ul>";
                foreach (UserFacility uf in localUser.Facilities) {
                    facilities += string.Format("<li>{0}</li>", uf.Facility.FacilityName);
                }
                facilities += "</ul>";

                // pepare roles
                string roles = "<ul>";
                if (HttpContext.Current.User.IsInRole("Admin")) {
                    roles += string.Format("<li>{0}</li>", "Administrator");
                }
                if (HttpContext.Current.User.IsInRole("RegClerk")) {
                    roles += string.Format("<li>{0}</li>", "Registration Clerk");
                }
                if (HttpContext.Current.User.IsInRole("DataCap")) {
                    roles += string.Format("<li>{0}</li>", "Data Capturer");
                }
                if (HttpContext.Current.User.IsInRole("Clinician")) {
                    roles += string.Format("<li>{0}</li>", "Clinician");
                }
                if (HttpContext.Current.User.IsInRole("Analyst")) {
                    roles += string.Format("<li>{0}</li>", "Analytics");
                }
                if (HttpContext.Current.User.IsInRole("Reporter")) {
                    roles += string.Format("<li>{0}</li>", "Reporter");
                }
                if (HttpContext.Current.User.IsInRole("Publisher")) {
                    roles += string.Format("<li>{0}</li>", "Publisher");
                }
                if (HttpContext.Current.User.IsInRole("ReporterAdmin")) {
                    roles += string.Format("<li>{0}</li>", "Reporter Administrator");
                }
                if (HttpContext.Current.User.IsInRole("PublisherAdmin")) {
                    roles += string.Format("<li>{0}</li>", "Publisher Administrator");
                }
                roles += "</ul>";

                HyperLink storeHyp = new HyperLink()
                {
                    NavigateUrl = "#",
                    Text = User.UserName
                };
                storeHyp.Attributes.Add("style", "color:white");
                storeHyp.Attributes.Add("data-toggle", "modal");
                storeHyp.Attributes.Add("data-target", "#userModal");
                storeHyp.Attributes.Add("data-facility", facilities);
                storeHyp.Attributes.Add("data-role", roles);
                pnlLogin.Controls.Add(storeHyp);
            }

            ShouldPopUpBeDisplayed();
        }
    }
}