using System.Web;
using System.Web.Optimization;

namespace PVIMS.Web
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
         "~/Scripts/jquery-ui.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js"));

#if DEBUG
            bundles.Add(new ScriptBundle("~/scripts/lib/bundle")
                    .Include("~/Scripts/knockout-{version}.debug.js"));
#else
			bundles.Add(new ScriptBundle("~/scripts/lib/bundle")
					.Include("~/Scripts/knockout-{version}.js"));
#endif
            // All application JS files
            bundles.Add(new ScriptBundle("~/bundles/jsapplibs")
                .IncludeDirectory("~/Scripts/knockout/", "*.js", searchSubdirectories: false));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/css/bootstrap.min.css",
                      "~/css/font-awesome.min.css",
                      "~/css/smartadmin-production.min.css",
                      "~/css/smartadmin-skins.min.css",
                       "~/css/themes/base/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/bundles/fonts").Include(
                    "~/fonts/fontawesome-webfont.eot",
                    "~/fonts/fontawesome-webfont.svg",
                    "~/fonts/fontawesome-webfont.ttf",
                    "~/fonts/fontawesome-webfont.woff",
                    "~/fonts/FontAwesome.otf"));
		}
	}
}
