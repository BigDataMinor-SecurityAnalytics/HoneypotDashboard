using Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher {

    internal class CSVLogReader {
        private readonly DataAccess LogAccess;

        public CSVLogReader(DataAccess logAccess) {
            LogAccess = logAccess;
        }

        public Exception AddCSVToDB(string csvPath) {
            try {
                string fileContent = string.Empty;
                using(var sr = new StreamReader(csvPath)) {
                    fileContent = sr.ReadToEnd();
                }
                return null;
            } catch(Exception e) {
                return e;
            }
        }
    }
}