using System;
using System.Linq;
using System.Web;
using System.Web.UI;

using PVIMS.Core.Entities;

namespace PVIMS.Web
{
    public partial class SpontaneousAnalyser : MainPageBase
    {
        private enum FormMode { EditMode = 1, ViewMode = 2, AddMode = 3 };
        private FormMode _formMode = FormMode.ViewMode;

        protected void Page_Init(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                User user = UnitOfWork.Repository<User>().Queryable().Single(u => u.UserName == HttpContext.Current.User.Identity.Name);
                divDownload.Visible = user.AllowDatasetDownload;
            }

            Master.MainMenu.SetActive("SpontaneousAnalyserView");
        }

    }
}