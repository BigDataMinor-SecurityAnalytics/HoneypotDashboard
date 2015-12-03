using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data {

    public class DataAccess {
        private LogContext Context;

        public DataAccess() {
            Context = new LogContext();
        }

        public IEnumerable<ReadFile> GetReadFiles() {
            return Context.ReadFiles;
        }
    }
};