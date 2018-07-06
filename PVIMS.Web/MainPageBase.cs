using System;
using System.Web.UI;

using VPS.Common.Repositories;

namespace PVIMS.Web
{
    public class MainPageBase : Page
    {
        public IUnitOfWorkInt UnitOfWork { get; set; }

        public new Main Master
        {
            get { return (Main)base.Master; }
        }

        public Menu Menu
        {
            get { return Master.MainMenu; }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            UnitOfWork.Complete();
        }

    }
}