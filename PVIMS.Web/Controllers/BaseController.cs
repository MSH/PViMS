using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public string PreparePopUpMessage()
        {
            var ret = "";

            var popUpMessage = "";
            if (Request.Cookies["PopUpMessage"] != null)
            {
                popUpMessage = Convert.ToString(Request.Cookies["PopUpMessage"].Value);
            }

            if (!string.IsNullOrWhiteSpace(popUpMessage))
            {
                ret = String.Format(@"
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

                Response.Cookies["PopUpMessage"].Expires = DateTime.Now.AddDays(-1); ;

            }
            return ret;
        }
    }

}