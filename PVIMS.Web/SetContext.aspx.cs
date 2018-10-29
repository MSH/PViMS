using System;
using System.Linq;
using System.Web;

using PVIMS.Entities.EF;
using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class SetContext : MainPageBase
    {
        private PVIMSDbContext _db = new PVIMSDbContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["SetContext"] != null)
            {
                User user = GetCurrentUser();

                var context = Convert.ToString(Request.QueryString["SetContext"]);
                var url = "";

                switch (context)
	            {
                    case "0":
                        url = @"\Patient\PatientSearch.aspx";
                        break;

                    case "1":
                        url = @"\Analytical\ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219";
                        break;

                    case "2":
                        url = @"\Reports\System\ReportPxOnStudy.aspx";
                        break;

                    case "3":
                        url = @"\Publisher\PageViewer.aspx?guid=a63e9f29-a22f-43df-87a0-d0c8dec50548";
                        break;

                    case "4":
                        url = @"\Admin\Index";
                        break;

                    default:
                        break;
                }

                if (user != null)
                {
                    user.CurrentContext = context;
                    _db.SaveChanges();
                }
                Response.Redirect(url);

            }
        }

        private User GetCurrentUser()
        {
            return _db.Users.SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
        }
    }
}