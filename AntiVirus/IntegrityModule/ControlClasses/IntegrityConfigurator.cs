/**************************************************************************
 * File:        IntegrityConfigurator.cs
 * Author:      Christopher Thompson, etc.
 * Description: A simplified interface for the database.
 * Last Modified: 26/08/2024
 **************************************************************************/

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
        private int _displaySet;
        public IntegrityConfigurator(IntegrityDatabaseIntermediary integrityDatabase)
        {
            _database = integrityDatabase;
            _displaySet = 10;
        }

        /// <summary>
        /// Add windows directory / file to Integrity Database
        /// </summary>
        /// <param name="path">Windows directory.</param>
        /// <param name="debug">Whether console information is shown</param>
        /// <returns></returns>
        public async Task<bool> AddIntegrityDirectory(string path, bool debug = false)
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
            bool returnItem = await _database.AddEntry(path, amountPerSet);
            if (debug)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"AmountPerSet: {amountPerSet}");
                Console.WriteLine($"Directory adding process duration: {timer.Elapsed}");
                Console.ResetColor();
                if (returnItem == false)
                {
                    Console.WriteLine("Duration timing invalid, as addition to database was cancelled");
                }
                timer.Stop();
            }
            return returnItem;
        }

        /// <summary>
        /// Remove a certain path (item or folder) from IntegrityDatabase
        /// </summary>
        /// <param name="path">Windows directory</param>
        /// <returns></returns>
        public bool RemoveIntegrityDirectory(string path)
        {
            return _database.RemoveEntry(path);
        }

        public Dictionary<string, string> GetPage(int page)
        {
            return _database.GetSetEntries(page, _displaySet);
        }

        public int GetPageAmount()
        {
            return  Convert.ToInt32((_database.QueryAmount() / _displaySet));
        }

        /// <summary>
        /// Remove all items within IntegrityTrack table.
        /// </summary>
        /// <returns></returns>
        public bool RemoveAll()
        {
            return _database.DeleteAll();
        }
    }
}
