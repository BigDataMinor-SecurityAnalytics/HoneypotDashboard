using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;

namespace WebAccess {

    public static class WebApiConfig {

        public static void Register(HttpConfiguration config) {
            config.MapHttpAttributeRoutes();

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<LogEntry>("LogEntries");
            config.Routes.MapODataServiceRoute("odata", "api", builder.GetEdmModel());
        }
    }
}