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

        private readonly DataAccess LogData;
        private readonly string FolderPath;
        private readonly CSVLogReader CSVLogReader;

        private bool Started = false;

        public LogfileUpdate(string folderPath, DataAccess logData) {
            FolderPath = folderPath;
            LogData = logData;

            CSVLogReader = new CSVLogReader(LogData);

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
            try {
                ReadFile[] savedLogs = LogData.Context.ReadFiles.ToArray();
                foreach(var file in new DirectoryInfo(FolderPath).GetFiles("*.csv")) {
                    if(savedLogs.FirstOrDefault(rf => rf.FileName == file.Name) == null) {
                        Log?.Invoke($"File '{file.Name}' is not yet in DB");
                        var exception = CSVLogReader.AddCSVToDB(file.FullName);
                        if(exception == null) {
                            LogData.AddReadFile(new ReadFile() { FileName = file.Name, ReadTime = DateTime.Now });
                            Log?.Invoke($"Successfully added '{file.Name}' to DB", ConsoleColor.White, ConsoleColor.DarkGreen);
                        } else {
                            Log?.Invoke($"Something went wrong while adding '{file.Name}' to DB:\n{exception}", ConsoleColor.Red);
                        }
                    }
                }
            } catch(Exception e) {
                Log?.Invoke($"Something went wrong in 'AddFolderFiles()':\n{e}", ConsoleColor.Red);
            }
        }
    }
}