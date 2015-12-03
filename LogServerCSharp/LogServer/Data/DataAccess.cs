using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data {

    public class DataAccess {
        private LogContext Context;
        private ObjectContext ObjContext;

        public DataAccess() {
            Context = new LogContext();
            ObjContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)Context).ObjectContext;
        }

        public IEnumerable<ReadFile> GetReadFiles() {
            return Context.ReadFiles;
        }

        public void AddReadFile(ReadFile file) {
            Context.ReadFiles.Add(file);
            Context.SaveChanges();
        }

        public void Clear() {
            Context.Database.ExecuteSqlCommand($"DELETE from {GetDBTableName<ReadFile>()}");
        }

        private string GetDBTableName<T>() where T : class {
            string name = (Context as System.Data.Entity.Infrastructure.IObjectContextAdapter).ObjectContext.CreateObjectSet<T>().EntitySet.Name;
            return name;
        }
    }
};