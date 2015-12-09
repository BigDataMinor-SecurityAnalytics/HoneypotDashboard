using System;
using System.Collections.Generic;

namespace Data.Models {

    public class ReadFile {

        public int ID {
            get; set;
        }

        public string FileName {
            get; set;
        }

        public DateTime LastWrite {
            get; set;
        }
    }
}