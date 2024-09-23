﻿using SimpleAntivirus.AntiTampering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAntivirus.GUI.Services
{
    /// <summary>
    /// Mainly helpful for Anti-Tampering, set up of keys and what not, but will provide any setup routines that need to be done when
    /// running the program <para/>
    /// Has operations for first run, and run every time.
    /// <para/> Provides error messages if things are missing, ideal for when we decide to publish the software.
    /// </summary>
    public class SetupService
    {

        public bool _firstSetup;
        public SetupService()
        {
            _firstSetup = false;
        }

        private void ErrorMessage(string problem)
        {
            System.Windows.MessageBox.Show(problem, "Operation Failure", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
        }


        // Create a path that represents a new dir or file
        private string CreateFilePathInProjectDirectory(string nameOrDir)
        {
            return (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameOrDir));
        }

        // Create a path that represents a new dir or file
        private string CreateFilePathProgramDataDirectory(string nameOrDir)
        {
            return (Path.Combine("C:\\ProgramData\\SimpleAntiVirus", nameOrDir));
        }



        public async Task<bool> Run()
        {
            System.Diagnostics.Debug.WriteLine("Startup Routine\n\n");
            string configPath = CreateFilePathProgramDataDirectory("config.enc");
            if (Path.Exists(configPath))
            {
                // Decrypt via set keys.

                // Decrypt config file which is expected to have line "firstRun=1"
                byte[] configInfo = EncryptionHandler.DecryptIntoMemory(configPath, EncryptionHandler.DecryptionKeyStorage(), EncryptionHandler.DecryptionIVStorage());

                string fileInfo = new(UTF8Encoding.UTF8.GetChars(configInfo));

                string[] lines = fileInfo.Split("\n");

                if (lines[1].Contains("firstRun=1"))
                {
                    _firstSetup = false;
                    GeneralRun();
                }
                else
                { // If we decrypt the config file and don't have this string, there is an issue.
                    ErrorMessage("Configuration file has been tampered with and cannot be read, reinstall program.");
                }
            }
            else
            {
                await FirstRun();
            }
            return true;
        }


        /// <summary>
        /// We are assuming, on the first run that the current items will already exist: <para/>
        /// * Database Folder <para/>
        /// * Hash Database <para/>
        /// * Malicious Commands Database <para/>
        /// This function is to create <para/>
        /// Config File, Key for database, key for encryption
        /// </summary>
        /// <returns></returns>
        public async Task<bool> FirstRun()
        {
            _firstSetup = true;
            string[] CreateFolders =
            {
                            @"C:\ProgramData\SimpleAntiVirus\EncryptionKey",
                            @"C:\ProgramData\SimpleAntiVirus\DatabaseKey"
                        };

            foreach (string folders in CreateFolders)
            {
                if (Directory.Exists(folders))
                {
                    Directory.Delete(folders, true);
                }
            }

            foreach (string folders in CreateFolders)
            {
                try
                {
                    Directory.CreateDirectory(folders);
                    Console.WriteLine($"Created Encryption storage: {folders}");
                }
                catch (Exception errormsg)
                {
                    Console.WriteLine($"Unexpected error creating: {folders}. \nDescription: {errormsg.Message}");
                }
            }

            string configPath = CreateFilePathProgramDataDirectory("config");
            if (!Path.Exists(configPath))
            {
                // Setup config.txt file for basic info.
                using (StreamWriter fileS = new(configPath))
                {
                    await fileS.WriteAsync("Config_File_SAV");
                    await fileS.WriteAsync("\nfirstRun=1");
                }
                EncryptionHandler.GenerateEncryptionKey(out byte[] aesKey, out byte[] aesIV);
                EncryptionHandler.InitialEncryptFiles(configPath, aesKey, aesIV);
                EncryptionHandler.EncryptionKeyStorage(aesKey);
                EncryptionHandler.EncryptionIVStorage(aesIV);
            }
            return true;
        }

        /// <summary>
        /// Ran everytime the program is started up
        /// </summary>
        /// <returns>True - Program cannot detect any issues <para/> False - Program files are missing / tampering detected</returns>
        public bool GeneralRun()
        {
            // On first run
            if (!Path.Exists(CreateFilePathInProjectDirectory("Databases")))
            {
                ErrorMessage("Databases folder does not exist!");
                return false;
            }
            //if (!Path.Exists(CreateFilePathInProjectDirectory("Databases\IntegrityDatabase.db"));
            return true;
        }

        public bool FirstTimeRunning
        {
            get
            {
                return _firstSetup;
            }
        }
    }
}
