using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data {

    public class DataInit : DropCreateDatabaseIfModelChanges<LogContext> {

        protected override void Seed(LogContext context) {
            var testReadFile = new ReadFile() { FileName = "TestFileName", ReadTime = new DateTime(2000, 1, 1, 1, 1, 1) };
            context.ReadFiles.Add(testReadFile);
            context.SaveChanges();
        }
    }
}