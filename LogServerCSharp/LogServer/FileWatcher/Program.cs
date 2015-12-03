﻿using Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace FileWatcher {

    internal class Program {
        private static DataAccess Data;
        private const string ConfigLogFolderKey = "LogFilesFolder";

        private static void Main(string[] args) {
            Log($"Started Log-Watcher @ {DateTime.Now}");

            var settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;
            string logFolderPath = string.Empty;
            if(settings.AllKeys.Contains(ConfigLogFolderKey) && Directory.Exists(logFolderPath = settings[ConfigLogFolderKey].Value)) {
                Log($"Log folder set to: '{logFolderPath}'", ConsoleColor.Green);
                Data = new DataAccess();

                var lfu = new LogfileUpdate(logFolderPath, Data);
                lfu.Log += Log;
                lfu.Init();
            } else {
                Log($"Config: '{ConfigLogFolderKey}' does not exist, or is not valid.", ConsoleColor.Red);
            }

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
                    Data.Clear();
                    Log("Cleared DB");
                    break;
                    case ConsoleKey.Q:
                    running = false;
                    Log("Quiting...");
                    break;
                }
            }
        }

        private static void PrintHelp() {
            Console.WriteLine("\nHelp - Press:");
            Console.WriteLine("H: Help");
            Console.WriteLine("C: Clear DB");
            Console.WriteLine("Q: Quit");
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