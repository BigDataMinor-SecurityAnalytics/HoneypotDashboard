using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;

namespace WebAccess.Controllers {
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Data.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<LogEntry>("LogEntries");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

    public class LogEntriesController : ODataController {
        private LogContext db = new LogContext();

        // GET: odata/LogEntries
        [EnableQuery]
        public IQueryable<LogEntry> GetLogEntries() {
            return db.LogEntries;
        }

        // GET: odata/LogEntries(5)
        [EnableQuery]
        public SingleResult<LogEntry> GetLogEntry([FromODataUri] int key) {
            return SingleResult.Create(db.LogEntries.Where(logEntry => logEntry.ID == key));
        }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}