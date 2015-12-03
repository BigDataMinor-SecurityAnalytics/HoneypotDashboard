using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher {

    public delegate void ConsoleLog(string msg, ConsoleColor? foreColor = null, ConsoleColor? backColor = null);
}