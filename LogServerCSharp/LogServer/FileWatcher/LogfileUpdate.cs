using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher {

    internal class LogfileUpdate {

        public event ConsoleLog Log;

        private DataAccess LogData;
        private string FolderPath;

        private bool Started = false;

        public LogfileUpdate(string folderPath, DataAccess logData) {
            FolderPath = folderPath;
            LogData = logData;

            AddFolderFiles();
        }

        public void Init() {
            if(!Started) {
                var watcher = new FileSystemWatcher() {
                    Path = FolderPath,
                    IncludeSubdirectories = false,
                    Filter = "*.csv",
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true
                };
                watcher.Changed += FolderChanged;
                Log?.Invoke("Folder watcher has been set up", ConsoleColor.Green);
            }
        }

        private void FolderChanged(object sender, FileSystemEventArgs e) {
            Log?.Invoke($"Something changed in Log folder - {e.ChangeType} : {e.Name}");
            AddFolderFiles();
        }

        private void AddFolderFiles() {
            ReadFile[] savedLogs = LogData.GetReadFiles().ToArray();
            foreach(var file in new DirectoryInfo(FolderPath).GetFiles("*.csv")) {
                if(savedLogs.FirstOrDefault(rf => rf.FileName == file.Name) == null) {
                    Log?.Invoke($"File '{file.Name}' is not yet in DB", ConsoleColor.White, ConsoleColor.DarkGreen);
                }
            }
        }
    }
}