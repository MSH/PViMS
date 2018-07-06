using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VPS.MIS.WebInterface.Views.Shared
{
    public partial class _MvcMenu : System.Web.Mvc.ViewUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Menu.SetActive(ViewBag.MenuItem);
        }
    }
}