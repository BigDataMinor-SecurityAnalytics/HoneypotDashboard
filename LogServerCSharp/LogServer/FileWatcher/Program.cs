using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher {

    internal class Program {

        private static void Main(string[] args) {
            Console.WriteLine("Startin connection...");
            var access = new DataAccess();
            foreach(var file in access.GetReadFiles()) {
                Console.WriteLine($"{file.ID,-4} | {file.FileName,-10} | {file.ReadTime,-10}");
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}