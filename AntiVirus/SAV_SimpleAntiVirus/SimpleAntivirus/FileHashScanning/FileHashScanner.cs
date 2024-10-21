/**************************************************************************
* File:        FileHashScanner
* Author:      Christopher Thompson & Joel Parks
* Description: The main program file for the file hash scanner.
* Last Modified: 13/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System.Diagnostics;
using System.IO;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.FileQuarantine;


/// <summary>
/// The File Hash Scanner is used to run scans comparing hashes of files in the given scanning directories and searching the hash database for matches.
/// If a match is found, it is marked as a threat and the file is quarantined and an alert is raised.
/// </summary>
namespace SimpleAntivirus.FileHashScanning
{
    public class FileHashScanner
    {
        public AlertManager AlertManager;
        public EventBus EventBus;
        public CancellationToken Token;
        public QuarantineManager QuarantineManager;

        public FileHashScanner(AlertManager alertManager, EventBus eventBus, CancellationToken token, QuarantineManager quarantineManager)
        {
            EventBus = eventBus;
            AlertManager = alertManager;
            Token = token;
            QuarantineManager = quarantineManager;
        }

        static DirectoryManager directoryManager = new DirectoryManager();
        // Get directory to database.
        static string databaseDirectory => directoryManager.getDatabaseDirectory("sighash.db");

        public async Task Scan(string scanType, List<string> customScanDirs)
        {
            await Task.Run(async () =>
            {
                List<string> directories = new List<string>();

                if (scanType == "quick")
                {
                    /* Directories chosen by doing a Google search on common quick scan locations
                    * Most information regarding this topic is not public, for obvious security reasons, as antivirus companies do not wish for this information
                    * to be available to attackers.
                    * Common locations include: Scanning contents of active memory, program files, system files and startup items.
                    * Memory scanning is out of scope for this project given the limited time constraints. 
                    * Hence, Program Files, System files (The Windows directory) and the Startup directory are being scanned
                    * A paper I found regarding this topic can be found here:
                    * https://www.researchgate.net/profile/Oemer-Aslan-5/publication/321759536_Performance_Comparison_of_Static_Malware_Analysis_Tools_Versus_Antivirus_Scanners_To_Detect_Malware/links/5a30d86c0f7e9b0d50f905c3/Performance-Comparison-of-Static-Malware-Analysis-Tools-Versus-Antivirus-Scanners-To-Detect-Malware.pdf
                    */
                    directories.AddRange
                    ([
                     $"C:\\Program Files",
                     "C:\\Program Files (x86)",
                     "C:\\Windows",
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Startup")
                    ]);
                }
                else if (scanType == "full")
                {
                    string[] drives = Environment.GetLogicalDrives();
                    foreach (string drive in drives)
                    {
                        directories.Add(drive);
                    }
                }
                else if (scanType == "custom")
                {
                    if (customScanDirs != null && customScanDirs.Count > 0)
                    {
                        foreach (string dir in customScanDirs)
                        {
                            Debug.WriteLine($"Currently added dir: {dir}");
                            directories.Add(dir);
                        }
                    }
                }

                foreach (string directorySearch in directories)
                {
                    Token.ThrowIfCancellationRequested();
                    SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory, this, Token);
                    await splitprocessInstance.FillUpSearch(directorySearch);
                    await splitprocessInstance.SearchDirectory();
                }
            });
        }
    }
}