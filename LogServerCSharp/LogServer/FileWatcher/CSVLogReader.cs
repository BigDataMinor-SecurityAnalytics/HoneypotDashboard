using Data;
using Data.Models;
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
                var groupGuidBindings = new Dictionary<int, Guid>();

                string fileContent = string.Empty;
                using(var sr = new StreamReader(csvPath)) {
                    fileContent = sr.ReadToEnd();
                }

                foreach(var line in fileContent.Replace("\r", "").Split('\n').Where(s => s.Length > 1)) {
                    var items = line.Split(',');

                    int fileGroupId = int.Parse(items[1]);
                    Guid groupId = Guid.Empty;
                    if(groupGuidBindings.ContainsKey(fileGroupId)) {
                        groupId = groupGuidBindings[fileGroupId];
                    } else {
                        groupId = Guid.NewGuid();
                        groupGuidBindings.Add(fileGroupId, groupId);
                    }

                    var date = DateTime.Parse($"{items[2]} {items[3]}");
                    bool incomming = items[11] == "RX";
                    int contentSize = int.Parse(items[13]);
                    string content = contentSize > 0 ? ConvertHex(items[12]) : string.Empty;

                    var newEntry = new LogEntry() {
                        GroupID = groupId,
                        Time = date,
                        RemoteIp = items[6],
                        RemotePort = int.Parse(items[7]),
                        LocalIp = items[8],
                        LocalPort = int.Parse(items[9]),
                        Protocol = items[10],
                        Incomming = incomming,
                        Content = content,
                        ContentSizeBytes = contentSize
                    };

                    LogAccess.Context.LogEntries.Add(newEntry);
                }
                LogAccess.Context.SaveChanges();

                return null;
            } catch(Exception e) {
                return e;
            }
        }

        private string ConvertHex(string hexString) {
            var sb = new StringBuilder();
            for(int i = 0; i < hexString.Length; i += 2) {
                string hs = hexString.Substring(i, 2);
                sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
            }
            return sb.ToString();
        }
    }
}