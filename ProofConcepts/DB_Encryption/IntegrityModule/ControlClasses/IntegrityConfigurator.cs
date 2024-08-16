using DatabaseFoundations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.ControlClasses
{
    public class IntegrityConfigurator
    {
        private IntegrityDatabaseIntermediary _database;
        public IntegrityConfigurator(IntegrityDatabaseIntermediary integrityDatabase)
        {
            _database = integrityDatabase;
        }

        public bool AddIntegrityDirectory(string path, bool debug = false)
        {
            // This parameter affects adding baseline performance, especially for adding huge folders.
            // Higher the value, slower it is.
            // Lesser the value, more parallel processing occurs.
            // Amount that is separated for each thread.
            int amountPerSet = 100;

            Stopwatch timer = new();
            if (debug)
            {
                timer.Start();
            }
            bool returnItem = _database.AddEntry(path, amountPerSet);
            if (debug)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"AmountPerSet: {amountPerSet}");
                Console.WriteLine($"Directory adding process duration: {timer.Elapsed}");
                Console.ResetColor();
                timer.Stop();
            }
            return returnItem;
        }

        public bool RemoveIntegrityDirectory(string path)
        {
            return _database.RemoveEntry(path);
        }

        public bool RemoveAll()
        {
            return _database.DeleteAll();
        }
    }
}
