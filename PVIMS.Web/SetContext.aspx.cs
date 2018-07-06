using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using PVIMS.Core;
using PVIMS.Entities.EF;
using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class SetContext : MainPageBase
    {
        private PVIMSDbContext _db = new PVIMSDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if context set
            if (Request.QueryString["SetContext"] != null)
            {
                Session["CurrentContext"] = null;

                User user = GetCurrentUser();

                var context = Convert.ToString(Request.QueryString["SetContext"]);
                var url = "";

                switch (context.ToLower())
	            {
                    case "oltp":
                        if(user != null)
                        {
                            user.CurrentContext = "Clinical";
                            _db.SaveChanges();
                        }
                        url = @"\Patient\PatientSearch.aspx";
                        Response.Redirect(url);
                        break;

                    case "analytical":
                        if (user != null)
                        {
                            user.CurrentContext = "Analytical";
                            _db.SaveChanges();
                        }
                        url = @"\Analytical\ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219";
                        Response.Redirect(url);
                        break;

                    case "reporter":
                        if (user != null)
                        {
                            user.CurrentContext = "Reports";
                            _db.SaveChanges();
                        }
                        url = @"\Reports\System\ReportPxOnStudy.aspx";
                        Response.Redirect(url);
                        break;

                    case "publisher":
                        if (user != null)
                        {
                            user.CurrentContext = "Info";
                            _db.SaveChanges();
                        }
                        url = @"\Publisher\PageViewer.aspx?guid=a63e9f29-a22f-43df-87a0-d0c8dec50548";
                        Response.Redirect(url);
                        break;

                    default:
                        break;
                }
            }
        }

        private User GetCurrentUser()
        {
            return _db.Users.SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
        }
    }
}