using Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher {

    internal class LogfileUpdate {

        public event ConsoleLog Log;

        private string FolderPath;

        public LogfileUpdate(string folderPath, DataAccess logData) {
            FolderPath = folderPath;
            AddFolderFiles();

            var watcher = new FileSystemWatcher() {
                Path = folderPath,
                IncludeSubdirectories = false,
                Filter = "*.csv",
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
            watcher.Changed += FolderChanged;
        }

        private void FolderChanged(object sender, FileSystemEventArgs e) {
            Log?.Invoke($"Something changed in Log folder - {e.ChangeType} : {e.Name}");
            AddFolderFiles();
        }

        private void AddFolderFiles() {
            //Todo
        }
    }
}