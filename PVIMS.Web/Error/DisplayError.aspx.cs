using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PVIMS.Web
{
    public partial class DisplayError : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // Create safe error messages.
            string unhandledErrorMsg = "The error was unhandled by application code.";

            // Determine where error was handled.
            string errorHandler = Request.QueryString["handler"];
            if (errorHandler == null)
            {
                errorHandler = "Error Page";
            }

            // Get the last error from the server.
            Exception ex = Server.GetLastError();

            // If the exception no longer exists, create a generic exception.
            if (ex == null)
            {
                ex = new Exception(unhandledErrorMsg);
            }

            // Detailed Error Message.
            lblMessage.Text = ex.Message;

            if (ex.InnerException != null)
            {
                lblInnerMessage.Text = ex.GetType().ToString() + "<br/>" +
                    ex.InnerException.Message;
                lblInnerTrace.Text = ex.InnerException.StackTrace;
            }
            else
            {
                lblInnerMessage.Text = ex.GetType().ToString();
                if (ex.StackTrace != null)
                {
                    lblInnerTrace.Text = ex.StackTrace.ToString().TrimStart();
                }
            }

            Local.Visible = false;

            // Clear the error from the server.
            Server.ClearError();
        }
    }
}