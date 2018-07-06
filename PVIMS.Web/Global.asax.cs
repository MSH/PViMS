using Autofac;
using Autofac.Integration.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PVIMS.Core;

namespace PVIMS.Web
{
    public class MvcApplication : System.Web.HttpApplication, IContainerProviderAccessor
    {
        // Web Forms Container
        private static IContainerProvider containerProvider;

        public IContainerProvider ContainerProvider
        {
            get { return containerProvider; }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(config => 
            {
                ODataConfig.Register(config);
                WebApiConfig.Register(config);
            });
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            containerProvider = new ContainerProvider(ContainerConfig.RegisterContainer());

            UserContext.SetUpUserContext(new HttpContextUserContext());
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();

            if (exc != null)
            {
                // Pass the error on to the error page.
                //Server.Transfer("/Error/DisplayError.aspx", true);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("x-frame-options", "DENY");
        }
    }
}
