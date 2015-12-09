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

        private const string FileExtension = ".log";
        private string FileFilter => $"*{FileExtension}";

        private readonly DataAccess LogData;
        private readonly string FolderPath;
        private readonly LogReader LogReader;

        private bool Started = false;
        private long lastAddCheck = Environment.TickCount;

        public LogfileUpdate(string folderPath, DataAccess logData) {
            FolderPath = folderPath;
            LogData = logData;

            LogReader = new LogReader(LogData);

            AddFolderFiles();
        }

        public void Init() {
            if(!Started) {
                var watcher = new FileSystemWatcher() {
                    Path = FolderPath,
                    IncludeSubdirectories = false,
                    Filter = FileFilter,
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };
                watcher.Changed += FolderChanged;
                Log?.Invoke("Folder watcher has been set up", ConsoleColor.Green);
            }
        }

        private void FolderChanged(object sender, FileSystemEventArgs e) {
            if(Environment.TickCount - lastAddCheck > 1000) {
                Log?.Invoke("---", ConsoleColor.DarkBlue);
                AddFolderFiles();
                lastAddCheck = Environment.TickCount;
            }
        }

        private void AddFolderFiles() {
            try {
                ReadFile[] savedLogs = LogData.Context.ReadFiles.ToArray();
                foreach(var file in new DirectoryInfo(FolderPath).GetFiles(FileFilter).Where(f => f.Extension == FileExtension)) {
                    var knownFile = savedLogs.FirstOrDefault(rf => rf.FileName == file.Name);

                    //Add to DB Needed
                    if(knownFile == null || knownFile.LastWrite < file.LastWriteTimeUtc) {
                        Log?.Invoke($"DB is not up-to-date with file: '{file.Name}'");
                        var exception = LogReader.AddLogToDB(file.FullName);
                        if(exception == null) {
                            if(knownFile != null) {
                                knownFile.LastWrite = file.LastWriteTimeUtc;
                                LogData.SaveChanges();
                            } else {
                                LogData.AddReadFile(new ReadFile() { FileName = file.Name, LastWrite = file.LastWriteTimeUtc });
                            }
                            Log?.Invoke($"Successfully updated DB with '{file.Name}'", ConsoleColor.White, ConsoleColor.DarkGreen);
                        } else {
                            Log?.Invoke($"Something went wrong while syncing '{file.Name}' with DB:\n{exception}", ConsoleColor.Red);
                        }
                    } else {
                        Log?.Invoke($"No DB sync necessary for {file.Name}");
                    }
                }
            } catch(Exception e) {
                Log?.Invoke($"Something went wrong in 'AddFolderFiles()':\n{e}", ConsoleColor.Red);
            }
        }
    }
}