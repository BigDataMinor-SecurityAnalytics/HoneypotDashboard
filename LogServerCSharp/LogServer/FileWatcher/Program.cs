using Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace FileWatcher {

    internal class Program {
        private static DataAccess DataAcc;
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
                DataAcc = new DataAccess();

                var lfu = new LogfileUpdate(logFolderPath, DataAcc);
                lfu.Log += Log;
                lfu.Init();
            } else {
                Log($"Config: '{ConfigLogFolderKey}' does not exist, or is not valid.", ConsoleColor.Red);
            }

            IPLocate.Init();
            Log($"Read IP locations");

            foreach(var entry in DataAcc.Context.LogEntries.Where(le => le.Country == null)) {
                var calced = IPLocate.GetIpCountry(entry.RemoteIp);
                entry.Country = calced;
            }
            DataAcc.SaveChanges();

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
                    DataAcc.Clear();
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
            Console.WriteLine("\nStats:");
            Console.WriteLine($"ReadFiles: {DataAcc.Context.ReadFiles.Count()}\nLogEntries: {DataAcc.Context.LogEntries.Count()}\n");
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