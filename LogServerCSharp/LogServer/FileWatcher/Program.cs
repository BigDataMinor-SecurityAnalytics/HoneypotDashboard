using Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace FileWatcher {

    internal class Program {
        private static DataAccess DataObj;
        private const string ConfigLogFolderKey = "LogFilesFolder";

        private static void Main(string[] args) {
#if !DEBUG
            try {
#endif
            Log($"Started Log-Watcher @ {DateTime.Now}");
            var settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;
            string logFolderPath = string.Empty;
            if(settings.AllKeys.Contains(ConfigLogFolderKey) && Directory.Exists(logFolderPath = settings[ConfigLogFolderKey].Value)) {
                Log($"Log folder set to: '{logFolderPath}'");
                DataObj = new DataAccess();

                var lfu = new LogfileUpdate(logFolderPath, DataObj);
                lfu.Log += Log;
                lfu.Init();
            } else {
                Log($"Config: '{ConfigLogFolderKey}' does not exist, or is not valid.", ConsoleColor.Red);
            }

            IPLocate.Init();
            Log($"Read IP locations");

            foreach(var entry in DataObj.Context.LogEntries.Where(le => le.Country == null)) {
                var calced = IPLocate.GetIpCountry(entry.RemoteIp);
                entry.Country = calced;
            }
            DataObj.SaveChanges();

            PrintHelp();
            bool running = true;
            while(running) {
                var key = Console.ReadKey().Key;
                Console.WriteLine();
                switch(key) {
                    case ConsoleKey.H:
                    PrintHelp();
                    break;
                    case ConsoleKey.C:
                    DataObj.Clear();
                    Log("Cleared DB");
                    break;
                    case ConsoleKey.S:
                    PrintStats();
                    break;
                    case ConsoleKey.Q:
                    running = false;
                    Log("Quiting...");
                    break;
                }
            }
#if !DEBUG
        } catch(Exception e) {
                Log($"Unable to handle exception:\n{e}", ConsoleColor.Red);
                Console.ReadKey();
                Environment.Exit(-1);
            }
#endif
        }

        private static void PrintHelp() {
            Console.WriteLine("\nHelp - Press:");
            Console.WriteLine("H: Help");
            Console.WriteLine("C: Clear DB");
            Console.WriteLine("S: Stats");
            Console.WriteLine("Q: Quit");
        }

        private static void PrintStats() {
            var logCount = DataObj.Context.LogEntries.Count();
            Console.WriteLine("\nStats:");
            Console.WriteLine($"ReadFiles: {DataObj.Context.ReadFiles.Count()}\nLogEntries: {logCount}\n");

            if(logCount > 0) {
                var r = new Random().Next(0, logCount - 1);
                var entry = DataObj.Context.LogEntries.OrderBy(le => le.ID).Skip(r).First();
                Console.WriteLine($"Random (#{r}) entry:");
                foreach(var prop in typeof(Data.Models.LogEntry).GetProperties()) {
                    var val = prop.GetValue(entry).ToString();
                    if(val.Length > 30)
                        val = val.Substring(0, 29);
                    Console.WriteLine($"{prop.Name}: {val}");
                }
            }
        }

        private static void Log(string msg, ConsoleColor? ForeColor = null, ConsoleColor? BackColor = null) {
            var startFore = Console.ForegroundColor;
            var startBack = Console.BackgroundColor;

            Console.ForegroundColor = ForeColor ?? startFore;
            Console.BackgroundColor = BackColor ?? startBack;

            Console.WriteLine(msg);

            Console.ForegroundColor = startFore;
            Console.BackgroundColor = startBack;
        }
    }
}