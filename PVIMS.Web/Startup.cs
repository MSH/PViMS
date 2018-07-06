using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using Microsoft.Owin;
using Owin;
using PVIMS.Entities.EF;
using PVIMS.Entities.EF.Migrations;

[assembly: OwinStartupAttribute(typeof(PVIMS.Web.Startup))]
namespace PVIMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            InitialiseDatabase();
        }

        private void InitialiseDatabase()
        {
            Database.SetInitializer(new DatabaseInitializer());
        }
    }
}
