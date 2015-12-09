using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data {

    public class DataAccess {

        public LogContext Context {
            get; private set;
        }

        private ObjectContext ObjContext;

        public DataAccess() {
            Context = new LogContext();
            ObjContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)Context).ObjectContext;
        }

        public void AddReadFile(ReadFile file) {
            Context.ReadFiles.Add(file);
            Context.SaveChanges();
        }

        public void AddLogEntry(LogEntry entry) {
            Context.LogEntries.Add(entry);
            Context.SaveChanges();
        }

        public void Clear() {
            Context.Database.ExecuteSqlCommand($"DELETE FROM {GetDBTableName<ReadFile>()}");
            Context.Database.ExecuteSqlCommand($"DELETE FROM {GetDBTableName<LogEntry>()}");
        }

        private string GetDBTableName<T>() where T : class {
            string name = (Context as System.Data.Entity.Infrastructure.IObjectContextAdapter).ObjectContext.CreateObjectSet<T>().EntitySet.Name;
            return name;
        }

        public void SaveChanges() {
            Context.SaveChanges();
        }
    }
};