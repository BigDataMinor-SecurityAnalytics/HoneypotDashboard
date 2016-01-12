using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models {

    public class LogEntry {

        public int ID {
            get; set;
        }

        public int Nr {
            get; set;
        }

        public string RemoteDomain {
            get; set;
        }

        public string RemoteIp {
            get; set;
        }

        public int RemotePort {
            get; set;
        }

        public string LocalIp {
            get; set;
        }

        public int LocalPort {
            get; set;
        }

        public string TargetService {
            get; set;
        }

        public string Protocol {
            get; set;
        }

        public string Content {
            get; set;
        }

        public int ContentSizeBytes {
            get; set;
        }

        public string Severity {
            get; set;
        }

        public DateTime Time {
            get; set;
        }

        public string Country {
            get; set;
        }
    }
}