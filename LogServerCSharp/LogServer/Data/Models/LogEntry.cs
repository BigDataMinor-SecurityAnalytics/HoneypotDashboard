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

        public Guid GroupID {
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

        public string Protocol {
            get; set;
        }

        public string Content {
            get; set;
        }

        public int ContentSizeBytes {
            get; set;
        }

        public bool Incomming {
            get; set;
        }

        public DateTime Time {
            get; set;
        }
    }
}