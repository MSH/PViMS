using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using PVIMS.Entities.EF.Repositories;
using PVIMS.Services;

namespace PVIMS.Web
{
    public class ContainerConfig
    {
        public static IContainer RegisterContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly())
                .OnActivated(c => c.Context.InjectProperties(c.Instance));
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<UserInfoStore>()
                .AsImplementedInterfaces();

            builder.Register(c => new IdentityFactoryOptions<UserInfoManager>
            {
                DataProtectionProvider = new DpapiDataProtectionProvider("PVIMS")
            });

            builder.RegisterType<UserInfoManager>();

            builder.RegisterType<CustomAttributeService>()
                .AsImplementedInterfaces();

            builder.RegisterType<MedDraService>()
                .AsImplementedInterfaces();

            builder.RegisterType<InfrastructureService>()
                .AsImplementedInterfaces();

            builder.RegisterType<PatientService>()
                .AsImplementedInterfaces();

            builder.RegisterType<WorkFlowService>()
                .AsImplementedInterfaces();

            builder.RegisterType<ReportService>()
                .AsImplementedInterfaces();

            builder.RegisterType<ArtefactService>()
                .AsImplementedInterfaces();

            builder.RegisterType<EntityFrameworkUnitOfWork>()
                   .AsImplementedInterfaces();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            return container;
        }
    }
}