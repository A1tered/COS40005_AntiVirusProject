/**************************************************************************
* File:        [Program].cs
* Author:      [Pawan]
* Description: [Startup of the application]
* Last Modified: [17/09/2024]
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System;
using System.IO;
using System.Threading.Tasks;

namespace MaliciousCode
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Define the path to the SQLite database
            string dbPath = Path.Combine(AppContext.BaseDirectory, "malicious_commands.db");

            // Initialize the DatabaseHandler with the SQLite database path
            DatabaseHandler dbHandler = new DatabaseHandler(dbPath);

            // Initialize the Detector
            Detector detector = new Detector(dbHandler);

            // Initialize the Scanner
            Scanner scanner = new Scanner(dbHandler, detector);

            // Ask for directory path to scan
            Console.WriteLine("Enter the directory path to scan: ");
            string directoryToScan = Console.ReadLine();

            // Start scanning the directory
            await scanner.ScanDirectoryAsync(directoryToScan);
        }
    }
}
