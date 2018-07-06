using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web
{
    public static class ODataConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.AddODataQueryFilter();

            ODataModelBuilder builder = new ODataConventionModelBuilder();

            EntitySetConfiguration<Patient> patients = builder.EntitySet<Patient>("patients");
            patients.EntityType.Namespace = "PVIMS.Web.Models";
            patients.EntityType.Name = "PatientDTO";
            patients.EntityType.Property(p => p.PatientGuid).Name = "PatientUniqueIdentifier";
            patients.EntityType.Property(p => p.FirstName).Name = "PatientFirstName";
            patients.EntityType.Property(p => p.Surname).Name = "PatientLastName";

            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}