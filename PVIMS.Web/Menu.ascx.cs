using System;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using PVIMS.Entities.EF;
using PVIMS.Core.Entities;
using PVIMS.Core.Services;

namespace PVIMS.Web
{
    public partial class Menu : System.Web.UI.UserControl
    {
        public IWorkFlowService _workFlowService { get; set; }

        private enum CurrentContext
        {
            ccClinical,
            ccAnalytical,
            ccReporting,
            ccPublication
        }
        private CurrentContext _currentContext = CurrentContext.ccClinical;

        protected void Page_Load(object sender, EventArgs e)
        {
            navCommon.Visible = false;
            navAnalytical.Visible = false;
            navOLTP.Visible = false;
            navReporter.Visible = false;
            navPublisher.Visible = false;

            mnu_calendarview.Visible = false;
            mnu_patient.Visible = false;
            mnu_encounter.Visible = false;
            mnu_cohort.Visible = false;

            SetContext();

            switch (_currentContext)
            {
                case CurrentContext.ccClinical:
                    // Permissions
                    navOLTP.Visible = true;

                    if (HttpContext.Current.User.IsInRole("Admin")) { navCommon.Visible = true; }
                    if (HttpContext.Current.User.IsInRole("RegClerk") || HttpContext.Current.User.IsInRole("DataCap") || HttpContext.Current.User.IsInRole("Clinician")) { mnu_patient.Visible = true; }
                    if (HttpContext.Current.User.IsInRole("RegClerk")) { mnu_calendarview.Visible = true; }
                    if (HttpContext.Current.User.IsInRole("DataCap") || HttpContext.Current.User.IsInRole("Clinician")) { mnu_encounter.Visible = true; }
                    if (HttpContext.Current.User.IsInRole("Clinician")) { mnu_cohort.Visible = true; }

                    break;

                case CurrentContext.ccAnalytical:
                    navAnalytical.Visible = true;

                    // Reporting instance alert
                    var activeCount = 0;
                    var spontCount = 0;
                    if(_workFlowService != null)
                    {
                        activeCount = _workFlowService.CheckWorkFlowInstanceCount("New Active Surveilliance Report");
                        spontCount = _workFlowService.CheckWorkFlowInstanceCount("New Spontaneous Surveilliance Report");
                    }

                    if (activeCount > 0)
                    {
                        spnActiveCount.InnerText = activeCount.ToString();
                        spnActiveCount.Style["display"] = "block";
                    }
                    if (spontCount > 0)
                    {
                        spnSpontaneousCount.InnerText = spontCount.ToString();
                        spnSpontaneousCount.Style["display"] = "block";
                    }

                    break;

                case CurrentContext.ccReporting:
                    navReporter.Visible = true;

                    //SetCustomMenus();
                    if (HttpContext.Current.User.IsInRole("ReporterAdmin")) { mnu_reportlist.Visible = true; }

                    break;

                case CurrentContext.ccPublication:
                    navPublisher.Visible = true;

                    SetCustomPublicationMenus();
                    if (HttpContext.Current.User.IsInRole("PublisherAdmin")) { mnu_publishadmin.Visible = true; }

                    break;
            }
        }

        private void SetContext()
        {
            var currentContext = "";
            if(Session["CurrentContext"] == null)
            {
                PVIMSDbContext db = new PVIMSDbContext();

                var user = db.Users.SingleOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);
                if (user != null) {
                    currentContext = user.CurrentContext;
                }
                else {
                    currentContext = string.Empty;
                }

                db = null;
            }

            switch (currentContext)
            {
                case "Clinical":
                    _currentContext = CurrentContext.ccClinical;
                    break;
                case "Analytical":
                    _currentContext = CurrentContext.ccAnalytical;
                    break;
                case "Info":
                    _currentContext = CurrentContext.ccPublication;
                    break;
                case "Reports":
                    _currentContext = CurrentContext.ccReporting;
                    break;
                case "":
                    _currentContext = CurrentContext.ccClinical;
                    break;
            }
        }

        public void SetActive(string itemname)
        {
            if (itemname == null) { itemname = ""; };

            var name = "mnu_" + itemname.ToLower();

            InitActive();

            var mnu = (HtmlGenericControl)FindControl(name);
            mnu.Attributes.Add("class", "active");
        }

        private void InitActive()
        {
            foreach(HtmlGenericControl control in Controls.OfType<HtmlGenericControl>())
            {
                if (control.ID.Contains("mnu"))
                {
                    control.Attributes.Add("class", "");
                }
            }
        }

        private void SetCustomReportMenus()
        {
            // Generate list of custom menus
            HtmlGenericControl ul = new HtmlGenericControl("ul");

            PVIMSDbContext db = new PVIMSDbContext();
            foreach (MetaReport report in db.MetaReports.OrderBy(mr => mr.Id))
            {
                HtmlGenericControl li = new HtmlGenericControl("li");
                li.Attributes.Add("id", "mnu_report" + report.Id.ToString());

                HyperLink hyp = new HyperLink() { NavigateUrl = "/Reports/ReportViewer.aspx?id=" + report.Id.ToString() };
                HtmlGenericControl i = new HtmlGenericControl("i");
                i.Attributes.Add("class", "fa fa-lg fa-fw fa-bar-chart-o");
                hyp.Controls.Add(i);
                HtmlGenericControl span = new HtmlGenericControl("span");
                span.Attributes.Add("class", "menu-item-parent");
                span.InnerHtml = "&nbsp; " + report.ReportName;
                hyp.Controls.Add(span);

                li.Controls.Add(hyp);
                ul.Controls.Add(li);
            }
            db = null;

            spnCustomReportList.Controls.Add(ul);
        }

        private void SetCustomPublicationMenus()
        {
            // Generate list of custom menus
            HtmlGenericControl ul = new HtmlGenericControl("ul");

            PVIMSDbContext db = new PVIMSDbContext();
            foreach (MetaPage page in db.MetaPages.Where(mp => mp.IsVisible).OrderBy(mp => mp.Id))
            {
                HtmlGenericControl li = new HtmlGenericControl("li");
                li.Attributes.Add("id", "mnu_info" + page.Id.ToString());

                HyperLink hyp = new HyperLink() { NavigateUrl = "/Publisher/PageViewer.aspx?guid=" + page.metapage_guid.ToString() };
                HtmlGenericControl i = new HtmlGenericControl("i");
                i.Attributes.Add("class", "fa fa-lg fa-fw fa-files-o");
                hyp.Controls.Add(i);
                HtmlGenericControl span = new HtmlGenericControl("span");
                span.InnerHtml = "&nbsp; " + page.PageName;
                hyp.Controls.Add(span);

                li.Controls.Add(hyp);
                ul.Controls.Add(li);
            }
            db = null;

            spnCustomPublisherList.Controls.Add(ul);
        }

    }
}