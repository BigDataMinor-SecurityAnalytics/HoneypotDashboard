using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data {

    public class LogContext : DbContext {

        public LogContext() : base("LogContext") {
        }

        public DbSet<ReadFile> ReadFiles {
            get; set;
        }

        public DbSet<LogEntry> LogEntries {
            get; set;
        }
    }
}